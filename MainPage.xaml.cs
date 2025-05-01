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

namespace MarkGPT
{

    public sealed partial class MainPage : Page
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public MainPage()
        {
            this.InitializeComponent();
            ShowRandomGreeting();
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


        private async Task ShowBotMessageAsync(string message)
        {
            var botResponse = await DeepSeekTextAI.GetDeepSeekResponseAsync(message);

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
                Background = new SolidColorBrush(Color.FromArgb(255, 51, 51, 51)),
                CornerRadius = new CornerRadius(25),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 6),
                Child = botTextBlock,
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxWidth = 1000
            };

            ChatStack.Children.Add(botBorder);
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            var message = InputBox.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                //AiResponse = 

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
                InputBox.Text = "";


                ShowBotMessageAsync(message);
                
            }



        }

        private void ShowRandomGreeting()
        {
            Random rand = new Random();
            string greeting = greetings[rand.Next(greetings.Count)];

            // Create a text block to display the greeting
            var textBlock = new TextBlock
            {
                Text = greeting,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 17,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
            };

            // Create a border around the text block (like a message bubble)
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(255, 51, 51, 51)),
                CornerRadius = new CornerRadius(25),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 6),
                Child = textBlock,
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxWidth = 1000
            };

            // Add the greeting to the chat stack
            ChatStack.Children.Add(border);
        }



    }
}

