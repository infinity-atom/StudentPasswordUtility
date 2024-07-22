// Decompiled with JetBrains decompiler
// Type: SPU.DirectoryEntryExtensions
// Assembly: SPU, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BE280C72-BC6C-40D8-B4E7-47FDE97A70CF
// Assembly location: C:\Users\infinity-atom\Documents\SPU.exe

using System;
using System.DirectoryServices;
using System.Text.RegularExpressions;

#nullable disable
namespace SPU
{
  public static class DirectoryEntryExtensions
  {
    public static string GetDomainFQDN(this DirectoryEntry directoryEntry)
    {
      int startIndex = directoryEntry.Path.IndexOf("DC=", StringComparison.OrdinalIgnoreCase);
      return startIndex < 0 ? string.Empty : directoryEntry.Path.Substring(startIndex).Replace("DC=", "").Replace(",", ".").ToLower();
    }

    public static string GetEQID(this DirectoryEntry directoryEntry)
    {
      return (string) directoryEntry.Properties["EQEQID"].Value ?? string.Empty;
    }

    public static bool IsStudent(this DirectoryEntry directoryEntry)
    {
      string str = (string) directoryEntry.Properties["EQIdentityType"].Value;
      return str != null && str.Equals("Student", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsProvisioned(this DirectoryEntry directoryEntry)
    {
      return !Regex.IsMatch(directoryEntry.Path, "OU=...._Lost and Found,", RegexOptions.IgnoreCase);
    }
  }
}
