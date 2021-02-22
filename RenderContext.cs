using System;

namespace Thuja
{
    /// <summary>
    /// Вспомогательный класс для отрисовки.
    /// </summary>
    public class RenderContext
    {
        /// <summary>
        /// Положение начала кординат контекста относительно глобального начала координат.
        /// </summary>
        private readonly (int x, int y, int layer) absoluteOffset;

        /// <summary>
        /// Холст, на который происходит отрисовка.
        /// </summary>
        private readonly Canvas canvas;

        /// <summary>
        /// Ссылка на родительский контекст, если такой есть.
        /// </summary>
        private readonly ParentRef? parentRef;

        /// <summary>
        /// Текущий размер содержимого.
        /// Начианется с нуля, т.е. (0, 0) соответсвует виджету размером 1x1.
        /// </summary>
        private (int x, int y) size;

        /// <summary>
        /// Приватный конструктор, который позволяет установить все поля.
        /// </summary>
        private RenderContext((int x, int y, int layer) absoluteOffset, Canvas canvas, ParentRef parentRef) : this(
            absoluteOffset, canvas)
        {
            this.parentRef = parentRef;
        }

        /// <summary>
        /// Создаёт новый корневой контекст с заданным положением и используя данный холст.
        /// </summary>
        public RenderContext((int x, int y, int layer) absoluteOffset, Canvas canvas)
        {
            this.absoluteOffset = absoluteOffset;
            this.canvas = canvas;
            size = (-1, -1);
        }

        /// <summary>
        /// Размер содержимого внутри этого контекста.
        /// </summary>
        public (int x, int y) Size => (size.x + 1, size.y + 1);

        /// <summary>
        /// Помещает символ по указанным координатам.
        /// </summary>
        public ColoredChar this[int x, int y]
        {
            get => Get(x, y) ?? throw new IndexOutOfRangeException();
            set => Set(x, y, value);
        }

        /// <summary>
        /// Устаналивает положение курсора консоли.
        /// </summary>
        public (int x, int y) CursorPosition
        {
            get => (
                Console.CursorLeft - absoluteOffset.x,
                Console.CursorTop - absoluteOffset.y
            );
            set
            {
                Console.CursorLeft = absoluteOffset.x + value.x;
                Console.CursorTop = absoluteOffset.y + value.y;
            }
        }

        /// <summary>
        /// Возвращает символ по указанным координатам,
        /// если они не превышают размер холста после пересчета в абсолютные.
        /// </summary>
        public ColoredChar? Get(int x, int y)
        {
            return canvas.Get(x + absoluteOffset.x, y + absoluteOffset.y);
        }

        /// <summary>
        /// Устаналивает данный символ по указанным относительным координатам.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Если переданные координаты отрицательны</exception>
        public void Set(int x, int y, ColoredChar ch)
        {
            if (x < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (y < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            UpdateSize(x, y);
            if (absoluteOffset.layer != 0)
            {
                ch = new ColoredChar(ch.Style, ch.Char, ch.Layer + absoluteOffset.layer);
            }

            canvas.TrySet(x + absoluteOffset.x, y + absoluteOffset.y, ch);
        }

        /// <summary>
        /// Перерасчитывает свой <see cref="Size" /> и размеры всех родителбских контекстов,
        /// если считать, что указанные координаты используются содержимым.
        /// </summary>
        private void UpdateSize(int x, int y)
        {
            if (x > size.x)
            {
                size.x = x;
            }

            if (y > size.y)
            {
                size.y = y;
            }

            if (parentRef != null)
            {
                var p = parentRef.Value;
                p.Parent.UpdateSize(x + p.RelativeOffset.x, y + p.RelativeOffset.y);
            }
        }

        /// <summary>
        /// Создает новый контекст на основе существующего,
        /// отрисовывая всё его содержимое с заданным отступом относительно содержимого текщуго.
        /// </summary>
        public RenderContext Derive((int x, int y, int layer) offset)
        {
            var newOffset = (
                absoluteOffset.x + offset.x,
                absoluteOffset.y + offset.y,
                absoluteOffset.layer + offset.layer
            );
            return new RenderContext(newOffset, canvas, new ParentRef(this, offset));
        }

        /// <summary>
        /// Отрисовывает в начале координат контекста строку, используя указанный стиль.
        /// </summary>
        public void PlaceString(string str, Style style)
        {
            for (var i = 0; i < str.Length; i++)
            {
                this[i, 0] = new ColoredChar(style, str[i]);
            }
        }

        /// <summary>
        /// Вспомогательная структура: содержит ссылку на родителя и информаию о смещении относительно его.
        /// </summary>
        private readonly struct ParentRef
        {
            /// <summary>
            /// Ссылка на родителя.
            /// </summary>
            public readonly RenderContext Parent;

            /// <summary>
            /// Информаиця о смещении относительно его.
            /// </summary>
            public readonly (int x, int y, int layer) RelativeOffset;

            /// <summary>
            /// Создает структуру, заполняя все поля.
            /// </summary>
            public ParentRef(RenderContext parent, (int x, int y, int layer) relativeOffset)
            {
                Parent = parent;
                RelativeOffset = relativeOffset;
            }
        }
    }
}