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
using Modeler.Data.Scene;
using System.IO;
using Modeler.Graphics;
using System.Drawing.Imaging;

namespace Modeler.Panels
{
    /// <summary>
    /// Interaction logic for LightsPanel.xaml
    /// </summary>
    public partial class LightsPanel : UserControl
    {
        private Light_ light;

        public LightsPanel()
        {
            InitializeComponent();
            SortedList<float, float> goniometry = new SortedList<float, float>()
            {
                //new Pair<float, float>(-45, 0),
                //new Pair<float, float>(-60, 0.05f),
                //new Pair<float, float>(-50, 0.1f),
                //new Pair<float, float>(-40, 0.12f),
                //new Pair<float, float>(-30, 0.8123f),
                //new Pair<float, float>(-20, 0.563f),
                //new Pair<float, float>(-10, 0.835f),
                //new Pair<float, float>(0, 0.1231f),
                //new Pair<float, float>(10, 0.3453f),
                //new Pair<float, float>(20, 0.68968f),
                //new Pair<float, float>(30, 0.234f),
                //new Pair<float, float>(40, 0.6789f),
                //new Pair<float, float>(50, 0.9f),
                //new Pair<float, float>(60, 1f),                
                //new Pair<float, float>(45, 0)
                //new Pair<float, float>(-90, 0),
                //new Pair<float, float>(-60, 0),
                //new Pair<float, float>(-30, 0.5f),
                //new Pair<float, float>(-5, 0.9f),
                //new Pair<float, float>(0, 1f),
                //new Pair<float, float>(5, 0.5f),
                //new Pair<float, float>(30, 0.5f),
                //new Pair<float, float>(60, 0),
                //new Pair<float, float>(90, 0)
            };
            //goniometry.Add(-90, 0);
            //goniometry.Add(-60, 0);
            //goniometry.Add(-30, 0.5f);
            //goniometry.Add(-5, 0.9f);
            //goniometry.Add(0, 1f);
            //goniometry.Add(5, 0.5f);
            //goniometry.Add(30, 0.5f);
            //goniometry.Add(60, 0);
            //goniometry.Add(90, 0);

            //Graphics.LightRaytracer.Render(new Light_("lgt", Light_Type.Spot, true, 1, 1, 1, 3,
            //                                            new SlimDX.Vector3(0, 0, 0), new SlimDX.Vector3(1, 1, 1),
            //                                            30, 60, null));
            SetLight(new Light_("lgt", Light_Type.Goniometric, true, 1, 1, 1, 3,
                                                        new SlimDX.Vector3(0, 0, 0), new SlimDX.Vector3(1, 1, 1),
                                                        30, 90, goniometry));
        }

        public void SetLight(Light_ lgt)
        {
            light = lgt;
            goniometry.SetGoniometry(light.goniometric);

            colorRed_slider.Value = light.colorR;
            colorGreen_slider.Value = light.colorG;
            colorBlue_slider.Value = light.colorB;

            flux_slider.Value = light.power;

            innerAngle_slider.Value = light.innerAngle;
            outerAngle_slider.Value = light.outerAngle;

            UpdateElements();
            RenderLight();
        }

        public void RenderLight()
        {
            MemoryStream ms = new MemoryStream();
            LightRaytracer.Render(light).Save(ms, ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            lightPreview.Source = bi;
        }

        private void clolorRed_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            colorRed_box.Text = colorRed_slider.Value.ToString();
            light.colorR = (float)colorRed_slider.Value;

            RenderLight();
        }

        private void colorGreen_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            colorGreen_box.Text = colorGreen_slider.Value.ToString();
            light.colorG = (float)colorGreen_slider.Value;

            RenderLight();
        }

        private void colorBlue_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            colorBlue_box.Text = colorBlue_slider.Value.ToString();
            light.colorB = (float)colorBlue_slider.Value;

            RenderLight();
        }

        private void colorRed_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (colorRed_box.Text == null || colorRed_box.Text == "") colorRed_box.Text = "0";
                if (Double.Parse(colorRed_box.Text.Replace(".", ",")) > 1) colorRed_box.Text = "1";
                colorRed_slider.Value = Double.Parse(colorRed_box.Text.Replace(".", ","));
                colorRed_box.SelectionStart = colorRed_box.Text.Length;
            }
            catch (Exception)
            {
                colorRed_box.Text = colorRed_slider.Value.ToString();
            }
        }

        private void colorGreen_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (colorGreen_box.Text == null || colorGreen_box.Text == "") colorGreen_box.Text = "0";
                if (Double.Parse(colorGreen_box.Text.Replace(".", ",")) > 1) colorGreen_box.Text = "1";
                colorGreen_slider.Value = Double.Parse(colorGreen_box.Text.Replace(".", ","));
                colorGreen_box.SelectionStart = colorGreen_box.Text.Length;
            }
            catch (Exception)
            {
                colorGreen_box.Text = colorGreen_slider.Value.ToString();
            }
        }

        private void colorBlue_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (colorBlue_box.Text == null || colorBlue_box.Text == "") colorBlue_box.Text = "0";
                if (Double.Parse(colorBlue_box.Text.Replace(".", ",")) > 1) colorBlue_box.Text = "1";
                colorBlue_slider.Value = Double.Parse(colorBlue_box.Text.Replace(".", ","));
                colorBlue_box.SelectionStart = colorBlue_box.Text.Length;
            }
            catch (Exception)
            {
                colorBlue_box.Text = colorBlue_slider.Value.ToString();
            }
        }

        private void flux_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            flux_box.Text = flux_slider.Value.ToString();
            light.power = (float)flux_slider.Value;

            RenderLight();
        }

        private void flux_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (flux_box.Text == null || flux_box.Text == "") flux_box.Text = "0";
                if (Double.Parse(flux_box.Text.Replace(".", ",")) > 200) flux_box.Text = "200";
                flux_slider.Value = Double.Parse(flux_box.Text.Replace(".", ","));
                flux_box.SelectionStart = flux_box.Text.Length;
            }
            catch (Exception)
            {
                flux_box.Text = flux_slider.Value.ToString();
            }
        }

        private void innerAngle_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            innerAngle_box.Text = innerAngle_slider.Value.ToString();
            light.innerAngle = (float)innerAngle_slider.Value;

            RenderLight();
        }

        private void innerAngle_box_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                if (innerAngle_box.Text == null || innerAngle_box.Text == "") innerAngle_box.Text = "0";
                if (Double.Parse(innerAngle_box.Text.Replace(".", ",")) > 180) innerAngle_box.Text = "180";
                innerAngle_slider.Value = Double.Parse(innerAngle_box.Text.Replace(".", ","));
                innerAngle_box.SelectionStart = innerAngle_box.Text.Length;
            }
            catch (Exception)
            {
                innerAngle_box.Text = innerAngle_slider.Value.ToString();
            }
        }

        private void lightType_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Console.WriteLine(lightType_combo.SelectedItem + " " + lightType_combo.SelectedValue + " " + lightType_combo.SelectedIndex);
            //light.type = (Light_Type)Enum.Parse(typeof(Light_Type), lightType_combo.SelectedItem.ToString());
            
            if (light != null)
            {
                light.type = (Light_Type)lightType_combo.SelectedIndex;
                RenderLight();
                UpdateElements();
            }
        }

        private void outerAngle_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            outerAngle_box.Text = outerAngle_slider.Value.ToString();
            light.outerAngle = (float)outerAngle_slider.Value;

            RenderLight();
        }

        private void outerAngle_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (outerAngle_box.Text == null || outerAngle_box.Text == "") outerAngle_box.Text = "0";
                if (Double.Parse(outerAngle_box.Text.Replace(".", ",")) > 180) outerAngle_box.Text = "180";
                outerAngle_slider.Value = Double.Parse(outerAngle_box.Text.Replace(".", ","));
                outerAngle_box.SelectionStart = outerAngle_box.Text.Length;
            }
            catch (Exception)
            {
                outerAngle_box.Text = outerAngle_slider.Value.ToString();
            }
        }

        private void lightPreview_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.DataObject dataObject = new System.Windows.DataObject();
                dataObject.SetData("Object", light);
                DragDrop.DoDragDrop(this, dataObject, System.Windows.DragDropEffects.Copy);
                //Console.WriteLine(_elementsCol.ElementAt(gotowe_ListView.SelectedIndex).ToString());
            }
        }

        private void UpdateElements()
        {
            switch (light.type)
            {
                case Light_Type.Point:
                    lightType_combo.SelectedIndex = (int)Light_Type.Point;
                    goniometry.Visibility = Visibility.Hidden;
                    innerAngle_box.Visibility = Visibility.Hidden;
                    innerAngle_label.Visibility = Visibility.Hidden;
                    innerAngle_slider.Visibility = Visibility.Hidden;
                    outerAngle_box.Visibility = Visibility.Hidden;
                    outerAngle_label.Visibility = Visibility.Hidden;
                    outerAngle_slider.Visibility = Visibility.Hidden;
                    break;
                case Light_Type.Spot:
                    lightType_combo.SelectedIndex = (int)Light_Type.Spot;
                    goniometry.Visibility = Visibility.Hidden;
                    innerAngle_box.Visibility = Visibility.Visible;
                    innerAngle_label.Visibility = Visibility.Visible;
                    innerAngle_slider.Visibility = Visibility.Visible;
                    outerAngle_box.Visibility = Visibility.Visible;
                    outerAngle_label.Visibility = Visibility.Visible;
                    outerAngle_slider.Visibility = Visibility.Visible;
                    break;
                case Light_Type.Goniometric:
                    lightType_combo.SelectedIndex = (int)Light_Type.Goniometric;
                    goniometry.Visibility = Visibility.Visible;
                    innerAngle_box.Visibility = Visibility.Hidden;
                    innerAngle_label.Visibility = Visibility.Hidden;
                    innerAngle_slider.Visibility = Visibility.Hidden;
                    outerAngle_box.Visibility = Visibility.Hidden;
                    outerAngle_label.Visibility = Visibility.Hidden;
                    outerAngle_slider.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }
        }
    }
}
