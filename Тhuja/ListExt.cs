using System.Collections.Generic;

namespace Тhuja
{
    public static class ListExt
    {
        public static void RemoveLast<T>(this List<T> self, int count)
        {
            self.RemoveRange(self.Count - count, count);
        }
    }
}