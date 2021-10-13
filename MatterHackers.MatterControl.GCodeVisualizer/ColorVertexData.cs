using System.Runtime.InteropServices;
using MatterHackers.Agg;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.GCodeVisualizer
{
	public struct ColorVertexData
	{
		public byte r;

		public byte g;

		public byte b;

		public byte a;

		public float normalX;

		public float normalY;

		public float normalZ;

		public float positionX;

		public float positionY;

		public float positionZ;

		public static readonly int Stride = Marshal.SizeOf(default(ColorVertexData));

		public ColorVertexData(Vector3 position, Vector3 normal, RGBA_Bytes color)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			r = (byte)((RGBA_Bytes)(ref color)).get_Red0To255();
			g = (byte)((RGBA_Bytes)(ref color)).get_Green0To255();
			b = (byte)((RGBA_Bytes)(ref color)).get_Blue0To255();
			a = (byte)((RGBA_Bytes)(ref color)).get_Alpha0To255();
			normalX = (float)normal.x;
			normalY = (float)normal.y;
			normalZ = (float)normal.z;
			positionX = (float)position.x;
			positionY = (float)position.y;
			positionZ = (float)position.z;
		}
	}
}
