#define MEASURE_TIMES

using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;
using Modeler.Data.Scene;
using System.Windows.Interop;
using System.IO;
using System.Threading.Tasks;

namespace Modeler.Graphics
{
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public int Color;
        public float tex0, tex1;
    }

    struct ViewportInfo
    {
        public int resX, resY;

        public int[] posX, posY;
        public int[] sizeX, sizeY;

        public ViewportInfo(int resX, int resY, int[] posX, int[] posY, int[] sizeX, int[] sizeY)
        {
            this.resX = resX;
            this.resY = resY;
            this.posX = posX;
            this.posY = posY;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
        }
    }

    class Renderer
    {
#if MEASURE_TIMES
        StreamWriter w = File.AppendText("times3.txt");
#endif
        IntPtr handle;
        IntPtr handleBezier;

        PresentParameters pp;
        Device device;
        Direct3D d3d;
        Device deviceBezier;
        Direct3D d3dBezier;

        Viewport perspective;
        Viewport top;
        Viewport front;
        Viewport side;

        VertexElement[] vertexElems;

        Light defLight;

        float[] orthoWidht;
        Vector3[] orthoPos;
        Vector3[] orthoLookAt;

        //Texture selectionTex;

        Random r;

        public float[] OrthoWidth
        {
            get
            {
                return orthoWidht;
            }
        }

        public Vector3[] OrthoPos
        {
            get
            {
                return orthoPos;
            }
        }

        public Vector3[] OrthoLookAt
        {
            get
            {
                return orthoLookAt;
            }
        }

        private bool wireframe;

        public void ChangeWireframe()
        {
            wireframe = !wireframe;
        }

        public Renderer(IntPtr handle, IntPtr handleBezier)
        {
            this.handle = handle;
            this.handleBezier = handleBezier;

            r = new Random();

            orthoWidht = new float[] { 10, 10, 10 };
            orthoPos = new Vector3[] { new Vector3(0, 10000, 0), new Vector3(0, 0, 10000), new Vector3(10000, 0, 0) };
            orthoLookAt = new Vector3[] { new Vector3(0, 0, -0.01f), new Vector3(0, 0, 0), new Vector3(0, 0, 0) };

            perspective = new Viewport();
            perspective.X = 0;
            perspective.Y = 0;
            perspective.Width = 1;
            perspective.Height = 1;
            perspective.MinZ = -1000;
            perspective.MaxZ = 1000;

            top = new Viewport();
            top.X = 0;
            top.Y = 0;
            top.Width = 1;
            top.Height = 1;
            top.MinZ = -1000;
            top.MaxZ = 1000;

            front = new Viewport();
            front.X = 0;
            front.Y = 0;
            front.Width = 1;
            front.Height = 1;
            front.MinZ = -1000;
            front.MaxZ = 1000;

            side = new Viewport();
            side.X = 0;
            side.Y = 0;
            side.Width = 1;
            side.Height = 1;
            side.MinZ = -1000;
            side.MaxZ = 1000;

            defLight = new Light();
            defLight.Type = LightType.Directional;
            defLight.Diffuse = Color.White;
            defLight.Direction = new Vector3(1, -1, -2);

            vertexElems = new[] {
        		new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
        		new VertexElement(0, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
                new VertexElement(0, 24, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                new VertexElement(0, 28, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
				VertexElement.VertexDeclarationEnd
        	};

            wireframe = true;
            pp = new PresentParameters();
            pp.SwapEffect = SwapEffect.Discard;
            pp.Windowed = true;
            pp.BackBufferFormat = Format.A8R8G8B8;
        }

        public void RenderViews(ViewportInfo viewportInfo, Scene scene)
        {
#if MEASURE_TIMES
            DateTime ts = DateTime.Now;

            DateTime t1 = DateTime.Now;
#endif
            if (pp.BackBufferWidth != viewportInfo.resX || pp.BackBufferHeight != viewportInfo.resY)
            {
                pp = new PresentParameters();
                pp.SwapEffect = SwapEffect.Discard;
                pp.Windowed = true;
                pp.BackBufferWidth = viewportInfo.resX;
                pp.BackBufferHeight = viewportInfo.resY;
                pp.BackBufferFormat = Format.A8R8G8B8;

                if (d3d != null)
                {
                    d3d.Dispose();
                }
                if (device != null)
                {
                    device.Dispose();
                }

                d3d = new Direct3D();
                device = new Device(d3d, 0, DeviceType.Hardware, handle, CreateFlags.HardwareVertexProcessing, pp);
            }
#if MEASURE_TIMES
            DateTime t2 = DateTime.Now;
            TimeSpan t = t2 - t1;
            w.WriteLine("Tworzenie device'a                          " + t.Milliseconds);
#endif

            //selectionTex = Texture.FromFile(device, "..\\..\\selectionTex.png");

#if MEASURE_TIMES
            t1 = DateTime.Now;
            device.SetRenderState(RenderState.Lighting, true);
#endif

            int l = 0;
            if (scene.lights.Count == 0)
            {
                device.SetLight(0, defLight);
                device.EnableLight(0, true);
                device.EnableLight(1, false);
            }
            else
            {
                foreach (Light_ light_ in scene.lights)
                {
                    Light light = new Light();
                    light.Diffuse = new Color4(light_.colorR, light_.colorG, light_.colorB);
                    light.Position = light_.position;
                    light.Range = 100;
                    light.Attenuation0 = 10.0f / light_.power;
                    if (light_.type==Light_Type.Point) light.Type = LightType.Point;
                    else light.Type = LightType.Spot;
                    light.Direction = light_.direction;
                    device.SetLight(l, light);
                    device.EnableLight(l, true);
                    l++;
                }
                device.EnableLight(l + 1, false);
            }
#if MEASURE_TIMES
            t2 = DateTime.Now;
            t = t2 - t1;
            w.WriteLine("Tworzenie świateł                           " + t.Milliseconds);

            t1 = DateTime.Now;
#endif
            device.SetRenderState(RenderState.FillMode, wireframe ? FillMode.Wireframe : FillMode.Solid);
            device.SetRenderState(RenderState.CullMode, Cull.Clockwise);
            device.SetRenderState(RenderState.ShadeMode, ShadeMode.Flat);
#if MEASURE_TIMES
            t2 = DateTime.Now;
            t = t2 - t1;
            w.WriteLine("SetRenderState                              " + t.Milliseconds);

            t1 = DateTime.Now;
#endif
            //Vertex[] vertices = new Vertex[scene.points.Count];
            //List<int>[] vertexTriangle = new List<int>[scene.points.Count];
            //Parallel.For(0, vertexTriangle.Length, index => vertexTriangle[index] = new List<int>()); 
            ///*for(int i = 0; i < vertexTriangle.Length; ++i)
            //{
            //    vertexTriangle[i] = new List<int>();
            //}*/

            //int[] indices = new int[3 * scene.triangles.Count];
            //int[] selIndices = new int[3 * scene.triangles.Count];
            //uint numIndices = 0;
            //uint numSelIndices = 0;
            
            bool[] selPoints = new bool[scene.points.Count];
            Parallel.For(0, selPoints.Length, index => selPoints[index] = false); 
            /*for(int i = 0; i < selPoints.Length; ++i)
            {
                selPoints[i] = false;
            }*/
            bool[] selected = scene.GetSelectedTriangles();
#if MEASURE_TIMES
            t2 = DateTime.Now;
            t = t2 - t1;
            w.WriteLine("t1                          " + t.Milliseconds);
            t1 = DateTime.Now;
#endif
            for (int i = 0; i < scene.triangles.Count; ++i)
            {
                Triangle triangle = scene.triangles[i];

                //if (selected[i] == false)
                //{
                //    scene.indices[numIndices] = (int)triangle.p1;
                //    scene.indices[numIndices + 1] = (int)triangle.p3;
                //    scene.indices[numIndices + 2] = (int)triangle.p2;

                //    numIndices += 3;
                //}
                //else
                if (selected[i])
                {
                    //scene.selIndices[numSelIndices] = (int)triangle.p1;
                    //scene.selIndices[numSelIndices + 1] = (int)triangle.p3;
                    //scene.selIndices[numSelIndices + 2] = (int)triangle.p2;

                    selPoints[triangle.p1] = true;
                    selPoints[triangle.p2] = true;
                    selPoints[triangle.p3] = true;

                    //numSelIndices += 3;
                }

                //scene.vertexTriangle[triangle.p1].Add(i);
                //scene.vertexTriangle[triangle.p2].Add(i);
                //scene.vertexTriangle[triangle.p3].Add(i);
            }
#if MEASURE_TIMES
            t2 = DateTime.Now;
            t = t2 - t1;
            w.WriteLine("t2            " + t.Milliseconds);

            t1 = DateTime.Now;
#endif
            for(int i = 0; i < scene.points.Count; ++i)
            {
                if(selPoints[i] || scene.normals.Count <= i)
                {
                    Vector3D normal = new Vector3D();

                    foreach(int face in scene.vertexTriangle[i])
                    {
                        normal += Utilities.CalculateNormal(scene.points[(int)scene.triangles[face].p3], scene.points[(int)scene.triangles[face].p2],
                            scene.points[(int)scene.triangles[face].p1]);
                    }

                    normal.Normalize();
                    if(scene.normals.Count <= i)
                    {
                        scene.normals.Add(normal);
                    }
                    else
                    {
                        scene.normals[i] = normal;
                    }
                }
            }
#if MEASURE_TIMES
            t2 = DateTime.Now;
            t = t2 - t1;
            w.WriteLine("Liczenie normalnych                         " + t.Milliseconds);

            t1 = DateTime.Now;
#endif
            Dictionary<string, int> matNames = new Dictionary<string,int>();
            for(int i = 0; i < scene.materials.Count; ++i)
            {
                matNames.Add(scene.materials[i].name, i);
            }

            int[] trPart = new int[scene.triangles.Count];
            for(int i = 0; i < scene.parts.Count; ++i)
            {
                for(int j = 0; j < scene.parts[i].triangles.Count; ++j)
                {
                    trPart[scene.parts[i].triangles[j]] = i;
                }
            }

            for (int i = 0; i < scene.vertices.Length; ++i)
            {
                int partIndex = trPart[scene.vertexTriangle[i][0]];

                String matName = scene.materialAssign[partIndex];
                Material_ material = scene.materials[matNames[matName]];

                Vector3D point = scene.points[i];

                scene.vertices[i].Position = new Vector3(point.x, point.y, point.z);
                scene.vertices[i].Normal = new Vector3(scene.normals[i].x, scene.normals[i].y, scene.normals[i].z);
                scene.vertices[i].Color = Color.FromArgb((int)(255 * material.colorR), (int)(255 * material.colorG), (int)(255 * material.colorB)).ToArgb();
                scene.vertices[i].tex0 = 0;
                scene.vertices[i].tex1 = 0;
            }
#if MEASURE_TIMES
            t2 = DateTime.Now;
            t = t2 - t1;
            w.WriteLine("Znajdowanie części i przyp. wierz.            " + t.Milliseconds);

            t1 = DateTime.Now;
#endif
            Mesh mesh = scene.numIndices >= 3 ? new Mesh(device, (int)scene.numIndices / 3, scene.points.Count, MeshFlags.Managed | MeshFlags.Use32Bit, vertexElems) : null;
            VertexBuffer vb = mesh != null ? mesh.VertexBuffer : null;
            IndexBuffer ib = mesh != null ? mesh.IndexBuffer : null;
#if MEASURE_TIMES
            t2 = DateTime.Now;
            t = t2 - t1;
            w.WriteLine("Tworzenie mesh1                             " + t.Milliseconds);

            t1 = DateTime.Now;
#endif
            if (mesh != null)
            {
                vb.Lock(0, 0, LockFlags.None).WriteRange(scene.vertices);
                vb.Unlock();

                ib.Lock(0, 0, LockFlags.None).WriteRange(scene.indices, 0, (int)scene.numIndices);
                ib.Unlock();
#if MEASURE_TIMES
                t2 = DateTime.Now;
                t = t2 - t1;
                w.WriteLine("Kopiowanie buforów mesh1                    " + t.Milliseconds);
#endif
            }

#if MEASURE_TIMES
            t1 = DateTime.Now;
#endif
            Mesh selMesh = scene.numSelIndices >= 3 ? new Mesh(device, (int)scene.numSelIndices / 3, scene.points.Count, MeshFlags.Managed | MeshFlags.Use32Bit, vertexElems) : null;
            VertexBuffer selvb = selMesh != null ? selMesh.VertexBuffer : null;
            IndexBuffer selib = selMesh != null ? selMesh.IndexBuffer : null;
#if MEASURE_TIMES
            t2 = DateTime.Now;
            t = t2 - t1;
            w.WriteLine("Tworzenie mesh2                              " + t.Milliseconds);

            t1 = DateTime.Now;
#endif
            if (selMesh != null)
            {
                selvb.Lock(0, 0, LockFlags.None).WriteRange(scene.vertices);
                selvb.Unlock();

                selib.Lock(0, 0, LockFlags.None).WriteRange(scene.selIndices, 0, (int)scene.numSelIndices);
                selib.Unlock();
#if MEASURE_TIMES
                t2 = DateTime.Now;
                t = t2 - t1;
                w.WriteLine("Kopiowanie buforów mesh2                    " + t.Milliseconds);
#endif
            }

#if MEASURE_TIMES
            t1 = DateTime.Now;
#endif
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            if (perspective.X != viewportInfo.posX[0] || perspective.Y != viewportInfo.posY[0] || perspective.Width != viewportInfo.sizeX[0] ||
                perspective.Height != viewportInfo.sizeY[0])
            {
                perspective = new Viewport();
                perspective.X = viewportInfo.posX[0];
                perspective.Y = viewportInfo.posY[0];
                perspective.Width = viewportInfo.sizeX[0];
                perspective.Height = viewportInfo.sizeY[0];
                perspective.MinZ = 0;
                perspective.MaxZ = 1;
            }

            if (top.X != viewportInfo.posX[1] || top.Y != viewportInfo.posY[1] || top.Width != viewportInfo.sizeX[1] ||
                top.Height != viewportInfo.sizeY[1])
            {
                top = new Viewport();
                top.X = viewportInfo.posX[1];
                top.Y = viewportInfo.posY[1];
                top.Width = viewportInfo.sizeX[1];
                top.Height = viewportInfo.sizeY[1];
                top.MinZ = 0;
                top.MaxZ = 1;
            }

            if (front.X != viewportInfo.posX[2] || front.Y != viewportInfo.posY[2] || front.Width != viewportInfo.sizeX[2] ||
                front.Height != viewportInfo.sizeY[2])
            {
                front = new Viewport();
                front.X = viewportInfo.posX[2];
                front.Y = viewportInfo.posY[2];
                front.Width = viewportInfo.sizeX[2];
                front.Height = viewportInfo.sizeY[2];
                front.MinZ = 0;
                front.MaxZ = 1;
            }

            if (side.X != viewportInfo.posX[3] || side.Y != viewportInfo.posY[3] || side.Width != viewportInfo.sizeX[3] ||
                side.Height != viewportInfo.sizeY[3])
            {
                side = new Viewport();
                side.X = viewportInfo.posX[3];
                side.Y = viewportInfo.posY[3];
                side.Width = viewportInfo.sizeX[3];
                side.Height = viewportInfo.sizeY[3];
                side.MinZ = 0;
                side.MaxZ = 1;
            }
#if MEASURE_TIMES
            t2 = DateTime.Now;
            t = t2 - t1;
            w.WriteLine("Modyfikacja viewport'ów                     " + t.Milliseconds);

            t1 = DateTime.Now;
#endif
            if (perspective.Width > 0 && perspective.Height > 0)
            {
                device.Viewport = perspective;

                float camRotAngle = scene.cams.ElementAt(scene.activeCamera).rotateAngle;
                float aspect = (float)perspective.Width / perspective.Height;
                float angle = 2.0f * (float)Math.Atan(Math.Tan(Utilities.DegToRad(scene.cams.ElementAt(scene.activeCamera).fovAngle) / 2.0f) / aspect);

                device.SetTransform(TransformState.View, Matrix.LookAtRH(
                    scene.cams[scene.activeCamera].position,
                    scene.cams[scene.activeCamera].lookAt,
                    Utilities.RotatePointAroundVector(new Vector3(0, 1, 0),
                    Vector3.Normalize(scene.cams[scene.activeCamera].lookAt - scene.cams[scene.activeCamera].position), camRotAngle)));

                device.SetTransform(TransformState.Projection, Matrix.PerspectiveFovRH(
                    angle,
                    aspect,
                    0.01f,
                    20000));

                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb(230, 230, 230), 1.0f, 0);
                device.BeginScene();

                if (mesh != null)
                {
                    mesh.DrawSubset(0);
                }

                //device.SetTexture(0, selectionTex);
                device.SetRenderState(RenderState.Lighting, false);
                if (selMesh != null)
                {
                    selMesh.DrawSubset(0);
                }
                device.SetRenderState(RenderState.Lighting, true);
                //device.SetTexture(0, null);

                device.EndScene();
            }

            if (top.Width > 0 && top.Height > 0)
            {
                device.Viewport = top;

                device.SetTransform(TransformState.View, Matrix.LookAtRH(
                   orthoPos[0],
                   orthoLookAt[0],
                   new Vector3(0, 1, 0)));

                device.SetTransform(TransformState.Projection, Matrix.OrthoRH(
                    orthoWidht[0],
                    (float)(top.Height) / top.Width * orthoWidht[0],
                    0.01f,
                    20000));

                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb(230, 230, 230), 1.0f, 0);
                device.BeginScene();

                if (mesh != null)
                {
                    mesh.DrawSubset(0);
                }

                //device.SetTexture(0, selectionTex);
                device.SetRenderState(RenderState.Lighting, false);
                if (selMesh != null)
                {
                    selMesh.DrawSubset(0);
                }
                device.SetRenderState(RenderState.Lighting, true);
                //device.SetTexture(0, null);

                device.EndScene();
            }

            if (front.Width > 0 && front.Height > 0)
            {
                device.Viewport = front;

                device.SetTransform(TransformState.View, Matrix.LookAtRH(
                   orthoPos[1],
                   orthoLookAt[1],
                   new Vector3(0, 1, 0)));

                device.SetTransform(TransformState.Projection, Matrix.OrthoRH(
                    orthoWidht[1],
                    (float)(front.Height) / front.Width * orthoWidht[1],
                    0.01f,
                    20000));

                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb(230, 230, 230), 1.0f, 0);
                device.BeginScene();

                if (mesh != null)
                {
                    mesh.DrawSubset(0);
                }

                //device.SetTexture(0, selectionTex);
                device.SetRenderState(RenderState.Lighting, false);
                if (selMesh != null)
                {
                    selMesh.DrawSubset(0);
                }
                device.SetRenderState(RenderState.Lighting, true);
                //device.SetTexture(0, null);

                device.EndScene();
            }

            if (side.Width > 0 && side.Height > 0)
            {
                device.Viewport = side;

                device.SetTransform(TransformState.View, Matrix.LookAtRH(
                   orthoPos[2],
                   orthoLookAt[2],
                   new Vector3(0, 1, 0)));

                device.SetTransform(TransformState.Projection, Matrix.OrthoRH(
                    orthoWidht[2],
                    (float)(side.Height) / side.Width * orthoWidht[2],
                    0.01f,
                    20000));

                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb(230, 230, 230), 1.0f, 0);
                device.BeginScene();

                if (mesh != null)
                {
                    mesh.DrawSubset(0);
                }

                //device.SetTexture(0, selectionTex);
                device.SetRenderState(RenderState.Lighting, false);
                if (selMesh != null)
                {
                    selMesh.DrawSubset(0);
                }
                device.SetRenderState(RenderState.Lighting, true);
                //device.SetTexture(0, null);

                device.EndScene();
            }
#if MEASURE_TIMES
            t2 = DateTime.Now;
            t = t2 - t1;
            w.WriteLine("Renderowanie                               " + t.Milliseconds);

            t1 = DateTime.Now;
#endif
            device.Present();
#if MEASURE_TIMES
            t2 = DateTime.Now;
            t = t2 - t1;
            w.WriteLine("Present                                    " + t.Milliseconds);

            t1 = DateTime.Now;
#endif
            //selectionTex.Dispose();
            if (selMesh != null)
            {
                selvb.Dispose();
                selib.Dispose();
                selMesh.Dispose();
            }
            if (mesh != null)
            {
                vb.Dispose();
                ib.Dispose();
                mesh.Dispose();
            }
#if MEASURE_TIMES
            t2 = DateTime.Now;
            t = t2 - t1;
            w.WriteLine("Czyszczenie                              " + t.Milliseconds);

            DateTime te = DateTime.Now;
            t = te - ts;
            w.WriteLine("Całość                             " + t.Milliseconds);
            w.WriteLine("-----------------------------------------------------------------");
            w.WriteLine();
            w.WriteLine();
            w.WriteLine();
#endif
        }

        public void RenderBezier(ViewportInfo viewportInfo)
        {
            pp = new PresentParameters();
            pp.SwapEffect = SwapEffect.Discard;
            pp.Windowed = true;
            pp.BackBufferWidth = viewportInfo.resX;
            pp.BackBufferHeight = viewportInfo.resY;
            pp.BackBufferFormat = Format.A8R8G8B8;

            d3dBezier = new Direct3D();

            deviceBezier = new Device(d3dBezier, 0, DeviceType.Hardware, handleBezier, CreateFlags.HardwareVertexProcessing, pp);

            deviceBezier.SetRenderState(RenderState.Lighting, true);

            deviceBezier.SetLight(0, defLight);
            deviceBezier.EnableLight(0, true);

            deviceBezier.SetRenderState(RenderState.FillMode, wireframe ? 2 : 0);
            deviceBezier.SetRenderState(RenderState.CullMode, 1);

            deviceBezier.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb(230, 230, 230), 1.0f, 0);

            deviceBezier.Present();

            d3dBezier.Dispose();
            deviceBezier.Dispose();
        }
    }
}
