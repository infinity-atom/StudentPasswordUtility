// Decompiled with JetBrains decompiler
// Type: SPU.MainForm
// Assembly: SPU, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BE280C72-BC6C-40D8-B4E7-47FDE97A70CF
// Assembly location: C:\Users\infinity-atom\Documents\SPU.exe

using Common;
using SPU.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace SPU
{
    public class MainForm : Form
    {
        private ADUser _adUser;
        private Bitmap _defaultImage;
        private IContainer components;
        private TextBox txtAccountName;
        private Label lblUsername;
        private Button btnOK;
        private TextBox txtPwd1;
        private Label label2;
        private TextBox txtPwd2;
        private Label lblPasswordConfirm;
        private Button btnQuit;
        private Button btnFind;
        private Button btnAbout;
        private Label lblName;
        private CheckBox chkDisabled;
        private GroupBox grpProperties;
        private CheckBox chkPasswordExpired;
        private CheckBox chkLockedOut;
        private Label lblLockedOut;
        private Label label3;
        private CheckBox chkChangeNextLogon;
        private GroupBox grpPassword;
        private Label lblPasswordLastSetValue;
        private Label lblPasswordExpiresValue;
        private Label lblPasswordExpired;
        private Label lblPasswordLastset;
        private Label lblPasswordExpires;
        private GroupBox grpManageUser;
        private CheckBox chkSetPassword;
        private CheckBox chkUnlock;
        private Label lblNameValue;
        private ToolTip toolTip;
        private PictureBox userImage;

        public MainForm()
        {
            this.InitializeComponent();
            this.Icon = Resources.SPU;
            this.txtAccountName.GotFocus += new EventHandler(this.OnTextBoxFocus);
            this.txtPwd1.GotFocus += new EventHandler(this.OnTextBoxFocus);
            this.txtPwd2.GotFocus += new EventHandler(this.OnTextBoxFocus);
            this.Images = this.GetImages();
            if (this.ImageMode)
            {
                this.userImage.Image = (Image)this.DefaultImage;
            }
            else
            {
                this.userImage.Visible = false;
                this.Height = 510;
            }
        }

        private ADUser ADUser
        {
            get => this._adUser;
            set
            {
                this._adUser = value;
                if (this.ImageMode)
                    this.userImage.Image = this.GetUserImage(this._adUser);
                this.chkDisabled.InvokeEx<CheckBox>((Action<CheckBox>)(x => x.Checked = this._adUser != null && !this._adUser.Enabled));
                this.chkLockedOut.InvokeEx<CheckBox>((Action<CheckBox>)(x => x.Checked = this._adUser != null && this._adUser.LockedOut));
                this.chkPasswordExpired.InvokeEx<CheckBox>((Action<CheckBox>)(x => x.Checked = this._adUser != null && this._adUser.PasswordRequiresChange));
                if (this._adUser == null)
                    this.lblPasswordExpiresValue.InvokeEx<Label>((Action<Label>)(x => x.Text = string.Empty));
                else if (this._adUser.PasswordNeverExpires)
                    this.lblPasswordExpiresValue.InvokeEx<Label>((Action<Label>)(x => x.Text = "Never"));
                else
                    this.lblPasswordExpiresValue.InvokeEx<Label>((Action<Label>)(x =>
                    {
                        Label label = x;
                        DateTime? passwordExpires = this._adUser.PasswordExpires;
                        ref DateTime? local = ref passwordExpires;
                        string str = (local.HasValue ? local.GetValueOrDefault().ToString() : (string)null) ?? "Pending reset";
                        label.Text = str;
                    }));
                if (this._adUser == null)
                    this.lblPasswordLastSetValue.InvokeEx<Label>((Action<Label>)(x => x.Text = string.Empty));
                else
                    this.lblPasswordLastSetValue.InvokeEx<Label>((Action<Label>)(x =>
                    {
                        Label label = x;
                        DateTime? nullable = this._adUser.PasswordLastSet;
                        ref DateTime? local = ref nullable;
                        string str = local.HasValue ? local.GetValueOrDefault().ToString() : (string)null;
                        if (str == null)
                        {
                            nullable = this._adUser.LastLogon;
                            str = nullable.HasValue ? "Pending reset" : "Never";
                        }
                        label.Text = str;
                    }));
                if (this._adUser == null)
                {
                    this.lblNameValue.InvokeEx<Label>((Action<Label>)(x => x.Text = string.Empty));
                }
                else
                {
                    string fullname = this._adUser.FullName;
                    string usernameSuffix;
                    for (usernameSuffix = string.Format(" ({0})", (object)this._adUser.Username); TextRenderer.MeasureText(fullname + usernameSuffix, this.Font).Width > this.lblNameValue.Width; fullname = fullname.Remove(fullname.LastIndexOf("...", StringComparison.OrdinalIgnoreCase) - 1, 1))
                    {
                        if (!fullname.EndsWith("..."))
                            fullname += "...";
                    }
                    this.lblNameValue.BeginInvoke((Action)(() =>
                    {
                        if (fullname.Equals(this._adUser.FullName))
                            this.toolTip.SetToolTip((Control)this.lblNameValue, string.Empty);
                        else
                            this.toolTip.SetToolTip((Control)this.lblNameValue, this._adUser.FullName + usernameSuffix);
                        this.lblNameValue.InvokeEx<Label>((Action<Label>)(x => x.Text = fullname + usernameSuffix));
                    }));
                }
            }
        }

        private bool ImageMode => this.Images != null && this.Images.Count > 0;

        private Dictionary<string, string> Images { get; }

        private string Domain
        {
            get
            {
                return !this.txtAccountName.Text.Contains("\\") ? (Program.UserCredentials == null ? Environment.UserDomainName : Program.UserCredentials.Domain) : this.txtAccountName.Text.Split('\\')[0];
            }
        }

        private string Username
        {
            get
            {
                if (!this.txtAccountName.Text.Contains("\\"))
                    return this.txtAccountName.Text;
                return this.txtAccountName.Text.Split('\\')[1];
            }
        }

        private string DomainUsername
        {
            get => string.Format("{0}\\{1}", (object)this.Domain, (object)this.Username);
        }

        private string Password
        {
            get
            {
                return this.txtPwd1.Text.Length <= 0 || this.txtPwd2.Text.Length <= 0 ? string.Empty : this.txtPwd1.Text;
            }
        }

        private bool SetPassword
        {
            get
            {
                return this.chkSetPassword.Enabled && this.chkSetPassword.Checked && !string.IsNullOrEmpty(this.Password);
            }
        }

        private bool UnlockAccount => this.chkUnlock.Enabled && this.chkUnlock.Checked;

        private void btnAbout_Click(object sender, EventArgs e)
        {
            int num = (int)MessageBox.Show(string.Format("Version: {0}\r\n\r\nThis utility allows privileged users to reset the passwords or unlock accounts with of \"Student\" identity type. If you do not have permission to manage a user and believe you should please contact the Service Centre.", (object)Application.ProductVersion), "Student Password Utility", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void btnCheckName_Click(object sender, EventArgs e)
        {
            if (this.ADUser != null && !this.ADUser.Username.Equals(this.Username, StringComparison.OrdinalIgnoreCase))
            {
                this.ADUser = (ADUser)null;
                this.SetGUI(sender);
            }
            Exception exception = (Exception)null;
            try
            {
                SplashScreen.ShowSplashScreen((Form)this, "Finding user...", false);
                ((Action)(() => this.ADUser = ADUser.FindStudent(this.Domain, this.Username))).InvokeInThread();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                if (exception != null)
                {
                    Messages.ShowError(exception.GetType() == typeof(SPUException) ? exception.Message : string.Format("An error occurred while querying Active Directory for user '{0}'\r\n\r\n{1}", (object)this.DomainUsername, (object)exception.Message));
                    this.ADUser = (ADUser)null;
                }
                SplashScreen.CloseSplashScreen();
            }
            this.SetGUI(sender);
        }

        private void OnTextBoxFocus(object sender, EventArgs e)
        {
            if (sender == this.txtAccountName)
            {
                this.AcceptButton = (IButtonControl)this.btnFind;
            }
            else
            {
                if (sender != this.txtPwd1 && sender != this.txtPwd2)
                    return;
                this.AcceptButton = (IButtonControl)this.btnOK;
            }
        }

        private void SetGUI(object sender, EventArgs e = null)
        {
            if (this.ADUser == null)
            {
                this.txtAccountName.Focus();
                this.txtAccountName.SelectAll();
            }
            else
            {
                this.chkChangeNextLogon.Enabled = this.ADUser.SetPasswordPermission;
                this.chkSetPassword.Enabled = this.ADUser.SetPasswordPermission;
                this.chkUnlock.Enabled = this.ADUser.UnlockPermission && this.ADUser.LockedOut;
                if (sender == this.btnFind)
                {
                    this.chkSetPassword.Checked = this.chkSetPassword.Enabled;
                    this.chkChangeNextLogon.Checked = this.chkChangeNextLogon.Enabled;
                    this.chkUnlock.Checked = this.chkUnlock.Enabled;
                    if (sender == this.btnFind && this.chkSetPassword.Checked)
                        this.txtPwd1.Select();
                }
                this.chkChangeNextLogon.Enabled = this.chkSetPassword.Checked;
                this.txtPwd1.Enabled = this.chkSetPassword.Checked;
                this.txtPwd2.Enabled = this.chkSetPassword.Checked;
                if (!this.chkChangeNextLogon.Enabled)
                    this.chkChangeNextLogon.Checked = false;
                if (sender == this.chkSetPassword)
                    this.chkChangeNextLogon.Checked = this.chkSetPassword.Checked && this.chkChangeNextLogon.Enabled;
                if (!this.chkSetPassword.Checked)
                {
                    this.txtPwd1.Clear();
                    this.txtPwd2.Clear();
                }
                if (this.SetPassword)
                    this.btnOK.Enabled = true;
                else if (!this.chkSetPassword.Checked && this.UnlockAccount)
                    this.btnOK.Enabled = true;
                else
                    this.btnOK.Enabled = false;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text += Program.UserCredentials != null ? string.Format(" ({0})", (object)Program.UserCredentials.DomainAndUser) : string.Format(" ({0}\\{1})", (object)Environment.UserDomainName, (object)Environment.UserName);
        }

        private void txtAccountName_TextChanged(object sender, EventArgs e)
        {
            this.btnFind.Enabled = !string.IsNullOrEmpty(this.Username);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bool flag = false;
            if (this.UnlockAccount && !this.SetPassword)
            {
                SplashScreen.ShowSplashScreen((Form)this, "Unlocking user...", false);
                Exception exception = (Exception)null;
                try
                {
                    ((Action)(() => this.ADUser = ADUser.UnlockUser(this.Domain, this.Username))).InvokeInThread();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                finally
                {
                    SplashScreen.CloseSplashScreen();
                    if (exception != null)
                        Messages.ShowError(string.Format("Failed to unlock user '{0}'\r\n\r\n{1}", (object)this.DomainUsername, (object)exception.Message));
                    else
                        Messages.ShowInfo(string.Format("Successfully unlocked user '{0}' ({1})", (object)this.DomainUsername, (object)this.ADUser.FullName));
                }
            }
            else if (this.SetPassword)
            {
                if (this.txtPwd1.Text != this.txtPwd2.Text)
                {
                    flag = true;
                    Messages.ShowError("The New and Confirm passwords must match. Please re-type them.");
                }
                else if (((IEnumerable<char>)this.Password.ToCharArray()).Any<char>((Func<char, bool>)(x => !MaskedTextProvider.IsValidPasswordChar(x))))
                {
                    flag = true;
                    Messages.ShowError("The password contains invalid characters. Please re-type them.");
                }
                else
                {
                    Exception exception = (Exception)null;
                    if (this.UnlockAccount)
                    {
                        try
                        {
                            SplashScreen.ShowSplashScreen((Form)this, "Unlocking user and\r\nresetting password...", false);
                            ((Action)(() => this.ADUser = ADUser.SetPasswordAndUnlock(this.Domain, this.Username, this.Password, this.chkChangeNextLogon.Checked))).InvokeInThread();
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }
                        finally
                        {
                            SplashScreen.CloseSplashScreen();
                            if (exception != null)
                                Messages.ShowError(string.Format("Failed to unlock and reset password for user '{0}'\r\n\r\n{1}", (object)this.DomainUsername, (object)exception.Message));
                            else
                                Messages.ShowInfo(string.Format("Successfully unlocked and reset password for user '{0}' ({1})", (object)this.DomainUsername, (object)this.ADUser.FullName));
                        }
                    }
                    else
                    {
                        try
                        {
                            SplashScreen.ShowSplashScreen((Form)this, "Resetting user password...", false);
                            ((Action)(() => this.ADUser = ADUser.SetPassword(this.Domain, this.Username, this.Password, this.chkChangeNextLogon.Checked))).InvokeInThread();
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }
                        finally
                        {
                            SplashScreen.CloseSplashScreen();
                            if (exception != null)
                            {
                                flag = true;
                                Messages.ShowError(string.Format("Failed reset password for user '{0}'\r\n\r\n{1}", (object)this.DomainUsername, (object)exception.Message));
                            }
                            else
                                Messages.ShowInfo(string.Format("Successfully reset password for user '{0}' ({1})", (object)this.DomainUsername, (object)this.ADUser.FullName));
                        }
                    }
                }
            }
            this.SetGUI(sender);
            this.txtPwd1.Clear();
            this.txtPwd2.Clear();
            if (flag)
            {
                this.txtPwd1.Focus();
                this.txtPwd1.Select();
            }
            else
            {
                this.chkSetPassword.Checked = false;
                this.chkUnlock.Checked = this.ADUser.LockedOut;
                this.txtAccountName.Focus();
                this.txtAccountName.Select();
            }
        }

        private void txtAccountName_Enter(object sender, EventArgs e)
        {
            this.txtAccountName.SelectAll();
        }

        private void btnQuit_Click(object sender, EventArgs e) => Program.Quit();

        private Bitmap DefaultImage
        {
            get
            {
                if (this._defaultImage != null)
                    return this._defaultImage;
                int num = (int)((double)this.userImage.Width * 0.7);
                int x1 = this.userImage.Width / 2 - num / 2;
                int y1 = this.userImage.Height - num / 2;
                int uint16 = (int)Decimal.ToUInt16((Decimal)((double)num * 0.6));
                int x2 = this.userImage.Width / 2 - uint16 / 2;
                int y2 = (int)((double)y1 * 0.15);
                Bitmap bitmap = new Bitmap(this.userImage.Width, this.userImage.Height);
                using (Graphics graphics = Graphics.FromImage((Image)bitmap))
                {
                    using (SolidBrush solidBrush1 = new SolidBrush(Color.LightGray))
                    {
                        using (SolidBrush solidBrush2 = new SolidBrush(Color.Gray))
                        {
                            graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            graphics.FillRectangle((Brush)solidBrush1, 0, 0, bitmap.Width, bitmap.Height);
                            graphics.FillEllipse((Brush)solidBrush2, x2, y2, uint16, uint16);
                            graphics.FillEllipse((Brush)solidBrush2, x1, y1, num, num);
                            this._defaultImage = bitmap;
                            return this._defaultImage;
                        }
                    }
                }
            }
        }

        private Dictionary<string, string> GetImages()
        {
            try
            {
                string[] commandLineArgs = Environment.GetCommandLineArgs();
                if (commandLineArgs.Length != 2 || !Directory.Exists(commandLineArgs[1]))
                    return (Dictionary<string, string>)null;
                Dictionary<string, string> images = new Dictionary<string, string>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);
                string[] source = new string[6]
                {
          ".bmp",
          ".gif",
          ".jpg",
          ".jpeg",
          ".png",
          ".tiff"
                };
                foreach (string file in Directory.GetFiles(commandLineArgs[1]))
                {
                    string extension = Path.GetExtension(file);
                    if (extension != null && ((IEnumerable<string>)source).Contains<string>(extension.ToLower()))
                    {
                        string withoutExtension = Path.GetFileNameWithoutExtension(file);
                        if (withoutExtension != null)
                        {
                            if (!images.ContainsKey(withoutExtension))
                                images.Add(withoutExtension, file);
                            else
                                images[withoutExtension] = file;
                        }
                    }
                }
                return images;
            }
            catch
            {
                return (Dictionary<string, string>)null;
            }
        }

        private Image GetUserImage(ADUser adUser)
        {
            try
            {
                if (adUser == null || this.Images == null)
                    return (Image)this.DefaultImage;
                if (this.Images.ContainsKey(adUser.Username))
                    return (Image)this.LoadImage(this.Images[adUser.Username]);
                return this.Images.ContainsKey(adUser.EQID) ? (Image)this.LoadImage(this.Images[adUser.EQID]) : (Image)this.DefaultImage;
            }
            catch
            {
                return (Image)this.DefaultImage;
            }
        }

        private Bitmap LoadImage(string path)
        {
            Bitmap bitmap;
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                Image original = Image.FromStream((Stream)fileStream);
                bitmap = new Bitmap(original);
                original.Dispose();
            }
            return bitmap;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = (IContainer)new System.ComponentModel.Container();
            this.lblName = new Label();
            this.btnAbout = new Button();
            this.btnFind = new Button();
            this.btnQuit = new Button();
            this.lblPasswordConfirm = new Label();
            this.txtPwd2 = new TextBox();
            this.label2 = new Label();
            this.txtPwd1 = new TextBox();
            this.btnOK = new Button();
            this.lblUsername = new Label();
            this.txtAccountName = new TextBox();
            this.chkDisabled = new CheckBox();
            this.grpProperties = new GroupBox();
            this.grpPassword = new GroupBox();
            this.lblPasswordLastSetValue = new Label();
            this.lblPasswordExpiresValue = new Label();
            this.lblPasswordExpired = new Label();
            this.lblPasswordLastset = new Label();
            this.chkPasswordExpired = new CheckBox();
            this.lblPasswordExpires = new Label();
            this.lblLockedOut = new Label();
            this.label3 = new Label();
            this.chkLockedOut = new CheckBox();
            this.chkChangeNextLogon = new CheckBox();
            this.grpManageUser = new GroupBox();
            this.chkSetPassword = new CheckBox();
            this.chkUnlock = new CheckBox();
            this.lblNameValue = new Label();
            this.toolTip = new ToolTip(this.components);
            this.userImage = new PictureBox();
            this.grpProperties.SuspendLayout();
            this.grpPassword.SuspendLayout();
            this.grpManageUser.SuspendLayout();
            ((ISupportInitialize)this.userImage).BeginInit();
            this.SuspendLayout();
            this.lblName.Anchor = AnchorStyles.Top;
            this.lblName.Location = new Point(32, 53);
            this.lblName.Name = "lblName";
            this.lblName.Size = new Size(55, 23);
            this.lblName.TabIndex = 20;
            this.lblName.Text = "Name:";
            this.lblName.TextAlign = ContentAlignment.MiddleLeft;
            this.btnAbout.Anchor = AnchorStyles.Bottom;
            this.btnAbout.Location = new Point(44, 583);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new Size(69, 23);
            this.btnAbout.TabIndex = 10;
            this.btnAbout.Text = "About";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new EventHandler(this.btnAbout_Click);
            this.btnFind.Anchor = AnchorStyles.Top;
            this.btnFind.Enabled = false;
            this.btnFind.Location = new Point(202, 19);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new Size(65, 23);
            this.btnFind.TabIndex = 1;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new EventHandler(this.btnCheckName_Click);
            this.btnQuit.Anchor = AnchorStyles.Bottom;
            this.btnQuit.Location = new Point(207, 583);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new Size(69, 23);
            this.btnQuit.TabIndex = 9;
            this.btnQuit.Text = "Quit";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new EventHandler(this.btnQuit_Click);
            this.lblPasswordConfirm.Location = new Point(17, 71);
            this.lblPasswordConfirm.Name = "lblPasswordConfirm";
            this.lblPasswordConfirm.Size = new Size(100, 23);
            this.lblPasswordConfirm.TabIndex = 18;
            this.lblPasswordConfirm.Text = "Confirm password:";
            this.lblPasswordConfirm.TextAlign = ContentAlignment.MiddleLeft;
            this.txtPwd2.Enabled = false;
            this.txtPwd2.Location = new Point(121, 73);
            this.txtPwd2.MaxLength = (int)sbyte.MaxValue;
            this.txtPwd2.Name = "txtPwd2";
            this.txtPwd2.Size = new Size(129, 20);
            this.txtPwd2.TabIndex = 6;
            this.txtPwd2.UseSystemPasswordChar = true;
            this.txtPwd2.TextChanged += new EventHandler(this.SetGUI);
            this.label2.Location = new Point(17, 45);
            this.label2.Name = "label2";
            this.label2.Size = new Size(100, 23);
            this.label2.TabIndex = 14;
            this.label2.Text = "New password:";
            this.label2.TextAlign = ContentAlignment.MiddleLeft;
            this.txtPwd1.Enabled = false;
            this.txtPwd1.Location = new Point(121, 47);
            this.txtPwd1.MaxLength = (int)sbyte.MaxValue;
            this.txtPwd1.Name = "txtPwd1";
            this.txtPwd1.Size = new Size(129, 20);
            this.txtPwd1.TabIndex = 5;
            this.txtPwd1.UseSystemPasswordChar = true;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new Point(181, 132);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(69, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new EventHandler(this.btnOK_Click);
            this.lblUsername.Anchor = AnchorStyles.Top;
            this.lblUsername.Location = new Point(32, 18);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new Size(62, 23);
            this.lblUsername.TabIndex = 11;
            this.lblUsername.Text = "Username:";
            this.lblUsername.TextAlign = ContentAlignment.MiddleLeft;
            this.txtAccountName.Anchor = AnchorStyles.Top;
            this.txtAccountName.Location = new Point(100, 21);
            this.txtAccountName.Name = "txtAccountName";
            this.txtAccountName.Size = new Size(87, 20);
            this.txtAccountName.TabIndex = 0;
            this.txtAccountName.TextChanged += new EventHandler(this.txtAccountName_TextChanged);
            this.txtAccountName.Enter += new EventHandler(this.txtAccountName_Enter);
            this.chkDisabled.AutoSize = true;
            this.chkDisabled.CheckAlign = ContentAlignment.MiddleRight;
            this.chkDisabled.Enabled = false;
            this.chkDisabled.Location = new Point(93, 26);
            this.chkDisabled.Name = "chkDisabled";
            this.chkDisabled.Size = new Size(15, 14);
            this.chkDisabled.TabIndex = 3;
            this.chkDisabled.UseVisualStyleBackColor = true;
            this.grpProperties.Anchor = AnchorStyles.Bottom;
            this.grpProperties.Controls.Add((Control)this.grpPassword);
            this.grpProperties.Controls.Add((Control)this.lblLockedOut);
            this.grpProperties.Controls.Add((Control)this.label3);
            this.grpProperties.Controls.Add((Control)this.chkLockedOut);
            this.grpProperties.Controls.Add((Control)this.chkDisabled);
            this.grpProperties.Location = new Point(25, 236);
            this.grpProperties.Name = "grpProperties";
            this.grpProperties.Size = new Size(264, 157);
            this.grpProperties.TabIndex = 23;
            this.grpProperties.TabStop = false;
            this.grpProperties.Text = "User properties";
            this.grpPassword.Controls.Add((Control)this.lblPasswordLastSetValue);
            this.grpPassword.Controls.Add((Control)this.lblPasswordExpiresValue);
            this.grpPassword.Controls.Add((Control)this.lblPasswordExpired);
            this.grpPassword.Controls.Add((Control)this.lblPasswordLastset);
            this.grpPassword.Controls.Add((Control)this.chkPasswordExpired);
            this.grpPassword.Controls.Add((Control)this.lblPasswordExpires);
            this.grpPassword.Location = new Point(11, 49);
            this.grpPassword.Name = "grpPassword";
            this.grpPassword.Size = new Size(240, 93);
            this.grpPassword.TabIndex = 31;
            this.grpPassword.TabStop = false;
            this.grpPassword.Text = "Password";
            this.lblPasswordLastSetValue.AutoSize = true;
            this.lblPasswordLastSetValue.Location = new Point(79, 66);
            this.lblPasswordLastSetValue.Name = "lblPasswordLastSetValue";
            this.lblPasswordLastSetValue.Size = new Size(0, 13);
            this.lblPasswordLastSetValue.TabIndex = 32;
            this.lblPasswordLastSetValue.TextAlign = ContentAlignment.MiddleLeft;
            this.lblPasswordExpiresValue.AutoSize = true;
            this.lblPasswordExpiresValue.Location = new Point(79, 44);
            this.lblPasswordExpiresValue.Name = "lblPasswordExpiresValue";
            this.lblPasswordExpiresValue.Size = new Size(0, 13);
            this.lblPasswordExpiresValue.TabIndex = 31;
            this.lblPasswordExpiresValue.TextAlign = ContentAlignment.MiddleLeft;
            this.lblPasswordExpired.AutoSize = true;
            this.lblPasswordExpired.Location = new Point(7, 22);
            this.lblPasswordExpired.Name = "lblPasswordExpired";
            this.lblPasswordExpired.Size = new Size(208, 13);
            this.lblPasswordExpired.TabIndex = 28;
            this.lblPasswordExpired.Text = "User must change password at next logon:";
            this.lblPasswordExpired.TextAlign = ContentAlignment.MiddleLeft;
            this.lblPasswordLastset.AutoSize = true;
            this.lblPasswordLastset.Location = new Point(7, 66);
            this.lblPasswordLastset.Name = "lblPasswordLastset";
            this.lblPasswordLastset.Size = new Size(47, 13);
            this.lblPasswordLastset.TabIndex = 30;
            this.lblPasswordLastset.Text = "Last set:";
            this.lblPasswordLastset.TextAlign = ContentAlignment.MiddleLeft;
            this.chkPasswordExpired.AutoSize = true;
            this.chkPasswordExpired.CheckAlign = ContentAlignment.MiddleRight;
            this.chkPasswordExpired.Enabled = false;
            this.chkPasswordExpired.Location = new Point(216, 22);
            this.chkPasswordExpired.Name = "chkPasswordExpired";
            this.chkPasswordExpired.Size = new Size(15, 14);
            this.chkPasswordExpired.TabIndex = 4;
            this.chkPasswordExpired.UseVisualStyleBackColor = true;
            this.lblPasswordExpires.AutoSize = true;
            this.lblPasswordExpires.Location = new Point(7, 44);
            this.lblPasswordExpires.Name = "lblPasswordExpires";
            this.lblPasswordExpires.Size = new Size(44, 13);
            this.lblPasswordExpires.TabIndex = 29;
            this.lblPasswordExpires.Text = "Expires:";
            this.lblPasswordExpires.TextAlign = ContentAlignment.MiddleLeft;
            this.lblLockedOut.AutoSize = true;
            this.lblLockedOut.Location = new Point(153, 27);
            this.lblLockedOut.Name = "lblLockedOut";
            this.lblLockedOut.Size = new Size(64, 13);
            this.lblLockedOut.TabIndex = 26;
            this.lblLockedOut.Text = "Locked out:";
            this.lblLockedOut.TextAlign = ContentAlignment.MiddleLeft;
            this.label3.AutoSize = true;
            this.label3.Location = new Point(17, 26);
            this.label3.Name = "label3";
            this.label3.Size = new Size(51, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Disabled:";
            this.label3.TextAlign = ContentAlignment.MiddleLeft;
            this.chkLockedOut.AutoSize = true;
            this.chkLockedOut.CheckAlign = ContentAlignment.MiddleRight;
            this.chkLockedOut.Enabled = false;
            this.chkLockedOut.Location = new Point(227, 27);
            this.chkLockedOut.Name = "chkLockedOut";
            this.chkLockedOut.Size = new Size(15, 14);
            this.chkLockedOut.TabIndex = 4;
            this.chkLockedOut.UseVisualStyleBackColor = true;
            this.chkChangeNextLogon.AutoSize = true;
            this.chkChangeNextLogon.Enabled = false;
            this.chkChangeNextLogon.Location = new Point(19, 106);
            this.chkChangeNextLogon.Name = "chkChangeNextLogon";
            this.chkChangeNextLogon.Size = new Size(241, 17);
            this.chkChangeNextLogon.TabIndex = 7;
            this.chkChangeNextLogon.Text = "Set user must change password at next logon";
            this.chkChangeNextLogon.UseVisualStyleBackColor = true;
            this.grpManageUser.Anchor = AnchorStyles.Bottom;
            this.grpManageUser.Controls.Add((Control)this.chkSetPassword);
            this.grpManageUser.Controls.Add((Control)this.chkUnlock);
            this.grpManageUser.Controls.Add((Control)this.btnOK);
            this.grpManageUser.Controls.Add((Control)this.label2);
            this.grpManageUser.Controls.Add((Control)this.chkChangeNextLogon);
            this.grpManageUser.Controls.Add((Control)this.txtPwd1);
            this.grpManageUser.Controls.Add((Control)this.txtPwd2);
            this.grpManageUser.Controls.Add((Control)this.lblPasswordConfirm);
            this.grpManageUser.Location = new Point(25, 401);
            this.grpManageUser.Name = "grpManageUser";
            this.grpManageUser.Size = new Size(264, 171);
            this.grpManageUser.TabIndex = 24;
            this.grpManageUser.TabStop = false;
            this.grpManageUser.Text = "Manage User";
            this.chkSetPassword.AutoSize = true;
            this.chkSetPassword.Enabled = false;
            this.chkSetPassword.Location = new Point(19, 25);
            this.chkSetPassword.Name = "chkSetPassword";
            this.chkSetPassword.Size = new Size(91, 17);
            this.chkSetPassword.TabIndex = 20;
            this.chkSetPassword.Text = "Set Password";
            this.chkSetPassword.UseVisualStyleBackColor = true;
            this.chkSetPassword.CheckedChanged += new EventHandler(this.SetGUI);
            this.chkUnlock.AutoSize = true;
            this.chkUnlock.Enabled = false;
            this.chkUnlock.Location = new Point(19, 136);
            this.chkUnlock.Name = "chkUnlock";
            this.chkUnlock.Size = new Size(102, 17);
            this.chkUnlock.TabIndex = 19;
            this.chkUnlock.Text = "Unlock account";
            this.chkUnlock.UseVisualStyleBackColor = true;
            this.chkUnlock.CheckedChanged += new EventHandler(this.SetGUI);
            this.lblNameValue.Anchor = AnchorStyles.Top;
            this.lblNameValue.BorderStyle = BorderStyle.Fixed3D;
            this.lblNameValue.Location = new Point(100, 54);
            this.lblNameValue.Name = "lblNameValue";
            this.lblNameValue.Size = new Size(176, 20);
            this.lblNameValue.TabIndex = 25;
            this.lblNameValue.TextAlign = ContentAlignment.MiddleLeft;
            this.userImage.Anchor = AnchorStyles.None;
            this.userImage.Location = new Point(67, 85);
            this.userImage.Name = "userImage";
            this.userImage.Size = new Size(180, 140);
            this.userImage.SizeMode = PictureBoxSizeMode.Zoom;
            this.userImage.TabIndex = 26;
            this.userImage.TabStop = false;
            this.AcceptButton = (IButtonControl)this.btnFind;
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(314, 621);
            this.Controls.Add((Control)this.userImage);
            this.Controls.Add((Control)this.lblNameValue);
            this.Controls.Add((Control)this.grpManageUser);
            this.Controls.Add((Control)this.grpProperties);
            this.Controls.Add((Control)this.lblName);
            this.Controls.Add((Control)this.btnAbout);
            this.Controls.Add((Control)this.btnFind);
            this.Controls.Add((Control)this.btnQuit);
            this.Controls.Add((Control)this.lblUsername);
            this.Controls.Add((Control)this.txtAccountName);
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Student Password Utility";
            this.Load += new EventHandler(this.MainForm_Load);
            this.grpProperties.ResumeLayout(false);
            this.grpProperties.PerformLayout();
            this.grpPassword.ResumeLayout(false);
            this.grpPassword.PerformLayout();
            this.grpManageUser.ResumeLayout(false);
            this.grpManageUser.PerformLayout();
            ((ISupportInitialize)this.userImage).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
