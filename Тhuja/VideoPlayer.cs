using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Тhuja
{
    public class VideoPlayer
    {
        public static void Play(string path)
        {
            Console.WriteLine("Reading...");
            var bytes = File.ReadAllBytes(path);

            var width = BitConverter.ToInt32(bytes, 0);
            var height = BitConverter.ToInt32(bytes, 4);
            var fps1 = BitConverter.ToUInt16(bytes, 8);
            var fps2 = BitConverter.ToUInt16(bytes, 10);
            var invFps = (double)fps2 / fps1;

            var offset = 12;
            var count = (bytes.Length - offset) / (width * height * 8);
            var frames = new VideoFrame[count];
            for (var i = 0; i < count; i++)
            {
                var frame = new ColoredChar?[width, height];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var background = bytes[offset];
                        var foreground = bytes[offset + 1];
                        var flags = BitConverter.ToUInt16(bytes, offset + 2);
                        var chars = Encoding.UTF32.GetChars(bytes, offset + 4, 4);

                        var character = chars.Length == 1 ? chars[0] : '�';
                        offset += 8;

                        frame[x, y] = new ColoredChar((MyColor) foreground, (MyColor) background, flags, character);
                    }
                }

                frames[i] = new VideoFrame {Content = frame};
            }

            var display = new Display();
            var delay = Stopwatch.Frequency * invFps;
            Console.WriteLine($"Playing {frames.Length} frames...");
            for (var i = 0; i < frames.Length; i++)
            {
                var start = Stopwatch.GetTimestamp();
                var end = start + delay;

                var frame = frames[i];
                display.CurrentScreen().PlaceWidget(0, 0, frame);
                display.Draw();

                while (Stopwatch.GetTimestamp() < end)
                {
                    Thread.Sleep(1);
                }

                var time = (double) (Stopwatch.GetTimestamp() - start) / Stopwatch.Frequency;
                Debug.WriteLine($"frame #{i}/{frames.Length} rendered in {time} s");
            }

            Console.WriteLine("Done");
            Console.ReadLine();
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
