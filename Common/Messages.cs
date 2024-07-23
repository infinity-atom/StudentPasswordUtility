// Decompiled with JetBrains decompiler
// Type: Common.Messages
// Assembly: SPU, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BE280C72-BC6C-40D8-B4E7-47FDE97A70CF
// Assembly location: C:\Users\infinity-atom\Documents\SPU.exe

using System.Windows.Forms;

#nullable disable
namespace Common
{
    public static class Messages
    {
        private const string Title = "Student Password Utility";

        public static void ShowInfo(string infoMessage)
        {
            Messages.ShowInfo(infoMessage, "Student Password Utility");
        }

        public static void ShowInfo(string infoMessage, string title)
        {
            Messages.ShowMessage(infoMessage, title, MessageBoxIcon.Asterisk);
        }

        public static void ShowWarning(string warningMessage)
        {
            Messages.ShowWarning(warningMessage, "Student Password Utility");
        }

        public static void ShowWarning(string warningMessage, string title)
        {
            Messages.ShowMessage(warningMessage, title, MessageBoxIcon.Exclamation);
        }

        public static void ShowError(string errorMessage)
        {
            Messages.ShowError(errorMessage, "Student Password Utility");
        }

        public static void ShowError(string errorMessage, string title)
        {
            Messages.ShowMessage(errorMessage, title, MessageBoxIcon.Hand);
        }

        private static void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            if (string.IsNullOrEmpty(message))
                return;
            int num = (int)MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }
    }
}
