using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Thuja.Widgets;

namespace Thuja
{
    public interface IKeyHandler: IEnumerable<(KeySelector, Action)>, IFocusable
    {
        Dictionary<KeySelector, Action> Actions { get; }
        
        IEnumerator<(KeySelector, Action)> IEnumerable<(KeySelector, Action)>.GetEnumerator()
        {
            return Actions
                .Select(kv => (kv.Key, kv.Value))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // Builder pattern
        IKeyHandler Add(KeySelector selector, Action action)
        {
            Actions.Add(selector, action);
            return this;
        }

        // IEnumerable implementation
        void Add((KeySelector selector, Action action) handler)
        {
            Add(handler.selector, handler.action);
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