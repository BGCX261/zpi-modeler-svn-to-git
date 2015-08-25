using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using Modeler.Data.Scene;
using Modeler.Data.Surfaces;

namespace Modeler.Data.Galleries
{
    class SurfaceGallery : ObservableCollection<Surface>
    {
        public SurfaceGallery()
            : base()
        {
            LoadGallery();
            Add(new Surface(new Material_("Niebieskie szło", 122 / 255.0f, 131 / 255.0f, 255 / 255.0f,
                                            0.5f, 0.3f, 0.48f,
                                            0.67f, 0.69f, 0.44f,
                                            0.9f, 0.9f, 0.9f,
                                            0, 0, 0,
                                            205, 1.66f),
                                            "Ikony/Materialy/NiebieskieSzklo.png"));
            Add(new Surface(new Material_("Drewno", 167 / 255.0f, 104 / 255.0f, 53 / 255.0f,
                                            1, 1, 1,
                                            0, 0, 0,
                                            0, 0, 0,
                                            0, 0, 0,
                                            205, 0),
                                            "Ikony/Materialy/Drewno.png"));
            Add(new Surface(new Material_("Lustro", 122 / 255.0f, 131 / 255.0f, 255 / 255.0f,
                                            0.5f, 0.3f, 0.48f,
                                            0.67f, 0.69f, 0.44f,
                                            0.9f, 0.9f, 0.9f,
                                            0, 0, 0,
                                            205, 1.66f),
                                            "Ikony/Materialy/Lustro.png"));
        }

        public void LoadGallery()
        {
            List<Material_> materials = Material_.LoadMaterials("../../galleries/MaterialsGallery.txt"); // TODO poprawić ścieżkę w finalnej wersji

            if(materials == null)
            {
                SaveGallery();
            }
            else
            {
                foreach(Material_ material in materials)
                {
                    Add(new Surface(material, "Ikony/Swiatla/" + material.name + ".png"));
                }
            }
        }

        public void SaveGallery()
        {
            List<Material_> materials = new List<Material_>();

            foreach(Surface material in Items)
            {
                materials.Add(material.Material);

                // TODO zapisać ikonę
            }

            Material_.SaveMaterials(materials, "../../galleries/MaterialsGallery.txt"); // TODO poprawić ścieżkę w finalnej wersji
        }
    }
}
