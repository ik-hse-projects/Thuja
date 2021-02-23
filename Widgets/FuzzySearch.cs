using System;
using System.Collections.Generic;

namespace Thuja.Widgets
{
    /// <summary>
    /// Виджет, который позволяет искать и выбирать строки из списка.
    /// </summary>
    public class FuzzySearch<T> : DelegateIFocusable, IKeyHandler
    {
        /// <summary>
        /// Объекты, среди которых происходит поиск.
        /// </summary>
        private readonly IList<T> choices;

        /// <summary>
        /// Контейнер, в котормом находится всё содержимое виджета.
        /// </summary>
        private readonly BaseContainer container;

        /// <summary>
        /// Ф-ция, ктрая преобразует <typeparamref name="T"/> в строку. 
        /// </summary>
        private readonly Func<T, string> converter;

        /// <summary>
        /// Поле, в которое вводится текс, который будет искаться.
        /// </summary>
        private readonly InputField inputField;

        /// <summary>
        /// <see cref="choices"/>, но в UI.
        /// </summary>
        private readonly ListOf<T> list;

        /// <summary>
        /// Изменился ли с прошлого обновления текст.
        /// </summary>
        private bool isTextChanged;

        /// <summary>
        /// Создаёт новый FuzzySearch.
        /// </summary>
        /// <param name="choices">Доустпные варианты для выбора.</param>
        /// <param name="converter">Функция, проеобразовывающая элемент в строку. Вызывается очень часто.</param>
        public FuzzySearch(IList<T> choices, Func<T, string> converter)
        {
            inputField = new InputField
            {
                Placeholder = new Placeholder(Style.Decoration, "(поиск)")
            };
            inputField.TextChanged += () => isTextChanged = true;
            list = new StackContainer
            {
                MaxVisibleCount = 10
            }.ListOf<T>(s => new Button(converter(s)).OnClick(() => Select(s)));
            container = new StackContainer()
                .Add(inputField)
                .Add(list.Widget);
            this.converter = converter;
            this.choices = choices;
            foreach (var choice in choices)
            {
                list.Add(choice);
            }
        }

        /// <inheritdoc />
        protected override IFocusable FocusableImplementation => container;

        /// <summary>
        /// Выбранный элемент списка.
        /// </summary>
        public T? Choice { get; private set; }

        /// <summary>
        /// Число видимых вариантов.
        /// </summary>
        public int MaxVisibleCount
        {
            get => list.Widget.MaxVisibleCount;
            set => list.Widget.MaxVisibleCount = value;
        }

        /// <summary>
        /// Максимальная длина поля для поиска.
        /// </summary>
        public int MaxLength
        {
            get => inputField.MaxLength;
            set => inputField.MaxLength = value;
        }

        /// <inheritdoc />
        public override void Render(RenderContext context)
        {
            if (!isTextChanged)
            {
                base.Render(context);
                return;
            }

            var search = inputField.Text.ToString();
            list.Clear();
            foreach (var choice in choices)
            {
                if (converter(choice).Contains(search))
                {
                    list.Add(choice);
                }
            }

            isTextChanged = false;

            base.Render(context);
        }

        /// <inheritdoc />
        public override bool BubbleDown(ConsoleKeyInfo key)
        {
            if (AsIKeyHandler().TryHandleKey(key))
            {
                return true;
            }

            // У поля поиска есть приоритет: оно обрабатывает все нажатия, независимо от сфокусированности.
            if (inputField.BubbleDown(key))
            {
                container.Focused = inputField;
                return true;
            }

            return container.BubbleDown(key);
        }

        /// <inheritdoc />
        public Dictionary<HashSet<KeySelector>, Action> Actions { get; } = new();

        /// <summary>
        /// Вызывается, когда пользователь выбирает элемент.
        /// </summary>
        public event Action<FuzzySearch<T>>? Chosen;

        /// <summary>
        /// Выбирает переданный элемент.
        /// </summary>
        private void Select(T selected)
        {
            Choice = selected;
            Chosen?.Invoke(this);
        }

        /// <summary>
        /// Устанавливает обработчик события <see cref="Chosen" />
        /// </summary>
        /// <returns>Возвращает себя.</returns>
        public FuzzySearch<T> OnChosen(Action<FuzzySearch<T>> handler)
        {
            Chosen += handler;
            return this;
        }

        /// <summary>
        /// Преобразовывает этот виджет в экземпляр <see cref="IKeyHandler" />.
        /// </summary>
        /// <returns>Объект типа <see cref="IKeyHandler" />, который может быть преобразован в <see cref="FuzzySearch{T}" />.</returns>
        public IKeyHandler AsIKeyHandler()
        {
            return this;
        }
    }
}