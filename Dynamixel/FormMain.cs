using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EZ_B;
using EZ_Builder;

namespace Dynamixel {

  public partial class FormMain : EZ_Builder.UCForms.FormPluginMaster {

    Servo_AX12 _servo_AX12 = new Servo_AX12();
    Servo_XL_320 _servoXL_320 = new Servo_XL_320();
    Servo_xl430_w250_t _servoXL430_w250_t = new Servo_xl430_w250_t();

    // This is a duplicate of the _cf.CustomObjectv2 for performance
    ConfigServos _servoConfig = new ConfigServos();

    public FormMain() {

      InitializeComponent();
    }

    void displayVersion() {

      try {

        string xmlFilename = Common.CombinePath(Constants.PLUGINS_FOLDER, _cf._pluginGUID, "plugin.xml");

        EZ_Builder.Config.Sub.PluginV1XML xml = (EZ_Builder.Config.Sub.PluginV1XML)Common.DeserializeObjectFile(xmlFilename, typeof(EZ_Builder.Config.Sub.PluginV1XML));

        Invokers.SetAppendText(tbLog, true, "Version: {0}", xml.VersionRelease);
      } catch {

        Invokers.SetAppendText(tbLog, true, "Unable to read plugin.xml");
      }
    }

    private void Form1_Load(object sender, EventArgs e) {

      displayVersion();

      // Bind to the events for moving a servo and changing connection state
      EZBManager.EZBs[0].OnConnectionChange += FormMain_OnConnectionChange;
      EZBManager.EZBs[0].Servo.OnServoMove += Servo_OnServoMove;
      EZBManager.EZBs[0].Servo.OnServoRelease += Servo_OnServoRelease;
      EZBManager.EZBs[0].Servo.OnServoSpeed += Servo_OnServoSpeed;
      EZBManager.EZBs[0].Servo.OnServoGetPosition += Servo_OnServoGetPosition;

      ExpressionEvaluation.FunctionEval.AdditionalFunctionEvent += FunctionEval_AdditionalFunctionEvent;

      Invokers.SetAppendText(tbLog, true, "Connected Events");

      if (EZBManager.EZBs[0].IsConnected) {

        initUART();

        disableAllFeedback();
      }
    }

    void FormMain_OnConnectionChange(bool isConnected) {

      // If the connection is established, send an initialization to the ez-b for the uart which we will be using
      if (isConnected) {

        if (_cf.STORAGE[ConfigTitles.GLOBAL_SERVO_POSITIONS] != null)
          EZ_B.Servo.SERVO_MAX = Convert.ToInt32(_cf.STORAGE[ConfigTitles.GLOBAL_SERVO_POSITIONS]);

        initUART();

        disableAllFeedback();
      }
    }

    private void FormMain_FormClosing(object sender, FormClosingEventArgs e) {

      EZBManager.EZBs[0].OnConnectionChange -= FormMain_OnConnectionChange;
      EZBManager.EZBs[0].Servo.OnServoMove -= Servo_OnServoMove;
      EZBManager.EZBs[0].Servo.OnServoRelease -= Servo_OnServoRelease;
      EZBManager.EZBs[0].Servo.OnServoSpeed -= Servo_OnServoSpeed;
      EZBManager.EZBs[0].Servo.OnServoGetPosition -= Servo_OnServoGetPosition;
      ExpressionEvaluation.FunctionEval.AdditionalFunctionEvent -= FunctionEval_AdditionalFunctionEvent;
    }

    int getUARTIndex() {

      if (((ConfigTitles.PortTypeEnum)_cf.STORAGE[ConfigTitles.PORT_TYPE]) == ConfigTitles.PortTypeEnum.UART0)
        return 0;
      else
        return 1;
    }

    private void initUART() {

      if (((ConfigTitles.PortTypeEnum)_cf.STORAGE[ConfigTitles.PORT_TYPE]) == ConfigTitles.PortTypeEnum.DigitalPort)
        return;

      if (EZBManager.EZBs[0].IsConnected) {

        UInt32 baud = Convert.ToUInt32(_cf.STORAGE[ConfigTitles.BAUD_RATE]);

        Invokers.SetAppendText(tbLog, true, "Init UART #{0} @ {1}bps", getUARTIndex(), baud);

        EZBManager.EZBs[0].Uart.UARTExpansionInit(getUARTIndex(), baud);
      }
    }

    public override void SetConfiguration(EZ_Builder.Config.Sub.PluginV1 cf) {

      cf.STORAGE.AddIfNotExist(ConfigTitles.BAUD_RATE, 1000000);

      cf.STORAGE.AddIfNotExist(ConfigTitles.GLOBAL_SERVO_POSITIONS, EZ_B.Servo.SERVO_MAX);

      cf.STORAGE.AddIfNotExist(ConfigTitles.PORT_TYPE, ConfigTitles.PortTypeEnum.UART1);

      cf.STORAGE.AddIfNotExist(ConfigTitles.DIGITAL_PORT, Digital.DigitalPortEnum.D0);

      EZ_B.Servo.SERVO_MAX = Convert.ToInt32(cf.STORAGE[ConfigTitles.GLOBAL_SERVO_POSITIONS]);

      // If the servo config is empty, assign a blank one
      if (cf._customObjectEncodedV2.Length == 0)
        cf.SetCustomObjectV2(_servoConfig);

      _servoConfig = (ConfigServos)cf.GetCustomObjectV2(typeof(ConfigServos));

      base.SetConfiguration(cf);
    }

    public override EZ_Builder.Config.Sub.PluginV1 GetConfiguration() {

      _cf.SetCustomObjectV2(_servoConfig);

      return base.GetConfiguration();
    }

    void serialWrite(byte[] data) {

      if (((ConfigTitles.PortTypeEnum)_cf.STORAGE[ConfigTitles.PORT_TYPE]) == ConfigTitles.PortTypeEnum.DigitalPort) {

        var baudRate = (Uart.BAUD_RATE_ENUM)Enum.Parse(typeof(Uart.BAUD_RATE_ENUM), "Baud_" + _cf.STORAGE[ConfigTitles.BAUD_RATE].ToString(), true);

        EZBManager.EZBs[0].Uart.SendSerial((Digital.DigitalPortEnum)_cf.STORAGE[ConfigTitles.DIGITAL_PORT], baudRate, data);
      } else {

        EZBManager.EZBs[0].Uart.UARTExpansionWrite(getUARTIndex(), data);
      }
    }

    public override void SendCommand(string windowCommand, params string[] values) {

      if (windowCommand.Equals("setled", StringComparison.InvariantCultureIgnoreCase)) {

        if (values.Length != 2)
          throw new Exception(string.Format("Expecting 2 parameters, you passed {0}", values.Length));

        Servo.ServoPortEnum servoPort = new EZ_Builder.Scripting.HelperPortParser(values[0]).ServoPort;

        if (servoPort < EZ_B.Servo.ServoPortEnum.V1 || servoPort > EZ_B.Servo.ServoPortEnum.V99)
          throw new Exception("Only virtual servos are supported for dynamixel. Virtual servos start with a 'v', such as v1, v2, v3..");

        bool status = EZ_Builder.Scripting.Helper.GetTrueOrFalse(values[1]) == EZ_Builder.Scripting.Helper.TrueFalseEnum.True;

        var servoConfig = _servoConfig.GetPort(servoPort);

        if (servoConfig == null)
          throw new Exception(string.Format("Virtual Servo {0} is not configured for dynamixel usage", servoPort));

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
          serialWrite(_servo_AX12.LED(Utility.GetIdFromServo(servoPort), status));
        else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
          serialWrite(_servoXL430_w250_t.LED(Utility.GetIdFromServo(servoPort), status));
        else
          serialWrite(_servoXL_320.LED(Utility.GetIdFromServo(servoPort), status));
      } else if (windowCommand.Equals("TorqueEnable", StringComparison.InvariantCultureIgnoreCase)) {

        if (values.Length != 2)
          throw new Exception(string.Format("Expecting 2 parameters, you passed {0}", values.Length));

        Servo.ServoPortEnum servoPort = new EZ_Builder.Scripting.HelperPortParser(values[0]).ServoPort;

        bool status = EZ_Builder.Scripting.Helper.GetTrueOrFalse(values[1]) == EZ_Builder.Scripting.Helper.TrueFalseEnum.True;

        if (servoPort < EZ_B.Servo.ServoPortEnum.V1 || servoPort > EZ_B.Servo.ServoPortEnum.V99)
          throw new Exception("Only virtual servos are supported for dynamixel. Virtual servos start with a 'v', such as v1, v2, v3..");

        var servoConfig = _servoConfig.GetPort(servoPort);

        if (servoConfig == null)
          throw new Exception(string.Format("Virtual Servo {0} is not configured for dynamixel usage", servoPort));

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.XL_320)
          serialWrite(_servoXL430_w250_t.TorqueEnable(Utility.GetIdFromServo(servoPort), status));
      } else {

        base.SendCommand(windowCommand, values);
      }
    }

    public override object[] GetSupportedControlCommands() {

      List<string> cmds = new List<string>();

      cmds.Add("SetLED, Port, true|false");
      cmds.Add("TorqueEnable, Port, true|false");

      return cmds.ToArray();
    }

    private void Servo_OnServoGetPosition(Servo.ServoPortEnum servoPort, EZ_B.Classes.GetServoValueResponse getServoResponse) {

      if (getServoResponse.Success)
        return;

      if (!EZBManager.EZBs[0].IsConnected) {

        getServoResponse.Success = false;
        getServoResponse.ErrorStr = "Not connected to EZ-B";

        return;
      }

      var resp = getServoPosition(servoPort);

      getServoResponse.Success = resp.Success;
      getServoResponse.Position = resp.Position;
      getServoResponse.ErrorStr = resp.ErrorStr;
    }

    void Servo_OnServoRelease(Servo.ServoPortEnum[] servos) {

      List<byte> cmdData = new List<byte>();

      foreach (var servoPort in servos) {

        if (servoPort < EZ_B.Servo.ServoPortEnum.V1 || servoPort > EZ_B.Servo.ServoPortEnum.V99)
          continue;

        var servoConfig = _servoConfig.GetPort(servoPort);

        if (servoConfig == null)
          continue;

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
          cmdData.AddRange(_servo_AX12.ReleaseServo(Utility.GetIdFromServo(servoConfig.Port)));
        else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
          cmdData.AddRange(_servoXL430_w250_t.ReleaseServo(Utility.GetIdFromServo(servoConfig.Port)));
        else
          cmdData.AddRange(_servoXL_320.ReleaseServo(Utility.GetIdFromServo(servoConfig.Port)));
      }

      if (cmdData.Count != 0)
        serialWrite(cmdData.ToArray());
    }

    void Servo_OnServoSpeed(EZ_B.Classes.ServoSpeedItem[] servos) {

      List<byte> cmdData = new List<byte>();

      foreach (var servo in servos) {

        if (servo.Port < EZ_B.Servo.ServoPortEnum.V1 || servo.Port > EZ_B.Servo.ServoPortEnum.V99)
          continue;

        var servoConfig = _servoConfig.GetPort(servo.Port);

        if (servoConfig == null)
          continue;

        int speed = servo.Speed * 51;

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
          cmdData.AddRange(_servo_AX12.ServoSpeed(Utility.GetIdFromServo(servo.Port), speed));
        else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
          cmdData.AddRange(_servoXL430_w250_t.ServoSpeed(Utility.GetIdFromServo(servo.Port), speed));
        else
          cmdData.AddRange(_servoXL_320.ServoSpeed(Utility.GetIdFromServo(servo.Port), speed));
      }

      if (cmdData.Count != 0)
        serialWrite(cmdData.ToArray());
    }

    void Servo_OnServoMove(EZ_B.Classes.ServoPositionItem[] servos) {

      List<byte> cmdData = new List<byte>();

      foreach (var servo in servos) {

        if (servo.Port < EZ_B.Servo.ServoPortEnum.V1 || servo.Port > EZ_B.Servo.ServoPortEnum.V99)
          continue;

        var servoConfig = _servoConfig.GetPort(servo.Port);

        if (servoConfig == null)
          continue;

        int position = (int)EZ_B.Functions.RemapScalar(servo.Position, Servo.SERVO_MIN, Servo.SERVO_MAX, 0, servoConfig.MaxPosition - 1);

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12) {

          cmdData.AddRange(_servo_AX12.MoveServoCmd(Utility.GetIdFromServo(servo.Port), position));
        } else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t) {

          if (EZBManager.EZBs[0].Servo.IsServoReleased(servo.Port))
            serialWrite(_servoXL430_w250_t.TorqueEnable(Utility.GetIdFromServo(servo.Port), true));

          cmdData.AddRange(_servoXL430_w250_t.MoveServoCmd(Utility.GetIdFromServo(servo.Port), position));
        } else {

          cmdData.AddRange(_servoXL_320.MoveServoCmd(Utility.GetIdFromServo(servo.Port), position));
        }
      }

      if (cmdData.Count != 0)
        serialWrite(cmdData.ToArray());
    }

    Servo.ServoPortEnum getCmdParameterPort(object[] parameters) {

      if (parameters.Length != 1)
        throw new Exception("Requires only 1 parameter, which is the virtual servo (i.e. V1)");

      var port = (Servo.ServoPortEnum)Enum.Parse(typeof(EZ_B.Servo.ServoPortEnum), parameters[0].ToString(), true);

      if (port < Servo.ServoPortEnum.V1 || port > Servo.ServoPortEnum.V99)
        throw new Exception("Servo must be a Vxx servo between V1 and V99");

      return port;
    }

    private void FunctionEval_AdditionalFunctionEvent(object sender, ExpressionEvaluation.AdditionalFunctionEventArgs e) {

      if (e.Name.Equals("GetDynamixelTemp", StringComparison.InvariantCultureIgnoreCase)) {

        var port = getCmdParameterPort(e.Parameters);

        e.ReturnValue = getServoTemp(port);
      } else if (e.Name.Equals("GetDynamixelLoadDir", StringComparison.InvariantCultureIgnoreCase)) {

        var port = getCmdParameterPort(e.Parameters);

        e.ReturnValue = getLoad(port).LoadDirection;
      } else if (e.Name.Equals("GetDynamixelLoad", StringComparison.InvariantCultureIgnoreCase)) {

        var port = getCmdParameterPort(e.Parameters);

        e.ReturnValue = getLoad(port).Load;
      } else if (e.Name.Equals("GetDynamixelPing", StringComparison.InvariantCultureIgnoreCase)) {

        var port = getCmdParameterPort(e.Parameters);

        e.ReturnValue = getServoPing(port);
      } else if (e.Name.Equals("GetDynamixelPosition", StringComparison.InvariantCultureIgnoreCase)) {

        var port = getCmdParameterPort(e.Parameters);

        var resp = getServoPosition(port);

        if (!resp.Success)
          throw new Exception("Servo did not respond");

        e.ReturnValue = resp.Position;
      }
    }

    private void ucConfigurationButton1_Click(object sender, EventArgs e) {

      using (FormDynamixelConfig form = new FormDynamixelConfig()) {

        form.SetConfiguration(_cf);

        if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK) {

          _cf = form.GetConfiguration();

          initUART();

          _servoConfig = (ConfigServos)_cf.GetCustomObjectV2(typeof(ConfigServos));

          disableAllFeedback();

          EZ_B.Servo.SERVO_MAX = Convert.ToInt32(_cf.STORAGE[ConfigTitles.GLOBAL_SERVO_POSITIONS]);
        }
      }
    }

    void disableAllFeedback() {

      if (!EZBManager.EZBs[0].IsConnected)
        return;

      Invokers.SetAppendText(tbLog, true, "Disable Status Packet");

      List<byte> cmdData = new List<byte>();

      serialWrite(_servo_AX12.DisableStatusPacketForAll());

      serialWrite(_servoXL_320.DisableStatusPacketForAll());

      serialWrite(_servoXL430_w250_t.DisableStatusPacketForAll());
    }

    private void btnForceInit_Click(object sender, EventArgs e) {

      if (!EZBManager.EZBs[0].IsConnected) {

        MessageBox.Show("You must be connected to an EZ-B to send the initialization.");

        return;
      }

      try {

        initUART();

        disableAllFeedback();
      } catch (Exception ex) {

        MessageBox.Show("Dynamixel Error: " + ex.Message);
      }
    }

    EZ_B.Classes.GetServoValueResponse getServoPosition(Servo.ServoPortEnum servo) {

      var resp = new EZ_B.Classes.GetServoValueResponse();

      if (((ConfigTitles.PortTypeEnum)_cf.STORAGE[ConfigTitles.PORT_TYPE]) == ConfigTitles.PortTypeEnum.DigitalPort) {

        resp.ErrorStr = "This feature is only available when using the hardware uart";
        resp.Success = false;

        return resp;
      }

      var servoConfig = _servoConfig.GetPort(servo);

      System.Diagnostics.Debug.WriteLine("requesting for " + servo);

      if (servoConfig == null) {

        System.Diagnostics.Debug.WriteLine("doesnt exist " + servo);

        resp.ErrorStr = "Not the correct servo";
        resp.Success = false;

        return resp;
      }

      var id = Utility.GetIdFromServo(servo);

      Invokers.SetAppendText(tbLog, true, "Reading load from {0}", servo);

      initUART();

      if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12) {

        serialWrite(_servo_AX12.GetCurrentPositionCmd(id));

        System.Threading.Thread.Sleep(100);

        var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

        if (ret.Length != 16) {

          resp.ErrorStr = "Servo did not respond";
          resp.Success = false;

          return resp;
        }

        resp.Position = (int)EZ_B.Functions.RemapScalar(BitConverter.ToUInt16(ret, 13), 1, servoConfig.MaxPosition, Servo.SERVO_MIN, Servo.SERVO_MAX);
        resp.Success = true;
      } else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t) {

        serialWrite(_servoXL430_w250_t.GetCurrentPositionCmd(id));

        System.Threading.Thread.Sleep(100);

        var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

        if (ret.Length != 29) {

          resp.ErrorStr = "Servo did not respond";
          resp.Success = false;

          return resp;
        }

        resp.Position = (int)EZ_B.Functions.RemapScalar(BitConverter.ToUInt16(ret, 23), 1, servoConfig.MaxPosition, Servo.SERVO_MIN, Servo.SERVO_MAX);
        resp.Success = true;
      } else {

        serialWrite(_servoXL_320.GetCurrentPositionCmd(id));

        System.Threading.Thread.Sleep(100);

        var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

        if (ret.Length != 27) {

          resp.ErrorStr = "Servo did not respond";
          resp.Success = false;

          return resp;
        }

        int tmpPost = ret[22] << 8 | ret[23];

        resp.Position = (int)EZ_B.Functions.RemapScalar(BitConverter.ToUInt16(ret, 23), 1, servoConfig.MaxPosition, Servo.SERVO_MIN, Servo.SERVO_MAX);
        resp.Success = true;
      }

      return resp;
    }

    bool getServoPing(Servo.ServoPortEnum servo) {

      if (((ConfigTitles.PortTypeEnum)_cf.STORAGE[ConfigTitles.PORT_TYPE]) == ConfigTitles.PortTypeEnum.DigitalPort)
        throw new Exception("This feature is only available when using the hardware uart");

      var servoConfig = _servoConfig.GetPort(servo);

      if (servoConfig == null)
        throw new Exception("No servo configured for this ID");

      var id = Utility.GetIdFromServo(servo);

      Invokers.SetAppendText(tbLog, true, "Reading load from {0}", servo);

      initUART();

      if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12) {

        serialWrite(_servo_AX12.SendPing(id));

        System.Threading.Thread.Sleep(500);

        var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

        if (ret.Length < 12)
          return false;
        else if (ret.Length > 12)
          throw new Exception("More than one servo responded");
      } else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t) {

        serialWrite(_servoXL430_w250_t.SendPing(id));

        System.Threading.Thread.Sleep(500);

        var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

        if (ret.Length < 24)
          return false;
        else if (ret.Length > 24)
          throw new Exception("More than one servo responded");
      } else {

        serialWrite(_servoXL_320.SendPing(id));

        System.Threading.Thread.Sleep(500);

        var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

        if (ret.Length < 24)
          return false;
        else if (ret.Length > 24)
          throw new Exception("More than one servo responded");
      }

      return true;
    }

    int getServoTemp(Servo.ServoPortEnum servo) {

      if (((ConfigTitles.PortTypeEnum)_cf.STORAGE[ConfigTitles.PORT_TYPE]) == ConfigTitles.PortTypeEnum.DigitalPort)
        throw new Exception("This feature is only available when using the hardware uart");

      var servoConfig = _servoConfig.GetPort(servo);

      if (servoConfig == null)
        throw new Exception("No servo configured for this ID");

      var id = Utility.GetIdFromServo(servo);

      Invokers.SetAppendText(tbLog, true, "Reading load from {0}", servo);

      initUART();

      if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
        serialWrite(_servo_AX12.GetTemp(id));
      else
        throw new Exception("Not supported for that servo yet");

      System.Threading.Thread.Sleep(500);

      var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

      if (ret.Length != 15)
        throw new Exception("Servo did not respond");

      return ret[13];
    }

    GetLoadResponseCls getLoad(Servo.ServoPortEnum servo) {

      if (((ConfigTitles.PortTypeEnum)_cf.STORAGE[ConfigTitles.PORT_TYPE]) == ConfigTitles.PortTypeEnum.DigitalPort)
        throw new Exception("This feature is only available when using the hardware uart");

      var servoConfig = _servoConfig.GetPort(servo);

      if (servoConfig == null)
        throw new Exception("No servo configured for this ID");

      var id = Utility.GetIdFromServo(servo);

      Invokers.SetAppendText(tbLog, true, "Reading load from {0}", servo);

      initUART();

      if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
        serialWrite(_servo_AX12.GetLoad(id));
      else
        throw new Exception("Not supported for that servo yet");

      System.Threading.Thread.Sleep(1000);

      var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

      if (ret.Length != 16)
        throw new Exception("Servo did not respond");

      var pos = BitConverter.ToUInt16(ret, 13);

      bool dir = Functions.IsBitSet(pos, 10);

      int load = pos & 0x1ff;

      var resp = new GetLoadResponseCls();
      resp.Load = load;
      resp.LoadDirection = dir ? GetLoadResponseCls.LoadDirectionEnum.Clockwise : GetLoadResponseCls.LoadDirectionEnum.CounterClockwise;

      return resp;
    }
  }
}
