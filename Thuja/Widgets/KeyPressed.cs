using System;

namespace Thuja.Widgets
{
    internal class KeyPressed : IWidget
    {
        private ConsoleKeyInfo info;

        public (int x, int y, int layer) Position { get; set; }

        public ColoredChar[,] Render()
        {
            var s = info.KeyChar.ToString();
            var res = new ColoredChar[s.Length, 1];
            for (var i = 0; i < s.Length; i++) res[i, 0] = new ColoredChar(new Style(MyColor.White, MyColor.Red), s[i]);

            return res;
        }

        public int Fps => 0;

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