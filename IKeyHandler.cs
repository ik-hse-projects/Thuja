using System;
using System.Collections.Generic;
using System.Linq;

namespace Thuja
{
    /// <summary>
    /// Обработчик нажатия клавиш.
    /// </summary>
    public interface IKeyHandler : IFocusable
    {
        /// <summary>
        /// Возможные действия при нажатии тех или иных комбинаций
        /// </summary>
        Dictionary<HashSet<KeySelector>, Action> Actions { get; }

        /// <summary>
        /// Добавляет новое действие для указанной комбинации. 
        /// </summary>
        /// <param name="selector">Комбинация клавиш.</param>
        /// <param name="action">Действие, которое необходимо выполнить.</param>
        /// <returns>Возвращает себя же.</returns>
        IKeyHandler Add(KeySelector selector, Action action)
        {
            return Add(new HashSet<KeySelector> {selector}, action);
        }
        
        /// <summary>
        /// Добавляет новое действие для указанных комбинаций. 
        /// </summary>
        /// <param name="selectors">Комбинации клавиш.</param>
        /// <param name="action">Действие, которое необходимо выполнить.</param>
        /// <returns>Возвращает себя же.</returns>
        IKeyHandler Add(IEnumerable<KeySelector> selectors, Action action)
        {
            return Add(selectors.ToHashSet(), action);
        }

        /// <summary>
        /// Добавляет новое действие для множества комбинаций. 
        /// </summary>
        /// <param name="selectors">Множество комбинаций клавиш.</param>
        /// <param name="action">Действие, которое необходимо выполнить.</param>
        /// <returns>Возвращает себя же.</returns>
        IKeyHandler Add(HashSet<KeySelector> selectors, Action action)
        {
            Actions.Add(selectors, action);
            return this;
        }

        /// <summary>
        /// Обрабатывает нажатие клавиши и вызывает необходимые действия.
        /// </summary>
        /// <param name="key">Информация о нажатой клавиша.</param>
        /// <returns>true, если такая комбинация была обработана хотя бы одним обработчиком.</returns>
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