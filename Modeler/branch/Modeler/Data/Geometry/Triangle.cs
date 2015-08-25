using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modeler.Data.Geometry
{
    class Triangle
    {
        private Point vertice1;
        private Point vertice2;
        private Point vertice3;

        public Point Vertice1
        { 
            get { return vertice1; } 
            set { vertice1 = Vertice1; } 
        }

        public Point Vertice2
        {
            get { return vertice2; }
            set { vertice2 = Vertice2; }
        }

        public Point Vertice3
        {
            get { return vertice3; }
            set { vertice3 = Vertice3; }
        }

        public Triangle(Point point1, Point point2, Point point3)
        {
            vertice1 = point1;
            vertice2 = point2;
            vertice3 = point3;
        }

        public Triangle(float p1_x, float p1_y, float p1_z,
                        float p2_x, float p2_y, float p2_z,
                        float p3_x, float p3_y, float p3_z)
        {
            vertice1 = new Point(p1_x, p1_y, p1_z);
            vertice2 = new Point(p2_x, p2_y, p2_z);
            vertice3 = new Point(p3_x, p3_y, p3_z);
        }

        public override string ToString()
        {
            return "Vertice 1: " + vertice1.ToString() + "Vertice 2: " +
                    vertice2.ToString() + "Vertice 3: " + vertice3.ToString();
        }
    }
}
