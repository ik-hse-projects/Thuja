using System;
using Thuja.Widgets;

namespace Thuja
{
    /// <summary>
    ///     Всплывающий диалог с вопросом к пользователю.
    /// </summary>
    /// <typeparam name="T">Тип выбираемого значения.</typeparam>
    public class Dialog<T>
    {
        /// <summary>
        ///     Вспомогательное всплывающее окно.
        /// </summary>
        private readonly Popup popup = new Popup();

        /// <summary>
        ///     Вопрос к пользователю.
        /// </summary>
        public string? Question { get; set; }

        /// <summary>
        ///     Возможные варианты ответа.
        /// </summary>
        public (string text, T obj)[] Answers { get; set; } = new (string, T)[0];

        /// <summary>
        ///     Вызывается, если пользователь выбирает тот или иной вариант.
        ///     Передается соответствующее значение из <see cref="Answers" />.
        /// </summary>
        public Action<T>? OnAnswered { get; set; }

        /// <summary>
        ///     Вызывается, когда пользователь отменяет действие.
        /// </summary>
        public Action? OnCancelled { get; set; }

        /// <summary>
        ///     Отображает диалог в переданном контейнере.
        /// </summary>
        /// <param name="root">Контейнер, который был выбран для отображения диалога.</param>
        public void Show(BaseContainer root)
        {
            if (Question != null)
            {
                popup.Add(new MultilineLabel(Question))
                    .Add(new Label(""));
            }

            foreach (var (text, obj) in Answers)
            {
                popup.Add(new Button(text)
                    .AsIKeyHandler()
                    .Add(KeySelector.SelectItem, () =>
                    {
                        popup.Close();
                        OnAnswered?.Invoke(obj);
                    })
                );
            }

            popup.Add(new Label("")).Add(
                new Button("Отмена")
                    .AsIKeyHandler()
                    .Add(KeySelector.SelectItem, () =>
                    {
                        popup.Close();
                        OnCancelled?.Invoke();
                    })
            );

            popup.Show(root);
        }
    }
}