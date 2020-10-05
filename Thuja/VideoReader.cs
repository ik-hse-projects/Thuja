using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Thuja
{
    public struct VideoInfo
    {
        public readonly int Width;
        public readonly int Height;
        public readonly int Fps1;
        public readonly int Fps2;

        public VideoInfo(int width, int height, int fps1, int fps2 = 1)
        {
            Width = width;
            Height = height;
            Fps1 = fps1;
            Fps2 = fps2;
        }

        public int TotalLength => Width * Height;

        public int CountFrames(int totalBytes)
        {
            return (totalBytes - 12) / (TotalLength * 8);
        }
    }

    public class VideoReader : IEnumerator<ColoredChar?[,]>
    {
        public readonly VideoInfo Info;
        private Stream _stream;
        private byte[] _buffer;
        public ColoredChar?[,] Current { get; }

        private VideoReader(VideoInfo info, Stream stream)
        {
            Info = info;
            _stream = stream;
            Current = new ColoredChar?[Info.Width, Info.Height];
            _buffer = new byte[info.TotalLength * 5];
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
            var fps2 = buffer[4];
            var fps1 = buffer[5];

            if (fps1 == 0 && fps2 == 0)
            {
                fps1 = 1;
                fps2 = 24;
            }

            return new VideoReader(new VideoInfo(width, height, fps1, fps2), stream);
        }
        
        public void Reset()
        {
            _stream.Seek(6, SeekOrigin.Begin);
        }

        public bool MoveNext()
        {
            var bytesRead = _stream.Read(_buffer);
            if (bytesRead == 0)
            {
                return false;
            }
            if (bytesRead != Info.TotalLength * 5)
            {
                throw new Exception("Can't read full frame");
            }

            var offset = 0;
            for (int y = 0; y < Info.Height; y++)
            {
                for (int x = 0; x < Info.Width; x++)
                {
                    var style = BitConverter.ToUInt16(_buffer, offset);
                    var foreground = MyColorExt.FromInt((style & 0b11111_00000_000000) >> 11);
                    var background = MyColorExt.FromInt((style & 0b00000_11111_000000) >> 6);
                    var flags = (byte)(style & 0b00000_00000_111111);

                    if (!Enum.IsDefined(typeof(MyColor), foreground))
                    {
                        foreground = default;
                    }
                    if (!Enum.IsDefined(typeof(MyColor), background))
                    {
                        background = default;
                    }

                    var charNumber =
                        _buffer[offset + 2]
                        | (_buffer[offset + 3] << 8)
                        | (_buffer[offset + 4] << 16);
                    var character = (char) charNumber;
                    offset += 5;

                    Current[x, y] = new ColoredChar(new Style(foreground, background), character);
                }
            }

            return true;
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}