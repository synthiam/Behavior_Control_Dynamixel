using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ARC;
using ARC.Config.Sub;
using EZ_B;

namespace Dynamixel {

  public partial class FormConfig : Form {

    PluginV1    _cf = new PluginV1();

    Servo_AX12 _servo_AX12;
    Servo_XL_320 _servoXL_320;
    Servo_xl430_w250_t _servoXL430_w250_t;

    public FormConfig() {

      InitializeComponent();

      // ---------------------------------------------

      cbChangeIDNew.BeginUpdate();
      cbTestServoPort.BeginUpdate();

      cbChangeIDNew.Items.Clear();
      cbTestServoPort.Items.Clear();

      for (int x = (int)Servo.ServoPortEnum.V1; x < (int)Servo.ServoPortEnum.V99; x++) {

        Servo.ServoPortEnum item = (Servo.ServoPortEnum)x;

        cbChangeIDNew.Items.Add(item);
        cbTestServoPort.Items.Add(item);
      }

      cbChangeIDNew.EndUpdate();
      cbTestServoPort.EndUpdate();

      // ---------------------------------------------

      cbVersion.BeginUpdate();
      cbVersion.Items.Clear();
      foreach (ConfigServos.ServoTypeEnum v in Enum.GetValues(typeof(ConfigServos.ServoTypeEnum)))
        cbVersion.Items.Add(v);
      cbVersion.EndUpdate();

      // ---------------------------------------------

      cbBaudRates.BeginUpdate();
      cbBaudRates.Items.Clear();
      foreach (Servo_AX12.BAUD_RATES baud in Enum.GetValues(typeof(Servo_AX12.BAUD_RATES)))
        cbBaudRates.Items.Add(baud);
      cbBaudRates.EndUpdate();

      // ---------------------------------------------

      cbBaudRates.SelectedItem = Servo_AX12.BAUD_RATES.BAUD_57600;
      cbChangeIDNew.SelectedItem = Servo.ServoPortEnum.V2;
      cbTestServoPort.SelectedItem = Servo.ServoPortEnum.V1;
      cbVersion.SelectedItem = ConfigServos.ServoTypeEnum.AX_12;

      _servo_AX12 = new Servo_AX12();
      _servo_AX12.OnCommunication += onCommunication;

      _servoXL_320 = new Servo_XL_320();
      _servoXL_320.OnCommunication += onCommunication;

      _servoXL430_w250_t = new Servo_xl430_w250_t();
      _servoXL430_w250_t.OnCommunication += onCommunication;
    }

    private void Form_FormClosing(object sender, FormClosingEventArgs e) {

      _servo_AX12.OnCommunication -= onCommunication;
      _servoXL_320.OnCommunication -= onCommunication;
      _servoXL430_w250_t.OnCommunication -= onCommunication;
    }

    public void SetConfiguration(PluginV1 cf) {

      _cf = cf;

      ConfigServos configServos = (ConfigServos)cf.GetCustomObjectV2(typeof(ConfigServos));

      flowLayoutPanel1.SuspendLayout();

      foreach (EZ_B.Servo.ServoPortEnum port in Enum.GetValues(typeof(EZ_B.Servo.ServoPortEnum)))
        if (port >= Servo.ServoPortEnum.V1 && port <= Servo.ServoPortEnum.V99) {

          UCServoPort ucServoPort = new UCServoPort();
          ucServoPort.Name = port.ToString();
          ucServoPort.SetPort = port;

          ConfigServosDetail configServoDetail = configServos.GetPort(port);

          if (configServoDetail != null) {

            ucServoPort.SetEnabled = true;
            ucServoPort.SetMaxPosition = configServoDetail.MaxPosition;
            ucServoPort.SetVersion = configServoDetail.ServoType;
            ucServoPort.SetOperatingMode = configServoDetail.OperatingMode;
          }

          flowLayoutPanel1.Controls.Add(ucServoPort);
        }

      flowLayoutPanel1.ResumeLayout(true);

      tbBaudRate.Text = cf.STORAGE.GetKey(ConfigTitles.BAUD_RATE, 1000000).ToString();

      var portType = (ConfigTitles.PortTypeEnum)cf.STORAGE.GetKey(ConfigTitles.PORT_TYPE, ConfigTitles.PortTypeEnum.UART0);

      if (portType == ConfigTitles.PortTypeEnum.UART0)
        rbUARTPort0.Checked = true;
      else if (portType == ConfigTitles.PortTypeEnum.UART1)
        rbUARTPort1.Checked = true;
      else if (portType == ConfigTitles.PortTypeEnum.UART2)
        rbUARTPort2.Checked = true;
      else
        rbDigitalPort.Checked = true;

      ucPortButton1.SetConfig((Digital.DigitalPortEnum)cf.STORAGE.GetKey(ConfigTitles.DIGITAL_PORT, Digital.DigitalPortEnum.D0));

      cbDebug.Checked = Convert.ToBoolean(cf.STORAGE[ConfigTitles.DEBUG]);
    }

    public PluginV1 GetConfiguration() {

      _cf.STORAGE.AddOrUpdate(ConfigTitles.BAUD_RATE, Convert.ToUInt32(tbBaudRate.Text));

      if (rbUARTPort0.Checked) {

        _cf.STORAGE.AddOrUpdate(ConfigTitles.PORT_TYPE, ConfigTitles.PortTypeEnum.UART0);
      } else if (rbUARTPort1.Checked) {

        _cf.STORAGE.AddOrUpdate(ConfigTitles.PORT_TYPE, ConfigTitles.PortTypeEnum.UART1);
      } else if (rbUARTPort2.Checked) {

        _cf.STORAGE.AddOrUpdate(ConfigTitles.PORT_TYPE, ConfigTitles.PortTypeEnum.UART2);
      } else {

        _cf.STORAGE.AddOrUpdate(ConfigTitles.PORT_TYPE, ConfigTitles.PortTypeEnum.DigitalPort);
        _cf.STORAGE.AddOrUpdate(ConfigTitles.DIGITAL_PORT, ucPortButton1.PortDigital);
      }

      List<ConfigServosDetail> configServosDetails = new List<ConfigServosDetail>();

      foreach (UCServoPort ucServoPort in flowLayoutPanel1.Controls)
        if (ucServoPort.SetEnabled)
          configServosDetails.Add(new ConfigServosDetail() {
            MaxPosition = ucServoPort.SetMaxPosition,
            Port = ucServoPort.SetPort,
            ServoType = ucServoPort.SetVersion,
            OperatingMode = ucServoPort.SetOperatingMode
          });

      ConfigServos configServos = new ConfigServos();
      configServos.Servos = configServosDetails.ToArray();

      // Release all xl430 servos so the torque is enabled on first move
      // This is because the xl430 doesn't automatically enable torque when goal position is set
      foreach (var servo in configServos.Servos)
        if (servo.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
          EZBManager.EZBs[0].Servo.ReleaseServo(servo.Port);

      _cf.STORAGE[ConfigTitles.DEBUG] = cbDebug.Checked;

      _cf.SetCustomObjectV2(configServos);

      return _cf;
    }

    int getUARTIndex() {

      if (rbUARTPort0.Checked)
        return 0;
      else if (rbUARTPort1.Checked)
        return 1;
      else
        return 2;
    }

    void initUART() {

      EZBManager.EZBs[0].Uart.UARTExpansionInit(getUARTIndex(), Convert.ToUInt32(tbBaudRate.Text));
    }

    private void onCommunication(object sender, OnCommCls e) {

      bool debug = Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]);

      if (((ConfigTitles.PortTypeEnum)_cf.STORAGE[ConfigTitles.PORT_TYPE]) == ConfigTitles.PortTypeEnum.DigitalPort)
        throw new Exception("This feature is only available when using the hardware uart");

      if (debug)
        Invokers.SetAppendText(tbLog, true, $"Performing a request to servo...");

      initUART();

      serialWrite(e.SendBytes);

      System.Threading.Thread.Sleep(500);

      var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

      if (debug)
        Invokers.SetAppendText(tbLog, true, $"Read {ret.Length} bytes.");

      e.ResponseBytes = ret.Skip(e.SendBytes.Length).ToArray();
    }

    void serialWrite(byte[] data) {

      if (rbDigitalPort.Checked) {

        try {

          var baudRate = (Uart.BAUD_RATE_ENUM)Enum.Parse(typeof(Uart.BAUD_RATE_ENUM), "Baud_" + tbBaudRate.Text);

          EZBManager.EZBs[0].Uart.SendSerial(ucPortButton1.PortDigital, baudRate, data);
        } catch {

          MessageBox.Show("Invalid baud rate for low speed tx port. Valid baud rates are 4800, 9600, 19200, 38400, 57600, 115200");
        }
      } else {

        EZBManager.EZBs[0].Uart.UARTExpansionWrite(getUARTIndex(), data);
      }
    }

    private void btnChangeID_Click(object sender, EventArgs e) {

      try {

        if (MessageBox.Show("Are you sure?", "Change ID", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
          return;

        var fromId = Utility.GetIdFromServo((Servo.ServoPortEnum)cbTestServoPort.SelectedItem);
        var toId = Utility.GetIdFromServo((Servo.ServoPortEnum)cbChangeIDNew.SelectedItem);
        var servoVersion = (ConfigServos.ServoTypeEnum)cbVersion.SelectedItem;

        if (servoVersion == ConfigServos.ServoTypeEnum.AX_12)
          serialWrite(_servo_AX12.ChangeID(fromId, toId));
        else if (servoVersion == ConfigServos.ServoTypeEnum.xl430_w250_t)
          serialWrite(_servoXL430_w250_t.SetServoID(fromId, toId));
        else
          serialWrite(_servoXL_320.ChangeID(fromId, toId));

        MessageBox.Show("Done");
      } catch (Exception ex) {

        MessageBox.Show(ex.Message);
      }
    }

    private void btnLEDOn_Click(object sender, EventArgs e) {

      try {

        var id = Utility.GetIdFromServo((Servo.ServoPortEnum)cbTestServoPort.SelectedItem);
        var servoVersion = (ConfigServos.ServoTypeEnum)cbVersion.SelectedItem;

        if (servoVersion == ConfigServos.ServoTypeEnum.AX_12)
          serialWrite(_servo_AX12.SetLED(id, true));
        else if (servoVersion == ConfigServos.ServoTypeEnum.xl430_w250_t)
          serialWrite(_servoXL430_w250_t.SetLED(id, 1));
        else
          serialWrite(_servoXL_320.SetLED(id, true));
      } catch (Exception ex) {

        MessageBox.Show(ex.Message);
      }
    }

    private void btnLEDOff_Click(object sender, EventArgs e) {

      try {

        var id = Utility.GetIdFromServo((Servo.ServoPortEnum)cbTestServoPort.SelectedItem);
        var servoVersion = (ConfigServos.ServoTypeEnum)cbVersion.SelectedItem;

        if (servoVersion == ConfigServos.ServoTypeEnum.AX_12)
          serialWrite(_servo_AX12.SetLED(id, false));
        else if (servoVersion == ConfigServos.ServoTypeEnum.xl430_w250_t)
          serialWrite(_servoXL430_w250_t.SetLED(id, 0));
        else
          serialWrite(_servoXL_320.SetLED(id, false));
      } catch (Exception ex) {

        MessageBox.Show(ex.Message);
      }
    }

    private void button1_Click(object sender, EventArgs e) {

      try {
        if (cbChangeIDNew.SelectedIndex < cbChangeIDNew.Items.Count - 1) {

          cbChangeIDNew.SelectedIndex++;

          cbTestServoPort.SelectedItem = cbChangeIDNew.SelectedItem;
        }
      } catch (Exception ex) {

        MessageBox.Show(ex.Message);
      }
    }

    private void btnSave_Click(object sender, EventArgs e) {

      if (!ARC.Common.IsInteger(tbBaudRate.Text)) {

        MessageBox.Show("Baud rate must be an integer number");

        return;
      }

      if (rbDigitalPort.Checked)
        try {

          Enum.Parse(typeof(Uart.BAUD_RATE_ENUM), "Baud_" + tbBaudRate.Text);

        } catch {

          MessageBox.Show("Valid baudrates for TX low speed serial mode are: 4800, 9600, 19200, 38400, 57600, 115200");

          return;
        }

      DialogResult = System.Windows.Forms.DialogResult.OK;
    }

    private void btnCancel_Click(object sender, EventArgs e) {

      DialogResult = System.Windows.Forms.DialogResult.Cancel;
    }

    private void button4_Click(object sender, EventArgs e) {

      if (!ARC.Common.IsInteger(tbBaudRate.Text)) {

        MessageBox.Show("Baud rate must be an integer number");

        return;
      }

      if (!EZBManager.EZBs[0].IsConnected) {

        MessageBox.Show("You must be connected to an EZ-B");

        return;
      }

      if (!rbDigitalPort.Checked)
        EZBManager.EZBs[0].Uart.UARTExpansionInit(getUARTIndex(), Convert.ToUInt32(tbBaudRate.Text));
    }

    private void btnChangeBaud_Click(object sender, EventArgs e) {

      try {

        if (MessageBox.Show(string.Format("Change baud rate on port {0}?", cbTestServoPort.SelectedItem), "Are you sure?", MessageBoxButtons.YesNo) != DialogResult.Yes)
          return;

        if ((ConfigServos.ServoTypeEnum)cbVersion.SelectedItem == ConfigServos.ServoTypeEnum.AX_12) {

          var baud = (Servo_AX12.BAUD_RATES)cbBaudRates.SelectedItem;
          var id = Utility.GetIdFromServo((Servo.ServoPortEnum)cbTestServoPort.SelectedItem);

          serialWrite(_servo_AX12.SetBaudRate(id, baud));

          MessageBox.Show("The baud rate has chnged. To communicate with this servo, you must change the baud rate in the first page of this configuration menu", "Done");

        } else {

          MessageBox.Show("This function is not implemented for this servo. Please let us know on the synthiam community forum if you wish to use it");
        }
      } catch (Exception ex) {

        MessageBox.Show(ex.Message);
      }
    }

    private void button6_Click(object sender, EventArgs e) {

      try {

        var id = Utility.GetIdFromServo((Servo.ServoPortEnum)cbTestServoPort.SelectedItem);
        var servoVersion = (ConfigServos.ServoTypeEnum)cbVersion.SelectedItem;

        if (servoVersion == ConfigServos.ServoTypeEnum.AX_12)
          serialWrite(_servo_AX12.ReleaseTorque(id));
        else if (servoVersion == ConfigServos.ServoTypeEnum.xl430_w250_t)
          serialWrite(_servoXL430_w250_t.SetTorqueEnable(id, 0));
        else
          serialWrite(_servoXL_320.DisableTorque(id));
      } catch (Exception ex) {

        MessageBox.Show(ex.Message);
      }
    }

    private void button8_Click(object sender, EventArgs e) {

      try {

        if (rbDigitalPort.Checked)
          throw new Exception("This feature is only available when using the hardware uart");

        initUART();

        EZBManager.EZBs[0].Uart.UARTExpansionInit(getUARTIndex(), Convert.ToUInt32(tbBaudRate.Text));

        var id = Utility.GetIdFromServo((Servo.ServoPortEnum)cbTestServoPort.SelectedItem);
        var servoVersion = (ConfigServos.ServoTypeEnum)cbVersion.SelectedItem;

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, $"Ping requested for {servoVersion} {id}");

        bool resp;

        if (servoVersion == ConfigServos.ServoTypeEnum.AX_12)
          resp = _servo_AX12.Ping(id);
        else
          resp = _servoXL430_w250_t.Ping(id);

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, $"Response: {resp}");

        Invokers.SetAppendText(tbLog, true, "{0} Responded to ping", (Servo.ServoPortEnum)cbTestServoPort.SelectedItem);
      } catch (Exception ex) {

        Invokers.SetAppendText(tbLog, true, "{0} {1}", (Servo.ServoPortEnum)cbTestServoPort.SelectedItem, ex.Message);
      }
    }

    private void btnScanAndFind_Click(object sender, EventArgs e) {

      tbLog.Clear();

      try {

        if (rbDigitalPort.Checked)
          throw new Exception("This feature is only available when using the hardware uart");

        Invokers.SetAppendText(tbLog, true, $"Using {Convert.ToUInt32(tbBaudRate.Text)} baud on UART #{getUARTIndex()}");
        {
          Invokers.SetAppendText(tbLog, true, "Scanning for AX-12...");

          EZBManager.EZBs[0].Uart.UARTExpansionInit(getUARTIndex(), Convert.ToUInt32(tbBaudRate.Text));

          serialWrite(_servo_AX12.PingCmd(0xfe));

          System.Threading.Thread.Sleep(2000);

          var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

          for (int cnt = 6; cnt < ret.Length; cnt += 6)
            Invokers.SetAppendText(tbLog, true, "- AX_12 found at ID {0}", ret[cnt + 2]);
        }

        tbLog.AppendText(Environment.NewLine);

        System.Threading.Thread.Sleep(1000);

        {
          Invokers.SetAppendText(tbLog, true, "Scanning for XL430 or XL-320...");

          EZBManager.EZBs[0].Uart.UARTExpansionInit(getUARTIndex(), Convert.ToUInt32(tbBaudRate.Text));

          serialWrite(_servoXL430_w250_t.PingCmd(0xfe));

          System.Threading.Thread.Sleep(2000);

          var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

          for (int cnt = 10; cnt < ret.Length; cnt += 14) {

            byte id = ret[cnt + 4];
            var modelNo = _servoXL430_w250_t.GetModelNumber(id);

            Invokers.SetAppendText(tbLog, true, $"- XL430 or XL-320 found at ID {id} (Model No: {modelNo})");
          }
        }

        tbLog.AppendText(Environment.NewLine);

        Invokers.SetAppendText(tbLog, true, "Scan Completed");
      } catch (Exception ex) {

        Invokers.SetAppendText(tbLog, true, ex.ToString());
      }
    }

    private void BtnResetToDefaults_Click(object sender, EventArgs e) {

      if (MessageBox.Show("This will reset the ID and baud rate and any other settings you have. Are you sure?", "Reset? Hardcore!", MessageBoxButtons.YesNoCancel) != DialogResult.Yes) {

        MessageBox.Show("Smart :)");

        return;
      }

      EZBManager.EZBs[0].Uart.UARTExpansionInit(getUARTIndex(), Convert.ToUInt32(tbBaudRate.Text));

      var id = Utility.GetIdFromServo((Servo.ServoPortEnum)cbTestServoPort.SelectedItem);

      if ((ConfigServos.ServoTypeEnum)cbVersion.SelectedItem == ConfigServos.ServoTypeEnum.AX_12) {

        serialWrite(_servo_AX12.ResetToFactoryDefaults(id));

        MessageBox.Show("Servo restored to factory settings. Baud and ID are now changed to factory default values");
      } else if ((ConfigServos.ServoTypeEnum)cbVersion.SelectedItem == ConfigServos.ServoTypeEnum.xl430_w250_t) {

        serialWrite(_servoXL430_w250_t.ResetToFactoryDefault(id));

        MessageBox.Show("Servo restored to factory settings except the Baudrate and ID have not changed.");
      } else {

        MessageBox.Show("This servo type doesn't support this feature (yet). Ask for it on the Dynamixel robot skill comments.");
      }
    }

    private void Button2_Click(object sender, EventArgs e) {

      try {

        var id = Utility.GetIdFromServo((Servo.ServoPortEnum)cbTestServoPort.SelectedItem);

        if ((ConfigServos.ServoTypeEnum)cbVersion.SelectedItem == ConfigServos.ServoTypeEnum.AX_12) {

          initUART();

          var ret = _servo_AX12.GetCurrentPosition(id);

          Invokers.SetAppendText(
            tbLog,
            true,
            "Servo {0} Raw Position {1}",
            id,
            ret);
        } else if ((ConfigServos.ServoTypeEnum)cbVersion.SelectedItem == ConfigServos.ServoTypeEnum.xl430_w250_t) {

          initUART();

          var ret = _servoXL430_w250_t.GetCurrentPosition(id);

          Invokers.SetAppendText(
            tbLog,
            true,
            "Servo {0} Raw Position {1}",
            id,
            ret);
        }
      } catch (Exception ex) {

        Invokers.SetAppendText(tbLog, true, ex.Message);
      }
    }
  }
}
