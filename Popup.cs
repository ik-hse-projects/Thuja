using Thuja.Widgets;

namespace Thuja
{
    public class Popup
    {
        private readonly int maxWidth;
        private readonly (int x, int y, int layer) position;
        private readonly StackContainer stack = new StackContainer();

        private BaseContainer? container;

        private IWidget? last;
        private IFocusable? oldFocus;
        private BaseContainer? wrapped;

        public Popup(int maxWidth = 70, int x = 5, int y = 5, int layer = 10)
        {
            position = (x, y, layer);
            this.maxWidth = maxWidth;
        }

        public Popup Add(Label label)
        {
            label.MaxWidth = maxWidth;
            return Add((IWidget) label);
        }

        public Popup Add(MultilineLabel label)
        {
            label.MaxWidth = maxWidth;
            return Add((IWidget) label);
        }

        public Popup Add(InputField field)
        {
            field.MaxLength = maxWidth;
            return Add((IWidget) field);
        }

        public Popup Add(IWidget widget)
        {
            stack.Add(widget);
            last = widget;
            return this;
        }

        public Popup AndFocus()
        {
            if (last is IFocusable focusable)
            {
                stack.Focused = focusable;
            }

            return this;
        }

        public BaseContainer InnerContent()
        {
            return stack;
        }

        public Popup Show(BaseContainer root)
        {
            container = root;
            oldFocus = root.Focused;
            wrapped = new RelativePosition(position.x, position.y, position.layer).Add(
                new Frame().Add(stack)
            );

            root.AddFocused(wrapped);

            return this;
        }

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