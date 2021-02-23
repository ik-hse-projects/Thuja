using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Thuja.Video
{
    /// <summary>
    /// Метаданные видео. Не само содержимое, но тоже очень важное.
    /// </summary>
    public readonly struct VideoInfo
    {
        /// <summary>
        /// Ширина кадров.
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// Высота кадров.
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// Скорость смены кадров.
        /// </summary>
        public readonly int Fps;

        public VideoInfo(int width, int height, int fps)
        {
            Width = width;
            Height = height;
            Fps = fps;
        }

        /// <summary>
        /// Сколько символов содержится в одном кадре.
        /// </summary>
        public int TotalLength => Width * Height;
    }

    /// <summary>
    /// Класс, который считывает кадры из файла с видео.
    /// </summary>
    public class VideoReader : IEnumerator<ColoredChar[,]>
    {
        /// <summary>
        /// Место для закодированных данных одного кадра.
        /// </summary>
        private readonly byte[] buffer;

        /// <summary>
        /// Информация о видео в целом.
        /// </summary>
        public readonly VideoInfo Info;

        /// <summary>
        /// Поток, из которого считывается видео.
        /// </summary>
        private readonly Stream stream;

        private VideoReader(VideoInfo info, Stream stream)
        {
            Info = info;
            this.stream = stream;
            Current = new ColoredChar[Info.Width, Info.Height];
            buffer = new byte[info.TotalLength * 5];
        }

        /// <summary>
        /// Текущий кадр (декодирован).
        /// </summary>
        public ColoredChar[,] Current { get; }

        /// <summary>
        /// Если <see cref="stream"/> поддерживает Seek, то отматывает видео в самое начало.
        /// </summary>
        public void Reset()
        {
            stream.Seek(6, SeekOrigin.Begin);
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            var bytesRead = stream.Read(buffer);
            if (bytesRead == 0)
            {
                return false;
            }

            if (bytesRead != Info.TotalLength * 5)
            {
                throw new Exception("Can't read full frame");
            }

            var offset = 0;
            for (var y = 0; y < Info.Height; y++)
            for (var x = 0; x < Info.Width; x++)
            {
                var style = BitConverter.ToUInt16(buffer, offset);
                var foreground = MyColorExt.FromInt((style & 0b11111_00000_000000) >> 11);
                var background = MyColorExt.FromInt((style & 0b00000_11111_000000) >> 6);
                var flags = (byte) (style & 0b00000_00000_111111);

                if (!Enum.IsDefined(typeof(MyColor), foreground))
                {
                    foreground = default;
                }

                if (!Enum.IsDefined(typeof(MyColor), background))
                {
                    background = default;
                }

                // Собираем из трёх байтов один int. Порядок little-endian, старшие разряды остаются пустыми.
                var charNumber =
                    buffer[offset + 2]
                    | (buffer[offset + 3] << 8)
                    | (buffer[offset + 4] << 16);
                var character = (char) charNumber;
                offset += 5;

                Current[x, y] = new ColoredChar(new Style(foreground, background), character);
            }

            return true;
        }

        /// <inheritdoc />
        object IEnumerator.Current => Current;

        /// <inheritdoc />
        public void Dispose()
        {
            stream.Dispose();
        }

        /// <summary>
        /// Считывает информацию о видео из потока и создаёт итератор по кадрам этог видео.
        /// </summary>
        public static VideoReader Init(Stream stream)
        {
            var buffer = new byte[6];
            var bytesRead = stream.Read(buffer, 0, 6);
            if (bytesRead != 6)
            {
                throw new Exception("Can't read header");
            }

            var width = BitConverter.ToInt16(buffer, 0);
            var height = BitConverter.ToInt16(buffer, 2);
            var fps = buffer[4];

            return new VideoReader(new VideoInfo(width, height, fps), stream);
        }
    }
}