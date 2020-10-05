using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading;

namespace Thuja
{
    public class VideoPlayer: IWidget
    {
        public static void Play(string path)
        {
            var root = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var file = File.OpenRead(Path.Combine(root, path));

            var loop = new MainLoop();
            loop.Add(new VideoPlayer(file));
            loop.Start();
        }
        
        public VideoPlayer(FileStream file)
        {
            FileStream = file;
            Frame = new ColoredChar?[0,0];
            Reset();
        }

        private FileStream FileStream;
        private ColoredChar?[,] Frame;
        private VideoReader Reader;
        
        public (int x, int y) Position { get; set; }
        public ColoredChar?[,] Render() => Frame;

        public (int, int) Fps => (Reader.Info.Fps1, Reader.Info.Fps2);
        public void Update(Tick tick)
        {
            if (!Reader.MoveNext())
            {
                Reset();
                Reader.MoveNext();
            }
            Frame = Reader.Current;
        }

        private void Reset()
        {
            FileStream.Seek(0, SeekOrigin.Begin);
            Reader = VideoReader.Init(new BufferedStream(new BrotliStream(FileStream, CompressionMode.Decompress)));
        }

        public bool BubbleDown(ConsoleKeyInfo key)
        {
            return false;
        }
    }
}
