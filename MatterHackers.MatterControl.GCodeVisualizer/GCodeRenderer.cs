using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.RenderOpenGl;
using MatterHackers.RenderOpenGl.OpenGl;

namespace MatterHackers.MatterControl.GCodeVisualizer
{
	public class GCodeRenderer : IDisposable
	{
		private List<List<int>> featureStartIndex = new List<List<int>>();

		private List<List<int>> featureEndIndex = new List<List<int>>();

		private List<List<RenderFeatureBase>> renderFeatures = new List<List<RenderFeatureBase>>();

		public static RGBA_Bytes ExtrusionColor = RGBA_Bytes.White;

		public static RGBA_Bytes TravelColor = RGBA_Bytes.Green;

		private GCodeFile gCodeFileToDraw;

		private ExtrusionColors extrusionColors;

		private List<GCodeVertexBuffer> layerVertexBuffer;

		private RenderType lastRenderType;

		public static double ExtruderWidth
		{
			get;
			set;
		} = 0.4;


		public GCodeFile GCodeFileToDraw => gCodeFileToDraw;

		public GCodeRenderer(GCodeFile gCodeFileToDraw)
		{
			if (gCodeFileToDraw != null)
			{
				this.gCodeFileToDraw = gCodeFileToDraw;
				for (int i = 0; i < gCodeFileToDraw.NumChangesInZ; i++)
				{
					renderFeatures.Add(new List<RenderFeatureBase>());
				}
			}
		}

		public void CreateFeaturesForLayerIfRequired(int layerToCreate)
		{
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			if (extrusionColors == null && gCodeFileToDraw != null && gCodeFileToDraw.LineCount > 0)
			{
				extrusionColors = new ExtrusionColors();
				HashSet<float> val = new HashSet<float>();
				PrinterMachineInstruction printerMachineInstruction = gCodeFileToDraw.Instruction(0);
				for (int i = 1; i < gCodeFileToDraw.LineCount; i++)
				{
					PrinterMachineInstruction printerMachineInstruction2 = gCodeFileToDraw.Instruction(i);
					if (printerMachineInstruction2.EPosition > printerMachineInstruction.EPosition && (printerMachineInstruction2.Line.IndexOf('X') != -1 || printerMachineInstruction2.Line.IndexOf('Y') != -1))
					{
						val.Add((float)printerMachineInstruction2.FeedRate);
					}
					printerMachineInstruction = printerMachineInstruction2;
				}
				Enumerator<float> enumerator = val.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						float current = enumerator.get_Current();
						extrusionColors.GetColorForSpeed(current);
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
			}
			if (renderFeatures.Count == 0 || renderFeatures[layerToCreate].Count > 0)
			{
				return;
			}
			List<RenderFeatureBase> list = renderFeatures[layerToCreate];
			int instructionIndexAtLayer = gCodeFileToDraw.GetInstructionIndexAtLayer(layerToCreate);
			int num = gCodeFileToDraw.LineCount - 1;
			if (layerToCreate < gCodeFileToDraw.NumChangesInZ - 1)
			{
				num = gCodeFileToDraw.GetInstructionIndexAtLayer(layerToCreate + 1);
			}
			for (int j = instructionIndexAtLayer; j < num; j++)
			{
				PrinterMachineInstruction printerMachineInstruction3 = gCodeFileToDraw.Instruction(j);
				PrinterMachineInstruction printerMachineInstruction4 = printerMachineInstruction3;
				if (j > 0)
				{
					printerMachineInstruction4 = gCodeFileToDraw.Instruction(j - 1);
				}
				if (printerMachineInstruction3.Position == printerMachineInstruction4.Position)
				{
					if (Math.Abs(printerMachineInstruction3.EPosition - printerMachineInstruction4.EPosition) > 0.0)
					{
						list.Add(new RenderFeatureRetract(printerMachineInstruction3.Position, printerMachineInstruction3.EPosition - printerMachineInstruction4.EPosition, printerMachineInstruction3.ExtruderIndex, printerMachineInstruction3.FeedRate));
					}
					if (printerMachineInstruction3.Line.StartsWith("G10"))
					{
						list.Add(new RenderFeatureRetract(printerMachineInstruction3.Position, -1.0, printerMachineInstruction3.ExtruderIndex, printerMachineInstruction3.FeedRate));
					}
					else if (printerMachineInstruction3.Line.StartsWith("G11"))
					{
						list.Add(new RenderFeatureRetract(printerMachineInstruction3.Position, 1.0, printerMachineInstruction3.ExtruderIndex, printerMachineInstruction3.FeedRate));
					}
				}
				else if (gCodeFileToDraw.Instruction(j).Line.StartsWith("G1"))
				{
					double layerHeight = gCodeFileToDraw.GetLayerHeight();
					if (layerToCreate == 0)
					{
						layerHeight = gCodeFileToDraw.GetFirstLayerHeight();
					}
					RGBA_Bytes colorForSpeed = extrusionColors.GetColorForSpeed((float)printerMachineInstruction3.FeedRate);
					double extrusionWidth = ((printerMachineInstruction3.ExtruderIndex < gCodeFileToDraw.PrintTools.get_Count()) ? gCodeFileToDraw.PrintTools.get_Item(printerMachineInstruction3.ExtruderIndex).get_Width() : ExtruderWidth);
					list.Add(new RenderFeatureExtrusion(printerMachineInstruction4.Position, printerMachineInstruction3.Position, printerMachineInstruction3.ExtruderIndex, j, extrusionWidth, printerMachineInstruction3.FeedRate, printerMachineInstruction3.EPosition - printerMachineInstruction4.EPosition, gCodeFileToDraw.GetFilamentDiameter(), layerHeight, colorForSpeed));
				}
				else
				{
					list.Add(new RenderFeatureTravel(printerMachineInstruction4.Position, printerMachineInstruction3.Position, printerMachineInstruction3.ExtruderIndex, printerMachineInstruction3.FeedRate));
				}
			}
		}

		public int GetNumFeatures(int layerToCountFeaturesOn)
		{
			CreateFeaturesForLayerIfRequired(layerToCountFeaturesOn);
			return renderFeatures[layerToCountFeaturesOn].Count;
		}

		public void Render(Graphics2D graphics2D, GCodeRenderInfo renderInfo)
		{
			if (renderFeatures.Count <= 0)
			{
				return;
			}
			CreateFeaturesForLayerIfRequired(renderInfo.EndLayerIndex);
			int count = renderFeatures[renderInfo.EndLayerIndex].Count;
			int val = (int)((double)count * renderInfo.FeatureToEndOnRatio0To1 + 0.5);
			val = Math.Max(0, Math.Min(val, count));
			int val2 = (int)((double)count * renderInfo.FeatureToStartOnRatio0To1 + 0.5);
			val2 = Math.Max(0, Math.Min(val2, count));
			if (val <= val2)
			{
				val = Math.Min(val2 + 1, count);
			}
			if (val2 >= val)
			{
				val2 = Math.Max(val - 1, 0);
			}
			Graphics2DOpenGL val3 = graphics2D as Graphics2DOpenGL;
			if (val3 != null)
			{
				val3.PreRender();
				GL.Begin((BeginMode)4);
				for (int i = val2; i < val; i++)
				{
					renderFeatures[renderInfo.EndLayerIndex][i]?.Render((Graphics2D)(object)val3, renderInfo);
				}
				GL.End();
				val3.PopOrthoProjection();
			}
			else
			{
				for (int j = val2; j < val; j++)
				{
					renderFeatures[renderInfo.EndLayerIndex][j]?.Render(graphics2D, renderInfo);
				}
			}
		}

		private void Create3DDataForLayer(int layerIndex, VectorPOD<ColorVertexData> colorVertexData, VectorPOD<int> vertexIndexArray, GCodeRenderInfo renderInfo)
		{
			colorVertexData.Clear();
			vertexIndexArray.Clear();
			featureStartIndex[layerIndex].Clear();
			featureEndIndex[layerIndex].Clear();
			for (int i = 0; i < renderFeatures[layerIndex].Count; i++)
			{
				featureStartIndex[layerIndex].Add(vertexIndexArray.get_Count());
				renderFeatures[layerIndex][i]?.CreateRender3DData(colorVertexData, vertexIndexArray, renderInfo);
				featureEndIndex[layerIndex].Add(vertexIndexArray.get_Count());
			}
		}

		public void Dispose()
		{
			Clear3DGCode();
		}

		public void Clear3DGCode(int layerIndex)
		{
			if (layerVertexBuffer != null && layerVertexBuffer[layerIndex] != null)
			{
				layerVertexBuffer[layerIndex].Dispose();
				layerVertexBuffer[layerIndex] = null;
			}
		}

		public void Clear3DGCode()
		{
			if (layerVertexBuffer == null)
			{
				return;
			}
			for (int i = 0; i < layerVertexBuffer.Count; i++)
			{
				if (layerVertexBuffer[i] != null)
				{
					layerVertexBuffer[i].Dispose();
					layerVertexBuffer[i] = null;
				}
			}
		}

		private static bool Is32Bit()
		{
			if (IntPtr.Size == 4)
			{
				return true;
			}
			return false;
		}

		public void Render3D(GCodeRenderInfo renderInfo)
		{
			if (layerVertexBuffer == null)
			{
				layerVertexBuffer = new List<GCodeVertexBuffer>();
				layerVertexBuffer.Capacity = gCodeFileToDraw.NumChangesInZ;
				for (int i = 0; i < gCodeFileToDraw.NumChangesInZ; i++)
				{
					layerVertexBuffer.Add(null);
					featureStartIndex.Add(new List<int>());
					featureEndIndex.Add(new List<int>());
				}
			}
			for (int j = 0; j < gCodeFileToDraw.NumChangesInZ; j++)
			{
				CreateFeaturesForLayerIfRequired(j);
			}
			if (lastRenderType != renderInfo.CurrentRenderType)
			{
				Clear3DGCode();
				lastRenderType = renderInfo.CurrentRenderType;
			}
			if (renderFeatures.Count <= 0)
			{
				return;
			}
			if (Is32Bit() && !GL.get_GlHasBufferObjects())
			{
				int num = 125000;
				int num2 = 0;
				bool flag = false;
				int num3 = renderInfo.EndLayerIndex - 1;
				while (num3 >= renderInfo.StartLayerIndex)
				{
					if (num2 + renderFeatures[num3].Count < num)
					{
						num2 += renderFeatures[num3].Count;
						num3--;
						continue;
					}
					renderInfo.startLayerIndex = num3 + 1;
					flag = true;
					break;
				}
				if (flag)
				{
					for (int k = 0; k < layerVertexBuffer.Count; k++)
					{
						if ((k < renderInfo.StartLayerIndex || k >= renderInfo.EndLayerIndex) && layerVertexBuffer[k] != null)
						{
							layerVertexBuffer[k].Dispose();
							layerVertexBuffer[k] = null;
						}
					}
				}
			}
			for (int num4 = renderInfo.EndLayerIndex - 1; num4 >= renderInfo.StartLayerIndex; num4--)
			{
				if (layerVertexBuffer[num4] == null)
				{
					VectorPOD<ColorVertexData> val = new VectorPOD<ColorVertexData>();
					VectorPOD<int> val2 = new VectorPOD<int>();
					Create3DDataForLayer(num4, val, val2, renderInfo);
					layerVertexBuffer[num4] = new GCodeVertexBuffer();
					layerVertexBuffer[num4].SetVertexData(val.get_Array());
					layerVertexBuffer[num4].SetIndexData(val2.get_Array());
				}
			}
			GL.Disable((EnableCap)3553);
			GL.PushAttrib((AttribMask)8192);
			GL.DisableClientState((ArrayCap)32888);
			GL.Enable((EnableCap)2881);
			if (renderInfo.EndLayerIndex - 1 > renderInfo.StartLayerIndex)
			{
				for (int l = renderInfo.StartLayerIndex; l < renderInfo.EndLayerIndex - 1; l++)
				{
					int count = renderFeatures[l].Count;
					if (count > 1)
					{
						layerVertexBuffer[l].renderRange(0, featureEndIndex[l][count - 1]);
					}
				}
			}
			int index = renderInfo.EndLayerIndex - 1;
			int count2 = renderFeatures[index].Count;
			int val3 = (int)((double)count2 * renderInfo.FeatureToStartOnRatio0To1 + 0.5);
			val3 = Math.Max(0, Math.Min(val3, count2));
			int val4 = (int)((double)count2 * renderInfo.FeatureToEndOnRatio0To1 + 0.5);
			val4 = Math.Max(0, Math.Min(val4, count2));
			if (val4 <= val3)
			{
				val4 = Math.Min(val3 + 1, count2);
			}
			if (val3 >= val4)
			{
				val3 = Math.Max(val4 - 1, 0);
			}
			if (val4 > val3)
			{
				int count3 = featureEndIndex[index][val4 - 1] - featureStartIndex[index][val3];
				layerVertexBuffer[index].renderRange(featureStartIndex[index][val3], count3);
			}
			GL.PopAttrib();
		}
	}
}
