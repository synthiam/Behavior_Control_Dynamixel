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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.ucConfigurationButton1 = new EZ_Builder.UCForms.UC.UCConfigurationButton();
      this.btnForceInit = new System.Windows.Forms.Button();
      this.panel1 = new System.Windows.Forms.Panel();
      this.tbLog = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // pictureBox1
      // 
      this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
      this.pictureBox1.Image = global::Dynamixel.Properties.Resources.Dynamixel;
      this.pictureBox1.Location = new System.Drawing.Point(0, 30);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(130, 99);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // ucConfigurationButton1
      // 
      this.ucConfigurationButton1.Dock = System.Windows.Forms.DockStyle.Left;
      this.ucConfigurationButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.ucConfigurationButton1.Image = ((System.Drawing.Image)(resources.GetObject("ucConfigurationButton1.Image")));
      this.ucConfigurationButton1.Location = new System.Drawing.Point(0, 0);
      this.ucConfigurationButton1.Name = "ucConfigurationButton1";
      this.ucConfigurationButton1.Size = new System.Drawing.Size(30, 30);
      this.ucConfigurationButton1.TabIndex = 1;
      this.ucConfigurationButton1.UseVisualStyleBackColor = true;
      this.ucConfigurationButton1.Click += new System.EventHandler(this.ucConfigurationButton1_Click);
      // 
      // btnForceInit
      // 
      this.btnForceInit.Dock = System.Windows.Forms.DockStyle.Right;
      this.btnForceInit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnForceInit.Location = new System.Drawing.Point(168, 0);
      this.btnForceInit.Name = "btnForceInit";
      this.btnForceInit.Size = new System.Drawing.Size(133, 30);
      this.btnForceInit.TabIndex = 2;
      this.btnForceInit.Text = "Force UART Init";
      this.btnForceInit.UseVisualStyleBackColor = true;
      this.btnForceInit.Click += new System.EventHandler(this.btnForceInit_Click);
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.ucConfigurationButton1);
      this.panel1.Controls.Add(this.btnForceInit);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(301, 30);
      this.panel1.TabIndex = 4;
      // 
      // tbLog
      // 
      this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tbLog.Location = new System.Drawing.Point(130, 30);
      this.tbLog.Multiline = true;
      this.tbLog.Name = "tbLog";
      this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.tbLog.Size = new System.Drawing.Size(171, 99);
      this.tbLog.TabIndex = 5;
      // 
      // FormMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(301, 129);
      this.Controls.Add(this.tbLog);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.panel1);
      this.Name = "FormMain";
      this.Text = "Form1";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
      this.Load += new System.EventHandler(this.Form1_Load);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBox1;
    private EZ_Builder.UCForms.UC.UCConfigurationButton ucConfigurationButton1;
    private System.Windows.Forms.Button btnForceInit;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.TextBox tbLog;
  }
}

