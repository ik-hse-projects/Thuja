using System;
using System.Collections.Generic;

namespace Thuja
{
    public abstract class DelegateIWidget : AssertRegistered
    {
        protected abstract IWidget WidgetImplementation { get; }

        public virtual int Fps => WidgetImplementation.Fps;

        public override void OnRegistered(MainLoop loop)
        {
            WidgetImplementation.OnRegistered(loop);
        }

        public override void OnUnregistered()
        {
            WidgetImplementation.OnUnregistered();
        }

        public override void Render(RenderContext context)
        {
            WidgetImplementation.Render(context);
        }

        public virtual void Update()
        {
            WidgetImplementation.Update();
        }
    }

    public abstract class DelegateIFocusable : DelegateIWidget, IFocusable
    {
        protected override IWidget WidgetImplementation => FocusableImplementation;

        protected abstract IFocusable FocusableImplementation { get; }

        public virtual bool CanFocus => FocusableImplementation.CanFocus;

        public virtual void FocusChange(bool isFocused)
        {
            FocusableImplementation.FocusChange(isFocused);
        }

        public virtual bool BubbleDown(ConsoleKeyInfo key)
        {
            return FocusableImplementation.BubbleDown(key);
        }
    }

    public abstract class DelegateIKeyHandler : DelegateIFocusable, IKeyHandler
    {
        protected override IFocusable FocusableImplementation => KeyHandlerImplementation;
        protected abstract IKeyHandler KeyHandlerImplementation { get; }

        public virtual Dictionary<HashSet<KeySelector>, Action> Actions => KeyHandlerImplementation.Actions;
    }
}