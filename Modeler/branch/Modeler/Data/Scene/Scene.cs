using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Modeler.Graphics;
using System.Threading.Tasks;

namespace Modeler.Data.Scene
{
    partial class Scene
    {
        public List<Vector3D> points;
        public List<Triangle> triangles;
        public List<Part> parts;
        public List<Material_> materials;
        public List<String> materialAssign;
        public List<Light_> lights;
        public List<Camera> cams;
        public int activeCamera;
        public Hierarchy hierarchy;

        public List<HierarchyMesh> selTriangles;
        public List<Light_> selLights;
        public List<Camera> selCams;

        public List<HierarchyMesh> addedObject;

        public List<Vector3D> normals;

        // Przyspieszanie dzialania renderera
        public int[] indices;
        public int[] selIndices;
        public uint numIndices;
        public uint numSelIndices;
        public Vertex[] vertices;
        public List<int>[] vertexTriangle;

        public Scene()
        {
            points = new List<Vector3D>();
            triangles = new List<Triangle>();
            parts = new List<Part>();
            materials = new List<Material_>();
            materialAssign = new List<String>();
            lights = new List<Light_>();
            cams = new List<Camera>();
            hierarchy = new Hierarchy();

            selTriangles = new List<HierarchyMesh>();
            addedObject = new List<HierarchyMesh>();

            selLights = new List<Light_>();
            selCams = new List<Camera>();

            cams.Add(new Camera("Cam1", 800, 600, new Vector3(5, 2, 5), new Vector3(-1, 0, -1), 60, 0));
            activeCamera = 0;

            indices = new int[3 * triangles.Count];
            selIndices = new int[3 * triangles.Count];
            numIndices = 0;
            numSelIndices = 0;
            vertices = new Vertex[points.Count];
            vertexTriangle = new List<int>[points.Count];

            normals = new List<Vector3D>();
        }

        public Scene(Scene copy)
        {
            points = new List<Vector3D>();
            foreach (Vector3D copyElem in copy.points) points.Add(new Vector3D(copyElem));
            triangles = new List<Triangle>();
            foreach (Triangle copyElem in copy.triangles) triangles.Add(new Triangle(copyElem.p1,copyElem.p2,copyElem.p3));
            parts = new List<Part>();
            foreach (Part copyElem in copy.parts) parts.Add(new Part(copyElem.triangles));
            materials = new List<Material_>();
            foreach (Material_ copyElem in copy.materials) materials.Add(new Material_(copyElem));
            materialAssign = new List<String>(copy.materialAssign);
            lights = new List<Light_>();
            foreach (Light_ copyElem in copy.lights) lights.Add(new Light_(copyElem));
            cams = new List<Camera>(copy.cams);
            hierarchy = new Hierarchy();
            hierarchy.objects = new List<HierarchyObject>(copy.hierarchy.objects);

            selTriangles = new List<HierarchyMesh>(copy.selTriangles);
            selLights = new List<Light_>(copy.selLights);
            selCams = new List<Camera>(copy.selCams);

            addedObject = new List<HierarchyMesh>(copy.addedObject);

            normals = new List<Vector3D>(copy.normals);

            numIndices = copy.numIndices;
            numSelIndices = copy.numSelIndices;

            indices = new int[3 * copy.triangles.Count];
            selIndices = new int[3 * copy.triangles.Count];

            //for (int i = 0; i < indices.Length; i++)
            //{
            //    indices[i] = copy.indices[i];
            //    selIndices[i] = copy.selIndices[i];
            //}

            vertices = new Vertex[copy.points.Count];
            //for (int i = 0; i < vertices.Length; i++)
            //{
            //    vertices[i] = copy.vertices[i];
            //}

            vertexTriangle = new List<int>[copy.points.Count];
            Parallel.For(0, vertexTriangle.Length, index => vertexTriangle[index] = new List<int>());

            activeCamera = copy.activeCamera;
        }

        public int estimatedMemory()
        {
            int m_parts=0;
            foreach (Part part in parts) m_parts+=part.triangles.Count*4;
            return points.Count*12+triangles.Count*12+m_parts+materials.Count*60
                +materialAssign.Count*8+lights.Count*72+cams.Count*48;
        }

        public bool IsTriangleSelected(uint triangle)
        {
            bool selected = false;

            foreach(HierarchyMesh mesh in selTriangles)
            {
                foreach(uint triang in mesh.triangles)
                {
                    if(triang == triangle)
                    {
                        selected = true;
                        break;
                    }
                }

                if(selected == true)
                {
                    break;
                }
            }

            return selected;
        }
        public bool[] GetSelectedTriangles()
        {
            numSelIndices = 0;
            bool[] selected = new bool[triangles.Count];
            for(int i = 0; i < selected.Length; ++i)
            {
                selected[i] = false;
            }

            foreach(HierarchyMesh mesh in selTriangles)
            {
                for(int i = 0; i < mesh.triangles.Count; ++i)
                {
                    selected[mesh.triangles[i]] = true;
                    selIndices[numSelIndices++] = (int)triangles[(int)mesh.triangles[i]].p1;
                    selIndices[numSelIndices++] = (int)triangles[(int)mesh.triangles[i]].p3;
                    selIndices[numSelIndices++] = (int)triangles[(int)mesh.triangles[i]].p2;

                }
            }

            return selected;
        }

        public void AddObject(Scene sceneObject, string hierarchyName)
        {
            if (sceneObject != null)
            {
                uint point_idx = (uint)points.Count();
                points.AddRange(sceneObject.points);

                int triangles_idx = triangles.Count();
                foreach (Triangle triangle in sceneObject.triangles)
                {
                    triangle.p1 += point_idx;
                    triangle.p2 += point_idx;
                    triangle.p3 += point_idx;
                }

                int offset = triangles.Count;

                triangles.AddRange(sceneObject.triangles);

                // Jeśli w scenie nie ma jeszcze zadnego obiektu, tworzony zostaje
                // domyslny material.

                if (parts.Count == 0)
                {
                    materials.Add(new Material_("default", 0.6f, 0.95f, 0.5f, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1));
                }
                parts.Add(new Part(new List<int>()));
                materialAssign.Add("default");

                HierarchyMesh obj = new HierarchyMesh(hierarchyName);

                for (int i = 0; i < sceneObject.triangles.Count(); i++)
                {
                    parts[parts.Count-1].triangles.Add(triangles_idx);
                    obj.triangles.Add((uint)triangles_idx++);
                }

                hierarchy.objects.Add(obj);

                addedObject.Add(obj);

                //int offset = indices.Length;

                int vertTriangOffset = vertexTriangle.Length;

                Array.Resize<int>(ref indices, indices.Length + 3 * sceneObject.triangles.Count);
                Array.Resize<int>(ref selIndices, selIndices.Length + 3 * sceneObject.triangles.Count);
                Array.Resize<Vertex>(ref vertices, vertices.Length + sceneObject.points.Count);
                Array.Resize<List<int>>(ref vertexTriangle, vertexTriangle.Length + sceneObject.points.Count);
                Parallel.For(vertTriangOffset, vertexTriangle.Length, index => vertexTriangle[index] = new List<int>());

                //bool[] selected = GetSelectedTriangles();
                for (int i = 0; i < sceneObject.triangles.Count; i++)
                {
                    indices[numIndices++] = (int)sceneObject.triangles[i].p1;
                    indices[numIndices++] = (int)sceneObject.triangles[i].p3;
                    indices[numIndices++] = (int)sceneObject.triangles[i].p2;

                    vertexTriangle[sceneObject.triangles[i].p1].Add(offset + i);
                    vertexTriangle[sceneObject.triangles[i].p2].Add(offset + i);
                    vertexTriangle[sceneObject.triangles[i].p3].Add(offset + i);
                }
            }
        }

        public Scene SceneFromSelection()
        {
            Scene newScene = new Scene();

            bool[] pointsFilled = new bool[points.Count];
            for(int i = 0; i < pointsFilled.Length; ++i)
            {
                pointsFilled[i] = false;
            }

            List<Vector3D> newPoints = new List<Vector3D>();
            for(int i = 0; i < points.Count; ++i)
            {
                newPoints.Add(points[i]);
            }

            List<Triangle> newTriangles = new List<Triangle>();
            for(int i = 0; i < triangles.Count; ++i)
            {
                if(IsTriangleSelected((uint)i))
                {
                    newTriangles.Add(triangles[i]);

                    pointsFilled[triangles[i].p1] = true;
                    pointsFilled[triangles[i].p2] = true;
                    pointsFilled[triangles[i].p3] = true;
                }
            }

            Dictionary<int, int> pointsPositions = new Dictionary<int, int>();
            int start = 0;
            int end = pointsFilled.Length - 1;

            while(start < end)
            {
                if(pointsFilled[start] == false)
                {
                    while(pointsFilled[end] == false)
                    {
                        --end;
                    }

                    if(start < end)
                    {
                        pointsPositions.Add(start, end);
                        pointsFilled[start] = true;
                        pointsFilled[end] = false;
                    }
                }

                ++start;
            }

            for(int i = 0; i < newPoints.Count; ++i)
            {
                if(pointsPositions.ContainsKey(i))
                {
                    int value = pointsPositions[i];
                    newPoints[i] = newPoints[value];
                }
            }

            int endPos = newPoints.Count - 1;
            while(pointsFilled[endPos] == false)
            {
                newPoints.RemoveAt(endPos--);
            }

            foreach(Triangle tr in newTriangles)
            {
                if(pointsPositions.ContainsValue((int)tr.p1))
                {
                    tr.p1 = (uint)Utilities.FindFirstKeyInDictionary<int, int>(pointsPositions, (int)tr.p1);
                }
                if(pointsPositions.ContainsValue((int)tr.p2))
                {
                    tr.p2 = (uint)Utilities.FindFirstKeyInDictionary<int, int>(pointsPositions, (int)tr.p2);
                }
                if(pointsPositions.ContainsValue((int)tr.p3))
                {
                    tr.p3 = (uint)Utilities.FindFirstKeyInDictionary<int, int>(pointsPositions, (int)tr.p3);
                }
            }

            newScene.points = newPoints;
            newScene.triangles = newTriangles;

            return newScene;
        }

        public static Scene GetExampleScene()
        {
            Scene scene = new Scene();

            scene.points.Add(new Vector3D(-1, -1, 1));
            scene.points.Add(new Vector3D(1, -1, 1));
            scene.points.Add(new Vector3D(1, -1, -1));
            scene.points.Add(new Vector3D(-1, -1, -1));
            scene.points.Add(new Vector3D(-1, 1, 1));
            scene.points.Add(new Vector3D(1, 1, 1));
            scene.points.Add(new Vector3D(1, 1, -1));
            scene.points.Add(new Vector3D(-1, 1, -1));

            scene.points.Add(new Vector3D(1.5f, -0.5f, 0.5f));
            scene.points.Add(new Vector3D(3, -0.3f, 0));
            scene.points.Add(new Vector3D(2.5f, 0, -0.8f));
            scene.points.Add(new Vector3D(2, 2, -0.2f));

            scene.triangles.Add(new Triangle(0, 1, 5));
            scene.triangles.Add(new Triangle(0, 5, 4));
            scene.triangles.Add(new Triangle(1, 2, 5));
            scene.triangles.Add(new Triangle(2, 6, 5));
            scene.triangles.Add(new Triangle(2, 3, 6));
            scene.triangles.Add(new Triangle(3, 6, 7));
            scene.triangles.Add(new Triangle(3, 0, 7));
            scene.triangles.Add(new Triangle(0, 7, 4));
            scene.triangles.Add(new Triangle(0, 1, 2));
            scene.triangles.Add(new Triangle(0, 2, 3));
            scene.triangles.Add(new Triangle(4, 5, 6));
            scene.triangles.Add(new Triangle(6, 7, 4));

            scene.triangles.Add(new Triangle(8, 9, 10));
            scene.triangles.Add(new Triangle(8, 9, 11));
            scene.triangles.Add(new Triangle(9, 10, 11));
            scene.triangles.Add(new Triangle(8, 10, 11));

            List<int> part1 = new List<int>();
            part1.Add(0);
            part1.Add(1);
            part1.Add(2);
            part1.Add(3);
            part1.Add(4);
            part1.Add(5);
            part1.Add(6);
            part1.Add(7);
            part1.Add(8);
            part1.Add(9);
            part1.Add(10);
            part1.Add(11);
            scene.parts.Add(new Part(part1));

            List<int> part2 = new List<int>();
            part2.Add(12);
            part2.Add(13);
            part2.Add(14);
            part2.Add(15);
            scene.parts.Add(new Part(part2));

            scene.materials.Add(new Material_("mat1", 0.6f, 0.95f, 0.5f, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1));
            scene.materials.Add(new Material_("mat2", 0.4f, 0.3f, 0.9f, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1));

            scene.materialAssign.Add("mat1");
            scene.materialAssign.Add("mat2");

            scene.cams[0] = new Camera("Cam1", 800, 600, new Vector3(-8, -4, -10), new Vector3(3, 2, 2), 60, 0);

            scene.activeCamera = 0;

            HierarchyMesh cube = new HierarchyMesh("Cube");
            cube.triangles.AddRange(new uint[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 });

            HierarchyMesh pyramid = new HierarchyMesh("Pyramid");
            pyramid.triangles.AddRange(new uint[] { 12, 13, 14, 15 });

            scene.hierarchy.objects.AddRange( new HierarchyMesh[] { cube, pyramid } );

            return scene;
        }
    }
}