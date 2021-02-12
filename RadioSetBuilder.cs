using System.Collections.Generic;
using Thuja.Widgets;

namespace Thuja
{
    /// <summary>
    ///     Позволяет создавать несколько связанных друг с другом radio-buttons
    /// </summary>
    /// <typeparam name="T">Тип выбираемого значения.</typeparam>
    public class RadioSetBuilder<T>
    {
        private List<Button> buttons = new();

        /// <summary>
        ///     Выбранное значение.
        /// </summary>
        public T? Checked { get; private set; }

        /// <summary>
        ///     Снимает выделение со всех кнопок.
        /// </summary>
        private void UncheckAll()
        {
            foreach (var button in buttons)
            {
                var prefix = "[ ] ";
                var text = button.Text.Substring(prefix.Length);
                button.Text = $"{prefix}{text}";
            }
        }

        /// <summary>
        ///     Создаёт новую кнопку, связанную с остальными. 
        /// </summary>
        /// <param name="text">Текст кнопки.</param>
        /// <param name="value">Значение, которые будет выбрано при выборе этой кнопки.</param>
        /// <param name="widget">Виджет, который оттвечает за выбор этого варианта.</param>
        public RadioSetBuilder<T> Add(string text, T value, out IFocusable widget)
        {
            var button = new Button($"[ ] {text}");
            button.OnClick(() =>
            {
                UncheckAll();
                button.Text = $"[x] {text}";
                Checked = value;
            });
            buttons.Add(button);
            widget = button;
            return this;
        }

        /// <summary>
        ///     Создаёт новую кнопку, связанную с остальными. 
        /// </summary>
        /// <param name="text">Текст кнопки.</param>
        /// <param name="value">Значение, которые будет выбрано при выборе этой кнопки.</param>
        public RadioSetBuilder<T> Add(string text, T value) => Add(text, value, out _);

        /// <summary>
        ///     Собирает все виджеты в один StackContainer и возвращает его.
        /// </summary>
        public StackContainer ToStack()
        {
            var result = new StackContainer();
            foreach (var button in buttons)
            {
                result.Add(button);
            }

            return result;
        }
    }
}