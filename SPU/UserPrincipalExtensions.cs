// Decompiled with JetBrains decompiler
// Type: SPU.UserPrincipalExtensions
// Assembly: SPU, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BE280C72-BC6C-40D8-B4E7-47FDE97A70CF
// Assembly location: C:\Users\infinity-atom\Documents\SPU.exe

using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

#nullable disable
namespace SPU
{
  public static class UserPrincipalExtensions
  {
    public static bool IsStudent(this UserPrincipal userPrincipal)
    {
      return ((DirectoryEntry) userPrincipal.GetUnderlyingObject()).IsStudent();
    }
  }
}
