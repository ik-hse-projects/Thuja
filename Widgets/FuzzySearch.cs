using System;
using System.Collections.Generic;

namespace Thuja.Widgets
{
    /// <summary>
    ///     Виджет, который позволяет искать и выбирать строки из списка.
    /// </summary>
    public class FuzzySearch<T> : DelegateIFocusable, IKeyHandler
    {
        protected override IFocusable FocusableImplementation => container;

        private readonly InputField inputField;

        private bool isTextChanged;

        private readonly Func<T, string> converter;

        private readonly IList<T> choices;

        private readonly ListOf<T> list;

        private readonly BaseContainer container;

        public event Action<FuzzySearch<T>>? Chosen;

        /// <summary>
        ///     Выбранный элемент списка.
        /// </summary>
        public T? Choice { get; private set; }

        /// <summary>
        ///     Создаёт новый FuzzySearch.
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

        /// <summary>
        ///     Выбирает переданный элемент.
        /// </summary>
        private void Select(T selected)
        {
            Choice = selected;
            Chosen?.Invoke(this);
        }

        /// <summary>
        ///     Устанавливает обработчик события <see cref="Chosen"/>
        /// </summary>
        /// <returns>Возвращает себя.</returns>
        public FuzzySearch<T> OnChosen(Action<FuzzySearch<T>> handler)
        {
            Chosen += handler;
            return this;
        }

        /// <summary>
        ///     Число видимых вариантов.
        /// </summary>
        public int MaxVisibleCount
        {
            get => list.Widget.MaxVisibleCount;
            set => list.Widget.MaxVisibleCount = value;
        }

        /// <summary>
        ///     Максимальная длина поля для поиска.
        /// </summary>
        public int MaxLength
        {
            get => inputField.MaxLength;
            set => inputField.MaxLength = value;
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (!isTextChanged)
            {
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
        ///     Преобразовывает этот виджет в экземпляр <see cref="IKeyHandler" />
        /// </summary>
        /// <returns>Объект типа <see cref="IKeyHandler" />, который может быть преобразован в <see cref="InputField" />.</returns>
        public IKeyHandler AsIKeyHandler()
        {
            return this;
        }
    }
}