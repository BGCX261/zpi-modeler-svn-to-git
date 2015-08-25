using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Modeler.Data.Scene
{
    class Camera
    {
        public String name;
        public int resolutionX, resolutionY;
        public Vector3 position, lookAt;
        public float fovAngle;
        public float rotateAngle;

        public Camera(String name, int resolutionX, int resolutionY, Vector3 position, Vector3 lookAt, float fovAngle, float rotateAngle)
        {
            this.name = name;
            this.resolutionX = resolutionX;
            this.resolutionY = resolutionY;
            this.position = position;
            this.lookAt = lookAt;
            this.fovAngle = fovAngle;
            this.rotateAngle = rotateAngle;
        }
    }
}
