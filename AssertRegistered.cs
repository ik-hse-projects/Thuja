using System;
using System.Diagnostics;

namespace Thuja
{
    /// <summary>
    ///     Слишком легко забыть зарегистрировать виджет, поэтому рекомендуется
    ///     по возможности наследоваться от этого класса вместо IWdiget.
    ///     Он проверяет, чтобы IWidget.Render не вызывался  без регистрации.
    ///
    ///     Этот интерфейс полностью совместим с IWidget, но требует AssertData.
    /// </summary>
    public abstract class AssertRegistered : IWidget
    {
        private class RegistrationStatus
        {
            public bool isRegistered;
            public bool isUnregistered;
            public bool isIgnored;
        }

        /// <summary>
        ///     Используется исключительно для нужд AssertRegistered.
        /// </summary>
        private RegistrationStatus AssertData { get; } = new();

        /// <summary>
        ///     Этот метод использовать очень-очень нежелательно.
        /// </summary>
        internal void IgnoreAssertRegistered()
        {
            AssertData.isIgnored = true;
        }

        public abstract void Render(RenderContext context);

        void IWidget.Render(RenderContext context)
        {
            if (AssertData.isIgnored)
            {
                Render(context);
                return;
            }

            if (!AssertData.isRegistered)
            {
                // Отрисовка без регистрации.
                Debugger.Break();
            }

            if (AssertData.isUnregistered)
            {
                // Отрисовка после разрегистрации.
                Debugger.Break();
            }

            Render(context);
        }

        public virtual void OnRegistered(MainLoop loop)
        {
        }

        void IWidget.OnRegistered(MainLoop loop)
        {
            if (AssertData.isIgnored)
            {
                OnRegistered(loop);
                return;
            }

            if (AssertData.isUnregistered)
            {
                // Регистрация после разрегистрации.
                Debugger.Break();
            }

            if (AssertData.isRegistered)
            {
                // Регистрация дважды.
                Debugger.Break();
            }

            AssertData.isRegistered = true;

            OnRegistered(loop);
        }

        public virtual void OnUnregistered()
        {
        }

        void IWidget.OnUnregistered()
        {
            if (AssertData.isIgnored)
            {
                OnUnregistered();
                return;
            }

            if (!AssertData.isRegistered)
            {
                // Разрегистрация до регистрации.
                Debugger.Break();
            }

            if (AssertData.isUnregistered)
            {
                // Разрегистрация дважды.
                Debugger.Break();
            }

            AssertData.isUnregistered = true;
            OnUnregistered();
        }
    }
}