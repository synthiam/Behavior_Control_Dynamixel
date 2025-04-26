using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EZ_B;
using ARC;
using ARC.Scripting.EZScript;
using System.Linq;
using ARC.UCForms.Cls;
using System.Threading;

namespace Dynamixel {

  public partial class FormMain : ARC.UCForms.FormPluginMaster {

    Servo_AX12         _servo_AX12;
    Servo_XL_320       _servoXL_320;
    Servo_xl430_w250_t _servoXL430_w250_t;

    // This is a duplicate of the _cf.CustomObjectv2 for performance
    ConfigServos _configServos = new ConfigServos();

    public FormMain() {

      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e) {

      displayVersion();

      _servo_AX12 = new Servo_AX12();
      _servo_AX12.OnCommunication += onCommunication;

      _servoXL_320 = new Servo_XL_320();
      _servoXL_320.OnCommunication += onCommunication;

      _servoXL430_w250_t = new Servo_xl430_w250_t();
      _servoXL430_w250_t.OnCommunication += onCommunication;

      // Bind to the events for moving a servo and changing connection state
      EZBManager.EZBs[0].OnConnectionChange += FormMain_OnConnectionChange;
      EZBManager.EZBs[0].Servo.OnServoMove += Servo_OnServoMove;
      EZBManager.EZBs[0].Servo.OnServoRelease += Servo_OnServoRelease;
      EZBManager.EZBs[0].Servo.OnServoSpeed += Servo_OnServoSpeed;
      EZBManager.EZBs[0].Servo.OnServoGetPosition += Servo_OnServoGetPosition;
      EZBManager.EZBs[0].Servo.OnServoAcceleration += Servo_OnServoAcceleration;
      EZBManager.EZBs[0].Servo.OnServoVelocity += Servo_OnServoVelocity;

      Invokers.SetAppendText(tbLog, true, "Connected Events");
    }

    private void FormMain_FormClosing(object sender, FormClosingEventArgs e) {

      EZBManager.EZBs[0].OnConnectionChange -= FormMain_OnConnectionChange;
      EZBManager.EZBs[0].Servo.OnServoMove -= Servo_OnServoMove;
      EZBManager.EZBs[0].Servo.OnServoRelease -= Servo_OnServoRelease;
      EZBManager.EZBs[0].Servo.OnServoSpeed -= Servo_OnServoSpeed;
      EZBManager.EZBs[0].Servo.OnServoGetPosition -= Servo_OnServoGetPosition;
      EZBManager.EZBs[0].Servo.OnServoAcceleration -= Servo_OnServoAcceleration;
      EZBManager.EZBs[0].Servo.OnServoVelocity -= Servo_OnServoVelocity;

      _servo_AX12.OnCommunication -= onCommunication;
      _servoXL_320.OnCommunication -= onCommunication;
      _servoXL430_w250_t.OnCommunication -= onCommunication;
    }

    private void onCommunication(object sender, OnCommCls e) {

      bool debug = Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]);

      if (((ConfigTitles.PortTypeEnum)_cf.STORAGE[ConfigTitles.PORT_TYPE]) == ConfigTitles.PortTypeEnum.DigitalPort)
        throw new Exception("This feature is only available when using the hardware uart");

      if (debug)
        Invokers.SetAppendText(tbLog, true, $"Performing a request to servo...");

      // initialize the servo only to flush the data buffer, skip sending config data
      initUART(true);

      serialWrite(e.SendBytes);

      System.Threading.Thread.Sleep(500);

      var ret = EZBManager.EZBs[0].Uart.UARTExpansionReadAvailable(getUARTIndex());

      if (debug)
        Invokers.SetAppendText(tbLog, true, $"Read {ret.Length} bytes. Using last {e.SendBytes.Length - ret.Length} bytes.");

      e.ResponseBytes = ret.Skip(e.SendBytes.Length).ToArray();

      if (debug)
        Invokers.SetAppendText(tbLog, true, $"Filtered response: {string.Join(" ", e.ResponseBytes.Select(b => b.ToString("x2")))}");
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

    void FormMain_OnConnectionChange(bool isConnected) {

      initUART(false);
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

        if (_configServos.Servos.Any(x => x.ServoType == ConfigServos.ServoTypeEnum.AX_12)) {

          if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
            Invokers.SetAppendText(tbLog, true, "Disabling status packet for AX12");

          serialWrite(_servo_AX12.SetDisableStatusPacket(0xfe));
        }

        if (_configServos.Servos.Any(x => x.ServoType == ConfigServos.ServoTypeEnum.XL_320)) {

          if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
            Invokers.SetAppendText(tbLog, true, "Disabling status packet for XL320");

          serialWrite(_servoXL_320.SetDisableStatusPacket(0xfe));
        }

        if (_configServos.Servos.Any(x => x.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)) {

          if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
            Invokers.SetAppendText(tbLog, true, "Disabling status packet for XL430_w250_t");

          serialWrite(_servoXL430_w250_t.SetDisableStatusPacket(0xfe, 1));
        }

        foreach (var servoConfig in _configServos.Servos)
          if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t) {

            if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
              Invokers.SetAppendText(tbLog, true, $"XL430 set operating mode {servoConfig.Port} {servoConfig.OperatingMode}");

            serialWrite(_servoXL430_w250_t.SetOperatingMode(Utility.GetIdFromServo(servoConfig.Port), (byte)servoConfig.OperatingMode));
          } else {

            if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
              Invokers.SetAppendText(tbLog, true, $"Setting operating mode is not implemented for {servoConfig.ServoType} {servoConfig.Port}. Request it on the Synthiam Community Forum. Using default mode (Position mode?)");
          }
      }
    }

    public override void SetConfiguration(ARC.Config.Sub.PluginV1 cf) {

      base.SetConfiguration(cf);

      cf.STORAGE.AddIfNotExist(ConfigTitles.BAUD_RATE, 1000000);

      cf.STORAGE.AddIfNotExist(ConfigTitles.PORT_TYPE, ConfigTitles.PortTypeEnum.UART1);

      cf.STORAGE.AddIfNotExist(ConfigTitles.DIGITAL_PORT, Digital.DigitalPortEnum.D0);

      cf.STORAGE.AddIfNotExist(ConfigTitles.DEBUG, false);

      // If the servo config is empty, assign a blank one
      if (cf._customObjectEncodedV2.Length == 0)
        cf.SetCustomObjectV2(_configServos);

      _configServos = (ConfigServos)cf.GetCustomObjectV2(typeof(ConfigServos));

      initUART(false);
    }

    void serialWrite(byte[] data) {

      if (!EZBManager.PrimaryEZB.IsConnected)
        return;

      if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
        Invokers.SetAppendText(tbLog, true, $"Sending: {string.Join(" ", data.Select(b => b.ToString("x2")))}");

      if (((ConfigTitles.PortTypeEnum)_cf.STORAGE[ConfigTitles.PORT_TYPE]) == ConfigTitles.PortTypeEnum.DigitalPort) {

        var baudRate = (Uart.BAUD_RATE_ENUM)Enum.Parse(typeof(Uart.BAUD_RATE_ENUM), "Baud_" + _cf.STORAGE[ConfigTitles.BAUD_RATE].ToString(), true);

        EZBManager.PrimaryEZB.Uart.SendSerial(getDigitalPortIndex(), baudRate, data);
      } else {

        EZBManager.PrimaryEZB.Uart.UARTExpansionWrite(getUARTIndex(), data);
      }

      System.Threading.Thread.Sleep(10);
    }

    public override object SendCommandV2(CancellationToken cancellationToken, string windowCommand, params string[] values) {

      if (windowCommand.Equals("ReadRam", StringComparison.InvariantCultureIgnoreCase)) {

        if (values.Length != 3)
          throw new Exception(string.Format("Expecting 3 parameter, you passed {0}", values.Length));

        Servo.ServoPortEnum port = new HelperPortParser(values[0]).ServoPort;

        if (port < EZ_B.Servo.ServoPortEnum.V1 || port > EZ_B.Servo.ServoPortEnum.V99)
          throw new Exception("Only virtual servos are supported for dynamixel. Virtual servos start with a 'v', such as v1, v2, v3..");

        byte id = Utility.GetIdFromServo(port);

        byte address = Convert.ToByte(values[1]);

        byte bytesToRead = Convert.ToByte(values[2]);

        var servoConfig = _configServos.GetPort(port);

        if (servoConfig == null)
          throw new Exception($"There is no dynamixel servo configured for port {port}");

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
          return _servo_AX12.GetRam(id, address, bytesToRead);
        else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
          return _servoXL430_w250_t.GetRam(id, address, bytesToRead);

        throw new Exception("Not implemented. Ask us on the synthiam community forum to add this if you need it");
      }

      if (windowCommand.Equals("ReadRamByte", StringComparison.InvariantCultureIgnoreCase)) {

        if (values.Length != 2)
          throw new Exception(string.Format("Expecting 2 parameter, you passed {0}", values.Length));

        Servo.ServoPortEnum port = new HelperPortParser(values[0]).ServoPort;

        if (port < EZ_B.Servo.ServoPortEnum.V1 || port > EZ_B.Servo.ServoPortEnum.V99)
          throw new Exception("Only virtual servos are supported for dynamixel. Virtual servos start with a 'v', such as v1, v2, v3..");

        byte id = Utility.GetIdFromServo(port);

        byte address = Convert.ToByte(values[1]);

        var servoConfig = _configServos.GetPort(port);

        if (servoConfig == null)
          throw new Exception($"There is no dynamixel servo configured for port {port}");

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
          return _servo_AX12.GetRam(id, address, 1)[0];
        else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
          return _servoXL430_w250_t.GetRam(id, address, 1)[0];

        throw new Exception("Not implemented. Ask us on the synthiam community forum to add this if you need it");
      }

      if (windowCommand.Equals("ReadRamInt16", StringComparison.InvariantCultureIgnoreCase)) {

        if (values.Length != 2)
          throw new Exception(string.Format("Expecting 2 parameter, you passed {0}", values.Length));

        Servo.ServoPortEnum port = new HelperPortParser(values[0]).ServoPort;

        if (port < EZ_B.Servo.ServoPortEnum.V1 || port > EZ_B.Servo.ServoPortEnum.V99)
          throw new Exception("Only virtual servos are supported for dynamixel. Virtual servos start with a 'v', such as v1, v2, v3..");

        byte id = Utility.GetIdFromServo(port);

        byte address = Convert.ToByte(values[1]);

        var servoConfig = _configServos.GetPort(port);

        if (servoConfig == null)
          throw new Exception($"There is no dynamixel servo configured for port {port}");

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
          return BitConverter.ToInt16(_servo_AX12.GetRam(id, address, 2), 0);
        else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
          return BitConverter.ToInt16(_servoXL430_w250_t.GetRam(id, address, 2), 0);

        throw new Exception("Not implemented. Ask us on the synthiam community forum to add this if you need it");
      }

      if (windowCommand.Equals("ReadRamUInt16", StringComparison.InvariantCultureIgnoreCase)) {

        if (values.Length != 2)
          throw new Exception(string.Format("Expecting 2 parameter, you passed {0}", values.Length));

        Servo.ServoPortEnum port = new HelperPortParser(values[0]).ServoPort;

        if (port < EZ_B.Servo.ServoPortEnum.V1 || port > EZ_B.Servo.ServoPortEnum.V99)
          throw new Exception("Only virtual servos are supported for dynamixel. Virtual servos start with a 'v', such as v1, v2, v3..");

        byte id = Utility.GetIdFromServo(port);

        byte address = Convert.ToByte(values[1]);

        var servoConfig = _configServos.GetPort(port);

        if (servoConfig == null)
          throw new Exception($"There is no dynamixel servo configured for port {port}");

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
          return BitConverter.ToUInt16(_servo_AX12.GetRam(id, address, 2), 0);
        else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
          return BitConverter.ToUInt16(_servoXL430_w250_t.GetRam(id, address, 2), 0);

        throw new Exception("Not implemented. Ask us on the synthiam community forum to add this if you need it");
      }

      if (windowCommand.Equals("ReadRamInt32", StringComparison.InvariantCultureIgnoreCase)) {

        if (values.Length != 2)
          throw new Exception(string.Format("Expecting 2 parameter, you passed {0}", values.Length));

        Servo.ServoPortEnum port = new HelperPortParser(values[0]).ServoPort;

        if (port < EZ_B.Servo.ServoPortEnum.V1 || port > EZ_B.Servo.ServoPortEnum.V99)
          throw new Exception("Only virtual servos are supported for dynamixel. Virtual servos start with a 'v', such as v1, v2, v3..");

        byte id = Utility.GetIdFromServo(port);

        byte address = Convert.ToByte(values[1]);

        var servoConfig = _configServos.GetPort(port);

        if (servoConfig == null)
          throw new Exception($"There is no dynamixel servo configured for port {port}");

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
          return BitConverter.ToInt32(_servo_AX12.GetRam(id, address, 4), 0);
        else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
          return BitConverter.ToInt32(_servoXL430_w250_t.GetRam(id, address, 4), 0);

        throw new Exception("Not implemented. Ask us on the synthiam community forum to add this if you need it");
      }

      if (windowCommand.Equals("GetDynamixelTemp", StringComparison.InvariantCultureIgnoreCase)) {

        if (values.Length != 1)
          throw new Exception(string.Format("Expecting 1 parameter, you passed {0}", values.Length));

        Servo.ServoPortEnum port = new HelperPortParser(values[0]).ServoPort;

        if (port < EZ_B.Servo.ServoPortEnum.V1 || port > EZ_B.Servo.ServoPortEnum.V99)
          throw new Exception("Only virtual servos are supported for dynamixel. Virtual servos start with a 'v', such as v1, v2, v3..");

        return getServoTemp(port);
      }

      if (windowCommand.Equals("GetDynamixelLoadDir", StringComparison.InvariantCultureIgnoreCase)) {

        if (values.Length != 1)
          throw new Exception(string.Format("Expecting 1 parameter, you passed {0}", values.Length));

        Servo.ServoPortEnum port = new HelperPortParser(values[0]).ServoPort;

        if (port < EZ_B.Servo.ServoPortEnum.V1 || port > EZ_B.Servo.ServoPortEnum.V99)
          throw new Exception("Only virtual servos are supported for dynamixel. Virtual servos start with a 'v', such as v1, v2, v3..");

        return getLoad(port).LoadDirection.ToString();
      }

      if (windowCommand.Equals("GetDynamixelLoad", StringComparison.InvariantCultureIgnoreCase)) {

        if (values.Length != 1)
          throw new Exception(string.Format("Expecting 1 parameter, you passed {0}", values.Length));

        Servo.ServoPortEnum port = new HelperPortParser(values[0]).ServoPort;

        if (port < EZ_B.Servo.ServoPortEnum.V1 || port > EZ_B.Servo.ServoPortEnum.V99)
          throw new Exception("Only virtual servos are supported for dynamixel. Virtual servos start with a 'v', such as v1, v2, v3..");

        return getLoad(port).Load;
      }

      if (windowCommand.Equals("GetDynamixelPing", StringComparison.InvariantCultureIgnoreCase)) {

        if (values.Length != 1)
          throw new Exception(string.Format("Expecting 1 parameter, you passed {0}", values.Length));

        Servo.ServoPortEnum port = new HelperPortParser(values[0]).ServoPort;

        if (port < EZ_B.Servo.ServoPortEnum.V1 || port > EZ_B.Servo.ServoPortEnum.V99)
          throw new Exception("Only virtual servos are supported for dynamixel. Virtual servos start with a 'v', such as v1, v2, v3..");

        return getServoPing(port);
      }

      if (windowCommand.Equals("setled", StringComparison.InvariantCultureIgnoreCase)) {

        if (values.Length != 2)
          throw new Exception(string.Format("Expecting 2 parameters, you passed {0}", values.Length));

        Servo.ServoPortEnum servoPort = new HelperPortParser(values[0]).ServoPort;

        if (servoPort < EZ_B.Servo.ServoPortEnum.V1 || servoPort > EZ_B.Servo.ServoPortEnum.V99)
          throw new Exception("Only virtual servos are supported for dynamixel. Virtual servos start with a 'v', such as v1, v2, v3..");

        bool status = Helper.GetTrueOrFalse(values[1]) == Helper.TrueFalseEnum.True;

        var servoConfig = _configServos.GetPort(servoPort);

        if (servoConfig == null)
          throw new Exception(string.Format("Virtual Servo {0} is not configured for dynamixel usage", servoPort));

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, $"Setting LED on {servoPort} to {status}");

        var id = Utility.GetIdFromServo(servoPort);
        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
          serialWrite(_servo_AX12.SetLED(id, status));
        else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
          serialWrite(_servoXL430_w250_t.SetLED(id, (byte)(status ? 1 : 0)));
        else
          serialWrite(_servoXL_320.SetLED(id, status));

        return true;
      }

      if (windowCommand.Equals("TorqueEnable", StringComparison.InvariantCultureIgnoreCase)) {

        if (values.Length != 2)
          throw new Exception(string.Format("Expecting 2 parameters, you passed {0}", values.Length));

        Servo.ServoPortEnum servoPort = new HelperPortParser(values[0]).ServoPort;

        bool status = Helper.GetTrueOrFalse(values[1]) == Helper.TrueFalseEnum.True;

        if (servoPort < EZ_B.Servo.ServoPortEnum.V1 || servoPort > EZ_B.Servo.ServoPortEnum.V99)
          throw new Exception("Only virtual servos are supported for dynamixel. Virtual servos start with a 'v', such as v1, v2, v3..");

        var servoConfig = _configServos.GetPort(servoPort);

        if (servoConfig == null)
          throw new Exception(string.Format("Virtual Servo {0} is not configured for dynamixel usage", servoPort));

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, $"Torque Enable {servoPort} to {status}");

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.XL_320) {

          serialWrite(_servoXL430_w250_t.SetTorqueEnable(Utility.GetIdFromServo(servoPort), (byte)(status ? 1 : 0)));
        } else {

          if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
            Invokers.SetAppendText(tbLog, true, $"Torque enable command not implemented for {servoConfig.ServoType}");
        }

        return true;
      }

      if (windowCommand.StartsWith("WriteRam", StringComparison.InvariantCultureIgnoreCase)) {

        if (values.Length != 3)
          throw new Exception(string.Format("Expecting 3 parameters, you passed {0}", values.Length));

        var address = Convert.ToInt32(values[1]);

        Servo.ServoPortEnum servoPort = new HelperPortParser(values[0]).ServoPort;

        if (servoPort < EZ_B.Servo.ServoPortEnum.V1 || servoPort > EZ_B.Servo.ServoPortEnum.V99)
          throw new Exception("Only virtual servos are supported for dynamixel. Virtual servos start with a 'v', such as v1, v2, v3..");

        var servoId = Utility.GetIdFromServo(servoPort);

        var servoConfig = _configServos.GetPort(servoPort);

        if (servoConfig == null)
          throw new Exception(string.Format("Virtual Servo {0} is not configured for dynamixel usage", servoPort));

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, $"{windowCommand} {servoPort} {servoConfig.ServoType} address {address} value {values[2]}");

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t) {

          if (windowCommand.Equals("WriteRamByte", StringComparison.InvariantCultureIgnoreCase))
            serialWrite(_servoXL430_w250_t.CreateDynamixelCommandByte(servoId, 0x03, address, Convert.ToByte(values[2])));
          else if (windowCommand.Equals("WriteRamUInt16", StringComparison.InvariantCultureIgnoreCase))
            serialWrite(_servoXL430_w250_t.CreateDynamixelCommandUInt16(servoId, 0x03, (ushort)address, Convert.ToUInt16(values[2])));
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

        return true;
      }

      return base.SendCommandV2(cancellationToken, windowCommand, values);
    }

    public override void GetSupportedControlCommandsV2(List<GetSupportedControlCommandsDefCls> items) {

      items.Add(
        new GetSupportedControlCommandsDefCls() {
          Command = "SetLED",
          Params = new string[] {
            "ServoPort",
            "true|false"
            },
          Description = "Set the LED state of the servo to on or off by passing true or false",
        });

      items.Add(
        new GetSupportedControlCommandsDefCls() {
          Command = "TorqueEnable",
          Params = new string[] {
            "ServoPort",
            "true|false"
            },
          Description = "Enable or disable the torque which releases the servo if disabled",
        });

      items.Add(
        new GetSupportedControlCommandsDefCls() {
          Command = "ReadRam",
          Params = new string[] {
            "ServoPort",
            "Address",
            "BytesToRead"
          },
          Description = "read the number of bytes from the specified address of the servo port.",
        });

      items.Add(
        new GetSupportedControlCommandsDefCls() {
          Command = "ReadRamByte",
          Params = new string[] {
            "ServoPort",
            "Address",
          },
          Description = "read a byte from the specified address of the servo port.",
        });

      items.Add(
        new GetSupportedControlCommandsDefCls() {
          Command = "ReadRamUInt16",
          Params = new string[] {
            "ServoPort",
            "Address",
          },
          Description = "read a uint16 from the specified address of the servo port.",
        });

      items.Add(
        new GetSupportedControlCommandsDefCls() {
          Command = "ReadRamInt16",
          Params = new string[] {
            "ServoPort",
            "Address",
          },
          Description = "read a int16 byte from the specified address of the servo port.",
        });

      items.Add(
        new GetSupportedControlCommandsDefCls() {
          Command = "ReadRamInt32",
          Params = new string[] {
            "ServoPort",
            "Address",
          },
          Description = "read a int32 from the specified address of the servo port.",
        });

      items.Add(
        new GetSupportedControlCommandsDefCls() {
          Command = "WriteRamByte",
          Params = new string[] {
            "ServoPort",
            "Address",
            "Value"
          },
          Description = "write a value byte to the ram at the specified address",
        });

      items.Add(
        new GetSupportedControlCommandsDefCls() {
          Command = "WriteRamUInt16",
          Params = new string[] {
            "ServoPort",
            "Address",
            "Value"
          },
          Description = "write a value unsigned int16 to the ram at the specified address",
        });

      items.Add(
        new GetSupportedControlCommandsDefCls() {
          Command = "WriteRamInt32",
          Params = new string[] {
            "ServoPort",
            "Address",
            "Value"
          },
          Description = "write a value signed int32 to the ram at the specified address",
        });

      items.Add(
        new GetSupportedControlCommandsDefCls() {
          Command = "GetDynamixelTemp",
          Params = new string[] {
            "ServoPort",
          },
          ResponseType = typeof(int),
          Description = "Returns the temp of the dynamixel servo at the specified port.",
        });

      items.Add(
        new GetSupportedControlCommandsDefCls() {
          Command = "GetDynamixelLoadDir",
          Params = new string[] {
            "ServoPort",
          },
          ResponseType = typeof(string),
          Description = "Returns the direction of load from the servo at the specified port.",
        });

      items.Add(
        new GetSupportedControlCommandsDefCls() {
          Command = "GetDynamixelLoad",
          Params = new string[] {
            "ServoPort",
          },
          ResponseType = typeof(int),
          Description = "Returns the load value from the servo at the specified port.",
        });

      items.Add(
        new GetSupportedControlCommandsDefCls() {
          Command = "GetDynamixelPing",
          Params = new string[] {
            "ServoPort",
          },
          ResponseType = typeof(bool),
          Description = "Returns the ping result of the servo.",
        });
    }

    private void Servo_OnServoGetPosition(Servo.ServoPortEnum servoPort, EZ_B.Classes.GetServoValueResponse getServoResponse) {

      if (getServoResponse.Success)
        return;

      try {

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, $"Get servo position requested for {servoPort}");

        if (!EZBManager.EZBs[0].IsConnected)
          throw new Exception("Not connected to EZB");

        var servoConfig = _configServos.GetPort(servoPort);

        if (servoConfig == null)
          throw new Exception($"Port {servoPort} is not configured as a dynamixel servo");

        var id = Utility.GetIdFromServo(servoPort);

        ushort resp;

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
          resp = _servo_AX12.GetCurrentPosition(id);
        else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
          resp = _servoXL430_w250_t.GetCurrentPosition(id);
        else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.XL_320)
          resp = _servoXL_320.GetCurrentPosition(id);
        else
          throw new Exception($"Get servo position not supported for {servoConfig.ServoType}");

        var scaled =  (int)EZ_B.Functions.RemapScalar(resp, 1, servoConfig.MaxPosition, Servo.SERVO_MIN, Servo.SERVO_MAX);

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, $"Position for {servoConfig.ServoType} is raw: {resp} scaled: {scaled}");

        getServoResponse.Success = true;
        getServoResponse.Position = scaled;
      } catch (Exception ex) {

        getServoResponse.Success = false;
        getServoResponse.ErrorStr = ex.Message;

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, $"Get servo position error: {ex.Message}");
      }
    }

    void Servo_OnServoRelease(Servo.ServoPortEnum[] servos) {

      foreach (var servoPort in servos) {

        if (servoPort < EZ_B.Servo.ServoPortEnum.V1 || servoPort > EZ_B.Servo.ServoPortEnum.V99)
          continue;

        var servoConfig = _configServos.GetPort(servoPort);

        if (servoConfig == null)
          continue;

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, $"Release servo for {servoConfig.ServoType} {servoPort}");

        var id = Utility.GetIdFromServo(servoConfig.Port);

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
          serialWrite(_servo_AX12.ReleaseTorque(id));
        else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
          serialWrite(_servoXL430_w250_t.SetTorqueEnable(id, 0));
        else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.XL_320)
          serialWrite(_servoXL_320.DisableTorque(id));
        else
          throw new Exception($"Release servo not supported for {servoConfig.ServoType}");
      }
    }

    void Servo_OnServoSpeed(EZ_B.Classes.ServoSpeedItem[] servos) {

      foreach (var servo in servos) {

        if (servo.Speed == -1)
          continue;

        if (servo.Port < EZ_B.Servo.ServoPortEnum.V1 || servo.Port > EZ_B.Servo.ServoPortEnum.V99)
          continue;

        var servoConfig = _configServos.GetPort(servo.Port);

        if (servoConfig == null)
          continue;

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, $"Speed servo for {servoConfig.ServoType} {servo.Port} to {servo.Speed}");

        var id = Utility.GetIdFromServo(servoConfig.Port);

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
          serialWrite(_servo_AX12.SetMovingSpeed(id, servo.Speed));
        else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
          serialWrite(_servoXL430_w250_t.SetSpeed(id, servo.Speed));
        else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.XL_320)
          serialWrite(_servoXL_320.SetMovingSpeed(id, servo.Speed));
        else
          throw new Exception($"Speed servo not supported for {servoConfig.ServoType}");
      }
    }

    private void Servo_OnServoVelocity(EZ_B.Classes.ServoVelocityItem[] servos) {

      foreach (var servo in servos) {

        if (servo.Velocity == -1)
          continue;

        if (servo.Port < EZ_B.Servo.ServoPortEnum.V1 || servo.Port > EZ_B.Servo.ServoPortEnum.V99)
          continue;

        var servoConfig = _configServos.GetPort(servo.Port);

        if (servoConfig == null)
          continue;

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, $"Velocity servo for {servoConfig.ServoType} {servo.Port} to {servo.Velocity}");

        var id = Utility.GetIdFromServo(servoConfig.Port);

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
          serialWrite(_servo_AX12.SetMovingSpeed(id, servo.Velocity));
        else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
          serialWrite(_servoXL430_w250_t.SetSpeed(id, servo.Velocity));
        else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.XL_320)
          serialWrite(_servoXL_320.SetMovingSpeed(id, servo.Velocity));
        else
          throw new Exception($"Velocity servo not supported for {servoConfig.ServoType}");
      }
    }

    private void Servo_OnServoAcceleration(EZ_B.Classes.ServoAccelerationItem[] servos) {

      foreach (var servo in servos) {

        if (servo.Acceleration == -1)
          continue;

        if (servo.Port < EZ_B.Servo.ServoPortEnum.V1 || servo.Port > EZ_B.Servo.ServoPortEnum.V99)
          continue;

        var servoConfig = _configServos.GetPort(servo.Port);

        if (servoConfig == null)
          continue;

        if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
          Invokers.SetAppendText(tbLog, true, $"Acceleration servo for {servoConfig.ServoType} {servo.Port} to {servo.Acceleration}");

        var id = Utility.GetIdFromServo(servoConfig.Port);

        if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
          serialWrite(_servoXL430_w250_t.SetAcceleration(id, servo.Acceleration));
        else
          throw new Exception($"Acceleration servo not supported for {servoConfig.ServoType}");
      }
    }

    void Servo_OnServoMove(EZ_B.Classes.ServoPositionItem[] servos) {

      foreach (var servo in servos) {

        if (servo.Port < Servo.ServoPortEnum.V1 || servo.Port > Servo.ServoPortEnum.V99)
          continue;

        var servoConfig = _configServos.GetPort(servo.Port);

        if (servoConfig == null)
          continue;

        byte id = Utility.GetIdFromServo(servo.Port);

        if (servoConfig.OperatingMode == ConfigServos.OperatingModeEnum.Position) {

          int position = (int)EZ_B.Functions.RemapScalar(servo.Position, Servo.SERVO_MIN, Servo.SERVO_MAX, 0, servoConfig.MaxPosition - 1);

          // POSITION SERVO MODE

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

          if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
            Invokers.SetAppendText(tbLog, true, $"Set servo position (Position mode) {servoConfig.ServoType} {servo.Port} to {servo.Position} ({position})");

          if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12) {

            serialWrite(_servo_AX12.SetGoalPosition(id, position));
          } else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t) {

            if (EZBManager.EZBs[0].Servo.IsServoReleased(servo.Port)) {

              if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
                Invokers.SetAppendText(tbLog, true, $"Enable torque {servoConfig.ServoType} {servo.Port}");

              serialWrite(_servoXL430_w250_t.SetTorqueEnable(id, 1));
            }

            serialWrite(_servoXL430_w250_t.SetGoalPosition(id, position));
          } else {

            serialWrite(_servoXL_320.SetGoalPosition(id, position));
          }
        } else {

          // WHEEL SERVO MODE (velocity mode)

          if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t) {

            if (EZBManager.EZBs[0].Servo.IsServoReleased(servo.Port)) {

              if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
                Invokers.SetAppendText(tbLog, true, $"Enable torque {servoConfig.ServoType} {servo.Port}");

              serialWrite(_servoXL430_w250_t.SetTorqueEnable(id, 1));
            }

            int velocity = (int)EZ_B.Functions.RemapScalar(servo.Position, Servo.SERVO_MIN, Servo.SERVO_MAX, -480, 480);

            if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
              Invokers.SetAppendText(tbLog, true, $"Set servo velocity (Wheel mode) {servoConfig.ServoType} {servo.Port} to {servo.Position} ({velocity})");

            serialWrite(_servoXL430_w250_t.SetGoalVelocity(id, velocity));
          } else {

            Invokers.SetAppendText(tbLog, true, $"Wheeled operating mode not implemented for {servoConfig.ServoType} on {servo.Port}");
          }
        }
      }
    }

    public override void ConfigPressed() {

      using (FormConfig form = new FormConfig()) {

        form.SetConfiguration(_cf);

        if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK) {

          SetConfiguration(form.GetConfiguration());
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

    bool getServoPing(Servo.ServoPortEnum servo) {

      var servoConfig = _configServos.GetPort(servo);

      if (servoConfig == null)
        throw new Exception($"Port {servo} is not configured as a dynamixel servo");

      var id = Utility.GetIdFromServo(servo);

      if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
        Invokers.SetAppendText(tbLog, true, $"Ping requested for {servoConfig.ServoType} {servo}");

      bool resp;

      if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
        resp = _servo_AX12.Ping(id);
      else
        resp = _servoXL430_w250_t.Ping(id);

      if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
        Invokers.SetAppendText(tbLog, true, $"Response: {resp}");

      return resp;
    }

    int getServoTemp(Servo.ServoPortEnum servo) {

      var servoConfig = _configServos.GetPort(servo);

      if (servoConfig == null)
        throw new Exception($"Port {servo} is not configured as a dynamixel servo");

      var id = Utility.GetIdFromServo(servo);

      if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
        Invokers.SetAppendText(tbLog, true, $"Get temp requested for {servoConfig.ServoType} {servo}");

      byte resp;

      if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
        resp = _servo_AX12.GetTemperature(id);
      else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
        resp = _servoXL430_w250_t.GetTemperature(id);
      else
        throw new Exception($"Get temp not supported for {servoConfig.ServoType}");

      if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
        Invokers.SetAppendText(tbLog, true, $"Response: {resp}");

      return resp;
    }

    GetLoadResponseCls getLoad(Servo.ServoPortEnum servo) {

      var servoConfig = _configServos.GetPort(servo);

      if (servoConfig == null)
        throw new Exception($"Port {servo} is not configured as a dynamixel servo");

      var id = Utility.GetIdFromServo(servo);

      if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
        Invokers.SetAppendText(tbLog, true, $"Get load requested for {servoConfig.ServoType} {servo}");

      GetLoadResponseCls resp;

      if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.AX_12)
        resp = _servo_AX12.GetLoad(id);
      else if (servoConfig.ServoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
        resp = _servoXL430_w250_t.GetPresentLoad(id);
      else
        throw new Exception($"Get load not supported for {servoConfig.ServoType}");

      if (Convert.ToBoolean(_cf.STORAGE[ConfigTitles.DEBUG]))
        Invokers.SetAppendText(tbLog, true, $"Response: {resp.Load} / {resp.LoadDirection}");

      return resp;
    }

    private void btnClearLog_Click(object sender, EventArgs e) {

      Invokers.SetText(tbLog, string.Empty);
    }
  }
}
