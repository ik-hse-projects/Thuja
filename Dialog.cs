using System;
using Thuja;
using Thuja.Widgets;

namespace FileManager
{
    public class Dialog<T>
    {
        private readonly Popup popup = new Popup();
        public string? Question { get; set; }
        public (string text, T obj)[] Answers { get; set; } = new (string, T)[0];
        public Action<T>? OnAnswered { get; set; }
        public Action? OnCancelled { get; set; }

        public void Show(BaseContainer root)
        {
            if (Question != null)
            {
                popup.Add(new MultilineLabel(Question))
                    .Add(new Label(""));
            }

            foreach (var (text, obj) in Answers)
            {
                popup.Add(
                    new Button(text)
                        .AsIKeyHandler()
                        .Add(new[] {new KeySelector(ConsoleKey.Enter), new KeySelector(ConsoleKey.Spacebar)},
                            () => Select(obj))
                );
            }

            popup.Add(new Label("")).Add(
                new Button("Отмена")
                    .AsIKeyHandler()
                    .Add(new[] {new KeySelector(ConsoleKey.Enter), new KeySelector(ConsoleKey.Spacebar)},
                        Cancel)
            );

            popup.Show(root);
        }

        private void Select(T answer)
        {
            popup.Close();
            OnAnswered?.Invoke(answer);
        }

        private void Cancel()
        {
            popup.Close();
            OnCancelled?.Invoke();
        }
    }
}