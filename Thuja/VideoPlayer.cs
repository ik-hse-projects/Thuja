using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading;

namespace Thuja
{
    public class VideoPlayer
    {
        public static void Play()
        {
            var path = Console.BufferWidth >= 84 && Console.BufferHeight >= 63 ? "apple.br" : "small.br";
            var root = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var file = File.OpenRead(Path.Combine(root, path));
            var brotli = new BrotliStream(file, CompressionMode.Decompress);
            var reader = VideoReader.Init(brotli);
            
            var invFps = (double) reader.Info.Fps2 / reader.Info.Fps1;
            var display = new Display();
            var delay = Stopwatch.Frequency * invFps;
            Console.WriteLine($"Playing...");
            while (reader.MoveNext())
            {
                var end = Stopwatch.GetTimestamp() + delay;

                var frame = reader.Current;
                display.CurrentScreen().PlaceWidget(0, 0, frame);
                display.Draw();

                while (Stopwatch.GetTimestamp() < end)
                {
                    Thread.Sleep(1);
                }
            }
        }
    }

    public class VideoFrame : IDisplayable
    {
        public ColoredChar?[,] Content;

        public ColoredChar?[,] Render()
        {
            return Content;
        }
    }
}
