using System;
using System.Collections.Generic;
using System.Linq;
using Thuja.Widgets;

namespace Thuja
{
    /// <summary>
    /// Позволяет создавать несколько связанных друг с другом radio-buttons
    /// </summary>
    /// <typeparam name="T">Тип выбираемого значения.</typeparam>
    public class RadioSetBuilder<T>
    {
        private readonly List<(T, Checkbox)> variants = new();

        /// <summary>
        /// Выбранное значение.
        /// </summary>
        public T? Checked { get; private set; }

        public event Action<T>? OnChecked;

        /// <summary>
        /// Снимает выделение со всех кнопок.
        /// </summary>
        private void UncheckAll()
        {
            foreach (var (_, button) in variants)
            {
                button.Checked = false;
            }
        }

        /// <summary>
        /// Создаёт новую кнопку, связанную с остальными.
        /// </summary>
        /// <param name="text">Текст кнопки.</param>
        /// <param name="value">Значение, которые будет выбрано при выборе этой кнопки.</param>
        /// <param name="widget">Виджет, который оттвечает за выбор этого варианта.</param>
        public RadioSetBuilder<T> Add(string text, T value, out IFocusable widget)
        {
            var checkbox = new Checkbox(text)
            {
                Marker = 'o'
            };
            checkbox.OnClick(() =>
            {
                UncheckAll();
                checkbox.Checked = true;
                Checked = value;
                OnChecked?.Invoke(value);
            });
            variants.Add((value, checkbox));
            widget = checkbox;
            return this;
        }

        /// <summary>
        /// Отмечает переданный вариант как выбранный.
        /// Если подходящих кнопок несколько, то отмечает ранее добавленную.
        /// Важно, что событие <see cref="OnChecked" /> всё равно будет вызвано.
        /// </summary>
        public void Check(T value)
        {
            var checkbox = variants
                .Where(i => i.Item1!.Equals(value))
                .Select(i => i.Item2)
                .FirstOrDefault();
            UncheckAll();
            if (checkbox == null)
            {
                return;
            }

            checkbox.Checked = true;
            OnChecked?.Invoke(value);
        }

        /// <summary>
        /// Создаёт новую кнопку, связанную с остальными.
        /// </summary>
        /// <param name="text">Текст кнопки.</param>
        /// <param name="value">Значение, которые будет выбрано при выборе этой кнопки.</param>
        public RadioSetBuilder<T> Add(string text, T value)
        {
            return Add(text, value, out _);
        }

        /// <summary>
        /// Собирает все виджеты в один StackContainer и возвращает его.
        /// </summary>
        public StackContainer ToStack()
        {
            var result = new StackContainer();
            foreach (var (_, button) in variants)
            {
                result.Add(button);
            }

            return result;
        }
    }
}