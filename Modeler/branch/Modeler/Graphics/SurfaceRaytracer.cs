using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Modeler.Data.Scene;

/*
 * Renderowana scena: kula, za którą jest ściana z teksturą szachownicy.
 * 
 */

namespace Modeler.Graphics
{
    // Obserwator w punkcie (0, 0, -8)
    // Kula jednostkowa w punkcie (0, 0, 0)
    // Plaszczyzna za kula (x, y, 2)
    // Rzutnia opisana punktami (-1, 1, -3), (1, 1, -3), (-1, -1, -3), (1, -1, -3)    
    class SurfaceRaytracer
    {
        private static int xResolution = 200;
        private static int yResolution = 200;
        private static Vector3D observer = new Vector3D(0, 0, -8);
        private static Vector3D sphereCenter = new Vector3D(0, 0, 0);
        private static float sphereRadius = 1;
        private static uint maxDepth = 10;
        // Wektor między lewym górnym a lewym dolnym wierzchołkiem rzutni
        private static Vector3D vectorV = new Vector3D(0, -1.9999f, 0);
        // Wektor między lewym górnym a prawym górnym wierzchołkiem rzutni
        private static Vector3D vectorU = new Vector3D(1.999f, 0, 0);
        // Lewy gorny punkt plaszczyzny rzutni
        private static Vector3D topLeft = new Vector3D(-1.000f, 0.9999f, -3);
        private static Material_ material;
        private static Light_ light = new Light_(null, Light_Type.Point, true, 1, 1, 1, 2, -1, 1, -2);
        public static Bitmap output = new Bitmap(xResolution, yResolution);
        // Płaszczyzna za kula
        private static Plane plane = new Plane(new Vector3D(0, 0, 1), -2);

        public static Bitmap Render(Material_ mat)
        {
            //System.Drawing.Graphics canvas = System.Drawing.Graphics.FromImage(output);
            //canvas.FillRectangle(Brushes.Black, 0, 0, xResolution, yResolution);
            material = mat;
            Vector3D directionVector = new Vector3D();
            Color color;
            for (int i = 0; i < xResolution; i++)
            {
                for (int j = 0; j < yResolution; j++)
                {
                    directionVector = vectorU * (i / (float)(xResolution - 1)) + vectorV * (j / (float)(yResolution - 1));
                    directionVector += topLeft;
                    directionVector -= observer;
                    directionVector.Normalize();

                    color = CalculateColor(observer, directionVector, 0);
                    output.SetPixel(i, j, color);
                }
            }

            return output;
        }

        public static Color CalculateColor(Vector3D origin, Vector3D direction, uint depth)
        {
            /*
             * Najpierw sprawdzamy czy promien uderza w kule, jesli nie to musi
             * uderzac w plaszczyzne za kula (ze wzgledu na przyjeta specyfike
             * konstrukcji sceny. Przeciecie z plaszczyzna rowniez nalezy
             * sprawdzic, aby obliczyc kolor i stworzyc "szachownice"
             */
            Color returnColor = Color.FromArgb(0, 0, 0);
            Vector3D intersectionPoint = new Vector3D();
            float distance = float.MaxValue;
            int inOut = 0;

            // Sprawdz przecięcie z kulą
            float b = -(origin.DotProduct(direction));
            float delta = (b * b) - (origin.DotProduct(origin)) + sphereRadius*sphereRadius;
            if (delta > 0)
            {
                delta = (float)Math.Sqrt(delta);
                float i1 = b - delta;
                float i2 = b + delta;

                if (i2 > 0)
                {
                    if (i1 < 0)
                    {
                        if (i2 < distance)
                        {
                            // Trafienie z wnętrza kuli
                            distance = i2;
                            inOut = -1;
                        }
                    }
                    else
                    {
                        if (i1 < distance)
                        {
                            // Trafienie od zewnatrz
                            distance = i1;
                            inOut = 1;
                        }
                    }
                }
            }

            // Jeśli kula nie zostala trafiona
            if (distance == float.MaxValue)
            {
                // Sprawdz przeciecie z plaszczyznami (chwilowo tylko plaszczyzna
                // za kula)
                float tmpDistance = -1;
                tmpDistance = -(plane.d + plane.vectorNormal.DotProduct(origin));
                tmpDistance /= plane.vectorNormal.DotProduct(direction);

                if (tmpDistance > -1 && tmpDistance < float.MaxValue)
                {
                    intersectionPoint = origin + direction * tmpDistance;

                    return returnColor = BackgroundColor(intersectionPoint);
                }

                return returnColor;                
            }

            Vector3D vectorObser = new Vector3D();
            Vector3D vectorLight = new Vector3D();
            Vector3D reflectedRay = new Vector3D();

            // Intersection point jest jednoczesnie wektorem normlanym trafionej
            // sfery, rowniez znormalizowanym (poniewaz sfera jest o pormieniu 1)
            intersectionPoint = origin + direction * distance;

            // Ponieważ mamy tylko jedno światło nie trzeba robić pętli po wszystkich
            // Wektor od punktu przeciecia do swiatla
            vectorLight.x = (float)light.position.X - intersectionPoint.x;
            vectorLight.y = (float)light.position.Y - intersectionPoint.y;
            vectorLight.z = (float)light.position.Z - intersectionPoint.z;

            float lengthLight = vectorLight.Length();
            vectorLight /= lengthLight;

            // Wektor od punktu przeciecia do obserwatora
            vectorObser = origin - intersectionPoint;
            vectorObser.Normalize();

            // Wektor odbicia
            reflectedRay = intersectionPoint * 2;
            reflectedRay *= vectorObser.DotProduct(intersectionPoint);
            reflectedRay -= vectorObser;
            reflectedRay.Normalize();

            int red = 0, green = 0, blue = 0;

            // Zalamanie
            if ((material.krcR > 0 || material.krcG > 0 || material.krcB > 0) && depth < maxDepth)
            {
                Vector3D refractedRey = new Vector3D();
                Vector3D sphereNormal = new Vector3D();
                float n = 0;
                if (inOut == 1)
                    n = 1.0f / material.n;
                else
                    n = material.n / 1.0f;

                sphereNormal = intersectionPoint * inOut;

                float cosI = -(sphereNormal.DotProduct(direction));
                float cosT2 = 1.0f - n * n * (1.0f - cosI * cosI);

                if (cosT2 > 0)
                {

                    refractedRey = direction * n + sphereNormal * (n * cosI - (float)Math.Sqrt(cosT2));

                    refractedRey.Normalize();

                    // Punkt należy troszkę odsunąć od ściany, aby nie udeżył 
                    // w ten sam element

                    intersectionPoint += refractedRey * 0.001f;

                    Color result = CalculateColor(intersectionPoint, refractedRey, ++depth);
                    red += (int)(result.R * material.krcR);
                    green += (int)(result.G * material.krcG);
                    blue += (int)(result.B * material.krcB);
                }
            }

            // Obliczanie swiatla rozproszonego i odbitego (diffuse, specular)
            // W przypadku kuli punkt trafienia jest jednoczesnie wektorem normalnym
            float lightNormDotProduct = intersectionPoint.DotProduct(vectorLight);
            float reflectedLightDotProduct = reflectedRay.DotProduct(vectorLight);

            // Swiatlo rozproszone (diffuse)
            red += (int) (material.colorR * material.kdcR * (light.colorR * light.power * lightNormDotProduct) / lengthLight);
            green += (int) (material.colorG * material.kdcG * (light.colorG * light.power * lightNormDotProduct) / lengthLight );
            blue += (int)(material.colorB * material.kdcB * (light.colorB * light.power * lightNormDotProduct) / lengthLight);
            // Swiatlo odbite (specular)
            if (reflectedLightDotProduct > 0)
            {
                red += (int) (material.kscR * (light.colorR * 255*light.power*Math.Pow(reflectedLightDotProduct, material.g)));;
                green += (int) (material.kscG * (light.colorG * 255*light.power*Math.Pow(reflectedLightDotProduct, material.g)));;
                blue += (int) (material.kscB * (light.colorB * 255 * light.power * Math.Pow(reflectedLightDotProduct, material.g))); ;
            }

            red = red > 255 ? 255 : red;
            green = green > 255 ? 255 : green;
            blue = blue > 255 ? 255 : blue;

            red = red < 0 ? 0 : red;
            green = green < 0 ? 0 : green;
            blue = blue < 0 ? 0 : blue;

            returnColor = Color.FromArgb(red, green, blue);
            return returnColor;
        }

        private static Color BackgroundColor(Vector3D point)
        {
            // Uzywane sa tylko kolory x
            int x = (int)Math.Floor(point.x) + 2;
            int y = (int)Math.Floor(point.y) + 2;

            if ((x % 2 == 0 && y % 2 == 0) || (x % 2 != 0 && y % 2 != 0))
            {
                return Color.Black;
            }
            else
            {
                return Color.Gray;
            }
        }

        public static void SaveImage()
        {
            Image imgToSave = (Image)output.Clone();
            imgToSave = imgToSave.GetThumbnailImage(56, 56, null, IntPtr.Zero);
            imgToSave.Save("..\\..\\Ikony\\Materialy\\"+material.name+".png", ImageFormat.Png);

            imgToSave.Dispose();
        }
    }
}