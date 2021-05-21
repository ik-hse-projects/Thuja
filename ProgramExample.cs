using System;
using System.IO;
using Thuja.Video;
using Thuja.Widgets;

namespace Thuja
{
    /// <summary>
    /// Пример простой программы при помощи Thuja.
    /// </summary>
    internal static class ProgramExample
    {
        private static void ThujaMain(string[] args)
        {
            var root = new BaseContainer()
                .Add(new VideoPlayer(File.OpenRead(Path.Combine(args[0]))))
                .Add(new Frame()
                    .Add(new StackContainer(maxVisibleCount: 5)
                        .Add(new Label("Hello!"))
                        .Add(new Button("Press me to exit").AsIKeyHandler()
                            .Add(new KeySelector(ConsoleKey.Enter), () => Environment.Exit(0))
                        )
                    )
                );
            new MainLoop(root).Start();
        }
    }
}