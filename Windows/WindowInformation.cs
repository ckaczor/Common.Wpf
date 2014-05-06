using Common.Native;
using System;
using System.Drawing;

namespace Common.Wpf.Windows
{
    public class WindowInformation
    {
        public IntPtr Handle { get; private set; }
        public Rectangle Location { get; private set; }

        public WindowInformation(IntPtr handle)
        {
            Handle = handle;

            var windowPlacement = new Structures.WindowPlacement();
            Functions.User32.GetWindowPlacement(Handle, ref windowPlacement);

            var normalPosition = windowPlacement.NormalPosition;

            Location = new Rectangle(normalPosition.X, normalPosition.Y, normalPosition.Width, normalPosition.Height);
        }

        public WindowInformation(IntPtr handle, Rectangle location)
        {
            Handle = handle;
            Location = location;
        }
    }
}
