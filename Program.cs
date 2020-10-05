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
            loop.AddFocused(new InputField
            {
                RelativePosition = (2, 2, 1),
                MaxLength = 15,
                AllowedChars = {CharRange.Ascii},
                Placeholder = new Placeholder(new Style(MyColor.Cyan, MyColor.Black), "Enter text")
            });
            loop.Start();
        }
    }
}