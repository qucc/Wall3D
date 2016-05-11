using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wall3D
{
    /// <summary>
    /// Interaction logic for Wall3DControl.xaml
    /// </summary>
    public partial class Wall3DControl : UserControl
    {
        public event EventHandler SettingChanged;
        int m_radius = 12;
        double m_angle = 0;

        int Column = -1;
        double Scale = -1;
        double Camera_Z = -1;
        double Camera_Y = -1;

        List<Tile> m_tiles = new List<Tile>();
        List<BitmapSource> m_logos;

        public Wall3DControl()
        {
            InitializeComponent();
            this.Loaded += Wall3DControl_Loaded;
        }

        private void Wall3DControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            LoadSetting();
            m_logos = LoadImages();
            InitilizeWorld();
        }

        public void InitilizeWorld()
        {
            tileGroup.Children.Clear();
            m_tiles.Clear();
            double ratio = m_logos[0].Width / m_logos[0].Height;
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = Point3DCollection.Parse("0 0 0, 1 0 0, 1 1 0, 0 1 0");
            mesh.Positions.Add(new Point3D(0, 0, 0));
            mesh.Positions.Add(new Point3D(1, 0 , 0));
            mesh.Positions.Add(new Point3D(1, 1/ratio, 0));
            mesh.Positions.Add(new Point3D(1, 1/ratio, 0));
            mesh.TriangleIndices = Int32Collection.Parse("0 1 2, 0 2 3");
            mesh.TextureCoordinates = PointCollection.Parse("0 1,1 1, 1 0, 0 0");

            double angle = 360 / Column;
            for (int i = 0; i < Column; i++)
            {
                DiffuseMaterial materail = new DiffuseMaterial();
                materail.Brush = new ImageBrush(m_logos[i % m_logos.Count]);

                DiffuseMaterial backmaterial = new DiffuseMaterial();
                backmaterial.Brush = new ImageBrush(m_logos[i % m_logos.Count]);

                GeometryModel3D tileModel3D = new GeometryModel3D();
                tileModel3D.Geometry = mesh;
                tileModel3D.Material = materail;
                tileModel3D.BackMaterial = backmaterial;

                Tile tile = new Tile();
                tile.Rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), i * angle), 0, 0, m_radius);
                tile.Scale = new ScaleTransform3D(new Vector3D(Scale,Scale,1), new Point3D(0.5, 1/ratio/2, 0));
                m_tiles.Add(tile);

                Transform3DGroup transforms = new Transform3DGroup();         
                transforms.Children.Add(tile.Scale);
                transforms.Children.Add(tile.Rotate);
                tileModel3D.Transform = transforms;
                tileGroup.Children.Add(tileModel3D);
            }

            //mirror effect
            for (int i = 0; i < Column; i++)
            {
                DiffuseMaterial materail = new DiffuseMaterial();
                Image image = new Image();
                image.RenderTransform = new ScaleTransform(1, -1);
                image.RenderTransformOrigin = new Point(0.5, 0.5);
                image.Source = m_logos[i % m_logos.Count];
                LoadImageMirror(image);
                materail.Brush = new VisualBrush(image);

                GeometryModel3D tileModel3D = new GeometryModel3D();
                tileModel3D.Geometry = mesh;
                tileModel3D.Material = materail;

                Tile tile = new Tile();
                tile.Translate = new TranslateTransform3D(0,-1,0);
                tile.Rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), i * angle), 0, 0, m_radius);
                tile.Scale = new ScaleTransform3D(new Vector3D(Scale, Scale, 1), new Point3D(0.5, 1 / ratio / 2, 0));
                m_tiles.Add(tile);

                Transform3DGroup transforms = new Transform3DGroup();
                transforms.Children.Add(tile.Translate);
                transforms.Children.Add(tile.Scale);
                transforms.Children.Add(tile.Rotate);


                tileModel3D.Transform = transforms;
                tileGroup.Children.Add(tileModel3D);
            }

        }

        public void LoadImageMirror(FrameworkElement tempControl)
        {
            GradientStopCollection gsc = new GradientStopCollection();
            GradientStop gs1 = new GradientStop();
            gs1.Color = Color.FromArgb(127, 255, 255, 255);
            gs1.Offset = 0;
            gsc.Add(gs1);
            GradientStop gs2 = new GradientStop();
            gs2.Color = Color.FromArgb(60, 255, 255, 255);
            gs2.Offset = 0.5;
            gsc.Add(gs2);
            GradientStop gs3 = new GradientStop();
            gs3.Color = Color.FromArgb(0, 255, 255, 255);
            gs3.Offset = 1;
            gsc.Add(gs3);
            
            tempControl.OpacityMask = new LinearGradientBrush(gsc, new Point(0, 1), new Point(0, 0));
        }

        private void LoadSetting()
        {
            //scale
            Scale = double.Parse(GameSetting.Scale);
            foreach (var tile in m_tiles)
            {
                tile.Scale.ScaleX = Scale;
                tile.Scale.ScaleY = Scale;
            }
            //camera position
            Camera_Y = double.Parse(GameSetting.CameraY);
            Camera_Z = double.Parse(GameSetting.CameraZ);
            camera.Position = new Point3D(0, Camera_Y, Camera_Z);

            //
            Column = int.Parse(GameSetting.Column);
            RaiseSettingChangedEvent();
        }

        public void SaveSetting()
        {
            GameSetting.Scale = Scale.ToString();
            GameSetting.CameraY = Camera_Y.ToString();
            GameSetting.CameraZ = Camera_Z.ToString();
            GameSetting.Save();
        }

        public void ResetSetting()
        {
            GameSetting.Reset();
            LoadSetting();
        }

        internal void PointLightDown()
        {
            pointLight.LinearAttenuation -= 0.1;
        }

        internal void PointLightUp()
        {
            pointLight.LinearAttenuation += 0.1;
            Console.WriteLine(pointLight.LinearAttenuation);
        }

        private List<BitmapSource> LoadImages()
        {
            List<BitmapSource> logoList = new List<BitmapSource>();
            ImageSourceConverter imageConverter = new ImageSourceConverter();
            var logos = System.IO.Path.Combine(Environment.CurrentDirectory , "logos");
            foreach (var p in Directory.GetFiles("logos").Where(f => f.EndsWith(".png") || f.EndsWith(".jpg")))
            {
                logoList.Add((BitmapSource)imageConverter.ConvertFromString(p));
            }
            return logoList;
        }

        public void MoveForward()
        {
            Camera_Z += 0.1;
            camera.Position = new Point3D(0, Camera_Y, Camera_Z);
            RaiseSettingChangedEvent();

        }

        internal void MoveBackward()
        {
            Camera_Z -= 0.1;
            camera.Position = new Point3D(0, Camera_Y, Camera_Z);
            RaiseSettingChangedEvent();

        }

        internal void MoveDown()
        {
            Camera_Y += 0.1;
            camera.Position = new Point3D(0, Camera_Y, Camera_Z);
            RaiseSettingChangedEvent();
        }

        internal void MoveUp()
        {
            Camera_Y -= 0.1;
            camera.Position = new Point3D(0, Camera_Y, Camera_Z);
            RaiseSettingChangedEvent();
        }

        public void ScaleUp()
        {
            Scale += 0.1;
            foreach (var tile in m_tiles)
            {
                tile.Scale.ScaleX = Scale;
                tile.Scale.ScaleY = Scale;
            }
            RaiseSettingChangedEvent();
        }

        public void ScaleDown()
        {
            Scale -= 0.1;
            foreach (var tile in m_tiles)
            {
                tile.Scale.ScaleX = Scale;
                tile.Scale.ScaleY = Scale;
            }
            RaiseSettingChangedEvent();
        }

        public void AddColumn()
        {
            Column += 1;
            while (360 % Column != 0)
                Column += 1;
            if (Column >= 360)
                Column = int.Parse(GameSetting.Column);
            InitilizeWorld();
            RaiseSettingChangedEvent();
        }

        public void RemoveColumn()
        {
            Column -= 1;
            while (360 % Column != 0)
                Column -= 1;
            if (Column < 10)
                Column = int.Parse(GameSetting.Column);
            InitilizeWorld();
            RaiseSettingChangedEvent();
        }

        private void Rotate(double angle)
        {
            var cameraRoate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), m_angle), 0, 0, m_radius);
            camera.Transform = cameraRoate;
            directionLight.Transform = cameraRoate;
            m_angle += angle;
            
        }

        private void Viewport3D_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = this;
            e.Handled = true;
        }

        private void Viewport3D_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            double translateX = e.DeltaManipulation.Translation.X;
          
            Rotate(translateX / 100);
            
            e.Handled = true;

        }

        private void Viewport3D_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 0.05;
            e.Handled = true;
        }

        private void Viewport3D_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {

        }

        public string SettingString
        {
            get
            {
                return "Tile Count: " + Column + Environment.NewLine +
                        "Scale: " + Scale + Environment.NewLine +
                        "Camera Position: " + "(0, " + Camera_Y + ", " + Math.Round(Camera_Z, 2) + ")" + Environment.NewLine;
            }
        }

        private void RaiseSettingChangedEvent()
        {
            if (SettingChanged != null)
                SettingChanged.Invoke(this, EventArgs.Empty);
        }
    }
}
