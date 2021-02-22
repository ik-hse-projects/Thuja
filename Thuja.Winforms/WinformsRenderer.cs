using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Thuja.Winforms
{
    public class WinformsRenderer : IRenderer
    {
        private MainForm form;
        private bool needRedraw;

        public void Start()
        {
            form = new MainForm();
            new Thread(() => Application.Run(form)).Start();
        }

        public void BeginShow()
        {
            needRedraw = false;
        }

        public void ShowString(Style style, string str)
        {
            form.actions.Add((Cursor, style, str));
            needRedraw = true;
        }

        public void EndShow()
        {
            if (needRedraw)
            {
                form.Invoke((MethodInvoker) delegate
                {
                    foreach (var (cooridantes, _, str) in form.actions)
                    {
                        form.Invalidate(MainForm.GetRectangleForText(cooridantes, str));
                    }

                    form.Update();
                });
            }
        }

        public void Reset()
        {
            form.clear = true;
            needRedraw = true;
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