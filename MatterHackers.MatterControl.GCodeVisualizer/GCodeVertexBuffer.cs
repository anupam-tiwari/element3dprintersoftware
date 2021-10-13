using System;
using MatterHackers.Agg.UI;
using MatterHackers.RenderOpenGl.OpenGl;

namespace MatterHackers.MatterControl.GCodeVisualizer
{
	public class GCodeVertexBuffer : IDisposable
	{
		public int myIndexId;

		public int myIndexLength;

		public BeginMode myMode = (BeginMode)4;

		public int myVertexId;

		public int myVertexLength;

		public GCodeVertexBuffer()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			GL.GenBuffers(1, ref myVertexId);
			GL.GenBuffers(1, ref myIndexId);
		}

		public void Dispose()
		{
			if (myVertexId != -1)
			{
				int holdVertexId = myVertexId;
				int holdIndexId = myIndexId;
				UiThread.RunOnIdle((Action)delegate
				{
					GL.DeleteBuffers(1, ref holdVertexId);
					GL.DeleteBuffers(1, ref holdIndexId);
				});
				myVertexId = -1;
			}
		}

		~GCodeVertexBuffer()
		{
			Dispose();
		}

		public void renderRange(int offset, int count)
		{
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			GL.EnableClientState((ArrayCap)32886);
			GL.EnableClientState((ArrayCap)32885);
			GL.EnableClientState((ArrayCap)32884);
			GL.DisableClientState((ArrayCap)32888);
			GL.Disable((EnableCap)3553);
			GL.EnableClientState((ArrayCap)32887);
			GL.BindBuffer((BufferTarget)34962, myVertexId);
			GL.BindBuffer((BufferTarget)34963, myIndexId);
			GL.ColorPointer(4, (ColorPointerType)5121, ColorVertexData.Stride, new IntPtr(0));
			GL.NormalPointer((NormalPointerType)5126, ColorVertexData.Stride, new IntPtr(4));
			GL.VertexPointer(3, (VertexPointerType)5126, ColorVertexData.Stride, new IntPtr(16));
			GL.DrawRangeElements(myMode, 0, myIndexLength, count, (DrawElementsType)5125, new IntPtr(offset * 4));
			GL.BindBuffer((BufferTarget)34962, 0);
			GL.BindBuffer((BufferTarget)34963, 0);
			GL.DisableClientState((ArrayCap)32887);
			GL.DisableClientState((ArrayCap)32884);
			GL.DisableClientState((ArrayCap)32885);
			GL.DisableClientState((ArrayCap)32886);
		}

		public void SetIndexData(int[] data)
		{
			SetIndexData(data, data.Length);
		}

		public unsafe void SetIndexData(int[] data, int count)
		{
			myIndexLength = count;
			GL.BindBuffer((BufferTarget)34963, myIndexId);
			fixed (int* value = data)
			{
				GL.BufferData((BufferTarget)34963, data.Length * 4, (IntPtr)value, (BufferUsageHint)35044);
			}
		}

		public void SetVertexData(ColorVertexData[] data)
		{
			SetVertexData(data, data.Length);
		}

		public unsafe void SetVertexData(ColorVertexData[] data, int count)
		{
			myVertexLength = count;
			GL.BindBuffer((BufferTarget)34962, myVertexId);
			fixed (ColorVertexData* value = data)
			{
				GL.BufferData((BufferTarget)34962, data.Length * ColorVertexData.Stride, (IntPtr)value, (BufferUsageHint)35044);
			}
		}
	}
}
