using System;
using System.Collections.Generic;
using System.Linq;

namespace Thuja.Widgets
{
    /// <summary>
    ///     Расположение <see cref="StackContainer" />.
    /// </summary>
    public enum Orientation
    {
        Vertical,
        Horizontal
    }

    /// <summary>
    ///     Контейнер, который располагает другие виджиты рядом друг с другом.
    /// </summary>
    public class StackContainer : BaseContainer, IWidget
    {
        private IFocusable? lastFocused;
        
        /// <summary>
        ///     Индекс элемента, который на данный момент сфокусирован.
        /// </summary>
        public int Position { get; private set; }

        /// <param name="orientation">Ориентация <see cref="StackContainer" />.</param>
        /// <param name="margin">Промежуток между виджетами.</param>
        /// <param name="maxVisibleCount">Максимальное количество отрисовываемых виджетов.</param>
        public StackContainer(Orientation orientation = Orientation.Vertical, int margin = 0,
            int maxVisibleCount = int.MaxValue)
        {
            Orientation = orientation;
            Margin = margin;
            MaxVisibleCount = maxVisibleCount;
        }

        /// <summary>
        ///     Ориентация.
        /// </summary>
        public Orientation Orientation { get; set; }

        /// <summary>
        ///     Промежуток между виджетами.
        /// </summary>
        public int Margin { get; set; }

        /// <summary>
        ///     Максимальное количество отрисовываемых виджетов.
        /// </summary>
        public int MaxVisibleCount { get; set; }

        /// <summary>
        ///     Вызывается, когда меняется текущий сфокусированный элемент.
        ///     Поля Position и Focused могут быть особенно полезны.
        /// </summary>
        public event Action? FocusedChanged;

        /// <summary>
        ///     Создаёт <see cref="ListOf{T}"/> на основе этого списка.
        /// </summary>
        /// <param name="converter">Функция, которая превращает элементы списка в элементы интерфейса.</param>
        public ListOf<T> ListOf<T>(Func<T, IWidget> converter)
        {
            return new ListOf<T>(this, converter);
        }

        /// <inheritdoc />
        public void Update()
        {
            if (Focused != lastFocused)
            {
                lastFocused = Focused;
                MoveSelection(0);
            }
        }

        /// <inheritdoc />
        public override void Render(RenderContext context)
        {
            var offsetY = 0;
            var offsetX = 0;
            foreach (var widget in FindVisible())
            {
                var newPosition = Orientation switch
                {
                    Orientation.Vertical => (0, offsetY, 0),
                    Orientation.Horizontal => (offsetX, 0, 0)
                };
                var ctx = context.Derive(newPosition);
                widget.Render(ctx);
                offsetX += ctx.Size.x + Margin;
                offsetY += ctx.Size.y + Margin;
            }
        }

        /// <summary>
        ///     Находит виджеты, которые должны быть видны.
        /// </summary>
        private IEnumerable<IWidget> FindVisible()
        {
            if (Widgets.Count <= MaxVisibleCount)
            {
                return Widgets;
            }

            var offsetBefore = MaxVisibleCount / 2;
            var offsetAfter = MaxVisibleCount / 2;
            if (MaxVisibleCount % 2 != 0)
            {
                offsetAfter++;
            }

            var start = Position - offsetBefore;
            var end = Position + offsetAfter;

            if (start < 0)
            {
                return Widgets.GetRange(0, MaxVisibleCount);
            }

            if (end > Widgets.Count)
            {
                return Widgets.GetRange(Widgets.Count - MaxVisibleCount, MaxVisibleCount);
            }

            return Widgets.GetRange(start, end - start);
        }

        /// <summary>
        ///     Находит виджеты, которые могут быть сфокусированы, и их индексы среди всех виджетов.
        /// </summary>
        private List<(IFocusable?, int index)> FindFocusable()
        {
            return Widgets
                .Select((widget, index) => (widget as IFocusable, index))
                .Where(w => w.Item1?.CanFocus ?? false)
                .ToList();
        }

        /// <summary>
        ///     Передивагет фокус на указанное количество элементов.
        /// </summary>
        /// <returns>Удалось ли передвинуть.</returns>
        private bool MoveSelection(int direction)
        {
            var focusable = FindFocusable();
            if (focusable.Count == 0)
            {
                return false;
            }

            var currentlySelected = focusable.FindIndex(x => ReferenceEquals(x.Item1, Focused));
            if (currentlySelected == -1)
            {
                var nearest = focusable.MinBy(i => Math.Abs(i.index - Position));
                if (nearest == null)
                {
                    (Focused, Position) = (null, 0);
                    FocusedChanged?.Invoke();
                    return false;
                }

                currentlySelected = nearest.Value.index;
            }

            var newSelected = currentlySelected + direction;
            if (newSelected < 0 || newSelected >= focusable.Count)
            {
                return false;
            }

            (Focused, Position) = focusable[newSelected];
            FocusedChanged?.Invoke();
            return true;
        }

        /// <inheritdoc />
        protected override bool BubbleUp(ConsoleKeyInfo key)
        {
            if (base.BubbleUp(key))
            {
                return true;
            }

            switch (key.Key)
            {
                case ConsoleKey.Tab when key.Modifiers.HasFlag(ConsoleModifiers.Shift):
                case ConsoleKey.UpArrow when Orientation == Orientation.Vertical:
                case ConsoleKey.LeftArrow when Orientation == Orientation.Horizontal:
                    return MoveSelection(-1);
                case ConsoleKey.Enter:
                case ConsoleKey.Tab:
                case ConsoleKey.DownArrow when Orientation == Orientation.Vertical:
                case ConsoleKey.RightArrow when Orientation == Orientation.Horizontal:
                    return MoveSelection(+1);
                case ConsoleKey.End:
                case ConsoleKey.PageDown:
                {
                    var focusable = FindFocusable();
                    if (focusable.Count == 0)
                    {
                        return false;
                    }

                    (Focused, Position) = focusable.Last();
                    FocusedChanged?.Invoke();
                    return true;
                }
                case ConsoleKey.Home:
                case ConsoleKey.PageUp:
                {
                    var focusable = FindFocusable();
                    if (focusable.Count == 0)
                    {
                        return false;
                    }

                    (Focused, Position) = focusable.First();
                    FocusedChanged?.Invoke();
                    return true;
                }
                default:
                    return false;
            }
        }
    }
}