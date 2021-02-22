using System;
using System.Collections.Generic;

namespace Thuja.Widgets
{
    /// <summary>
    /// Нетипизированная вкладка внутри <see cref="Tabs" />.
    /// Если возможно, то лучше использовать <see cref="TabPage{T}" />.
    /// </summary>
    public abstract class TabPage
    {
        /// <summary>
        /// Вызывается, когда эта вкладка становится сфокусированной.
        /// </summary>
        public Action? Focused;

        /// <summary>
        /// Заголовок вкладки.
        /// </summary>
        public abstract Button Button { get; }

        /// <summary>
        /// Содержимое вкладки.
        /// </summary>
        public abstract IWidget Widget { get; }
    }

    /// <summary>
    /// Вкладка внутри <see cref="Tabs" />.
    /// </summary>
    /// <typeparam name="T">Тип её содержимого.</typeparam>
    public class TabPage<T> : TabPage where T : IWidget
    {
        internal TabPage(Button button, T widget)
        {
            Button = button;
            SpecificWidget = widget;
        }

        internal TabPage(string title, T widget) : this(new Button(title) {OverrideStyle = Style.Inactive}, widget)
        {
        }

        /// <summary>
        /// Типизированое содержимое вкладки.
        /// </summary>
        public T SpecificWidget { get; }

        /// <inheritdoc />
        public override IWidget Widget => SpecificWidget;

        /// <inheritdoc />
        public override Button Button { get; }
    }

    /// <summary>
    /// Виджет, который представляет из себя несколько вкладок, которые можно фокусировать.
    /// </summary>
    public class Tabs : DelegateIFocusable, IKeyHandler
    {
        private readonly StackContainer container = new();
        private readonly BaseContainer content;
        private readonly StackContainer titles = new(Orientation.Horizontal, 1);

        private TabPage? current;
        private MainLoop? mainLoop;
        private readonly List<TabPage> Pages = new();

        /// <summary>
        /// Создаёт виджет с вкладками.
        /// </summary>
        /// <param name="framed">Следует ли поместить содержимое в рамочку.</param>
        public Tabs(bool framed = true)
        {
            content = framed ? new Frame() : new BaseContainer();
            container.Add(titles).Add(content);
            titles.FocusedChanged += () =>
            {
                if (titles.Focused != null && titles.Position < Pages.Count && titles.Position >= 0)
                {
                    Focus(Pages[titles.Position]);
                }
            };
        }

        protected override IFocusable FocusableImplementation => container;

        /// <summary>
        /// Максимальное число видимых заголовков.
        /// </summary>
        public int MaxLabelsVisible
        {
            get => titles.MaxVisibleCount;
            set => titles.MaxVisibleCount = value;
        }

        public Dictionary<HashSet<KeySelector>, Action> Actions { get; } = new();

        public override bool BubbleDown(ConsoleKeyInfo key)
        {
            // Нажатия обрабатывааются в следюущем порядке:
            // 1. Те обработчики, которые были привязаны напрямую к этому объекту;
            // 2. Содержимое текущей вкладки;
            // 3. Список вкладок.
            return AsIKeyHandler().TryHandleKey(key)
                   || content.BubbleDown(key)
                   || titles.BubbleDown(key);
        }

        /// <inheritdoc />
        public void Render(RenderContext context)
        {
            container.Render(context);
        }

        /// <inheritdoc />
        public void FocusChange(bool isFocused)
        {
            content.FocusChange(isFocused);
        }

        /// <summary>
        /// Удаляет вкладку из списка. Но не сбрасывает с неё фокус!
        /// </summary>
        public void Remove(TabPage page)
        {
            Pages.Remove(page);
            titles.Remove(page.Button);
        }

        /// <summary>
        /// Создаёт новую вкладку и помещает её заголовок на указанное место.
        /// </summary>
        /// <param name="i">Индекс вкладки.</param>
        /// <param name="title">Её заголовок.</param>
        /// <param name="widget">Содержимое вкладки.</param>
        /// <typeparam name="T">Тип содержимого.</typeparam>
        public TabPage<T> Insert<T>(int i, string title, T widget) where T : IWidget
        {
            var tab = new TabPage<T>(title, widget);
            titles.Insert(i, tab.Button);
            Pages.Insert(i, tab);
            return tab;
        }

        /// <summary>
        /// Добавляет новую вкладку.
        /// </summary>
        /// <param name="title">Заголовок вкладки.</param>
        /// <param name="widget">Содержимое вкладки.</param>
        /// <returns>Возвращает этот же самый объект.</returns>
        public Tabs Add<T>(string title, T widget) where T : IWidget
        {
            return Add(title, widget, out _);
        }

        /// <summary>
        /// Добавляет новую вкладку.
        /// </summary>
        /// <param name="title">Заголовок вкладки.</param>
        /// <param name="widget">Содержимое вкладки.</param>
        /// <param name="page">Свежедобавленная вкладка.</param>
        /// <returns>Возвращает этот же самый объект.</returns>
        public Tabs Add<T>(string title, T widget, out TabPage<T> page) where T : IWidget
        {
            var tab = new TabPage<T>(title, widget);
            titles.Add(tab.Button);
            Pages.Add(tab);
            page = tab;
            return this;
        }

        /// <summary>
        /// Фокусируется на выбранной вкладке.
        /// </summary>
        /// <param name="page"></param>
        public void Focus(TabPage page)
        {
            // Сбрасываем фокус с предыдущей кнопки.
            if (current != null)
            {
                current.Button.OverrideStyle = Style.Inactive;
            }

            // И ставим стили для новой.
            page.Button.OverrideStyle = Style.Active;
            titles.Focused = page.Button;

            // Затем заменяем основное содержимое. Возможно, фокусируясь.
            content.Clear();
            if (page.Widget is IFocusable focusable)
            {
                content.AddFocused(focusable);
            }
            else
            {
                content.Add(page.Widget);
            }

            // Готово!
            current = page;
            page.Focused?.Invoke();
        }

        /// <summary>
        /// Фокусируется на последней добавленной вкладке.
        /// </summary>
        /// <returns>Возвращает этот же самый объект.</returns>
        public Tabs AndFocus()
        {
            if (Pages.Count != 0)
            {
                Focus(Pages[^1]);
            }

            return this;
        }

        /// <summary>
        /// Преобразовывает этот контейнер в экземпляр <see cref="IKeyHandler" />
        /// </summary>
        /// <returns>Объект типа <see cref="IKeyHandler" />, который может быть преобразован в <see cref="Tabs" />.</returns>
        public IKeyHandler AsIKeyHandler()
        {
            return this;
        }
    }
}