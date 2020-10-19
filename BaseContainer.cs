using System;
using System.Collections.Generic;

namespace Thuja
{
    public class BaseContainer : IKeyHandler
    {
        protected readonly List<IWidget> widgets = new List<IWidget>();

        private IFocusable? focused;

        private bool isFocused;
        public MainLoop? Loop { get; private set; }

        public IFocusable? Focused
        {
            get => focused;
            set
            {
                if (isFocused)
                {
                    focused?.FocusChange(false);
                    value?.FocusChange(true);
                }

                focused = value;
            }
        }

        public Dictionary<HashSet<KeySelector>, Action> Actions { get; } =
            new Dictionary<HashSet<KeySelector>, Action>();

        public void OnRegistered(MainLoop loop)
        {
            foreach (var widget in widgets)
            {
                loop.Register(widget);
            }

            Loop = loop;
        }

        public virtual void Render(RenderContext context)
        {
            foreach (var widget in widgets)
            {
                widget.Render(context);
            }
        }

        public bool CanFocus => Focused?.CanFocus ?? false;

        public void FocusChange(bool isFocused)
        {
            this.isFocused = isFocused;
            focused?.FocusChange(isFocused);
        }

        public bool BubbleDown(ConsoleKeyInfo key)
        {
            if (focused != null && focused.BubbleDown(key))
            {
                return true;
            }

            return BubbleUp(key);
        }

        public void Clear()
        {
            Focused = null;
            widgets.Clear();
        }

        public BaseContainer Add(IWidget widget)
        {
            if (Focused == null && widget is IFocusable focusable)
            {
                Focused = focusable;
            }

            Loop?.Register(widget);
            widgets.Add(widget);
            return this;
        }

        public BaseContainer AddFocused(IFocusable widget)
        {
            Focused = widget;
            Add(widget);
            return this;
        }

        public bool Remove(IWidget widget)
        {
            Loop?.Unregister(widget);
            var isRemoved = widgets.Remove(widget);
            if (isRemoved && Focused == widget)
            {
                Focused = null;
            }

            return isRemoved;
        }

        public virtual bool BubbleUp(ConsoleKeyInfo key)
        {
            return AsIKeyHandler().TryHandleKey(key);
        }

        public IKeyHandler AsIKeyHandler()
        {
            return this;
        }
    }
}