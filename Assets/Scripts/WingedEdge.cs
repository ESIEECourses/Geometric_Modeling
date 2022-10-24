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

        public WingedEdge(int a, Vertex startVertex, Vertex endVertex, Face rightface)
        {
            this.index = a;
            this.startVertex = startVertex;
            this.endVertex = endVertex;
            this.rightFace = rightface;
        }
        public void setleftFace(Face leftFace)
        {
            this.leftFace = leftFace;
        }
        public void setstartCWEdge(WingedEdge startCWEdge)
        {
            this.startCWEdge = startCWEdge;
        }
        public void setstartCCWEdge(WingedEdge startCCWEdge)
        {
            this.startCCWEdge = startCCWEdge;
        }
        public void setendCWEdge(WingedEdge endCWEdge)
        {
            this.endCWEdge = endCWEdge;
        }
        public void setCCWEdge(WingedEdge endCCWEdge)
        {
            this.endCCWEdge = endCCWEdge;
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
        public void setEdge(WingedEdge edge)
        {
            this.edge = edge;
        }
    }
    public class Face
    {
        public int index;
        public WingedEdge edge;
        public Face(int index)
        {
            this.index = index;
        }
        public void setWingedEdge(WingedEdge edge)
        {
            this.edge = edge;
        }
    }
    public class WingedEdgeMesh
    {
        public List<Vertex> vertices;
        public List<WingedEdge> edges;
        public List<Face> faces;
        public WingedEdgeMesh(Mesh mesh)
        {// constructeur prenant un mesh Vertex-Face en param�tre
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
            
            //On r�cup�re les quads et on les parcourt pour trouver les meshs
            int[] quads = mesh.GetIndices(0);
            Dictionary<ulong, WingedEdge> keys = new Dictionary<ulong, WingedEdge>();
            for (int i = 0; i < quads.Length / 4; i++)
            {
                int [] indexes = new int [4];
                indexes[0] = quads[4 * i];
                indexes[1] = quads[4 * i + 1];
                indexes[2] = quads[4 * i + 2];
                indexes[3] = quads[4 * i + 3];


                //On complete la liste de face 
                faces.Add(new Face(i));

                //On complete startVertex, endVertex, leftFace et rightFace
                for (int j=0; j < vertices.Count; j++)
                {
                    // creer un dictionnaire cle a laquell est associ� une seule edge utiliser la m�thode try get Value
                   
                    ulong key = ((ulong)Mathf.Min(indexes[j], indexes[j+1])) + (((ulong)Mathf.Max(indexes[j], indexes[j+1])) << 32);
                
                    WingedEdge temp = null;

                    if (keys.TryGetValue(key,out temp) == false)
                    {
                        edges.Add(new WingedEdge(j, vertices[indexes[j]], vertices[indexes[j++]], faces[i]));
                        keys.Add(key,edges[j]);
                    }
                    else
                    {
                        //Compl�te les info manquante comme la leftFace par exemple(je te laissse r�fl�chir)
                    }
                    //Puis apres avoir tous les Face vertices et edges.
                    //Il manque plus qu'� compl�ter les CCW et CW
                }
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
