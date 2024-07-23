// Decompiled with JetBrains decompiler
// Type: SPU.Extensions.ObjectExtensions
// Assembly: SPU, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BE280C72-BC6C-40D8-B4E7-47FDE97A70CF
// Assembly location: C:\Users\infinity-atom\Documents\SPU.exe

using System;
using System.ComponentModel;

#nullable disable
namespace SPU.Extensions
{
    public static class ObjectExtensions
    {
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
