using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EZ_B;
using ARC;
using ARC.Scripting.EZScript;

namespace Dynamixel {

  public partial class FormMain : ARC.UCForms.FormPluginMaster {

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

        ARC.Config.Sub.PluginV1XML xml = (ARC.Config.Sub.PluginV1XML)Common.DeserializeObjectFile(xmlFilename, typeof(ARC.Config.Sub.PluginV1XML));

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
      EZBManager.EZBs[0].Servo.OnServoAcceleration += Servo_OnServoAcceleration;
      EZBManager.EZBs[0].Servo.OnServoVelocity += Servo_OnServoVelocity;

      ExpressionEvaluation.FunctionEval.AdditionalFunctionEvent += FunctionEval_AdditionalFunctionEvent;

      Invokers.SetAppendText(tbLog, true, "Connected Events");

      if (EZBManager.EZBs[0].IsConnected)
        initUART(false);
    }

    void FormMain_OnConnectionChange(bool isConnected) {

      initUART(false);
    }

    private void FormMain_FormClosing(object sender, FormClosingEventArgs e) {

      EZBManager.EZBs[0].OnConnectionChange -= FormMain_OnConnectionChange;
      EZBManager.EZBs[0].Servo.OnServoMove -= Servo_OnServoMove;
      EZBManager.EZBs[0].Servo.OnServoRelease -= Servo_OnServoRelease;
      EZBManager.EZBs[0].Servo.OnServoSpeed -= Servo_OnServoSpeed;
      EZBManager.EZBs[0].Servo.OnServoGetPosition -= Servo_OnServoGetPosition;
      EZBManager.EZBs[0].Servo.OnServoAcceleration -= Servo_OnServoAcceleration;
      EZBManager.EZBs[0].Servo.OnServoVelocity -= Servo_OnServoVelocity;
      ExpressionEvaluation.FunctionEval.AdditionalFunctionEvent -= FunctionEval_AdditionalFunctionEvent;
    }

    EZ_B.Digital.DigitalPortEnum getDigitalPortIndex() {

      return (Digital.DigitalPortEnum)_cf.STORAGE[ConfigTitles.DIGITAL_PORT];
    }

    int getUARTIndex() {

      var port = (ConfigTitles.PortTypeEnum)_cf.STORAGE[ConfigTitles.PORT_TYPE];

      if (port == ConfigTitles.PortTypeEnum.UART0)
        return 0;
      else if (port == ConfigTitles.PortTypeEnum.UART1)
        return 1;
      else
        return 2;
    }

    private void initUART(bool skipSettings) {

      if (!EZBManager.EZBs[0].IsConnected)
        return;

      UInt32 baud = Convert.ToUInt32(_cf.STORAGE[ConfigTitles.BAUD_RATE]);

      if (((ConfigTitles.PortTypeEnum)_cf.STORAGE[ConfigTitles.PORT_TYPE]) == ConfigTitles.PortTypeEnum.DigitalPort) {

        Invokers.SetAppendText(tbLog, true, "Init Digital #{0} @ {1}bps", getDigitalPortIndex(), baud);
      } else {

        Invokers.SetAppendText(tbLog, true, "Init UART #{0} @ {1}bps", getUARTIndex(), baud);

        EZBManager.EZBs[0].Uart.UARTExpansionInit(getUARTIndex(), baud);
      }

      if (!skipSettings) {

        List<byte> cmdData = new List<byte>();

        Invokers.SetAppendText(tbLog, true, "Disable Status Packet");

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, "Disabling status packet for AX12");

        cmdData.AddRange(_servo_AX12.DisableStatusPacketForAll());

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, "Disabling status packet for XL320");

        cmdData.AddRange(_servoXL_320.DisableStatusPacketForAll());

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, "Disabling status packet for XL430");

        cmdData.AddRange(_servoXL430_w250_t.DisableStatusPacketForAll());

        foreach (var servoConfig in _servoConfig.Servos)
          if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t) {

            if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
              Invokers.SetAppendText(tbLog, true, $"XL430 set operating mode {servoConfig.Port} {servoConfig.OperatingMode}");

            cmdData.AddRange(_servoXL430_w250_t.SetOperatingMode(Utility.GetIdFromServo(servoConfig.Port), servoConfig.OperatingMode));
          } else {

            if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
              Invokers.SetAppendText(tbLog, true, $"Setting operating mode is not implemented for {servoConfig.ServoType} {servoConfig.Port}. Using default mode (Position mode?)");
          }

        if (cmdData.Count != 0)
          serialWrite(cmdData.ToArray());
      }
    }

    public override void SetConfiguration(ARC.Config.Sub.PluginV1 cf) {

      cf.STORAGE.AddIfNotExist(ConfigTitles.BAUD_RATE, 1000000);

      cf.STORAGE.AddIfNotExist(ConfigTitles.PORT_TYPE, ConfigTitles.PortTypeEnum.UART1);

      cf.STORAGE.AddIfNotExist(ConfigTitles.DIGITAL_PORT, Digital.DigitalPortEnum.D0);

      cf.STORAGE.AddIfNotExist(ConfigTitles.DEBUG, false);

      // If the servo config is empty, assign a blank one
      if (cf._customObjectEncodedV2.Length == 0)
        cf.SetCustomObjectV2(_servoConfig);

      _servoConfig = (ConfigServos)cf.GetCustomObjectV2(typeof(ConfigServos));

      base.SetConfiguration(cf);
    }

    public override ARC.Config.Sub.PluginV1 GetConfiguration() {

      _cf.SetCustomObjectV2(_servoConfig);

      return base.GetConfiguration();
    }

    void serialWrite(byte[] data) {

      if (!EZBManager.PrimaryEZB.IsConnected)
        return;

      if (((ConfigTitles.PortTypeEnum)_cf.STORAGE[ConfigTitles.PORT_TYPE]) == ConfigTitles.PortTypeEnum.DigitalPort) {

        var baudRate = (Uart.BAUD_RATE_ENUM)Enum.Parse(typeof(Uart.BAUD_RATE_ENUM), "Baud_" + _cf.STORAGE[ConfigTitles.BAUD_RATE].ToString(), true);

        EZBManager.PrimaryEZB.Uart.SendSerial(getDigitalPortIndex(), baudRate, data);
      } else {

        EZBManager.PrimaryEZB.Uart.UARTExpansionWrite(getUARTIndex(), data);
      }
    }

    public override void SendCommand(string windowCommand, params string[] values) {

      if (windowCommand.Equals("setled", StringComparison.InvariantCultureIgnoreCase)) {

        if (values.Length != 2)
          throw new Exception(string.Format("Expecting 2 parameters, you passed {0}", values.Length));

        Servo.ServoPortEnum servoPort = new HelperPortParser(values[0]).ServoPort;

        if (servoPort < EZ_B.Servo.ServoPortEnum.V1 || servoPort > EZ_B.Servo.ServoPortEnum.V99)
          throw new Exception("Only virtual servos are supported for dynamixel. Virtual servos start with a 'v', such as v1, v2, v3..");

        bool status = Helper.GetTrueOrFalse(values[1]) == Helper.TrueFalseEnum.True;

        var servoConfig = _servoConfig.GetPort(servoPort);

        if (servoConfig == null)
          throw new Exception(string.Format("Virtual Servo {0} is not configured for dynamixel usage", servoPort));

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, $"Setting LED on {servoPort} to {status}");

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
          serialWrite(_servo_AX12.LED(Utility.GetIdFromServo(servoPort), status));
        else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
          serialWrite(_servoXL430_w250_t.LED(Utility.GetIdFromServo(servoPort), status));
        else
          serialWrite(_servoXL_320.LED(Utility.GetIdFromServo(servoPort), status));
      } else if (windowCommand.Equals("TorqueEnable", StringComparison.InvariantCultureIgnoreCase)) {

        if (values.Length != 2)
          throw new Exception(string.Format("Expecting 2 parameters, you passed {0}", values.Length));

        Servo.ServoPortEnum servoPort = new HelperPortParser(values[0]).ServoPort;

        bool status = Helper.GetTrueOrFalse(values[1]) == Helper.TrueFalseEnum.True;

        if (servoPort < EZ_B.Servo.ServoPortEnum.V1 || servoPort > EZ_B.Servo.ServoPortEnum.V99)
          throw new Exception("Only virtual servos are supported for dynamixel. Virtual servos start with a 'v', such as v1, v2, v3..");

        var servoConfig = _servoConfig.GetPort(servoPort);

        if (servoConfig == null)
          throw new Exception(string.Format("Virtual Servo {0} is not configured for dynamixel usage", servoPort));

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, $"Torque Enable {servoPort} to {status}");

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.XL_320) {

          serialWrite(_servoXL430_w250_t.TorqueEnable(Utility.GetIdFromServo(servoPort), status));
        } else {

          if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
            Invokers.SetAppendText(tbLog, true, $"Torque enable command not implemented for {servoConfig.ServoType}");
        }
      } else if (windowCommand.StartsWith("WriteRam", StringComparison.InvariantCultureIgnoreCase)) {

        if (values.Length != 3)
          throw new Exception(string.Format("Expecting 3 parameters, you passed {0}", values.Length));

        var address = Convert.ToInt32(values[1]);

        Servo.ServoPortEnum servoPort = new HelperPortParser(values[0]).ServoPort;

        if (servoPort < EZ_B.Servo.ServoPortEnum.V1 || servoPort > EZ_B.Servo.ServoPortEnum.V99)
          throw new Exception("Only virtual servos are supported for dynamixel. Virtual servos start with a 'v', such as v1, v2, v3..");

        var servoId = Utility.GetIdFromServo(servoPort);

        var servoConfig = _servoConfig.GetPort(servoPort);

        if (servoConfig == null)
          throw new Exception(string.Format("Virtual Servo {0} is not configured for dynamixel usage", servoPort));

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, $"{windowCommand} {servoPort} {servoConfig.ServoType} address {address} value {values[2]}");

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t) {

          if (windowCommand.Equals("WriteRamByte", StringComparison.InvariantCultureIgnoreCase))
            serialWrite(_servoXL430_w250_t.CreateDynamixelCommandByte(servoId, 0x03, address, Convert.ToByte(values[2])));
          else if (windowCommand.Equals("WriteRamUInt16", StringComparison.InvariantCultureIgnoreCase))
            serialWrite(_servoXL430_w250_t.CreateDynamixelCommandUInt16(servoId, 0x03, address, Convert.ToUInt16(values[2])));
          else if (windowCommand.Equals("WriteRamInt32", StringComparison.InvariantCultureIgnoreCase))
            serialWrite(_servoXL430_w250_t.CreateDynamixelCommandInt32(servoId, 0x03, address, Convert.ToInt32(values[2])));
          else
            throw new Exception($"{windowCommand} not supported for {servoConfig.ServoType}");
        } else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12) {

          if (windowCommand.Equals("WriteRamByte", StringComparison.InvariantCultureIgnoreCase))
            serialWrite(_servo_AX12.CreateDynamixelCommandByte(servoId, 0x03, (byte)address, Convert.ToByte(values[2])));
          else if (windowCommand.Equals("WriteRamUInt16", StringComparison.InvariantCultureIgnoreCase))
            serialWrite(_servo_AX12.CreateDynamixelCommandUInt16(servoId, 0x03, (byte)address, Convert.ToUInt16(values[2])));
          else if (windowCommand.Equals("WriteRamInt32", StringComparison.InvariantCultureIgnoreCase))
            serialWrite(_servo_AX12.CreateDynamixelCommandInt32(servoId, 0x03, (byte)address, Convert.ToInt32(values[2])));
          else
            throw new Exception($"{windowCommand} not supported for {servoConfig.ServoType}");
        } else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.XL_320) {

          if (windowCommand.Equals("WriteRamByte", StringComparison.InvariantCultureIgnoreCase))
            serialWrite(_servoXL_320.CreateDynamixelCommandByte(servoId, 0x03, (byte)address, Convert.ToByte(values[2])));
          else
            throw new Exception($"{windowCommand} not supported for {servoConfig.ServoType}");

        } else {

          if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
            Invokers.SetAppendText(tbLog, true, $"Torque enable command not implemented for {servoConfig.ServoType}");
        }
      } else {

        base.SendCommand(windowCommand, values);
      }
    }

    public override object[] GetSupportedControlCommands() {

      List<string> cmds = new List<string>();

      cmds.Add("\"SetLED\", ServoPort, true|false");
      cmds.Add("\"TorqueEnable\", ServoPort, true|false");
      cmds.Add("\"WriteRamByte\", ServoPort, Address, Value");
      cmds.Add("\"WriteRamUInt16\", ServoPort, Address, Value");
      cmds.Add("\"WriteRamInt32\", ServoPort, Address, Value");

      return cmds.ToArray();
    }

    private void Servo_OnServoGetPosition(Servo.ServoPortEnum servoPort, EZ_B.Classes.GetServoValueResponse getServoResponse) {

      if (getServoResponse.Success)
        return;

      if (!EZBManager.EZBs[0].IsConnected) {

        getServoResponse.Success = false;
        getServoResponse.ErrorStr = "Not connected to EZB";

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, $"Not connected to EZB");

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

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, $"Release servo for {servoConfig.ServoType} {servoPort}");

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

        if (servo.Speed == -1) {

          if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
            Invokers.SetAppendText(tbLog, true, $"Set servo speed {servoConfig.ServoType} {servo.Port} to {servo.Speed} (skipping)");
        } else {

          int speed = servo.Speed * 51;

          if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
            Invokers.SetAppendText(tbLog, true, $"Set servo speed {servoConfig.ServoType} {servo.Port} to {servo.Speed} ({speed})");

          if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
            cmdData.AddRange(_servo_AX12.ServoSpeed(Utility.GetIdFromServo(servo.Port), speed));
          else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
            cmdData.AddRange(_servoXL430_w250_t.ServoSpeed(Utility.GetIdFromServo(servo.Port), speed));
          else
            cmdData.AddRange(_servoXL_320.ServoSpeed(Utility.GetIdFromServo(servo.Port), speed));
        }
      }

      if (cmdData.Count != 0)
        serialWrite(cmdData.ToArray());
    }

    private void Servo_OnServoVelocity(EZ_B.Classes.ServoVelocityItem[] servos) {

      List<byte> cmdData = new List<byte>();

      foreach (var servo in servos) {

        if (servo.Port < EZ_B.Servo.ServoPortEnum.V1 || servo.Port > EZ_B.Servo.ServoPortEnum.V99)
          continue;

        var servoConfig = _servoConfig.GetPort(servo.Port);

        if (servoConfig == null)
          continue;

        if (servo.Velocity == -1) {

          if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
            Invokers.SetAppendText(tbLog, true, $"Set servo velocity {servoConfig.ServoType} {servo.Port} to {servo.Velocity} (skipping)");
        } else {

          if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
            Invokers.SetAppendText(tbLog, true, $"Set servo velocity {servoConfig.ServoType} {servo.Port} to {servo.Velocity}");

          if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t) {

            cmdData.AddRange(_servoXL430_w250_t.ServoVelocity(Utility.GetIdFromServo(servo.Port), servo.Velocity));
          } else {

            if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
              Invokers.SetAppendText(tbLog, true, $"{servoConfig.ServoType} does not support Velocity");
          }
        }
      }

      if (cmdData.Count != 0)
        serialWrite(cmdData.ToArray());
    }

    private void Servo_OnServoAcceleration(EZ_B.Classes.ServoAccelerationItem[] servos) {

      List<byte> cmdData = new List<byte>();

      foreach (var servo in servos) {

        if (servo.Port < EZ_B.Servo.ServoPortEnum.V1 || servo.Port > EZ_B.Servo.ServoPortEnum.V99)
          continue;

        var servoConfig = _servoConfig.GetPort(servo.Port);

        if (servoConfig == null)
          continue;

        if (servo.Acceleration == -1) {

          if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
            Invokers.SetAppendText(tbLog, true, $"Set servo acceleration {servoConfig.ServoType} {servo.Port} to {servo.Acceleration} (skipping)");
        } else {

          if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
            Invokers.SetAppendText(tbLog, true, $"Set servo acceleration {servoConfig.ServoType} {servo.Port} to {servo.Acceleration}");

          if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t) {

            cmdData.AddRange(_servoXL430_w250_t.ServoAcceleration(Utility.GetIdFromServo(servo.Port), servo.Acceleration));
          } else {

            if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
              Invokers.SetAppendText(tbLog, true, $"{servoConfig.ServoType} does not support Acceleration");
          }
        }
      }

      if (cmdData.Count != 0)
        serialWrite(cmdData.ToArray());
    }

    void Servo_OnServoMove(EZ_B.Classes.ServoPositionItem[] servos) {

      List<byte> cmdData = new List<byte>();

      foreach (var servo in servos) {

        if (servo.Port < Servo.ServoPortEnum.V1 || servo.Port > Servo.ServoPortEnum.V99)
          continue;

        var servoConfig = _servoConfig.GetPort(servo.Port);

        if (servoConfig == null)
          continue;

        if (servoConfig.OperatingMode == ConfigServos.OperatingModeEnum.Position) {

          if (servo.Speed != -1)
            Servo_OnServoSpeed(
              new EZ_B.Classes.ServoSpeedItem[] {
                new EZ_B.Classes.ServoSpeedItem(servo.Port, servo.Speed)
            });

          if (servo.Velocity != -1)
            Servo_OnServoVelocity(
              new EZ_B.Classes.ServoVelocityItem[] {
                new EZ_B.Classes.ServoVelocityItem(servo.Port, servo.Velocity)
            });

          if (servo.Acceleration != -1)
            Servo_OnServoAcceleration(
              new EZ_B.Classes.ServoAccelerationItem[] {
                new EZ_B.Classes.ServoAccelerationItem(servo.Port, servo.Acceleration)
            });

          int position = (int)EZ_B.Functions.RemapScalar(servo.Position, Servo.SERVO_MIN, Servo.SERVO_MAX, 0, servoConfig.MaxPosition - 1);

          if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
            Invokers.SetAppendText(tbLog, true, $"Set servo position (Position mode) {servoConfig.ServoType} {servo.Port} to {servo.Position} ({position})");

          if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12) {

            cmdData.AddRange(_servo_AX12.MoveServoCmd(Utility.GetIdFromServo(servo.Port), position));
          } else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t) {

            if (EZBManager.EZBs[0].Servo.IsServoReleased(servo.Port)) {

              if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
                Invokers.SetAppendText(tbLog, true, $"Enable torque {servoConfig.ServoType} {servo.Port}");

              cmdData.AddRange(_servoXL430_w250_t.TorqueEnable(Utility.GetIdFromServo(servo.Port), true));
            }

            cmdData.AddRange(_servoXL430_w250_t.MoveServoCmd(Utility.GetIdFromServo(servo.Port), position));
          } else {

            cmdData.AddRange(_servoXL_320.MoveServoCmd(Utility.GetIdFromServo(servo.Port), position));
          }
        } else {

          int velocity = (int)EZ_B.Functions.RemapScalar(servo.Position, Servo.SERVO_MIN, Servo.SERVO_MAX, -134, 134);

          if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
            Invokers.SetAppendText(tbLog, true, $"Set servo velocity (Wheel mode) {servoConfig.ServoType} {servo.Port} to {servo.Position} ({velocity})");

          if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t) {

            if (EZBManager.EZBs[0].Servo.IsServoReleased(servo.Port)) {

              if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
                Invokers.SetAppendText(tbLog, true, $"Enable torque {servoConfig.ServoType} {servo.Port}");

              cmdData.AddRange(_servoXL430_w250_t.TorqueEnable(Utility.GetIdFromServo(servo.Port), true));
            }

            cmdData.AddRange(_servoXL430_w250_t.GoalVelocity(Utility.GetIdFromServo(servo.Port), velocity));
          } else {

            if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
              Invokers.SetAppendText(tbLog, true, $"Wheeled operating mode not implemented for {servoConfig.ServoType} on {servo.Port}");
          }
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

    public override void ConfigPressed() {

      using (FormConfig form = new FormConfig()) {

        form.SetConfiguration(_cf);

        if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK) {

          _cf = form.GetConfiguration();

          _servoConfig = (ConfigServos)_cf.GetCustomObjectV2(typeof(ConfigServos));

          initUART(false);
        }
      }
    }

    private void btnForceInit_Click(object sender, EventArgs e) {

      if (!EZBManager.PrimaryEZB.IsConnected) {

        Invokers.SetAppendText(tbLog, true, "Not connected to EZB");

        return;
      }

      try {

        initUART(false);
      } catch (Exception ex) {

        Invokers.SetAppendText(tbLog, true, ex.Message);
      }
    }

    EZ_B.Classes.GetServoValueResponse getServoPosition(Servo.ServoPortEnum servo) {

      if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
        Invokers.SetAppendText(tbLog, true, $"Get servo position requested for {servo}");

      var resp = new EZ_B.Classes.GetServoValueResponse();

      if (((ConfigTitles.PortTypeEnum)_cf.STORAGE[ConfigTitles.PORT_TYPE]) == ConfigTitles.PortTypeEnum.DigitalPort) {

        resp.ErrorStr = "This feature is only available when using the hardware uart";
        resp.Success = false;

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, resp.ErrorStr);

        return resp;
      }

      var servoConfig = _servoConfig.GetPort(servo);

      if (servoConfig == null) {

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, $"Servo is not configured as a dynamixel: {servo}");

        resp.ErrorStr = "Not the correct servo";
        resp.Success = false;

        return resp;
      }

      var id = Utility.GetIdFromServo(servo);

      // initialize the servo port only to flush the input buffer
      initUART(true);

      if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12) {

        serialWrite(_servo_AX12.GetCurrentPositionCmd(id));

        System.Threading.Thread.Sleep(100);

        var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

        if (ret.Length != 16) {

          resp.ErrorStr = "Servo did not respond";
          resp.Success = false;
        } else {

          resp.Position = (int)EZ_B.Functions.RemapScalar(BitConverter.ToUInt16(ret, 13), 1, servoConfig.MaxPosition, Servo.SERVO_MIN, Servo.SERVO_MAX);
          resp.Success = true;
        }
      } else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t) {

        serialWrite(_servoXL430_w250_t.GetCurrentPositionCmd(id));

        System.Threading.Thread.Sleep(100);

        var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

        if (ret.Length != 29) {

          resp.ErrorStr = "Servo did not respond";
          resp.Success = false;
        } else {

          resp.Position = (int)EZ_B.Functions.RemapScalar(BitConverter.ToUInt16(ret, 23), 1, servoConfig.MaxPosition, Servo.SERVO_MIN, Servo.SERVO_MAX);
          resp.Success = true;
        }
      } else {

        serialWrite(_servoXL_320.GetCurrentPositionCmd(id));

        System.Threading.Thread.Sleep(100);

        var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

        if (ret.Length != 27) {

          resp.ErrorStr = "Servo did not respond";
          resp.Success = false;
        } else {

          int tmpPost = ret[22] << 8 | ret[23];

          resp.Position = (int)EZ_B.Functions.RemapScalar(BitConverter.ToUInt16(ret, 23), 1, servoConfig.MaxPosition, Servo.SERVO_MIN, Servo.SERVO_MAX);
          resp.Success = true;
        }
      }

      if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG])) {

        if (resp.Success)
          Invokers.SetAppendText(tbLog, true, $"Position for {servoConfig.ServoType} is: {resp.Position}");
        else
          Invokers.SetAppendText(tbLog, true, resp.ErrorStr);
      }

      return resp;
    }

    bool getServoPing(Servo.ServoPortEnum servo) {

      if (((ConfigTitles.PortTypeEnum)_cf.STORAGE[ConfigTitles.PORT_TYPE]) == ConfigTitles.PortTypeEnum.DigitalPort)
        throw new Exception("Servo ping is only available when using the hardware uart");

      var servoConfig = _servoConfig.GetPort(servo);

      if (servoConfig == null)
        throw new Exception("No servo configured for this ID to ping");

      var id = Utility.GetIdFromServo(servo);

      if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
        Invokers.SetAppendText(tbLog, true, $"Ping {servoConfig.ServoType} {servo}");
      
      // initialize the servo only to flush the data buffer
      initUART(true);

      if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12) {

        serialWrite(_servo_AX12.SendPing(id));

        System.Threading.Thread.Sleep(500);

        var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

        if (ret.Length < 12)
          return false;
        else if (ret.Length > 12)
          throw new Exception("More than one servo responded on this id");
      } else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t) {

        serialWrite(_servoXL430_w250_t.SendPing(id));

        System.Threading.Thread.Sleep(500);

        var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

        if (ret.Length < 24)
          return false;
        else if (ret.Length > 24)
          throw new Exception("More than one servo responded on this id");
      } else {

        serialWrite(_servoXL_320.SendPing(id));

        System.Threading.Thread.Sleep(500);

        var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

        if (ret.Length < 24)
          return false;
        else if (ret.Length > 24)
          throw new Exception("More than one servo responded on this id");
      }

      return true;
    }

    int getServoTemp(Servo.ServoPortEnum servo) {

      if (((ConfigTitles.PortTypeEnum)_cf.STORAGE[ConfigTitles.PORT_TYPE]) == ConfigTitles.PortTypeEnum.DigitalPort)
        throw new Exception("This feature is only available when using the hardware uart");

      var servoConfig = _servoConfig.GetPort(servo);

      if (servoConfig == null)
        throw new Exception("No servo configured for this ID to get temp");

      var id = Utility.GetIdFromServo(servo);

      if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
        Invokers.SetAppendText(tbLog, true, $"Get temp requested for {servoConfig.ServoType} {servo}");

      // initialize the servo only to flush the data buffer
      initUART(true);

      if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
        serialWrite(_servo_AX12.GetTemp(id));
      else
        throw new Exception($"Get temp is not supported for {servoConfig.ServoType}");

      System.Threading.Thread.Sleep(500);

      var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

      if (ret.Length != 15)
        throw new Exception($"Servo did not respond. Expected 15 bytes got {ret.Length}");

      return ret[13];
    }

    GetLoadResponseCls getLoad(Servo.ServoPortEnum servo) {

      if (((ConfigTitles.PortTypeEnum)_cf.STORAGE[ConfigTitles.PORT_TYPE]) == ConfigTitles.PortTypeEnum.DigitalPort)
        throw new Exception("This feature is only available when using the hardware uart");

      var servoConfig = _servoConfig.GetPort(servo);

      if (servoConfig == null)
        throw new Exception("No servo configured for this ID to get load");

      var id = Utility.GetIdFromServo(servo);

      if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
        Invokers.SetAppendText(tbLog, true, $"Get load requested for {servoConfig.ServoType} {servo}");

      // initialize the servo only to flush the data buffer
      initUART(true);

      if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
        serialWrite(_servo_AX12.GetLoad(id));
      else
        throw new Exception($"Get load not supported for {servoConfig.ServoType}");

      System.Threading.Thread.Sleep(1000);

      var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

      if (ret.Length != 16)
        throw new Exception($"Servo did not respond. Expected 16 bytes got {ret.Length}");

      var pos = BitConverter.ToUInt16(ret, 13);

      bool dir = Functions.IsBitSet(pos, 10);

      int load = pos & 0x1ff;

      var resp = new GetLoadResponseCls();
      resp.Load = load;
      resp.LoadDirection = dir ? GetLoadResponseCls.LoadDirectionEnum.Clockwise : GetLoadResponseCls.LoadDirectionEnum.CounterClockwise;

      if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
        Invokers.SetAppendText(tbLog, true, $"Load response: {resp.Load} / {resp.LoadDirection}");

      return resp;
    }

    private void btnClearLog_Click(object sender, EventArgs e) {

      Invokers.SetText(tbLog, string.Empty);
    }
  }
}
