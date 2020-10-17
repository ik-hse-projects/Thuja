using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Thuja
{
    public class MainLoop
    {
        private readonly IWidget root;
        private readonly List<IWidget> widgets = new List<IWidget>();
        private Display? display;
        
        public Action? OnPaused { get; set; }

        private Action? OnStop { get; set; }

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
            OnStop = null;
            display = new Display();
            if (root is IFocusable focusable) focusable.FocusChange(true);
            
            display.Clear();
            while (OnStop == null)
            {
                var fps = FindFps();
                var delay = Stopwatch.Frequency / fps;
                var maxCounter = 0;
                var counter = 0;
                do
                {
                    var end = Stopwatch.GetTimestamp() + delay;

                    if (OnPaused == null)
                    {
                        counter++;

                        var newMaxCounter = Tick(fps, counter);
                        if (newMaxCounter > maxCounter)
                        {
                            maxCounter = newMaxCounter;
                        }
                    }
                    else
                    {
                        display.Clear();
                        OnPaused?.Invoke();
                        OnPaused = null;
                        display.Clear();
                    }

                    while (Stopwatch.GetTimestamp() < end)
                    {
                        Thread.Sleep(1);
                    }
                } while (counter <= maxCounter);
            }

            display.Clear();
            OnStop?.Invoke();
        }

        private int Tick(int fps, int counter)
        {
            if (Console.KeyAvailable && root is IFocusable focusable)
            {
                var key = Console.ReadKey(true);
                focusable.BubbleDown(key);
            }

            var maxCounter = 0;
            for (var index = 0; index < widgets.Count; index++)
            {
                var widget = widgets[index];
                var scaled = widget.Fps == 0 ? 1 : fps / widget.Fps;
                if (scaled > maxCounter)
                {
                    maxCounter = scaled;
                }

                if (counter % scaled == 0)
                {
                    widget.Update();
                }
            }

            var context = display!.CurrentScreen.BeginRender();
            root.Render(context);

            display!.Draw();

            return maxCounter;
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