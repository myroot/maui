﻿using Tizen.UIExtensions.Common;
using Tizen.UIExtensions.NUI;
using GColor = Microsoft.Maui.Graphics.Color;
using TReturnType = Tizen.UIExtensions.Common.ReturnType;
using TTextAlignment = Tizen.UIExtensions.Common.TextAlignment;

namespace Microsoft.Maui.Platform
{
	public static class EntryExtensions
	{
		public static void UpdateText(this Entry platformEntry, IText entry)
		{
			platformEntry.Text = entry.Text ?? "";
		}

		public static void UpdateTextColor(this Entry platformEntry, ITextStyle entry)
		{
			platformEntry.TextColor = entry.TextColor.ToPlatform();
		}

		public static void UpdateHorizontalTextAlignment(this Entry platformEntry, ITextAlignment entry)
		{
			platformEntry.HorizontalTextAlignment = entry.HorizontalTextAlignment.ToPlatform();
		}

		public static void UpdateVerticalTextAlignment(this Entry platformEntry, ITextAlignment entry)
		{
			platformEntry.VerticalTextAlignment = entry.VerticalTextAlignment.ToPlatform();
		}

		public static void UpdateIsPassword(this Entry platformEntry, IEntry entry)
		{
			platformEntry.IsPassword = entry.IsPassword;
		}

		public static void UpdateReturnType(this Entry platformEntry, IEntry entry)
		{
			platformEntry.ReturnType = entry.ReturnType.ToPlatform();
		}

		public static void UpdateFont(this Entry platformEntry, ITextStyle textStyle, IFontManager fontManager)
		{
			platformEntry.FontSize = textStyle.Font.Size > 0 ? textStyle.Font.Size.ToScaledPoint() : 25d.ToScaledPoint();
			platformEntry.FontAttributes = textStyle.Font.GetFontAttributes();
			platformEntry.FontFamily = fontManager.GetFontFamily(textStyle.Font.Family) ?? "";
		}

		public static void UpdatePlaceholder(this Entry platformEntry, ITextInput entry)
		{
			platformEntry.Placeholder = entry.Placeholder ?? string.Empty;
		}

		public static void UpdatePlaceholder(this Entry nativeEntry, string placeholder)
		{
			nativeEntry.Placeholder = placeholder;
		}

		public static void UpdatePlaceholderColor(this Entry nativeEntry, ITextInput entry)
		{
			platformEntry.PlaceholderColor = entry.PlaceholderColor.ToPlatform();
		}

		public static void UpdatePlaceholderColor(this Entry nativeEntry, GColor color)
		{
			nativeEntry.PlaceholderColor = color.ToNative();
		}

		public static void UpdateIsReadOnly(this Entry nativeEntry, ITextInput entry)
		{
			platformEntry.IsReadOnly = entry.IsReadOnly;
		}

		public static void UpdateIsTextPredictionEnabled(this Entry platformEntry, ITextInput entry)
		{
			platformEntry.IsTextPredictionEnabled = entry.IsTextPredictionEnabled;
		}

		public static void UpdateKeyboard(this Entry platformEntry, ITextInput entry)
		{
			
		}

		/* Updates both the IEntry.CursorPosition and IEntry.SelectionLength properties. */
		[PortHandler]
		public static void UpdateSelectionLength(this Entry platformEntry, ITextInput entry)
		{
		}

		public static TReturnType ToPlatform(this ReturnType returnType)
		{
			switch (returnType)
			{
				case ReturnType.Go:
					return TReturnType.Go;
				case ReturnType.Next:
					return TReturnType.Next;
				case ReturnType.Send:
					return TReturnType.Send;
				case ReturnType.Search:
					return TReturnType.Search;
				case ReturnType.Done:
					return TReturnType.Done;
				case ReturnType.Default:
					return TReturnType.Default;
				default:
					throw new System.NotImplementedException($"ReturnType {returnType} not supported");
			}
		}

		public static TTextAlignment ToPlatform(this TextAlignment alignment)
		{
			switch (alignment)
			{
				case TextAlignment.Center:
					return TTextAlignment.Center;

				case TextAlignment.Start:
					return TTextAlignment.Start;

				case TextAlignment.End:
					return TTextAlignment.End;

				default:
					Log.Warn("Warning: unrecognized HorizontalTextAlignment value {0}. " +
						"Expected: {Start|Center|End}.", alignment);
					Log.Debug("Falling back to platform's default settings.");
					return TTextAlignment.Auto;
			}
		}
	}
}
