using System.Windows;
using System.Windows.Input;

namespace Wall3D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            wall.SettingChanged += Wall_SettingChanged;
        }

        private void Wall_SettingChanged(object sender, System.EventArgs e)
        {
            settingTxt.Content = wall.SettingString;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if(e.Key == Key.H)
            {
                setting.Visibility = setting.Visibility != Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
            }
            if(e.Key == Key.Left)
            {
                wall.MoveForward();
            }
            if(e.Key == Key.Right)
            {
                wall.MoveBackward();
            }
            if(e.Key == Key.Up)
            {
                wall.MoveUp();
            }
            if(e.Key == Key.Down)
            {
                wall.MoveDown();
            }
            if(e.Key == Key.S)
            {
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    wall.ScaleDown();
                }
                else
                {
                    wall.ScaleUp();
                }
            }
            if(e.Key == Key.C)
            {
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    wall.RemoveColumn();
                }
                else
                {
                    wall.AddColumn();
                }
            }
            
        }

        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            wall.ResetSetting();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            wall.SaveSetting();
        }
    }
}
