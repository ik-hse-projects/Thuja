using System;
using System.Collections.Generic;
using System.Linq;

namespace Thuja
{
    public readonly struct KeySelector
    {
        public static KeySelector[] SelectItem =
        {
            new KeySelector(ConsoleKey.Enter),
            new KeySelector(ConsoleKey.Spacebar)
        };

        public readonly ConsoleModifiers Modifiers;
        public readonly ConsoleKey? Key;
        public readonly char? Character;

        public KeySelector(ConsoleKey key, ConsoleModifiers modifiers = 0)
        {
            Modifiers = modifiers;
            Key = key;
            Character = null;
        }

        public KeySelector(char character, ConsoleModifiers modifiers = 0)
        {
            Modifiers = modifiers;
            Key = null;
            Character = character;
        }

        public KeySelector(ConsoleKey? key = null, char? character = null, ConsoleModifiers modifiers = 0)
        {
            Modifiers = modifiers;
            Key = key;
            Character = character;
        }

        public bool Match(in ConsoleKeyInfo key)
        {
            return (Character == null || key.KeyChar == Character)
                   && (Key == null || key.Key == Key)
                   && key.Modifiers.HasFlag(Modifiers);
        }
    }

    public interface IKeyHandler : IFocusable
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