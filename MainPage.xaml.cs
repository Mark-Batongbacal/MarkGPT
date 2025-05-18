using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Text;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Newtonsoft.Json;
using System.Threading.Tasks;
using MarkGPT.AI;
using MarkGPT.Methods;
using System.Diagnostics;
using static MarkGPT.Methods.StepModels;
using MarkGPT.Helpers;
using System.Text.RegularExpressions;
using Windows.UI.Core;
using Windows.System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices;
namespace MarkGPT
{

    public sealed partial class MainPage : Page
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public MainPage()
        {
            this.InitializeComponent();
            ShowRandomGreeting();
            InputBox.Foreground = new SolidColorBrush(Colors.White);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        public async Task getMessage(string message)
        {
            await ShowBotMessageAsync(message);
        }

        private List<string> greetings = new List<string>
        {
            "Hey there! What numerical method can I help you solve today? 🤔🔢",

            "Hello! Ready to solve some math problems? What method are we tackling today? 💡📐",

            "Hi! Which numerical method are you looking to work on? 🧠🔍",

            "Good day! What problem would you like to solve today? Let me know the method! 🌞📝",

            "Hi! Got a problem for me to solve? Let’s pick a method! 💭🔧",

            "Hey! Ready to dive into some math? What numerical method do you need help with? 🚀📊",

            "Hello! What’s the next challenge? Choose a numerical method, and I’ll help you solve it! 🔎💪",

            "Hi there! What problem are we solving today? Let’s pick a method and get started! 😊📈"
        };

        public async void ScrollToBottom()
        {
            await Task.Delay(50);
            scrollViewer.ChangeView(null, scrollViewer.ExtentHeight, null);

        }


        public async Task ShowBotMessageAsync(string message)
        {
            //string botResponse = await DeepSeekTextAI.GetLlamaResponseAsync(message);
            string botResponse = await LlamaTextAI.GetLlamaResponseAsync(message);


            string tempresp = botResponse;

            var botTextBlock = new TextBlock
            {
                Text = botResponse,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 17,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
            };

            var botBorder = new Border
            {
                CornerRadius = new CornerRadius(25),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 6),
                Child = botTextBlock,
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxWidth = 1000
            };

            ChatStack.Children.Add(botBorder);
            ScrollToBottom();

            var matches = Regex.Matches(botResponse, @"120219\|[^:\n]+");
            

            foreach (Match match in matches)
            {
                Debug.WriteLine(match.Value);
            }

            List<string> combinedMethods = new List<string>();
            for (int i = 0; i < matches.Count; i++)
            {
                string result = matches[i].Value;
                string[] parts = result.Split('|');

                if (parts.Length >= 3)
                {
                    try
                    {
                        var (msg, display) = MethodDispatcher.Dispatch(this, parts, ChatStack);
                        ScrollToBottom();
                        display?.Invoke();

                        if (i == matches.Count - 1)
                            combinedMethods.Add(parts[1]);
                    }
                    catch
                    {
                        Debug.WriteLine($"Error processing: {result}");
                    }
                }
            }
            
            for (int i = 1; i <= botResponse.Length; i++)
            {
                botTextBlock.Text = botResponse.Substring(0, i);
                await Task.Delay(10); // adjust delay for speed
            }
        }

        [DllImport("user32.dll")]
        static extern short GetKeyState(int nVirtKey);

        private void InputBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            const int VK_SHIFT = 0x10;

            bool isShiftDown = (GetKeyState(VK_SHIFT) & 0x8000) != 0;

            if (e.Key == Windows.System.VirtualKey.Enter && !isShiftDown)
            {
                e.Handled = true; // Prevent newline
                Send_Click(null, null); // Call your send
            }
        }

    private async void Send_Click(object sender, RoutedEventArgs e)
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
                    CornerRadius = new CornerRadius(25),
                    Padding = new Thickness(15),
                    Margin = new Thickness(0, 0, 0, 6),
                    Child = textBlock,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    MaxWidth = 1000
                };


                ChatStack.Children.Add(border);
                ScrollToBottom();
                InputBox.Text = "";


                ShowBotMessageAsync(message);

                for (int i = 1; i <= message.Length; i++)
                {
                    textBlock.Text = message.Substring(0, i);
                    await Task.Delay(10); // adjust delay for speed
                }
            }
        }

        private async void ShowRandomGreeting()
        {
            Random rand = new Random();
            string greeting = greetings[rand.Next(greetings.Count)];

            var textBlock = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 17,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
            };

            var border = new Border
            {
                CornerRadius = new CornerRadius(25),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 6),
                Child = textBlock,
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxWidth = 1000
            };

            ChatStack.Children.Add(border);

            // Typing animation
            for (int i = 1; i <= greeting.Length; i++)
            {
                textBlock.Text = greeting.Substring(0, i);
                await Task.Delay(10); // adjust delay for speed
            }
        }


        private void Page_Loaded_1(object sender, RoutedEventArgs e)
        {

        }
    }
}

