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
using Modeler.Transformations;
using Modeler;


namespace Modeler.Panels
{
    /// <summary>
    /// Interaction logic for TransformPanel.xaml
    /// </summary>
    public partial class TransformPanel : UserControl
    {
        float x1=0, x2=0, x3=0, y1=0, y2=0, y3=0, z1=1, z2=1, z3=1;

        public TransformPanel()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (textBox1.Text == null || textBox1.Text == "") textBox1.Text = "0";
                x1 = (float)Double.Parse(textBox1.Text);
                textBox1.SelectionStart = textBox1.Text.Length;
            }
            catch (Exception)
            {
                textBox1.Text = x1.ToString();
            }
        }

        private void textBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (textBox2.Text == null || textBox2.Text == "") textBox2.Text = "0";
                y1 = (float)Double.Parse(textBox2.Text);
                textBox2.SelectionStart = textBox2.Text.Length;
            }
            catch (Exception) 
            {
                textBox2.Text = y1.ToString(); 
            }
        }

        private void textBox3_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (textBox3.Text == null || textBox3.Text == "") textBox3.Text = "0";
                z1 = (float)Double.Parse(textBox3.Text);
                textBox3.SelectionStart = textBox3.Text.Length;
            }
            catch (Exception)
            {
                textBox3.Text = z1.ToString();
            }
        }

        private void textBox4_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (textBox4.Text == null || textBox4.Text == "") textBox4.Text = "0";
                x2 = (float)Double.Parse(textBox4.Text);
                textBox4.SelectionStart = textBox4.Text.Length;
            }
            catch (Exception)
            {
                textBox4.Text = x2.ToString();
            }
        }

        private void textBox5_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (textBox5.Text == null || textBox5.Text == "") textBox5.Text = "0";
                y2 = (float)Double.Parse(textBox5.Text);
                textBox5.SelectionStart = textBox5.Text.Length;
            }
            catch (Exception)
            {
                textBox5.Text = y2.ToString();
            }
        }

        private void textBox6_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (textBox6.Text == null || textBox6.Text == "") textBox6.Text = "0";
                z2 = (float)Double.Parse(textBox6.Text);
                textBox6.SelectionStart = textBox6.Text.Length;
            }
            catch (Exception)
            {
                textBox6.Text = z2.ToString();
            }
        }

        private void textBox7_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (textBox7.Text == null || textBox7.Text == "") textBox7.Text = "0";
                x3 = (float)Double.Parse(textBox7.Text);
                textBox7.SelectionStart = textBox7.Text.Length;
            }
            catch (Exception)
            {
                textBox7.Text = x3.ToString();
            }
        }

        private void textBox8_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (textBox8.Text == null || textBox8.Text == "") textBox8.Text = "0";
                y3 = (float)Double.Parse(textBox8.Text);
                textBox8.SelectionStart = textBox8.Text.Length;
            }
            catch (Exception)
            {
                textBox8.Text = y3.ToString();
            }
        }

        private void textBox9_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (textBox9.Text == null || textBox9.Text == "") textBox9.Text = "0";
                z3 = (float)Double.Parse(textBox9.Text);
                textBox9.SelectionStart = textBox9.Text.Length;
            }
            catch (Exception)
            {
                textBox9.Text = z3.ToString();
            }
        }

        private void button1_Clicked(object sender, RoutedEventArgs e)
        {
            DependencyObject depObj = this.Parent;

            do
            {
                depObj = LogicalTreeHelper.GetParent(depObj);
            } while (depObj.GetType() != typeof(MainWindow));
            MainWindow parent = (MainWindow)depObj;
            Scene tmpScene = new Scene();
            parent.transformPanelButtonClick(x1, y1, z1, x2, y2, z2, x3, y3, z3);
            
        }

 
    }
}
