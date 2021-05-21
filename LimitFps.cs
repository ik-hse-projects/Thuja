using System.Diagnostics;

namespace Thuja
{
    /// <summary>
    /// Позволяет легко делать что-то не больше N раз в секунду.
    /// </summary>
    public class LimitFps
    {
        /// <summary>
        /// Задержка между "кадрами", измеряется в тиках Stopwatch.
        /// </summary>
        private long delay;
        
        /// <summary>
        /// Когда был последний "кадр", в тиках Stopwatch.
        /// </summary>
        private long lastFrame;

        /// <summary>
        /// Сколько раз в секунду <see cref="IsTimeToDraw"/> будет возвращать true.
        /// </summary>
        public int Fps
        {
            set => delay = Stopwatch.Frequency / value;
        }

        /// <summary>
        /// Вовзвращает true, если с момента последнего true прошло столько времени,
        /// чтобы true возвращались <see cref="Fps"/> раз в секунду.
        ///
        /// Рекомендуется вызывать этот метод так часто, как можно. 
        /// </summary>
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