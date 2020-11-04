using System;

namespace Thuja
{
    /// <summary>
    ///     Комбинация клавиш.
    /// </summary>
    public readonly struct KeySelector
    {
        /// <summary>
        ///     Набор комбинаций для выбора текущего элемента.
        /// </summary>
        public static readonly KeySelector[] SelectItem =
        {
            new KeySelector(ConsoleKey.Enter),
            new KeySelector(ConsoleKey.Spacebar)
        };

        /// <summary>
        ///     Модификаторы нажатой клавиши.
        /// </summary>
        public readonly ConsoleModifiers Modifiers;

        /// <summary>
        ///     Вид нажатой клавиши. Null, если это не важно.
        /// </summary>
        public readonly ConsoleKey? Key;

        /// <summary>
        ///     Нажатый символ. Null, если это не важно.
        /// </summary>
        public readonly char? Character;

        /// <summary>
        ///     Создаёт новый KeySelector на основании вида клавиши и, возможно, модификаторов.
        /// </summary>
        /// <param name="key">Вид клавиши.</param>
        /// <param name="modifiers">Модификаторы.</param>
        public KeySelector(ConsoleKey key, ConsoleModifiers modifiers = 0)
        {
            Modifiers = modifiers;
            Key = key;
            Character = null;
        }

        /// <summary>
        ///     Создаёт новый KeySelector на основании нажатого символа и, возможно, модификаторов.
        /// </summary>
        /// <param name="character">Нажатый символ.</param>
        /// <param name="modifiers">Модификаторы.</param>
        public KeySelector(char character, ConsoleModifiers modifiers = 0)
        {
            Modifiers = modifiers;
            Key = null;
            Character = character;
        }

        /// <summary>
        ///     Проверяет соответсвует ли переданное нажатие этой комбинации.
        /// </summary>
        /// <param name="key">Информация о нажатой клавише.</param>
        /// <returns>True, если нажатие соответсвует этой клавише.</returns>
        public bool Match(ConsoleKeyInfo key)
        {
            return (Character == null || key.KeyChar == Character)
                   && (Key == null || key.Key == Key)
                   && key.Modifiers.HasFlag(Modifiers);
        }
    }
}