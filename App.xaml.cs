using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using WinRT;
using System.Drawing;

namespace MarkGPT
{
    public partial class App : Application
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        const uint WM_SETICON = 0x0080;
        const int ICON_SMALL = 0;
        const int ICON_BIG = 1;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(m_window);

            SetWindowIcon(hwnd, "C:\\users\\Mark\\source\\repos\\MarkGPT\\Assets\\MarkGPT.ico");

            m_window.Activate();
        }

        private void SetWindowIcon(IntPtr hwnd, string iconPath)
        {
            using var icon = new System.Drawing.Icon(iconPath);
            SendMessage(hwnd, WM_SETICON, (IntPtr)ICON_SMALL, icon.Handle);
            SendMessage(hwnd, WM_SETICON, (IntPtr)ICON_BIG, icon.Handle);
        }

        private Window? m_window;
    }
}
