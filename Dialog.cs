using System;
using Thuja;
using Thuja.Widgets;

namespace FileManager
{
    public class Dialog<T>
    {
        public string Question { get; set; }
        public T[] Answers { get; set; }
        public Action<T> OnAnswered { get; set; }
        public Action OnCancelled { get; set; }

        private BaseContainer container;
        private IFocusable wrapped;
        private IFocusable oldFocus;

        public void Show(BaseContainer root)
        {
            var stack = new StackContainer()
                .Add(new MultilineLabel(Question ?? "(null)", 70));

            stack.Add(new Label(""));

            foreach (var answer in Answers)
            {
                stack.Add(new Button(answer?.ToString() ?? "(null)", 70)
                    .AsIKeyHandler()
                    .Add(new[] {new KeySelector(ConsoleKey.Enter), new KeySelector(ConsoleKey.Spacebar)},
                        () => Select(answer))
                );
            }
            
            stack.Add(new Label(""));

            stack.Add(new Button("Отмена")
                .AsIKeyHandler()
                .Add(new[] {new KeySelector(ConsoleKey.Enter), new KeySelector(ConsoleKey.Spacebar)}, Cancel));

            container = root;
            oldFocus = root.Focused;
            wrapped = new RelativePosition(5, 5, 10).Add(
                new Frame().Add(stack)
            );

            root.AddFocused(wrapped);
        }

        private void Select(T answer)
        {
            OnAnswered?.Invoke(answer);
            Close();
        }

        private void Cancel()
        {
            OnCancelled?.Invoke();
            Close();
        }

        private void Close()
        {
            container.Focused = oldFocus;
            container.Remove(wrapped);
        }
    }
}