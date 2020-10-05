using System.IO;
using Thuja.Video;
using Thuja.Widgets;

namespace Thuja
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var root = new Container();
            root.Add(new VideoPlayer(File.OpenRead(Path.Combine(args[0]))));
            root.AddFocused(new InputField
            {
                RelativePosition = (2, 2, 1),
                MaxLength = 15,
                AllowedChars = {CharRange.Ascii},
                Placeholder = new Placeholder(new Style(MyColor.Cyan, MyColor.Black), "Enter text")
            });

            new MainLoop(root).Start();
        }
    }
}