using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Thuja
{
    public readonly struct Tick
    {
    }

    public class MainLoop
    {
        private readonly List<IWidget> widgets = new List<IWidget>();
        private IWidget root;
        private Display? display;

        public MainLoop(IWidget root)
        {
            Register(root);
            this.root = root;
        }

        public void Register(IWidget widget)
        {
            widget.OnRegistered(this);
            widgets.Add(widget);
        }

        public void Start()
        {
            display = new Display();
            while (true)
            {
                var fps = FindFps();
                var delay = Stopwatch.Frequency / fps;
                var scaledCounters = widgets
                    .Select(w => w.Fps == 0 ? 0 : fps / w.Fps)
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
            if (Console.KeyAvailable && root is IFocusable focusable)
            {
                var key = Console.ReadKey(true);
                focusable.BubbleDown(key);
            }

            for (var index = 0; index < widgets.Count; index++)
            {
                var widget = widgets[index];
                var scaled = scaledCounters[index];
                if (scaled % counter == 0)
                {
                    widget.Update(new Tick());
                }
            }

            var context = display!.CurrentScreen.BeginRender()
                .Derive(root.RelativePosition);
            root.Render(context);

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
                .Select(w => w.Fps)
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