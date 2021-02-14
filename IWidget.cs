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
        ///     Сколько раз в секунду требуется вызывать Update.
        ///     0, если требуется делать это как можно чаще.
        /// </summary>
        public int Fps => 0;

        /// <summary>
        ///     Вызывается в тот момент, когда виджет привязывается к определенному циклу.
        ///     С этого момента может быть вызыван Update или Draw.
        /// </summary>
        /// <param name="loop">Цикл, к которому привязан виджет.</param>
        public void OnRegistered(MainLoop loop)
        {
        }
        
        /// <summary>
        ///     Вызывается в тот момент, когда виджет отвязывается от цикла.
        /// </summary>
        public void OnUnregistered()
        {
        }

        /// <summary>
        ///     Отрисовывает виджет в переданном контексте.
        /// </summary>
        /// <param name="context">Конекст, в котором будет отрисован данный виджет.</param>
        void Render(RenderContext context);

        /// <summary>
        ///     Обновляет состояние виджета.
        ///     Вызывается циклом примерно <see cref="Fps" /> раз в секунду (по возможности).
        /// </summary>
        public void Update()
        {
        }
    }
}