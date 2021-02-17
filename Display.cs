using System;
using System.Linq;

namespace Thuja
{
    /// <summary>
    ///     Класс, который занмается отрисовкой холста на экран консоли.
    /// </summary>
    public class Display
    {
        /// <summary>
        ///     Предыдущий холст.
        /// </summary>
        private Canvas? prev;

        /// <summary>
        ///     Отвечает за саму реализацию отрисовки.
        /// </summary>
        private IRenderer renderer;

        /// <summary>
        ///     Создаёт новый, пустой дисплей.
        /// </summary>
        public Display(IRenderer renderer)
        {
            prev = null;
            this.renderer = renderer;
            CurrentScreen = new Canvas(renderer.Size);
        }

        /// <summary>
        ///     Текущий холст, на котором следует рисовать.
        /// </summary>
        public Canvas CurrentScreen { get; private set; }

        /// <summary>
        ///     Сбраасывает настройки консоли и очищает её.
        /// </summary>
        public void Clear()
        {
            renderer.Reset();
            prev = new Canvas(CurrentScreen.Size);
        }

        /// <summary>
        ///     Отрисовывает текущий слой на экран.
        /// </summary>
        public void Draw()
        {
            var cursorPosition = renderer.Cursor;

            renderer.BeginShow();
            if (prev == null || prev.Size != CurrentScreen.Size)
            {
                Clear();
            }

            foreach (var difference in CurrentScreen.FindDifferences(prev!))
            {
                DrawDifference(difference);
            }

            renderer.Cursor = cursorPosition;
            renderer.EndShow();

            Swap();
        }

        /// <summary>
        ///     Подготавливает дисплей к отрисовке нового содержимого.
        /// </summary>
        private void Swap()
        {
            var size = renderer.Size;

            if (prev != null && prev.Size == size)
            {
                (CurrentScreen, prev) = (prev, CurrentScreen);
                CurrentScreen.Clear();
            }
            else
            {
                prev = null;
                CurrentScreen = new Canvas(size);
            }
        }

        /// <summary>
        ///     Отрисовывает ровно одно отличие.
        /// </summary>
        /// <param name="difference">Отличие, которое должно быть отрисовано.</param>
        private void DrawDifference(Difference difference)
        {
            renderer.Cursor = difference.Position;
            foreach (var (style, coloredChars) in difference.Chars.MakeGroups(c => c.Style))
            {
                var chars = coloredChars.Select(c => c.Char).ToArray();
                var str = new string(chars);
                renderer.ShowString(style, str);
            }
        }
    }
}