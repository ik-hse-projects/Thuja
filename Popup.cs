using Thuja.Widgets;

namespace Thuja
{
    /// <summary>
    /// Всплывающее окно.
    /// </summary>
    public class Popup
    {
        /// <summary>
        /// Максимальная ширина.
        /// </summary>
        private readonly int maxWidth;
        
        /// <summary>
        /// Положение окна относительно контейнера.
        /// </summary>
        private readonly (int x, int y, int layer) position;
        
        /// <summary>
        /// Внутреннее содержимое окна.
        /// </summary>
        private readonly StackContainer stack = new StackContainer();

        /// <summary>
        /// Контейнер, в котором находится это окно.
        /// </summary>
        private BaseContainer? container;

        /// <summary>
        /// Последний вставленный в окно виджет.
        /// </summary>
        private IWidget? last;
        
        /// <summary>
        /// Виджет контейнера, который был сфокусирован до того, как появилось окно. 
        /// </summary>
        private IFocusable? oldFocus;
        
        /// <summary>
        /// Контейнер, который содержит себе всё необходимое для правильной отрисовки окна.
        /// </summary>
        private BaseContainer? wrapped;

        /// <summary>
        /// Создаёт новое всплывающее окно.
        /// </summary>
        /// <param name="maxWidth">Максимальная ширина.</param>
        /// <param name="maxHeight">Максимальная высота.</param>
        /// <param name="x">Отступ слева.</param>
        /// <param name="y">Остутп сверху.</param>
        /// <param name="layer">Слой, на котором отрисовывается окно.</param>
        public Popup(int maxWidth = 70, int maxHeight = 19, int x = 5, int y = 5, int layer = 10)
        {
            position = (x, y, layer);
            this.maxWidth = maxWidth;
            stack.MaxVisibleCount = maxHeight;
        }

        /// <summary>
        /// Добавляет виджет в окно и настривает его ширину.
        /// </summary>
        /// <returns>Возвращает это же всплывающее окно.</returns>
        public Popup Add(Label label)
        {
            label.MaxWidth = maxWidth;
            return Add((IWidget) label);
        }

        /// <summary>
        /// Добавляет виджет в окно и настривает его ширину.
        /// </summary>
        /// <returns>Возвращает это же всплывающее окно.</returns>
        public Popup Add(MultilineLabel label)
        {
            label.MaxWidth = maxWidth;
            return Add((IWidget) label);
        }

        /// <summary>
        /// Добавляет виджет в окно и настривает его ширину.
        /// </summary>
        /// <returns>Возвращает это же всплывающее окно.</returns>
        public Popup Add(InputField field)
        {
            field.MaxLength = maxWidth;
            return Add((IWidget) field);
        }

        /// <summary>
        /// Добавляет виджет в окно.
        /// </summary>
        /// <returns>Возвращает это же всплывающее окно.</returns>
        public Popup Add(IWidget widget)
        {
            stack.Add(widget);
            last = widget;
            return this;
        }

        /// <summary>
        /// Устанавливает фокус на последний добавленный виджет, если это возможно.
        /// </summary>
        /// <returns>Возвращает это же всплывающее окно.</returns>
        public Popup AndFocus()
        {
            if (last is IFocusable focusable)
            {
                stack.Focused = focusable;
            }

            return this;
        }

        /// <summary>
        /// Добавляет конпку с указанным текстом, нажатие на которую закрывает окно. 
        /// </summary>
        /// <param name="text">Текст на кнопке закрытия.</param>
        /// <returns>Возвращает это же всплывающее окно.</returns>
        public Popup AddClose(string text)
        {
            var button = new Button(text);
            button.AsIKeyHandler().Add(KeySelector.SelectItem, Close);
            return Add(button);
        }

        /// <summary>
        /// Отрисовывает окно в данном контейнере и устанавливает на него фокус.
        /// </summary>
        public void Show(BaseContainer root)
        {
            container = root;
            oldFocus = root.Focused;
            wrapped = new RelativePosition(position.x, position.y, position.layer).Add(
                new Frame().Add(stack)
            );

            root.AddFocused(wrapped);
        }

        /// <summary>
        /// Закрывает окно и восстанавливает состояние контейнера обратно. 
        /// </summary>
        public void Close()
        {
            if (wrapped == null)
            {
                return;
            }

            container!.Focused = oldFocus;
            container.Remove(wrapped);

            container = null;
            oldFocus = null;
            wrapped = null;
        }
    }
}