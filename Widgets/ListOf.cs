using System;
using System.Collections;
using System.Collections.Generic;

namespace Thuja.Widgets
{
    public class ListOf<T> : IList<T>
    {
        private readonly Func<T, IWidget> converter;
        private readonly IList<T> items;
        private readonly List<IWidget> widgets = new();

        /// <summary>
        /// Создаёт пустой ListOf{T}, который конвертирует элементы при помощи converter и помещает их в container.
        /// </summary>
        internal ListOf(StackContainer container, Func<T, IWidget> converter)
            : this(container, new List<T>(), converter)
        {
        }

        /// <summary>
        /// Создаёт ListOf{T} с элементами из items.
        /// </summary>
        internal ListOf(StackContainer container, IList<T> items, Func<T, IWidget> converter)
        {
            this.items = items;
            Widget = container;
            this.converter = converter;
            foreach (var item in items)
            {
                var converted = converter(item);
                Widget.Add(converted);
                widgets.Add(converted);
            }
        }

        /// <summary>
        /// Виджет, который содержит элементы этого списка.
        /// </summary>
        public StackContainer Widget { get; }

        /// <summary>
        /// Возвращает список, на котором основывается этот объект.
        ///
        /// Редактировать его следует очень осторожно.
        /// В частности, противопоказано менять число элементов: это чревато неожиданным исключением.
        /// После редактирования имеет смысл вызвать Update, чтобы синхронизировать данные с UI.
        /// </summary>
        public IList<T> GetInner()
        {
            return items;
        }

        /// <summary>
        /// Заново конвертирует все объекты в виджеты. Удобно, если данные изменились.
        /// </summary>
        /// <remarks>Могут быть неожиданные результаты, если вдруг изначально `container` не был пуст.</remarks>
        public void Update()
        {
            for (var i = 0; i < items.Count; i++)
            {
                Update(i);
            }
        }

        /// <summary>
        /// Заново конвертирует указанный элемент. Работает быстрее, чем <see cref="Update()" />.
        /// </summary>
        /// <remarks>Могут быть неожиданные результаты, если вдруг изначально `container` не был пуст.</remarks>
        /// <returns>false, если был передан некорректный индекс.</returns>
        public bool Update(int i)
        {
            if (i < 0 || i >= items.Count)
            {
                return false;
            }

            var updated = converter(items[i]);
            Widget.RemoveAt(i);
            Widget.Insert(i, updated);
            widgets[i] = updated;
            return true;
        }

        // Дальнейшие функции — реализиация IList<T> через list. Оставлю их без документации.

        /// <inheritdoc />
        public void Add(T item)
        {
            var widget = converter(item);
            Widget.Add(widget);
            items.Add(item);
            widgets.Add(widget);
        }

        /// <inheritdoc />
        public void Clear()
        {
            Widget.Clear();
            items.Clear();
            widgets.Clear();
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            var index = items.IndexOf(item);
            if (index == -1)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        /// <remarks>Могут быть неожиданные результаты, если вдруг изначально `container` не был пуст.</remarks>
        /// <inheritdoc />
        public void Insert(int index, T item)
        {
            var widget = converter(item);
            Widget.Insert(index, widget);
            items.Insert(index, item);
            widgets.Insert(index, widget);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            Widget.Remove(widgets[index]);
            items.RemoveAt(index);
            widgets.RemoveAt(index);
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) items).GetEnumerator();
        }

        /// <inheritdoc />
        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public int Count => items.Count;

        /// <inheritdoc />
        public bool IsReadOnly => items.IsReadOnly;

        /// <inheritdoc />
        public int IndexOf(T item)
        {
            return items.IndexOf(item);
        }

        /// <inheritdoc />
        public T this[int index]
        {
            get => items[index];
            set
            {
                var widget = converter(value);
                Widget.RemoveAt(index);
                Widget.Insert(index, widget);
                widgets[index] = widget;
                items[index] = value;
            }
        }
    }
}