using System;
using System.Collections;
using System.Collections.Generic;

namespace Thuja
{
    public class BaseContainer : IFocusable, IEnumerable<IWidget>
    {
        protected readonly List<IWidget> widgets = new List<IWidget>();

        private IFocusable? focused;

        private bool isFocused;
        protected MainLoop? loop;

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

        public IEnumerator<IWidget> GetEnumerator()
        {
            return widgets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            Focused = null;
            widgets.Clear();
        }

        public void OnRegistered(MainLoop loop)
        {
            foreach (var widget in widgets) loop.Register(widget);

            this.loop = loop;
        }

        public virtual void Render(RenderContext context)
        {
            foreach (var widget in widgets) widget.Render(context);
        }

        public bool CanFocus => Focused?.CanFocus ?? false;

        public void FocusChange(bool isFocused)
        {
            this.isFocused = isFocused;
            focused?.FocusChange(isFocused);
        }

        public bool BubbleDown(ConsoleKeyInfo key)
        {
            if (focused != null && focused.BubbleDown(key)) return true;

            return BubbleUp(key);
        }

        public void Add(IWidget widget)
        {
            if (Focused == null && widget is IFocusable focusable) Focused = focusable;
            loop?.Register(widget);
            widgets.Add(widget);
        }

        public void AddFocused(IFocusable widget)
        {
            Focused = widget;
            Add(widget);
        }

        public virtual bool BubbleUp(ConsoleKeyInfo key)
        {
            return false;
        }
    }
}