using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modeler.Data.Scene;

namespace Modeler.Transformations
{
    class Transformations
    {
        public static void TranslateAddedObject(Scene scene, float x, float y, float z)
        {
            HashSet<uint> uniqueVertices = new HashSet<uint>();
            if (scene.addedObject.Count > 0)
            {
                //DateTime startSearchVert = DateTime.Now;
                foreach (HierarchyMesh obj in scene.addedObject)
                {
                    foreach (uint triagleIdx in obj.triangles)
                    {
                        uniqueVertices.Add(scene.triangles[(int)triagleIdx].p1);
                        uniqueVertices.Add(scene.triangles[(int)triagleIdx].p2);
                        uniqueVertices.Add(scene.triangles[(int)triagleIdx].p3);
                    }
                }
                //TimeSpan Elapsed = DateTime.Now - startSearchVert;

                //DateTime startTranslateVert = DateTime.Now;
                foreach (int vertIdx in uniqueVertices)
                {
                    scene.points[vertIdx].x += x;
                    scene.points[vertIdx].y += y;
                    scene.points[vertIdx].z += z;
                }
                //T/imeSpan ElapsedTranslate = DateTime.Now - startTranslateVert;

                //Console.WriteLine("Czas wyszukiwania: {0}, czas przesuwania {1}", Elapsed, ElapsedTranslate);

                scene.addedObject.RemoveRange(0, scene.addedObject.Count);
            }
        }

        public static void Translate(Scene scene, float x, float y, float z)
        {
            // Transformacja odbywa się tylko jeśli zaznaczony jest jakiś obiekt
            // tymczasowo tylko siatka trojkatow
            //List<uint> uniqueVertices = new List<uint>();
            HashSet<uint> uniqueVertices = new HashSet<uint>();
            if (scene.selTriangles.Count > 0)
            {
                //DateTime startSearchVert = DateTime.Now;
                foreach (HierarchyMesh obj in scene.selTriangles)
                {
                    foreach (uint triagleIdx in obj.triangles)
                    {
                        uniqueVertices.Add(scene.triangles[(int)triagleIdx].p1);
                        uniqueVertices.Add(scene.triangles[(int)triagleIdx].p2);
                        uniqueVertices.Add(scene.triangles[(int)triagleIdx].p3);
                    }
                }
                //TimeSpan Elapsed = DateTime.Now - startSearchVert;

                //DateTime startTranslateVert = DateTime.Now;
                foreach (int vertIdx in uniqueVertices)
                {
                    scene.points[vertIdx].x += x;
                    scene.points[vertIdx].y += y;
                    scene.points[vertIdx].z += z;
                }
                //TimeSpan ElapsedTranslate = DateTime.Now - startTranslateVert;

                //Console.WriteLine("Czas wyszukiwania: {0}, czas przesuwania {1}", Elapsed, ElapsedTranslate);
            }
            foreach (Light_ light in scene.selLights)
            {
                light.position.X += x;
                light.position.Y += y;
                light.position.Z += z;
            }
        }

        public static void Scale(Scene scene, float x, float y, float z)
        {
            List<uint> selectedTriangles = new List<uint>();

            foreach (HierarchyMesh mesh in scene.selTriangles)
            {
                selectedTriangles.AddRange(mesh.triangles);
            }

            Vector3D center = new Vector3D(0, 0, 0);

            HashSet<uint> uniquePoints = new HashSet<uint>();
            foreach (uint triangleIndex in selectedTriangles)
            {
                uint p1 = scene.triangles[(int)triangleIndex].p1;
                uint p2 = scene.triangles[(int)triangleIndex].p2;
                uint p3 = scene.triangles[(int)triangleIndex].p3;

                uniquePoints.Add(p1);
                uniquePoints.Add(p2);
                uniquePoints.Add(p3);
            }

            foreach (uint uniquePoint in uniquePoints)
            {
                center.x += scene.points[(int)uniquePoint].x;
                center.y += scene.points[(int)uniquePoint].y;
                center.z += scene.points[(int)uniquePoint].z;
            }

            center.x /= uniquePoints.Count;
            center.y /= uniquePoints.Count;
            center.z /= uniquePoints.Count;

            float factorX = 0.3f * x;
            float factorY = 0.3f * y;
            float factorZ = 0.3f * z;

            foreach (uint pointIndex in uniquePoints)
            {
                scene.points[(int)pointIndex] -= center;
                scene.points[(int)pointIndex] *= new Vector3D(1 + factorX, 1 + factorY, 1 + factorZ);
                scene.points[(int)pointIndex] += center;
            }
        }

        public static void RotateOX(Scene scene, float phi)
        {
            //List<uint> uniqueVertices = new List<uint>();
            HashSet<uint> uniqueVertices = new HashSet<uint>();
            if (scene.selTriangles.Count > 0)
            {
                foreach (HierarchyMesh obj in scene.selTriangles)
                {
                    //uint tmp;
                    foreach (uint triagleIdx in obj.triangles)
                    {
                        //tmp = scene.triangles[(int)triagleIdx].p1;
                        //if (!uniqueVertices.Contains(tmp))
                        uniqueVertices.Add(scene.triangles[(int)triagleIdx].p1);
                        //tmp = scene.triangles[(int)triagleIdx].p2;
                        //if (!uniqueVertices.Contains(tmp))
                        uniqueVertices.Add(scene.triangles[(int)triagleIdx].p2);
                        //tmp = scene.triangles[(int)triagleIdx].p3;
                        //if (!uniqueVertices.Contains(tmp))
                        uniqueVertices.Add(scene.triangles[(int)triagleIdx].p3);
                    }
                }



                //Wyznaczenie środka
                Vector3D center = new Vector3D(0, 0, 0);
                foreach (int vertIdx in uniqueVertices)
                {
                    center.x = center.x + scene.points[vertIdx].x / uniqueVertices.Count;
                    center.y = center.y + scene.points[vertIdx].y / uniqueVertices.Count;
                    center.z = center.z + scene.points[vertIdx].z / uniqueVertices.Count;
                }

                //Obrót względem środka
                foreach (int vertIdx in uniqueVertices)
                {
                    scene.points[vertIdx].z = scene.points[vertIdx].z - center.z;
                    scene.points[vertIdx].y = scene.points[vertIdx].y - center.y;
                    float temp = scene.points[vertIdx].z * (float)Math.Cos(-phi) - scene.points[vertIdx].y * (float)Math.Sin(-phi);
                    scene.points[vertIdx].y = scene.points[vertIdx].z * (float)Math.Sin(-phi) + scene.points[vertIdx].y * (float)Math.Cos(-phi);
                    scene.points[vertIdx].z = temp;
                    scene.points[vertIdx].z = scene.points[vertIdx].z + center.z;
                    scene.points[vertIdx].y = scene.points[vertIdx].y + center.y;
                }
            }
        }

        public static void RotateOY(Scene scene, float phi)
        {
            //List<uint> uniqueVertices = new List<uint>();
            HashSet<uint> uniqueVertices = new HashSet<uint>();
            if (scene.selTriangles.Count > 0)
            {
                foreach (HierarchyMesh obj in scene.selTriangles)
                {
                    //uint tmp;
                    foreach (uint triagleIdx in obj.triangles)
                    {
                        //tmp = scene.triangles[(int)triagleIdx].p1;
                        //if (!uniqueVertices.Contains(tmp))
                        uniqueVertices.Add(scene.triangles[(int)triagleIdx].p1);
                        //tmp = scene.triangles[(int)triagleIdx].p2;
                        //if (!uniqueVertices.Contains(tmp))
                        uniqueVertices.Add(scene.triangles[(int)triagleIdx].p2);
                        //tmp = scene.triangles[(int)triagleIdx].p3;
                        //if (!uniqueVertices.Contains(tmp))
                        uniqueVertices.Add(scene.triangles[(int)triagleIdx].p3);
                    }
                }



                //Wyznaczenie środka
                Vector3D center = new Vector3D(0, 0, 0);
                foreach (int vertIdx in uniqueVertices)
                {
                    center.x = center.x + scene.points[vertIdx].x / uniqueVertices.Count;
                    center.y = center.y + scene.points[vertIdx].y / uniqueVertices.Count;
                    center.z = center.z + scene.points[vertIdx].z / uniqueVertices.Count;
                }

                //Obrót względem środka
                foreach (int vertIdx in uniqueVertices)
                {
                    scene.points[vertIdx].x = scene.points[vertIdx].x - center.x;
                    scene.points[vertIdx].z = scene.points[vertIdx].z - center.z;
                    float temp = scene.points[vertIdx].x * (float)Math.Cos(-phi) - scene.points[vertIdx].z * (float)Math.Sin(-phi);
                    scene.points[vertIdx].z = scene.points[vertIdx].x * (float)Math.Sin(-phi) + scene.points[vertIdx].z * (float)Math.Cos(-phi);
                    scene.points[vertIdx].x = temp;
                    scene.points[vertIdx].x = scene.points[vertIdx].x + center.x;
                    scene.points[vertIdx].z = scene.points[vertIdx].z + center.z;
                }
            }
        }

        public static void RotateOZ(Scene scene, float phi)
        {
            //List<uint> uniqueVertices = new List<uint>();
            HashSet<uint> uniqueVertices = new HashSet<uint>();
            if (scene.selTriangles.Count > 0)
            {
                foreach (HierarchyMesh obj in scene.selTriangles)
                {
                    //uint tmp;
                    foreach (uint triagleIdx in obj.triangles)
                    {
                        //tmp = scene.triangles[(int)triagleIdx].p1;
                        //if (!uniqueVertices.Contains(tmp))
                        uniqueVertices.Add(scene.triangles[(int)triagleIdx].p1);
                        //tmp = scene.triangles[(int)triagleIdx].p2;
                        //if (!uniqueVertices.Contains(tmp))
                        uniqueVertices.Add(scene.triangles[(int)triagleIdx].p2);
                        //tmp = scene.triangles[(int)triagleIdx].p3;
                        //if (!uniqueVertices.Contains(tmp))
                        uniqueVertices.Add(scene.triangles[(int)triagleIdx].p3);
                    }
                }



                //Wyznaczenie środka
                Vector3D center = new Vector3D(0, 0, 0);
                foreach (int vertIdx in uniqueVertices)
                {
                    center.x = center.x + scene.points[vertIdx].x / uniqueVertices.Count;
                    center.y = center.y + scene.points[vertIdx].y / uniqueVertices.Count;
                    center.z = center.z + scene.points[vertIdx].z / uniqueVertices.Count;
                }

                //Obrót względem środka
                foreach (int vertIdx in uniqueVertices)
                {
                    scene.points[vertIdx].x = scene.points[vertIdx].x - center.x;
                    scene.points[vertIdx].y = scene.points[vertIdx].y - center.y;
                    float temp = scene.points[vertIdx].x * (float)Math.Cos(-phi) - scene.points[vertIdx].y * (float)Math.Sin(-phi);
                    scene.points[vertIdx].y = scene.points[vertIdx].x * (float)Math.Sin(-phi) + scene.points[vertIdx].y * (float)Math.Cos(-phi);
                    scene.points[vertIdx].x = temp;
                    scene.points[vertIdx].x = scene.points[vertIdx].x + center.x;
                    scene.points[vertIdx].y = scene.points[vertIdx].y + center.y;
                }
            }
        }
    }
}
