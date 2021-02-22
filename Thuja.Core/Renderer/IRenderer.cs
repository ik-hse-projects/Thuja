using System;
using System.Collections.Generic;

namespace Thuja
{
    /// <summary>
    ///     Renderer — тот, кто отвечает за превращение внутренних объектов в то,
    ///     что может увидеть и потрогать пользователь.
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        ///     Вызывается при запуске MainLoop.
        /// </summary>
        void Start();
        
        /// <summary>
        ///     Вызывается перед началом отрисовки.
        /// </summary>
        void BeginShow();

        /// <summary>
        ///     Отрисовывает строку текста со стилем.
        /// </summary>
        /// <param name="style">Стиль символов строки.</param>
        /// <param name="str">Символы строки</param>
        void ShowString(Style style, string str);

        /// <summary>
        ///     Вызывается после завершения отрисовки.
        /// </summary>
        void EndShow();

        /// <summary>
        ///     Сбрасывает всё содержимое и состояние экрана.
        /// </summary>
        void Reset();

        /// <summary>
        ///     Получает от пользователя нажатия на кнопки.
        /// </summary>
        IEnumerable<ConsoleKeyInfo> GetKeys();

        /// <summary>
        ///     Получает от пользователя клики мышкой.
        /// </summary>
        IEnumerable<Cooridantes> GetClicks();

        /// <summary>
        ///     Ширина высота окна.
        /// </summary>
        Cooridantes Size { get; }

        Cooridantes Cursor { get; set; }
    }

    /// <summary>
    ///     Координаты на экране.
    /// </summary>
    public struct Cooridantes : IEquatable<Cooridantes>
    {
        public int Column { get; }
        public int Row { get; }

        public int Height => Row;
        public int Width => Column;

        public Cooridantes(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public bool Equals(Cooridantes other)
        {
            return Column == other.Column && Row == other.Row;
        }

        public override bool Equals(object? obj)
        {
            return obj is Cooridantes other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Column, Row);
        }

        public static bool operator ==(Cooridantes left, Cooridantes right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Cooridantes left, Cooridantes right)
        {
            return !left.Equals(right);
        }
    }
}