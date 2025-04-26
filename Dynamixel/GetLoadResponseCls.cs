namespace Dynamixel {

  public class GetLoadResponseCls {

    public enum LoadDirectionEnum {
      Clockwise = 0,
      CounterClockwise = 1,
      NA = 99
    };

    public int Load;
    public LoadDirectionEnum LoadDirection = LoadDirectionEnum.NA;
  }
}
