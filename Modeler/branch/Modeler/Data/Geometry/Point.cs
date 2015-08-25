using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modeler.Data.Geometry
{
    struct Point
    {
        private float x;
        private float y;
        private float z;

        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = X;
            }
        }

        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = Y;
            }
        }

        public float Z
        {
            get
            {
                return z;
            }
            set
            {
                z = Z;
            }
        }

        public Point(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return "x: " + x + ", y: " + y + ", z: " + z + "\n";
        }
    }
}
