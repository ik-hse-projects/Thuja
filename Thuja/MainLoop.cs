using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Thuja
{
    public struct Tick
    {
    }

    public interface IWidget : IDisplayable
    {
        public (int, int) Fps { get; }
        public void Update(Tick tick);
        public bool BubbleDown(ConsoleKeyInfo key);
    }

    public class MainLoop
    {
        private Display? display;

        public IWidget? Focused;
        private readonly List<IWidget> widgets = new List<IWidget>();

        public void Add(IWidget widget)
        {
            widgets.Add(widget);
        }

        public void AddFocused(IWidget widget)
        {
            Add(widget);
            Focused = widget;
        }

        public void Start()
        {
            display = new Display();
            while (true)
            {
                var fps = FindFps();
                var delay = Stopwatch.Frequency / fps;
                var scaledCounters = widgets
                    .Select(w => w.Fps.Item2 == 0 ? 0 : w.Fps.Item1 * (fps / w.Fps.Item2))
                    .ToArray();
                var maxCounter = scaledCounters.Max();
                var counter = 0;
                do
                {
                    counter++;

                    var end = Stopwatch.GetTimestamp() + delay;

                    Tick(scaledCounters, counter);

                    while (Stopwatch.GetTimestamp() < end) Thread.Sleep(1);
                } while (counter <= maxCounter);
            }

            // ReSharper disable once FunctionNeverReturns
        }

        private void Tick(int[] scaledCounters, int counter)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);
                Focused?.BubbleDown(key);
            }

            for (var index = 0; index < widgets.Count; index++)
            {
                var widget = widgets[index];
                var scaled = scaledCounters[index];
                if (scaled % counter == 0)
                {
                    widget.Update(new Tick());
                    display!.CurrentScreen.PlaceWidget(widget);
                }
            }

            display!.Draw();
        }

        private int FindFps()
        {
            static int Lcm(int a, int b)
            {
                return a * b / Euclid(a, b);
            }

            // Find lowest common divisor of all widgets 
            const double minimum = 20;
            var calculated = widgets
                .Select(w => w.Fps.Item2)
                .Where(fps => fps != 0)
                .Aggregate(1, Lcm);
            return calculated * (int) Math.Ceiling(minimum / calculated);
        }

        private static int Euclid(int a, int b)
        {
            while (a != 0 && b != 0)
                if (a > b)
                    a %= b;
                else
                    b %= a;

            return a + b;
        }
    }
}