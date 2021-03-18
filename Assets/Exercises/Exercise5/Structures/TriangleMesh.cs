using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AfGD.Exercise5
{
    public class TriangleMesh
    {
        public int[] triangles;
        public Vector3[] vertices;
        public Color32[] colors;
        public Vector3[] normals;
        public BoneWeight[] weights;

        public TriangleMesh(Mesh mesh)
        {
            triangles = mesh.triangles;
            vertices = mesh.vertices;
            normals = mesh.normals;
            weights = mesh.boneWeights;
            colors = new Color32[vertices.Length];
        }

        public Mesh GetUnityMesh()
        {
            Mesh uMesh = new Mesh();
            uMesh.vertices = vertices;
            uMesh.colors32 = colors;
            uMesh.normals = normals;
            uMesh.triangles = triangles;
            uMesh.boneWeights = weights;
            return uMesh;
        }
    }
}
