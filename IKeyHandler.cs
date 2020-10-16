using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Thuja.Widgets;

namespace Thuja
{
    public interface IKeyHandler: IFocusable
    {
        Dictionary<KeySelector, Action> Actions { get; }

        // Builder pattern
        IKeyHandler Add(KeySelector selector, Action action)
        {
            Actions.Add(selector, action);
            return this;
        }

        bool TryHandleKey(ConsoleKeyInfo key)
        {
            var result = false;
            foreach (var (selector, handler) in Actions)
            {
                if (selector.Match(key))
                {
                    handler();
                    result = true;
                }
            }

            return result;
        }
    }
}