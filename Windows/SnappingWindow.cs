using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

using Common.Native;

using Rectangle = System.Drawing.Rectangle;
using Screen = System.Windows.Forms.Screen;

namespace Common.Wpf.Windows
{
    public class SnappingWindow : Window
    {
        #region Member variables

        private HwndSource _hwndSource;
        private Structures.WindowPosition _lastWindowPosition;

        #endregion

        #region Enumerations

        private enum SnapMode
        {
            Move,
            Resize
        }

        #endregion

        #region Properties

        protected virtual int SnapDistance
        {
            get { return 20; }
        }

        protected virtual List<Rect> OtherWindows
        {
            get { return null; }
        }

        #endregion

        #region Window overrides

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Store the window handle
            _hwndSource = PresentationSource.FromVisual(this) as HwndSource;

            // If we failed to get the hwnd then don't bother
            if (_hwndSource == null)
                return;

            // Add a hook
            _hwndSource.AddHook(WndProc);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            // Initialize the last window position
            _lastWindowPosition = new Structures.WindowPosition
            {
                Left = (int) Left,
                Width = (int) Width,
                Height = (int) Height,
                Top = (int) Top
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Unhook the window procedure
            _hwndSource.RemoveHook(WndProc);
            _hwndSource.Dispose();
        }

        #endregion

        #region Window procedure

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == (int) Constants.WindowMessage.WindowPositionChanging)
                return OnWindowPositionChanging(lParam, ref handled);

            return IntPtr.Zero;
        }

        #endregion

        #region Snapping

        private IntPtr OnWindowPositionChanging(IntPtr lParam, ref bool handled)
        {
            int snapDistance = SnapDistance;

            // Initialize whether we've updated the position
            bool updated = false;

            // Convert the lParam into the current structure
            var windowPosition = (Structures.WindowPosition) Marshal.PtrToStructure(lParam, typeof(Structures.WindowPosition));

            // If the window flags indicate no movement then do nothing
            if ((windowPosition.Flags & Constants.WindowPositionFlags.NoMove) != 0)
                return IntPtr.Zero;

            // If nothing changed then do nothing
            if (_lastWindowPosition.IsSameLocationAndSize(windowPosition))
                return IntPtr.Zero;

            // Figure out if the window is being moved or resized
            SnapMode snapMode = (_lastWindowPosition.IsSameSize(windowPosition) ? SnapMode.Move : SnapMode.Resize);

            // Get the screen the cursor is currently on
            Screen screen = Screen.FromPoint(System.Windows.Forms.Cursor.Position);

            // Create a rectangle based on the current working area of the screen
            Rectangle snapToBorder = screen.WorkingArea;

            // Deflate the rectangle based on the snap distance
            snapToBorder.Inflate(-snapDistance, -snapDistance);

            if (snapMode == SnapMode.Resize)
            {
                // See if we need to snap on the left
                if (windowPosition.Left < snapToBorder.Left)
                {
                    windowPosition.Width += windowPosition.Left - screen.WorkingArea.Left;
                    windowPosition.Left = screen.WorkingArea.Left;

                    updated = true;
                }

                // See if we need to snap on the right
                if (windowPosition.Right > snapToBorder.Right)
                {
                    windowPosition.Width += (screen.WorkingArea.Right - windowPosition.Right);

                    updated = true;
                }

                // See if we need to snap to the top
                if (windowPosition.Top < snapToBorder.Top)
                {
                    windowPosition.Height += windowPosition.Top - screen.WorkingArea.Top;
                    windowPosition.Top = screen.WorkingArea.Top;

                    updated = true;
                }

                // See if we need to snap to the bottom
                if (windowPosition.Bottom > snapToBorder.Bottom)
                {
                    windowPosition.Height += (screen.WorkingArea.Bottom - windowPosition.Bottom);

                    updated = true;
                }
            }
            else
            {
                // See if we need to snap on the left
                if (windowPosition.Left < snapToBorder.Left)
                {
                    windowPosition.Left = screen.WorkingArea.Left;
                    updated = true;
                }

                // See if we need to snap on the top
                if (windowPosition.Top < snapToBorder.Top)
                {
                    windowPosition.Top = screen.WorkingArea.Top;
                    updated = true;
                }

                // See if we need to snap on the right
                if (windowPosition.Right > snapToBorder.Right)
                {
                    windowPosition.Left = (screen.WorkingArea.Right - windowPosition.Width);
                    updated = true;
                }

                // See if we need to snap on the bottom
                if (windowPosition.Bottom > snapToBorder.Bottom)
                {
                    windowPosition.Top = (screen.WorkingArea.Bottom - windowPosition.Height);
                    updated = true;
                }
            }

            var otherWindows = OtherWindows;

            if (otherWindows != null && otherWindows.Count > 0)
            {
                // Loop over all other windows looking to see if we should stick
                foreach (Rect otherWindow in otherWindows)
                {
                    // Get a rectangle with the bounds of the other window
                    var otherWindowRect = new Rectangle(Convert.ToInt32(otherWindow.Left), Convert.ToInt32(otherWindow.Top), Convert.ToInt32(otherWindow.Width), Convert.ToInt32(otherWindow.Height));

                    // Check the current window left against the other window right
                    var otherWindowSnapBorder = new Rectangle(otherWindowRect.Right, otherWindowRect.Top, snapDistance, otherWindowRect.Height);
                    var thisWindowSnapBorder = new Rectangle(windowPosition.Left, windowPosition.Top, 1, windowPosition.Height);

                    if (thisWindowSnapBorder.IntersectsWith(otherWindowSnapBorder))
                    {
                        windowPosition.Left = otherWindowRect.Right;
                        CheckSnapTopAndBottom(ref windowPosition, otherWindowRect, snapMode);
                        updated = true;
                    }

                    // Check the current window right against the other window left
                    otherWindowSnapBorder = new Rectangle(otherWindowRect.Left - snapDistance + 1, otherWindowRect.Top, snapDistance, otherWindowRect.Height);
                    thisWindowSnapBorder = new Rectangle(windowPosition.Right, windowPosition.Top, 1, windowPosition.Height);

                    if (thisWindowSnapBorder.IntersectsWith(otherWindowSnapBorder))
                    {
                        windowPosition.Left = otherWindowRect.Left - windowPosition.Width;
                        CheckSnapTopAndBottom(ref windowPosition, otherWindowRect, snapMode);
                        updated = true;
                    }

                    // Check the current window bottom against the other window top
                    otherWindowSnapBorder = new Rectangle(otherWindowRect.Left, otherWindowRect.Top - snapDistance + 1, otherWindowRect.Width, snapDistance);
                    thisWindowSnapBorder = new Rectangle(windowPosition.Left, windowPosition.Bottom, windowPosition.Width, 1);

                    if (thisWindowSnapBorder.IntersectsWith(otherWindowSnapBorder))
                    {
                        windowPosition.Top = otherWindowRect.Top - windowPosition.Height;
                        CheckSnapLeftAndRight(ref windowPosition, otherWindowRect, snapMode);
                        updated = true;
                    }

                    // Check the current window top against the other window bottom
                    otherWindowSnapBorder = new Rectangle(otherWindowRect.Left, otherWindowRect.Bottom, otherWindowRect.Width, snapDistance);
                    thisWindowSnapBorder = new Rectangle(windowPosition.Left, windowPosition.Top, windowPosition.Width, 1);

                    if (thisWindowSnapBorder.IntersectsWith(otherWindowSnapBorder))
                    {
                        windowPosition.Top = otherWindowRect.Bottom;
                        CheckSnapLeftAndRight(ref windowPosition, otherWindowRect, snapMode);
                        updated = true;
                    }
                }
            }

            // Update the last window position
            _lastWindowPosition = windowPosition;

            if (updated)
            {
                Marshal.StructureToPtr(windowPosition, lParam, true);
                handled = true;
            }

            return IntPtr.Zero;
        }

        private void CheckSnapTopAndBottom(ref Structures.WindowPosition windowPosition, Rectangle otherWindowRect, SnapMode snapMode)
        {
            int snapDistance = SnapDistance;

            switch (snapMode)
            {
                case SnapMode.Move:
                    if (Math.Abs(windowPosition.Top - otherWindowRect.Top) <= snapDistance)
                        windowPosition.Top = otherWindowRect.Top;
                    else if (Math.Abs(windowPosition.Bottom - otherWindowRect.Bottom) <= snapDistance)
                        windowPosition.Top = otherWindowRect.Bottom - windowPosition.Height;

                    break;
                case SnapMode.Resize:
                    if (Math.Abs(windowPosition.Top - otherWindowRect.Top) <= snapDistance)
                    {
                        windowPosition.Height += (windowPosition.Top - otherWindowRect.Top);
                        windowPosition.Top = otherWindowRect.Top;
                    }
                    else
                        if (Math.Abs(windowPosition.Bottom - otherWindowRect.Bottom) <= snapDistance)
                            windowPosition.Height = otherWindowRect.Bottom - windowPosition.Top;

                    break;
            }
        }

        private void CheckSnapLeftAndRight(ref Structures.WindowPosition windowPosition, Rectangle otherWindowRect, SnapMode snapMode)
        {
            int snapDistance = SnapDistance;

            switch (snapMode)
            {
                case SnapMode.Move:
                    if (Math.Abs(windowPosition.Left - otherWindowRect.Left) <= snapDistance)
                        windowPosition.Left = otherWindowRect.Left;
                    else if (Math.Abs(windowPosition.Right - otherWindowRect.Right) <= snapDistance)
                        windowPosition.Left = otherWindowRect.Right - windowPosition.Width;

                    break;
                case SnapMode.Resize:
                    if (Math.Abs(windowPosition.Left - otherWindowRect.Left) <= snapDistance)
                    {
                        windowPosition.Width += (windowPosition.Left - otherWindowRect.Left);
                        windowPosition.Left = otherWindowRect.Left;
                    }
                    else
                        if (Math.Abs(windowPosition.Right - otherWindowRect.Right) <= snapDistance)
                            windowPosition.Width = otherWindowRect.Right - windowPosition.Left;

                    break;
            }
        }

        #endregion
    }
}
