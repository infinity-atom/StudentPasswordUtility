// Decompiled with JetBrains decompiler
// Type: SPU.CredentialsForm
// Assembly: SPU, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BE280C72-BC6C-40D8-B4E7-47FDE97A70CF
// Assembly location: C:\Users\infinity-atom\Documents\SPU.exe

using ActiveDirectory;
using Common;
using SPU.Properties;
using System;
using System.ComponentModel;
using System.DirectoryServices;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#nullable disable
namespace SPU
{
  public class CredentialsForm : Form
  {
    private IContainer components;
    private Button btnOK;
    private Button btnCancel;
    private TextBox txtUsername;
    private TextBox txtPassword;
    private Label lblUsername;
    private Label label1;
    private Label lblCredentialsMessage;

    public CredentialsForm()
    {
      this.InitializeComponent();
      this.Icon = Resources.SPU;
    }

    public string Domain
    {
      get
      {
        if (!this.IsValid())
          return string.Empty;
        return this.txtUsername.Text.Split('\\')[0].ToUpper();
      }
    }

    public string Password => !this.IsValid() ? string.Empty : this.txtPassword.Text;

    public string Username
    {
      get
      {
        if (!this.IsValid())
          return string.Empty;
        return this.txtUsername.Text.Split('\\')[1];
      }
    }

    private bool IsValid()
    {
      return Regex.IsMatch(this.txtUsername.Text, ".*\\\\.*") && !string.IsNullOrEmpty(this.txtPassword.Text);
    }

    private void btnCancel_Click(object sender, EventArgs e) => this.Close();

    private void TextBox_TextChanged(object sender, EventArgs e)
    {
      this.btnOK.Enabled = this.IsValid();
    }

    private void ValidateCredentials()
    {
      if (!this.IsValid())
        throw new SPUException("Credential input is incomplete");
      using (DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://" + AD.GetFQDN(this.Domain), this.Username, this.Password))
      {
        try
        {
          directoryEntry.RefreshCache();
        }
        catch (DirectoryServicesCOMException ex)
        {
          Match match = Regex.Match(ex.ExtendedErrorMessage, "data (532|533|701|773|775),");
          if (!match.Success)
          {
            throw;
          }
          else
          {
            switch (match.Value.Substring(5, 3))
            {
              case "532":
                throw new SPUException("User password is expired");
              case "533":
                throw new SPUException("User account is disabled");
              case "701":
                throw new SPUException("User account is expired");
              case "773":
                throw new SPUException("User account requires password reset");
              case "775":
                throw new SPUException("User account is locked out");
              default:
                throw;
            }
          }
        }
        catch (COMException ex)
        {
          if (ex.ErrorCode == -2147016646)
            throw new SPUException(string.Format("Cannot contact {0} domain. Check network connectivity.", (object) this.Domain));
          throw;
        }
      }
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      bool flag1 = false;
      bool flag2 = false;
      SplashScreen.ShowSplashScreen((Form) this, "Validating credentials...", false);
      string errorMessage = (string) null;
      try
      {
        AD.IsValidDomain(this.Domain, true);
      }
      catch (Exception ex)
      {
        flag1 = true;
        errorMessage = ex.Message;
      }
      if (!flag1)
      {
        try
        {
          new Action(this.ValidateCredentials).InvokeInThread();
        }
        catch (Exception ex)
        {
          flag2 = true;
          errorMessage = ex.Message;
        }
      }
      if (!string.IsNullOrEmpty(errorMessage))
        Messages.ShowError(errorMessage);
      SplashScreen.CloseSplashScreen();
      if (flag2 | flag1)
      {
        this.txtPassword.Clear();
        if (flag1)
        {
          this.txtUsername.Select();
          this.txtUsername.Clear();
        }
        else
          this.txtPassword.Select();
      }
      else
      {
        this.DialogResult = DialogResult.OK;
        this.Close();
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.btnOK = new Button();
      this.btnCancel = new Button();
      this.txtUsername = new TextBox();
      this.txtPassword = new TextBox();
      this.lblUsername = new Label();
      this.label1 = new Label();
      this.lblCredentialsMessage = new Label();
      this.SuspendLayout();
      this.btnOK.Enabled = false;
      this.btnOK.Location = new Point(53, 125);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new Size(75, 23);
      this.btnOK.TabIndex = 2;
      this.btnOK.Text = "OK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new EventHandler(this.btnOK_Click);
      this.btnCancel.DialogResult = DialogResult.Cancel;
      this.btnCancel.Location = new Point(161, 125);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new Size(75, 23);
      this.btnCancel.TabIndex = 3;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
      this.txtUsername.Location = new Point(143, 49);
      this.txtUsername.Name = "txtUsername";
      this.txtUsername.Size = new Size(114, 20);
      this.txtUsername.TabIndex = 0;
      this.txtUsername.TextChanged += new EventHandler(this.TextBox_TextChanged);
      this.txtPassword.Location = new Point(143, 88);
      this.txtPassword.Name = "txtPassword";
      this.txtPassword.Size = new Size(114, 20);
      this.txtPassword.TabIndex = 1;
      this.txtPassword.UseSystemPasswordChar = true;
      this.txtPassword.TextChanged += new EventHandler(this.TextBox_TextChanged);
      this.lblUsername.AutoSize = true;
      this.lblUsername.Location = new Point(24, 52);
      this.lblUsername.Name = "lblUsername";
      this.lblUsername.Size = new Size(94, 26);
      this.lblUsername.TabIndex = 4;
      this.lblUsername.Text = "Username:\r\n(e.g. GBN\\auser1)";
      this.label1.AutoSize = true;
      this.label1.Location = new Point(24, 91);
      this.label1.Name = "label1";
      this.label1.Size = new Size(56, 13);
      this.label1.TabIndex = 5;
      this.label1.Text = "Password:";
      this.lblCredentialsMessage.AutoSize = true;
      this.lblCredentialsMessage.Location = new Point(24, 20);
      this.lblCredentialsMessage.Name = "lblCredentialsMessage";
      this.lblCredentialsMessage.Size = new Size(210, 13);
      this.lblCredentialsMessage.TabIndex = 6;
      this.lblCredentialsMessage.Text = "Please enter your departmental credentials:";
      this.AcceptButton = (IButtonControl) this.btnOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btnCancel;
      this.ClientSize = new Size(293, 166);
      this.Controls.Add((Control) this.lblCredentialsMessage);
      this.Controls.Add((Control) this.label1);
      this.Controls.Add((Control) this.lblUsername);
      this.Controls.Add((Control) this.txtPassword);
      this.Controls.Add((Control) this.txtUsername);
      this.Controls.Add((Control) this.btnCancel);
      this.Controls.Add((Control) this.btnOK);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "CredentialsForm";
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "Student Password Utility";
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
