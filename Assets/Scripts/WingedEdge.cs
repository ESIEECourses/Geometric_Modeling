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

            Vector3[] tabVertices = mesh.vertices;
            for (int i=0;i<tabVertices.Length; i++)
            {
                vertices.Add(new Vertex (i, tabVertices[i]));
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
