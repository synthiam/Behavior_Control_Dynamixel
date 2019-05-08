using System;
using System.Collections.Generic;

namespace Dynamixel {

  public class Servo_AX12 {

    byte[] createDynamixelCommand(byte id, byte[] buffer) {

      if (buffer.Length > 255)
        throw new Exception("AX-12 Command cannot be longer than 255 bytes");

      List<byte> bList = new List<byte>();
      byte dataLength = (byte)(buffer.Length + 1);

      bList.Add(0xff);
      bList.Add(0xff);
      bList.Add(id);
      bList.Add(dataLength);

      int checksum = id;

      checksum += dataLength;

      for (int x = 0; x < buffer.Length; x++) {

        checksum += buffer[x];

        bList.Add(buffer[x]);
      }

      bList.Add((byte)(0xff - (checksum % 256)));

      return bList.ToArray();
    }

    public enum BAUD_RATES {
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

    /// <summary>
    /// Change Baud Rate
    /// </summary>
    public byte[] ChangeBaudRate(byte id, BAUD_RATES baud) {

      List<byte> buffer = new List<byte>();

      buffer.Add(3);  // Write data instruction

      buffer.Add(0x04); // Baud Rate

      buffer.Add((byte)baud);

      return createDynamixelCommand(id, buffer.ToArray());
    }

    /// <summary>
    /// Factory Default
    /// </summary>
    public byte[] ResetToFactoryDefault(byte id) {

      List<byte> buffer = new List<byte>();

      buffer.Add(0x06);  // Reset instruction

      return createDynamixelCommand(id, buffer.ToArray());
    }

    /// <summary>
    /// Return the data packet that will release a servo with the specified ID
    /// </summary>
    public byte[] ReleaseServo(byte id) {

      List<byte> buffer = new List<byte>();

      buffer.Add(3);  // Write data instruction

      buffer.Add(24); // Torque Enable

      buffer.Add(0);

      return createDynamixelCommand(id, buffer.ToArray());
    }

    /// <summary>
    /// Returns the data packet that will  move a servo with the specified id to the position
    /// </summary>
    public byte[] MoveServoCmd(byte id, int position) {

      if (position < 0)
        position = 0;

      List<byte> buffer = new List<byte>();

      buffer.Add(3);  // Write data instruction

      buffer.Add(30); // Goal Position

      buffer.AddRange(BitConverter.GetBytes((UInt16)position));

      return createDynamixelCommand(id, buffer.ToArray());
    }

    /// <summary>
    /// Returns the data packet that contains the position of the servo
    /// </summary>
    public byte[] GetCurrentPositionCmd(byte id) {

      List<byte> buffer = new List<byte>();

      buffer.Add(2);  // Read data instruction
      buffer.Add(36); // Current Position
      buffer.Add(2);  // two bytes

      return createDynamixelCommand(id, buffer.ToArray());
    }

    public byte[] GetTemp(byte id) {

      List<byte> buffer = new List<byte>();

      buffer.Add(2);  // read data
      buffer.Add(43); // read temp
      buffer.Add(1);  // 1 byte

      return createDynamixelCommand(id, buffer.ToArray());
    }

    public byte[] GetLoad(byte id) {

      List<byte> buffer = new List<byte>();

      buffer.Add(2);  // read data
      buffer.Add(40); // read temp
      buffer.Add(2);  // 1 byte

      return createDynamixelCommand(id, buffer.ToArray());
    }

    /// <summary>
    /// Return a packet that will set the speed of the servo with the id to the speed
    /// </summary>
    public byte[] ServoSpeed(byte id, int speed) {

      if (speed < 0)
        speed = 0;

      List<byte> buffer = new List<byte>();

      buffer.Add(3);  // Write data instruction

      buffer.Add(34); // Goal Position

      buffer.AddRange(BitConverter.GetBytes((UInt16)speed));

      return createDynamixelCommand(id, buffer.ToArray());
    }

    /// <summary>
    /// Change the LED status of the specified servo
    /// </summary>
    public byte[] LED(byte id, bool status) {

      List<byte> buffer = new List<byte>();

      buffer.Add(3);  // Write data instruction
      buffer.Add(25); // LED
      buffer.Add(status ? (byte)1 : (byte)0);

      return createDynamixelCommand(id, buffer.ToArray());
    }

    public byte[] DisableStatusPacket(byte id) {

      List<byte> buffer = new List<byte>();

      buffer.Add(3);  // Write data instruction
      buffer.Add(0x10);
      buffer.Add(1);

      return createDynamixelCommand(id, buffer.ToArray());
    }

    public byte[] DisableStatusPacketForAll() {

      return DisableStatusPacket(0xfe);
    }

    public byte[] SendPing(byte id) {

      List<byte> buffer = new List<byte>();

      buffer.Add(1);  // Ping

      return createDynamixelCommand(id, buffer.ToArray());
    }

    public byte[] ChangeID(byte fromId, byte toId) {

      List<byte> buffer = new List<byte>();

      buffer.Add(3);  // Write data instruction
      buffer.Add(3);  // Change ID
      buffer.Add(toId);

      return createDynamixelCommand(fromId, buffer.ToArray());
    }
  }
}
