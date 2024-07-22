// Decompiled with JetBrains decompiler
// Type: SPU.ADUser
// Assembly: SPU, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BE280C72-BC6C-40D8-B4E7-47FDE97A70CF
// Assembly location: C:\Users\infinity-atom\Documents\SPU.exe

using ActiveDirectory;
using Common;
using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

#nullable disable
namespace SPU
{
  public class ADUser
  {
    private string _fullname;

    public ADUser(UserPrincipal userPrincipal)
    {
      DirectoryEntry underlyingObject = (DirectoryEntry) userPrincipal.GetUnderlyingObject();
      this.Username = userPrincipal.SamAccountName;
      this.Domain = underlyingObject.GetDomainFQDN();
      this.Enabled = !Convert.ToBoolean((int) underlyingObject.Properties["userAccountControl"].Value & 2);
      if (this.Enabled && userPrincipal.AccountExpirationDate.HasValue)
        this.Enabled = userPrincipal.AccountExpirationDate.Value > DateTime.Now;
      this.PasswordNeverExpires = userPrincipal.PasswordNeverExpires;
      if (userPrincipal.LastPasswordSet.HasValue)
        this.PasswordLastSet = new DateTime?(userPrincipal.LastPasswordSet.Value.ToLocalTime());
      else
        this.PasswordRequiresChange = true;
      this.EQID = underlyingObject.GetEQID();
      this.IsStudent = underlyingObject.IsStudent();
      this.IsProvisioned = underlyingObject.IsProvisioned();
      DateTime? nullable;
      if (!this.PasswordNeverExpires)
      {
        if (this.PasswordLastSet.HasValue)
          this.PasswordExpires = new DateTime?((DateTime) underlyingObject.InvokeGet("PasswordExpirationDate"));
        if (!this.PasswordRequiresChange)
        {
          DateTime now = DateTime.Now;
          nullable = this.PasswordExpires;
          this.PasswordRequiresChange = nullable.HasValue && now > nullable.GetValueOrDefault();
        }
      }
      nullable = userPrincipal.LastLogon;
      if (nullable.HasValue)
      {
        nullable = userPrincipal.LastLogon;
        this.LastLogon = new DateTime?(nullable.Value.ToLocalTime());
      }
      this.LockedOut = userPrincipal.IsAccountLockedOut();
      this.FullName = string.Join(" ", new string[2]
      {
        userPrincipal.GivenName,
        userPrincipal.Surname
      });
      underlyingObject.RefreshCache(new string[1]
      {
        "allowedAttributesEffective"
      });
      if (underlyingObject.Properties["allowedAttributesEffective"].Value != null)
      {
        this.SetPasswordPermission = underlyingObject.Properties["allowedAttributesEffective"].Contains((object) "pwdLastSet");
        this.UnlockPermission = underlyingObject.Properties["allowedAttributesEffective"].Contains((object) "lockoutTime");
      }
      this.Path = underlyingObject.Path;
    }

    public bool Enabled { get; }

    public bool LockedOut { get; }

    public string Username { get; }

    public string Domain { get; }

    public string Path { get; }

    public string FullName
    {
      get => !string.IsNullOrEmpty(this._fullname) ? this._fullname : this.Username;
      set => this._fullname = value;
    }

    public string EQID { get; }

    public bool IsStudent { get; }

    public bool IsProvisioned { get; }

    public DateTime? LastLogon { get; }

    public DateTime? PasswordLastSet { get; }

    public DateTime? PasswordExpires { get; }

    public bool PasswordNeverExpires { get; }

    public bool PasswordRequiresChange { get; }

    public bool UnlockPermission { get; }

    public bool SetPasswordPermission { get; }

    public static ADUser FindUser(string domain, string username)
    {
      if (string.IsNullOrEmpty(username))
        return (ADUser) null;
      UserPrincipal user = AD.FindUser(domain, username);
      return user == null ? (ADUser) null : new ADUser(user);
    }

    public static ADUser FindStudent(string domain, string username)
    {
      string str = string.Format("{0}\\{1}", (object) AD.GetNETBIOS(domain), (object) username.ToLower());
      ADUser user = ADUser.FindUser(domain, username);
      if (user == null)
        throw new SPUException(string.Format("User '{0}' was not found", (object) str));
      if (!user.IsProvisioned)
        throw new SPUException(string.Format("User '{0}' is not provisioned", (object) str));
      if (!user.IsStudent)
        throw new SPUException(string.Format("User '{0}' is not a student user account", (object) str));
      if (!user.SetPasswordPermission && !user.UnlockPermission)
        throw new SPUException(string.Format("You do not have permission to manage user '{0}'. If you believe you should have permission please contact the Service Centre", (object) str));
      return user;
    }

    public static ADUser UnlockUser(string domain, string username)
    {
      try
      {
        return ADUser.SetAccount(domain, username, (string) null, new bool?(), true);
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("Failed to unlock user\r\n\r\n{0}", (object) ex.Message));
      }
    }

    public static ADUser SetPassword(
      string domain,
      string username,
      string password,
      bool changeAtNextLogon)
    {
      return ADUser.SetAccount(domain, username, password, new bool?(changeAtNextLogon), false);
    }

    public static ADUser SetPasswordAndUnlock(
      string domain,
      string username,
      string password,
      bool changeAtNextLogon)
    {
      return ADUser.SetAccount(domain, username, password, new bool?(changeAtNextLogon), true);
    }

    private static ADUser SetAccount(
      string domain,
      string username,
      string password,
      bool? changeAtNextLogon,
      bool unlock)
    {
      string str = string.Format("{0}\\\\{1}", (object) AD.GetNETBIOS(domain), (object) username);
      UserPrincipal user = AD.FindUser(domain, username);
      if (user == null)
        throw new ApplicationException(string.Format("User '{0}' does not exist", (object) str));
      if (password != null)
        user.SetPassword(password);
      DirectoryEntry underlyingObject = (DirectoryEntry) user.GetUnderlyingObject();
      bool flag = false;
      if (changeAtNextLogon.HasValue)
      {
        underlyingObject.Properties["pwdLastSet"].Value = (object) (changeAtNextLogon.Value ? 0 : -1);
        flag = true;
      }
      if (unlock)
      {
        underlyingObject.Properties["lockoutTime"].Value = (object) 0;
        flag = true;
      }
      if (flag)
        underlyingObject.CommitChanges();
      return ADUser.FindStudent(domain, username);
    }
  }
}
