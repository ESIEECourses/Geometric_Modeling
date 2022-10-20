using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WingedEdge
{
    public class WingedEdge
    {
        public int index;
        public Vertex startVertex;
        public Vertex endVertex;
        public Face leftFace;
        public Face rightFace;
        public WingedEdge startCWEdge;
        public WingedEdge startCCWEdge;
        public WingedEdge endCWEdge;
        public WingedEdge endCCWEdge;

        public WingedEdge(int a, int b, int c, int d)
        {
            index = 1;
        }
    }
    public class Vertex
    {
        public int index;
        public Vector3 position;
        public WingedEdge edge;
        public Vertex(int index, Vector3 vertex)
        {
            this.index = index;
            this.position = vertex;
        }
    }
    public class Face
    {
        public int index;
        public WingedEdge edge;
    }
    public class WingedEdgeMesh
    {
        public List<Vertex> vertices;
        public List<WingedEdge> edges;
        public List<Face> faces;
        public WingedEdgeMesh(Mesh mesh)
        {// constructeur prenant un mesh Vertex-Face en paramètre
         // magic happens
            vertices = new List<Vertex>();
            edges = new List<WingedEdge>();
            faces = new List<Face>();
            Vector3[] tabVertices = mesh.vertices;
            for (int i = 0; i < tabVertices.Length; i++)
            {
                Debug.Log("Vertex : " + i + " : " +tabVertices[i]);
                vertices.Add(new Vertex(i, tabVertices[i]));
            }
            
            //On récupère les quads et on les parcourt pour trouver les meshs
            int[] quads = mesh.GetIndices(0);

            for (int i = 0; i < quads.Length / 4; i++)
            {
                int index1 = quads[4 * i];
                int index2 = quads[4 * i + 1];
                int index3 = quads[4 * i + 2];
                int index4 = quads[4 * i + 3];
                edges.Add(new WingedEdge(index1, index2, index3, index4));

                ulong key1 = ((ulong)Mathf.Min(index1, index2)) + (((ulong)Mathf.Max(index1, index2)) << 32);
                ulong key2 = ((ulong)Mathf.Min(index2, index3)) + (((ulong)Mathf.Max(index2, index3)) << 32);
                ulong key3 = ((ulong)Mathf.Min(index3, index4)) + (((ulong)Mathf.Max(index3, index4)) << 32);
                ulong key4 = ((ulong)Mathf.Min(index4, index1)) + (((ulong)Mathf.Max(index4, index1)) << 32);


            }

        }
        public Mesh ConvertToFaceVertexMesh()
        {
            Mesh faceVertexMesh = new Mesh();
            // magic happens
            return faceVertexMesh;
        }
        /*string ConvertToCSV(string separator)
        {
            if (!(m_Mf && m_Mf.mesh)) return "";

            Vector3[] vertices = m_Mf.mesh.vertices;
            int[] quads = m_Mf.mesh.GetIndices(0);

            List<string> strings = new List<string>();

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 pos = vertices[i];
                strings.Add(i.ToString() + separator + pos.x.ToString("N03") + " " + pos.y.ToString("N03") + " " + pos.z.ToString("N03") + separator + separator);

            }
            for (int i = vertices.Length; i < quads.Length / 4; i++)
                strings.Add(separator + separator + separator);

            for (int i = 0; i < quads.Length / 4; i++)
            {
                strings[i] += i.ToString() + separator
                    + quads[4 * i + 0].ToString() + ","
                    + quads[4 * i + 1].ToString() + ","
                    + quads[4 * i + 2].ToString() + ","
                    + quads[4 * i + 3].ToString();
            }

            return "Vertices" + separator + separator + separator + "Faces\n"
                + "Index" + separator + "Position" + separator + separator + "Index" + separator + "Indices des vertices\n"
                + string.Join("\n", strings);
        }

        public string ConvertToCSVFormat(string separator = "\t")
        {
            string str = "";
            //magic happens
            GUIUtility.systemCopyBuffer = ConvertToCSV(separator);
            Debug.Log(ConvertToCSV("\t"));
            return str;
        }*/
        public void DrawGizmos(bool drawVertices, bool drawEdges, bool drawFaces)
        {
            //magic happens
        }
    }
}
