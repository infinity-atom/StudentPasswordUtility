// Decompiled with JetBrains decompiler
// Type: ActiveDirectory.AD
// Assembly: SPU, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BE280C72-BC6C-40D8-B4E7-47FDE97A70CF
// Assembly location: C:\Users\infinity-atom\Documents\SPU.exe

using Common;
using SPU;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#nullable disable
namespace ActiveDirectory
{
  public static class AD
  {
    private static readonly Dictionary<string, PrincipalContext> DomainContexts = new Dictionary<string, PrincipalContext>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    private static PrincipalContext GetDomainContext(string domain)
    {
      domain = AD.GetFQDN(domain);
      string netbios = AD.GetNETBIOS(domain);
      if (AD.DomainContexts.ContainsKey(netbios))
        return AD.DomainContexts[netbios];
      try
      {
        using (Domain computerDomain = Domain.GetComputerDomain())
        {
          if (Regex.IsMatch(computerDomain.Name, domain, RegexOptions.IgnoreCase))
          {
            try
            {
              ActiveDirectorySite computerSite = ActiveDirectorySite.GetComputerSite();
              DomainControllerCollection domainControllers = computerDomain.FindAllDomainControllers(computerSite.Name);
              if (domainControllers.Count > 0)
              {
                bool flag = false;
                foreach (DomainController domainController in (ReadOnlyCollectionBase) domainControllers)
                {
                  if (Regex.IsMatch(domainController.Name, "^EQ.......001(\\.|$)", RegexOptions.IgnoreCase))
                  {
                    flag = true;
                    try
                    {
                      PrincipalContext principalContext = AD.GetDomainPrincipalContext(domain, domainController.Name);
                      AD.DomainContexts.Add(netbios, principalContext);
                      return principalContext;
                    }
                    catch (Exception ex)
                    {
                      Messages.ShowWarning(string.Format("Failed to connect to preferred domain controller {0}\r\n\r\n{1}", (object) domainController.Name, (object) ex.Message));
                    }
                  }
                }
                foreach (DomainController domainController in (ReadOnlyCollectionBase) domainControllers)
                {
                  if (!Regex.IsMatch(domainController.Name, "^EQ.......001(\\.|$)", RegexOptions.IgnoreCase))
                  {
                    try
                    {
                      PrincipalContext principalContext = AD.GetDomainPrincipalContext(domain, domainController.Name);
                      AD.DomainContexts.Add(netbios, principalContext);
                      return principalContext;
                    }
                    catch
                    {
                    }
                  }
                }
                if (domainControllers.Count != 1 || flag)
                {
                  if (domainControllers.Count <= 1)
                    goto label_32;
                }
                Messages.ShowWarning("Failed to connect to a site domain controller");
              }
            }
            catch (Exception ex)
            {
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
label_32:
      try
      {
        PrincipalContext principalContext = AD.GetDomainPrincipalContext(domain);
        AD.DomainContexts.Add(netbios, principalContext);
        return principalContext;
      }
      catch (Exception ex)
      {
        throw new SPUException(string.Format("Failed to connect to domain controller for domain {0}\r\n\r\n{1}", (object) domain, (object) ex.Message));
      }
    }

    private static PrincipalContext GetDomainPrincipalContext(string domain)
    {
      return AD.GetDomainPrincipalContext(domain, string.Empty);
    }

    private static PrincipalContext GetDomainPrincipalContext(
      string domain,
      string domainController)
    {
      string name = AD.GetFQDN(domain);
      if (!string.IsNullOrEmpty(domainController))
        name = string.Join(".", new string[2]
        {
          domainController.Split('.')[0],
          name
        });
      return Program.UserCredentials == null ? new PrincipalContext(ContextType.Domain, name) : new PrincipalContext(ContextType.Domain, name, Program.UserCredentials.DomainAndUser, Program.UserCredentials.Password);
    }

    public static UserPrincipal FindUser(string domain, string username)
    {
      PrincipalContext domainContext = AD.GetDomainContext(domain);
      if ((Control.ModifierKeys & Keys.Control) != Keys.None)
        Messages.ShowInfo(string.Format("Domain Controller: {0}", (object) domainContext.ConnectedServer));
      return UserPrincipal.FindByIdentity(domainContext, username);
    }

    public static bool IsValidDomain(string domain) => AD.IsValidDomain(domain, false);

    public static bool IsValidDomain(string domain, bool throwException)
    {
      bool flag = Regex.IsMatch(domain, "^(DDS|FCW|FNQ|GBN|MTN|MYW|NOQ|SOC|SUN|WBB)($|\\.EQ\\.EDU\\.AU$)", RegexOptions.IgnoreCase);
      return !(!flag & throwException) ? flag : throw new SPUException(string.Format("{0} is not a supported domain", (object) domain.Split('.')[0].ToUpper()));
    }

    public static string GetFQDN(string domain)
    {
      return string.Format("{0}.eq.edu.au", (object) AD.GetNETBIOS(domain).ToLower());
    }

    public static string GetNETBIOS(string domain)
    {
      if (!AD.IsValidDomain(domain, true))
        return string.Empty;
      return domain.Split('.')[0].ToUpper();
    }
  }
}
