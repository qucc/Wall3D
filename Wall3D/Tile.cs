using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Wall3D
{
    public class Tile
    {
        public TranslateTransform3D Translate { get; set; }
        public ScaleTransform3D Scale { get; set; }
        public RotateTransform3D Rotate { get; set; }
    }
}
