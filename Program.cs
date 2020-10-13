using System;
using System.IO;
using Thuja.Video;
using Thuja.Widgets;

namespace Thuja
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var button = new Button("Hello");
            button.OnClick += button1 =>
            {
                Environment.Exit(0);
            };
            
            var root = new BaseContainer
            {
                new VideoPlayer(File.OpenRead(Path.Combine(args[0]))),
                new RelativePosition(10, 10)
                {
                    new Frame
                    {
                        new StackContainer
                        {
                            button,
                            new InputField
                            {
                                MaxLength = 15,
                                AllowedChars = {CharRange.Ascii},
                                Placeholder =
                                    new Placeholder(new Style(MyColor.Cyan, MyColor.Black),
                                        "Enter text")
                            },
                            new StackContainer(Orientation.Horizontal, 2)
                            {
                                new Label("1234567890")
                                {
                                    MaxWidth = 4
                                },
                                new Label("abcdefg")
                                {
                                    MaxWidth = 4
                                },
                            },
                            new StackContainer(Orientation.Horizontal, 2)
                            {
                                new InputField
                                {
                                    MaxLength = 5,
                                    AllowedChars = {CharRange.Digits},
                                    Placeholder = new Placeholder(
                                        new Style(MyColor.DarkMagenta, MyColor.Black), "Digits!")
                                },
                                new InputField
                                {
                                    MaxLength = 5,
                                    AllowedChars = {CharRange.Letters},
                                    Placeholder = new Placeholder(
                                        new Style(MyColor.DarkMagenta, MyColor.Black), "Letters!")
                                }
                            },
                        }
                    }
                }
            };

            new MainLoop(root).Start();
        }
    }
}