using System;

namespace Thuja
{
    class Program
    {
        static void Main(string[] args)
        {
            var loop = new MainLoop();
            loop.AddFocused(new KeyPressed
            {
                Position = (3, 3)
            });
            loop.Start();
        }
    }

    class KeyPressed : IWidget
    {
        private ConsoleKeyInfo info;
        
        public (int x, int y) Position { get; set; }
        public ColoredChar?[,] Render()
        {
            var s = info.KeyChar.ToString();
            var res = new ColoredChar?[s.Length, 1];
            for (var i = 0; i < s.Length; i++)
            {
                res[i, 0] = new ColoredChar(s[i]);
            }

            return res;
        }

        public (int, int) Fps => (0, 0);
        public void Update(Tick tick)
        {
        }

        public bool BubbleDown(ConsoleKeyInfo key)
        {
            info = key;
            return true;
        }
    }
}