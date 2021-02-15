using System;

namespace Thuja
{
    /// <summary>
    ///     Интерфейс виджета, который может быть сфокусирован и обрабатывать нажатия клавиш.
    /// </summary>
    public interface IFocusable : IWidget
    {
        /// <summary>
        ///     Может ли виджет быть сфокусирован в этот момент.
        /// </summary>
        public bool CanFocus => true;

        /// <summary>
        ///     Вызывается когда виджет меняет свою сфокусированность.
        /// </summary>
        /// <param name="isFocused">Является ли фиджет сфокусированным теперь.</param>
        public void FocusChange(bool isFocused);

        /// <summary>
        ///     Вызывается, когда была нажата клавиша и этот виджет на данный момент сфокусирван.
        /// </summary>
        /// <param name="key">Информаиця о нажатой клавише.</param>
        /// <returns>Удалось ли обработать нажатие.</returns>
        public bool BubbleDown(ConsoleKeyInfo key);
    }

    /// <summary>
    ///     Фиджет, который может быть отрисован.
    /// </summary>
    public interface IWidget
    {
        /// <summary>
        ///     Отрисовывает виджет в переданном контексте.
        /// </summary>
        /// <param name="context">Конекст, в котором будет отрисован данный виджет.</param>
        void Render(RenderContext context);
    }
}