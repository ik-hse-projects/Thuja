using System;
using System.Collections.Generic;
using System.Linq;

namespace Thuja
{
    public class ConsoleRenderer : IRenderer
    {
        public void Start()
        {
        }

        public void BeginShow()
        {
            Console.CursorVisible = false;
        }
        
        public void ShowString(Style style, string str)
        {
            if (style.Foreground == MyColor.Transparent)
            {
                str = new string(' ', str.Length);
            }

            var background = style.Background == MyColor.Transparent
                ? MyColor.Default
                : style.Background;

            var changed = false;
            if (style.Foreground != MyColor.Default)
            {
                Console.ForegroundColor = style.Foreground.ToConsoleColor();
                changed = true;
            }

            if (background != MyColor.Default)
            {
                Console.BackgroundColor = background.ToConsoleColor();
                changed = true;
            }

            Console.Write(str);

            if (changed)
            {
                Console.ResetColor();
            }
        }

        public void EndShow()
        {
            Console.CursorVisible = true;
        }

        public void Reset()
        {
            Console.ResetColor();
            Console.Clear();
        }

        public IEnumerable<ConsoleKeyInfo> GetKeys()
        {
            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(true);
                return Enumerable.Repeat(keyInfo, 1);
            }

            return Enumerable.Empty<ConsoleKeyInfo>();
        }

        public IEnumerable<Cooridantes> GetClicks()
        {
            return Enumerable.Empty<Cooridantes>();
        }

        public Cooridantes Size => new Cooridantes(Console.BufferWidth, Console.BufferHeight);

        public Cooridantes Cursor
        {
            get => new(Console.CursorLeft, Console.CursorTop);
            set => Console.SetCursorPosition(value.Column, value.Row);
        }
    }
}