using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.PolygonMesh;
using MatterHackers.RayTracer;

namespace MatterHackers.MatterControl.WellPlate
{
	public class WellPlatePart : GuiWidget
	{
		public Action ThumbnailGenerationDone;

		private ImageWidget thumbnailWidget;

		private MeshGroup originalMeshGroup;

		private MeshGroup newGroup;

		private WellPlateWidget wellPlateWidget;

		private bool selected;

		private RGBA_Bytes baseColor;

		public ImageBuffer OriginalImage
		{
			get;
			private set;
		}

		public MeshGroup MeshGroup => newGroup;

		public WellPlatePart(WellPlateWidget wellPlate, MeshGroup meshGroup)
			: this()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Expected O, but got Unknown
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Expected O, but got Unknown
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((GuiWidget)this).set_Margin(new BorderDouble(4.0));
			((GuiWidget)this).set_Padding(new BorderDouble(4.0));
			((GuiWidget)this).set_HAnchor((HAnchor)8);
			((GuiWidget)this).set_VAnchor((VAnchor)8);
			wellPlateWidget = wellPlate;
			originalMeshGroup = meshGroup;
			OriginalImage = new ImageBuffer();
			StaticData.get_Instance().LoadIcon("building_thumbnail_100x100.png", OriginalImage);
			thumbnailWidget = new ImageWidget(OriginalImage);
			((GuiWidget)thumbnailWidget).AnchorCenter();
			((GuiWidget)thumbnailWidget).add_Click((EventHandler<MouseEventArgs>)delegate(object sender, MouseEventArgs e)
			{
				((GuiWidget)this).OnClick(e);
			});
			((GuiWidget)this).AddChild((GuiWidget)(object)thumbnailWidget, -1);
		}

		public void MakeThumbnailNow()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			List<MeshGroup> list = new List<MeshGroup>();
			newGroup = new MeshGroup();
			list.Add(newGroup);
			foreach (Mesh mesh in originalMeshGroup.get_Meshes())
			{
				newGroup.get_Meshes().Add(Mesh.Copy(mesh, (ReportProgressRatio)null, true));
			}
			ThumbnailTracer thumbnailTracer = new ThumbnailTracer(list, 100, 100);
			thumbnailTracer.DoTrace();
			OriginalImage = thumbnailTracer.destImage;
			thumbnailWidget.set_Image(OriginalImage);
			((GuiWidget)this).Invalidate();
			ThumbnailGenerationDone?.Invoke();
		}

		public void ClearSelection()
		{
			selected = false;
		}

		public override void OnClick(MouseEventArgs mouseEvent)
		{
			((GuiWidget)this).OnClick(mouseEvent);
			selected = !selected;
			if (selected)
			{
				wellPlateWidget.SetSelectedWellPlatePart(this);
			}
			else
			{
				wellPlateWidget.SetSelectedWellPlatePart(null);
			}
			((GuiWidget)this).Invalidate();
		}

		public override void OnMouseEnterBounds(MouseEventArgs mouseEvent)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).OnMouseEnterBounds(mouseEvent);
			baseColor = ActiveTheme.get_Instance().get_TertiaryBackgroundColor();
			((GuiWidget)this).Invalidate();
		}

		public override void OnMouseLeaveBounds(MouseEventArgs mouseEvent)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).OnMouseLeaveBounds(mouseEvent);
			baseColor = ActiveTheme.get_Instance().get_SecondaryBackgroundColor();
			((GuiWidget)this).Invalidate();
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			if (selected && ((GuiWidget)this).get_BackgroundColor() != ActiveTheme.get_Instance().get_PrimaryBackgroundColor())
			{
				((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			}
			else if (!selected && ((GuiWidget)this).get_BackgroundColor() != baseColor)
			{
				((GuiWidget)this).set_BackgroundColor(baseColor);
			}
			((GuiWidget)this).OnDraw(graphics2D);
		}
	}
}
