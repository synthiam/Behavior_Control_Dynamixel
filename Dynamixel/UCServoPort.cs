using System;
using System.Windows.Forms;

namespace Dynamixel {

  public partial class UCServoPort : UserControl {

    EZ_B.Servo.ServoPortEnum _port;

    public UCServoPort() {

      InitializeComponent();

      cbEnabled_CheckedChanged(null, null);

      cbVersion.BeginUpdate();
      cbVersion.Items.Clear();
      foreach (ConfigServos.ServoTypeEnum v in Enum.GetValues(typeof(ConfigServos.ServoTypeEnum)))
        cbVersion.Items.Add(v);

      cbVersion.SelectedItem = ConfigServos.ServoTypeEnum.AX_12;
      cbVersion.EndUpdate();

      cbOperatingMode.BeginUpdate();
      cbOperatingMode.Items.Clear();
      foreach (ConfigServos.OperatingModeEnum v in Enum.GetValues(typeof(ConfigServos.OperatingModeEnum)))
        cbOperatingMode.Items.Add(v);

      cbOperatingMode.SelectedItem = ConfigServos.OperatingModeEnum.Position;
      cbOperatingMode.EndUpdate();
    }

    public EZ_B.Servo.ServoPortEnum SetPort {
      get {
        return _port;
      }
      set {
        groupBox1.Text = value.ToString();
        _port = value;
      }
    }

    public bool SetEnabled {
      get {
        return cbEnabled.Checked;
      }
      set {
        cbEnabled.Checked = value;

        cbEnabled_CheckedChanged(null, null);
      }
    }

    public int SetMaxPosition {
      get {
        return Convert.ToInt32(tbMaxResolution.Text);
      }
      set {
        tbMaxResolution.Text = value.ToString();
      }
    }

    public ConfigServos.ServoTypeEnum SetVersion {
      get {
        return (ConfigServos.ServoTypeEnum)cbVersion.SelectedItem;
      }
      set {
        cbVersion.SelectedItem = value;
      }
    }

    public ConfigServos.OperatingModeEnum SetOperatingMode {
      get {
        return (ConfigServos.OperatingModeEnum)cbOperatingMode.SelectedItem;
      }
      set {
        cbOperatingMode.SelectedItem = value;
      }
    }

    private void cbEnabled_CheckedChanged(object sender, EventArgs e) {

      tbMaxResolution.Enabled = cbEnabled.Checked;
      cbVersion.Enabled = cbEnabled.Checked;
    }

    private void cbVersion_SelectedIndexChanged(object sender, EventArgs e) {

      var servoType = (ConfigServos.ServoTypeEnum)cbVersion.SelectedItem;

      if (servoType == ConfigServos.ServoTypeEnum.xl430_w250_t)
        tbMaxResolution.Text = "4095";
      else
        tbMaxResolution.Text = "1024";
    }
  }
}
