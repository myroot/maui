using ElmSharp;
using ElmSharp.Wearable;

namespace Microsoft.Maui.Controls.Compatibility.Platform.Tizen.Native.Watch
{
	[System.Obsolete]
	public class WatchTableView : WatchListView, ITableView
	{
		static readonly SectionCellRenderer _sectionCellRenderer = new SectionCellRenderer();

		public WatchTableView(EvasObject parent, CircleSurface surface) : base(parent, surface)
		{
		}

		public void ApplyTableRoot(TableRoot root)
		{
			Clear();
			var cls = new PaddingItemClass();
			Append(cls, null);
			foreach (TableSection ts in root)
			{
				if (!string.IsNullOrEmpty(ts.Title))
					AddSectionTitle(ts.Title, ts.TextColor);
				AddSource(ts);
			}
			Append(cls, null);
		}

		protected override CellRenderer GetCellRenderer(Cell cell, bool isGroup = false)
		{
			if (cell.GetType() == typeof(SectionCell))
			{
				return _sectionCellRenderer;
			}
			return base.GetCellRenderer(cell, isGroup);
		}

		void AddSectionTitle(string title, Graphics.Color textColor)
		{
			Cell cell = new SectionCell()
			{
				Text = title,
				TextColor = textColor
			};
			AddCell(cell);
		}

		internal class SectionCellRenderer : TextCellRenderer
		{
			public SectionCellRenderer() : this(ThemeConstants.GenItemClass.Styles.GroupIndex)
			{
			}
			protected SectionCellRenderer(string style) : base(style) { }
		}
		class SectionCell : TextCell
		{
		}

		class PaddingItemClass : GenItemClass
		{
			public PaddingItemClass() : base(ThemeConstants.GenItemClass.Styles.Watch.Padding)
			{
			}
		}
	}
}
