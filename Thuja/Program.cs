using System.IO;
using Thuja.Video;
using Thuja.Widgets;

namespace Thuja
{
    internal class Program
    {
        private static void Main(string[] args)
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
}