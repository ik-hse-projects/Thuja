using System;

namespace Thuja.Widgets
{
    /// <summary>
    /// Виджет, который совмещает в себе два других: один отображается, когда фокус есть, а другой — когда нет.
    /// </summary>
    public class Expandable : IFocusable
    {
        private bool isFocused;

        public Expandable(IWidget collapsed, IWidget expanded)
        {
            Collapsed = collapsed;
            Expanded = expanded;
        }

        /// <summary>
        /// Виджет, который отображается, когда фокуса нет.
        /// </summary>
        public IWidget Collapsed { get; set; }
        
        /// <summary>
        /// Виджет, который отображается, когда фокус стоит.
        /// </summary>
        public IWidget Expanded { get; set; }
        
        /// <summary>
        /// Текущий отображаемый виджет.
        /// </summary>
        public IWidget Current => isFocused ? Expanded : Collapsed;

        /// <inheritdoc />
        public void Render(RenderContext context)
        {
            Current.Render(context);
        }

        /// <inheritdoc />
        public void FocusChange(bool isFocused)
        {
            if (this.isFocused == isFocused)
            {
                return;
            }

            this.isFocused = isFocused;

            if (Expanded is IFocusable focusable)
            {
                focusable.FocusChange(isFocused);
            }
        }

        /// <inheritdoc />
        public bool BubbleDown(ConsoleKeyInfo key)
        {
            if (Current is IFocusable focusable)
            {
                return focusable.BubbleDown(key);
            }

            return false;
        }
    }
}