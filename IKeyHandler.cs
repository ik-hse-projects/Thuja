using System;
using System.Collections.Generic;
using System.Linq;

namespace Thuja
{
    public readonly struct KeySelector
    {
        public readonly ConsoleModifiers Modifiers;
        public readonly ConsoleKey Key;

        public KeySelector(ConsoleKey key, ConsoleModifiers modifiers = 0)
        {
            Modifiers = modifiers;
            Key = key;
        }

        public bool Match(in ConsoleKeyInfo key)
        {
            return key.Modifiers.HasFlag(Modifiers) && key.Key == Key;
        }
    }
    
    public interface IKeyHandler: IFocusable
    {
        Dictionary<HashSet<KeySelector>, Action> Actions { get; }

        // Builder pattern
        IKeyHandler Add(KeySelector selector, Action action)
        {
            return Add(new HashSet<KeySelector> {selector}, action);
        }
        
        IKeyHandler Add(IEnumerable<KeySelector> selectors, Action action)
        {
            return Add(selectors.ToHashSet(), action);
        }
        
        IKeyHandler Add(HashSet<KeySelector> selectors, Action action)
        {
            Actions.Add(selectors, action);
            return this;
        }

        bool TryHandleKey(ConsoleKeyInfo key)
        {
            var result = false;
            foreach (var (selectors, handler) in Actions)
            {
                if (selectors.Any(selector => selector.Match(key)))
                {
                    handler();
                    result = true;
                }
            }

            return result;
        }
    }
}