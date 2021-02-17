using System;
using System.Collections.Generic;

namespace Thuja
{
    public class WinformsRenderer: IRenderer
    {
        public WinformsRenderer()
        {
            
        }
        
        public void BeginShow()
        {
            throw new NotImplementedException();
        }

        public void ShowString(Style style, string str)
        {
            throw new NotImplementedException();
        }

        public void EndShow()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ConsoleKeyInfo> GetKeys()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Cooridantes> GetClicks()
        {
            throw new NotImplementedException();
        }

        public Cooridantes Size { get; }
        public Cooridantes Cursor { get; set; }
    }
}