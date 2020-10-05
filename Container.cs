using System.Collections.Generic;

namespace Thuja
{
    public abstract class Container
    {
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
    }
}