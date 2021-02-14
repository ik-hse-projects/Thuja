using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Thuja
{
    /// <summary>
    ///     Главный цикл программы. Отрисовывает и обновляет все виджеты.
    /// </summary>
    public class MainLoop
    {
        /// <summary>
        ///     Корневой виджет, который будет отрисовываться.
        /// </summary>
        private readonly IWidget root;

        /// <summary>
        ///     Список виджетов, привязанных к этому циклу.
        /// </summary>
        private List<IWidget> widgets = new List<IWidget>();

        /// <summary>
        ///     Дисплей, на котором происходит отрисовка.
        /// </summary>
        private Display? display;

        /// <summary>
        ///     Создает новый цикл с данным виджетом в качестве корневого.
        /// </summary>
        /// <param name="root">Виджет, который будет корневым в новом цикле.</param>
        public MainLoop(IWidget root)
        {
            Register(root);
            this.root = root;
        }

        /// <summary>
        ///     Если не null, то цикл приостановит отрисовку, очистит консоль и вызовет этот метод,
        ///     после чего продолжит работу как и раньше.
        /// </summary>
        public Action? OnPaused { get; set; }

        /// <summary>
        ///     Если не null, то цикл остановит отрисовку, очистит консоль и вызовет этот метод,
        ///     после чего завершит работу.
        /// </summary>
        public Action? OnStop { get; set; }

        /// <summary>
        ///     Отвязывает виджет от цикла.
        /// </summary>
        /// <param name="widget">Виджет, который будет отвязан от цикла.</param>
        public void Unregister(IWidget widget)
        {
            widget.OnUnregistered();
            widgets.Remove(widget);
        }

        /// <summary>
        ///     Привязывает новый виджет к этому циклу.
        /// </summary>
        /// <param name="widget">Виджет, который будет привязан к циклу.</param>
        public void Register(IWidget widget)
        {
            widget.OnRegistered(this);
            widgets.Add(widget);
        }

        /// <summary>
        ///     Запускает цикл отрисовки и обновления виджетов.
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
            const int fps = 60;
            var delay = Stopwatch.Frequency / fps;
            while (OnStop == null)
            {
                var end = Stopwatch.GetTimestamp() + delay;

                if (OnPaused == null)
                {
                    Tick();
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
            }

            display.Clear();
            OnStop?.Invoke();
        }

        /// <summary>
        ///     Вспомогательная функция, которая обновляет и отрисовывает все виджеты.
        /// </summary>
        private void Tick()
        {
            if (Console.KeyAvailable && root is IFocusable focusable)
            {
                var key = Console.ReadKey(true);
                focusable.BubbleDown(key);
            }

            // Виджеты могут быть добавлены во время Update. Всех таких обновим уже в следующий тик. 
            var oldWidgets = widgets;
            widgets = new List<IWidget>();

            foreach (var widget in oldWidgets)
            {
                widget.Update();
            }

            // Добавляем добавленные.
            oldWidgets.AddRange(widgets);
            widgets = oldWidgets;

            var context = display!.CurrentScreen.BeginRender();
            root.Render(context);

            display!.Draw();
        }
    }
}