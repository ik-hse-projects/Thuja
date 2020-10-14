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
            var root = new BaseContainer
            {
                new VideoPlayer(File.OpenRead(Path.Combine(args[0]))),
                new RelativePosition(10, 10)
                {
                    new StackContainer(maxVisibleCount: 5)
                    {
                        new Label("1"),
                        new Button("Enter") { (new KeySelector(ConsoleKey.Enter), () => Environment.Exit(0)) },
                        new Button("F1") { (new KeySelector(ConsoleKey.F1), () => Environment.Exit(0)) },
                        new Label("4"),
                        new Button("Ctrl+Q") { (new KeySelector(ConsoleKey.Q, ConsoleModifiers.Control), () => Environment.Exit(0)) },
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