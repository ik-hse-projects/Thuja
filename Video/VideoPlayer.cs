using System.IO;
using System.IO.Compression;

namespace Thuja.Video
{
    /// <summary>
    /// Виджет, который бесконечно проигрывает видео.
    /// </summary>
    public class VideoPlayer : LimitFps, IWidget
    {
        /// <summary>
        /// Поток, из котрого считывается видео.
        /// </summary>
        private readonly Stream fileStream;
        
        /// <summary>
        /// Текущий кадр.
        /// </summary>
        private ColoredChar[,] frame;
        
        /// <summary>
        /// Итератор по кадрам.
        /// </summary>
        private VideoReader reader;

        public VideoPlayer(Stream file)
        {
            fileStream = file;
            frame = new ColoredChar[0, 0];
            Reset();
        }

        /// <inheritdoc />
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

        /// <summary>
        /// Обновляет кадр, если время пришло.
        /// </summary>
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

        /// <summary>
        /// Запускает видео с самого начала.
        /// </summary>
        private void Reset()
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            reader = VideoReader.Init(new BufferedStream(new BrotliStream(fileStream, CompressionMode.Decompress)));
            Fps = reader.Info.Fps;
        }
    }
}