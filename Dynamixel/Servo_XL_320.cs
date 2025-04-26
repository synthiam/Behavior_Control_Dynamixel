using System;
using System.Collections.Generic;

namespace Dynamixel {
  /// <summary>
  /// This class encapsulates command creation and communication for the XL‑320 servo (Protocol 2.0).
  /// It provides low‑level methods for building commands as well as high‑level methods (Get/Set) that work
  /// with specific control table registers.
  /// </summary>
  public class Servo_XL_320 {
    #region Static CRC Table
    // Static readonly CRC table (Protocol 2.0) to avoid recreating it on every CRC update.
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
    #endregion

    #region Communication Helpers

    /// <summary>
    /// Calculates the CRC for a given data block.
    /// </summary>
    /// <param name="data">Byte array over which to calculate the CRC.</param>
    /// <returns>Computed CRC as a UInt16 value.</returns>
    private UInt16 UpdateCRC(byte[] data) {
      int crc_accum = 0;
      for (int j = 0; j < data.Length; j++) {
        int i = ((UInt16)(crc_accum >> 8) ^ data[j]) & 0xFF;
        crc_accum = (crc_accum << 8) ^ crc_table[i];
      }
      return (UInt16)crc_accum;
    }

    /// <summary>
    /// Adds the common Protocol 2.0 header to the transmit buffer.
    /// Header format: 0xFF, 0xFF, 0xFD, 0x00, followed by the servo ID.
    /// </summary>
    /// <param name="txBuffer">The transmit buffer list.</param>
    /// <param name="id">Servo ID.</param>
    private void AddHeader(List<byte> txBuffer, byte id) {
      txBuffer.Add(0xFF);
      txBuffer.Add(0xFF);
      txBuffer.Add(0xFD);
      txBuffer.Add(0x00);
      txBuffer.Add(id);
    }

    public event EventHandler<OnCommCls> OnCommunication;

    /// <summary>
    /// Sends the command packet to the servo and returns its response.
    /// This method must be implemented to communicate with your servo hardware (e.g. via a serial port).
    /// </summary>
    /// <param name="command">The command packet to send.</param>
    /// <returns>The response packet from the servo.</returns>
    private byte[] ExecuteCommand(byte[] command) {

      var commCls = new OnCommCls() {
        SendBytes = command
      };

      OnCommunication?.Invoke(this, commCls);

      return commCls.ResponseBytes;
    }

    /// <summary>
    /// Extracts the parameter bytes from a response packet.
    /// Assumes Protocol 2.0 response format: [0xFF, 0xFF, 0xFD, 0x00, id, length (2 bytes), error, parameters..., CRC (2 bytes)].
    /// </summary>
    /// <param name="response">The complete response packet.</param>
    /// <returns>Byte array containing only the parameter bytes.</returns>
    private byte[] ExtractParameterBytes(byte[] response) {
      if (response.Length < 11)
        throw new Exception("Invalid response length.");
      // The LENGTH field is at index 5 (2 bytes). Subtract 3 (1 error + 2 CRC) to get parameter length.
      ushort length = BitConverter.ToUInt16(response, 5);
      int parameterLength = length - 3;
      byte[] parameters = new byte[parameterLength];
      Array.Copy(response, 9, parameters, 0, parameterLength);
      return parameters;
    }

    /// <summary>
    /// Retrieves the error byte from the response packet.
    /// </summary>
    /// <param name="response">The complete response packet.</param>
    /// <returns>Error code as a byte.</returns>
    private byte GetError(byte[] response) {

      if (response.Length < 9)
        throw new Exception($"Nope, response was {response.Length} bytes.");

      // Error is at index 8.
      return response[8];
    }
    #endregion

    #region Command Creation Methods

    /// <summary>
    /// Creates a command packet with no extra parameters.
    /// Length is 3 (instruction + 2 CRC bytes).
    /// </summary>
    private byte[] CreateDynamixelCommand(byte id, byte instruction) {
      List<byte> txBuffer = new List<byte>();
      AddHeader(txBuffer, id);
      // Length: 3 = instruction (1) + CRC (2)
      txBuffer.AddRange(BitConverter.GetBytes((UInt16)3));
      txBuffer.Add(instruction);
      txBuffer.AddRange(BitConverter.GetBytes(UpdateCRC(txBuffer.ToArray())));
      return txBuffer.ToArray();
    }

    /// <summary>
    /// Creates a command packet that includes an address parameter.
    /// Length is 5 (instruction + address (2) + 2 CRC bytes).
    /// </summary>
    private byte[] CreateDynamixelCommand(byte id, byte instruction, ushort address) {
      List<byte> txBuffer = new List<byte>();
      AddHeader(txBuffer, id);
      // Length: 5 = instruction (1) + address (2) + CRC (2)
      txBuffer.AddRange(BitConverter.GetBytes((UInt16)5));
      txBuffer.Add(instruction);
      txBuffer.AddRange(BitConverter.GetBytes(address));
      txBuffer.AddRange(BitConverter.GetBytes(UpdateCRC(txBuffer.ToArray())));
      return txBuffer.ToArray();
    }

    /// <summary>
    /// Creates a command packet with a 1‑byte value parameter.
    /// Length is 7 (instruction + address (2) + value (1) + CRC (2)).
    /// </summary>
    public byte[] CreateDynamixelCommandByte(byte id, byte instruction, ushort address, byte value) {
      List<byte> txBuffer = new List<byte>();
      AddHeader(txBuffer, id);
      // Length: 7 = instruction (1) + address (2) + value (1) + CRC (2)
      txBuffer.AddRange(BitConverter.GetBytes((UInt16)7));
      txBuffer.Add(instruction);
      txBuffer.AddRange(BitConverter.GetBytes(address));
      txBuffer.Add(value);
      txBuffer.AddRange(BitConverter.GetBytes(UpdateCRC(txBuffer.ToArray())));
      return txBuffer.ToArray();
    }

    /// <summary>
    /// Creates a command packet with a UInt16 value parameter.
    /// Length is 7 (instruction + address (2) + value (2) + CRC (2)).
    /// </summary>
    public byte[] CreateDynamixelCommandUInt16(byte id, byte instruction, ushort address, ushort value) {
      List<byte> txBuffer = new List<byte>();
      AddHeader(txBuffer, id);
      // Length: 7 = instruction (1) + address (2) + value (2) + CRC (2)
      txBuffer.AddRange(BitConverter.GetBytes((UInt16)7));
      txBuffer.Add(instruction);
      txBuffer.AddRange(BitConverter.GetBytes(address));
      txBuffer.AddRange(BitConverter.GetBytes(value));
      txBuffer.AddRange(BitConverter.GetBytes(UpdateCRC(txBuffer.ToArray())));
      return txBuffer.ToArray();
    }

    /// <summary>
    /// Creates a command packet with an Int32 value parameter.
    /// Length is 9 (instruction + address (2) + value (4) + CRC (2)).
    /// </summary>
    public byte[] CreateDynamixelCommandInt32(byte id, byte instruction, ushort address, int value) {
      List<byte> txBuffer = new List<byte>();
      AddHeader(txBuffer, id);
      // Length: 9 = instruction (1) + address (2) + value (4) + CRC (2)
      txBuffer.AddRange(BitConverter.GetBytes((UInt16)9));
      txBuffer.Add(instruction);
      txBuffer.AddRange(BitConverter.GetBytes(address));
      txBuffer.AddRange(BitConverter.GetBytes(value));
      txBuffer.AddRange(BitConverter.GetBytes(UpdateCRC(txBuffer.ToArray())));
      return txBuffer.ToArray();
    }
    #endregion

    #region High-Level Read Methods (Return Value)
    /// <summary>
    /// Reads the 2‑byte Current Position from the XL‑320 (Control Table address 37).
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <returns>Current Position as a UInt16 value.</returns>
    public ushort GetCurrentPosition(byte id) {
      // Build read command: instruction 2, address 37, and length 2.
      List<byte> buffer = new List<byte>();
      buffer.Add(2);         // Read instruction
      buffer.Add(37);        // Current Position address
      buffer.Add(2);         // Number of bytes to read
      byte[] command = CreateDynamixelCommand(id, 2, 37);
      // Instead, we need to append the length parameter. For a read command with parameters, 
      // we could also build it manually. Here we use CreateDynamixelCommandUInt16 overload:
      // Note: In Protocol 2.0 read command, the parameters are [address (2 bytes), data length (2 bytes)].
      command = CreateDynamixelCommandUInt16(id, 2, 37, 2);
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Current Position (Address 37).");
      byte[] parameters = ExtractParameterBytes(response);
      return BitConverter.ToUInt16(parameters, 0);
    }
    #endregion

    #region High-Level Write Methods (Set Value)
    /// <summary>
    /// Disables torque on the XL‑320 by writing 0 to Torque Enable (Control Table address 24).
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <returns>True if the command executed successfully; otherwise, false.</returns>
    public byte[] DisableTorque(byte id) {

      byte[] command = CreateDynamixelCommandByte(id, 3, 24, 0);
      return command;
    }

    /// <summary>
    /// Sets the Goal Position of the XL‑320 (Control Table address 30).
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <param name="position">Goal Position (non-negative UInt16).</param>
    /// <returns>True if the command executed successfully; otherwise, false.</returns>
    public byte[] SetGoalPosition(byte id, int position) {
      if (position < 0)
        position = 0;
      byte[] command = CreateDynamixelCommandUInt16(id, 3, 30, (ushort)position);
      return command;
    }

    /// <summary>
    /// Sets the Moving Speed of the XL‑320 (Control Table address 32).
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <param name="speed">Moving Speed (non-negative UInt16).</param>
    /// <returns>True if the command executed successfully; otherwise, false.</returns>
    public byte[] SetMovingSpeed(byte id, int speed) {
      if (speed < 0)
        speed = 0;
      byte[] command = CreateDynamixelCommandUInt16(id, 3, 32, (ushort)speed);
      return command;
    }

    /// <summary>
    /// Sets the LED state of the XL‑320 (Control Table address 25).
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <param name="status">True to turn the LED on; false to turn it off.</param>
    /// <returns>True if the command executed successfully; otherwise, false.</returns>
    public byte[] SetLED(byte id, bool status) {
      byte[] command = CreateDynamixelCommandByte(id, 3, 25, status ? (byte)1 : (byte)0);
      return command;
    }

    /// <summary>
    /// Disables the status packet response on the XL‑320 (Control Table address 17).
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <returns>True if the command executed successfully; otherwise, false.</returns>
    public byte[] SetDisableStatusPacket(byte id) {
      byte[] command = CreateDynamixelCommandByte(id, 3, 17, 1);
      return command;
    }

    /// <summary>
    /// Changes the Servo ID of the XL‑320 (Control Table address 3).
    /// </summary>
    /// <param name="fromId">Current Servo ID.</param>
    /// <param name="toId">New Servo ID.</param>
    /// <returns>True if the command executed successfully; otherwise, false.</returns>
    public byte[] ChangeID(byte fromId, byte toId) {
      byte[] command = CreateDynamixelCommandByte(fromId, 3, 3, toId);
      return command;
    }

    /// <summary>
    /// Sends a Ping command to the XL‑320.
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <returns>True if the servo responds without error; otherwise, false.</returns>
    public bool Ping(byte id) {
      byte[] command = CreateDynamixelCommand(id, 1);
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }
    #endregion
  }
}
