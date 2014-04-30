using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Microsoft.Win32;

namespace Common.Wpf.Extensions
{
    public static class FontExtensions
    {
        public static bool IsComposite(FontFamily fontFamily)
        {
            return fontFamily.Source.StartsWith("Global");
        }

        public static bool IsSymbol(FontFamily fontFamily)
        {
            Typeface typeface = fontFamily.GetTypefaces().First();
            GlyphTypeface glyph;
            typeface.TryGetGlyphTypeface(out glyph);
            return glyph.Symbol;
        }

        public static bool IsVisible(FontFamily fontFamily)
        {
            return !IsHidden(fontFamily);
        }

        public static bool IsHidden(FontFamily fontFamily)
        {
            const string fontManagementKey = @"Software\Microsoft\Windows NT\CurrentVersion\Font Management";
            const string inactiveFontsValue = "Inactive Fonts";

            RegistryKey key = Registry.CurrentUser.OpenSubKey(fontManagementKey);

            if (key == null)
                return false;

            IEnumerable<string> hiddenFonts = (string[]) key.GetValue(inactiveFontsValue);

            return hiddenFonts.Contains(fontFamily.Source);
        }
    }
}
