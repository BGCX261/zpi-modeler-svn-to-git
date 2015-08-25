using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modeler.Data.Scene;

namespace Modeler.Data.Shapes
{
    class Cone : Shape_
    {
        private const int maxStep = 360;

        public Cone(string _name, string uri)
            : base(_name, uri)
        { }

        public override Modeler.Data.Scene.Scene Triangulate(float density)
        {
            uint step = (uint)(maxStep * density);

            float density_deg = 360.0f / step;
            List<Vector3D> vertices = new List<Vector3D>();
            List<Triangle> triangles = new List<Triangle>();

            if (step < 1)
                step = 1;

            // Tworzenie wierzchołków podstawy i dodawanie ich do listy
            float x, y, z;
            float deg = 0;
            for (int i = 0; i < step; i++)
            {
                y = -1;
                x = (float)Math.Cos(Utilities.DegToRad(deg));
                z = (float)Math.Sin(Utilities.DegToRad(deg));
                Console.WriteLine(deg);
                deg += density_deg;
                vertices.Add(new Vector3D(x, y, z));
            }
            vertices.Add(new Vector3D(0, 2, 0));

            // Łączenie trójkątów w podstawie stożka
            for (uint i = 1; i < step - 1; i++)
            {
                triangles.Add(new Triangle(0, i+1, i));
            }

            // Łączenie trójkątów między podstawą a wystającym wierzchołkiem
            for (uint i = 0; i < step; i++)
            {
                triangles.Add(new Triangle(i, (i + 1) % step, (uint)vertices.Count() - 1));
            }

            Modeler.Data.Scene.Scene scene = new Modeler.Data.Scene.Scene();
            scene.points = vertices;
            scene.triangles = triangles;

            return scene;
        }
    }
}
