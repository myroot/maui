﻿using Tizen.UIExtensions.Common;
using EColor = ElmSharp.Color;

namespace Microsoft.Maui.Handlers
{
	public partial class PageHandler : ContentViewHandler
	{
		public static void MapBackground(IPageHandler handler, IContentView page)
		{
			handler.UpdateValue(nameof(handler.ContainerView));
			if (page.Background != null && handler.PlatformView.BackgroundColor != EColor.Transparent)
			{
				handler.PlatformView.BackgroundColor = EColor.Transparent;
			}
			handler.ToPlatform()?.UpdateBackground(page);
		}

		[MissingMapper]
		public static void MapTitle(IPageHandler handler, IContentView page) { }

		protected override ContentCanvas CreatePlatformView()
		{
			var view = base.CreatePlatformView();
			view.BackgroundColor = (DeviceInfo.DeviceType == DeviceType.TV) ? EColor.Transparent : EColor.White;

			return view;
		}

		void UpdateContent()
		{
			_ = NativeView ?? throw new InvalidOperationException($"{nameof(NativeView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			NativeView.Children.Add(VirtualView.Content.ToNative(MauiContext, false));
			// TODO : Fix me later
			//if (VirtualView.Content.Handler is INativeViewHandler thandler)
			//{
			//	thandler?.SetParent(this);
			//}
		}
	}
}