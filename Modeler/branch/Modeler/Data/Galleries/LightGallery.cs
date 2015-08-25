using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using Modeler.Data.Scene;
using Modeler.Data.Light;


namespace Modeler.Data.Galleries
{
    class LightGallery : ObservableCollection<LightObj>
    {
        public LightGallery()
            : base()
        {
            LoadGallery();
            SortedList<float, float> gonio = new SortedList<float, float>();
            gonio.Add(-90, 0);
            gonio.Add(-60, 0);
            gonio.Add(-30, 0.5f);
            gonio.Add(-5, 0.9f);
            gonio.Add(0, 1f);
            gonio.Add(5, 0.5f);
            gonio.Add(30, 0.5f);
            gonio.Add(60, 0);
            gonio.Add(90, 0);

            Add(new LightObj(new Light_("Point", Light_Type.Point, true, 
                                        1, 1, 1, 3, new SlimDX.Vector3(0, 0, 0),
                                        new SlimDX.Vector3(), 0, 0, null), "Ikony/Swiatla/lgt - point.png"));
            Add(new LightObj(new Light_("Spot", Light_Type.Spot, true,
                                        1, 1, 1, 3, new SlimDX.Vector3(0, 0, 0),
                                        new SlimDX.Vector3(1, 1, 1), 30, 90, null), "Ikony/Swiatla/lgt - spot.png"));
            Add(new LightObj(new Light_("Goniometric", Light_Type.Point, true,
                                        1, 1, 1, 3, new SlimDX.Vector3(0, 0, 0),
                                        new SlimDX.Vector3(1, 1, 1), 0, 0, gonio), "Ikony/Swiatla/lgt - goni.png"));
        }

        public void LoadGallery()
        {
            List<Light_> lights = Light_.LoadLights("../../galleries/LightsGallery.txt"); // TODO poprawić ścieżkę w finalnej wersji

            if(lights == null)
            {
                SaveGallery();
            }
            else
            {
                foreach(Light_ light in lights)
                {
                    Add(new LightObj(light, "Ikony/Swiatla/" + light.name + ".png"));
                }
            }
        }

        public void SaveGallery()
        {
            List<Light_> lights = new List<Light_>();

            foreach(LightObj light in Items)
            {
                lights.Add(light.Light);

                // TODO zapisać ikonę
            }

            Light_.SaveLights(lights, "../../galleries/LightsGallery.txt"); // TODO poprawić ścieżkę w finalnej wersji
        }
    }
}
