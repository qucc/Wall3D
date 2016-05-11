using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestActivator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var objectHandle = Activator.CreateInstanceFrom("Wall3D.exe", "Wall3D.Wall3DControl");
            var wall3d = objectHandle.Unwrap() as UserControl;
            wall3d.Width = root.Width;
            wall3d.Height = root.Height;
            root.Children.Add(wall3d);
            
        }
    }
}
