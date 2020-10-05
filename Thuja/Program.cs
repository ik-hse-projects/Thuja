using System;
using System.IO;

namespace Thuja
{
    class Program
    {
        static void Main(string[] args)
        {
            var loop = new MainLoop();
            loop.Add(new VideoPlayer(File.OpenRead(Path.Combine(args[0]))));
            loop.AddFocused(new KeyPressed
            {
                Position = (3, 3, 1)
            });
            loop.Start();
        }
    }

    class KeyPressed : IWidget
    {
        private ConsoleKeyInfo info;
        
        public (int x, int y, int layer) Position { get; set; }
        public ColoredChar?[,] Render()
        {
            var s = info.KeyChar.ToString();
            var res = new ColoredChar?[s.Length, 1];
            for (var i = 0; i < s.Length; i++)
            {
                res[i, 0] = new ColoredChar(new Style(MyColor.White, MyColor.Red), s[i]);
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