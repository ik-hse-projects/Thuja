using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Thuja
{
    /// <summary>
    /// Главный цикл программы. Отрисовывает и обновляет все виджеты.
    /// </summary>
    public class MainLoop
    {
        /// <summary>
        /// Корневой виджет, который будет отрисовываться.
        /// </summary>
        private readonly IWidget root;
        
        /// <summary>
        /// Список виджетов, привязанных к этому циклу.
        /// </summary>
        private readonly List<IWidget> widgets = new List<IWidget>();

        /// <summary>
        /// Дисплей, на котором происходит отрисовка.
        /// </summary>
        private Display? display;

        /// <summary>
        /// Создает новый цикл с данным виджетом в качестве корневого.
        /// </summary>
        /// <param name="root">Виджет, который будет корневым в новом цикле.</param>
        public MainLoop(IWidget root)
        {
            Register(root);
            this.root = root;
        }

        /// <summary>
        /// Если не null, то цикл приостановит отрисовку, очистит консоль и вызовет этот метод,
        /// после чего продолжит работу как и раньше.
        /// </summary>
        public Action? OnPaused { get; set; }

        /// <summary>
        /// Если не null, то цикл остановит отрисовку, очистит консоль и вызовет этот метод,
        /// после чего завершит работу.
        /// </summary>
        public Action? OnStop { get; set; }

        /// <summary>
        /// Отвязывает виджет от цикла.
        /// </summary>
        /// <param name="widget">Виджет, который будет отвязан от цикла.</param>
        public void Unregister(IWidget widget)
        {
            if (widget is BaseContainer container)
            {
                container.Clear();
            }
            widgets.Remove(widget);
        }

        /// <summary>
        /// Привязывает новый виджет к этому циклу.
        /// </summary>
        /// <param name="widget">Виджет, который будет привязан к циклу.</param>
        public void Register(IWidget widget)
        {
            widget.OnRegistered(this);
            widgets.Add(widget);
        }

        /// <summary>
        /// Запускает цикл отрисовки и обновления виджетов.
        /// </summary>
        public void Start()
        {
            OnStop = null;
            display = new Display();
            if (root is IFocusable focusable)
            {
                focusable.FocusChange(true);
            }

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

        /// <summary>
        /// Вспомогательная функция, которая обновляет и отрисовывает все виджеты.
        /// </summary>
        /// <param name="fps">Текущая частота обновлений в секунду.</param>
        /// <param name="counter">Номер обновления.</param>
        /// <returns>Новое необходимое количество обновлений.</returns>
        private int Tick(int fps, int counter)
        {
            if (Console.KeyAvailable && root is IFocusable focusable)
            {
                var key = Console.ReadKey(true);
                focusable.BubbleDown(key);
            }

            var maxCounter = 0;
            foreach (var widget in widgets)
            {
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

        /// <summary>
        /// Вычисляет FPS, чтобы иметь возможность обновлять каждый виджет так часто, как он этого хочет.
        /// </summary>
        /// <returns>Возвращает вычисленный FPS.</returns>
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

        /// <summary>
        /// Алгоритм Евклида для поиска НОД делением.
        /// </summary>
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