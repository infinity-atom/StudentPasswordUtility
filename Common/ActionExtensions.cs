// Decompiled with JetBrains decompiler
// Type: Common.ActionExtensions
// Assembly: SPU, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BE280C72-BC6C-40D8-B4E7-47FDE97A70CF
// Assembly location: C:\Users\infinity-atom\Documents\SPU.exe

using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace Common
{
    public static class ActionExtensions
    {
        public static void InvokeInThread(this Action action)
        {
            Exception exception = (Exception)null;
            Thread thread = new Thread((ThreadStart)(() => exception = ActionExtensions.InvokeAction(action)))
            {
                IsBackground = true
            };
            thread.Start();
            while (thread.IsAlive)
            {
                Thread.Sleep(100);
                Application.DoEvents();
            }
            if (exception != null)
                throw exception;
        }

        private static Exception InvokeAction(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                return ex;
            }
            return (Exception)null;
        }

        public static void InvokeEx<T>(this T @this, Action<T> action) where T : ISynchronizeInvoke
        {
            if (@this.InvokeRequired)
                @this.Invoke((Delegate)action, new object[1]
                {
          (object) @this
                });
            else
                action(@this);
        }
    }
}
