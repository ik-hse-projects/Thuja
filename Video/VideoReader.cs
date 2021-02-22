using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Thuja.Video
{
    public readonly struct VideoInfo
    {
        public readonly int Width;
        public readonly int Height;
        public readonly int Fps;

        public VideoInfo(int width, int height, int fps)
        {
            Width = width;
            Height = height;
            Fps = fps;
        }

        public int TotalLength => Width * Height;
    }

    public class VideoReader : IEnumerator<ColoredChar[,]>
    {
        private readonly byte[] buffer;
        public readonly VideoInfo Info;
        private readonly Stream stream;

        private VideoReader(VideoInfo info, Stream stream)
        {
            Info = info;
            this.stream = stream;
            Current = new ColoredChar[Info.Width, Info.Height];
            buffer = new byte[info.TotalLength * 5];
        }

        public ColoredChar[,] Current { get; }

        public void Reset()
        {
            stream.Seek(6, SeekOrigin.Begin);
        }

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

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            stream.Dispose();
        }

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