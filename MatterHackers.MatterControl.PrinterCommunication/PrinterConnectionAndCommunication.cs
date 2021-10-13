using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Gaming.Game;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.ConfigurationPage.PrintLeveling;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.FrostedSerial;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.PrinterCommunication.Io;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.SerialPortCommunication;
using MatterHackers.VectorMath;
using Microsoft.Win32.SafeHandles;

namespace MatterHackers.MatterControl.PrinterCommunication
{
	public class PrinterConnectionAndCommunication
	{
		[Flags]
		public enum Axis
		{
			X = 0x1,
			Y = 0x2,
			Z = 0x4,
			E = 0x8,
			XY = 0x3,
			XYZ = 0x7
		}

		public enum CommunicationStates
		{
			Disconnected,
			AttemptingToConnect,
			FailedToConnect,
			Connected,
			PreparingToPrint,
			Printing,
			PrintingFromSd,
			Paused,
			FinishedPrint,
			Disconnecting,
			ConnectionLost
		}

		public enum DetailedPrintingState
		{
			HomingAxis,
			HeatingBed,
			HeatingExtruder,
			Printing
		}

		public enum FirmwareTypes
		{
			Unknown,
			Repetier,
			Marlin,
			Sprinter
		}

		public class ReadThread
		{
			private static int currentReadThreadIndex;

			private int creationIndex;

			private static int numRunning;

			public static int NumRunning => numRunning;

			private ReadThread()
			{
				numRunning++;
				currentReadThreadIndex++;
				creationIndex = currentReadThreadIndex;
				Task.Run(delegate
				{
					//IL_0020: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Expected O, but got Unknown
					try
					{
						Instance.ReadFromPrinter(this);
					}
					catch
					{
					}
					Instance.CommunicationUnconditionalToPrinter.CallEvents((object)this, (EventArgs)new StringEventArgs("Read Thread Has Exited.\n"));
					numRunning--;
				});
			}

			internal static void Join()
			{
				currentReadThreadIndex++;
			}

			internal static void Start()
			{
				new ReadThread();
			}

			internal bool IsCurrentThread()
			{
				return currentReadThreadIndex == creationIndex;
			}
		}

		private class CheckSumLines
		{
			private static readonly int RingBufferCount = 64;

			private int addedCount;

			private string[] ringBuffer = new string[RingBufferCount];

			public int Count => addedCount;

			public string this[int index]
			{
				get
				{
					return ringBuffer[index % RingBufferCount];
				}
				set
				{
					ringBuffer[index % RingBufferCount] = value;
				}
			}

			internal void Add(string lineWithChecksum)
			{
				this[addedCount++] = lineWithChecksum;
			}

			internal void SetStartingIndex(int startingIndex)
			{
				addedCount = startingIndex;
			}
		}

		public RootedObjectEventHandler ActivePrintItemChanged = new RootedObjectEventHandler();

		public RootedObjectEventHandler BedTemperatureRead = new RootedObjectEventHandler();

		public RootedObjectEventHandler BedTemperatureSet = new RootedObjectEventHandler();

		public RootedObjectEventHandler CommunicationStateChanged = new RootedObjectEventHandler();

		public RootedObjectEventHandler CommunicationUnconditionalFromPrinter = new RootedObjectEventHandler();

		public RootedObjectEventHandler CommunicationUnconditionalToPrinter = new RootedObjectEventHandler();

		public RootedObjectEventHandler ConnectionFailed = new RootedObjectEventHandler();

		public RootedObjectEventHandler ConnectionSucceeded = new RootedObjectEventHandler();

		public RootedObjectEventHandler DestinationChanged = new RootedObjectEventHandler();

		public RootedObjectEventHandler EnableChanged = new RootedObjectEventHandler();

		public RootedObjectEventHandler ExtruderTemperatureRead = new RootedObjectEventHandler();

		public RootedObjectEventHandler ExtruderTemperatureSet = new RootedObjectEventHandler();

		public RootedObjectEventHandler FanSpeedSet = new RootedObjectEventHandler();

		public RootedObjectEventHandler FirmwareVersionRead = new RootedObjectEventHandler();

		public RootedObjectEventHandler PositionRead = new RootedObjectEventHandler();

		public RootedObjectEventHandler PrintFinished = new RootedObjectEventHandler();

		public RootedObjectEventHandler PauseOnLayer = new RootedObjectEventHandler();

		public RootedObjectEventHandler FilamentRunout = new RootedObjectEventHandler();

		public RootedObjectEventHandler PrintingStateChanged = new RootedObjectEventHandler();

		public RootedObjectEventHandler ReadLine = new RootedObjectEventHandler();

		public RootedObjectEventHandler WroteLine = new RootedObjectEventHandler();

		public RootedObjectEventHandler AtxPowerStateChanged = new RootedObjectEventHandler();

		private bool atxPowerIsOn;

		internal const int MAX_EXTRUDERS = 16;

		private const int MAX_INVALID_CONNECTION_CHARS = 3;

		private static PrinterConnectionAndCommunication globalInstance;

		private object locker = new object();

		private readonly int JoinThreadTimeoutMs = 5000;

		private PrintItemWrapper activePrintItem;

		private PrintTask activePrintTask;

		private double actualBedTemperature;

		private int currentlyActiveExtruderIndex;

		private double[] actualExtruderTemperature = new double[16];

		private CheckSumLines allCheckSumLinesSent = new CheckSumLines();

		private int backupAmount;

		private CommunicationStates communicationState;

		private string connectionFailureMessage = "Unknown Reason";

		private Thread connectThread;

		private PrinterMove currentDestination;

		private double currentSdBytes;

		private string doNotAskAgainMessage = "Don't remind me again".Localize();

		private PrinterMachineInstruction.MovementTypes extruderMode;

		private int fanSpeed;

		private bool firmwareUriGcodeSend;

		private int currentLineIndexToSend;

		private bool ForceImmediateWrites;

		private string gcodeWarningMessage = "The file you are attempting to print is a GCode file.\n\nIt is recommended that you only print Gcode files known to match your printer's configuration.\n\nAre you sure you want to print this GCode file?".Localize();

		private string itemNotFoundMessage = "Item not found".Localize();

		private string lastLineRead = "";

		private PrinterMove lastReportedPosition;

		private DataViewGraph sendTimeAfterOkGraph;

		private GCodeFile loadedGCode = new GCodeFileLoaded();

		private GCodeFileStream gCodeFileStream0;

		private PauseHandlingStream pauseHandlingStream1;

		private QueuedCommandsStream queuedCommandStream2;

		private RelativeToAbsoluteStream relativeToAbsoluteStream3;

		private PrintLevelingStream printLevelingStream4;

		private WaitForTempStream waitForTempStream5;

		private BabyStepsStream babyStepsStream6;

		private ExtrusionMultiplyerStream extrusionMultiplyerStream7;

		private FeedRateMultiplyerStream feedrateMultiplyerStream8;

		private RequestTemperaturesStream requestTemperaturesStream9;

		private ProcessWriteRegexStream processWriteRegExStream10;

		private GCodeStream totalGCodeStream;

		private PrinterMachineInstruction.MovementTypes movementMode;

		private DetailedPrintingState printingStatePrivate;

		private FoundStringContainsCallbacks ReadLineContainsCallBacks = new FoundStringContainsCallbacks();

		private FoundStringStartsWithCallbacks ReadLineStartCallBacks = new FoundStringStartsWithCallbacks();

		private string removeFromQueueMessage = "Cannot find this file\nWould you like to remove it from the queue?".Localize();

		private IFrostedSerialPort serialPort;

		private bool stopTryingToConnect;

		private double targetBedTemperature;

		private double[] targetExtruderTemperature = new double[16];

		private Stopwatch timeHaveBeenWaitingForOK = new Stopwatch();

		private Stopwatch timeSinceLastReadAnything = new Stopwatch();

		private Stopwatch timeSinceLastWrite = new Stopwatch();

		private Stopwatch timeSinceRecievedOk = new Stopwatch();

		private Stopwatch timeSinceStartedPrint = new Stopwatch();

		private Stopwatch timeWaitingForSdProgress = new Stopwatch();

		private double totalSdBytes;

		private Stopwatch waitingForPosition = new Stopwatch();

		private FoundStringContainsCallbacks WriteLineContainsCallBacks = new FoundStringContainsCallbacks();

		private FoundStringStartsWithCallbacks WriteLineStartCallBacks = new FoundStringStartsWithCallbacks();

		private double secondsSinceUpdateHistory;

		private EventHandler unregisterEvents;

		private double feedRateRatio = 1.0;

		private bool haveReportedError;

		private Regex getQuotedParts = new Regex("([\"'])(\\\\?.)*?\\1", (RegexOptions)8);

		private string read_regex = "";

		private List<RegReplace> ReadLineReplacements = new List<RegReplace>();

		private string currentSentLine;

		private string previousSentLine;

		private bool haveHookedDrawing;

		public bool WatingForPositionRead
		{
			get
			{
				if (waitingForPosition.get_ElapsedMilliseconds() > 60000)
				{
					waitingForPosition.Reset();
					PositionReadQueued = false;
				}
				if (!waitingForPosition.get_IsRunning())
				{
					return PositionReadQueued;
				}
				return true;
			}
		}

		public double CurrentExtruderDestination => currentDestination.extrusion;

		public double CurrentFeedRate => currentDestination.feedRate;

		public CommunicationStates PrePauseCommunicationState
		{
			get;
			private set;
		} = CommunicationStates.Printing;


		private bool PositionReadQueued
		{
			get;
			set;
		}

		public static PrinterConnectionAndCommunication Instance
		{
			get
			{
				if (globalInstance == null)
				{
					globalInstance = new PrinterConnectionAndCommunication();
				}
				return globalInstance;
			}
		}

		public PrintItemWrapper ActivePrintItem
		{
			get
			{
				return activePrintItem;
			}
			set
			{
				if (!PrinterIsPrinting)
				{
					if (activePrintItem != value)
					{
						activePrintItem = value;
						if (CommunicationState == CommunicationStates.FinishedPrint)
						{
							CommunicationState = CommunicationStates.Connected;
						}
						OnActivePrintItemChanged(null);
					}
					return;
				}
				throw new Exception("Cannot change active print while printing");
			}
		}

		public double ActualBedTemperature => actualBedTemperature;

		public int BaudRate
		{
			get
			{
				int result = 250000;
				if (ActivePrinter != null)
				{
					try
					{
						if (string.IsNullOrEmpty(ActiveSliceSettings.Instance.GetValue("baud_rate")))
						{
							return result;
						}
						result = Convert.ToInt32(ActiveSliceSettings.Instance.GetValue("baud_rate"));
						return result;
					}
					catch
					{
						return result;
					}
				}
				return result;
			}
		}

		public CommunicationStates CommunicationState
		{
			get
			{
				return communicationState;
			}
			set
			{
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Expected O, but got Unknown
				switch (value)
				{
				case CommunicationStates.Connected:
					SendLineToPrinterNow("M115");
					ReadPosition();
					break;
				case CommunicationStates.Disconnected:
				case CommunicationStates.ConnectionLost:
				{
					TurnOffBedAndExtruders();
					for (int i = 0; i < 16; i++)
					{
						actualExtruderTemperature[i] = 0.0;
						OnExtruderTemperatureRead(new TemperatureEventArgs(i, GetActualExtruderTemperature(i)));
					}
					actualBedTemperature = 0.0;
					OnBedTemperatureRead(new TemperatureEventArgs(0, ActualBedTemperature));
					break;
				}
				}
				if (communicationState == value)
				{
					return;
				}
				CommunicationUnconditionalToPrinter.CallEvents((object)this, (EventArgs)new StringEventArgs(StringHelper.FormatWith("Communication State: {0}\n", new object[1]
				{
					value.ToString()
				})));
				switch (communicationState)
				{
				case CommunicationStates.Printing:
				case CommunicationStates.PrintingFromSd:
					switch (value)
					{
					case CommunicationStates.Paused:
						if (communicationState == CommunicationStates.Printing)
						{
							PrePauseCommunicationState = CommunicationStates.Printing;
						}
						else
						{
							PrePauseCommunicationState = CommunicationStates.PrintingFromSd;
						}
						timeSinceStartedPrint.Stop();
						break;
					case CommunicationStates.FinishedPrint:
						if (activePrintTask != null)
						{
							DateTime.Now.Subtract(activePrintTask.PrintStart);
							activePrintTask.PrintEnd = DateTime.Now;
							activePrintTask.PercentDone = 100.0;
							activePrintTask.PrintComplete = true;
							activePrintTask.Commit();
						}
						communicationState = value;
						timeSinceStartedPrint.Stop();
						OnPrintFinished(null);
						break;
					default:
						timeSinceStartedPrint.Stop();
						timeSinceStartedPrint.Reset();
						break;
					}
					break;
				case CommunicationStates.Paused:
					if (value == CommunicationStates.Printing)
					{
						timeSinceStartedPrint.Start();
					}
					break;
				default:
					if (!timeSinceStartedPrint.get_IsRunning() && value == CommunicationStates.Printing)
					{
						timeSinceStartedPrint.Restart();
					}
					break;
				}
				communicationState = value;
				OnCommunicationStateChanged(null);
			}
		}

		public string ComPort => ActiveSliceSettings.Instance?.Helpers.ComPort();

		public string DriverType => ActiveSliceSettings.Instance?.GetValue("driver_type");

		public bool AtxPowerEnabled
		{
			get
			{
				return atxPowerIsOn;
			}
			set
			{
				if (value)
				{
					SendLineToPrinterNow("M80");
				}
				else
				{
					SendLineToPrinterNow("M81");
				}
			}
		}

		public string ConnectionFailureMessage => connectionFailureMessage;

		public Vector3 CurrentDestination => currentDestination.position;

		public int CurrentlyPrintingLayer
		{
			get
			{
				if (gCodeFileStream0 != null)
				{
					int instructionIndex = gCodeFileStream0.LineIndex - backupAmount;
					return loadedGCode.GetLayerIndex(instructionIndex);
				}
				return 0;
			}
		}

		public string DeviceCode
		{
			get;
			private set;
		}

		public bool Disconnecting => CommunicationState == CommunicationStates.Disconnecting;

		public int FanSpeed0To255
		{
			get
			{
				return fanSpeed;
			}
			set
			{
				fanSpeed = Math.Max(0, Math.Min(255, value));
				OnFanSpeedSet(null);
				if (PrinterIsConnected)
				{
					SendLineToPrinterNow(StringHelper.FormatWith("M106 S{0}", new object[1]
					{
						fanSpeed
					}));
				}
			}
		}

		public FirmwareTypes FirmwareType
		{
			get;
			private set;
		}

		public string FirmwareVersion
		{
			get;
			private set;
		}

		public bool HasHomedSinceConnect
		{
			get;
			private set;
		}

		public Vector3 LastReportedPosition => lastReportedPosition.position;

		public bool MonitorPrinterTemperature
		{
			get;
			set;
		}

		public double PercentComplete
		{
			get
			{
				if (CommunicationState == CommunicationStates.PrintingFromSd || (communicationState == CommunicationStates.Paused && PrePauseCommunicationState == CommunicationStates.PrintingFromSd))
				{
					if (totalSdBytes > 0.0)
					{
						return currentSdBytes / totalSdBytes * 100.0;
					}
					return 0.0;
				}
				if (PrintIsFinished && !PrinterIsPaused)
				{
					return 100.0;
				}
				if (NumberOfLinesInCurrentPrint > 0 && loadedGCode != null && gCodeFileStream0 != null)
				{
					return loadedGCode.PercentComplete(gCodeFileStream0.LineIndex);
				}
				return 0.0;
			}
		}

		public string PrinterConnectionStatusVerbose => CommunicationState switch
		{
			CommunicationStates.Disconnected => "Not Connected".Localize(), 
			CommunicationStates.Disconnecting => "Disconnecting".Localize(), 
			CommunicationStates.AttemptingToConnect => "Connecting".Localize() + "...", 
			CommunicationStates.ConnectionLost => "Connection Lost".Localize(), 
			CommunicationStates.FailedToConnect => "Unable to Connect".Localize(), 
			CommunicationStates.Connected => "Connected".Localize(), 
			CommunicationStates.PreparingToPrint => "Preparing To Print".Localize(), 
			CommunicationStates.Printing => "Printing".Localize(), 
			CommunicationStates.PrintingFromSd => "Printing From SD Card".Localize(), 
			CommunicationStates.Paused => "Paused".Localize(), 
			CommunicationStates.FinishedPrint => "Finished Print".Localize(), 
			_ => throw new NotImplementedException("Make sure every status returns the correct connected state."), 
		};

		public bool PrinterIsConnected
		{
			get
			{
				switch (CommunicationState)
				{
				case CommunicationStates.Disconnected:
				case CommunicationStates.AttemptingToConnect:
				case CommunicationStates.FailedToConnect:
				case CommunicationStates.ConnectionLost:
					return false;
				case CommunicationStates.Connected:
				case CommunicationStates.PreparingToPrint:
				case CommunicationStates.Printing:
				case CommunicationStates.PrintingFromSd:
				case CommunicationStates.Paused:
				case CommunicationStates.FinishedPrint:
				case CommunicationStates.Disconnecting:
					return true;
				default:
					throw new NotImplementedException("Make sure every status returns the correct connected state.");
				}
			}
		}

		public bool PrinterIsPaused => CommunicationState == CommunicationStates.Paused;

		public bool PrinterIsPrinting
		{
			get
			{
				switch (CommunicationState)
				{
				case CommunicationStates.Disconnected:
				case CommunicationStates.AttemptingToConnect:
				case CommunicationStates.FailedToConnect:
				case CommunicationStates.Connected:
				case CommunicationStates.PreparingToPrint:
				case CommunicationStates.Paused:
				case CommunicationStates.FinishedPrint:
				case CommunicationStates.Disconnecting:
				case CommunicationStates.ConnectionLost:
					return false;
				case CommunicationStates.Printing:
				case CommunicationStates.PrintingFromSd:
					return true;
				default:
					throw new NotImplementedException("Make sure every status returns the correct connected state.");
				}
			}
		}

		public DetailedPrintingState PrintingState
		{
			get
			{
				return printingStatePrivate;
			}
			set
			{
				if (printingStatePrivate != value)
				{
					printingStatePrivate = value;
					PrintingStateChanged.CallEvents((object)this, (EventArgs)null);
				}
			}
		}

		public string PrintingStateString => PrintingState switch
		{
			DetailedPrintingState.HomingAxis => "Homing Axis".Localize(), 
			DetailedPrintingState.HeatingBed => "Waiting for Bed to Heat to".Localize() + $" {TargetBedTemperature}°", 
			DetailedPrintingState.HeatingExtruder => "Waiting for Extruder to Heat to".Localize() + $" {GetTargetExtruderTemperature(0)}°", 
			DetailedPrintingState.Printing => "Currently Printing".Localize() + ":", 
			_ => "", 
		};

		public bool PrintIsActive
		{
			get
			{
				switch (CommunicationState)
				{
				case CommunicationStates.Disconnected:
				case CommunicationStates.AttemptingToConnect:
				case CommunicationStates.FailedToConnect:
				case CommunicationStates.Connected:
				case CommunicationStates.FinishedPrint:
				case CommunicationStates.Disconnecting:
				case CommunicationStates.ConnectionLost:
					return false;
				case CommunicationStates.PreparingToPrint:
				case CommunicationStates.Printing:
				case CommunicationStates.PrintingFromSd:
				case CommunicationStates.Paused:
					return true;
				default:
					throw new NotImplementedException("Make sure every status returns the correct connected state.");
				}
			}
		}

		public bool PrintIsFinished => CommunicationState == CommunicationStates.FinishedPrint;

		public string PrintJobName
		{
			get;
			private set;
		}

		public bool PrintWasCanceled
		{
			get;
			set;
		}

		public double RatioIntoCurrentLayer
		{
			get
			{
				if (gCodeFileStream0 == null)
				{
					return 0.0;
				}
				int instructionIndex = gCodeFileStream0.LineIndex - backupAmount;
				return loadedGCode.Ratio0to1IntoContainedLayer(instructionIndex);
			}
		}

		public int SecondsPrinted
		{
			get
			{
				if (PrinterIsPrinting || PrinterIsPaused || PrintIsFinished)
				{
					return (int)(timeSinceStartedPrint.get_ElapsedMilliseconds() / 1000);
				}
				return 0;
			}
		}

		public double TargetBedTemperature
		{
			get
			{
				return targetBedTemperature;
			}
			set
			{
				if (targetBedTemperature != value)
				{
					targetBedTemperature = value;
					OnBedTemperatureSet(new TemperatureEventArgs(0, TargetBedTemperature));
					if (PrinterIsConnected)
					{
						SendLineToPrinterNow(StringHelper.FormatWith("M140 S{0}", new object[1]
						{
							targetBedTemperature
						}));
					}
				}
			}
		}

		public int TotalLayersInPrint
		{
			get
			{
				try
				{
					return loadedGCode.NumChangesInZ;
				}
				catch (Exception)
				{
					return -1;
				}
			}
		}

		public int TotalSecondsInPrint
		{
			get
			{
				if (loadedGCode.LineCount > 0)
				{
					if (feedRateRatio != 0.0)
					{
						return (int)(loadedGCode.TotalSecondsInPrint / feedRateRatio);
					}
					return (int)loadedGCode.TotalSecondsInPrint;
				}
				return 0;
			}
		}

		public PrinterSettings ActivePrinter => ActiveSliceSettings.Instance;

		private int NumberOfLinesInCurrentPrint => loadedGCode.LineCount;

		private PrinterConnectionAndCommunication()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Expected O, but got Unknown
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Expected O, but got Unknown
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Expected O, but got Unknown
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Expected O, but got Unknown
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Expected O, but got Unknown
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Expected O, but got Unknown
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Expected O, but got Unknown
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Expected O, but got Unknown
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Expected O, but got Unknown
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Expected O, but got Unknown
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Expected O, but got Unknown
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Expected O, but got Unknown
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Expected O, but got Unknown
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Expected O, but got Unknown
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Expected O, but got Unknown
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Expected O, but got Unknown
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Expected O, but got Unknown
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Expected O, but got Unknown
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Expected O, but got Unknown
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Expected O, but got Unknown
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Expected O, but got Unknown
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Expected O, but got Unknown
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Expected O, but got Unknown
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Expected O, but got Unknown
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Expected O, but got Unknown
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Expected O, but got Unknown
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Expected O, but got Unknown
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Expected O, but got Unknown
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Expected O, but got Unknown
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Expected O, but got Unknown
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Expected O, but got Unknown
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Expected O, but got Unknown
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Expected O, but got Unknown
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Expected O, but got Unknown
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Expected O, but got Unknown
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Expected O, but got Unknown
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Expected O, but got Unknown
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Expected O, but got Unknown
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Expected O, but got Unknown
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Expected O, but got Unknown
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Expected O, but got Unknown
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Expected O, but got Unknown
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Expected O, but got Unknown
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Expected O, but got Unknown
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Expected O, but got Unknown
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Expected O, but got Unknown
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Expected O, but got Unknown
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c0: Expected O, but got Unknown
			//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Expected O, but got Unknown
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Expected O, but got Unknown
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Expected O, but got Unknown
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Expected O, but got Unknown
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_054c: Expected O, but got Unknown
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Expected O, but got Unknown
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0584: Expected O, but got Unknown
			//IL_0596: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a0: Expected O, but got Unknown
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Expected O, but got Unknown
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d8: Expected O, but got Unknown
			//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Expected O, but got Unknown
			//IL_0606: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Expected O, but got Unknown
			//IL_0622: Unknown result type (might be due to invalid IL or missing references)
			//IL_062c: Expected O, but got Unknown
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0648: Expected O, but got Unknown
			//IL_065a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Expected O, but got Unknown
			//IL_0676: Unknown result type (might be due to invalid IL or missing references)
			//IL_0680: Expected O, but got Unknown
			//IL_0692: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Expected O, but got Unknown
			//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b8: Expected O, but got Unknown
			//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d4: Expected O, but got Unknown
			//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f0: Expected O, but got Unknown
			//IL_0702: Unknown result type (might be due to invalid IL or missing references)
			//IL_070c: Expected O, but got Unknown
			//IL_071e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0728: Expected O, but got Unknown
			//IL_073a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0744: Expected O, but got Unknown
			//IL_0756: Unknown result type (might be due to invalid IL or missing references)
			//IL_0760: Expected O, but got Unknown
			//IL_0772: Unknown result type (might be due to invalid IL or missing references)
			//IL_077c: Expected O, but got Unknown
			//IL_078e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0798: Expected O, but got Unknown
			//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b4: Expected O, but got Unknown
			MonitorPrinterTemperature = true;
			_ = StringComparer.OrdinalIgnoreCase;
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("start", new FoundStringEventHandler(FoundStart));
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("start", new FoundStringEventHandler(PrintingCanContinue));
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("ok", new FoundStringEventHandler(SuppressEcho));
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("wait", new FoundStringEventHandler(SuppressEcho));
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("T:", new FoundStringEventHandler(SuppressEcho));
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("ok", new FoundStringEventHandler(PrintingCanContinue));
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("Done saving file", new FoundStringEventHandler(PrintingCanContinue));
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("ok T0:", new FoundStringEventHandler(ReadTemperatures));
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("B:", new FoundStringEventHandler(ReadTemperatures));
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("SD printing byte", new FoundStringEventHandler(ReadSdProgress));
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("C:", new FoundStringEventHandler(ReadTargetPositions));
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("ok C:", new FoundStringEventHandler(ReadTargetPositions));
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("X:", new FoundStringEventHandler(ReadTargetPositions));
			((FoundStringCallbacks)ReadLineContainsCallBacks).AddCallbackToKey("T:", new FoundStringEventHandler(ReadTemperatures));
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("rs ", new FoundStringEventHandler(PrinterRequestsResend));
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("RS:", new FoundStringEventHandler(PrinterRequestsResend));
			((FoundStringCallbacks)ReadLineContainsCallBacks).AddCallbackToKey("Resend:", new FoundStringEventHandler(PrinterRequestsResend));
			((FoundStringCallbacks)ReadLineContainsCallBacks).AddCallbackToKey("FIRMWARE_NAME:", new FoundStringEventHandler(PrinterStatesFirmware));
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("EXTENSIONS:", new FoundStringEventHandler(PrinterStatesExtensions));
			((FoundStringCallbacks)ReadLineContainsCallBacks).AddCallbackToKey("T:inf", new FoundStringEventHandler(PrinterReportsError));
			((FoundStringCallbacks)ReadLineContainsCallBacks).AddCallbackToKey("B:inf", new FoundStringEventHandler(PrinterReportsError));
			((FoundStringCallbacks)ReadLineContainsCallBacks).AddCallbackToKey("MINTEMP", new FoundStringEventHandler(PrinterReportsError));
			((FoundStringCallbacks)ReadLineContainsCallBacks).AddCallbackToKey("MAXTEMP", new FoundStringEventHandler(PrinterReportsError));
			((FoundStringCallbacks)ReadLineContainsCallBacks).AddCallbackToKey("M999", new FoundStringEventHandler(PrinterReportsError));
			((FoundStringCallbacks)ReadLineContainsCallBacks).AddCallbackToKey("Error: Extruder switched off", new FoundStringEventHandler(PrinterReportsError));
			((FoundStringCallbacks)ReadLineContainsCallBacks).AddCallbackToKey("Heater decoupled", new FoundStringEventHandler(PrinterReportsError));
			((FoundStringCallbacks)ReadLineContainsCallBacks).AddCallbackToKey("cold extrusion prevented", new FoundStringEventHandler(PrinterReportsError));
			((FoundStringCallbacks)ReadLineContainsCallBacks).AddCallbackToKey("Error:Thermal Runaway, system stopped!", new FoundStringEventHandler(PrinterReportsError));
			((FoundStringCallbacks)ReadLineContainsCallBacks).AddCallbackToKey("Error:Heating failed", new FoundStringEventHandler(PrinterReportsError));
			((FoundStringCallbacks)ReadLineContainsCallBacks).AddCallbackToKey("Error:Printer halted", new FoundStringEventHandler(PrinterReportsError));
			((FoundStringCallbacks)ReadLineContainsCallBacks).AddCallbackToKey("dry run mode", new FoundStringEventHandler(PrinterReportsError));
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("accelerometer send i2c error", new FoundStringEventHandler(PrinterReportsError));
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("accelerometer i2c recv error", new FoundStringEventHandler(PrinterReportsError));
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("temp sensor defect", new FoundStringEventHandler(PrinterReportsError));
			((FoundStringCallbacks)ReadLineContainsCallBacks).AddCallbackToKey("Bot is Shutdown due to Overheat", new FoundStringEventHandler(PrinterReportsError));
			((FoundStringCallbacks)WriteLineStartCallBacks).AddCallbackToKey("G28", (FoundStringEventHandler)delegate
			{
				HasHomedSinceConnect = true;
			});
			((FoundStringCallbacks)WriteLineStartCallBacks).AddCallbackToKey("G90", new FoundStringEventHandler(MovementWasSetToAbsoluteMode));
			((FoundStringCallbacks)WriteLineStartCallBacks).AddCallbackToKey("G91", new FoundStringEventHandler(MovementWasSetToRelativeMode));
			((FoundStringCallbacks)WriteLineStartCallBacks).AddCallbackToKey("M80", new FoundStringEventHandler(AtxPowerUpWasWritenToPrinter));
			((FoundStringCallbacks)WriteLineStartCallBacks).AddCallbackToKey("M81", new FoundStringEventHandler(AtxPowerDownWasWritenToPrinter));
			((FoundStringCallbacks)WriteLineStartCallBacks).AddCallbackToKey("M82", new FoundStringEventHandler(ExtruderWasSetToAbsoluteMode));
			((FoundStringCallbacks)WriteLineStartCallBacks).AddCallbackToKey("M83", new FoundStringEventHandler(ExtruderWasSetToRelativeMode));
			((FoundStringCallbacks)WriteLineStartCallBacks).AddCallbackToKey("M104", new FoundStringEventHandler(ExtruderTemperatureWasWritenToPrinter));
			((FoundStringCallbacks)WriteLineStartCallBacks).AddCallbackToKey("M106", new FoundStringEventHandler(FanSpeedWasWritenToPrinter));
			((FoundStringCallbacks)WriteLineStartCallBacks).AddCallbackToKey("M107", new FoundStringEventHandler(FanOffWasWritenToPrinter));
			((FoundStringCallbacks)WriteLineStartCallBacks).AddCallbackToKey("M109", new FoundStringEventHandler(ExtruderTemperatureWasWritenToPrinter));
			((FoundStringCallbacks)WriteLineStartCallBacks).AddCallbackToKey("M140", new FoundStringEventHandler(BedTemperatureWasWritenToPrinter));
			((FoundStringCallbacks)WriteLineStartCallBacks).AddCallbackToKey("M190", new FoundStringEventHandler(BedTemperatureWasWritenToPrinter));
			((FoundStringCallbacks)WriteLineStartCallBacks).AddCallbackToKey("T", new FoundStringEventHandler(ExtruderIndexSet));
			CommunicationStateChanged.RegisterEvent((EventHandler)delegate
			{
				if (!PrinterIsConnected)
				{
					HasHomedSinceConnect = false;
				}
			}, ref unregisterEvents);
			ActiveSliceSettings.SettingChanged.RegisterEvent((EventHandler)delegate(object s, EventArgs e)
			{
				object obj = (object)(e as StringEventArgs);
				if (((obj != null) ? ((StringEventArgs)obj).get_Data() : null) == "feedrate_ratio")
				{
					feedRateRatio = ActiveSliceSettings.Instance.GetValue<double>("feedrate_ratio");
				}
			}, ref unregisterEvents);
		}

		private void ExtruderIndexSet(object sender, EventArgs e)
		{
			FoundStringEventArgs val = e as FoundStringEventArgs;
			double readValue = 0.0;
			if (GCodeFile.GetFirstNumberAfter("T", val.get_LineToCheck(), ref readValue))
			{
				currentlyActiveExtruderIndex = (int)readValue;
			}
		}

		public void AbortConnectionAttempt(string abortReason, bool shutdownReadLoop = true)
		{
			CommunicationState = CommunicationStates.Disconnecting;
			if (connectThread != null)
			{
				connectThread.Join(JoinThreadTimeoutMs);
			}
			if (shutdownReadLoop)
			{
				ReadThread.Join();
			}
			if (serialPort != null)
			{
				serialPort.Close();
				serialPort.Dispose();
				serialPort = null;
			}
			CommunicationState = CommunicationStates.Disconnected;
			connectionFailureMessage = abortReason;
			OnConnectionFailed(null);
		}

		public void BedTemperatureWasWritenToPrinter(object sender, EventArgs e)
		{
			string[] array = (e as FoundStringEventArgs).get_LineToCheck().Split(new char[1]
			{
				'S'
			});
			if (array.Length != 2)
			{
				return;
			}
			string s = array[1];
			try
			{
				double num = double.Parse(s);
				if (TargetBedTemperature != num)
				{
					targetBedTemperature = num;
					OnBedTemperatureSet(new TemperatureEventArgs(0, TargetBedTemperature));
				}
			}
			catch (Exception)
			{
			}
		}

		public void ConnectToActivePrinter(bool showHelpIfNoPort = false)
		{
			if (ActivePrinter == null)
			{
				return;
			}
			if (!ActiveSliceSettings.Instance.GetValue<bool>("enable_network_printing") && !FrostedSerialPort.EnsureDeviceAccess())
			{
				CommunicationState = CommunicationStates.FailedToConnect;
				return;
			}
			PrinterOutputCache.Instance.Clear();
			stopTryingToConnect = false;
			FirmwareType = FirmwareTypes.Unknown;
			firmwareUriGcodeSend = false;
			if (SerialPortIsAvailable(ComPort))
			{
				new Timer(new TimerCallback(ConnectionCallbackTimer)).Change(100, 0);
				connectThread = new Thread(new ThreadStart(Connect_Thread));
				connectThread.Name = "Connect To Printer";
				connectThread.IsBackground = true;
				connectThread.Start();
			}
			else
			{
				connectionFailureMessage = string.Format("{0} is not available".Localize(), ComPort);
				OnConnectionFailed(null);
				if (showHelpIfNoPort)
				{
					WizardWindow.ShowComPortSetup();
				}
			}
		}

		public void DeleteFileFromSdCard(string fileName)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("File deleted:", new FoundStringEventHandler(FileDeleteConfirmed));
			SendLineToPrinterNow(StringHelper.FormatWith("M30 {0}", new object[1]
			{
				fileName.ToLower()
			}));
		}

		public void Disable()
		{
			if (PrinterIsConnected)
			{
				ForceImmediateWrites = true;
				ReleaseMotors();
				TurnOffBedAndExtruders();
				FanSpeed0To255 = 0;
				ForceImmediateWrites = false;
				CommunicationState = CommunicationStates.Disconnecting;
				ReadThread.Join();
				if (serialPort != null)
				{
					serialPort.Close();
					serialPort.Dispose();
				}
				serialPort = null;
				CommunicationState = CommunicationStates.Disconnected;
			}
			else
			{
				TurnOffBedAndExtruders();
				FanSpeed0To255 = 0;
			}
			OnEnabledChanged(null);
		}

		public void ExtruderTemperatureWasWritenToPrinter(object sender, EventArgs e)
		{
			FoundStringEventArgs val = e as FoundStringEventArgs;
			double readValue = 0.0;
			if (GCodeFile.GetFirstNumberAfter("S", val.get_LineToCheck(), ref readValue))
			{
				double readValue2 = 0.0;
				if (GCodeFile.GetFirstNumberAfter("T", val.get_LineToCheck(), ref readValue2))
				{
					int num = Math.Min((int)readValue2, 15);
					targetExtruderTemperature[num] = readValue;
				}
				else
				{
					targetExtruderTemperature[currentlyActiveExtruderIndex] = readValue;
				}
				OnExtruderTemperatureSet(new TemperatureEventArgs((int)readValue2, readValue));
			}
		}

		public void FanOffWasWritenToPrinter(object sender, EventArgs e)
		{
			fanSpeed = 0;
			OnFanSpeedSet(null);
		}

		public void FanSpeedWasWritenToPrinter(object sender, EventArgs e)
		{
			string[] array = (e as FoundStringEventArgs).get_LineToCheck().Split(new char[1]
			{
				'S'
			});
			if (array.Length != 2)
			{
				array = "M106 S255".Split(new char[1]
				{
					'S'
				});
			}
			if (array.Length != 2)
			{
				return;
			}
			string s = array[1];
			try
			{
				int num = int.Parse(s);
				if (FanSpeed0To255 != num)
				{
					fanSpeed = num;
					OnFanSpeedSet(null);
				}
			}
			catch (Exception)
			{
			}
		}

		public void FoundStart(object sender, EventArgs e)
		{
			(e as FoundStringEventArgs).set_SendToDelegateFunctions(false);
		}

		public double GetActualExtruderTemperature(int extruderIndex0Based)
		{
			extruderIndex0Based = Math.Min(extruderIndex0Based, 15);
			return actualExtruderTemperature[extruderIndex0Based];
		}

		public double GetTargetExtruderTemperature(int extruderIndex0Based)
		{
			extruderIndex0Based = Math.Min(extruderIndex0Based, 15);
			return targetExtruderTemperature[extruderIndex0Based];
		}

		public void HaltConnectionThread()
		{
			stopTryingToConnect = true;
		}

		public void HomeAxis(Axis axis)
		{
			string text = "G28";
			if (!axis.HasFlag(Axis.XYZ))
			{
				if ((axis & Axis.X) == Axis.X)
				{
					text += " X0";
				}
				if ((axis & Axis.Y) == Axis.Y)
				{
					text += " Y0";
				}
				if ((axis & Axis.Z) == Axis.Z)
				{
					text += " Z0";
				}
			}
			SendLineToPrinterNow(text);
			ReadPosition();
		}

		public void MoveAbsolute(Axis axis, double axisPositionMm, double feedRateMmPerMinute)
		{
			SetMovementToAbsolute();
			SendLineToPrinterNow(StringHelper.FormatWith("G1 {0}{1:0.###} F{2}", new object[3]
			{
				axis,
				axisPositionMm,
				feedRateMmPerMinute
			}));
		}

		public void MoveAbsolute(Vector3 position, double feedRateMmPerMinute)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			SetMovementToAbsolute();
			SendLineToPrinterNow(StringHelper.FormatWith("G1 X{0:0.###}Y{1:0.###}Z{2:0.###} F{3}", new object[4]
			{
				position.x,
				position.y,
				position.z,
				feedRateMmPerMinute
			}));
		}

		public void MoveExtruderRelative(double moveAmountMm, double feedRateMmPerMinute, int extruderNumber = 0)
		{
			if (moveAmountMm != 0.0)
			{
				bool flag = extruderNumber != 0;
				SetMovementToRelative();
				if (flag)
				{
					SendLineToPrinterNow(StringHelper.FormatWith("T{0}", new object[1]
					{
						extruderNumber
					}));
				}
				SendLineToPrinterNow(StringHelper.FormatWith("G1 E{0:0.###} F{1}", new object[2]
				{
					moveAmountMm,
					feedRateMmPerMinute
				}));
				if (flag)
				{
					SendLineToPrinterNow("T0");
				}
				SetMovementToAbsolute();
			}
		}

		public void MoveRelative(Axis axis, double moveAmountMm, double feedRateMmPerMinute)
		{
			if (moveAmountMm != 0.0)
			{
				SetMovementToRelative();
				SendLineToPrinterNow(StringHelper.FormatWith("G1 {0}{1:0.###} F{2}", new object[3]
				{
					axis,
					moveAmountMm,
					feedRateMmPerMinute
				}));
				SetMovementToAbsolute();
			}
		}

		public void OnCommunicationStateChanged(EventArgs e)
		{
			CommunicationStateChanged.CallEvents((object)this, e);
		}

		public void OnConnectionFailed(EventArgs e)
		{
			ConnectionFailed.CallEvents((object)this, e);
			CommunicationState = CommunicationStates.FailedToConnect;
			OnEnabledChanged(e);
		}

		public void OnIdle()
		{
			if (PrinterIsConnected && ReadThread.NumRunning == 0)
			{
				ReadThread.Start();
			}
		}

		public void OnPrintFinished(EventArgs e)
		{
			PrintFinished.CallEvents((object)this, (EventArgs)new PrintItemWrapperEventArgs(ActivePrintItem));
			bool flag = false;
			foreach (KeyValuePair<string, string> item in ActiveSliceSettings.Instance.BaseLayer)
			{
				string value = ActiveSliceSettings.Instance.GetValue(item.Key);
				bool flag2 = (value == "0") | (value == "");
				if ((SliceSettingsOrganizer.Instance.GetSettingsData(item.Key)?.ResetAtEndOfPrint ?? false) && !flag2)
				{
					flag = true;
					ActiveSliceSettings.Instance.ClearValue(item.Key);
				}
			}
			if (flag)
			{
				ApplicationController.Instance.ReloadAdvancedControlsPanel();
			}
		}

		public void PrintActivePart(bool overrideAllowGCode = false)
		{
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Expected O, but got Unknown
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!ActiveSliceSettings.Instance.GetValue<bool>("print_leveling_required_to_print") && !ActiveSliceSettings.Instance.GetValue<bool>("print_leveling_enabled"))
				{
					goto IL_004c;
				}
				PrintLevelingData printLevelingData = ActiveSliceSettings.Instance.Helpers.GetPrintLevelingData();
				if (printLevelingData != null && printLevelingData.HasBeenRunAndEnabled())
				{
					goto IL_004c;
				}
				LevelWizardBase.ShowPrintLevelWizard();
				goto end_IL_0000;
				IL_004c:
				if (ActivePrintItem == null)
				{
					return;
				}
				string fileLocation = ActivePrintItem.FileLocation;
				if (ActiveSliceSettings.Instance.GetValue<bool>("has_sd_card_reader") && fileLocation == QueueData.SdCardFileName)
				{
					StartSdCardPrint();
				}
				else if (File.Exists(fileLocation))
				{
					PrinterOutputCache.Instance.Clear();
					ApplicationSettings.Instance.get("HideGCodeWarning");
					if (Path.GetExtension(fileLocation)!.ToUpper() == ".GCODE" && !overrideAllowGCode)
					{
						CheckBox hideGCodeWarningCheckBox = new CheckBox(doNotAskAgainMessage);
						hideGCodeWarningCheckBox.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
						((GuiWidget)hideGCodeWarningCheckBox).set_Margin(new BorderDouble(6.0, 0.0, 0.0, 6.0));
						((GuiWidget)hideGCodeWarningCheckBox).set_HAnchor((HAnchor)1);
						((GuiWidget)hideGCodeWarningCheckBox).add_Click((EventHandler<MouseEventArgs>)delegate
						{
							if (hideGCodeWarningCheckBox.get_Checked())
							{
								ApplicationSettings.Instance.set("HideGCodeWarning", "true");
							}
							else
							{
								ApplicationSettings.Instance.set("HideGCodeWarning", null);
							}
						});
						if (ApplicationSettings.Instance.get("HideGCodeWarning") == null)
						{
							UiThread.RunOnIdle((Action)delegate
							{
								StyledMessageBox.ShowMessageBox(onConfirmPrint, gcodeWarningMessage, "Warning - GCode file".Localize(), (GuiWidget[])(object)new GuiWidget[2]
								{
									new VerticalSpacer(),
									(GuiWidget)hideGCodeWarningCheckBox
								}, StyledMessageBox.MessageType.YES_NO);
							});
						}
						else
						{
							onConfirmPrint(messageBoxResponse: true);
						}
					}
					else if (ActiveSliceSettings.Instance.IsValid())
					{
						CommunicationState = CommunicationStates.PreparingToPrint;
						PrintItemWrapper printItemWrapper = ActivePrintItem;
						SlicingQueue.Instance.QueuePartForSlicing(printItemWrapper);
						printItemWrapper.SlicingDone += partToPrint_SliceDone;
					}
				}
				else
				{
					string message = string.Format(removeFromQueueMessage, fileLocation);
					StyledMessageBox.ShowMessageBox(onRemoveMessageConfirm, message, itemNotFoundMessage, StyledMessageBox.MessageType.YES_NO, "Remove".Localize(), "Cancel".Localize());
				}
				end_IL_0000:;
			}
			catch (Exception)
			{
			}
		}

		public void PrintActivePartIfPossible(bool overrideAllowGCode = false)
		{
			if (CommunicationState == CommunicationStates.Connected || CommunicationState == CommunicationStates.FinishedPrint)
			{
				PrintActivePart(overrideAllowGCode);
			}
		}

		public void PrinterRequestsResend(object sender, EventArgs e)
		{
			FoundStringEventArgs val = e as FoundStringEventArgs;
			if (val == null || string.IsNullOrEmpty(val.get_LineToCheck()))
			{
				return;
			}
			string lineToCheck = val.get_LineToCheck();
			if (!GCodeFile.GetFirstNumberAfter(":", lineToCheck, ref currentLineIndexToSend))
			{
				if (currentLineIndexToSend == allCheckSumLinesSent.Count)
				{
					return;
				}
				if (GCodeFile.GetFirstNumberAfter("N", lineToCheck, ref currentLineIndexToSend))
				{
					PrintingCanContinue(null, null);
				}
			}
			if (currentLineIndexToSend != allCheckSumLinesSent.Count && (currentLineIndexToSend >= allCheckSumLinesSent.Count || currentLineIndexToSend == 1))
			{
				SendLineToPrinterNow("M110 N1");
				allCheckSumLinesSent.SetStartingIndex(1);
				waitingForPosition.Reset();
				PositionReadQueued = false;
			}
		}

		public void PrinterReportsError(object sender, EventArgs e)
		{
			if (haveReportedError)
			{
				return;
			}
			haveReportedError = true;
			FoundStringEventArgs val = e as FoundStringEventArgs;
			if (val != null)
			{
				string message = "Your printer is reporting a hardware Error. This may prevent your printer from functioning properly.".Localize() + "\n\n" + "Error Reported".Localize() + ":" + $" \"{val.get_LineToCheck()}\".";
				UiThread.RunOnIdle((Action)delegate
				{
					StyledMessageBox.ShowMessageBox(null, message, "Printer Hardware Error".Localize());
				});
			}
		}

		public void PrinterStatesExtensions(object sender, EventArgs e)
		{
			FoundStringEventArgs val = e as FoundStringEventArgs;
			if (val != null && val.get_LineToCheck().Contains("URI_GCODE_SEND"))
			{
				firmwareUriGcodeSend = true;
			}
		}

		public void PrinterStatesFirmware(object sender, EventArgs e)
		{
			FoundStringEventArgs val = e as FoundStringEventArgs;
			string nextString = "";
			if (GCodeFile.GetFirstStringAfter("FIRMWARE_NAME:", val.get_LineToCheck(), " ", ref nextString))
			{
				nextString = nextString.ToLower();
				if (nextString.Contains("repetier"))
				{
					FirmwareType = FirmwareTypes.Repetier;
				}
				else if (nextString.Contains("marlin"))
				{
					FirmwareType = FirmwareTypes.Marlin;
				}
				else if (nextString.Contains("sprinter"))
				{
					FirmwareType = FirmwareTypes.Sprinter;
				}
			}
			string nextString2 = "";
			if (!GCodeFile.GetFirstStringAfter("MACHINE_TYPE:", val.get_LineToCheck(), " EXTRUDER_COUNT", ref nextString2))
			{
				return;
			}
			char c = '^';
			if (Enumerable.Contains<char>((IEnumerable<char>)nextString2, c))
			{
				string[] array = nextString2.Split(new char[1]
				{
					c
				});
				if (Enumerable.Count<string>((IEnumerable<string>)array) == 2)
				{
					DeviceCode = array[0];
					nextString2 = array[1];
				}
			}
			if (nextString2 != "" && FirmwareVersion != nextString2)
			{
				FirmwareVersion = nextString2;
				OnFirmwareVersionRead(null);
			}
		}

		public void PrintingCanContinue(object sender, EventArgs e)
		{
			timeHaveBeenWaitingForOK.Stop();
		}

		public void ArduinoDtrReset()
		{
			if (serialPort == null && ActivePrinter != null)
			{
				IFrostedSerialPort frostedSerialPort = FrostedSerialPortFactory.GetAppropriateFactory(DriverType).Create(ComPort);
				frostedSerialPort.Open();
				Thread.Sleep(500);
				ToggleHighLowHigh(frostedSerialPort);
				frostedSerialPort.Close();
			}
		}

		public void ReadFromPrinter(ReadThread readThreadHolder)
		{
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Expected O, but got Unknown
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Expected O, but got Unknown
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Expected O, but got Unknown
			string text = string.Empty;
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			timeSinceLastReadAnything.Restart();
			while (CommunicationState == CommunicationStates.AttemptingToConnect || (PrinterIsConnected && serialPort != null && serialPort.IsOpen && !Disconnecting && readThreadHolder.IsCurrentThread()))
			{
				if ((PrinterIsConnected || communicationState == CommunicationStates.AttemptingToConnect) && CommunicationState != CommunicationStates.PrintingFromSd)
				{
					TryWriteNextLineFromGCodeFile();
				}
				try
				{
					while (serialPort != null && serialPort.BytesToRead > 0 && readThreadHolder.IsCurrentThread())
					{
						lock (locker)
						{
							string text2 = serialPort.ReadExisting();
							text2 = text2.Replace("\r\n", "\n");
							text2 = text2.Replace('\r', '\n');
							text += text2;
							while (true)
							{
								int num = text.IndexOf('\n');
								if (communicationState == CommunicationStates.AttemptingToConnect && num < 0 && Enumerable.Count<char>((IEnumerable<char>)text, (Func<char, bool>)((char c) => c == '?')) > 3)
								{
									AbortConnectionAttempt("Invalid printer response".Localize(), shutdownReadLoop: false);
								}
								if (num < 0)
								{
									break;
								}
								if (text.Length <= 0)
								{
									continue;
								}
								if (lastLineRead.StartsWith("ok"))
								{
									timeSinceRecievedOk.Restart();
								}
								lastLineRead = text.Substring(0, num);
								lastLineRead = ProcessReadRegEx(lastLineRead);
								text = text.Substring(num + 1);
								StringEventArgs val = new StringEventArgs(lastLineRead);
								if (PrinterIsPrinting)
								{
									CommunicationUnconditionalFromPrinter.CallEvents((object)this, (EventArgs)new StringEventArgs(StringHelper.FormatWith("{0} [{1:0.000}]\n", new object[2]
									{
										lastLineRead,
										timeSinceStartedPrint.get_Elapsed().TotalSeconds
									})));
								}
								else
								{
									CommunicationUnconditionalFromPrinter.CallEvents((object)this, (EventArgs)(object)val);
								}
								FoundStringEventArgs val2 = new FoundStringEventArgs(val.get_Data());
								ReadLineStartCallBacks.CheckForKeys((EventArgs)(object)val2);
								ReadLineContainsCallBacks.CheckForKeys((EventArgs)(object)val2);
								if (val2.get_SendToDelegateFunctions())
								{
									ReadLine.CallEvents((object)this, (EventArgs)(object)val);
								}
								if (CommunicationState != CommunicationStates.AttemptingToConnect)
								{
									continue;
								}
								if (lastLineRead.Split(new char[1]
								{
									'?'
								}).Length <= 3)
								{
									CommunicationState = CommunicationStates.Connected;
									TurnOffBedAndExtruders();
									haveReportedError = false;
									ClearQueuedGCode();
									string value = ActiveSliceSettings.Instance.GetValue("connect_gcode");
									SendLineToPrinterNow(value);
									UiThread.RunOnIdle((Action)delegate
									{
										ConnectionSucceeded.CallEvents((object)this, (EventArgs)null);
									});
									if (ActiveSliceSettings.Instance.GetValue<bool>("print_leveling_required_to_print") || ActiveSliceSettings.Instance.GetValue<bool>("print_leveling_enabled"))
									{
										PrintLevelingData printLevelingData = ActiveSliceSettings.Instance.Helpers.GetPrintLevelingData();
										if (printLevelingData == null || !printLevelingData.HasBeenRunAndEnabled())
										{
											UiThread.RunOnIdle((Action)LevelWizardBase.ShowPrintLevelWizard);
										}
									}
								}
								else
								{
									AbortConnectionAttempt("Invalid printer response".Localize(), shutdownReadLoop: false);
								}
							}
						}
						timeSinceLastReadAnything.Restart();
					}
					if (PrinterIsPrinting)
					{
						Thread.Sleep(0);
					}
					else
					{
						Thread.Sleep(1);
					}
				}
				catch (TimeoutException)
				{
				}
				catch (IOException ex2)
				{
					PrinterOutputCache.Instance.WriteLine("Exception:" + ex2.Message);
					OnConnectionFailed(null);
				}
				catch (InvalidOperationException ex3)
				{
					PrinterOutputCache.Instance.WriteLine("Exception:" + ex3.Message);
					OnConnectionFailed(null);
				}
				catch (UnauthorizedAccessException ex4)
				{
					PrinterOutputCache.Instance.WriteLine("Exception:" + ex4.Message);
					OnConnectionFailed(null);
				}
				catch (Exception)
				{
				}
			}
		}

		public void ReadPosition(bool forceToTopOfQueue = false)
		{
			SendLineToPrinterNow("M114", forceToTopOfQueue);
			PositionReadQueued = true;
		}

		public void ReadSdProgress(object sender, EventArgs e)
		{
			FoundStringEventArgs val = e as FoundStringEventArgs;
			if (val != null)
			{
				string[] array = val.get_LineToCheck().Substring("Sd printing byte ".Length).Split(new char[1]
				{
					'/'
				});
				currentSdBytes = long.Parse(array[0]);
				totalSdBytes = long.Parse(array[1]);
			}
			timeWaitingForSdProgress.Stop();
		}

		public void ReadTargetPositions(object sender, EventArgs e)
		{
			string lineToCheck = (e as FoundStringEventArgs).get_LineToCheck();
			GCodeFile.GetFirstNumberAfter("X:", lineToCheck, ref lastReportedPosition.position.x);
			GCodeFile.GetFirstNumberAfter("Y:", lineToCheck, ref lastReportedPosition.position.y);
			GCodeFile.GetFirstNumberAfter("Z:", lineToCheck, ref lastReportedPosition.position.z);
			GCodeFile.GetFirstNumberAfter("E:", lineToCheck, ref lastReportedPosition.extrusion);
			currentDestination = lastReportedPosition;
			DestinationChanged.CallEvents((object)this, (EventArgs)null);
			if (totalGCodeStream != null)
			{
				totalGCodeStream.SetPrinterPosition(currentDestination);
			}
			PositionRead.CallEvents((object)this, (EventArgs)null);
			waitingForPosition.Reset();
			PositionReadQueued = false;
		}

		public void ReadTemperatures(object sender, EventArgs e)
		{
			string lineToCheck = (e as FoundStringEventArgs).get_LineToCheck();
			double readValue = 0.0;
			if (GCodeFile.GetFirstNumberAfter("T:", lineToCheck, ref readValue) && actualExtruderTemperature[0] != readValue)
			{
				actualExtruderTemperature[0] = readValue;
				OnExtruderTemperatureRead(new TemperatureEventArgs(0, GetActualExtruderTemperature(0)));
			}
			for (int i = 0; i < 16; i++)
			{
				if (!GCodeFile.GetFirstNumberAfter(StringHelper.FormatWith("T{0}:", new object[1]
				{
					i
				}), lineToCheck, ref readValue))
				{
					break;
				}
				if (actualExtruderTemperature[i] != readValue)
				{
					actualExtruderTemperature[i] = readValue;
					OnExtruderTemperatureRead(new TemperatureEventArgs(i, GetActualExtruderTemperature(i)));
				}
			}
			double readValue2 = 0.0;
			if (GCodeFile.GetFirstNumberAfter("B:", lineToCheck, ref readValue2) && actualBedTemperature != readValue2)
			{
				actualBedTemperature = readValue2;
				OnBedTemperatureRead(new TemperatureEventArgs(0, ActualBedTemperature));
			}
		}

		public void RebootBoard()
		{
			try
			{
				if (!ActiveSliceSettings.Instance.PrinterSelected)
				{
					return;
				}
				if (serialPort != null)
				{
					Stop(markPrintCanceled: false);
					ClearQueuedGCode();
					CommunicationState = CommunicationStates.Disconnecting;
					ReadThread.Join();
					ToggleHighLowHigh(serialPort);
					if (serialPort != null)
					{
						serialPort.Close();
						serialPort.Dispose();
					}
					serialPort = null;
					CreateStreamProcessors(null, recoveryEnabled: false);
					CommunicationState = CommunicationStates.Disconnected;
					UiThread.RunOnIdle((Action)delegate
					{
						ConnectToActivePrinter();
					}, 2.0);
				}
				else
				{
					IFrostedSerialPort frostedSerialPort = FrostedSerialPortFactory.GetAppropriateFactory(DriverType).Create(ComPort);
					frostedSerialPort.Open();
					Thread.Sleep(500);
					ToggleHighLowHigh(frostedSerialPort);
					frostedSerialPort.Close();
					CommunicationState = CommunicationStates.Disconnected;
				}
			}
			catch (Exception)
			{
			}
		}

		private void ToggleHighLowHigh(IFrostedSerialPort serialPort)
		{
			serialPort.RtsEnable = true;
			serialPort.DtrEnable = true;
			Thread.Sleep(100);
			serialPort.RtsEnable = false;
			serialPort.DtrEnable = false;
			Thread.Sleep(100);
			serialPort.RtsEnable = true;
			serialPort.DtrEnable = true;
		}

		public void ReleaseMotors()
		{
			SendLineToPrinterNow("M84");
		}

		public void RequestPause()
		{
			if (PrinterIsPrinting)
			{
				if (CommunicationState == CommunicationStates.PrintingFromSd)
				{
					CommunicationState = CommunicationStates.Paused;
					SendLineToPrinterNow("M25");
				}
				else
				{
					pauseHandlingStream1.DoPause(PauseHandlingStream.PauseReason.UserRequested);
				}
			}
		}

		public void ResetToReadyState()
		{
			if (CommunicationState == CommunicationStates.FinishedPrint)
			{
				CommunicationState = CommunicationStates.Connected;
				return;
			}
			throw new Exception("You should only reset after a print has finished.");
		}

		public void Resume()
		{
			if (PrinterIsPaused)
			{
				if (PrePauseCommunicationState == CommunicationStates.PrintingFromSd)
				{
					CommunicationState = CommunicationStates.PrintingFromSd;
					SendLineToPrinterNow("M24");
				}
				else
				{
					pauseHandlingStream1.Resume();
					CommunicationState = CommunicationStates.Printing;
				}
			}
		}

		public void SendLinesToPrinterNow(string[] linesToWrite)
		{
			if (PrinterIsPrinting && CommunicationState != CommunicationStates.PrintingFromSd)
			{
				for (int num = linesToWrite.Length - 1; num >= 0; num--)
				{
					string text = linesToWrite[num].Trim();
					if (text.Length > 0)
					{
						SendLineToPrinterNow(text);
					}
				}
				return;
			}
			for (int i = 0; i < linesToWrite.Length; i++)
			{
				string text2 = linesToWrite[i].Trim();
				if (text2.Length > 0)
				{
					SendLineToPrinterNow(text2);
				}
			}
		}

		public void SendLineToPrinterNow(string lineToWrite, bool forceTopOfQueue = false)
		{
			lock (locker)
			{
				if (lineToWrite.Contains("\\n"))
				{
					lineToWrite = lineToWrite.Replace("\\n", "\n");
				}
				if (lineToWrite.Contains("\n"))
				{
					string[] linesToWrite = lineToWrite.Split(new string[1]
					{
						"\n"
					}, StringSplitOptions.None);
					SendLinesToPrinterNow(linesToWrite);
				}
				else if (CommunicationState == CommunicationStates.PrintingFromSd || ForceImmediateWrites)
				{
					lineToWrite = lineToWrite.Split(new char[1]
					{
						';'
					})[0].Trim();
					if (lineToWrite.Trim().Length > 0)
					{
						WriteRawToPrinter(lineToWrite + "\n", lineToWrite);
					}
				}
				else if (lineToWrite.Trim().Length > 0)
				{
					InjectGCode(lineToWrite, forceTopOfQueue);
				}
			}
		}

		private string ProcessReadRegEx(string lineBeingRead)
		{
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Expected O, but got Unknown
			if (read_regex != ActiveSliceSettings.Instance.GetValue("read_regex"))
			{
				ReadLineReplacements.Clear();
				string text = "\\n";
				read_regex = ActiveSliceSettings.Instance.GetValue("read_regex");
				string[] array = read_regex.Split(text.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				foreach (string text2 in array)
				{
					MatchCollection val = getQuotedParts.Matches(text2);
					if (val.get_Count() == 2)
					{
						string text3 = ((Capture)val.get_Item(0)).get_Value().Substring(1, ((Capture)val.get_Item(0)).get_Value().Length - 2);
						string replacement = ((Capture)val.get_Item(1)).get_Value().Substring(1, ((Capture)val.get_Item(1)).get_Value().Length - 2);
						ReadLineReplacements.Add(new RegReplace
						{
							Regex = new Regex(text3, (RegexOptions)8),
							Replacement = replacement
						});
					}
				}
			}
			foreach (RegReplace readLineReplacement in ReadLineReplacements)
			{
				lineBeingRead = readLineReplacement.Regex.Replace(lineBeingRead, readLineReplacement.Replacement);
			}
			return lineBeingRead;
		}

		public bool SerialPortIsAvailable(string portName)
		{
			if (IsNetworkPrinting())
			{
				return true;
			}
			try
			{
				return Enumerable.Any<string>((IEnumerable<string>)FrostedSerialPort.GetPortNames(), (Func<string, bool>)((string x) => string.Compare(x, portName, ignoreCase: true) == 0));
			}
			catch (Exception)
			{
				return false;
			}
		}

		public void SetMovementToAbsolute()
		{
			SendLineToPrinterNow("G90");
		}

		public void SetMovementToRelative()
		{
			SendLineToPrinterNow("G91");
		}

		public void SetTargetExtruderTemperature(int extruderIndex0Based, double temperature, bool forceSend = false)
		{
			extruderIndex0Based = Math.Min(extruderIndex0Based, 15);
			if (targetExtruderTemperature[extruderIndex0Based] != temperature || forceSend)
			{
				targetExtruderTemperature[extruderIndex0Based] = temperature;
				OnExtruderTemperatureSet(new TemperatureEventArgs(extruderIndex0Based, temperature));
				if (PrinterIsConnected)
				{
					SendLineToPrinterNow(StringHelper.FormatWith("M104 T{0} S{1}", new object[2]
					{
						extruderIndex0Based,
						targetExtruderTemperature[extruderIndex0Based]
					}));
				}
			}
		}

		public async void StartPrint(string gcodeFilename, PrintTask printTaskToUse = null)
		{
			if (PrinterIsConnected && !PrinterIsPrinting)
			{
				haveReportedError = false;
				PrintWasCanceled = false;
				waitingForPosition.Reset();
				PositionReadQueued = false;
				ClearQueuedGCode();
				activePrintTask = printTaskToUse;
				await Task.Run(delegate
				{
					LoadGCodeToPrint(gcodeFilename);
				});
				DoneLoadingGCodeToPrint();
			}
		}

		public bool StartSdCardPrint()
		{
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Expected O, but got Unknown
			if (!PrinterIsConnected || PrinterIsPrinting || ActivePrintItem.PrintItem.FileLocation != QueueData.SdCardFileName)
			{
				return false;
			}
			currentSdBytes = 0.0;
			ClearQueuedGCode();
			CommunicationState = CommunicationStates.PrintingFromSd;
			SendLineToPrinterNow(StringHelper.FormatWith("M23 {0}", new object[1]
			{
				ActivePrintItem.PrintItem.Name.ToLower()
			}));
			SendLineToPrinterNow("M24");
			((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("Done printing file", new FoundStringEventHandler(DonePrintingSdFile));
			return true;
		}

		public void Stop(bool markPrintCanceled = true)
		{
			switch (CommunicationState)
			{
			case CommunicationStates.PrintingFromSd:
				CancelSDCardPrint();
				break;
			case CommunicationStates.Printing:
				CancelPrint(markPrintCanceled);
				break;
			case CommunicationStates.Paused:
				if (PrePauseCommunicationState == CommunicationStates.PrintingFromSd)
				{
					CancelSDCardPrint();
					CommunicationState = CommunicationStates.Connected;
				}
				else
				{
					CancelPrint(markPrintCanceled);
					CommunicationState = CommunicationStates.Printing;
				}
				break;
			case CommunicationStates.AttemptingToConnect:
				CommunicationState = CommunicationStates.FailedToConnect;
				connectThread.Join(JoinThreadTimeoutMs);
				CommunicationState = CommunicationStates.Disconnecting;
				ReadThread.Join();
				if (serialPort != null)
				{
					serialPort.Close();
					serialPort.Dispose();
					serialPort = null;
				}
				CommunicationState = CommunicationStates.Disconnected;
				break;
			case CommunicationStates.PreparingToPrint:
				SlicingQueue.Instance.CancelCurrentSlicing();
				CommunicationState = CommunicationStates.Connected;
				break;
			case CommunicationStates.FailedToConnect:
			case CommunicationStates.Connected:
				break;
			}
		}

		private void CancelPrint(bool markPrintCanceled)
		{
			lock (locker)
			{
				ClearQueuedGCode();
				string value = ActiveSliceSettings.Instance.GetValue("cancel_gcode");
				if (value.Trim() != "")
				{
					InjectGCode(value);
				}
				PrintWasCanceled = true;
				if (markPrintCanceled && activePrintTask != null)
				{
					DateTime.Now.Subtract(activePrintTask.PrintStart);
					activePrintTask.PrintEnd = DateTime.Now;
					activePrintTask.PrintComplete = false;
					activePrintTask.PrintingGCodeFileName = "";
					activePrintTask.Commit();
				}
				activePrintTask = null;
			}
		}

		private void CancelSDCardPrint()
		{
			lock (locker)
			{
				ClearQueuedGCode();
				CommunicationState = CommunicationStates.Connected;
				SendLineToPrinterNow("M25");
				SendLineToPrinterNow("M26");
				DonePrintingSdFile(this, null);
			}
		}

		public void SuppressEcho(object sender, EventArgs e)
		{
			(e as FoundStringEventArgs).set_SendToDelegateFunctions(false);
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr securityAttrs, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

		private void AttemptToConnect(string serialPortName, int baudRate)
		{
			activePrintTask = null;
			connectionFailureMessage = "Unknown Reason".Localize();
			if (PrinterIsConnected)
			{
				return;
			}
			FrostedSerialPortFactory appropriateFactory = FrostedSerialPortFactory.GetAppropriateFactory(DriverType);
			bool num = SerialPortIsAvailable(serialPortName);
			bool flag = appropriateFactory.SerialPortAlreadyOpen(serialPortName);
			if (num && !flag)
			{
				if (!PrinterIsConnected)
				{
					try
					{
						serialPort = appropriateFactory.CreateAndOpen(serialPortName, baudRate, DtrEnableOnConnect: true);
						Thread.Sleep(500);
						CommunicationState = CommunicationStates.AttemptingToConnect;
						ReadThread.Join();
						Console.WriteLine("ReadFromPrinter thread created.");
						ReadThread.Start();
						CreateStreamProcessors(null, recoveryEnabled: false);
						SendLineToPrinterNow("M110 N1");
						ClearQueuedGCode();
						PrintingCanContinue(null, null);
					}
					catch (ArgumentOutOfRangeException ex)
					{
						PrinterOutputCache.Instance.WriteLine("Exception:" + ex.Message);
						connectionFailureMessage = "Unsupported Baud Rate".Localize();
						OnConnectionFailed(null);
					}
					catch (Exception ex2)
					{
						PrinterOutputCache.Instance.WriteLine("Exception:" + ex2.Message);
						OnConnectionFailed(null);
					}
				}
			}
			else
			{
				connectionFailureMessage = (flag ? $"{ComPort} in use" : "Port not found".Localize());
				OnConnectionFailed(null);
			}
		}

		private void ClearQueuedGCode()
		{
			loadedGCode.Clear();
			WriteChecksumLineToPrinter("M110 N1");
		}

		private void Connect_Thread()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			if (FrostedSerialPort.GetPortNames().Length != 0 || IsNetworkPrinting())
			{
				AttemptToConnect(ComPort, BaudRate);
				if (CommunicationState == CommunicationStates.FailedToConnect)
				{
					OnConnectionFailed(null);
				}
			}
			else
			{
				OnConnectionFailed(null);
			}
		}

		private void ConnectionCallbackTimer(object state)
		{
			Timer timer = (Timer)state;
			if (!ContinueConnectionThread())
			{
				timer.Dispose();
			}
			else
			{
				timer.Change(100, 0);
			}
		}

		private bool ContinueConnectionThread()
		{
			if (CommunicationState == CommunicationStates.AttemptingToConnect)
			{
				if (stopTryingToConnect)
				{
					connectThread.Join(JoinThreadTimeoutMs);
					Disable();
					connectionFailureMessage = "Canceled".Localize();
					OnConnectionFailed(null);
					return false;
				}
				return true;
			}
			connectThread.Join(JoinThreadTimeoutMs);
			return false;
		}

		private void DonePrintingSdFile(object sender, EventArgs e)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Expected O, but got Unknown
				((FoundStringCallbacks)ReadLineStartCallBacks).RemoveCallbackFromKey("Done printing file", new FoundStringEventHandler(DonePrintingSdFile));
			});
			CommunicationState = CommunicationStates.FinishedPrint;
			PrintJobName = null;
			TurnOffBedAndExtruders();
			ReleaseMotors();
		}

		private void ExtruderWasSetToAbsoluteMode(object sender, EventArgs e)
		{
			extruderMode = PrinterMachineInstruction.MovementTypes.Absolute;
		}

		private void ExtruderWasSetToRelativeMode(object sender, EventArgs e)
		{
			extruderMode = PrinterMachineInstruction.MovementTypes.Relative;
		}

		private void FileDeleteConfirmed(object sender, EventArgs e)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Expected O, but got Unknown
				((FoundStringCallbacks)ReadLineStartCallBacks).RemoveCallbackFromKey("File deleted:", new FoundStringEventHandler(FileDeleteConfirmed));
			});
			PrintingCanContinue(this, null);
		}

		private void InjectGCode(string codeToInject, bool forceTopOfQueue = false)
		{
			codeToInject = codeToInject.Replace("\\n", "\n");
			string[] array = codeToInject.Split(new char[1]
			{
				'\n'
			});
			for (int i = 0; i < array.Length; i++)
			{
				queuedCommandStream2?.Add(array[i], forceTopOfQueue);
			}
		}

		private void KeepTrackOfAbsolutePostionAndDestination(string lineBeingSent)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			if (lineBeingSent.StartsWith("G0 ") || lineBeingSent.StartsWith("G1 ") || lineBeingSent.StartsWith("G2 ") || lineBeingSent.StartsWith("G3 "))
			{
				PrinterMove printerMove = currentDestination;
				if (movementMode == PrinterMachineInstruction.MovementTypes.Relative)
				{
					printerMove.position = Vector3.Zero;
				}
				GCodeFile.GetFirstNumberAfter("X", lineBeingSent, ref printerMove.position.x);
				GCodeFile.GetFirstNumberAfter("Y", lineBeingSent, ref printerMove.position.y);
				GCodeFile.GetFirstNumberAfter("Z", lineBeingSent, ref printerMove.position.z);
				GCodeFile.GetFirstNumberAfter("E", lineBeingSent, ref printerMove.extrusion);
				GCodeFile.GetFirstNumberAfter("F", lineBeingSent, ref printerMove.feedRate);
				if (movementMode == PrinterMachineInstruction.MovementTypes.Relative)
				{
					ref Vector3 position = ref printerMove.position;
					position += currentDestination.position;
				}
				if (currentDestination.position != printerMove.position)
				{
					currentDestination = printerMove;
					DestinationChanged.CallEvents((object)this, (EventArgs)null);
				}
			}
		}

		private void CreateStreamProcessors(string gcodeFilename, bool recoveryEnabled)
		{
			totalGCodeStream?.Dispose();
			GCodeStream gCodeStream = null;
			if (gcodeFilename != null)
			{
				loadedGCode = GCodeFile.Load(gcodeFilename);
				gCodeFileStream0 = new GCodeFileStream(loadedGCode);
				if (ActiveSliceSettings.Instance.GetValue<bool>("recover_is_enabled") && activePrintTask != null)
				{
					pauseHandlingStream1 = new PauseHandlingStream(new PrintRecoveryStream(gCodeFileStream0, activePrintTask.PercentDone), loadedGCode.PrintTools);
					activePrintTask.RecoveryCount++;
					activePrintTask.Commit();
				}
				else
				{
					pauseHandlingStream1 = new PauseHandlingStream(gCodeFileStream0, loadedGCode.PrintTools);
				}
				gCodeStream = pauseHandlingStream1;
			}
			else
			{
				gCodeStream = new NotPrintingStream();
			}
			queuedCommandStream2 = new QueuedCommandsStream(gCodeStream);
			relativeToAbsoluteStream3 = new RelativeToAbsoluteStream(queuedCommandStream2);
			printLevelingStream4 = new PrintLevelingStream(relativeToAbsoluteStream3, activePrinting: true);
			waitForTempStream5 = new WaitForTempStream(printLevelingStream4);
			babyStepsStream6 = new BabyStepsStream(waitForTempStream5);
			extrusionMultiplyerStream7 = new ExtrusionMultiplyerStream(babyStepsStream6);
			feedrateMultiplyerStream8 = new FeedRateMultiplyerStream(extrusionMultiplyerStream7);
			requestTemperaturesStream9 = new RequestTemperaturesStream(feedrateMultiplyerStream8);
			processWriteRegExStream10 = new ProcessWriteRegexStream(requestTemperaturesStream9, queuedCommandStream2);
			totalGCodeStream = processWriteRegExStream10;
			ReadPosition();
		}

		private void LoadGCodeToPrint(string gcodeFilename)
		{
			CreateStreamProcessors(gcodeFilename, ActiveSliceSettings.Instance.GetValue<bool>("recover_is_enabled"));
		}

		private void DoneLoadingGCodeToPrint()
		{
			CommunicationStates communicationStates = communicationState;
			if (communicationStates != CommunicationStates.Connected && communicationStates == CommunicationStates.PreparingToPrint)
			{
				if (ActivePrintItem.PrintItem.Id == 0)
				{
					ActivePrintItem.PrintItem.Commit();
				}
				if (activePrintTask == null)
				{
					activePrintTask = new PrintTask();
					activePrintTask.PrintStart = DateTime.Now;
					activePrintTask.PrinterId = ActivePrinter.ID.GetHashCode();
					activePrintTask.PrintName = ActivePrintItem.PrintItem.Name;
					activePrintTask.PrintItemId = ActivePrintItem.PrintItem.Id;
					activePrintTask.PrintingGCodeFileName = ActivePrintItem.GetGCodePathAndFileName();
					activePrintTask.PrintComplete = false;
					activePrintTask.Commit();
				}
				CommunicationState = CommunicationStates.Printing;
			}
		}

		private void MovementWasSetToAbsoluteMode(object sender, EventArgs e)
		{
			movementMode = PrinterMachineInstruction.MovementTypes.Absolute;
		}

		private void MovementWasSetToRelativeMode(object sender, EventArgs e)
		{
			movementMode = PrinterMachineInstruction.MovementTypes.Relative;
		}

		private void AtxPowerUpWasWritenToPrinter(object sender, EventArgs e)
		{
			OnAtxPowerStateChanged(enableAtxPower: true);
		}

		private void AtxPowerDownWasWritenToPrinter(object sender, EventArgs e)
		{
			OnAtxPowerStateChanged(enableAtxPower: false);
		}

		private void OnActivePrintItemChanged(EventArgs e)
		{
			ActivePrintItemChanged.CallEvents((object)this, e);
		}

		private void OnBedTemperatureRead(EventArgs e)
		{
			BedTemperatureRead.CallEvents((object)this, e);
		}

		private void OnBedTemperatureSet(EventArgs e)
		{
			BedTemperatureSet.CallEvents((object)this, e);
		}

		private void onConfirmPrint(bool messageBoxResponse)
		{
			if (messageBoxResponse)
			{
				CommunicationState = CommunicationStates.PreparingToPrint;
				activePrintItem.SlicingDone += partToPrint_SliceDone;
				partToPrint_SliceDone(activePrintItem, null);
			}
		}

		private void OnEnabledChanged(EventArgs e)
		{
			EnableChanged.CallEvents((object)this, e);
		}

		private void OnExtruderTemperatureRead(EventArgs e)
		{
			ExtruderTemperatureRead.CallEvents((object)this, e);
		}

		private void OnExtruderTemperatureSet(EventArgs e)
		{
			ExtruderTemperatureSet.CallEvents((object)this, e);
		}

		private void OnFanSpeedSet(EventArgs e)
		{
			FanSpeedSet.CallEvents((object)this, e);
		}

		private void OnFirmwareVersionRead(EventArgs e)
		{
			FirmwareVersionRead.CallEvents((object)this, e);
		}

		private void onRemoveMessageConfirm(bool messageBoxResponse)
		{
			if (messageBoxResponse)
			{
				QueueData.Instance.RemoveAt(QueueData.Instance.SelectedIndex);
			}
		}

		private bool IsNetworkPrinting()
		{
			return ActiveSliceSettings.Instance.GetValue<bool>("enable_network_printing");
		}

		private void OnAtxPowerStateChanged(bool enableAtxPower)
		{
			atxPowerIsOn = enableAtxPower;
			AtxPowerStateChanged.CallEvents((object)this, (EventArgs)null);
		}

		private void partToPrint_SliceDone(object sender, EventArgs e)
		{
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Expected O, but got Unknown
			PrintItemWrapper printItemWrapper = sender as PrintItemWrapper;
			if (printItemWrapper == null)
			{
				return;
			}
			printItemWrapper.SlicingDone -= partToPrint_SliceDone;
			string gCodePathAndFileName = printItemWrapper.GetGCodePathAndFileName();
			if (!(gCodePathAndFileName != ""))
			{
				return;
			}
			bool flag = Path.GetExtension(printItemWrapper.FileLocation)!.ToUpper() == ".GCODE";
			if (File.Exists(gCodePathAndFileName))
			{
				if (flag)
				{
					StartPrint(gCodePathAndFileName);
					return;
				}
				int num = 32000;
				using Stream stream = new FileStream(gCodePathAndFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				byte[] array = new byte[num];
				stream.Seek(Math.Max(0L, stream.Length - num), SeekOrigin.Begin);
				stream.Read(array, 0, num);
				stream.Close();
				if (Encoding.UTF8.GetString(array).Contains("filament used"))
				{
					if (firmwareUriGcodeSend)
					{
						currentSdBytes = 0.0;
						ClearQueuedGCode();
						SendLineToPrinterNow(StringHelper.FormatWith("M23 {0}", new object[1]
						{
							gCodePathAndFileName
						}));
						SendLineToPrinterNow("M24");
						CommunicationState = CommunicationStates.PrintingFromSd;
						((FoundStringCallbacks)ReadLineStartCallBacks).AddCallbackToKey("Done printing file", new FoundStringEventHandler(DonePrintingSdFile));
					}
					else
					{
						StartPrint(gCodePathAndFileName);
					}
					return;
				}
			}
			CommunicationState = CommunicationStates.Connected;
		}

		private void SetDetailedPrintingState(string lineBeingSetToPrinter)
		{
			if (lineBeingSetToPrinter.StartsWith("G28"))
			{
				PrintingState = DetailedPrintingState.HomingAxis;
			}
			else if (waitForTempStream5?.HeatingBed ?? false)
			{
				PrintingState = DetailedPrintingState.HeatingBed;
			}
			else if (waitForTempStream5?.HeatingExtruder ?? false)
			{
				PrintingState = DetailedPrintingState.HeatingExtruder;
			}
			else
			{
				PrintingState = DetailedPrintingState.Printing;
			}
		}

		private int ExpectedWaitSeconds(string lastInstruction)
		{
			if (lastInstruction.Contains("G0 ") || lastInstruction.Contains("G1 "))
			{
				return 2;
			}
			return 10;
		}

		private void TryWriteNextLineFromGCodeFile()
		{
			if (totalGCodeStream == null)
			{
				return;
			}
			if (timeHaveBeenWaitingForOK.get_IsRunning())
			{
				lock (locker)
				{
					if (currentSentLine != null)
					{
						if ((!(timeSinceLastReadAnything.get_Elapsed().TotalSeconds > 10.0) || !(timeSinceLastWrite.get_Elapsed().TotalSeconds > 30.0)) && !(timeHaveBeenWaitingForOK.get_Elapsed().TotalSeconds > (double)ExpectedWaitSeconds(currentSentLine)))
						{
							return;
						}
						currentLineIndexToSend--;
					}
				}
			}
			lock (locker)
			{
				if (currentLineIndexToSend < allCheckSumLinesSent.Count)
				{
					WriteRawToPrinter(allCheckSumLinesSent[currentLineIndexToSend++] + "\n", "resend");
					return;
				}
				int num = 60000;
				if (waitingForPosition.get_IsRunning() && waitingForPosition.get_ElapsedMilliseconds() < num && PrinterIsConnected)
				{
					return;
				}
				previousSentLine = currentSentLine;
				currentSentLine = totalGCodeStream.ReadLine();
				if (currentSentLine != null)
				{
					if (currentSentLine.Contains("M114") && PrinterIsConnected)
					{
						waitingForPosition.Restart();
					}
					double totalSeconds = timeSinceStartedPrint.get_Elapsed().TotalSeconds;
					if (secondsSinceUpdateHistory > totalSeconds || secondsSinceUpdateHistory + 1.0 < totalSeconds)
					{
						double num2 = loadedGCode.PercentComplete(gCodeFileStream0.LineIndex);
						if (activePrintTask != null && babyStepsStream6 != null && activePrintTask.PercentDone < num2)
						{
							activePrintTask.PercentDone = num2;
							try
							{
								Task.Run(delegate
								{
									activePrintTask.Commit();
								});
							}
							catch
							{
							}
						}
						secondsSinceUpdateHistory = totalSeconds;
					}
					currentSentLine = currentSentLine.Trim();
					if (currentSentLine.Split(new char[1]
					{
						';'
					})[0].Trim().Length > 0)
					{
						if (currentSentLine.Length > 0)
						{
							WriteChecksumLineToPrinter(currentSentLine);
							currentLineIndexToSend++;
						}
					}
					else if (!string.IsNullOrEmpty(currentSentLine))
					{
						PrinterOutputCache.Instance.PrinterLines.Add(currentSentLine);
					}
				}
				else if (PrintWasCanceled)
				{
					CommunicationState = CommunicationStates.Connected;
					ReleaseMotors();
					TurnOffBedAndExtruders();
					PrintWasCanceled = false;
				}
				else if (communicationState == CommunicationStates.Printing)
				{
					CommunicationState = CommunicationStates.FinishedPrint;
					PrintJobName = null;
					CreateStreamProcessors(null, recoveryEnabled: false);
					ReleaseMotors();
					TurnOffBedAndExtruders();
				}
			}
		}

		private void TurnOffBedAndExtruders()
		{
			for (int i = 0; i < ActiveSliceSettings.Instance.GetValue<int>("extruder_count"); i++)
			{
				SetTargetExtruderTemperature(i, 0.0, forceSend: true);
			}
			TargetBedTemperature = 0.0;
		}

		private void WriteChecksumLineToPrinter(string lineToWrite)
		{
			SetDetailedPrintingState(lineToWrite);
			lineToWrite = RemoveCommentIfAny(lineToWrite);
			KeepTrackOfAbsolutePostionAndDestination(lineToWrite);
			string text;
			if (lineToWrite.StartsWith("M110"))
			{
				text = $"N1 {lineToWrite}";
				GCodeFile.GetFirstNumberAfter("N", lineToWrite, ref currentLineIndexToSend);
				allCheckSumLinesSent.SetStartingIndex(currentLineIndexToSend);
			}
			else
			{
				text = $"N{allCheckSumLinesSent.Count} {lineToWrite}";
				if (lineToWrite.StartsWith("M999"))
				{
					allCheckSumLinesSent.SetStartingIndex(1);
				}
			}
			string text2 = text + "*" + GCodeFile.CalculateChecksum(text);
			allCheckSumLinesSent.Add(text2);
			WriteRawToPrinter(text2 + "\n", lineToWrite);
		}

		private static string RemoveCommentIfAny(string lineToWrite)
		{
			int num = lineToWrite.IndexOf(';');
			if (num > 0)
			{
				lineToWrite = lineToWrite.Substring(0, num).Trim();
			}
			return lineToWrite;
		}

		private void WriteRawToPrinter(string lineToWrite, string lineWithoutChecksum)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Expected O, but got Unknown
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Expected O, but got Unknown
			if (!PrinterIsConnected && CommunicationState != CommunicationStates.AttemptingToConnect)
			{
				return;
			}
			if (serialPort != null && serialPort.IsOpen)
			{
				FoundStringEventArgs val = new FoundStringEventArgs(lineWithoutChecksum);
				if (lineWithoutChecksum.StartsWith("G28") || lineWithoutChecksum.StartsWith("G29") || lineWithoutChecksum.StartsWith("G30") || lineWithoutChecksum.StartsWith("G92") || (lineWithoutChecksum.StartsWith("T") && !lineWithoutChecksum.StartsWith("T:")))
				{
					ReadPosition(forceToTopOfQueue: true);
				}
				StringEventArgs val2 = new StringEventArgs(lineToWrite);
				if (PrinterIsPrinting)
				{
					string text = lineToWrite.TrimEnd(Array.Empty<char>());
					CommunicationUnconditionalToPrinter.CallEvents((object)this, (EventArgs)new StringEventArgs(StringHelper.FormatWith("{0} [{1:0.000}]\n", new object[2]
					{
						text,
						timeSinceStartedPrint.get_Elapsed().TotalSeconds
					})));
				}
				else
				{
					CommunicationUnconditionalToPrinter.CallEvents((object)this, (EventArgs)(object)val2);
				}
				if (lineWithoutChecksum != null)
				{
					WriteLineStartCallBacks.CheckForKeys((EventArgs)(object)val);
					WriteLineContainsCallBacks.CheckForKeys((EventArgs)(object)val);
					if (val.get_SendToDelegateFunctions())
					{
						WroteLine.CallEvents((object)this, (EventArgs)(object)val2);
					}
				}
				try
				{
					lock (locker)
					{
						serialPort.Write(lineToWrite);
						timeSinceLastWrite.Restart();
						timeHaveBeenWaitingForOK.Restart();
					}
				}
				catch (IOException ex)
				{
					PrinterOutputCache.Instance.WriteLine("Exception:" + ex.Message);
					if (CommunicationState == CommunicationStates.AttemptingToConnect)
					{
						AbortConnectionAttempt("Connection Lost - " + ex.Message);
					}
				}
				catch (TimeoutException ex2)
				{
					PrinterOutputCache.Instance.WriteLine("        Error writing command:" + ex2.Message);
				}
				catch (UnauthorizedAccessException ex3)
				{
					PrinterOutputCache.Instance.WriteLine("Exception:" + ex3.Message);
					AbortConnectionAttempt(ex3.Message);
				}
				catch (Exception)
				{
				}
			}
			else
			{
				OnConnectionFailed(null);
			}
		}

		public void MacroStart()
		{
			queuedCommandStream2?.Reset();
		}

		public void MacroCancel()
		{
			waitForTempStream5?.Cancel();
			queuedCommandStream2?.Cancel();
		}

		public void MacroContinue()
		{
			queuedCommandStream2?.Continue();
		}
	}
}
