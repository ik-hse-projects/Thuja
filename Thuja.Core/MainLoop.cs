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
        ///     Дисплей, на котором происходит отрисовка.
        /// </summary>
        private Display? display;

        private IRenderer renderer;

        /// <summary>
        ///     Создает новый цикл с данным виджетом в качестве корневого.
        /// </summary>
        /// <param name="root">Виджет, который будет корневым в новом цикле.</param>
        public MainLoop(IWidget root): this(root, new ConsoleRenderer())
        {
        }
        
        /// <summary>
        ///     Создает новый цикл с данным виджетом в качестве корневого и способом отрисовки на экран.
        /// </summary>
        /// <param name="root">Виджет, который будет корневым в новом цикле.</param>
        /// <param name="renderer">При помощи чего нужнео взаимодействовать с пользователем.</param>
        public MainLoop(IWidget root, IRenderer renderer)
        {
            this.root = root;
            this.renderer = renderer;
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
        ///     Запускает цикл отрисовки и обновления виджетов.
        /// </summary>
        public void Start()
        {
            OnStop = null;
            display = new Display(renderer);
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
        ///     Вспомогательная функция, которая отрисовывает все виджеты.
        /// </summary>
        private void Tick()
        {
            if (root is IFocusable focusable)
            {
                foreach (var key in renderer.GetKeys())
                {
                    focusable.BubbleDown(key);
                }
            }

            // Виджеты могут быть добавлены во время отрисовки. Всех таких обновим уже в следующий тик. 
            var context = display!.CurrentScreen.BeginRender(renderer);
            root.Render(context);
            display!.Draw();
        }
    }
}