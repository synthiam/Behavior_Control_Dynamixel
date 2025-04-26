namespace Dynamixel {

  public class OnCommCls {

    /// <summary>
    /// Bytes that are returned from the servo
    /// </summary>
    public byte[] ResponseBytes { get; set; }

    /// <summary>
    /// Bytes that will be sent to the servo
    /// </summary>
    public byte[] SendBytes { get; set; }
  }
}
