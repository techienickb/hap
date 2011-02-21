using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media;

namespace HAP.UserCard
{
    struct MARGINS
    {
        public MARGINS(Thickness t)
        {
            Left = (int)t.Left;
            Right = (int)t.Right;
            Top = (int)t.Top;
            Bottom = (int)t.Bottom;
        }
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }

    public class WindowBehavior
    {

        public static bool ExtendGlassFrame(Window window, Thickness margin)
        {
            try
            {
                if (!DwmIsCompositionEnabled())
                    return false;
            }
            catch { return false; }
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero)
                throw new InvalidOperationException("The Window must be shown before extending glass.");

            // Set the background to transparent from both the WPF and Win32 perspectives
            window.Background = Brushes.Transparent;
            HwndSource.FromHwnd(hwnd).CompositionTarget.BackgroundColor = Colors.Transparent;

            MARGINS margins = new MARGINS(margin);
            DwmExtendFrameIntoClientArea(hwnd, ref margins);
            return true;
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern bool DwmIsCompositionEnabled();


        private static readonly Type OwnerType = typeof(WindowBehavior);

        #region HideCloseButton (attached property)

        public static readonly DependencyProperty HideCloseButtonProperty =
            DependencyProperty.RegisterAttached(
                "HideCloseButton",
                typeof(bool),
                OwnerType,
                new FrameworkPropertyMetadata(false, new PropertyChangedCallback(HideCloseButtonChangedCallback)));

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static bool GetHideCloseButton(Window obj)
        {
            return (bool)obj.GetValue(HideCloseButtonProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static void SetHideCloseButton(Window obj, bool value)
        {
            obj.SetValue(HideCloseButtonProperty, value);
        }

        private static void HideCloseButtonChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window == null) return;

            var hideCloseButton = (bool)e.NewValue;
            if (hideCloseButton && !GetIsHiddenCloseButton(window))
            {
                if (!window.IsLoaded)
                {
                    window.Loaded += LoadedDelegate;
                }
                else
                {
                    HideCloseButton(window);
                }
                SetIsHiddenCloseButton(window, true);
            }
            else if (!hideCloseButton && GetIsHiddenCloseButton(window))
            {
                if (!window.IsLoaded)
                {
                    window.Loaded -= LoadedDelegate;
                }
                else
                {
                    ShowCloseButton(window);
                }
                SetIsHiddenCloseButton(window, false);
            }
        }

        #region Win32 imports

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        #endregion

        private static readonly RoutedEventHandler LoadedDelegate = (sender, args) =>
        {
            if (sender is Window == false) return;
            var w = (Window)sender;
            HideCloseButton(w);
            w.Loaded -= LoadedDelegate;
        };

        private static void HideCloseButton(Window w)
        {
            var hwnd = new WindowInteropHelper(w).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private static void ShowCloseButton(Window w)
        {
            var hwnd = new WindowInteropHelper(w).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) | WS_SYSMENU);
        }

        #endregion

        #region IsHiddenCloseButton (readonly attached property)

        private static readonly DependencyPropertyKey IsHiddenCloseButtonKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "IsHiddenCloseButton",
                typeof(bool),
                OwnerType,
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsHiddenCloseButtonProperty =
            IsHiddenCloseButtonKey.DependencyProperty;

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static bool GetIsHiddenCloseButton(Window obj)
        {
            return (bool)obj.GetValue(IsHiddenCloseButtonProperty);
        }

        private static void SetIsHiddenCloseButton(Window obj, bool value)
        {
            obj.SetValue(IsHiddenCloseButtonKey, value);
        }

        #endregion

    }

}
