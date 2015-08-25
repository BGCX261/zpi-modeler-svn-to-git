using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modeler.Data.Scene;

namespace Modeler.Data.Shapes
{
    class Cylinder : Shape_
    {
        private const int maxStep = 360;

        public Cylinder(string _name, string uri)
            : base(_name, uri)
        { }

        public override Scene.Scene Triangulate(float density)
        {
            // gestosc 1 - 360 krokow
            // gestosc 0 - 4 kroki
            uint step = (uint) (maxStep * density);

            if (step < 1)
                step = 1;

            float density_deg = 360.0f / step;
            List<Vector3D> vertices = new List<Vector3D>();
            List<Triangle> triangles = new List<Triangle>();

            // Tworzenie wierzchołków dolnej i górnej oraz dodawanie ich do listy
            float x, z;
            float deg = density_deg;
            for (int i = 0; i < step; i++)
            {
                x = (float)Math.Cos(Utilities.DegToRad(deg));
                z = (float)Math.Sin(Utilities.DegToRad(deg));
                deg += density_deg;
                vertices.Add(new Vector3D(x, -1, z));
                vertices.Add(new Vector3D(x, 1, z));

            }

            // Łączenie trójkątów w górnej i dolnej podstawie walca
            for (uint i = 1; i < step - 1; i++)
            {
                triangles.Add(new Triangle(1, 2 * i + 1, 2 * i + 3));
                triangles.Add(new Triangle(0, 2 * i + 2, 2 * i));
            }

            // Łączenie trójkątów między podstawami walca
            uint tmp = step + step;
            for (uint i = 0; i < step; i++)
            {
                uint k = 2 * i;
                triangles.Add(new Triangle(k, (k + 2) % tmp, (k + 1) % tmp));
                triangles.Add(new Triangle((k + 2) % tmp, (k + 3) % tmp, (k + 1) % tmp));
            }

            Scene.Scene scene = new Scene.Scene();
            scene.points = vertices;
            scene.triangles = triangles;

            return scene;
        }
    }
}
