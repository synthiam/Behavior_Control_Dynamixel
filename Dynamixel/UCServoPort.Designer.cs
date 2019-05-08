namespace Dynamixel {
  partial class UCServoPort {
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.cbVersion = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.tbMaxResolution = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.cbEnabled = new System.Windows.Forms.CheckBox();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.cbVersion);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.tbMaxResolution);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.cbEnabled);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Location = new System.Drawing.Point(0, 0);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(443, 55);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "groupBox1";
      // 
      // cbVersion
      // 
      this.cbVersion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.cbVersion.Dock = System.Windows.Forms.DockStyle.Left;
      this.cbVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbVersion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.cbVersion.FormattingEnabled = true;
      this.cbVersion.Location = new System.Drawing.Point(283, 16);
      this.cbVersion.Name = "cbVersion";
      this.cbVersion.Size = new System.Drawing.Size(85, 21);
      this.cbVersion.TabIndex = 4;
      this.cbVersion.SelectedIndexChanged += new System.EventHandler(this.cbVersion_SelectedIndexChanged);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Dock = System.Windows.Forms.DockStyle.Left;
      this.label2.Location = new System.Drawing.Point(238, 16);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(45, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Version:";
      // 
      // tbMaxResolution
      // 
      this.tbMaxResolution.Dock = System.Windows.Forms.DockStyle.Left;
      this.tbMaxResolution.Location = new System.Drawing.Point(138, 16);
      this.tbMaxResolution.Name = "tbMaxResolution";
      this.tbMaxResolution.Size = new System.Drawing.Size(100, 20);
      this.tbMaxResolution.TabIndex = 2;
      this.tbMaxResolution.Text = "1024";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Dock = System.Windows.Forms.DockStyle.Left;
      this.label1.Location = new System.Drawing.Point(68, 16);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(70, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Max Position:";
      // 
      // cbEnabled
      // 
      this.cbEnabled.AutoSize = true;
      this.cbEnabled.Dock = System.Windows.Forms.DockStyle.Left;
      this.cbEnabled.Location = new System.Drawing.Point(3, 16);
      this.cbEnabled.Name = "cbEnabled";
      this.cbEnabled.Size = new System.Drawing.Size(65, 36);
      this.cbEnabled.TabIndex = 1;
      this.cbEnabled.Text = "Enabled";
      this.cbEnabled.UseVisualStyleBackColor = true;
      this.cbEnabled.CheckedChanged += new System.EventHandler(this.cbEnabled_CheckedChanged);
      // 
      // UCServoPort
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.Controls.Add(this.groupBox1);
      this.Name = "UCServoPort";
      this.Size = new System.Drawing.Size(443, 55);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox tbMaxResolution;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox cbVersion;
    private System.Windows.Forms.CheckBox cbEnabled;
  }
}
