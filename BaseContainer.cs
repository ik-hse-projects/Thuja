using System;
using System.Collections.Generic;
using System.Linq;

namespace Thuja
{
    /// <summary>
    ///     Простейший контейнер, который просто объединяет несколько других виджетов в один.
    /// </summary>
    public class BaseContainer : IKeyHandler
    {
        /// <summary>
        ///     Список виджетов внутри этого контейнера.
        /// </summary>
        protected readonly List<IWidget> Widgets = new();

        /// <summary>
        ///     Сфокусированный на данный момент виджет среди всех виджетов внутри этого контейнера.
        /// </summary>
        private IFocusable? focused;

        /// <summary>
        ///     Является ли этот контейнер сфокусированным.
        /// </summary>
        private bool isFocused;

        /// <summary>
        ///     Сфокусированный на данный момент виджет среди всех виджетов внутри этого контейнера.
        /// </summary>
        public IFocusable? Focused
        {
            get => focused;
            set
            {
                if (isFocused)
                {
                    focused?.FocusChange(false);
                    value?.FocusChange(true);
                }

                focused = value;
            }
        }

        /// <inheritdoc />
        public Dictionary<HashSet<KeySelector>, Action> Actions { get; } = new();

        /// <inheritdoc />
        public virtual void Render(RenderContext context)
        {
            foreach (var widget in Widgets)
            {
                widget.Render(context);
            }
        }

        /// <inheritdoc />
        public bool CanFocus => Focused?.CanFocus ?? false;

        /// <inheritdoc />
        public virtual void FocusChange(bool isFocused)
        {
            this.isFocused = isFocused;
            focused?.FocusChange(isFocused);
        }

        /// <inheritdoc />
        public bool BubbleDown(ConsoleKeyInfo key)
        {
            if (focused != null && focused.BubbleDown(key))
            {
                return true;
            }

            return BubbleUp(key);
        }

        /// <summary>
        ///     Удаляет все виджеты внутри этого контейнера.
        /// </summary>
        public void Clear()
        {
            Focused = null;
            foreach (var widget in Widgets.ToList())
            {
                Remove(widget);
            }
        }

        /// <summary>
        ///     Помещает виджет в указанное место, сдвигая другие при необходимости.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывает исключение, если position меньше 0 или больше Widgets.Count</exception>
        /// <returns>Возвращает этот же контейнер.</returns>
        public BaseContainer Insert(int position, IWidget widget)
        {
            if (Widgets.Count == 0 && position == 0)
            {
                return Add(widget);
            }

            Widgets.Insert(position, widget);
            return this;
        }

        /// <summary>
        ///     Добавляет новый виджет в контейнер.
        /// </summary>
        /// <param name="widget">Новый виджет.</param>
        /// <returns>Возвращает этот же контейнер.</returns>
        public BaseContainer Add(IWidget widget)
        {
            if (Focused == null && widget is IFocusable focusable)
            {
                Focused = focusable;
            }

            Widgets.Add(widget);
            return this;
        }

        /// <summary>
        ///     Аналогично <see cref="Add" />, но ещё и фокусируется на этом виджете.
        /// </summary>
        /// <param name="widget">Виджет, который будет добавлен и сфокусирован.</param>
        /// <returns>Возвращает этот же контейнер.</returns>
        public BaseContainer AddFocused(IFocusable widget)
        {
            Focused = widget;
            Add(widget);
            return this;
        }

        /// <summary>
        ///     Удаляет виджет из контейнера и из цикла.
        /// </summary>
        /// <param name="widget">Виджет, который необходимо удалить.</param>
        /// <returns>Возвращает был ли такой виджет внутри этого контейнера.</returns>
        public bool Remove(IWidget widget)
        {
            var isRemoved = Widgets.Remove(widget);
            if (isRemoved && Focused == widget)
            {
                Focused = null;
            }

            return isRemoved;
        }

        /// <summary>
        ///     Удаляет виджет из контейнера и из цикла.
        /// </summary>
        /// <param name="index">Индекс виджета, который необходимо удалить.</param>
        /// <returns>Возвращает, удалось ли его удалить. Например, вернёт false, если передан некорректный индекс.</returns>
        public bool RemoveAt(int index)
        {
            if (index < 0 || index > Widgets.Count)
            {
                return false;
            }

            return Remove(Widgets[index]);
        }

        /// <summary>
        ///     Если ни один виджет внутри не смог обработать нажатие кнопки, но вызываетсся этот метод.
        /// </summary>
        /// <param name="key">Информация о нажатой клавише.</param>
        /// <returns>Удалось ли как-то обработать это нажатие.</returns>
        protected virtual bool BubbleUp(ConsoleKeyInfo key)
        {
            return AsIKeyHandler().TryHandleKey(key);
        }

        /// <summary>
        ///     Преобразовывает этот контейнер в экземпляр <see cref="IKeyHandler" />
        /// </summary>
        /// <returns>Объект типа <see cref="IKeyHandler" />, который может быть преобразован в тип этого контейнера.</returns>
        public IKeyHandler AsIKeyHandler()
        {
            return this;
        }
    }
}