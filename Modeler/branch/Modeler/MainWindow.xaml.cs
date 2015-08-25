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
using System.Windows.Media.Media3D;
using System.Windows.Forms;
using SlimDX;
using Modeler.Data;
using Modeler.Data.Elements;
using Modeler.Data.Shapes;
using Modeler.Graphics;
using Modeler.Data.Scene;
using Modeler.Transformations;
using Modeler.Data.Galleries;
using Modeler.Data.Surfaces;
using Modeler.Data.Light;

namespace Modeler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Renderer renderer;
        private int maxViewport;
        private const int xOffset = 16;
        private const int yOffset = 86;
        private Scene currScene;

        private Undo.UndoStack undo = new Undo.UndoStack();

        private ShapeGallery _shapesGallery;
        private PreparedObjectsGallery _elementsGallery;
        private SurfaceGallery _surfaceGallery;
        private LightGallery _lightGallery;
        private System.Windows.Forms.ContextMenu contextMenu;
        private float dragX, dragY;
        private int mRCx, mRCy;  // mouse right button click position

        private Point mousePos;

        public MainWindow()
        {
            InitializeComponent();

            renderer = new Renderer(Views.Handle, ViewsBezier.Handle);
            maxViewport = -1;

            //currScene = Scene.GetExampleScene();
            currScene = new Scene();

            // Tworzenie kolekcji obiektów i dodawanie jej do kontrolki ItemsControl
            // galerii ksztaltow.
            _shapesGallery = new ShapeGallery();
            ksztalty_ListView.ItemsSource = _shapesGallery;

            // Tworzenie kolekcji gotowych obiektów i dodawanie jej do kontrolki ItemsControl
            // galerii gotowych obiektów.
            _elementsGallery = new PreparedObjectsGallery();
            gotowe_ListView.ItemsSource = _elementsGallery;

            _surfaceGallery = new SurfaceGallery();
            materialy_ListView.ItemsSource = _surfaceGallery;

            _lightGallery = new LightGallery();
            swiatla_ListView.ItemsSource = _lightGallery;

            //Menu kontekstowe
            contextMenu = new System.Windows.Forms.ContextMenu();
            contextMenu.MenuItems.Add("Przenieś", contextMenuClick);
            contextMenu.MenuItems.Add("Obróć", contextMenuClick);
            contextMenu.MenuItems.Add("Skaluj", contextMenuClick);
            contextMenu.MenuItems.Add("Skaluj wzdłuż osi", contextMenuClick);
            contextMenu.MenuItems.Add("Powiększ widok", contextViewport);
            System.Windows.Forms.MenuItem[] subMenu = new System.Windows.Forms.MenuItem[4];
            System.Windows.Forms.MenuItem subLeft = new System.Windows.Forms.MenuItem("Lewej", subLeftClick);
            System.Windows.Forms.MenuItem subRight = new System.Windows.Forms.MenuItem("Prawej", subRightClick);
            System.Windows.Forms.MenuItem subUp = new System.Windows.Forms.MenuItem("Góry", subUpClick);
            System.Windows.Forms.MenuItem subDown = new System.Windows.Forms.MenuItem("Dołu", subDownClick);
            subMenu[0] = subLeft;
            subMenu[1] = subRight;
            subMenu[2] = subUp;
            subMenu[3] = subDown;
            contextMenu.MenuItems.Add("Dosuń do", subMenu);
            contextMenu.MenuItems[0].Checked = true;
            Views.ContextMenu = contextMenu;
        }

        private void subLeftClick(object sender, System.EventArgs e)
        {
            undo.Save(currScene);
            ViewportInfo coords = GetViewCoords();
            // Lewy górny panel
            if (mRCx >= coords.posX[0] && mRCx < coords.posX[0] + coords.sizeX[0] && mRCy >= coords.posY[0] && mRCy < coords.posY[0] + coords.sizeY[0])
            {
                Console.WriteLine("Brak opcji dosuwania w tym widoku. Dosuwanie do lewej.");
            }
            // Prawy górny panel
            else if (mRCx >= coords.posX[1] && mRCx < coords.posX[1] + coords.sizeX[1] && mRCy >= coords.posY[1] && mRCy < coords.posY[1] + coords.sizeY[1])
            {
                if(DosuwanieCB.IsChecked==true)
                    Transformations.Intersection.Slide(currScene, "xyl");
                else
                    Transformations.Intersection.SlideEZver(currScene, "xyl");
                Console.WriteLine("Dosuwanie w lewo : prawy górny - xyl");
            }
            // Lewy dolny panel
            else if (mRCx >= coords.posX[2] && mRCx < coords.posX[2] + coords.sizeX[2] && mRCy >= coords.posY[2] && mRCy < coords.posY[2] + coords.sizeY[2])
            {
                if (DosuwanieCB.IsChecked == true)
                    Transformations.Intersection.Slide(currScene, "xyl");
                else
                    Transformations.Intersection.SlideEZver(currScene, "xyl");
                Console.WriteLine("Dosuwanie w lewo : lewy dolny - xyl");
            }
            // Prawy dolny panel
            else if (mRCx >= coords.posX[3] && mRCx < coords.posX[3] + coords.sizeX[3] && mRCy >= coords.posY[3] && mRCy < coords.posY[3] + coords.sizeY[3])
            {
                if (DosuwanieCB.IsChecked == true)
                    Transformations.Intersection.Slide(currScene, "zxf");
                else
                    Transformations.Intersection.SlideEZver(currScene, "zxf");
                Console.WriteLine("Dosuwanie w lewo : prawy dolny - zxf");
            }
            //if (wynik metody zwracający widok z którego wywołano menu kontekstowe jest rzutem na płaszczyznę xy)
            //Transformations.Intersection.SlideEZver(currScene, "xyl");
            //else if(wynik metody zwracający widok z którego wywołano menu kontekstowe jest rzutem na płaszczyznę yz)
            //Transformations.Intersection.SlideEZver(currScene, "zyb");
            //else if(wynik metody zwracający widok z którego wywołano menu kontekstowe jest rzutem na płaszczyznę zx)
            //Transformations.Intersection.SlideEZver(currScene, "xyl");
            RenderViews();
        }

        private void subRightClick(object sender, System.EventArgs e)
        {
            undo.Save(currScene);
            ViewportInfo coords = GetViewCoords();
            // Lewy górny panel
            if (mRCx >= coords.posX[0] && mRCx < coords.posX[0] + coords.sizeX[0] && mRCy >= coords.posY[0] && mRCy < coords.posY[0] + coords.sizeY[0])
            {
                Console.WriteLine("Brak opcji dosuwania w tym widoku. Dosuwanie do prawej.");
            }
            // Prawy górny panel
            else if (mRCx >= coords.posX[1] && mRCx < coords.posX[1] + coords.sizeX[1] && mRCy >= coords.posY[1] && mRCy < coords.posY[1] + coords.sizeY[1])
            {
                if (DosuwanieCB.IsChecked == true)
                    Transformations.Intersection.Slide(currScene, "xyr");
                else
                    Transformations.Intersection.SlideEZver(currScene, "xyr");
                Console.WriteLine("Dosuwanie w prawo : prawy górny - xyr");
            }
            // Lewy dolny panel
            else if (mRCx >= coords.posX[2] && mRCx < coords.posX[2] + coords.sizeX[2] && mRCy >= coords.posY[2] && mRCy < coords.posY[2] + coords.sizeY[2])
            {
                if (DosuwanieCB.IsChecked == true)
                    Transformations.Intersection.Slide(currScene, "xyr");
                else
                    Transformations.Intersection.SlideEZver(currScene, "xyr");
                Console.WriteLine("Dosuwanie w prawo: lewy dolny - xyr");
            }
            // Prawy dolny panel
            else if (mRCx >= coords.posX[3] && mRCx < coords.posX[3] + coords.sizeX[3] && mRCy >= coords.posY[3] && mRCy < coords.posY[3] + coords.sizeY[3])
            {
                if (DosuwanieCB.IsChecked == true)
                    Transformations.Intersection.Slide(currScene, "zxb");
                else
                    Transformations.Intersection.SlideEZver(currScene, "zxb");
                Console.WriteLine("Dosuwanie w prawo : prawy dolny - zxb");
            }
            ////if (wynik metody zwracający widok z którego wywołano menu kontekstowe jest rzutem na płaszczyznę xy)
            //Transformations.Intersection.SlideEZver(currScene, "xyr");
            ////else if(wynik metody zwracający widok z którego wywołano menu kontekstowe jest rzutem na płaszczyznę yz)
            ////Transformations.Intersection.SlideEZver(currScene, "zyf");
            ////else if(wynik metody zwracający widok z którego wywołano menu kontekstowe jest rzutem na płaszczyznę zx)
            ////Transformations.Intersection.SlideEZver(currScene, "xyr");
            RenderViews();
        }

        private void subUpClick(object sender, System.EventArgs e)
        {
            undo.Save(currScene);
            ViewportInfo coords = GetViewCoords();
            // Lewy górny panel
            if (mRCx >= coords.posX[0] && mRCx < coords.posX[0] + coords.sizeX[0] && mRCy >= coords.posY[0] && mRCy < coords.posY[0] + coords.sizeY[0])
            {
                Console.WriteLine("Brak opcji dosuwania w tym widoku. Dosuwanie do góry.");
            }
            // Prawy górny panel
            else if (mRCx >= coords.posX[1] && mRCx < coords.posX[1] + coords.sizeX[1] && mRCy >= coords.posY[1] && mRCy < coords.posY[1] + coords.sizeY[1])
            {
                if (DosuwanieCB.IsChecked == true)
                    Transformations.Intersection.Slide(currScene, "zxb");
                else
                    Transformations.Intersection.SlideEZver(currScene, "zxb");
                Console.WriteLine("Dosuwanie do góry : prawy górny - zxb");
            }
            // Lewy dolny panel
            else if (mRCx >= coords.posX[2] && mRCx < coords.posX[2] + coords.sizeX[2] && mRCy >= coords.posY[2] && mRCy < coords.posY[2] + coords.sizeY[2])
            {
                if (DosuwanieCB.IsChecked == true)
                    Transformations.Intersection.Slide(currScene, "yzu");
                else
                    Transformations.Intersection.SlideEZver(currScene, "yzu");
                Console.WriteLine("Dosuwanie do góry : lewy dolny - yzu");
            }
            // Prawy dolny panel
            else if (mRCx >= coords.posX[3] && mRCx < coords.posX[3] + coords.sizeX[3] && mRCy >= coords.posY[3] && mRCy < coords.posY[3] + coords.sizeY[3])
            {
                if (DosuwanieCB.IsChecked == true)
                    Transformations.Intersection.Slide(currScene, "yzu");
                else
                    Transformations.Intersection.SlideEZver(currScene, "yzu");
                Console.WriteLine("Dosuwanie do góry : prawy dolny - yzu");
            }
            //if (wynik metody zwracający widok z którego wywołano menu kontekstowe jest rzutem na płaszczyznę xy)
            //Transformations.Intersection.SlideEZver(currScene, "yxu");
            //else if(wynik metody zwracający widok z którego wywołano menu kontekstowe jest rzutem na płaszczyznę yz)
            //Transformations.Intersection.SlideEZver(currScene, "yxu");
            //else if(wynik metody zwracający widok z którego wywołano menu kontekstowe jest rzutem na płaszczyznę zx)
            //Transformations.Intersection.SlideEZver(currScene, "zxf");
            RenderViews();
        }

        private void subDownClick(object sender, System.EventArgs e)
        {
            undo.Save(currScene);
            ViewportInfo coords = GetViewCoords();
            // Lewy górny panel
            if (mRCx >= coords.posX[0] && mRCx < coords.posX[0] + coords.sizeX[0] && mRCy >= coords.posY[0] && mRCy < coords.posY[0] + coords.sizeY[0])
            {
                Console.WriteLine("Brak opcji dosuwania w tym widoku. Dosuwanie w dół.");
            }
            // Prawy górny panel
            else if (mRCx >= coords.posX[1] && mRCx < coords.posX[1] + coords.sizeX[1] && mRCy >= coords.posY[1] && mRCy < coords.posY[1] + coords.sizeY[1])
            {
                if (DosuwanieCB.IsChecked == true)
                    Transformations.Intersection.Slide(currScene, "zxf");
                else
                    Transformations.Intersection.SlideEZver(currScene, "zxf");
                Console.WriteLine("Dosuwanie do dołu : prawy górny - zxf");
            }
            // Lewy dolny panel
            else if (mRCx >= coords.posX[2] && mRCx < coords.posX[2] + coords.sizeX[2] && mRCy >= coords.posY[2] && mRCy < coords.posY[2] + coords.sizeY[2])
            {
                if (DosuwanieCB.IsChecked == true)
                    Transformations.Intersection.Slide(currScene, "yzd");
                else
                    Transformations.Intersection.SlideEZver(currScene, "yzd");
                Console.WriteLine("Dosuwanie do dołu : lewy dolny - yzd");
            }
            // Prawy dolny panel
            else if (mRCx >= coords.posX[3] && mRCx < coords.posX[3] + coords.sizeX[3] && mRCy >= coords.posY[3] && mRCy < coords.posY[3] + coords.sizeY[3])
            {
                if (DosuwanieCB.IsChecked == true)
                    Transformations.Intersection.Slide(currScene, "yzd");
                else
                    Transformations.Intersection.SlideEZver(currScene, "yzd");
                Console.WriteLine("Dosuwanie do dołu : prawy dolny - yzd");
            }
            //if (wynik metody zwracający widok z którego wywołano menu kontekstowe jest rzutem na płaszczyznę xy)
            //Transformations.Intersection.SlideEZver(currScene, "yxd");
            //else if(wynik metody zwracający widok z którego wywołano menu kontekstowe jest rzutem na płaszczyznę yz)
            //Transformations.Intersection.SlideEZver(currScene, "yxd");
            //else if(wynik metody zwracający widok z którego wywołano menu kontekstowe jest rzutem na płaszczyznę zx)
            //Transformations.Intersection.SlideEZver(currScene, "zxb");
            RenderViews();
        }

        private void contextMenuClick(object sender, System.EventArgs e)
        {
            for (int i = 0; i < 4; i++) contextMenu.MenuItems[i].Checked = false;
            ((System.Windows.Forms.MenuItem)sender).Checked = true;

            if (contextMenu.MenuItems[0].Checked == true)
            {
                modeTBG1.Text = "Przenoszenie";
                modeTBG2.Text = "Przenoszenie";
                modeTBG3.Text = "Przenoszenie";
                modeTBG4.Text = "Przenoszenie";
            }
            else if (contextMenu.MenuItems[1].Checked == true)
            {
                modeTBG1.Text = "Obracanie";
                modeTBG2.Text = "Obracanie";
                modeTBG3.Text = "Obracanie";
                modeTBG4.Text = "Obracanie";
            }
            else if (contextMenu.MenuItems[2].Checked == true)
            {
                modeTBG1.Text = "Skalowanie";
                modeTBG2.Text = "Skalowanie";
                modeTBG3.Text = "Skalowanie";
                modeTBG4.Text = "Skalowanie";
            }
            else if (contextMenu.MenuItems[3].Checked == true)
            {
                modeTBG1.Text = "Skalowanie wzdłuż osi";
                modeTBG2.Text = "Skalowanie wzdłuż osi";
                modeTBG3.Text = "Skalowanie wzdłuż osi";
                modeTBG4.Text = "Skalowanie wzdłuż osi";
            } 
        }

        private void contextViewport(object sender, System.EventArgs e)
        {
            if (maxViewport == -1)
            {
                ViewportInfo views = GetViewCoords();

                int rect = -1;

                for (int i = 0; i < 4; ++i)
                {
                    if (views.posX[i] <= mousePos.X && mousePos.X < views.posX[i] + views.sizeX[i] && views.posY[i] <= mousePos.Y && mousePos.Y < views.posY[i] + views.sizeY[i])
                    {
                        rect = i;
                    }
                }

                if (rect >= 0)
                {
                    maxViewport = rect;
                    contextMenu.MenuItems[4].Text = "Zmniejsz widok";

                    RenderViews();
                }
            }
            else
            {
                maxViewport = -1;
                contextMenu.MenuItems[4].Text = "Powiększ widok";

                RenderViews();
            }
        }

        ViewportInfo GetViewBezierCoords()
        {
            return new ViewportInfo(ViewsBezier.Size.Width, ViewsBezier.Size.Height, new int[] { 0 }, new int[] { 0 },
                new int[] { ViewsBezier.Size.Width }, new int[] { ViewsBezier.Size.Height });
        }

        ViewportInfo GetViewCoords()
        {
            ViewportInfo viewInfo = new ViewportInfo();

            viewInfo.resX = Views.Size.Width;
            viewInfo.resY = Views.Size.Height;

            int panelX = 0;
            int panelY = 0;

            viewInfo.posX = new int[4];
            viewInfo.posY = new int[4];
            viewInfo.sizeX = new int[4];
            viewInfo.sizeY = new int[4];

            if (maxViewport == -1)
            {
                int elemWidth = (int)(Views.Size.Width / 2) - 1;
                int elemHeight = (int)(Views.Size.Height / 2) - 1;

                viewInfo.posX[0] = panelX;
                viewInfo.posY[0] = panelY;
                viewInfo.sizeX[0] = elemWidth;
                viewInfo.sizeY[0] = elemHeight;

                viewInfo.posX[1] = panelX + elemWidth + 2;
                viewInfo.posY[1] = panelY;
                viewInfo.sizeX[1] = viewInfo.resX - viewInfo.posX[1];
                viewInfo.sizeY[1] = elemHeight;

                viewInfo.posX[2] = panelX;
                viewInfo.posY[2] = panelY + elemHeight + 2;
                viewInfo.sizeX[2] = elemWidth;
                viewInfo.sizeY[2] = viewInfo.resY - viewInfo.posY[2];

                viewInfo.posX[3] = panelX + elemWidth + 2;
                viewInfo.posY[3] = panelY + elemHeight + 2;
                viewInfo.sizeX[3] = viewInfo.resX - viewInfo.posX[3];
                viewInfo.sizeY[3] = viewInfo.resY - viewInfo.posY[3];
            }
            else
            {
                for (int i = 0; i < 4; ++i)
                {
                    viewInfo.posX[i] = viewInfo.posY[i] = viewInfo.sizeX[i] = viewInfo.sizeY[i] = 0;
                }

                viewInfo.sizeX[maxViewport] = Views.Size.Width;
                viewInfo.sizeY[maxViewport] = Views.Size.Height;
            }

            return viewInfo;
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            MessageBoxResult ifSave = System.Windows.MessageBox.Show("Czy chesz zapisać bieżącą scenę ?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (ifSave == MessageBoxResult.Yes)
            {
                SaveFile(sender, e);
            }

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Filter = "Pliki tekstowe |*.txt|Wszystkie pliki |*.*";
            dlg.InitialDirectory = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                Scene fileScene = Scene.ReadSceneFromFile(dlg.FileName);
                if (fileScene != null)
                {
                    currScene = fileScene;
                    RenderViews();
                }
                else
                {
                    System.Windows.MessageBox.Show("Wybrany plik ma niepoprawny format.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                trNumTBG1.Text = currScene.triangles.Count().ToString();
                trNumTBG2.Text = currScene.triangles.Count().ToString();
                trNumTBG3.Text = currScene.triangles.Count().ToString();
                trNumTBG4.Text = currScene.triangles.Count().ToString();
            }
        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.Filter = "Pliki tekstowe |*.txt|Wszystkie pliki |*.*";
            dlg.InitialDirectory = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                currScene.SaveSceneFile(dlg.FileName);
            }
        }

        private void RenderViews()
        {
            renderer.RenderViews(GetViewCoords(), currScene);
        }

        private void RenderBezier()
        {
            renderer.RenderBezier(GetViewBezierCoords());
        }

        private void Wireframe(object sender, RoutedEventArgs e)
        {
            renderer.ChangeWireframe();

            RenderViews();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Views_Paint(object sender, PaintEventArgs e)
        {
            RenderViews();
        }

        private void ViewsBezier_Paint(object sender, PaintEventArgs e)
        {
            RenderBezier();
        }

        private void Views_Click(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ViewportInfo views = GetViewCoords();

                int x = e.X;
                int y = e.Y;

                int rect = -1;

                for (int i = 0; i < 4; ++i)
                {
                    if (views.posX[i] <= x && x < views.posX[i] + views.sizeX[i] && views.posY[i] <= y && y < views.posY[i] + views.sizeY[i])
                    {
                        rect = i;
                    }
                }

                if (rect >= 0)
                {
                    ViewportType viewportType = rect == 0 ? ViewportType.Perspective : ViewportType.Orto;
                    int orthoRect = rect == 0 ? rect : rect - 1;

                    SelectingElems.SelectElems(currScene, viewportType, new System.Drawing.Point(x - views.posX[rect], y - views.posY[rect]),
                        new System.Drawing.Point(views.sizeX[rect], views.sizeY[rect]), new Vector2(renderer.OrthoWidth[orthoRect], (float)views.sizeY[rect] / views.sizeX[rect] * renderer.OrthoWidth[orthoRect]),
                        renderer.OrthoPos[orthoRect], renderer.OrthoLookAt[orthoRect]);

                    RenderViews();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                ViewportInfo views = GetViewCoords();

                mRCx = e.X;
                mRCy = e.Y;

                Console.WriteLine("x " + mRCx + " y " + mRCy);
            }
        }

        /*private Vector2 GetOrthoCoeficient()
        {
            Vector2 size = new Vector2(0, 0);

            ViewportInfo views = GetViewCoords();            

            size.X = renderer.OrthoWidth[0];
            size.Y = (float)views.sizeY[0] / views.sizeX[0] * renderer.OrthoWidth[0];

            size.X /= views.sizeX[0];
            size.Y /= views.sizeY[0];

            return size;
        }*/

        private void widok_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.Copy | System.Windows.DragDropEffects.Move;
            ViewportInfo views = GetViewCoords();

            float x = (float)(e.GetPosition(this).X - dragX);
            float y = (float)(e.GetPosition(this).Y - dragY);

            // Przesuwanie
            if (e.Data.GetData("Viewport") != null)
            {
                if (contextMenu.MenuItems[0].Checked)
                {
                    if (e.Data.GetData("Viewport").Equals("front"))
                    {
                        // Lewy dolny panel dziala na x y, a więc jest to !!!PRZOD!!!
                        float factor = (renderer.OrthoWidth[1] / views.sizeX[2]);
                        Transformations.Transformations.Translate(currScene, x * factor, -y * factor, 0);
                        Console.WriteLine("x " + (x * factor) +" y "+ (-y * factor)+" z "+ 0);
                    }
                    if (e.Data.GetData("Viewport").Equals("top"))
                    {
                        // Prawy górny panel działa na x z, a więc jest to !!!GORA!!!
                        float factor = (renderer.OrthoWidth[0] / views.sizeX[1]);
                        Transformations.Transformations.Translate(currScene, x * factor, 0, y * factor);
                        Console.WriteLine("x " + (x * factor) + " y " + 0 + " z " + (y * factor));
                    }
                    if (e.Data.GetData("Viewport").Equals("side"))
                    {
                        // Prawy dolny panel działa na y z, a wiec jest to !!!BOK!!!
                        float factor = (renderer.OrthoWidth[2] / views.sizeX[3]);
                        Transformations.Transformations.Translate(currScene, 0, -y * factor, -x * factor);
                        Console.WriteLine("x " + 0 + " y " + (-y * factor) + " z " + (-x * factor));
                    }
                }
                // Obracanie
                else if (contextMenu.MenuItems[1].Checked)
                {
                    if (e.Data.GetData("Viewport").Equals("side"))
                    {
                        Transformations.Transformations.RotateOX(currScene, -x * (renderer.OrthoWidth[2] / views.sizeX[3]));
                    }
                    if (e.Data.GetData("Viewport").Equals("top"))
                    {
                        Transformations.Transformations.RotateOY(currScene, -x * (renderer.OrthoWidth[0] / views.sizeX[1]));
                    }
                    if (e.Data.GetData("Viewport").Equals("front"))
                    {
                        Transformations.Transformations.RotateOZ(currScene, x * (renderer.OrthoWidth[1] / views.sizeX[2]));
                    }
                }
                // Skalowanie
                else if (contextMenu.MenuItems[2].Checked)
                {
                    if (e.Data.GetData("Viewport").Equals("front"))
                    {
                        float factor = (renderer.OrthoWidth[1] / views.sizeX[2]);
                        Transformations.Transformations.Scale(currScene, x * factor, x * factor, x * factor);
                    }
                    if (e.Data.GetData("Viewport").Equals("top"))
                    {
                        float factor = (renderer.OrthoWidth[0] / views.sizeX[1]);
                        Transformations.Transformations.Scale(currScene, x * factor, x * factor, x * factor);
                    }
                    if (e.Data.GetData("Viewport").Equals("side"))
                    {
                        float factor = (renderer.OrthoWidth[2] / views.sizeX[3]);
                        Transformations.Transformations.Scale(currScene, x * factor, x * factor, x * factor);
                    }
                }
                // Skalowanie wzdłuż osi
                else if (contextMenu.MenuItems[3].Checked)
                {
                    if (e.Data.GetData("Viewport").Equals("front"))
                    {
                        float factor = (renderer.OrthoWidth[1] / views.sizeX[2]);
                        Transformations.Transformations.Scale(currScene, x * factor, y * factor, 0);
                    }
                    if (e.Data.GetData("Viewport").Equals("top"))
                    {
                        float factor = (renderer.OrthoWidth[0] / views.sizeX[1]);
                        Transformations.Transformations.Scale(currScene, x * factor, 0, y * factor);
                    }
                    if (e.Data.GetData("Viewport").Equals("side"))
                    {
                        float factor = (renderer.OrthoWidth[2] / views.sizeX[3]);
                        Transformations.Transformations.Scale(currScene, 0, y * factor, x * factor);
                    }
                }
                RenderViews();
            }

            dragX = (float)e.GetPosition(this).X;
            dragY = (float)e.GetPosition(this).Y;
        }


        ////////////////////////////////////////////////////////////////////////////////////
        // Sekcja odpowiadająca za interakcje DRAG
        private void ksztalt_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && ksztalty_ListView.SelectedIndex > -1)
            {
                System.Windows.DataObject dataObject = new System.Windows.DataObject();
                dataObject.SetData("Object", _shapesGallery.ElementAt(ksztalty_ListView.SelectedIndex));
                DragDrop.DoDragDrop(this, dataObject, System.Windows.DragDropEffects.Copy);
                //Console.WriteLine(_shapesCol.ElementAt(ksztalty_ListView.SelectedIndex).ToString());
            }
        }

        private void gotowe_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && gotowe_ListView.SelectedIndex > -1)
            {
                System.Windows.DataObject dataObject = new System.Windows.DataObject();
                dataObject.SetData("Object", _elementsGallery.ElementAt(gotowe_ListView.SelectedIndex));
                DragDrop.DoDragDrop(this, dataObject, System.Windows.DragDropEffects.Copy);
                //Console.WriteLine(_elementsCol.ElementAt(gotowe_ListView.SelectedIndex).ToString());
            }
        }

        private void materialy_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && materialy_ListView.SelectedIndex > -1)
            {
                System.Windows.DataObject dataObject = new System.Windows.DataObject();
                dataObject.SetData("Object", _surfaceGallery.ElementAt(materialy_ListView.SelectedIndex));
                DragDrop.DoDragDrop(this, dataObject, System.Windows.DragDropEffects.Copy);
                //Console.WriteLine(_elementsCol.ElementAt(gotowe_ListView.SelectedIndex).ToString());
            }
        }

        private void swiatla_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && swiatla_ListView.SelectedIndex > -1)
            {
                System.Windows.DataObject dataObject = new System.Windows.DataObject();
                dataObject.SetData("Object", _lightGallery.ElementAt(swiatla_ListView.SelectedIndex));
                DragDrop.DoDragDrop(this, dataObject, System.Windows.DragDropEffects.Copy);
                //Console.WriteLine(_elementsCol.ElementAt(gotowe_ListView.SelectedIndex).ToString());
            }
        }

        // Koniec sekcji
        ////////////////////////////////////////////////////////////////////////////////////

        private void objectToScene_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Object"))
            {
                object data = e.Data.GetData("Object");
                Console.WriteLine(data.GetType());
                ViewportInfo coords = GetViewCoords();
                if (data is Shape_)
                {
                    Shape_ shape = (Shape_)data;
                    undo.Save(currScene);
                    currScene.AddObject(shape.Triangulate((float)triang_Slider.Value), shape.GetType().ToString());
                    // przekształcenie współrzędnych
                    float x = (float)e.GetPosition(this).X - xOffset;
                    float y = (float)e.GetPosition(this).Y - yOffset;

                    float xtrans = 0, ytrans = 0, ztrans = 0;
                    float factor = (renderer.OrthoWidth[0] / coords.sizeX[1]);

                    // Lewy górny
                    if (x >= coords.posX[0] && x < coords.posX[0] + coords.sizeX[0] && y >= coords.posY[0] && y < coords.posY[0] + coords.sizeY[0])
                    {
                    }
                    // Prawy górny panel - gora (x, z)
                    else if (x >= coords.posX[1] && x < coords.posX[1] + coords.sizeX[1] && y >= coords.posY[1] && y < coords.posY[1] + coords.sizeY[1])
                    {
                        xtrans = (x - (coords.posX[1] + coords.sizeX[1] / 2)) / coords.sizeX[1] * renderer.OrthoWidth[0];
                        ytrans = renderer.OrthoLookAt[1].Y;
                        ztrans = (y - (coords.posY[1] + coords.sizeY[1] / 2)) / coords.sizeY[1] * (coords.sizeY[1] * renderer.OrthoWidth[0] / coords.sizeX[1]);
                    }
                    // Lewy dolny panel - przod (x, y)
                    else if (x >= coords.posX[2] && x < coords.posX[2] + coords.sizeX[2] && y >= coords.posY[2] && y < coords.posY[2] + coords.sizeY[2])
                    {
                        xtrans = (x - (coords.posX[2] + coords.sizeX[2] / 2)) / coords.sizeX[1] * renderer.OrthoWidth[1];
                        ztrans = renderer.OrthoLookAt[0].Z;
                        ytrans = - (y - (coords.posY[2] + coords.sizeY[2] / 2)) / coords.sizeY[2] * (coords.sizeY[2] * renderer.OrthoWidth[1] / coords.sizeX[2]);
                    }
                    // Prawy dolny panel - bok (z, y)
                    else if (x >= coords.posX[3] && x < coords.posX[3] + coords.sizeX[3] && y >= coords.posY[3] && y < coords.posY[3] + coords.sizeY[3])
                    {
                        ztrans = - (x - (coords.posX[3] + coords.sizeX[3] / 2)) / coords.sizeX[3] * renderer.OrthoWidth[2];
                        xtrans = renderer.OrthoLookAt[1].X;
                        ytrans = - (y - (coords.posY[3] + coords.sizeY[3] / 2)) / coords.sizeY[3] * (coords.sizeY[3] * renderer.OrthoWidth[2] / coords.sizeX[3]);
                    }

                    Transformations.Transformations.TranslateAddedObject(currScene, xtrans, ytrans, ztrans);

                    Console.WriteLine(currScene.points.Count() + " " + currScene.triangles.Count());
                }
                else if (data is PreparedElement)
                {
                    PreparedElement element = (PreparedElement)data;
                    Console.WriteLine("Dodanie do sceny");
                }
                else if (data is Surface)
                {
                    Surface surface = (Surface)data;
                    undo.Save(currScene);
                    bool zazn = true;
                    bool zastap = false;
                    for (int i = 0; i < currScene.materials.Count; i++) if (currScene.materials[i].name == surface.Material.Name)
                        {
                            zastap = true;
                            currScene.materials[i] = new Material_(surface.Material);
                        }
                    if (!zastap) currScene.materials.Add(new Material_(surface.Material));

                    if (currScene.selTriangles.Count == 0)
                    {
                        zazn = false;
                        ViewportInfo views = GetViewCoords();
                        int x = (int)e.GetPosition(this).X - xOffset;
                        int y = (int)e.GetPosition(this).Y - yOffset;

                        int rect = -1;

                        for (int i = 0; i < 4; ++i)
                        {
                            if (views.posX[i] <= x && x < views.posX[i] + views.sizeX[i] && views.posY[i] <= y && y < views.posY[i] + views.sizeY[i])
                            {
                                rect = i;
                            }
                        }


                        if (rect >= 0)
                        {
                            ViewportType viewportType = rect == 0 ? ViewportType.Perspective : ViewportType.Orto;
                            int orthoRect = rect == 0 ? rect : rect - 1;

                            SelectingElems.SelectElems(currScene, viewportType, new System.Drawing.Point(x - views.posX[rect], y - views.posY[rect]),
                                new System.Drawing.Point(views.sizeX[rect], views.sizeY[rect]), new Vector2(renderer.OrthoWidth[orthoRect], (float)views.sizeY[rect] / views.sizeX[rect] * renderer.OrthoWidth[orthoRect]),
                                renderer.OrthoPos[orthoRect], renderer.OrthoLookAt[orthoRect]);

                        }
                    }

                    for (int i = 0; i < currScene.parts.Count; i++)
                    {
                        foreach (HierarchyMesh obj in currScene.selTriangles)
                            if (currScene.parts[i].triangles.Contains((int)obj.triangles[0]))
                                currScene.materialAssign[i] = surface.Material.Name;
                    }
                    if (!zazn) currScene.selTriangles.Clear();
                    Console.WriteLine("Przypisanie atrybutów powierzchniowych");
                }
                else if (data is Material_)
                {
                    Material_ material = new Material_((Material_)data);
                    material.colorR = material.colorR / 255;
                    material.colorG = material.colorG / 255;
                    material.colorB = material.colorB / 255;
                    undo.Save(currScene);
                    bool zazn = true;
                    bool zastap = false;
                    for (int i = 0; i < currScene.materials.Count; i++) if (currScene.materials[i].name == material.Name)
                        {
                            zastap = true;
                            currScene.materials[i] = new Material_(material);
                        }
                    if (!zastap) currScene.materials.Add(new Material_(material));

                    if (currScene.selTriangles.Count == 0)
                    {
                        zazn = false;
                        ViewportInfo views = GetViewCoords();
                        int x = (int)e.GetPosition(this).X - xOffset;
                        int y = (int)e.GetPosition(this).Y - yOffset;

                        int rect = -1;

                        for (int i = 0; i < 4; ++i)
                        {
                            if (views.posX[i] <= x && x < views.posX[i] + views.sizeX[i] && views.posY[i] <= y && y < views.posY[i] + views.sizeY[i])
                            {
                                rect = i;
                            }
                        }


                        if (rect >= 0)
                        {
                            ViewportType viewportType = rect == 0 ? ViewportType.Perspective : ViewportType.Orto;
                            int orthoRect = rect == 0 ? rect : rect - 1;

                            SelectingElems.SelectElems(currScene, viewportType, new System.Drawing.Point(x - views.posX[rect], y - views.posY[rect]),
                                new System.Drawing.Point(views.sizeX[rect], views.sizeY[rect]), new Vector2(renderer.OrthoWidth[orthoRect], (float)views.sizeY[rect] / views.sizeX[rect] * renderer.OrthoWidth[orthoRect]),
                                renderer.OrthoPos[orthoRect], renderer.OrthoLookAt[orthoRect]);

                        }
                    }

                    for (int i = 0; i < currScene.parts.Count; i++)
                    {
                        foreach (HierarchyMesh obj in currScene.selTriangles)
                            if (currScene.parts[i].triangles.Contains((int)obj.triangles[0]))
                                currScene.materialAssign[i] = material.Name;
                    }
                    if (!zazn) currScene.selTriangles.Clear();
                    Console.WriteLine("Przypisanie atrybutów powierzchniowych");
                }
                else if (data is LightObj)
                {
                    LightObj light = (LightObj)data;
                    undo.Save(currScene);
                    Light_ sceneLight = new Light_(light.Light);
                    currScene.lights.Add(sceneLight);
                    currScene.selLights.Add(sceneLight);
                    Console.WriteLine("Dodanie światła");
                }

                else if (data is Light_)
                {
                    Light_ light = (Light_)data;
                    undo.Save(currScene);
                    Light_ sceneLight = new Light_(light);
                    currScene.lights.Add(sceneLight);
                    currScene.selLights.Add(sceneLight);
                }

            }

            e.Handled = true;

            trNumTBG1.Text = currScene.triangles.Count().ToString();
            trNumTBG2.Text = currScene.triangles.Count().ToString();
            trNumTBG3.Text = currScene.triangles.Count().ToString();
            trNumTBG4.Text = currScene.triangles.Count().ToString();

            RenderViews();
        }

        private void triang_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (triangValue_TextBox != null)
                triangValue_TextBox.Text = triang_Slider.Value.ToString();
        }

        private void triangValue_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (triangValue_TextBox.Text == null || triangValue_TextBox.Text == "") triangValue_TextBox.Text = "0,0001";
                if (Double.Parse(triangValue_TextBox.Text.Replace(".", ",")) > 1) triangValue_TextBox.Text = "1";
                triang_Slider.Value = Double.Parse(triangValue_TextBox.Text.Replace(".", ","));
                triangValue_TextBox.SelectionStart = triangValue_TextBox.Text.Length;
            }
            catch (Exception)
            {
                triangValue_TextBox.Text = triang_Slider.Value.ToString();
            }
        }

        private void Views_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (currScene.selCams.Count > 0 || currScene.selLights.Count > 0 || currScene.selTriangles.Count > 0)
                {
                    System.Windows.DataObject dataObject = new System.Windows.DataObject();
                    ViewportInfo coords = GetViewCoords();
                    undo.Save(currScene);

                    // Lewy górny panel
                    if (e.X >= coords.posX[0] && e.X < coords.posX[0] + coords.sizeX[0] && e.Y >= coords.posY[0] && e.Y < coords.posY[0] + coords.sizeY[0])
                    {
                        dataObject.SetData("Viewport", "perspective");
                    }
                    // Prawy górny panel
                    else if (e.X >= coords.posX[1] && e.X < coords.posX[1] + coords.sizeX[1] && e.Y >= coords.posY[1] && e.Y < coords.posY[1] + coords.sizeY[1])
                    {
                        dataObject.SetData("Viewport", "top");
                    }
                    // Lewy dolny panel
                    else if (e.X >= coords.posX[2] && e.X < coords.posX[2] + coords.sizeX[2] && e.Y >= coords.posY[2] && e.Y < coords.posY[2] + coords.sizeY[2])
                    {
                        dataObject.SetData("Viewport", "front");
                    }
                    // Prawy dolny panel
                    else if (e.X >= coords.posX[3] && e.X < coords.posX[3] + coords.sizeX[3] && e.Y >= coords.posY[3] && e.Y < coords.posY[3] + coords.sizeY[3])
                    {
                        dataObject.SetData("Viewport", "side");
                    }

                    dragX = (float)e.X + xOffset;
                    dragY = (float)e.Y + yOffset;
                    DragDrop.DoDragDrop(this, dataObject, System.Windows.DragDropEffects.Move);

                    mousePos.X = e.X;
                    mousePos.Y = e.Y;
                }
                else
                {
                    ViewportInfo views = GetViewCoords();
                    int x = (int)e.X;
                    int y = (int)e.Y;

                    int rect = -1;

                    for (int i = 0; i < 4; ++i)
                    {
                        if (views.posX[i] <= x && x < views.posX[i] + views.sizeX[i] && views.posY[i] <= y && y < views.posY[i] + views.sizeY[i])
                        {
                            rect = i;
                        }
                    }


                    if (rect >= 0)
                    {
                        ViewportType viewportType = rect == 0 ? ViewportType.Perspective : ViewportType.Orto;
                        int orthoRect = rect == 0 ? rect : rect - 1;

                        SelectingElems.SelectElems(currScene, viewportType, new System.Drawing.Point(x - views.posX[rect], y - views.posY[rect]),
                            new System.Drawing.Point(views.sizeX[rect], views.sizeY[rect]), new Vector2(renderer.OrthoWidth[orthoRect], (float)views.sizeY[rect] / views.sizeX[rect] * renderer.OrthoWidth[orthoRect]),
                            renderer.OrthoPos[orthoRect], renderer.OrthoLookAt[orthoRect]);

                    }


                    for (int i = 0; i < currScene.parts.Count; i++)
                    {
                        foreach (HierarchyMesh obj in currScene.selTriangles)
                            if (currScene.parts[i].triangles.Contains((int)obj.triangles[0]))
                            {
                                System.Windows.DataObject dataObject = new System.Windows.DataObject();
                                Material_ material = null;
                                foreach (Material_ m in currScene.materials)
                                {
                                    if (m.name == currScene.materialAssign[i])
                                    {
                                        material = new Material_(m);
                                        material.colorR = material.colorR * 255;
                                        material.colorG = material.colorG * 255;
                                        material.colorB = material.colorB * 255;
                                    }
                                }
                                dataObject.SetData("Object", material);
                                currScene.selTriangles.Clear();
                                DragDrop.DoDragDrop(this, dataObject, System.Windows.DragDropEffects.Move);
                                i = Int32.MaxValue-1;
                                break;
                            }
                    }
                    currScene.selTriangles.Clear();
                }
            }
        
        }

        private void NowyB_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult ifSave = System.Windows.MessageBox.Show("Czy chesz zapisać bieżącą scenę ?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (ifSave == MessageBoxResult.Yes)
            {
                SaveFile(sender, e);
            }

            //SurfaceRenderer.Render(new Material_("mat1", 0.6f, 0.95f, 0.5f, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 100, 1, 1, 1));
            currScene = null;
            currScene = new Scene();
            trNumTBG1.Text = currScene.triangles.Count().ToString();
            trNumTBG2.Text = currScene.triangles.Count().ToString();
            trNumTBG3.Text = currScene.triangles.Count().ToString();
            trNumTBG4.Text = currScene.triangles.Count().ToString();
            RenderViews();
        }

        private void undoClick(object sender, RoutedEventArgs e)
        {
            currScene = undo.Undo(currScene);
            RenderViews();
        }

        private void redoClick(object sender, RoutedEventArgs e)
        {
            currScene = undo.Redo(currScene);
            RenderViews();
        }

        private void materialy_ListView_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Object"))
            {
                object data = e.Data.GetData("Object");
                Console.WriteLine(data.GetType());
                if (data is Material_)
                {
                Material_ material=new Material_((Material_)data);
                material.colorR = material.colorR / 255;
                material.colorG = material.colorG / 255;
                material.colorB = material.colorB / 255;
                _surfaceGallery.Add(new Surface(material,"Ikony/Materialy/NiebieskieSzklo.png"));
                }
            }
        }

        private void swiatloGaleriaTab_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Object"))
            {
                object data = e.Data.GetData("Object");
                Console.WriteLine(data.GetType());
                if (data is Light_)
                {
                    Light_ light = new Light_((Light_)data);
                    if (light.type==Light_Type.Goniometric) _lightGallery.Add(new LightObj(light, "Ikony/Swiatla/lgt - goni.png"));
                    if (light.type == Light_Type.Point) _lightGallery.Add(new LightObj(light, "Ikony/Swiatla/lgt - point.png"));
                    if (light.type == Light_Type.Spot) _lightGallery.Add(new LightObj(light, "Ikony/Swiatla/lgt - spot.png"));
                }
            }
        }

        public void transformPanelButtonClick(float transx, float transy, float transz,
                                                float rotatex, float rotatey, float rotatez,
                                                float scalex, float scaley, float scalez)
        {
            Transformations.Transformations.Translate(currScene, transx, transy, transz);
            Console.WriteLine("Panel transformacji -przesunięcie: x " + transx + " y " + transy + " z " + transz);

            Transformations.Transformations.RotateOX(currScene, rotatex);
            Transformations.Transformations.RotateOY(currScene, rotatey);
            Transformations.Transformations.RotateOZ(currScene, rotatez);
            Console.WriteLine("Panel transformacji -obrót: x " + rotatex + " y " + rotatey + " z " + rotatez);

            Transformations.Transformations.Scale(currScene, scalex, scaley, scalez);
            Console.WriteLine("Panel transformacji -skalowanie: x " + scalex + " y " + scaley + " z " + scalez);

            RenderViews();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            _surfaceGallery.RemoveAt(materialy_ListView.SelectedIndex);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            _lightGallery.RemoveAt(swiatla_ListView.SelectedIndex);
        }

        //private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        DragDrop.DoDragDrop(this, sender, DragDropEffects.Move);
        //        Console.WriteLine(e.Source);
        //    }
        //}
    }
}