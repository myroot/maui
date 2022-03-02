using System.Collections;
using System.Collections.Generic;
using NView = Tizen.NUI.BaseComponents.View;
using TSize = Tizen.UIExtensions.Common.Size;
using XLabel = Microsoft.Maui.Controls.Label;

namespace Microsoft.Maui.Controls.Compatibility.Platform.Tizen
{
	[System.Obsolete]
	public class EmptyItemAdaptor : ItemTemplateAdaptor
	{
		static DataTemplate s_defaultEmptyTemplate = new DataTemplate(typeof(EmptyView));

		View _createdEmptyView;

		public EmptyItemAdaptor(ItemsView itemsView, IEnumerable items, DataTemplate template) : base(itemsView, items, template)
		{
		}

		public static EmptyItemAdaptor Create(ItemsView itemsView)
		{
			DataTemplate template = null;
			if (itemsView.EmptyView is View emptyView)
			{
				template = new DataTemplate(() =>
				{
					return emptyView;
				});
			}
			else
			{
				template = itemsView.EmptyViewTemplate ?? s_defaultEmptyTemplate;
			}

			return new EmptyItemAdaptor(itemsView, new List<object>(), template);
		}

		public override NView GetFooterView()
		{
			return null;
		}

		public override NView GetHeaderView()
		{
			View emptyView = ItemTemplate.CreateContent() as View;

			Platform.GetOrCreateRenderer(emptyView)?.Dispose();
			emptyView.Handler = null;
			if (emptyView != (Element as ItemsView)?.EmptyView)
				emptyView.BindingContext = (Element as ItemsView)?.EmptyView;

			var header = CreateHeaderView();
			var footer = CreateFooterView();

			var layout = new Controls.StackLayout
			{
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
			};

			if (header != null)
			{
				layout.Children.Add(header);
			}
			layout.Children.Add(emptyView);
			if (footer != null)
			{
				layout.Children.Add(footer);
			}

			layout.Parent = Element;
			_createdEmptyView = layout;
			var renderer = Platform.GetOrCreateRenderer(layout);
			(renderer as ILayoutRenderer)?.RegisterOnLayoutUpdated();
			return renderer.NativeView;
		}

		public override TSize MeasureHeader(double widthConstraint, double heightConstraint)
		{
			return _createdEmptyView?.Measure(widthConstraint, heightConstraint, MeasureFlags.IncludeMargins).Request.ToPixel() ?? new TSize(100, 100);
		}

		public override TSize MeasureItem(double widthConstraint, double heightConstraint)
		{
			return new TSize(widthConstraint, heightConstraint);
		}

		class EmptyView : Controls.StackLayout
		{
			public EmptyView()
			{
				HorizontalOptions = LayoutOptions.Fill;
				VerticalOptions = LayoutOptions.Fill;
				Children.Add(
					new XLabel
					{
						Text = "No items found",
						VerticalOptions = LayoutOptions.Center,
						HorizontalOptions = LayoutOptions.Center,
						HorizontalTextAlignment = TextAlignment.Center,
						VerticalTextAlignment = TextAlignment.Center,
					}
				);
			}
		}
	}
}