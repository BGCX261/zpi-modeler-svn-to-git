using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modeler.Data.Scene
{
    class Hierarchy
    {
        public List<HierarchyObject> objects;

        public Hierarchy()
        {
            objects = new List<HierarchyObject>();
        }

        public override string ToString()
        {
            string result = "";
            foreach (HierarchyObject obj in objects)
            {
                result += obj.ToString();
            }
            return result;
        }

        public HierarchyMesh GetMesh(string name)
        {
            return GetMeshRec(objects, name);
        }

        public List<HierarchyMesh> GetAllMeshes()
        {
            List<HierarchyMesh> meshes = new List<HierarchyMesh>();

            GetAllMeshesRec(objects, meshes);

            return meshes;
        }

        private void GetAllMeshesRec(List<HierarchyObject> objs, List<HierarchyMesh> outMeshes)
        {
            foreach(HierarchyObject obj in objs)
            {
                if(obj is HierarchyMesh)
                {
                    outMeshes.Add((HierarchyMesh)obj);
                }
                else if(obj is HierarchyNode)
                {
                    GetAllMeshesRec(((HierarchyNode)obj).hObjects, outMeshes);
                }
            }
        }

        private HierarchyMesh GetMeshRec(List<HierarchyObject> objs, string name)
        {
            foreach(HierarchyObject obj in objs)
            {
                if(obj is HierarchyMesh)
                {
                    if(((HierarchyMesh)obj).name == name)
                    {
                        return (HierarchyMesh)obj;
                    }
                }
                else if(obj is HierarchyNode)
                {
                    return GetMeshRec(((HierarchyNode)obj).hObjects, name);
                }
            }

            return null;
        }
    }

    abstract class HierarchyObject
    {
        public String name;

        public HierarchyObject(String name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name + " ";
        }
    }

    class HierarchyNode : HierarchyObject
    {
        public List<HierarchyObject> hObjects;

        public HierarchyNode(String name) : base(name)
        {
            hObjects = new List<HierarchyObject>();
        }
    }

    class HierarchyMesh : HierarchyObject
    {     
        public List<uint> triangles;

        public HierarchyMesh(String name) : base(name)
        {
            triangles = new List<uint>();
        }
    }

    class HierarchyLight : HierarchyObject
    {
        public Light_ light;

        public HierarchyLight(Light_ light) : base(light.name)
        {
            this.light = light;
        }
    }
}
