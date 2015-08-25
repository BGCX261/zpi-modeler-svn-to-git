using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modeler.Transformations
{
    class Matrix
    {
        private float [,] matrix;

        public Matrix()
        {
            matrix = new float[4, 4] 
                {{1, 0, 0, 0},
                {0, 1, 0, 0},
                {0, 0, 1, 0},
                {0, 0, 0, 1}};
        }
    }
}
