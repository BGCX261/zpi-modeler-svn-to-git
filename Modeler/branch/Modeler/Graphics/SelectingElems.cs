using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SlimDX;
using Modeler.Data.Scene;

namespace Modeler.Graphics
{
    enum ViewportType { Perspective, Orto }

    class Triang
    {
        public Vector3D p1, p2, p3;

        public Triang(Vector3D p1, Vector3D p2, Vector3D p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }
    }

    static class SelectingElems
    {
        private static void CalcCoordsImpl(Vector2 orthoSize, Vector3 orthoPos, Vector3 orthoLookAt, out Vector3 upLeft, out Vector3 upRight, out Vector3 loLeft, out Vector3 loRight)
        {
            float dx = orthoLookAt.X - orthoPos.X;
            float dy = orthoLookAt.Y - orthoPos.Y;
            float dz = orthoLookAt.Z - orthoPos.Z;
            float d = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
            float D = (float)Math.Sqrt(dx * dx + dz * dz);

            float sX = orthoSize.X;
            float sY = orthoSize.Y;

            float dy1 = sY * D / (2.0f * d);
            float dx1 = (dx * dy * sY) / (2.0f * d * D);
            float dz1 = (dz * dy * sY) / (2.0f * d * D);
            float dx2 = (dz * sX) / (2.0f * D);
            float dz2 = (dx * sX) / (2.0f * D);

            upLeft = new Vector3(orthoLookAt.X - dx1 + dx2, orthoLookAt.Y + dy1, orthoLookAt.Z - dz1 - dz2);
            upRight = new Vector3(orthoLookAt.X - dx1 - dx2, orthoLookAt.Y + dy1, orthoLookAt.Z - dz1 + dz2);
            loLeft = new Vector3(orthoLookAt.X + dx1 + dx2, orthoLookAt.Y - dy1, orthoLookAt.Z + dz1 - dz2);
            loRight = new Vector3(orthoLookAt.X + dx1 - dx2, orthoLookAt.Y - dy1, orthoLookAt.Z + dz1 + dz2);
        }

        private static void CalcPerspCoords(Point pos, Point size, float fovAngle, float rotateAngle, Vector3 perspPos, Vector3 perspLookAt, out Vector3 outCamPos, out Vector3 outSurfPos)
        {
            Vector3 oldLookAt = new Vector3(perspLookAt.X, perspLookAt.Y, perspLookAt.Z);
            perspPos -= perspLookAt;
            perspLookAt -= perspLookAt;

            float dx = perspLookAt.X - perspPos.X;
            float dy = perspLookAt.Y - perspPos.Y;
            float dz = perspLookAt.Z - perspPos.Z;
            float d = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
            float rectSizeX = 2.0f * d * (float)Math.Tan(Utilities.DegToRad(fovAngle) / 2.0f);
            float rectSizeY = (float)(rectSizeX * size.Y) / size.X;

            Vector3 upLeft, upRight, loLeft, loRight;
            CalcCoordsImpl(new Vector2(rectSizeX, rectSizeY), perspPos, perspLookAt, out upLeft, out upRight, out loLeft, out loRight);

            float scaleX = (float)pos.X / size.X;
            float scaleY = (float)pos.Y / size.Y;
            float rectDX = (scaleX - 0.5f) * rectSizeX;
            float rectDY = (scaleY - 0.5f) * rectSizeY;

            float alpha = Utilities.DegToRad(rotateAngle);
            float rotatedDX = rectDX * (float)Math.Cos(alpha) - rectDY * (float)Math.Sin(alpha);
            float rotatedDY = rectDX * (float)Math.Sin(alpha) + rectDY * (float)Math.Cos(alpha);

            scaleX = rotatedDX / rectSizeX + 0.5f;
            scaleY = rotatedDY / rectSizeY + 0.5f;

            float resX = upLeft.X + (upRight.X - upLeft.X) * scaleX + (loLeft.X - upLeft.X) * scaleY;
            float resY = upLeft.Y - (upLeft.Y - loLeft.Y) * scaleY;
            float resZ = upLeft.Z + (upRight.Z - upLeft.Z) * scaleX + (loLeft.Z - upLeft.Z) * scaleY;

            perspLookAt += oldLookAt;
            perspPos += oldLookAt;

            outCamPos = new Vector3(perspPos.X, perspPos.Y, perspPos.Z);
            outSurfPos = new Vector3(perspLookAt.X + resX, perspLookAt.Y + resY, perspLookAt.Z + resZ);
        }

        private static void CalcOrthoCoords(Point pos, Point size, Vector2 orthoSize, Vector3 orthoPos, Vector3 orthoLookAt, out Vector3 outCamPos, out Vector3 outSurfPos)
        {
            Vector3 oldLookAt = new Vector3(orthoLookAt.X,orthoLookAt.Y,orthoLookAt.Z);
            orthoPos -= orthoLookAt;
            orthoLookAt -= orthoLookAt;

            Vector3 upLeft, upRight, loLeft, loRight;
            CalcCoordsImpl(orthoSize, orthoPos, orthoLookAt, out upLeft, out upRight, out loLeft, out loRight);

            float scaleX = (float)pos.X / size.X;
            float scaleY = (float)pos.Y / size.Y;

            float resX = upLeft.X + (upRight.X - upLeft.X) * scaleX + (loLeft.X - upLeft.X) * scaleY;
            float resY = upLeft.Y - (upLeft.Y - loLeft.Y) * scaleY;
            float resZ = upLeft.Z + (upRight.Z - upLeft.Z) * scaleX + (loLeft.Z - upLeft.Z) * scaleY;

            orthoLookAt += oldLookAt;
            orthoPos += oldLookAt;

            outCamPos = new Vector3(orthoPos.X + resX, orthoPos.Y + resY, orthoPos.Z + resZ);
            outSurfPos = new Vector3(orthoLookAt.X + resX, orthoLookAt.Y + resY, orthoLookAt.Z + resZ);
        }

        private static HierarchyMesh GetSelectedMesh(List<HierarchyObject> objects, int foundTriangle)
        {
            foreach(HierarchyObject hObject in objects)
            {
                if(hObject is HierarchyMesh)
                {
                    foreach(uint tr in ((HierarchyMesh)hObject).triangles)
                    {
                        if(tr == foundTriangle)
                        {
                            return (HierarchyMesh)hObject;
                        }
                    }
                }
                else if(hObject is HierarchyNode)
                {
                    foreach(HierarchyObject subObject in ((HierarchyNode)hObject).hObjects)
                    {
                        HierarchyMesh mesh = GetSelectedMesh(((HierarchyNode)subObject).hObjects, foundTriangle);
                        
                        if(mesh != null)
                        {
                            return mesh;
                        }
                    }
                }
            }

            return null;
        }

        public static void SelectElems(Scene scene, ViewportType viewportType, Point pos, Point size, Vector2 orthoSize, Vector3 orthoPos, Vector3 orthoLookAt)
        {
            Vector3 outCamPos = new Vector3(), outSurfPos = new Vector3();
            
            switch(viewportType)
            {
                case ViewportType.Perspective:
                    CalcPerspCoords(pos, size, scene.cams.ElementAt(scene.activeCamera).fovAngle, scene.cams.ElementAt(scene.activeCamera).rotateAngle,
                scene.cams.ElementAt(scene.activeCamera).position, scene.cams.ElementAt(scene.activeCamera).lookAt, out outCamPos, out outSurfPos);
                    break;

                case ViewportType.Orto:
                    CalcOrthoCoords(pos, size, orthoSize, orthoPos, orthoLookAt, out outCamPos, out outSurfPos);
                    break;
            }

            List<Triang> triangs = new List<Triang>();
            for(int i = 0; i < scene.triangles.Count; ++i)
            {
                Triangle triangle = scene.triangles.ElementAt(i);
                triangs.Add(new Triang(scene.points.ElementAt((int)triangle.p1), scene.points.ElementAt((int)triangle.p2), scene.points.ElementAt((int)triangle.p3)));
            }

            const float lightDiameter = 1;
            const float cameraDiameter = 1;

            Vector3 rayDir = Vector3.Normalize(outSurfPos - outCamPos);

            SlimDX.Ray ray = new SlimDX.Ray(outCamPos + 0.01f * rayDir, rayDir);

            float[] triangleDist = new float[triangs.Count];

            for(int i = 0; i < triangs.Count; ++i)
            {
                float dist;

                if(SlimDX.Ray.Intersects(ray, triangs[i].p1, triangs[i].p2, triangs[i].p3, out dist))
                {
                    triangleDist[i] = dist;
                }
                else
                {
                    triangleDist[i] = -1;
                }
            }

            float minDist = float.PositiveInfinity;
            int minIndex = -1;

            for(int i = 0; i < triangleDist.Length; ++i)
            {
                if(triangleDist[i] >= 0 && triangleDist[i] < minDist)
                {
                    minIndex = i;
                    minDist = triangleDist[i];
                }
            }

            int foundTriangle = minIndex;
            Light_ foundLight = null; // TODO wyliczyć
            Camera foundCamera = null; // TODO wyliczyć

            if(foundTriangle >= 0)
            {
                HierarchyMesh mesh = GetSelectedMesh(scene.hierarchy.objects, foundTriangle);
                if(scene.selTriangles.Contains(mesh) == false)
                {
                    scene.selTriangles.Add(mesh);
                }
            }
            else if(foundLight != null)
            {

            }
            else if(foundCamera != null)
            {

            }
            else
            {
                scene.selTriangles.Clear();
                scene.selLights.Clear();
                scene.selCams.Clear();
            }
        }
    }
}
