using System;
using Thuja;
using Thuja.Widgets;

namespace FileManager
{
    public class Dialog<T>
    {
        public string Question { get; set; }
        public (string text, T obj)[] Answers { get; set; } = new (string, T)[0];
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

            foreach (var (text, obj) in Answers)
            {
                stack.Add(new Button(text ?? "(null)", 70)
                    .AsIKeyHandler()
                    .Add(new[] {new KeySelector(ConsoleKey.Enter), new KeySelector(ConsoleKey.Spacebar)},
                        () => Select(obj))
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
            Close();
            OnAnswered?.Invoke(answer);
        }

        private void Cancel()
        {
            Close();
            OnCancelled?.Invoke();
        }

        private void Close()
        {
            container.Focused = oldFocus;
            container.Remove(wrapped);
        }
    }
}