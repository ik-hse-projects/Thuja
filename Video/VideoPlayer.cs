using System;
using System.IO;
using System.IO.Compression;

namespace Thuja.Video
{
    public class VideoPlayer : AssertRegistered
    {
        private readonly FileStream fileStream;
        private ColoredChar[,] frame;
        private VideoReader reader;

        public VideoPlayer(FileStream file)
        {
            fileStream = file;
            frame = new ColoredChar[0, 0];
            Reset();
        }

        public override void Render(RenderContext context)
        {
            for (var x = 0; x < reader.Info.Width; x++)
            for (var y = 0; y < reader.Info.Height; y++)
                context[x, y] = frame[x, y];
        }

        public int Fps => reader.Info.Fps;

        public void Update()
        {
            if (!reader.MoveNext())
            {
                Reset();
                reader.MoveNext();
            }

            frame = reader.Current;
        }

        public bool BubbleDown(ConsoleKeyInfo key)
        {
            return false;
        }

        private void Reset()
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            reader = VideoReader.Init(new BufferedStream(new BrotliStream(fileStream, CompressionMode.Decompress)));
        }
    }
}