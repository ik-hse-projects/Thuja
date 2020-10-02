using System;

namespace Тhuja
{
    public class Display
    {
        private Screen? _prev;
        private Screen _curr;

        public Display()
        {
            _prev = null;
            _curr = new Screen(Console.WindowWidth, Console.WindowHeight);
        }

        public Screen CurrentScreen()
        {
            return _curr;
        }

        public void Clear()
        {
            Console.Clear();
            _prev = new Screen(_curr.Size.width, _curr.Size.height);
        }

        public void Draw()
        {
            if (_prev == null || _prev.Size != _curr.Size)
            {
                Clear();
            }
            
            foreach (var difference in _curr.FindDifferences(_prev))
            {
                Console.SetCursorPosition(difference.Column, difference.Line);
                var s = new string(difference.Chars.ToArray());
                Console.Write(s);
            }
            Console.SetCursorPosition(0, 0);

            Swap();
        }

        private void Swap()
        {
            var width = Console.WindowWidth;
            var height = Console.WindowHeight;

            if (_prev != null && _prev.Size == (width, height))
            {
                (_curr, _prev) = (_prev, _curr);
                _curr.Clear(' ');
            }
            else
            {
                _prev = null;
                _curr = new Screen(width, height);
            }
        }
    }
}