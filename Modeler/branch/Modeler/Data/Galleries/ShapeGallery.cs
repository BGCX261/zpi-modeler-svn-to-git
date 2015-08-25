using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Modeler.Data.Shapes;

namespace Modeler.Data
{
    class ShapeGallery : ObservableCollection<Shape_>
    {
        public ShapeGallery()
            : base()
        {
            Add(new Sphere("Kula", "Ikony/Ksztalty/Kula.png"));
            Add(new Cone("Stozek", "Ikony/Ksztalty/Stozek.png"));
            Add(new Cube("Szescian", "Ikony/Ksztalty/Prostopadl.png"));
            Add(new Cylinder("Walec", "Ikony/Ksztalty/Walec.png"));
            Add(new Rectangle("Kwadrat", "Ikony/Ksztalty/Kwadrat.png"));
            Add(new TriangleShape("Trojkat", "Ikony/Ksztalty/Trojkat.png"));
        }
    }
}
