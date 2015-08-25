using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using Modeler.Data.Elements;

namespace Modeler.Data
{
    class PreparedObjectsGallery : ObservableCollection<PreparedElement>
    {
        public PreparedObjectsGallery()
            : base()
        {
            LoadGallery();
            /*Add(new PreparedElement("Krzeslo", "/Ikony/GotoweElementy/Krzeslo.png"));
            Add(new PreparedElement("Stol", "/Ikony/GotoweElementy/Stol.png"));
            Add(new PreparedElement("Lampa", "/Ikony/GotoweElementy/Lampa.png"));*/
        }

        public void LoadGallery()
        {
            string currDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string fullPath = Path.Combine(currDirectory, "../../galleries/objects");

            string[] paths = Directory.GetFiles(fullPath); // TODO poprawić ścieżkę w finalnej wersji

            foreach(string path in paths)
            {
                string file = Path.GetFileNameWithoutExtension(path);

                Scene.Scene scene = Scene.Scene.ReadSceneFromFile(path);
                if(scene != null)
                {
                    Add(new PreparedElement(file, "../../Ikony/GotoweElementy/" + file + ".png", scene)); // TODO poprawić ścieżkę w finalnej wersji
                }
            }
        }

        // obiekty do galerii gotowych obiektów zapisywane są oddzielnie
        // element w scene zawiera wydzielony obiekt ze sceny
        public void SaveObjectToGallery(PreparedElement element)
        {
            element.Scene.SaveSceneFile("../../galleries/objects/" + element.Name + ".txt"); // TODO poprawić ścieżkę w finalnej wersji

            // TODO zapisać ikonę
        }
    }
}
