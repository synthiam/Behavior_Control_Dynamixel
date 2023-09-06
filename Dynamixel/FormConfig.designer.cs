using ARC.UCForms;
namespace Dynamixel {
  partial class FormConfig {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfig));
      this.button8 = new System.Windows.Forms.Button();
      this.button6 = new System.Windows.Forms.Button();
      this.btnLEDOff = new System.Windows.Forms.Button();
      this.btnLEDOn = new System.Windows.Forms.Button();
      this.cbTestServoPort = new System.Windows.Forms.ComboBox();
      this.button1 = new System.Windows.Forms.Button();
      this.btnChangeID = new System.Windows.Forms.Button();
      this.cbChangeIDNew = new System.Windows.Forms.ComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.groupBox5 = new System.Windows.Forms.GroupBox();
      this.label6 = new System.Windows.Forms.Label();
      this.groupBox9 = new System.Windows.Forms.GroupBox();
      this.ucHelpHover6 = new ARC.UCForms.UC.UCHelpHover();
      this.rbUARTPort2 = new System.Windows.Forms.RadioButton();
      this.ucHelpHover5 = new ARC.UCForms.UC.UCHelpHover();
      this.rbUARTPort0 = new System.Windows.Forms.RadioButton();
      this.ucHelpHover4 = new ARC.UCForms.UC.UCHelpHover();
      this.ucHelpHover3 = new ARC.UCForms.UC.UCHelpHover();
      this.ucPortButton1 = new ARC.UCForms.UC.UCPortButton();
      this.rbDigitalPort = new System.Windows.Forms.RadioButton();
      this.rbUARTPort1 = new System.Windows.Forms.RadioButton();
      this.button4 = new System.Windows.Forms.Button();
      this.ucHelpHover2 = new ARC.UCForms.UC.UCHelpHover();
      this.tbBaudRate = new System.Windows.Forms.TextBox();
      this.label7 = new System.Windows.Forms.Label();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.panel1 = new System.Windows.Forms.Panel();
      this.ucTabControl1 = new ARC.UCForms.UCTabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tbLog = new System.Windows.Forms.TextBox();
      this.btnScanAndFind = new System.Windows.Forms.Button();
      this.groupBox7 = new System.Windows.Forms.GroupBox();
      this.button2 = new System.Windows.Forms.Button();
      this.btnResetToDefaults = new System.Windows.Forms.Button();
      this.btnChangeBaud = new System.Windows.Forms.Button();
      this.cbBaudRates = new System.Windows.Forms.ComboBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.cbVersion = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.cbDebug = new System.Windows.Forms.CheckBox();
      this.ucHelpHover1 = new ARC.UCForms.UC.UCHelpHover();
      this.groupBox5.SuspendLayout();
      this.groupBox9.SuspendLayout();
      this.panel1.SuspendLayout();
      this.ucTabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.groupBox7.SuspendLayout();
      this.SuspendLayout();
      // 
      // button8
      // 
      this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.button8.Location = new System.Drawing.Point(99, 102);
      this.button8.Name = "button8";
      this.button8.Size = new System.Drawing.Size(75, 71);
      this.button8.TabIndex = 10;
      this.button8.Text = "Ping";
      this.button8.UseVisualStyleBackColor = true;
      this.button8.Click += new System.EventHandler(this.button8_Click);
      // 
      // button6
      // 
      this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.button6.Location = new System.Drawing.Point(180, 102);
      this.button6.Name = "button6";
      this.button6.Size = new System.Drawing.Size(75, 71);
      this.button6.TabIndex = 6;
      this.button6.Text = "Release Servo";
      this.button6.UseVisualStyleBackColor = true;
      this.button6.Click += new System.EventHandler(this.button6_Click);
      // 
      // btnLEDOff
      // 
      this.btnLEDOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnLEDOff.Location = new System.Drawing.Point(16, 139);
      this.btnLEDOff.Name = "btnLEDOff";
      this.btnLEDOff.Size = new System.Drawing.Size(75, 34);
      this.btnLEDOff.TabIndex = 4;
      this.btnLEDOff.Text = "LED Off";
      this.btnLEDOff.UseVisualStyleBackColor = true;
      this.btnLEDOff.Click += new System.EventHandler(this.btnLEDOff_Click);
      // 
      // btnLEDOn
      // 
      this.btnLEDOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnLEDOn.Location = new System.Drawing.Point(16, 102);
      this.btnLEDOn.Name = "btnLEDOn";
      this.btnLEDOn.Size = new System.Drawing.Size(75, 34);
      this.btnLEDOn.TabIndex = 3;
      this.btnLEDOn.Text = "LED On";
      this.btnLEDOn.UseVisualStyleBackColor = true;
      this.btnLEDOn.Click += new System.EventHandler(this.btnLEDOn_Click);
      // 
      // cbTestServoPort
      // 
      this.cbTestServoPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.cbTestServoPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbTestServoPort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.cbTestServoPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cbTestServoPort.FormattingEnabled = true;
      this.cbTestServoPort.Location = new System.Drawing.Point(79, 19);
      this.cbTestServoPort.Name = "cbTestServoPort";
      this.cbTestServoPort.Size = new System.Drawing.Size(88, 25);
      this.cbTestServoPort.TabIndex = 1;
      // 
      // button1
      // 
      this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.button1.Location = new System.Drawing.Point(195, 265);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(64, 48);
      this.button1.TabIndex = 7;
      this.button1.Text = "Next";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // btnChangeID
      // 
      this.btnChangeID.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnChangeID.Location = new System.Drawing.Point(265, 265);
      this.btnChangeID.Name = "btnChangeID";
      this.btnChangeID.Size = new System.Drawing.Size(75, 48);
      this.btnChangeID.TabIndex = 6;
      this.btnChangeID.Text = "Execute";
      this.btnChangeID.UseVisualStyleBackColor = true;
      this.btnChangeID.Click += new System.EventHandler(this.btnChangeID_Click);
      // 
      // cbChangeIDNew
      // 
      this.cbChangeIDNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.cbChangeIDNew.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbChangeIDNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.cbChangeIDNew.FormattingEnabled = true;
      this.cbChangeIDNew.Location = new System.Drawing.Point(95, 280);
      this.cbChangeIDNew.Name = "cbChangeIDNew";
      this.cbChangeIDNew.Size = new System.Drawing.Size(79, 21);
      this.cbChangeIDNew.TabIndex = 5;
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(14, 280);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(75, 22);
      this.label3.TabIndex = 4;
      this.label3.Text = "New ID:";
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // groupBox5
      // 
      this.groupBox5.Controls.Add(this.ucHelpHover1);
      this.groupBox5.Controls.Add(this.cbDebug);
      this.groupBox5.Controls.Add(this.label6);
      this.groupBox5.Controls.Add(this.button4);
      this.groupBox5.Controls.Add(this.ucHelpHover2);
      this.groupBox5.Controls.Add(this.tbBaudRate);
      this.groupBox5.Controls.Add(this.label7);
      this.groupBox5.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBox5.Location = new System.Drawing.Point(3, 3);
      this.groupBox5.Name = "groupBox5";
      this.groupBox5.Size = new System.Drawing.Size(1047, 196);
      this.groupBox5.TabIndex = 7;
      this.groupBox5.TabStop = false;
      this.groupBox5.Text = "Settings";
      // 
      // label6
      // 
      this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
      this.label6.Location = new System.Drawing.Point(494, 16);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(395, 152);
      this.label6.TabIndex = 17;
      this.label6.Text = resources.GetString("label6.Text");
      // 
      // groupBox9
      // 
      this.groupBox9.Controls.Add(this.ucHelpHover6);
      this.groupBox9.Controls.Add(this.rbUARTPort2);
      this.groupBox9.Controls.Add(this.ucHelpHover5);
      this.groupBox9.Controls.Add(this.rbUARTPort0);
      this.groupBox9.Controls.Add(this.ucHelpHover4);
      this.groupBox9.Controls.Add(this.ucHelpHover3);
      this.groupBox9.Controls.Add(this.ucPortButton1);
      this.groupBox9.Controls.Add(this.rbDigitalPort);
      this.groupBox9.Controls.Add(this.rbUARTPort1);
      this.groupBox9.Location = new System.Drawing.Point(3, 82);
      this.groupBox9.Name = "groupBox9";
      this.groupBox9.Size = new System.Drawing.Size(488, 111);
      this.groupBox9.TabIndex = 16;
      this.groupBox9.TabStop = false;
      this.groupBox9.Text = "Port";
      // 
      // ucHelpHover6
      // 
      this.ucHelpHover6.BackColor = System.Drawing.Color.Transparent;
      this.ucHelpHover6.Location = new System.Drawing.Point(7, 63);
      this.ucHelpHover6.Margin = new System.Windows.Forms.Padding(0);
      this.ucHelpHover6.Name = "ucHelpHover6";
      this.ucHelpHover6.SetHelpText = "Use the UART output port";
      this.ucHelpHover6.Size = new System.Drawing.Size(20, 20);
      this.ucHelpHover6.TabIndex = 22;
      // 
      // rbUARTPort2
      // 
      this.rbUARTPort2.AutoSize = true;
      this.rbUARTPort2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.rbUARTPort2.Location = new System.Drawing.Point(30, 65);
      this.rbUARTPort2.Name = "rbUARTPort2";
      this.rbUARTPort2.Size = new System.Drawing.Size(92, 17);
      this.rbUARTPort2.TabIndex = 21;
      this.rbUARTPort2.TabStop = true;
      this.rbUARTPort2.Text = "UART Port #2";
      this.rbUARTPort2.UseVisualStyleBackColor = true;
      // 
      // ucHelpHover5
      // 
      this.ucHelpHover5.BackColor = System.Drawing.Color.Transparent;
      this.ucHelpHover5.Location = new System.Drawing.Point(7, 17);
      this.ucHelpHover5.Margin = new System.Windows.Forms.Padding(0);
      this.ucHelpHover5.Name = "ucHelpHover5";
      this.ucHelpHover5.SetHelpText = "Use the UART output port";
      this.ucHelpHover5.Size = new System.Drawing.Size(20, 20);
      this.ucHelpHover5.TabIndex = 20;
      // 
      // rbUARTPort0
      // 
      this.rbUARTPort0.AutoSize = true;
      this.rbUARTPort0.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.rbUARTPort0.Location = new System.Drawing.Point(30, 19);
      this.rbUARTPort0.Name = "rbUARTPort0";
      this.rbUARTPort0.Size = new System.Drawing.Size(92, 17);
      this.rbUARTPort0.TabIndex = 19;
      this.rbUARTPort0.TabStop = true;
      this.rbUARTPort0.Text = "UART Port #0";
      this.rbUARTPort0.UseVisualStyleBackColor = true;
      // 
      // ucHelpHover4
      // 
      this.ucHelpHover4.BackColor = System.Drawing.Color.Transparent;
      this.ucHelpHover4.Location = new System.Drawing.Point(7, 87);
      this.ucHelpHover4.Margin = new System.Windows.Forms.Padding(0);
      this.ucHelpHover4.Name = "ucHelpHover4";
      this.ucHelpHover4.SetHelpText = "Use any selected digital port for low speed serial output (max 115200 baud rate)." +
    "  Use Hardware UART if available. Only use this as last resort.";
      this.ucHelpHover4.Size = new System.Drawing.Size(20, 20);
      this.ucHelpHover4.TabIndex = 18;
      // 
      // ucHelpHover3
      // 
      this.ucHelpHover3.BackColor = System.Drawing.Color.Transparent;
      this.ucHelpHover3.Location = new System.Drawing.Point(7, 40);
      this.ucHelpHover3.Margin = new System.Windows.Forms.Padding(0);
      this.ucHelpHover3.Name = "ucHelpHover3";
      this.ucHelpHover3.SetHelpText = "Use the UART output port";
      this.ucHelpHover3.Size = new System.Drawing.Size(20, 20);
      this.ucHelpHover3.TabIndex = 17;
      // 
      // ucPortButton1
      // 
      this.ucPortButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.ucPortButton1.Location = new System.Drawing.Point(155, 77);
      this.ucPortButton1.Name = "ucPortButton1";
      this.ucPortButton1.Size = new System.Drawing.Size(75, 30);
      this.ucPortButton1.TabIndex = 2;
      this.ucPortButton1.Text = "ucPortButton1";
      this.ucPortButton1.UseVisualStyleBackColor = true;
      // 
      // rbDigitalPort
      // 
      this.rbDigitalPort.AutoSize = true;
      this.rbDigitalPort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.rbDigitalPort.Location = new System.Drawing.Point(30, 88);
      this.rbDigitalPort.Name = "rbDigitalPort";
      this.rbDigitalPort.Size = new System.Drawing.Size(119, 17);
      this.rbDigitalPort.TabIndex = 1;
      this.rbDigitalPort.TabStop = true;
      this.rbDigitalPort.Text = "Digital Port (serial tx)";
      this.rbDigitalPort.UseVisualStyleBackColor = true;
      // 
      // rbUARTPort1
      // 
      this.rbUARTPort1.AutoSize = true;
      this.rbUARTPort1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.rbUARTPort1.Location = new System.Drawing.Point(30, 42);
      this.rbUARTPort1.Name = "rbUARTPort1";
      this.rbUARTPort1.Size = new System.Drawing.Size(92, 17);
      this.rbUARTPort1.TabIndex = 0;
      this.rbUARTPort1.TabStop = true;
      this.rbUARTPort1.Text = "UART Port #1";
      this.rbUARTPort1.UseVisualStyleBackColor = true;
      // 
      // button4
      // 
      this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.button4.Location = new System.Drawing.Point(220, 19);
      this.button4.Name = "button4";
      this.button4.Size = new System.Drawing.Size(176, 32);
      this.button4.TabIndex = 12;
      this.button4.Text = "Set Baud Rate";
      this.button4.UseVisualStyleBackColor = true;
      this.button4.Click += new System.EventHandler(this.button4_Click);
      // 
      // ucHelpHover2
      // 
      this.ucHelpHover2.BackColor = System.Drawing.Color.Transparent;
      this.ucHelpHover2.Location = new System.Drawing.Point(197, 24);
      this.ucHelpHover2.Margin = new System.Windows.Forms.Padding(0);
      this.ucHelpHover2.Name = "ucHelpHover2";
      this.ucHelpHover2.SetHelpText = resources.GetString("ucHelpHover2.SetHelpText");
      this.ucHelpHover2.Size = new System.Drawing.Size(20, 20);
      this.ucHelpHover2.TabIndex = 11;
      // 
      // tbBaudRate
      // 
      this.tbBaudRate.Location = new System.Drawing.Point(94, 24);
      this.tbBaudRate.Name = "tbBaudRate";
      this.tbBaudRate.Size = new System.Drawing.Size(100, 20);
      this.tbBaudRate.TabIndex = 3;
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(6, 27);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(61, 13);
      this.label7.TabIndex = 2;
      this.label7.Text = "Baud Rate:";
      // 
      // btnSave
      // 
      this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnSave.Location = new System.Drawing.Point(8, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 41);
      this.btnSave.TabIndex = 8;
      this.btnSave.Text = "&Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnCancel.Location = new System.Drawing.Point(103, 4);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 41);
      this.btnCancel.TabIndex = 9;
      this.btnCancel.Text = "&Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnSave);
      this.panel1.Controls.Add(this.btnCancel);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 598);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(1061, 48);
      this.panel1.TabIndex = 10;
      // 
      // ucTabControl1
      // 
      this.ucTabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
      this.ucTabControl1.BackColor = System.Drawing.Color.White;
      this.ucTabControl1.ButtonBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(170)))), ((int)(((byte)(234)))));
      this.ucTabControl1.ButtonSelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(146)))), ((int)(((byte)(66)))));
      this.ucTabControl1.ButtonTextColor = System.Drawing.Color.White;
      this.ucTabControl1.Controls.Add(this.tabPage1);
      this.ucTabControl1.Controls.Add(this.tabPage2);
      this.ucTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucTabControl1.ItemSize = new System.Drawing.Size(60, 50);
      this.ucTabControl1.Location = new System.Drawing.Point(0, 0);
      this.ucTabControl1.Margin = new System.Windows.Forms.Padding(0);
      this.ucTabControl1.MarginLeft = 0;
      this.ucTabControl1.MarginTop = 0;
      this.ucTabControl1.Multiline = true;
      this.ucTabControl1.Name = "ucTabControl1";
      this.ucTabControl1.Padding = new System.Drawing.Point(0, 0);
      this.ucTabControl1.SelectedIndex = 0;
      this.ucTabControl1.Size = new System.Drawing.Size(1061, 598);
      this.ucTabControl1.TabIndex = 11;
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.flowLayoutPanel1);
      this.tabPage1.Controls.Add(this.groupBox9);
      this.tabPage1.Controls.Add(this.groupBox5);
      this.tabPage1.Location = new System.Drawing.Point(4, 54);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(1053, 540);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Settings";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.AutoScroll = true;
      this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 199);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(1047, 338);
      this.flowLayoutPanel1.TabIndex = 8;
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.groupBox1);
      this.tabPage2.Controls.Add(this.groupBox7);
      this.tabPage2.Location = new System.Drawing.Point(4, 54);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(1053, 540);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Utilities";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.tbLog);
      this.groupBox1.Controls.Add(this.btnScanAndFind);
      this.groupBox1.Location = new System.Drawing.Point(410, 26);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(342, 486);
      this.groupBox1.TabIndex = 11;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Scan";
      // 
      // tbLog
      // 
      this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tbLog.Location = new System.Drawing.Point(3, 63);
      this.tbLog.Multiline = true;
      this.tbLog.Name = "tbLog";
      this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.tbLog.Size = new System.Drawing.Size(336, 420);
      this.tbLog.TabIndex = 9;
      // 
      // btnScanAndFind
      // 
      this.btnScanAndFind.Dock = System.Windows.Forms.DockStyle.Top;
      this.btnScanAndFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnScanAndFind.Location = new System.Drawing.Point(3, 16);
      this.btnScanAndFind.Name = "btnScanAndFind";
      this.btnScanAndFind.Size = new System.Drawing.Size(336, 47);
      this.btnScanAndFind.TabIndex = 10;
      this.btnScanAndFind.Text = "Scan And Find All Servos";
      this.btnScanAndFind.UseVisualStyleBackColor = true;
      this.btnScanAndFind.Click += new System.EventHandler(this.btnScanAndFind_Click);
      // 
      // groupBox7
      // 
      this.groupBox7.Controls.Add(this.button2);
      this.groupBox7.Controls.Add(this.btnResetToDefaults);
      this.groupBox7.Controls.Add(this.button8);
      this.groupBox7.Controls.Add(this.btnChangeBaud);
      this.groupBox7.Controls.Add(this.button6);
      this.groupBox7.Controls.Add(this.cbBaudRates);
      this.groupBox7.Controls.Add(this.btnLEDOff);
      this.groupBox7.Controls.Add(this.button1);
      this.groupBox7.Controls.Add(this.btnLEDOn);
      this.groupBox7.Controls.Add(this.label4);
      this.groupBox7.Controls.Add(this.label2);
      this.groupBox7.Controls.Add(this.btnChangeID);
      this.groupBox7.Controls.Add(this.cbChangeIDNew);
      this.groupBox7.Controls.Add(this.cbVersion);
      this.groupBox7.Controls.Add(this.label3);
      this.groupBox7.Controls.Add(this.label1);
      this.groupBox7.Controls.Add(this.cbTestServoPort);
      this.groupBox7.Location = new System.Drawing.Point(8, 26);
      this.groupBox7.Name = "groupBox7";
      this.groupBox7.Size = new System.Drawing.Size(359, 486);
      this.groupBox7.TabIndex = 8;
      this.groupBox7.TabStop = false;
      this.groupBox7.Text = "Manage Servo";
      // 
      // button2
      // 
      this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.button2.Location = new System.Drawing.Point(261, 102);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(79, 71);
      this.button2.TabIndex = 12;
      this.button2.Text = "Read Position";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.Button2_Click);
      // 
      // btnResetToDefaults
      // 
      this.btnResetToDefaults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnResetToDefaults.Location = new System.Drawing.Point(195, 337);
      this.btnResetToDefaults.Name = "btnResetToDefaults";
      this.btnResetToDefaults.Size = new System.Drawing.Size(145, 50);
      this.btnResetToDefaults.TabIndex = 11;
      this.btnResetToDefaults.Text = "Reset to Defaults";
      this.btnResetToDefaults.UseVisualStyleBackColor = true;
      this.btnResetToDefaults.Click += new System.EventHandler(this.BtnResetToDefaults_Click);
      // 
      // btnChangeBaud
      // 
      this.btnChangeBaud.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnChangeBaud.Location = new System.Drawing.Point(265, 196);
      this.btnChangeBaud.Name = "btnChangeBaud";
      this.btnChangeBaud.Size = new System.Drawing.Size(75, 41);
      this.btnChangeBaud.TabIndex = 11;
      this.btnChangeBaud.Text = "Change";
      this.btnChangeBaud.UseVisualStyleBackColor = true;
      this.btnChangeBaud.Click += new System.EventHandler(this.btnChangeBaud_Click);
      // 
      // cbBaudRates
      // 
      this.cbBaudRates.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.cbBaudRates.FormattingEnabled = true;
      this.cbBaudRates.Location = new System.Drawing.Point(123, 205);
      this.cbBaudRates.Name = "cbBaudRates";
      this.cbBaudRates.Size = new System.Drawing.Size(121, 21);
      this.cbBaudRates.TabIndex = 10;
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(14, 205);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(103, 23);
      this.label4.TabIndex = 9;
      this.label4.Text = "New Baud:";
      this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(13, 48);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(61, 27);
      this.label2.TabIndex = 10;
      this.label2.Text = "Version:";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // cbVersion
      // 
      this.cbVersion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.cbVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbVersion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.cbVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cbVersion.FormattingEnabled = true;
      this.cbVersion.Location = new System.Drawing.Point(79, 50);
      this.cbVersion.Name = "cbVersion";
      this.cbVersion.Size = new System.Drawing.Size(175, 25);
      this.cbVersion.TabIndex = 6;
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(12, 20);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(61, 27);
      this.label1.TabIndex = 8;
      this.label1.Text = "Port:";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // cbDebug
      // 
      this.cbDebug.AutoSize = true;
      this.cbDebug.Location = new System.Drawing.Point(220, 57);
      this.cbDebug.Name = "cbDebug";
      this.cbDebug.Size = new System.Drawing.Size(58, 17);
      this.cbDebug.TabIndex = 18;
      this.cbDebug.Text = "Debug";
      this.cbDebug.UseVisualStyleBackColor = true;
      // 
      // ucHelpHover1
      // 
      this.ucHelpHover1.BackColor = System.Drawing.Color.Transparent;
      this.ucHelpHover1.Location = new System.Drawing.Point(197, 54);
      this.ucHelpHover1.Margin = new System.Windows.Forms.Padding(0);
      this.ucHelpHover1.Name = "ucHelpHover1";
      this.ucHelpHover1.SetHelpText = "Will display debug information in the window. This will slow performance so do no" +
    "t use in production. Only use this to identify debug issues.";
      this.ucHelpHover1.Size = new System.Drawing.Size(20, 20);
      this.ucHelpHover1.TabIndex = 19;
      // 
      // FormConfig
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.AutoSize = true;
      this.BackColor = System.Drawing.Color.White;
      this.ClientSize = new System.Drawing.Size(1061, 646);
      this.Controls.Add(this.ucTabControl1);
      this.Controls.Add(this.panel1);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormConfig";
      this.Text = "Configuration";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
      this.groupBox5.ResumeLayout(false);
      this.groupBox5.PerformLayout();
      this.groupBox9.ResumeLayout(false);
      this.groupBox9.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.ucTabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.tabPage2.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox7.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.ComboBox cbTestServoPort;
    private System.Windows.Forms.ComboBox cbChangeIDNew;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button btnLEDOff;
    private System.Windows.Forms.Button btnLEDOn;
    private System.Windows.Forms.Button btnChangeID;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.GroupBox groupBox5;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.TextBox tbBaudRate;
    private System.Windows.Forms.Label label7;
    private ARC.UCForms.UC.UCHelpHover ucHelpHover2;
    private System.Windows.Forms.Panel panel1;
    private UCTabControl ucTabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.ComboBox cbVersion;
    private System.Windows.Forms.Button button4;
    private System.Windows.Forms.GroupBox groupBox7;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnChangeBaud;
    private System.Windows.Forms.ComboBox cbBaudRates;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.GroupBox groupBox9;
    private System.Windows.Forms.RadioButton rbDigitalPort;
    private System.Windows.Forms.RadioButton rbUARTPort1;
    private ARC.UCForms.UC.UCPortButton ucPortButton1;
    private ARC.UCForms.UC.UCHelpHover ucHelpHover4;
    private ARC.UCForms.UC.UCHelpHover ucHelpHover3;
    private System.Windows.Forms.Button button6;
    private System.Windows.Forms.TextBox tbLog;
    private System.Windows.Forms.Button button8;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button btnScanAndFind;
    private ARC.UCForms.UC.UCHelpHover ucHelpHover5;
    private System.Windows.Forms.RadioButton rbUARTPort0;
    private System.Windows.Forms.Button btnResetToDefaults;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Button button2;
    private ARC.UCForms.UC.UCHelpHover ucHelpHover6;
    private System.Windows.Forms.RadioButton rbUARTPort2;
    private System.Windows.Forms.CheckBox cbDebug;
    private ARC.UCForms.UC.UCHelpHover ucHelpHover1;
  }
}