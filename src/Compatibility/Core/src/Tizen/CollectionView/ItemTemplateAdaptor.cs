using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Maui.Graphics;
using Tizen.UIExtensions.NUI;
using NView = Tizen.NUI.BaseComponents.View;
using TSize = Tizen.UIExtensions.Common.Size;
using XLabel = Microsoft.Maui.Controls.Label;

namespace Microsoft.Maui.Controls.Compatibility.Platform.Tizen
{
	public class CollectionViewSelectionChangedEventArgs : EventArgs
	{
		public IList<object> SelectedItems { get; set; }
	}

	[Obsolete]
	public class ItemTemplateAdaptor : ItemAdaptor
	{
		Dictionary<NView, View> _nativeFormsTable = new Dictionary<NView, View>();
		Dictionary<object, View> _dataBindedViewTable = new Dictionary<object, View>();
		protected View _headerCache;
		protected View _footerCache;

		public ItemTemplateAdaptor(ItemsView itemsView) : this(itemsView, itemsView.ItemsSource, itemsView.ItemTemplate) { }

		protected ItemTemplateAdaptor(Element itemsView, IEnumerable items, DataTemplate template) : base(items)
		{
			ItemTemplate = template;
			Element = itemsView;
			IsSelectable = itemsView is SelectableItemsView;
		}

		public event EventHandler<CollectionViewSelectionChangedEventArgs> SelectionChanged;

		protected DataTemplate ItemTemplate { get; set; }

		protected Element Element { get; set; }

		protected virtual bool IsSelectable { get; }

		public override void SendItemSelected(IEnumerable<int> selected)
		{
			var items = new List<object>();
			foreach (var item in selected)
			{
				items.Add(this[item]);
			}
			SelectionChanged?.Invoke(this, new CollectionViewSelectionChangedEventArgs
			{
				SelectedItems = items
			});
		}

		public override void UpdateViewState(NView view, ViewHolderState state)
		{
			base.UpdateViewState(view, state);
			if (_nativeFormsTable.TryGetValue(view, out View formsView))
			{
				switch (state)
				{
					case ViewHolderState.Focused:
						VisualStateManager.GoToState(formsView, VisualStateManager.CommonStates.Focused);
						formsView.SetValue(VisualElement.IsFocusedPropertyKey, true);
						break;
					case ViewHolderState.Normal:
						VisualStateManager.GoToState(formsView, VisualStateManager.CommonStates.Normal);
						formsView.SetValue(VisualElement.IsFocusedPropertyKey, false);
						break;
					case ViewHolderState.Selected:
						if (IsSelectable)
							VisualStateManager.GoToState(formsView, VisualStateManager.CommonStates.Selected);
						break;
				}
			}
		}

		public View GetTemplatedView(NView view)
		{
			return _nativeFormsTable[view];
		}

		public View GetTemplatedView(int index)
		{

			if (Count > index && _dataBindedViewTable.TryGetValue(this[index], out View view))
			{
				return view;
			}
			return null;
		}

		public override object GetViewCategory(int index)
		{
			if (ItemTemplate is DataTemplateSelector selector)
			{
				return selector.SelectTemplate(this[index], Element);
			}
			return base.GetViewCategory(index);
		}

		public override NView CreateNativeView(int index)
		{
			View view = null;
			if (ItemTemplate is DataTemplateSelector selector)
			{
				view = selector.SelectTemplate(this[index], Element).CreateContent() as View;
			}
			else
			{
				view = ItemTemplate.CreateContent() as View;
			}
			var renderer = Platform.GetOrCreateRenderer(view);
			var native = renderer.NativeView;

			view.Parent = Element;
			(renderer as ILayoutRenderer)?.RegisterOnLayoutUpdated();

			_nativeFormsTable[native] = view;
			return native;
		}

		public override NView CreateNativeView()
		{
			return CreateNativeView(0);
		}

		public override NView GetHeaderView()
		{
			_headerCache = CreateHeaderView();
			if (_headerCache != null)
			{
				_headerCache.Parent = Element;
				var renderer = Platform.GetOrCreateRenderer(_headerCache);
				(renderer as ILayoutRenderer)?.RegisterOnLayoutUpdated();
				return renderer.NativeView;
			}
			return null;
		}

		public override NView GetFooterView()
		{
			_footerCache = CreateFooterView();
			if (_footerCache != null)
			{
				_footerCache.Parent = Element;
				var renderer = Platform.GetOrCreateRenderer(_footerCache);
				(renderer as ILayoutRenderer)?.RegisterOnLayoutUpdated();
				return renderer.NativeView;
			}
			return null;
		}

		public override void RemoveNativeView(NView native)
		{
			UnBinding(native);
			if (_nativeFormsTable.TryGetValue(native, out View view))
			{
				Platform.GetRenderer(view)?.Dispose();
				_nativeFormsTable.Remove(native);
			}
		}

		public override void SetBinding(NView native, int index)
		{
			if (_nativeFormsTable.TryGetValue(native, out View view))
			{
				ResetBindedView(view);
				view.BindingContext = this[index];
				_dataBindedViewTable[this[index]] = view;
				view.MeasureInvalidated += OnItemMeasureInvalidated;

				AddLogicalChild(view);
			}
		}

		public override void UnBinding(NView native)
		{
			if (_nativeFormsTable.TryGetValue(native, out View view))
			{
				view.MeasureInvalidated -= OnItemMeasureInvalidated;
				ResetBindedView(view);
			}
		}

		public override TSize MeasureItem(double widthConstraint, double heightConstraint)
		{
			return MeasureItem(0, widthConstraint, heightConstraint);
		}

		public override TSize MeasureItem(int index, double widthConstraint, double heightConstraint)
		{
			if (index < 0 || index >= Count)
				return new TSize(0, 0);

			widthConstraint = Forms.ConvertToScaledDP(widthConstraint);
			heightConstraint = Forms.ConvertToScaledDP(heightConstraint);
			// TODO. It is hack code, it should be updated by Tizen.UIExtensions
			if (widthConstraint > heightConstraint)
				widthConstraint = double.PositiveInfinity;
			else
				heightConstraint = double.PositiveInfinity;

			if (_dataBindedViewTable.TryGetValue(this[index], out View createdView) && createdView != null)
			{
				return createdView.Measure(widthConstraint, heightConstraint, MeasureFlags.IncludeMargins).Request.ToPixel();
			}

			View view = null;
			if (ItemTemplate is DataTemplateSelector selector)
			{
				view = selector.SelectTemplate(this[index], Element).CreateContent() as View;
			}
			else
			{
				view = ItemTemplate.CreateContent() as View;
			}
			using (var renderer = Platform.GetOrCreateRenderer(view))
			{
				view.Parent = Element;
				if (Count > index)
					view.BindingContext = this[index];
				var request = view.Measure(widthConstraint, heightConstraint, MeasureFlags.IncludeMargins).Request;
				return request.ToPixel();
			}
		}

		public override TSize MeasureHeader(double widthConstraint, double heightConstraint)
		{
			return _headerCache?.Measure(Forms.ConvertToScaledDP(widthConstraint), Forms.ConvertToScaledDP(heightConstraint)).Request.ToPixel() ?? new TSize(0, 0);
		}

		public override TSize MeasureFooter(double widthConstraint, double heightConstraint)
		{
			return _footerCache?.Measure(Forms.ConvertToScaledDP(widthConstraint), Forms.ConvertToScaledDP(heightConstraint)).Request.ToPixel() ?? new TSize(0, 0);
		}

		protected virtual View CreateHeaderView()
		{
			if (Element is StructuredItemsView structuredItemsView)
			{
				if (structuredItemsView.Header != null)
				{
					View header = null;
					if (structuredItemsView.Header is View view)
					{
						header = view;
					}
					else if (structuredItemsView.HeaderTemplate != null)
					{
						header = structuredItemsView.HeaderTemplate.CreateContent() as View;
						header.BindingContext = structuredItemsView.Header;
					}
					else if (structuredItemsView.Header is String str)
					{
						header = new XLabel { Text = str, };
					}
					return header;
				}
			}
			return null;
		}

		protected virtual View CreateFooterView()
		{
			if (Element is StructuredItemsView structuredItemsView)
			{
				if (structuredItemsView.Footer != null)
				{
					View footer = null;
					if (structuredItemsView.Footer is View view)
					{
						footer = view;
					}
					else if (structuredItemsView.FooterTemplate != null)
					{
						footer = structuredItemsView.FooterTemplate.CreateContent() as View;
						footer.BindingContext = structuredItemsView.Footer;
					}
					else if (structuredItemsView.Footer is string str)
					{
						footer = new XLabel { Text = str, };
					}
					return footer;
				}
			}
			return null;
		}

		void ResetBindedView(View view)
		{
			if (view.BindingContext != null && _dataBindedViewTable.ContainsKey(view.BindingContext))
			{
				_dataBindedViewTable[view.BindingContext] = null;
				RemoveLogicalChild(view);
				view.BindingContext = null;
			}
		}

		void OnItemMeasureInvalidated(object sender, EventArgs e)
		{
			var data = (sender as View)?.BindingContext ?? null;
			int index = GetItemIndex(data);
			if (index != -1)
			{
				CollectionView?.ItemMeasureInvalidated(index);
			}
		}

		void AddLogicalChild(Element element)
		{
			if (Element is ItemsView iv)
			{
				iv.AddLogicalChild(element);
			}
			else
			{
				element.Parent = Element;
			}
		}

		void RemoveLogicalChild(Element element)
		{
			if (Element is ItemsView iv)
			{
				iv.RemoveLogicalChild(element);
			}
			else
			{
				element.Parent = null;
			}
		}

	}

	public class ItemDefaultTemplateAdaptor : ItemTemplateAdaptor
	{
		class ToTextConverter : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				return value?.ToString() ?? string.Empty;
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
		}

		public ItemDefaultTemplateAdaptor(ItemsView itemsView) : base(itemsView)
		{
			ItemTemplate = new DataTemplate(() =>
			{
				var label = new XLabel
				{
					TextColor = Colors.Black,
				};
				label.SetBinding(XLabel.TextProperty, new Binding(".", converter: new ToTextConverter()));

				return new Controls.StackLayout
				{
					BackgroundColor = Colors.White,
					Padding = 30,
					Children =
					{
						label
					}
				};
			});
		}
	}

	public class CarouselViewItemTemplateAdaptor : ItemTemplateAdaptor
	{
		public CarouselViewItemTemplateAdaptor(ItemsView itemsView) : base(itemsView) { }

		public override TSize MeasureItem(double widthConstraint, double heightConstraint)
		{
			return MeasureItem(0, widthConstraint, heightConstraint);
		}

		public override TSize MeasureItem(int index, double widthConstraint, double heightConstraint)
		{
			return (CollectionView as NView).Size.ToCommon();
		}
	}

	public class CarouselViewItemDefaultTemplateAdaptor : ItemDefaultTemplateAdaptor
	{
		public CarouselViewItemDefaultTemplateAdaptor(ItemsView itemsView) : base(itemsView) { }

		public override TSize MeasureItem(double widthConstraint, double heightConstraint)
		{
			return MeasureItem(0, widthConstraint, heightConstraint);
		}

		public override TSize MeasureItem(int index, double widthConstraint, double heightConstraint)
		{
			return (CollectionView as NView).Size.ToCommon();
		}
	}
}