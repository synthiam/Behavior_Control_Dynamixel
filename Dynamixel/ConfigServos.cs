namespace Dynamixel {

  public class ConfigServos {

    public enum ServoTypeEnum {
      AX_12,
      XL_320,
      xl430_w250_t
    }

    public ConfigServosDetail [] Servos = new ConfigServosDetail[] { };

    public ConfigServosDetail GetPort(EZ_B.Servo.ServoPortEnum port) {

      foreach (var servo in Servos)
        if (servo.Port == port)
          return servo;

      return null;
    }
  }

  public class ConfigServosDetail {

    public EZ_B.Servo.ServoPortEnum Port;
    public ConfigServos.ServoTypeEnum ServoType;
    public int MaxPosition;
  }
}
