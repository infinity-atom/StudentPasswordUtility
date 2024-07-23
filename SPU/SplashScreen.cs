// Decompiled with JetBrains decompiler
// Type: SPU.SplashScreen
// Assembly: SPU, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BE280C72-BC6C-40D8-B4E7-47FDE97A70CF
// Assembly location: C:\Users\infinity-atom\Documents\SPU.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace SPU
{
  public class SplashScreen : Form
  {
    private const int TimerInterval = 50;
    private static SplashScreen _instance;
    private const double OpacityIncrement = 0.05;
    private IContainer components;
    private Label lblStatus;
    private Timer timer1;

    public SplashScreen()
    {
      this.InitializeComponent();
      this.Initialise();
    }

    public static bool Exists
    {
      get => SplashScreen._instance != null && !SplashScreen._instance.IsDisposed;
    }

    public static Color Color { get; set; } = Color.Empty;

    public new Form ParentForm { get; set; }

    public static void CloseSplashScreen()
    {
      if (SplashScreen._instance == null)
        return;
      if (SplashScreen._instance.InvokeRequired)
      {
        if (SplashScreen._instance.IsHandleCreated)
        {
          try
          {
            SplashScreen._instance.Invoke((Delegate) new MethodInvoker(SplashScreen.CloseSplashScreen));
            return;
          }
          catch (InvalidOperationException ex)
          {
            return;
          }
          catch (InvalidAsynchronousStateException ex)
          {
            return;
          }
        }
      }
      SplashScreen._instance.Close();
      if (SplashScreen._instance.ParentForm == null)
        return;
      SplashScreen._instance.ParentForm.Enabled = true;
      SplashScreen._instance.ParentForm.Focus();
    }

    public static void ShowSplashScreen(string text, bool showInTaskbar)
    {
      SplashScreen.ShowSplashScreen((Form) null, text, showInTaskbar);
    }

    public static void ShowSplashScreen(Form parent, string text, bool showInTaskbar)
    {
      SplashScreen.ShowForm(parent, text, showInTaskbar);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.components?.Dispose();
      base.Dispose(disposing);
    }

    private static void ShowForm(Form parent, string text, bool showInTaskbar)
    {
      bool flag = !SplashScreen.Exists;
      if (flag)
        SplashScreen._instance = new SplashScreen();
      else if (SplashScreen._instance.InvokeRequired)
      {
        SplashScreen._instance.Invoke((Action) (() => SplashScreen.ShowForm(parent, text, showInTaskbar)));
        return;
      }
      if (SplashScreen._instance.ParentForm != null && (parent == null || SplashScreen._instance.ParentForm != parent))
        SplashScreen._instance.ParentForm.Enabled = true;
      SplashScreen._instance.ParentForm = parent;
      if (SplashScreen._instance.ParentForm != null)
        SplashScreen._instance.ParentForm.Enabled = false;
      SplashScreen._instance.ParentForm = parent;
      SplashScreen._instance.ShowInTaskbar = showInTaskbar;
      SplashScreen._instance.Text = text;
      SplashScreen._instance.lblStatus.Text = text;
      if (SplashScreen.Color == Color.Empty && SplashScreen._instance.BackgroundImage != null)
      {
        SplashScreen._instance.ClientSize = SplashScreen._instance.BackgroundImage.Size;
      }
      else
      {
        SplashScreen._instance.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        SplashScreen._instance.BackColor = SplashScreen.Color;
        SplashScreen._instance.BackgroundImage = (Image) null;
      }
      SplashScreen._instance.SetPositionAndSize();
      if (!flag || SplashScreen._instance.Visible)
        return;
      SplashScreen._instance.Show((IWin32Window) parent);
    }

    private void Initialise()
    {
      this.Opacity = 0.0;
      this.timer1.Interval = 50;
      this.timer1.Start();
    }

    private void SetPositionAndSize()
    {
      if (this.InvokeRequired)
      {
        this.Invoke((Delegate) new MethodInvoker(this.SetPositionAndSize));
      }
      else
      {
        Label lblStatus = this.lblStatus;
        Size size1 = Screen.FromControl((Control) this).Bounds.Size;
        int width1 = size1.Width / 2;
        size1 = Screen.FromControl((Control) this).Bounds.Size;
        int height1 = size1.Height / 2;
        Size size2 = new Size(width1, height1);
        lblStatus.MaximumSize = size2;
        size1 = this.lblStatus.Size;
        int width2 = size1.Width + 80;
        size1 = this.lblStatus.Size;
        int height2 = size1.Height + 80;
        this.Size = new Size(width2, height2);
        size1 = this.Size;
        int num1 = size1.Width / 2;
        size1 = this.lblStatus.Size;
        int num2 = size1.Width / 2;
        int x1 = num1 - num2;
        size1 = this.Size;
        int num3 = size1.Height / 2;
        size1 = this.lblStatus.Size;
        int num4 = size1.Height / 2;
        int y1 = num3 - num4;
        this.lblStatus.Location = new Point(x1, y1);
        this.StartPosition = FormStartPosition.CenterParent;
        if (this.ParentForm != null)
        {
          int x2 = this.ParentForm.Location.X;
          size1 = this.ParentForm.Size;
          int num5 = size1.Width / 2;
          int num6 = x2 + num5;
          size1 = this.Size;
          int num7 = size1.Width / 2;
          int x3 = num6 - num7;
          int y2 = this.ParentForm.Location.Y;
          size1 = this.ParentForm.Size;
          int num8 = size1.Height / 2;
          int num9 = y2 + num8;
          size1 = this.Size;
          int num10 = size1.Height / 2;
          int y3 = num9 - num10;
          this.Location = new Point(x3, y3);
        }
        this.Focus();
      }
    }

    private void SplashScreen_Load(object sender, EventArgs e)
    {
      this.WindowState = FormWindowState.Normal;
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      if (this.InvokeRequired)
      {
        this.Invoke((Action) (() => this.timer1_Tick(sender, e)));
      }
      else
      {
        if (this.Opacity >= 0.8)
          return;
        this.Opacity += 0.05;
      }
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (SplashScreen));
      this.lblStatus = new Label();
      this.timer1 = new Timer(this.components);
      this.SuspendLayout();
      this.lblStatus.Anchor = AnchorStyles.Left;
      this.lblStatus.AutoSize = true;
      this.lblStatus.BackColor = Color.Transparent;
      this.lblStatus.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.lblStatus.Location = new Point(152, 116);
      this.lblStatus.Name = "lblStatus";
      this.lblStatus.Size = new Size(0, 17);
      this.lblStatus.TabIndex = 0;
      this.lblStatus.TextAlign = ContentAlignment.MiddleLeft;
      this.timer1.Tick += new EventHandler(this.timer1_Tick);
      this.AutoScaleBaseSize = new Size(5, 13);
      this.BackColor = Color.LightGray;
      this.ClientSize = new Size(419, 231);
      this.Controls.Add((Control) this.lblStatus);
      this.FormBorderStyle = FormBorderStyle.None;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.Name = "SplashScreen";
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "Joiner";
      this.Load += new EventHandler(this.SplashScreen_Load);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
