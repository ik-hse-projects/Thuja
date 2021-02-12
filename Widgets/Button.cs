using System;
using System.Collections.Generic;

namespace Thuja.Widgets
{
    /// <summary>
    ///     Обычная кнопка с текстом, которую можно нажимать.
    /// </summary>
    public class Button : Label, IKeyHandler
    {
        private bool isFocused;

        /// <summary>
        ///     Создаёт новую кнопку с указаным текстом и, возможно, максимальной шириной.
        /// </summary>
        public Button(string text, int maxWidth = int.MaxValue) : base(text, maxWidth)
        {
        }

        /// <summary>
        ///     Стиль, с которым на данный момент отображается кнопка.
        ///     Зависит от сфокусированности, менять смысла нет.
        /// </summary>
        public override Style CurrentStyle => OverrideStyle ?? (isFocused ? Style.Active : Style.Inactive);

        /// <summary>
        ///     Стиль кнопки, который будт использововаться, независимо от её сфокусированности.
        /// </summary>
        public Style? OverrideStyle { get; set; }

        /// <inheritdoc />
        public Dictionary<HashSet<KeySelector>, Action> Actions { get; } = new();

        /// <summary>
        ///     Добавляет обработчик нажатия на кнопку.
        /// </summary>
        /// <returns>Возвращает эту же самую кнопку.</returns>
        public Button OnClick(Action clicked)
        {
            AsIKeyHandler().Add(KeySelector.SelectItem, clicked);
            return this;
        }

        /// <inheritdoc />
        public override void Render(RenderContext context)
        {
            if (isFocused)
            {
                context.CursorPosition = (0, 0);
            }

            base.Render(context);
        }

        /// <inheritdoc />
        public bool BubbleDown(ConsoleKeyInfo key)
        {
            return AsIKeyHandler().TryHandleKey(key);
        }

        /// <inheritdoc />
        public void FocusChange(bool isFocused)
        {
            this.isFocused = isFocused;
        }

        /// <summary>
        ///     Преобразовывает этот контейнер в экземпляр <see cref="IKeyHandler" />
        /// </summary>
        /// <returns>Объект типа <see cref="IKeyHandler" />, который может быть преобразован в <see cref="Button" />.</returns>
        public IKeyHandler AsIKeyHandler()
        {
            return this;
        }
    }
}