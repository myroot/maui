using System;
using Tizen.UIExtensions.NUI.GraphicsView;


namespace Microsoft.Maui.Handlers
{
	public partial class SwitchHandler : ViewHandler<ISwitch, Switch>
	{
		protected override Switch CreatePlatformView() => new Switch
		{
			Focusable = true,
		};

		protected override void ConnectHandler(Switch nativeView)
		{
			base.ConnectHandler(nativeView);
			nativeView.Toggled += OnStateChanged;
		}

		protected override void DisconnectHandler(Switch nativeView)
		{
			base.DisconnectHandler(nativeView);
			nativeView.Toggled -= OnStateChanged;
		}

		public static void MapIsOn(ISwitchHandler handler, ISwitch view)
		{
			handler.PlatformView?.UpdateIsOn(view);
		}

		public static void MapTrackColor(ISwitchHandler handler, ISwitch view)
		{
			handler.PlatformView?.UpdateTrackColor(view);
		}

		public static void MapThumbColor(ISwitchHandler handler, ISwitch view)
		{
			handler.PlatformView?.UpdateThumbColor(view);
		}

		void OnStateChanged(object? sender, EventArgs e)
		{
			if (VirtualView == null || PlatformView == null)
				return;

			VirtualView.IsOn = PlatformView.IsToggled;
		}
	}
}