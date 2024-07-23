// Decompiled with JetBrains decompiler
// Type: Common.UserCredentials
// Assembly: SPU, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BE280C72-BC6C-40D8-B4E7-47FDE97A70CF
// Assembly location: C:\Users\infinity-atom\Documents\SPU.exe

using ActiveDirectory;
using System;

#nullable disable
namespace Common
{
    public class UserCredentials
    {
        public UserCredentials(string domain, string username, string password)
        {
            if (string.IsNullOrEmpty(domain))
                throw new ArgumentNullException(domain);
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(username);
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(password);
            this.Domain = AD.GetNETBIOS(domain);
            this.Username = username.ToLower();
            this.Password = password;
        }

        public string DomainAndUser
        {
            get => string.Format("{0}\\{1}", (object)this.Domain, (object)this.Username);
        }

        public string Domain { get; }

        public string Username { get; }

        public string Password { get; }
    }
}
