using System;
using System.Collections.Generic;
using System.Linq;

namespace Thuja.Widgets
{
    public abstract class TabPage
    {
        public abstract Button Button { get; }
        public abstract IWidget Widget { get; }
    }

    public class TabPage<T> : TabPage where T : IWidget
    {
        public T SpecificWidget { get; }
        public override IWidget Widget => SpecificWidget;
        public override Button Button { get; }

        internal TabPage(Button button, T widget)
        {
            Button = button;
            SpecificWidget = widget;
        }
    }

    /// <summary>
    ///     Виджет, который представляет из себя несколько вкладок, которые можно фокусировать.
    /// </summary>
    public class Tabs : IKeyHandler
    {
        private TabPage? current;
        private List<TabPage> Pages = new();
        private readonly StackContainer titles = new StackContainer(Orientation.Horizontal, 1);
        private readonly StackContainer container = new StackContainer();
        private readonly BaseContainer content;

        public Dictionary<HashSet<KeySelector>, Action> Actions { get; } = new();

        /// <summary>
        ///     Создаёт виджет с вкладками. 
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

        /// <summary>
        ///     Максимальное число видимых заголовков.
        /// </summary>
        public int MaxLabelsVisible
        {
            get => titles.MaxVisibleCount;
            set => titles.MaxVisibleCount = value;
        }

        /// <summary>
        ///     Добавляет новую вкладку.
        /// </summary>
        /// <param name="title">Заголовок вкладки.</param>
        /// <param name="widget">Содержимое вкладки.</param>
        /// <returns>Возвращает добавленную вкладку.</returns>
        public TabPage<T> Add<T>(string title, T widget) where T : IWidget
        {
            var button = new Button(title) {OverrideStyle = Style.Inactive};
            titles.Add(button);
            var page = new TabPage<T>(button, widget);
            button.AsIKeyHandler().Add(KeySelector.SelectItem, () => Focus(page));
            Pages.Add(page);
            return page;
        }

        /// <summary>
        ///     Удаляет вкладку из списка. Но не сбрасывает с неё фокус!
        /// </summary>
        public void Remove(TabPage page)
        {
            titles.Remove(page.Button);
        }

        /// <summary>
        ///     Добавляет новую вкладку.
        /// </summary>
        /// <param name="title">Заголовок вкладки.</param>
        /// <param name="widget">Содержимое вкладки.</param>
        /// <param name="page">Свежедобавленная вкладка.</param>
        /// <returns>Возвращает этот же самый объект (builder pattern).</returns>
        public Tabs Add<T>(string title, T widget, out TabPage<T> page) where T : IWidget
        {
            page = Add(title, widget);
            return this;
        }

        /// <summary>
        ///     Фокусируется на выбранной вкладке.
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
        }

        /// <summary>
        ///     Фокусируется на последней добавленной вкладке.
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

        public bool BubbleDown(ConsoleKeyInfo key)
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
        ///     Преобразовывает этот контейнер в экземпляр <see cref="IKeyHandler" />
        /// </summary>
        /// <returns>Объект типа <see cref="IKeyHandler" />, который может быть преобразован в <see cref="Tabs" />.</returns>
        public IKeyHandler AsIKeyHandler()
        {
            return this;
        }
    }
}