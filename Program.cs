using System.IO;
using Thuja.Video;
using Thuja.Widgets;

namespace Thuja
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var button = new Button("Hello");
            button.OnClick += button1 =>
            {
                
            };
            
            var root = new BaseContainer
            {
                new VideoPlayer(File.OpenRead(Path.Combine(args[0]))),
                new RelativePosition(10, 10)
                {
                    new Frame
                    {
                        new VerticalContainer
                        {
                            button,
                            new InputField
                            {
                                MaxLength = 15,
                                AllowedChars = {CharRange.Ascii},
                                Placeholder =
                                    new Placeholder(new Style(MyColor.Cyan, MyColor.Black),
                                        "Enter text")
                            },
                            new Label("World"),
                            new InputField
                            {
                                MaxLength = 5,
                                AllowedChars = {CharRange.Digits},
                                Placeholder = new Placeholder(
                                    new Style(MyColor.DarkMagenta, MyColor.Black), "Digits!")
                            }
                        }
                    }
                }
            };

            new MainLoop(root).Start();
        }
    }
}