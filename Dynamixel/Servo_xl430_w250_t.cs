using System;
using System.Collections.Generic;

namespace Dynamixel {
  /// <summary>
  /// Represents the basic instruction set used for Dynamixel servo communication.
  /// Each member corresponds to a specific command that can be sent to the servo.
  /// </summary>
  public enum Instruction : byte {
    /// <summary>
    /// Ping (1): Used to verify the presence of a servo.
    /// The servo responds with a status packet confirming its connectivity.
    /// </summary>
    Ping = 1,

    /// <summary>
    /// Read (2): Instructs the servo to return data from its control table.
    /// The command includes the starting address and the number of bytes to read.
    /// </summary>
    Read = 2,

    /// <summary>
    /// Write (3): Commands the servo to write data to a specified address in its control table.
    /// The command includes both the address and the data to be written.
    /// </summary>
    Write = 3,

    /// <summary>
    /// RegWrite (4): Buffers a write command to be executed later.
    /// Multiple RegWrite commands are synchronized using an Action command.
    /// </summary>
    RegWrite = 4,

    /// <summary>
    /// Action (5): Executes all previously buffered RegWrite commands simultaneously.
    /// This is useful for coordinated movement in multi-servo systems.
    /// </summary>
    Action = 5,

    /// <summary>
    /// Reset (6): Resets the servo to its factory default settings.
    /// (Some parameters, such as the servo ID and baud rate, may be retained.)
    /// </summary>
    Reset = 6
  }

  /// <summary>
  /// This class encapsulates command creation and communication for the Dynamixel XL430-W250-T servo.
  /// It includes methods to build commands as well as read (Get…) and write (Set…) methods for specific control table registers.
  /// </summary>
  public class Servo_xl430_w250_t {
    // Static readonly CRC table to avoid recreating it on every CRC update.
    private static readonly UInt16[] crc_table = new UInt16[] {
      0x0000, 0x8005, 0x800F, 0x000A, 0x801B, 0x001E, 0x0014, 0x8011,
      0x8033, 0x0036, 0x003C, 0x8039, 0x0028, 0x802D, 0x8027, 0x0022,
      0x8063, 0x0066, 0x006C, 0x8069, 0x0078, 0x807D, 0x8077, 0x0072,
      0x0050, 0x8055, 0x805F, 0x005A, 0x804B, 0x004E, 0x0044, 0x8041,
      0x80C3, 0x00C6, 0x00CC, 0x80C9, 0x00D8, 0x80DD, 0x80D7, 0x00D2,
      0x00F0, 0x80F5, 0x80FF, 0x00FA, 0x80EB, 0x00EE, 0x00E4, 0x80E1,
      0x00A0, 0x80A5, 0x80AF, 0x00AA, 0x80BB, 0x00BE, 0x00B4, 0x80B1,
      0x8093, 0x0096, 0x009C, 0x8099, 0x0088, 0x808D, 0x8087, 0x0082,
      0x8183, 0x0186, 0x018C, 0x8189, 0x0198, 0x819D, 0x8197, 0x0192,
      0x01B0, 0x81B5, 0x81BF, 0x01BA, 0x81AB, 0x01AE, 0x01A4, 0x81A1,
      0x01E0, 0x81E5, 0x81EF, 0x01EA, 0x81FB, 0x01FE, 0x01F4, 0x81F1,
      0x81D3, 0x01D6, 0x01DC, 0x81D9, 0x01C8, 0x81CD, 0x81C7, 0x01C2,
      0x0140, 0x8145, 0x814F, 0x014A, 0x815B, 0x015E, 0x0154, 0x8151,
      0x8173, 0x0176, 0x017C, 0x8179, 0x0168, 0x816D, 0x8167, 0x0162,
      0x8123, 0x0126, 0x012C, 0x8129, 0x0138, 0x813D, 0x8137, 0x0132,
      0x0110, 0x8115, 0x811F, 0x011A, 0x810B, 0x010E, 0x0104, 0x8101,
      0x8303, 0x0306, 0x030C, 0x8309, 0x0318, 0x831D, 0x8317, 0x0312,
      0x0330, 0x8335, 0x833F, 0x033A, 0x832B, 0x032E, 0x0324, 0x8321,
      0x0360, 0x8365, 0x836F, 0x036A, 0x837B, 0x037E, 0x0374, 0x8371,
      0x8353, 0x0356, 0x035C, 0x8359, 0x0348, 0x834D, 0x8347, 0x0342,
      0x03C0, 0x83C5, 0x83CF, 0x03CA, 0x83DB, 0x03DE, 0x03D4, 0x83D1,
      0x83F3, 0x03F6, 0x03FC, 0x83F9, 0x03E8, 0x83ED, 0x83E7, 0x03E2,
      0x83A3, 0x03A6, 0x03AC, 0x83A9, 0x03B8, 0x83BD, 0x83B7, 0x03B2,
      0x0390, 0x8395, 0x839F, 0x039A, 0x838B, 0x038E, 0x0384, 0x8381,
      0x0280, 0x8285, 0x828F, 0x028A, 0x829B, 0x029E, 0x0294, 0x8291,
      0x82B3, 0x02B6, 0x02BC, 0x82B9, 0x02A8, 0x82AD, 0x82A7, 0x02A2,
      0x82E3, 0x02E6, 0x02EC, 0x82E9, 0x02F8, 0x82FD, 0x82F7, 0x02F2,
      0x02D0, 0x82D5, 0x82DF, 0x02DA, 0x82CB, 0x02CE, 0x02C4, 0x82C1,
      0x8243, 0x0246, 0x024C, 0x8249, 0x0258, 0x825D, 0x8257, 0x0252,
      0x0270, 0x8275, 0x827F, 0x027A, 0x826B, 0x026E, 0x0264, 0x8261,
      0x0220, 0x8225, 0x822F, 0x022A, 0x823B, 0x023E, 0x0234, 0x8231,
      0x8213, 0x0216, 0x021C, 0x8219, 0x0208, 0x820D, 0x8207, 0x0202
    };

    /// <summary>
    /// Calculates the CRC for a given data block.
    /// </summary>
    /// <param name="data_blk_ptr">Array of bytes over which the CRC is calculated.</param>
    /// <returns>CRC value as UInt16.</returns>
    private UInt16 update_crc(byte[] data_blk_ptr) {
      int crc_accum = 0;
      for (int j = 0; j < data_blk_ptr.Length; j++) {
        int i = ((UInt16)(crc_accum >> 8) ^ data_blk_ptr[j]) & 0xFF;
        crc_accum = (crc_accum << 8) ^ crc_table[i];
      }
      return (UInt16)crc_accum;
    }

    /// <summary>
    /// Helper method to add the common Dynamixel protocol header to a command.
    /// </summary>
    /// <param name="txbuffer">The transmit buffer to which the header is added.</param>
    /// <param name="id">Servo ID.</param>
    private void AddHeader(List<byte> txbuffer, byte id) {
      // Dynamixel protocol header: 0xFF, 0xFF, 0xFD, 0x00, followed by the servo ID.
      txbuffer.Add(0xFF);
      txbuffer.Add(0xFF);
      txbuffer.Add(0xFD);
      txbuffer.Add(0x00);
      txbuffer.Add(id);
    }

    public event EventHandler<OnCommCls> OnCommunication;

    /// <summary>
    /// Sends the command to the servo and receives the response.
    /// This method should be implemented with the actual communication interface (e.g., serial port).
    /// </summary>
    /// <param name="command">Command byte array to send.</param>
    /// <returns>Response byte array from the servo.</returns>
    private byte[] ExecuteCommand(byte[] command) {

      var commCls = new OnCommCls() {
        SendBytes = command
      };

      OnCommunication?.Invoke(this, commCls);

      return commCls.ResponseBytes;
    }

    /// <summary>
    /// Extracts the parameter bytes from a servo response.
    /// </summary>
    /// <param name="response">Response byte array from the servo.</param>
    /// <returns>Parameter bytes extracted from the response.</returns>
    private byte[] ExtractParameterBytes(byte[] response) {

      if (response.Length < 11)
        throw new Exception("Invalid response length.");

      // LENGTH field is at index 5-6; error is at index 7.
      ushort length = BitConverter.ToUInt16(response, 5);
      int parameterLength = length - 3; // subtract error (1) and CRC (2)
      byte[] parameters = new byte[parameterLength];

      Array.Copy(response, 9, parameters, 0, parameterLength);

      return parameters;
    }

    /// <summary>
    /// Gets the error code from a servo response.
    /// </summary>
    /// <param name="response">Response byte array from the servo.</param>
    /// <returns>Error code byte.</returns>
    private byte GetError(byte[] response) {

      if (response.Length < 9)
        throw new Exception($"Nope, response was {response.Length} bytes.");

      // Error is at index 8.
      return response[8];
    }

    private byte[] CreateDynamixelCommand(byte id, byte instruction) {
      List<byte> txBuffer = new List<byte>();
      AddHeader(txBuffer, id);
      // Length: 3 = instruction (1) + CRC (2)
      txBuffer.AddRange(BitConverter.GetBytes((UInt16)3));
      txBuffer.Add(instruction);
      txBuffer.AddRange(BitConverter.GetBytes(update_crc(txBuffer.ToArray())));
      return txBuffer.ToArray();
    }

    /// <summary>
    /// Creates a generic read command for a given control table address and length.
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <param name="address">Control table address to read from.</param>
    /// <param name="length">Number of bytes to read.</param>
    /// <returns>Command byte array for reading the register.</returns>
    private byte[] ReadRegister(byte id, ushort address, ushort length) {
      return CreateDynamixelCommandUInt16(id, (byte)Instruction.Read, address, length);
    }

    /// <summary>
    /// Creates a command with a single UInt16 parameter.
    /// </summary>
    private byte[] CreateDynamixelCommandUInt16(byte id, byte instruction, UInt16 parameter1) {
      List<byte> txbuffer = new List<byte>();
      AddHeader(txbuffer, id);
      // Length = 3: instruction (1 byte) + parameter (2 bytes)
      txbuffer.AddRange(BitConverter.GetBytes((UInt16)3));
      txbuffer.Add(instruction);
      txbuffer.AddRange(BitConverter.GetBytes(parameter1));
      txbuffer.AddRange(BitConverter.GetBytes(update_crc(txbuffer.ToArray())));
      return txbuffer.ToArray();
    }

    /// <summary>
    /// Overload to create a command with two UInt16 parameters (used for read commands: address + data length).
    /// </summary>
    public byte[] CreateDynamixelCommandUInt16(byte id, byte instruction, UInt16 address, UInt16 value) {
      List<byte> txbuffer = new List<byte>();
      AddHeader(txbuffer, id);
      // Length = 7: instruction (1) + address (2) + value (2) + CRC (2)
      txbuffer.AddRange(BitConverter.GetBytes((UInt16)7));
      txbuffer.Add(instruction);
      txbuffer.AddRange(BitConverter.GetBytes(address));
      txbuffer.AddRange(BitConverter.GetBytes(value));
      txbuffer.AddRange(BitConverter.GetBytes(update_crc(txbuffer.ToArray())));
      return txbuffer.ToArray();
    }

    /// <summary>
    /// Creates a command with an Int32 parameter.
    /// </summary>
    public byte[] CreateDynamixelCommandInt32(byte id, byte instruction, int address, Int32 value) {
      List<byte> txbuffer = new List<byte>();
      AddHeader(txbuffer, id);
      // Length = 9: instruction (1) + address (2) + Int32 value (4) + CRC (2)
      txbuffer.AddRange(BitConverter.GetBytes((UInt16)9));
      txbuffer.Add(instruction);
      txbuffer.AddRange(BitConverter.GetBytes((UInt16)address));
      txbuffer.AddRange(BitConverter.GetBytes(value));
      txbuffer.AddRange(BitConverter.GetBytes(update_crc(txbuffer.ToArray())));
      return txbuffer.ToArray();
    }

    /// <summary>
    /// Creates a command with a single byte parameter.
    /// </summary>
    public byte[] CreateDynamixelCommandByte(byte id, byte instruction, int address, byte value) {
      List<byte> txbuffer = new List<byte>();
      AddHeader(txbuffer, id);
      // Length = 6: instruction (1) + address (2) + byte value (1) + CRC (2)
      txbuffer.AddRange(BitConverter.GetBytes((UInt16)6));
      txbuffer.Add(instruction);
      txbuffer.AddRange(BitConverter.GetBytes((UInt16)address));
      txbuffer.Add(value);
      txbuffer.AddRange(BitConverter.GetBytes(update_crc(txbuffer.ToArray())));
      return txbuffer.ToArray();
    }


    /// <summary>
    /// Gets the 2-byte Model Number from control table address 0.
    /// </summary>
    public ushort GetModelNumber(byte id) {
      byte[] command = ReadRegister(id, 0, 2);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Model Number (Address 0).");
      byte[] parameters = ExtractParameterBytes(response);
      return BitConverter.ToUInt16(parameters, 0);
    }

    /// <summary>
    /// Gets the 2-byte Model Information from control table address 2.
    /// </summary>
    public ushort GetModelInformation(byte id) {
      byte[] command = ReadRegister(id, 2, 2);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Model Information (Address 2).");
      byte[] parameters = ExtractParameterBytes(response);
      return BitConverter.ToUInt16(parameters, 0);
    }

    /// <summary>
    /// Gets the 1-byte Firmware Version from control table address 6.
    /// </summary>
    public byte GetFirmwareVersion(byte id) {
      byte[] command = ReadRegister(id, 6, 1);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Firmware Version (Address 6).");
      byte[] parameters = ExtractParameterBytes(response);
      return parameters[0];
    }

    /// <summary>
    /// Gets the 1-byte Servo ID from control table address 7.
    /// </summary>
    public byte GetServoID(byte id) {
      byte[] command = ReadRegister(id, 7, 1);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Servo ID (Address 7).");
      byte[] parameters = ExtractParameterBytes(response);
      return parameters[0];
    }

    /// <summary>
    /// Gets the 1-byte Baud Rate from control table address 8.
    /// </summary>
    public byte GetBaudRate(byte id) {
      byte[] command = ReadRegister(id, 8, 1);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Baud Rate (Address 8).");
      byte[] parameters = ExtractParameterBytes(response);
      return parameters[0];
    }

    /// <summary>
    /// Gets the 1-byte Return Delay Time from control table address 10.
    /// </summary>
    public byte GetReturnDelayTime(byte id) {
      byte[] command = ReadRegister(id, 10, 1);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Return Delay Time (Address 10).");
      byte[] parameters = ExtractParameterBytes(response);
      return parameters[0];
    }

    /// <summary>
    /// Gets the 1-byte Operating Mode from control table address 11.
    /// </summary>
    public byte GetOperatingMode(byte id) {
      byte[] command = ReadRegister(id, 11, 1);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Operating Mode (Address 11).");
      byte[] parameters = ExtractParameterBytes(response);
      return parameters[0];
    }

    /// <summary>
    /// Gets the 4-byte Homing Offset from control table address 20.
    /// </summary>
    public int GetHomingOffset(byte id) {
      byte[] command = ReadRegister(id, 20, 4);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Homing Offset (Address 20).");
      byte[] parameters = ExtractParameterBytes(response);
      return BitConverter.ToInt32(parameters, 0);
    }

    /// <summary>
    /// Gets the 4-byte Moving Threshold from control table address 24.
    /// </summary>
    public int GetMovingThreshold(byte id) {
      byte[] command = ReadRegister(id, 24, 4);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Moving Threshold (Address 24).");
      byte[] parameters = ExtractParameterBytes(response);
      return BitConverter.ToInt32(parameters, 0);
    }

    /// <summary>
    /// Gets the 2-byte value from reserved control table address 31.
    /// </summary>
    public ushort GetReserved31(byte id) {
      byte[] command = ReadRegister(id, 31, 2);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Reserved (Address 31).");
      byte[] parameters = ExtractParameterBytes(response);
      return BitConverter.ToUInt16(parameters, 0);
    }

    /// <summary>
    /// Gets the 2-byte value from reserved control table address 32.
    /// </summary>
    public ushort GetReserved32(byte id) {
      byte[] command = ReadRegister(id, 32, 2);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Reserved (Address 32).");
      byte[] parameters = ExtractParameterBytes(response);
      return BitConverter.ToUInt16(parameters, 0);
    }

    /// <summary>
    /// Gets the 2-byte value from reserved control table address 34.
    /// </summary>
    public ushort GetReserved34(byte id) {
      byte[] command = ReadRegister(id, 34, 2);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Reserved (Address 34).");
      byte[] parameters = ExtractParameterBytes(response);
      return BitConverter.ToUInt16(parameters, 0);
    }

    /// <summary>
    /// Gets the 2-byte value from reserved control table address 36.
    /// </summary>
    public ushort GetReserved36(byte id) {
      byte[] command = ReadRegister(id, 36, 2);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Reserved (Address 36).");
      byte[] parameters = ExtractParameterBytes(response);
      return BitConverter.ToUInt16(parameters, 0);
    }

    /// <summary>
    /// Gets the 2-byte value from reserved control table address 44.
    /// </summary>
    public ushort GetReserved44(byte id) {
      byte[] command = ReadRegister(id, 44, 2);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Reserved (Address 44).");
      byte[] parameters = ExtractParameterBytes(response);
      return BitConverter.ToUInt16(parameters, 0);
    }

    /// <summary>
    /// Gets the 2-byte value from reserved control table address 48.
    /// </summary>
    public ushort GetReserved48(byte id) {
      byte[] command = ReadRegister(id, 48, 2);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Reserved (Address 48).");
      byte[] parameters = ExtractParameterBytes(response);
      return BitConverter.ToUInt16(parameters, 0);
    }

    /// <summary>
    /// Gets the 2-byte value from reserved control table address 52.
    /// </summary>
    public ushort GetReserved52(byte id) {
      byte[] command = ReadRegister(id, 52, 2);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Reserved (Address 52).");
      byte[] parameters = ExtractParameterBytes(response);
      return BitConverter.ToUInt16(parameters, 0);
    }

    /// <summary>
    /// Gets the 1-byte Torque Enable value from control table address 64.
    /// </summary>
    public byte GetTorqueEnable(byte id) {
      byte[] command = ReadRegister(id, 64, 1);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Torque Enable (Address 64).");
      byte[] parameters = ExtractParameterBytes(response);
      return parameters[0];
    }

    /// <summary>
    /// Gets the 1-byte LED value from control table address 65.
    /// </summary>
    public byte GetLED(byte id) {
      byte[] command = ReadRegister(id, 65, 1);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading LED (Address 65).");
      byte[] parameters = ExtractParameterBytes(response);
      return parameters[0];
    }

    /// <summary>
    /// Gets the temperature control table address 146
    /// </summary>
    public byte GetTemperature(byte id) {

      byte[] command = ReadRegister(id, 146, 1);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Temp Status Packet (Address 146).");
      byte[] parameters = ExtractParameterBytes(response);

      return parameters[0];
    }


    /// <summary>
    /// Gets the ushort present load control table address 126
    /// </summary>
    public GetLoadResponseCls GetPresentLoad(byte id) {
      byte[] command = ReadRegister(id, 126, 2);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Present Load Status Packet (Address 126).");
      byte[] parameters = ExtractParameterBytes(response);

      return new GetLoadResponseCls() {
        Load = BitConverter.ToInt16(parameters, 0)
      };
    }

    /// <summary>
    /// Gets the 1-byte Disable Status Packet value from control table address 68.
    /// </summary>
    public byte GetDisableStatusPacket(byte id) {
      byte[] command = ReadRegister(id, 68, 1);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Disable Status Packet (Address 68).");
      byte[] parameters = ExtractParameterBytes(response);
      return parameters[0];
    }

    /// <summary>
    /// Gets the 4-byte Goal Velocity from control table address 104.
    /// </summary>
    public int GetGoalVelocity(byte id) {
      byte[] command = ReadRegister(id, 104, 4);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Goal Velocity (Address 104).");
      byte[] parameters = ExtractParameterBytes(response);
      return BitConverter.ToInt32(parameters, 0);
    }

    /// <summary>
    /// Gets the 4-byte Acceleration from control table address 108.
    /// </summary>
    public int GetAcceleration(byte id) {
      byte[] command = ReadRegister(id, 108, 4);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Acceleration (Address 108).");
      byte[] parameters = ExtractParameterBytes(response);
      return BitConverter.ToInt32(parameters, 0);
    }

    /// <summary>
    /// Gets the 4-byte Speed/Velocity from control table address 112.
    /// </summary>
    public int GetSpeed(byte id) {
      byte[] command = ReadRegister(id, 112, 4);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Speed/Velocity (Address 112).");
      byte[] parameters = ExtractParameterBytes(response);
      return BitConverter.ToInt32(parameters, 0);
    }

    /// <summary>
    /// Gets the 4-byte Goal Position from control table address 116.
    /// </summary>
    public int GetGoalPosition(byte id) {
      byte[] command = ReadRegister(id, 116, 4);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Goal Position (Address 116).");
      byte[] parameters = ExtractParameterBytes(response);
      return BitConverter.ToInt32(parameters, 0);
    }

    /// <summary>
    /// Gets the 1-byte value from reserved control table address 122.
    /// </summary>
    public byte GetReserved122(byte id) {
      byte[] command = ReadRegister(id, 122, 1);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Reserved (Address 122).");
      byte[] parameters = ExtractParameterBytes(response);
      return parameters[0];
    }

    /// <summary>
    /// Gets the 1-byte value from reserved control table address 123.
    /// </summary>
    public byte GetReserved123(byte id) {
      byte[] command = ReadRegister(id, 123, 1);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Reserved (Address 123).");
      byte[] parameters = ExtractParameterBytes(response);
      return parameters[0];
    }

    /// <summary>
    /// Gets the 1-byte value from reserved control table address 126.
    /// </summary>
    public byte GetReserved126(byte id) {
      byte[] command = ReadRegister(id, 126, 1);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Reserved (Address 126).");
      byte[] parameters = ExtractParameterBytes(response);
      return parameters[0];
    }

    /// <summary>
    /// Gets the 1-byte value from reserved control table address 128.
    /// </summary>
    public byte GetReserved128(byte id) {
      byte[] command = ReadRegister(id, 128, 1);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Reserved (Address 128).");
      byte[] parameters = ExtractParameterBytes(response);
      return parameters[0];
    }

    /// <summary>
    /// Gets the 2-byte Current Position from control table address 132.
    /// </summary>
    public ushort GetCurrentPosition(byte id) {
      byte[] command = ReadRegister(id, 132, 2);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Current Position (Address 132).");
      byte[] parameters = ExtractParameterBytes(response);
      return BitConverter.ToUInt16(parameters, 0);
    }

    public byte[] GetRam(byte id, byte address, byte bytesToRead) {
      byte[] command = ReadRegister(id, address, bytesToRead);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception($"Error reading {bytesToRead} bytes from address {address} of servo {id}");
      byte[] parameters = ExtractParameterBytes(response);
      return parameters;
    }

    public byte[] PingCmd(byte id) {

      // Create ping command using instruction code 1.
      byte[] command = CreateDynamixelCommand(id, 1);
      return command;
    }

    public bool Ping(byte id) {

      // Create ping command using instruction code 1.
      byte[] command = CreateDynamixelCommand(id, 1);
      byte[] response = ExecuteCommand(command);
      // Check if the error byte in the response is 0.
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 2-byte Model Number at control table address 0.
    /// Returns true if the operation was successful.
    /// </summary>
    public bool SetModelNumber(byte id, ushort value) {
      byte[] command = CreateDynamixelCommandUInt16(id, (byte)Instruction.Write, 0, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 2-byte Model Information at control table address 2.
    /// </summary>
    public bool SetModelInformation(byte id, ushort value) {
      byte[] command = CreateDynamixelCommandUInt16(id, (byte)Instruction.Write, 2, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 1-byte Firmware Version at control table address 6.
    /// </summary>
    public bool SetFirmwareVersion(byte id, byte value) {
      byte[] command = CreateDynamixelCommandByte(id, (byte)Instruction.Write, 6, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 1-byte Servo ID at control table address 7.
    /// </summary>
    public byte[] SetServoID(byte id, byte value) {
      byte[] command = CreateDynamixelCommandByte(id, (byte)Instruction.Write, 7, value);
      return command;
    }

    /// <summary>
    /// Sets the 1-byte Baud Rate at control table address 8.
    /// </summary>
    public bool SetBaudRate(byte id, byte value) {
      byte[] command = CreateDynamixelCommandByte(id, (byte)Instruction.Write, 8, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 1-byte Return Delay Time at control table address 10.
    /// </summary>
    public bool SetReturnDelayTime(byte id, byte value) {
      byte[] command = CreateDynamixelCommandByte(id, (byte)Instruction.Write, 10, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 1-byte Operating Mode at control table address 11.
    /// </summary>
    public byte[] SetOperatingMode(byte id, byte value) {
      byte[] command = CreateDynamixelCommandByte(id, (byte)Instruction.Write, 11, value);
      return command;
    }

    /// <summary>
    /// Sets the 4-byte Homing Offset at control table address 20.
    /// </summary>
    public bool SetHomingOffset(byte id, int value) {
      byte[] command = CreateDynamixelCommandInt32(id, (byte)Instruction.Write, 20, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 4-byte Moving Threshold at control table address 24.
    /// </summary>
    public bool SetMovingThreshold(byte id, int value) {
      byte[] command = CreateDynamixelCommandInt32(id, (byte)Instruction.Write, 24, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 2-byte value at reserved control table address 31.
    /// </summary>
    public bool SetReserved31(byte id, ushort value) {
      byte[] command = CreateDynamixelCommandUInt16(id, (byte)Instruction.Write, 31, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 2-byte value at reserved control table address 32.
    /// </summary>
    public bool SetReserved32(byte id, ushort value) {
      byte[] command = CreateDynamixelCommandUInt16(id, (byte)Instruction.Write, 32, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 2-byte value at reserved control table address 34.
    /// </summary>
    public bool SetReserved34(byte id, ushort value) {
      byte[] command = CreateDynamixelCommandUInt16(id, (byte)Instruction.Write, 34, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 2-byte value at reserved control table address 36.
    /// </summary>
    public bool SetReserved36(byte id, ushort value) {
      byte[] command = CreateDynamixelCommandUInt16(id, (byte)Instruction.Write, 36, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 2-byte value at reserved control table address 44.
    /// </summary>
    public bool SetReserved44(byte id, ushort value) {
      byte[] command = CreateDynamixelCommandUInt16(id, (byte)Instruction.Write, 44, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 2-byte value at reserved control table address 48.
    /// </summary>
    public bool SetReserved48(byte id, ushort value) {
      byte[] command = CreateDynamixelCommandUInt16(id, (byte)Instruction.Write, 48, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 2-byte value at reserved control table address 52.
    /// </summary>
    public bool SetReserved52(byte id, ushort value) {
      byte[] command = CreateDynamixelCommandUInt16(id, (byte)Instruction.Write, 52, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 1-byte Torque Enable at control table address 64.
    /// </summary>
    public byte[] SetTorqueEnable(byte id, byte value) {

      byte[] command = CreateDynamixelCommandByte(id, (byte)Instruction.Write, 64, value);
      return command;
    }

    /// <summary>
    /// Sets the 1-byte LED at control table address 65.
    /// </summary>
    public byte[] SetLED(byte id, byte value) {
      byte[] command = CreateDynamixelCommandByte(id, (byte)Instruction.Write, 65, value);
      return command;
    }

    /// <summary>
    /// Sets the 1-byte Disable Status Packet at control table address 68.
    /// </summary>
    public byte[] SetDisableStatusPacket(byte id, byte value) {
      byte[] command = CreateDynamixelCommandByte(id, (byte)Instruction.Write, 68, value);
      return command;
    }

    /// <summary>
    /// Sets the 4-byte Goal Velocity at control table address 104.
    /// </summary>
    public byte[] SetGoalVelocity(byte id, int value) {
      byte[] command = CreateDynamixelCommandInt32(id, (byte)Instruction.Write, 104, value);
      return command;
    }

    /// <summary>
    /// Sets the 4-byte Acceleration at control table address 108.
    /// </summary>
    public byte[] SetAcceleration(byte id, int value) {
      byte[] command = CreateDynamixelCommandInt32(id, (byte)Instruction.Write, 108, value);
      return command;
    }

    /// <summary>
    /// Sets the 4-byte Speed/Velocity at control table address 112.
    /// </summary>
    public byte[] SetSpeed(byte id, int value) {
      byte[] command = CreateDynamixelCommandInt32(id, (byte)Instruction.Write, 112, value);
      return command;
    }

    /// <summary>
    /// Sets the 4-byte Goal Position at control table address 116.
    /// </summary>
    public byte[] SetGoalPosition(byte id, int value) {
      byte[] command = CreateDynamixelCommandInt32(id, (byte)Instruction.Write, 116, value);
      return command;
    }

    /// <summary>
    /// Sets the 1-byte value at reserved control table address 122.
    /// </summary>
    public bool SetReserved122(byte id, byte value) {
      byte[] command = CreateDynamixelCommandByte(id, (byte)Instruction.Write, 122, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 1-byte value at reserved control table address 123.
    /// </summary>
    public bool SetReserved123(byte id, byte value) {
      byte[] command = CreateDynamixelCommandByte(id, (byte)Instruction.Write, 123, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 1-byte value at reserved control table address 126.
    /// </summary>
    public bool SetReserved126(byte id, byte value) {
      byte[] command = CreateDynamixelCommandByte(id, (byte)Instruction.Write, 126, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 1-byte value at reserved control table address 128.
    /// </summary>
    public bool SetReserved128(byte id, byte value) {
      byte[] command = CreateDynamixelCommandByte(id, (byte)Instruction.Write, 128, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Sets the 2-byte Current Position at control table address 132.
    /// </summary>
    public bool SetCurrentPosition(byte id, ushort value) {
      byte[] command = CreateDynamixelCommandUInt16(id, (byte)Instruction.Write, 132, value);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Reset to factory default
    /// </summary>
    public byte[] ResetToFactoryDefault(byte id) {
      byte[] command = CreateDynamixelCommandUInt16(id, (byte)Instruction.Write, 132);

      return command;
    }

  }
}
