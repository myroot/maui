﻿using System;
using PlatformView = Microsoft.Maui.Platform.ContentCanvas;

namespace Microsoft.Maui.Handlers
{
	public class SwipeItemViewHandler : ViewHandler<ISwipeItemView, LayoutViewGroup>
	{
		IPlatformViewHandler? _contentHandler;

		protected override LayoutViewGroup CreatePlatformView()
		{
			var view = new LayoutViewGroup(VirtualView)
			{
				CrossPlatformMeasure = VirtualView.CrossPlatformMeasure,
				CrossPlatformArrange = VirtualView.CrossPlatformArrange
			};

			view.Show();
			return view;
		}

		public override void SetVirtualView(IView view)
		{
			base.SetVirtualView(view);
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = PlatformView ?? throw new InvalidOperationException($"{nameof(PlatformView)} should have been set by base class.");

			PlatformView.CrossPlatformMeasure = VirtualView.CrossPlatformMeasure;
			PlatformView.CrossPlatformArrange = VirtualView.CrossPlatformArrange;
		}

		void UpdateContent()
		{
			_ = PlatformView ?? throw new InvalidOperationException($"{nameof(PlatformView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			// Cleanup the old view when reused
			PlatformView.Children.Clear();
			_contentHandler?.Dispose();
			_contentHandler = null;

			if (VirtualView.PresentedContent is IView view)
			{
				PlatformView.Children.Add(view.ToPlatform(MauiContext));
				if (view.Handler is IPlatformViewHandler thandler)
				{
					_contentHandler = thandler;
				}
			}
		}

		public static void MapContent(ISwipeItemViewHandler handler, ISwipeItemView page)
		{
			if (handler is SwipeItemViewHandler platformHandler)
				platformHandler.UpdateContent();
		}

		public static void MapVisibility(ISwipeItemViewHandler handler, ISwipeItemView view)
		{
			//TODO : need to update
			//var swipeView = handler.PlatformView.GetParentOfType<EvasObject>();
			//if (swipeView != null)
			//	swipeView.UpdateIsVisibleSwipeItem(view);

			//handler.PlatformView.UpdateVisibility(view.Visibility);
		}
	}
}
