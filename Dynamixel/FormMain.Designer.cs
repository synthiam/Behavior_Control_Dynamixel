namespace Dynamixel {
  partial class FormMain {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.btnForceInit = new System.Windows.Forms.Button();
      this.panel1 = new System.Windows.Forms.Panel();
      this.btnClearLog = new System.Windows.Forms.Button();
      this.tbLog = new System.Windows.Forms.TextBox();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnForceInit
      // 
      this.btnForceInit.Dock = System.Windows.Forms.DockStyle.Left;
      this.btnForceInit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnForceInit.Location = new System.Drawing.Point(0, 0);
      this.btnForceInit.Name = "btnForceInit";
      this.btnForceInit.Size = new System.Drawing.Size(127, 30);
      this.btnForceInit.TabIndex = 2;
      this.btnForceInit.Text = "Force UART Init";
      this.btnForceInit.UseVisualStyleBackColor = true;
      this.btnForceInit.Click += new System.EventHandler(this.btnForceInit_Click);
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnClearLog);
      this.panel1.Controls.Add(this.btnForceInit);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(251, 30);
      this.panel1.TabIndex = 4;
      // 
      // btnClearLog
      // 
      this.btnClearLog.Dock = System.Windows.Forms.DockStyle.Left;
      this.btnClearLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnClearLog.Location = new System.Drawing.Point(127, 0);
      this.btnClearLog.Name = "btnClearLog";
      this.btnClearLog.Size = new System.Drawing.Size(86, 30);
      this.btnClearLog.TabIndex = 3;
      this.btnClearLog.Text = "Clear Log";
      this.btnClearLog.UseVisualStyleBackColor = true;
      this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
      // 
      // tbLog
      // 
      this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tbLog.Location = new System.Drawing.Point(0, 30);
      this.tbLog.Multiline = true;
      this.tbLog.Name = "tbLog";
      this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.tbLog.Size = new System.Drawing.Size(251, 45);
      this.tbLog.TabIndex = 5;
      // 
      // FormMain
      // 
      this.ClientSize = new System.Drawing.Size(251, 75);
      this.Controls.Add(this.tbLog);
      this.Controls.Add(this.panel1);
      this.Name = "FormMain";
      this.Text = "Form1";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
      this.Load += new System.EventHandler(this.Form1_Load);
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Button btnForceInit;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.TextBox tbLog;
    private System.Windows.Forms.Button btnClearLog;
  }
}

