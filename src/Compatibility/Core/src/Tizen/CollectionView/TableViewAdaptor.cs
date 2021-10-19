using System.Collections;
using TSize = Tizen.UIExtensions.Common.Size;

namespace Microsoft.Maui.Controls.Compatibility.Platform.Tizen
{
	public class TableViewAdaptor : ItemTemplateAdaptor
	{
		new TableView Element { get; set; }

		public TableViewAdaptor(TableView tableView, IEnumerable items, DataTemplate template) : base(tableView, items, template)
		{
			Element = tableView;
		}

		protected override bool IsSelectable => true;

		public override TSize MeasureItem(double widthConstraint, double heightConstraint)
		{
			if (Element.RowHeight > 0)
			{
				return new TSize(widthConstraint, Element.RowHeight);
			}
			return MeasureItem(0, widthConstraint, heightConstraint);
		}

		public override TSize MeasureItem(int index, double widthConstraint, double heightConstraint)
		{
			if (index < 0 || index >= Count)
				return new TSize(0, 0);

			return base.MeasureItem(index, widthConstraint, heightConstraint);
		}
	}
}