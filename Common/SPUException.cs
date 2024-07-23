// Decompiled with JetBrains decompiler
// Type: Common.SPUException
// Assembly: SPU, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BE280C72-BC6C-40D8-B4E7-47FDE97A70CF
// Assembly location: C:\Users\infinity-atom\Documents\SPU.exe

using System;

#nullable disable
namespace Common
{
    public class SPUException : Exception
    {
        public SPUException(string message)
          : base(message)
        {
        }

        public SPUException(string message, Exception innerException)
          : base(message, innerException)
        {
        }
    }
}
