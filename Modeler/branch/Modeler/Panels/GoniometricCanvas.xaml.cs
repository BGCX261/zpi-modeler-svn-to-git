using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.Windows.Threading;

namespace Modeler.Panels
{
    /// <summary>
    /// Interaction logic for GoniometricCanvas.xaml
    /// </summary>
    public partial class GoniometricCanvas : UserControl
    {
        private SortedList<float, float> Goniometry;
        private const float minAngle = 0;
        private const float maxAngle = 360;
        private const float minY = 0;
        private const float maxY = 1;
        private bool initialized = false;
        private Brush backgroundBrush = Brushes.Gray;
        private Pen gridLineColor = new Pen(Brushes.Black, 0.5);
        private const int horizontalLines = 4;
        private const int verticalLines = 8;
        private static int horizontalDet;
        private static int verticalDet;
        private static int verticalAngleDet;

        public GoniometricCanvas()
        {
            InitializeComponent();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            DrawGrid(drawingContext);
            DrawChart(drawingContext);
            UpdateParent();
        }

        private void DrawGrid(DrawingContext drawingContext)
        {
            Point p1 = new Point();
            Point p2 = new Point();
            // Inicjalizacja koloru tła
            drawingContext.DrawRectangle(backgroundBrush, null, new Rect(0, 0, this.ActualWidth, this.ActualHeight));

            if (!initialized)
            {
                horizontalDet = (int)((maxY * this.ActualHeight - minY * this.ActualHeight) / horizontalLines);
                verticalAngleDet = (int)((maxAngle - minAngle) / verticalLines);
                verticalDet = (int)(this.ActualWidth * verticalAngleDet / (maxAngle - minAngle));
                initialized = true;
            }

            // Rysowanie podzialki na x i y
            // Podzialka osi Y
            for (int i = 0; i < horizontalLines; i++)
            {
                p1.X = 0;
                p1.Y = this.ActualHeight - i * horizontalDet;
                p2.X = this.ActualWidth;
                p2.Y = p1.Y;
                drawingContext.DrawLine(gridLineColor, p1, p2);
            }
            // Podzialka osi X
            for (int i = 0; i < verticalLines; i++)
            {
                p1.X = i * verticalDet;
                p1.Y = 0;
                p2.X = p1.X;
                p2.Y = this.ActualHeight;
                drawingContext.DrawLine(gridLineColor, p1, p2);
            }
            // Podpisywanie osi i niektorych wartosci
        }

        private void DrawChart(DrawingContext drawingContext)
        {
            if (Goniometry != null && Goniometry.Count > 0)
            {
                Point p1 = new Point();
                Point p2 = new Point();
                float tmp;

                float centrAngle = (minAngle + maxAngle) / 4;
                float range = (maxAngle - minAngle) / 4;
                for (int i = 0; i < Goniometry.Count - 1; i++)
                {
                    //tmp = (((maxAngle + minAngle) / 2) + Goniometry.Keys[i]) / (centrAngle);k
                    tmp = (Goniometry.Keys[i] - centrAngle) / range;
                    p1.X = this.ActualWidth / 2 + tmp * this.ActualWidth / 2;
                    p1.Y = this.ActualHeight * ((maxY - minY) - Goniometry.Values[i] * (maxY - minY));

                    tmp = (Goniometry.Keys[i + 1] - centrAngle) / range;
                    p2.X = this.ActualWidth / 2 + tmp * this.ActualWidth / 2;
                    p2.Y = this.ActualHeight * ((maxY - minY) - Goniometry.Values[i + 1] * (maxY - minY));

                    drawingContext.DrawLine(gridLineColor, p1, p2);
                    drawingContext.DrawEllipse(Brushes.White, null, p1, 3, 3);
                }
                // Rysowanie ostatniego punktu
                tmp = (Goniometry.Keys[Goniometry.Count - 1] - centrAngle) / range;
                p1.X = this.ActualWidth / 2 + tmp * this.ActualWidth / 2;
                p1.Y = this.ActualHeight * ((maxY - minY) - Goniometry.Values[Goniometry.Count - 1] * (maxY - minY));
                drawingContext.DrawEllipse(Brushes.White, null, p1, 3, 3);
            }
        }

        private void UpdateParent()
        {
            DependencyObject parentObj = VisualTreeHelper.GetParent(this);
            if (parentObj == null) return;
            while (!(parentObj is LightsPanel))
            {
                parentObj = VisualTreeHelper.GetParent(parentObj);
            }
            LightsPanel parent = (LightsPanel)parentObj;
            parent.RenderLight();
        }

        private void goniometricCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
                float x = (float)e.GetPosition(this).X;
                float y = (float)e.GetPosition(this).Y;
                int angle = (int)((maxAngle + minAngle) / 4 + ((x - this.ActualWidth / 2) / this.ActualWidth * (maxAngle - minAngle) / 2));
                float value = maxY - minY - (float)(y / this.ActualHeight) * (maxY - minY);
                if (Goniometry.IndexOfKey(angle) == -1)
                    Goniometry.Add(angle, value);
                this.InvalidateVisual();
        }

        public void goniometricCanvas_DragOver(object sender, DragEventArgs e)
        {
            float x = (float)e.GetPosition(this).X;
            float y = (float)e.GetPosition(this).Y;

            float newKey = 0;
            float newVal = 0;
            int idx = (int)e.Data.GetData("Point");

            newKey = (int)((maxAngle + minAngle) / 4 + ((x - this.ActualWidth / 2) / this.ActualWidth * (maxAngle - minAngle) / 2));
            newVal = maxY - minY - (float)(y / this.ActualHeight) * (maxY - minY);

            newKey = newKey > maxAngle ? maxAngle : newKey < minAngle ? minAngle : newKey;
            newVal = newVal > maxY ? maxY : newVal < minY ? minY : newVal;

            Goniometry.RemoveAt(idx);
            if (!Goniometry.ContainsKey(newKey))
                Goniometry.Add(newKey, newVal);
            e.Data.SetData("Point", Goniometry.IndexOfKey(newKey));
            this.InvalidateVisual();

            e.Handled = true;
        }

        private void goniometricCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            float x = (float)e.GetPosition(this).X;
            float y = (float)e.GetPosition(this).Y;
            int angle = (int)((x - this.ActualWidth / 2) / this.ActualWidth * (maxAngle - minAngle) / 2);
            float value = maxY - minY - (float)(y / this.ActualHeight) * (maxY - minY);

            int idx = -1;
            for (int i = 0; i < Goniometry.Count; i++)
            {
                if (angle > Goniometry.Keys[i] - 5 && angle < Goniometry.Keys[i] + 5)
                    idx = i;
            }
            if (idx != -1)
            {
                Goniometry.RemoveAt(idx);
                this.InvalidateVisual();
            }
        }

        private void goniometricCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            float x = (float)e.GetPosition(this).X;
            float y = (float)e.GetPosition(this).Y;

            float minValue = (float)(maxY - minY - (float)(y / this.ActualHeight) * (maxY - minY)) - 0.05f;
            float maxValue = (float)(maxY - minY - (float)(y / this.ActualHeight) * (maxY - minY)) + 0.05f;

            DataObject dataObject = new DataObject();

            // Znalezc trafiony punkt
            // Wyslac indeks punktu do dragover
            int idx = -1;
            int angle = (int)((maxAngle + minAngle) / 4 + ((x - this.ActualWidth / 2) / this.ActualWidth * (maxAngle - minAngle) / 2));

            for (int i = 0; i < Goniometry.Count; i++)
            {
                if (angle > Goniometry.Keys[i] - 5 && angle < Goniometry.Keys[i] + 5)
                    idx = i;
            }

            if (idx != -1 && Goniometry.Values[idx] > minValue && Goniometry.Values[idx] < maxValue)
            {
                Console.WriteLine(idx);
                dataObject.SetData("Point", idx);
                DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Move);
            }
        }

        public void SetGoniometry(SortedList<float, float> goniom)
        {
            Goniometry = goniom;
        }
    }
}