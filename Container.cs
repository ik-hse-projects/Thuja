using System;
using System.Collections.Generic;

namespace Thuja
{
    public class Container : IFocusable
    {
        private MainLoop? loop;
        private IFocusable? focused;
        protected readonly List<IWidget> widgets = new List<IWidget>();

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

        public (int x, int y, int layer) RelativePosition { get; set; }

        public void Render(RenderContext context)
        {
            foreach (var widget in widgets)
            {
                widget.Render(context.Derive(widget.RelativePosition));
            }
        }

        public void FocusChange(bool isFocused)
        {
            focused?.FocusChange(isFocused);
        }

        public bool BubbleDown(ConsoleKeyInfo key)
        {
            return focused?.BubbleDown(key) ?? false;
        }
    }
}