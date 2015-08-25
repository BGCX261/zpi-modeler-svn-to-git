using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modeler.Data.Scene;

namespace Modeler.Data.Surfaces
{
    class Surface
    {
        private Material_ material;
        private String imageUri;
        public Material_ Material
        {
            get { return material; }
            set { material = Material; }
        }
        public String ImageUri
        {
            get { return imageUri; }
            set { imageUri = ImageUri; }
        }

        public Surface(Material_ material, String imgUri)
        {
            this.material = material;
            imageUri = imgUri;
        }
    }
}
