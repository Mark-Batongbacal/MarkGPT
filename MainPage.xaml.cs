using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Text;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI; // for Color


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarkGPT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            var message = InputBox.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                var textBlock = new TextBlock
                {
                    Text = message,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    FontSize = 17,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Colors.White),
                    FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
                };

                var border = new Border
                {
                    Background = new SolidColorBrush(Color.FromArgb(255, 51, 51, 51)), 
                    CornerRadius = new CornerRadius(25), // Oval sides
                    Padding = new Thickness(15),
                    Margin = new Thickness(0, 0, 0, 6),
                    Child = textBlock,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    MaxWidth = 300 // Optional: wraps long text nicely
                };


                ChatStack.Children.Add(border);
                InputBox.Text = "";
            }
        }
    }
}
