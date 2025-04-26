using System;
using System.Collections.Generic;

namespace Dynamixel {
  /// <summary>
  /// This class encapsulates low‑level command creation and higher‑level read/write methods
  /// for the AX‑12 servo using Dynamixel Protocol 1.
  /// </summary>
  public class Servo_AX12 {

    /// <summary>
    /// Provides common baud rate values for AX‑12 communication.
    /// </summary>
    public enum BAUD_RATES : byte {
      BAUD_1000000 = 1,
      BAUD_500000 = 3,
      BAUD_400000 = 4,
      BAUD_250000 = 7,
      BAUD_200000 = 9,
      BAUD_115200 = 16,
      BAUD_57600 = 34,
      BAUD_19200 = 103,
      BAUD_9600 = 207
    }

    #region Low‐Level Command Creation

    /// <summary>
    /// Builds the full Dynamixel command packet (including header, data, and checksum) for Protocol 1.
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <param name="buffer">Instruction plus parameter bytes.</param>
    /// <returns>Byte array containing the complete command packet.</returns>
    byte[] createDynamixelCommand(byte id, byte[] buffer) {
      if (buffer.Length > 255)
        throw new Exception("AX‑12 command cannot be longer than 255 bytes");

      List<byte> bList = new List<byte>();
      byte dataLength = (byte)(buffer.Length + 1); // instruction + parameters

      // Protocol 1 header: two 0xFF bytes, then servo ID and length.
      bList.Add(0xFF);
      bList.Add(0xFF);
      bList.Add(id);
      bList.Add(dataLength);

      // Checksum is calculated over id, length, and all instruction/parameter bytes.
      int checksum = id + dataLength;
      for (int x = 0; x < buffer.Length; x++) {
        checksum += buffer[x];
        bList.Add(buffer[x]);
      }
      // Checksum is the lowest byte of ~(sum) i.e., 0xFF minus (sum mod 256)
      bList.Add((byte)(0xFF - (checksum % 256)));

      return bList.ToArray();
    }

    /// <summary>
    /// Creates a command packet with an Int32 value parameter.
    /// </summary>
    public byte[] CreateDynamixelCommandInt32(byte id, byte instruction, byte address, Int32 value) {
      List<byte> buffer = new List<byte>();
      buffer.Add(instruction);
      buffer.Add(address);
      buffer.AddRange(BitConverter.GetBytes((Int32)value));
      return createDynamixelCommand(id, buffer.ToArray());
    }

    /// <summary>
    /// Creates a command packet with a 1‑byte parameter.
    /// </summary>
    public byte[] CreateDynamixelCommandByte(byte id, byte instruction, byte address, byte value) {
      List<byte> buffer = new List<byte>();
      buffer.Add(instruction);
      buffer.Add(address);
      buffer.Add(value);
      return createDynamixelCommand(id, buffer.ToArray());
    }

    /// <summary>
    /// Creates a command packet with a UInt16 parameter.
    /// </summary>
    public byte[] CreateDynamixelCommandUInt16(byte id, byte instruction, byte address, UInt16 value) {
      List<byte> buffer = new List<byte>();
      buffer.Add(instruction);
      buffer.Add(address);
      buffer.AddRange(BitConverter.GetBytes((UInt16)value));
      return createDynamixelCommand(id, buffer.ToArray());
    }

    #endregion

    #region Communication Helpers (Protocol 1)

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
    /// Extracts the parameter bytes from a Protocol 1 response packet.
    /// Assumes response format: [0xFF, 0xFF, id, length, error, parameters..., checksum].
    /// </summary>
    /// <param name="response">The full response packet.</param>
    /// <returns>Byte array containing only the parameter bytes.</returns>
    private byte[] ExtractParameterBytes(byte[] response) {
      if (response.Length < 6)
        throw new Exception("Invalid response length.");
      // Length field at index 3 includes error and checksum.
      int paramCount = response[3] - 2;
      byte[] parameters = new byte[paramCount];
      Array.Copy(response, 5, parameters, 0, paramCount);
      return parameters;
    }

    /// <summary>
    /// Retrieves the error code from the Protocol 1 response packet.
    /// </summary>
    /// <param name="response">The full response packet.</param>
    /// <returns>Error code as a byte.</returns>
    private byte GetError(byte[] response) {

      if (response.Length < 5)
        throw new Exception($"Nope, response was {response.Length} bytes.");

      // Error is at index 4.
      return response[4];
    }

    #endregion

    #region High‑Level Read Methods (Return Value)

    /// <summary>
    /// Reads the 2‑byte Current Position from the AX‑12 (Control Table address 36).
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <returns>Current Position as a UInt16 value.</returns>
    public ushort GetCurrentPosition(byte id) {
      // Build the read command: instruction (2), address 36, length 2.
      List<byte> buffer = new List<byte>();
      buffer.Add(2);      // Read data instruction
      buffer.Add(36);     // Current Position address
      buffer.Add(2);      // Number of bytes to read

      byte[] command = createDynamixelCommand(id, buffer.ToArray());
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Current Position (Address 36).");
      byte[] parameters = ExtractParameterBytes(response);
      return BitConverter.ToUInt16(parameters, 0);
    }

    public byte[] GetRam(byte id, byte address, byte bytesToRead) {
      List<byte> buffer = new List<byte>();
      buffer.Add(2);      // Read data instruction
      buffer.Add(address);     // Temperature address
      buffer.Add(bytesToRead);      // 1 byte to read

      byte[] command = createDynamixelCommand(id, buffer.ToArray());
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception($"Error {bytesToRead} bytes from address {address} on servo {id}");
      byte[] parameters = ExtractParameterBytes(response);
      return parameters;
    }

    /// <summary>
    /// Reads the 1‑byte Temperature from the AX‑12 (Control Table address 43).
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <returns>Temperature as a byte value.</returns>
    public byte GetTemperature(byte id) {
      List<byte> buffer = new List<byte>();
      buffer.Add(2);      // Read data instruction
      buffer.Add(43);     // Temperature address
      buffer.Add(1);      // 1 byte to read

      byte[] command = createDynamixelCommand(id, buffer.ToArray());
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Temperature (Address 43).");
      byte[] parameters = ExtractParameterBytes(response);
      return parameters[0];
    }

    /// <summary>
    /// Reads the 2‑byte Load from the AX‑12 (Control Table address 40).
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <returns>Load as a UInt16 value.</returns>
    public GetLoadResponseCls GetLoad(byte id) {
      List<byte> buffer = new List<byte>();
      buffer.Add(2);      // Read data instruction
      buffer.Add(40);     // Load address
      buffer.Add(2);      // 2 bytes to read

      byte[] command = createDynamixelCommand(id, buffer.ToArray());
      byte[] response = ExecuteCommand(command);
      if (GetError(response) != 0)
        throw new Exception("Error reading Load (Address 40).");
      byte[] parameters = ExtractParameterBytes(response);
      var resp =  BitConverter.ToUInt16(parameters, 0);

      bool dir = EZ_B.Functions.IsBitSet(resp, 10);

      int load = resp & 0x1ff;

      var respObj = new GetLoadResponseCls();
      respObj.Load = load;
      respObj.LoadDirection = dir ? GetLoadResponseCls.LoadDirectionEnum.Clockwise : GetLoadResponseCls.LoadDirectionEnum.CounterClockwise;

      return respObj;
    }

    #endregion

    #region High‑Level Write Methods (Set Value)

    /// <summary>
    /// Sets the Baud Rate of the AX‑12 (Control Table address 4) to the specified value.
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <param name="baud">Baud rate as a BAUD_RATES enum value.</param>
    /// <returns>True if the command was executed successfully; otherwise, false.</returns>
    public byte[] SetBaudRate(byte id, BAUD_RATES baud) {
      List<byte> buffer = new List<byte>();
      buffer.Add(3);      // Write data instruction
      buffer.Add(4);      // Baud Rate address
      buffer.Add((byte)baud);
      byte[] command = createDynamixelCommand(id, buffer.ToArray());
      return command;
    }

    /// <summary>
    /// Resets the AX‑12 to factory defaults.
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <returns>True if the reset command was executed successfully; otherwise, false.</returns>
    public byte[] ResetToFactoryDefaults(byte id) {
      List<byte> buffer = new List<byte>();
      buffer.Add(0x06);   // Reset instruction
      byte[] command = createDynamixelCommand(id, buffer.ToArray());
      return command;
    }

    /// <summary>
    /// Releases the servo by disabling torque (sets Torque Enable at address 24 to 0).
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <returns>True if the command executed successfully; otherwise, false.</returns>
    public byte[] ReleaseTorque(byte id) {

      List<byte> buffer = new List<byte>();
      buffer.Add(3);      // Write data instruction
      buffer.Add(24);     // Torque Enable address
      buffer.Add(0);      // Disable torque
      byte[] command = createDynamixelCommand(id, buffer.ToArray());

      return command;
    }

    /// <summary>
    /// Sets the Goal Position of the AX‑12 (Control Table address 30).
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <param name="position">Goal Position value (non‑negative UInt16).</param>
    /// <returns>True if the command executed successfully; otherwise, false.</returns>
    public byte[] SetGoalPosition(byte id, int position) {
      if (position < 0)
        position = 0;
      // Use UInt16 for position.
      byte[] command = CreateDynamixelCommandUInt16(id, 3, 30, (UInt16)position);
      return command;
    }

    /// <summary>
    /// Sets the Moving Speed of the AX‑12 (Control Table address 34).
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <param name="speed">Moving Speed value (non‑negative UInt16).</param>
    /// <returns>True if the command executed successfully; otherwise, false.</returns>
    public byte[] SetMovingSpeed(byte id, int speed) {

      if (speed < 0)
        speed = 0;
      byte[] command = CreateDynamixelCommandUInt16(id, 3, 34, (UInt16)speed);
      return command;
    }

    /// <summary>
    /// Sets the LED state of the AX‑12 (Control Table address 25).
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <param name="status">True to turn the LED on; false to turn it off.</param>
    /// <returns>True if the command executed successfully; otherwise, false.</returns>
    public byte[] SetLED(byte id, bool status) {
      byte[] command = CreateDynamixelCommandByte(id, 3, 25, status ? (byte)1 : (byte)0);
      return command;
    }

    /// <summary>
    /// Disables the status packet response for the AX‑12 (Control Table address 16 in hex, i.e. 0x10).
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <returns>True if the command executed successfully; otherwise, false.</returns>
    public byte[] SetDisableStatusPacket(byte id) {
      List<byte> buffer = new List<byte>();
      buffer.Add(3);      // Write data instruction
      buffer.Add(0x10);   // Disable Status Packet address
      buffer.Add(1);      // Disable status packet flag
      byte[] command = createDynamixelCommand(id, buffer.ToArray());
      return command;
    }

    public byte[] PingCmd(byte id) {
      List<byte> buffer = new List<byte>();
      buffer.Add(1);      // Ping instruction
      byte[] command = createDynamixelCommand(id, buffer.ToArray());
      return command;
    }

    /// <summary>
    /// Sends a Ping command to the AX‑12.
    /// </summary>
    /// <param name="id">Servo ID.</param>
    /// <returns>True if the servo responds with no error; otherwise, false.</returns>
    public bool Ping(byte id) {
      List<byte> buffer = new List<byte>();
      buffer.Add(1);      // Ping instruction
      byte[] command = createDynamixelCommand(id, buffer.ToArray());
      byte[] response = ExecuteCommand(command);
      return GetError(response) == 0;
    }

    /// <summary>
    /// Changes the Servo ID of the AX‑12 (Control Table address 3).
    /// </summary>
    /// <param name="fromId">Current Servo ID.</param>
    /// <param name="toId">New Servo ID.</param>
    /// <returns>True if the command executed successfully; otherwise, false.</returns>
    public byte[] ChangeID(byte fromId, byte toId) {
      List<byte> buffer = new List<byte>();
      buffer.Add(3);      // Write data instruction
      buffer.Add(3);      // Change ID address (AX‑12 ID is at address 3)
      buffer.Add(toId);
      byte[] command = createDynamixelCommand(fromId, buffer.ToArray());
      return command;
    }

    #endregion
  }
}
