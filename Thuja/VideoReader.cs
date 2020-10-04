using System;
using System.Collections;
using System.Collections.Generic;
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

    public class VideoReader : IEnumerator<VideoFrame>
    {
        public readonly VideoInfo Info;
        private Stream _stream;
        private byte[] _buffer;

        private VideoReader(VideoInfo info, Stream stream)
        {
            Info = info;
            _stream = stream;
            Current = new VideoFrame
            {
                Content = new ColoredChar?[Info.Width, Info.Height]
            };
            _buffer = new byte[info.TotalLength * 8];
        }

        public static VideoReader Init(Stream stream)
        {
            var buffer = new byte[12];
            var bytesRead = stream.Read(buffer, 0, 12);
            if (bytesRead != 12)
            {
                throw new Exception("Can't read header");
            }

            var width = BitConverter.ToInt32(buffer, 0);
            var height = BitConverter.ToInt32(buffer, 4);
            var fps1 = BitConverter.ToUInt16(buffer, 8);
            var fps2 = BitConverter.ToUInt16(buffer, 10);

            return new VideoReader(new VideoInfo(width, height, fps1, fps2), stream);
        }

        public bool MoveNext()
        {
            var bytesRead = _stream.Read(_buffer);
            if (bytesRead == 0)
            {
                return false;
            }
            if (bytesRead != Info.TotalLength * 8)
            {
                throw new Exception("Can't read full frame");
            }

            var frame = Current.Content;
            var offset = 0;
            for (int y = 0; y < Info.Height; y++)
            {
                for (int x = 0; x < Info.Width; x++)
                {
                    var background = _buffer[offset];
                    var foreground = _buffer[offset + 1];
                    var flags = BitConverter.ToUInt16(_buffer, offset + 2);
                    var chars = Encoding.UTF32.GetChars(_buffer, offset + 4, 4);
                    offset += 8;

                    var character = chars.Length == 1 ? chars[0] : '�';

                    frame[x, y] = new ColoredChar((MyColor) foreground, (MyColor) background, flags, character);
                }
            }

            return true;
        }

        public void Reset()
        {
            _stream.Seek(12, SeekOrigin.Begin);
        }

        public VideoFrame Current { get; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}