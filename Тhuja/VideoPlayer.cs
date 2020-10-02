using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Тhuja
{
    public class VideoPlayer
    {
        public static void Play(string path)
        {
            Console.WriteLine("Reading...");
            var lines = File.ReadAllLines(path);
            var header = lines[0];
            var splitted = header.Split(' ');

            var height = int.Parse(splitted[0]);
            var fps = int.Parse(splitted[1]);
            var width = lines[1].Length;

            var length = (lines.Length - 1) / height;
            var frames = new VideoFrame[length];
            for (int i = 0; i < length; i++)
            {
                var frame = new char?[width, height];
                for (int y = 0; y < height; y++)
                {
                    var ln = 1 + i * height + y;
                    for (var x = 0; x < lines[ln].Length; x++)
                    {
                        frame[x, y] = lines[ln][x];
                    }
                }

                frames[i] = new VideoFrame {Content = frame};
            }
            
            var display = new Display();
            var delay = Stopwatch.Frequency * (1.0 / fps);
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

                var time = (double)(Stopwatch.GetTimestamp() - start) / Stopwatch.Frequency;
                Debug.WriteLine($"frame #{i}/{frames.Length} rendered in {time} s");
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }

    public class VideoFrame: IDisplayable
    {
        public char?[,] Content;
        
        public char?[,] Render()
        {
            return Content;
        }
    }
}