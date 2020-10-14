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
                    new StackContainer(maxVisibleCount: 5)
                    {
                        new Label("1"),
                        new Button("2 <-"),
                        new Button("3 <-"),
                        new Label("4"),
                        new Button("5 <-"),
                        new Label("6"),
                        new Label("7"),
                        new Label("8"),
                        new Button("9 <-"),
                        new Button("0 <-"),
                    }
                }
            };

            new MainLoop(root).Start();
        }
    }
}