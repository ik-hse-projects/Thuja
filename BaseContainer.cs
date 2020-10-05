using System;
using System.Collections;
using System.Collections.Generic;

namespace Thuja
{
    public abstract class BaseContainer : IFocusable, IEnumerable<IWidget>
    {
        protected MainLoop? loop;
        protected readonly List<IWidget> widgets = new List<IWidget>();

        private IFocusable? focused;
        public IFocusable? Focused
        {
            get => focused;
            set
            {
                focused?.FocusChange(false);
                value?.FocusChange(true);
                focused = value;
            }
        }

        public void Add(IWidget widget)
        {
            loop?.Register(widget);
            widgets.Add(widget);
        }

        public void AddFocused(IFocusable widget)
        {
            Add(widget);
            Focused = widget;
        }

        public void OnRegistered(MainLoop loop)
        {
            foreach (var widget in widgets)
            {
                loop.Register(widget);
            }

            this.loop = loop;
        }

        public abstract void Render(RenderContext context);

        public void FocusChange(bool isFocused)
        {
            focused?.FocusChange(isFocused);
        }

        public virtual bool BubbleUp(ConsoleKeyInfo key) => false;

        public bool BubbleDown(ConsoleKeyInfo key)
        {
            if (focused != null && focused.BubbleDown(key))
            {
                return true;
            }

            return BubbleUp(key);
        }

        public IEnumerator<IWidget> GetEnumerator()
        {
            return widgets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}