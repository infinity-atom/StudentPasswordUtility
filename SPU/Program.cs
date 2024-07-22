// Decompiled with JetBrains decompiler
// Type: SPU.Program
// Assembly: SPU, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BE280C72-BC6C-40D8-B4E7-47FDE97A70CF
// Assembly location: C:\Users\infinity-atom\Documents\SPU.exe

using ActiveDirectory;
using Common;
using System;
using System.DirectoryServices.AccountManagement;
using System.Management;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace SPU
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      Application.ThreadException += new ThreadExceptionEventHandler(Program.ThreadException_EventHandler);
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.CurrentDomain_UnhandledException);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      if ((Control.ModifierKeys & Keys.Control) != Keys.None || !Program.IsDomainJoined() || !AD.IsValidDomain(Environment.UserDomainName))
      {
        using (CredentialsForm credentialsForm = new CredentialsForm())
        {
          if (credentialsForm.ShowDialog() != DialogResult.OK)
            Program.Quit(1);
          Program.UserCredentials = new UserCredentials(credentialsForm.Domain, credentialsForm.Username, credentialsForm.Password);
        }
      }
      if (!Program.ValidUser())
        Program.Quit("You are not permitted to execute this application");
      Application.Run((Form) new MainForm());
    }

    public static UserCredentials UserCredentials { get; set; }

    private static bool ValidUser()
    {
      if (Program.UserCredentials == null)
        return !UserPrincipal.Current.IsStudent();
      UserPrincipal user = AD.FindUser(Program.UserCredentials.Domain, Program.UserCredentials.Username);
      return user != null && !user.IsStudent();
    }

    private static void CurrentDomain_UnhandledException(
      object sender,
      UnhandledExceptionEventArgs e)
    {
      Program.UnhandledExceptionHandler(e.ExceptionObject);
    }

    private static void ThreadException_EventHandler(object sender, ThreadExceptionEventArgs t)
    {
      Program.UnhandledExceptionHandler((object) t.Exception);
    }

    private static void UnhandledExceptionHandler(object exception)
    {
      Program.Quit(exception != null ? (exception.GetType() != typeof (SPUException) ? string.Format("A critical error has occurred!\r\n\r\n{0}", (object) (Exception) exception) : ((Exception) exception).Message) : "An unknown error has occurred");
    }

    public static void Quit() => Program.Quit(0, (Exception) null, (string) null);

    public static void Quit(int exitCode)
    {
      Program.Quit(exitCode, (Exception) null, (string) null);
    }

    public static void Quit(Exception exception) => Program.Quit(1, exception, (string) null);

    public static void Quit(string errorMessage) => Program.Quit(1, (Exception) null, errorMessage);

    public static void Quit(int exitCode, Exception exception, string errorMessage)
    {
      if (!string.IsNullOrEmpty(exception?.Message))
        Messages.ShowError(exception.Message);
      else if (!string.IsNullOrEmpty(errorMessage))
        Messages.ShowError(errorMessage);
      Environment.Exit(exitCode);
    }

    private static bool IsDomainJoined()
    {
      try
      {
        using (ManagementObject managementObject = new ManagementObject(string.Format("Win32_ComputerSystem.Name='{0}'", (object) Environment.MachineName)))
        {
          managementObject.Get();
          object obj = managementObject["PartOfDomain"];
          return obj != null && (bool) obj;
        }
      }
      catch (SystemException ex)
      {
        return false;
      }
    }
  }
}
