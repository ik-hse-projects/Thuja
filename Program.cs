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
            root.AddFocused(new RelativePosition(10, 10)
            {
                new VerticalContainer
                {
                    new InputField
                    {
                        MaxLength = 15,
                        AllowedChars = {CharRange.Ascii},
                        Placeholder = new Placeholder(new Style(MyColor.Cyan, MyColor.Black), "Enter text")
                    },
                    new InputField
                    {
                        MaxLength = 5,
                        AllowedChars = {CharRange.Digits},
                        Placeholder = new Placeholder(new Style(MyColor.DarkMagenta, MyColor.Black), "Digits!")
                    }
                }
            });

            new MainLoop(root).Start();
        }
    }
}