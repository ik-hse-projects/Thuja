using System.IO;
using System.IO.Compression;

namespace Thuja.Video
{
    public class VideoPlayer : LimitFps, IWidget
    {
        private readonly Stream fileStream;
        private ColoredChar[,] frame;
        private VideoReader reader;

        public VideoPlayer(Stream file)
        {
            fileStream = file;
            frame = new ColoredChar[0, 0];
            Reset();
        }

        public void Render(RenderContext context)
        {
            Update();

            if (frame.GetLength(0) != reader.Info.Width
                || frame.GetLength(1) != reader.Info.Height)
            {
                return;
            }

            for (var x = 0; x < reader.Info.Width; x++)
            for (var y = 0; y < reader.Info.Height; y++)
            {
                context[x, y] = frame[x, y];
            }
        }

        private void Update()
        {
            if (!IsTimeToDraw())
            {
                return;
            }

            if (!reader.MoveNext())
            {
                Reset();
                reader.MoveNext();
            }

            frame = reader.Current;
        }

        private void Reset()
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            reader = VideoReader.Init(new BufferedStream(new BrotliStream(fileStream, CompressionMode.Decompress)));
            Fps = reader.Info.Fps;
        }
    }
}