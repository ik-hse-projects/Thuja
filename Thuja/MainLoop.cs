using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Thuja
{
    public struct Tick {}
    
    public interface IWidget: IDisplayable
    {
        public (int, int) Fps { get; }
        public void Update(Tick tick);
        public bool BubbleDown(ConsoleKeyInfo key);
    }
    
    public class MainLoop
    {
        private List<IWidget> Widgets = new List<IWidget>();
        private Display _display;

        public IWidget? Foucused;

        public void Add(IWidget widget)
        {
            Widgets.Add(widget);
        }

        public void Start()
        {
            _display = new Display();
            while (true)
            {
                var fps = FindFps();
                var delay = Stopwatch.Frequency / fps;
                var scaledCounters = Widgets
                    .Select(w => w.Fps.Item1 * (fps / w.Fps.Item2))
                    .ToArray();
                var maxCounter = scaledCounters.Max();
                for (var counter = 1; counter <= maxCounter; counter++)
                {
                    var end = Stopwatch.GetTimestamp() + delay;
                    
                    Tick(scaledCounters, counter);

                    while (Stopwatch.GetTimestamp() < end)
                    {
                        Thread.Sleep(1);
                    }
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void Tick(int[] scaledCounters, int counter)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);
                Foucused?.BubbleDown(key);
            }
                    
            for (var index = 0; index < Widgets.Count; index++)
            {
                var widget = Widgets[index];
                var scaled = scaledCounters[index];
                if (scaled % counter == 0)
                {
                    widget.Update(new Tick());
                    _display.CurrentScreen.PlaceWidget(widget);
                }
            }
            _display.Draw();
        }

        private int FindFps()
        {
            static int Lcm(int a, int b) => a * b / Euclid(a, b);
            
            // Find lowest common divisor of all widgets 
            const double Minimum = 20;
            var calculated = Widgets
                .Select(w => w.Fps.Item2)
                .Aggregate(1, Lcm);
            return calculated * (int) Math.Ceiling(Minimum / calculated);
        }
        
        private static int Euclid(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                {
                    a %= b;
                }
                else
                {
                    b %= a;
                }
            }

            return a + b;
        }
    }
}