using System;
using MatterHackers.Agg.Image;

namespace MatterHackers.MatterControl
{
	public class AllWhite
	{
		public static void DoAllWhite(ImageBuffer sourceImageAndDest)
		{
			DoAllWhite(sourceImageAndDest, sourceImageAndDest);
		}

		public static void DoAllWhite(ImageBuffer result, ImageBuffer imageA)
		{
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Expected O, but got Unknown
			if (imageA.get_BitDepth() != result.get_BitDepth())
			{
				throw new NotImplementedException("All the images have to be the same bit depth.");
			}
			if (imageA.get_Width() != result.get_Width() || imageA.get_Height() != result.get_Height())
			{
				throw new Exception("All images must be the same size.");
			}
			int bitDepth = imageA.get_BitDepth();
			if (bitDepth == 32)
			{
				int height = imageA.get_Height();
				int width = imageA.get_Width();
				byte[] buffer = result.GetBuffer();
				byte[] buffer2 = imageA.GetBuffer();
				for (int i = 0; i < height; i++)
				{
					int num = imageA.GetBufferOffsetY(i);
					int bufferOffsetY = result.GetBufferOffsetY(i);
					for (int j = 0; j < width; j++)
					{
						int num2 = buffer2[num + 3];
						if (num2 > 0)
						{
							buffer[bufferOffsetY++] = byte.MaxValue;
							num++;
							buffer[bufferOffsetY++] = byte.MaxValue;
							num++;
							buffer[bufferOffsetY++] = byte.MaxValue;
							num++;
							buffer[bufferOffsetY++] = (byte)num2;
							num++;
						}
						else
						{
							buffer[bufferOffsetY++] = 0;
							num++;
							buffer[bufferOffsetY++] = 0;
							num++;
							buffer[bufferOffsetY++] = 0;
							num++;
							buffer[bufferOffsetY++] = 0;
							num++;
						}
					}
				}
				result.SetRecieveBlender((IRecieveBlenderByte)new BlenderPreMultBGRA());
				return;
			}
			throw new NotImplementedException();
		}
	}
}
