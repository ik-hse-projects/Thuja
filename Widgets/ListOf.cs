using System;
using System.Collections;
using System.Collections.Generic;

namespace Thuja.Widgets
{
    public class ListOf<T> : IList<T>
    {
        private StackContainer container;
        private Func<T, IWidget> converter;
        private List<T> items = new();
        private List<IWidget> widgets = new();

        /// <summary>
        ///     Виджет, который содержит элементы этого списка.
        /// </summary>
        public IWidget Widget => container;

        /// <summary>
        ///     Создаёт пустой ListOf{T}, который конвертирует элементы при помощи converter и помещает их в container.  
        /// </summary>
        internal ListOf(StackContainer container, Func<T, IWidget> converter)
        {
            this.container = container;
            this.converter = converter;
        }
        
        // Дальнейшие функции — реализиация IList<T> через list. Оставлю их без документации.

        /// <inheritdoc />
        public void Add(T item)
        {
            var widget = converter(item);
            container.Add(widget);
            items.Add(item);
            widgets.Add(widget);
        }

        /// <inheritdoc />
        public void Clear()
        {
            container.Clear();
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
            container.Insert(index, widget);
            items.Insert(index, item);
            widgets.Insert(index, widget);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            container.Remove(widgets[index]);
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
        public bool IsReadOnly => ((IList<T>) items).IsReadOnly;

        /// <inheritdoc />
        public int IndexOf(T item)
        {
            return items.IndexOf(item);
        }

        /// <inheritdoc />
        public T this[int index]
        {
            get => items[index];
            set => items[index] = value;
        }
    }
}