using System.Diagnostics;

namespace Thuja
{
    public class LimitFps
    {
        private long lastFrame;
        private long delay;

        public int Fps
        {
            set => delay = Stopwatch.Frequency / value;
        }

        public bool IsTimeToDraw()
        {
            var now = Stopwatch.GetTimestamp();
            if (now - lastFrame > delay)
            {
                lastFrame = now;
                return true;
            }

            return false;
        }
    }
}