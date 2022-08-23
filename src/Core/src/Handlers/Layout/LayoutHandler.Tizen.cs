﻿using System;
using System.Linq;
using NView = Tizen.NUI.BaseComponents.View;

namespace Microsoft.Maui.Handlers
{
	public partial class LayoutHandler : ViewHandler<ILayout, LayoutViewGroup>
	{
		public override bool NeedsContainer =>
			VirtualView?.Background != null ||
			VirtualView?.Clip != null ||
			base.NeedsContainer;

		protected override LayoutViewGroup CreatePlatformView()
		{
			if (VirtualView == null)
			{
				throw new InvalidOperationException($"{nameof(VirtualView)} must be set to create a Canvas");
			}

			var view = new LayoutViewGroup(VirtualView)
			{
				CrossPlatformMeasure = VirtualView.CrossPlatformMeasure,
				CrossPlatformArrange = VirtualView.CrossPlatformArrange
			};

			view.Show();
			return view;
		}

		public override Graphics.Size GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return VirtualView.CrossPlatformMeasure(widthConstraint, heightConstraint);
		}

		public override void SetVirtualView(IView view)
		{
			base.SetVirtualView(view);

			_ = PlatformView ?? throw new InvalidOperationException($"{nameof(PlatformView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			PlatformView.CrossPlatformMeasure = VirtualView.CrossPlatformMeasure;
			PlatformView.CrossPlatformArrange = VirtualView.CrossPlatformArrange;

			PlatformView.Children.Clear();

			foreach (var child in VirtualView.OrderByZIndex())
			{
				PlatformView.Children.Add(child.ToPlatform(MauiContext));
			}
		}

		public void Add(IView child)
		{
			_ = PlatformView ?? throw new InvalidOperationException($"{nameof(PlatformView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			var targetIndex = VirtualView.GetLayoutHandlerIndex(child);
			PlatformView.Children.Insert(targetIndex, child.ToPlatform(MauiContext));
		}

		public void Remove(IView child)
		{
			_ = PlatformView ?? throw new InvalidOperationException($"{nameof(PlatformView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");

			if (child.Handler is IPlatformViewHandler thandler && child?.ToPlatform() is NView childView)
			{
				PlatformView.Children.Remove(childView);
				thandler.Dispose();
			}
		}

		public void Clear()
		{
			if (PlatformView == null)
				return;

			var children = PlatformView.Children.ToList();
			PlatformView.Children.Clear();
			foreach (var child in children)
			{
				child.Dispose();
			}
		}

		public void Insert(int index, IView child)
		{
			_ = PlatformView ?? throw new InvalidOperationException($"{nameof(PlatformView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			var targetIndex = VirtualView.GetLayoutHandlerIndex(child);
			PlatformView.Children.Insert(targetIndex, child.ToPlatform(MauiContext));
		}

		public void Update(int index, IView child)
		{
			_ = PlatformView ?? throw new InvalidOperationException($"{nameof(PlatformView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			var toBeRemoved = PlatformView.Children[index];
			PlatformView.Children.RemoveAt(index);
			toBeRemoved.Dispose();

			var targetIndex = VirtualView.GetLayoutHandlerIndex(child);
			PlatformView.Children.Insert(targetIndex, child.ToPlatform(MauiContext));
		}

		public void UpdateZIndex(IView child)
		{
			_ = PlatformView ?? throw new InvalidOperationException($"{nameof(PlatformView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			EnsureZIndexOrder(child);
		}

		void EnsureZIndexOrder(IView child)
		{
			if (PlatformView.Children.Count == 0)
			{
				return;
			}

			var platformChildView = child.ToPlatform(MauiContext!);
			var currentIndex = PlatformView.Children.IndexOf(platformChildView);

			if (currentIndex == -1)
			{
				return;
			}

			var targetIndex = VirtualView.GetLayoutHandlerIndex(child);
			if (targetIndex > currentIndex)
			{
				child.ToPlatform(MauiContext!).RaiseTop();
				for (int i = targetIndex + 1; i < PlatformView.Children.Count; i++)
				{
					PlatformView.Children[i].RaiseTop();
				}
			}
			else
			{
				child.ToPlatform(MauiContext!).Lower();
				for (int i = targetIndex - 1; i >= 0; i--)
				{
					PlatformView.Children[i].Lower();
				}
			}
		}

		public void Update(int index, IView child)
		{
			_ = NativeView ?? throw new InvalidOperationException($"{nameof(NativeView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			var toBeRemoved = PlatformView.Children[index];
			PlatformView.Children.RemoveAt(index);
			toBeRemoved.Unrealize();

			var targetIndex = VirtualView.GetLayoutHandlerIndex(child);
			PlatformView.Children.Insert(targetIndex, child.ToPlatform(MauiContext));
			if (child.Handler is IPlatformViewHandler childHandler)
			{
				childHandler?.SetParent(this);
			}
		}

		public void UpdateZIndex(IView child)
		{
			_ = PlatformView ?? throw new InvalidOperationException($"{nameof(PlatformView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			EnsureZIndexOrder(child);
		}

		void EnsureZIndexOrder(IView child)
		{
			if (PlatformView.Children.Count == 0)
			{
				return;
			}

			var nativeChildView = child.ToPlatform(MauiContext!);
			var currentIndex = PlatformView.Children.IndexOf(nativeChildView);

			if (currentIndex == -1)
			{
				return;
			}

			var targetIndex = VirtualView.GetLayoutHandlerIndex(child);
			if (targetIndex > currentIndex)
			{
				child.ToPlatform(MauiContext!).RaiseTop();
				for (int i = targetIndex+1; i < PlatformView.Children.Count; i++)
				{
					PlatformView.Children[i].RaiseTop();
				}
			}
			else
			{
				child.ToPlatform(MauiContext!).Lower();
				for (int i = targetIndex-1; i >= 0; i--)
				{
					PlatformView.Children[i].Lower();
				}
			}
		}
	}
}
