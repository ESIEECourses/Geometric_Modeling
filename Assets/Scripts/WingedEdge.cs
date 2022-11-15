using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Collections;

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
        public void setendCCWEdge(WingedEdge endCCWEdge)
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
                //Debug.Log("Vertex : " + i + " : " +tabVertices[i]);
                vertices.Add(new Vertex(i, tabVertices[i]));
            }
            
            //On r�cup�re les quads et on les parcourt pour trouver les meshs
            int[] quads = mesh.GetIndices(0);
            Dictionary<ulong, WingedEdge> dicoWingedEdges = new Dictionary<ulong, WingedEdge>();
            for (int i = 0; i < quads.Length / 4; i++)
            {
                int [] indexes = new int [4];
                indexes[0] = quads[4 * i];
                indexes[1] = quads[4 * i + 1];
                indexes[2] = quads[4 * i + 2];
                indexes[3] = quads[4 * i + 3];

                
                //On complete la liste de face 
                faces.Add(new Face(i));
                Debug.Log(edges.Count);
                //On complete startVertex, endVertex, leftFace et rightFace
                for (int j=0; j < 4; j++)
                {
                    // creer un dictionnaire cle a laquell est associ� une seule edge utiliser la m�thode try get Value
                   
                    ulong key = ((ulong)Mathf.Min(indexes[j], indexes[(j+1)%4])) + (((ulong)Mathf.Max(indexes[j], indexes[(j+1)%4])) << 32);
                
                    WingedEdge temp = null;
                    Debug.Log("zou");
                    if (dicoWingedEdges.TryGetValue(key, out temp) == false)
                    {
                        WingedEdge myNewEdge = new WingedEdge(edges.Count, vertices[indexes[j]], vertices[indexes[(j + 1) % 4]], faces[i]);
                        edges.Add(myNewEdge);
                        dicoWingedEdges.Add(key, edges[edges.Count - 1 ]);
                        faces[i].setWingedEdge(edges[edges.Count - 1 ]);
                        vertices[indexes[j]].setEdge(edges[edges.Count - 1]);
                    }
                    else
                    {
                        //Compl�te les info manquante la leftFace 
                        edges[temp.index].setleftFace(faces[i]);
                    }
                    Debug.Log("zi");
                    //Puis apres avoir tous les Face vertices et edges.
                    //Il manque plus qu'� compl�ter les CCW et CW
                    /*edges[j].setstartCCWEdge(edges[j]);
                    edges[j].setstartCWEdge(edges[j]);
                    edges[j].setendCCWEdge(edges[j]);
                    edges[j].setendCWEdge(edges[j]);*/
                }
            }
            ////vertices
            string p = "Vertex - edges : \n";
            foreach (Vertex x in vertices)
            {
                p += "V" + x.index.ToString() + ": " + x.position.ToString() + " | e" + x.edge.index + " \n";
            }
            Debug.Log(p);
            ////faces
            p = "Faces - edges : \n";
            foreach (Face x in faces)
            {
                //p += x.index.ToString() + ": F" + x.index.ToString() + " - e "+ x.edge.index + " \n";
                p += x.index.ToString() + ": F" + x.index.ToString() + " - e \n";
            }
            Debug.Log(p);

            ////wingedEdge

            p = "WingedEdges : \n";
            foreach (WingedEdge x in edges)
            {
                p += $"e{x.index} : V{x.startVertex.index} - V{x.endVertex.index}| {(x.leftFace == null ? "NoLeftFace" : $"F{x.leftFace.index}")} | {(x.rightFace == null ? "NoRightFace" : $"F{x.rightFace.index}")}\n"; //| SCCW : e{x.startCCWEdge.index} - SCW : e{x.startCWEdge.index} - ECW : e{x.endCWEdge.index} - ECCW : e{x.endCCWEdge.index}\n";
            }
            Debug.Log(p);
        }
        public Mesh ConvertToFaceVertexMesh()
        {
            Mesh faceVertexMesh = new Mesh();
            // magic happens
            return faceVertexMesh;
        }
        public string ConvertToCSVFormat(string separator="\t")
        {
            string str = "";
            //magic happens
            return str;
        }
        public void DrawGizmos(bool drawVertices,bool drawEdges,bool drawFaces)
        {
            GUIStyle style = new GUIStyle();
            
            if(drawVertices){
                style.fontSize = 12;
                style.normal.textColor = Color.red;
                Gizmos.color = Color.orange;

                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3 worldPos =  transform.TransformPoint(vertices[i]);
                    Gizmos.DrawSphere(worldPos, 0.5f); // Points oranges 
                    Handles.Label(worldPos, i.ToString(), style);
                }
            }
            if(drawEdges || drawFaces){
                for (int i = 0; i < quads.Length/4; i++)
                {
                    int index1 = quads[4 * i];
                    int index2 = quads[4 * i+1];
                    int index3 = quads[4 * i+2];
                    int index4 = quads[4 * i+3];

                    Vector3 pt1 = transform.TransformPoint(vertices[index1]);
                    Vector3 pt2 = transform.TransformPoint(vertices[index2]);
                    Vector3 pt3 = transform.TransformPoint(vertices[index3]);
                    Vector3 pt4 = transform.TransformPoint(vertices[index4]);

                    if(drawEdges){
                        Gizmos.color = Color.black;
                        Gizmos.DrawLine(pt1, pt2);
                        Gizmos.DrawLine(pt2, pt3);
                        Gizmos.DrawLine(pt3, pt4);
                        Gizmos.DrawLine(pt4 , pt1);
                    }
                    if(drawFaces){
                        style.fontSize = 10;
                        style.normal.textColor = Color.blue;
                        string str = string.Format("{0} ({1},{2},{3},{4})", i, index1, index2, index3, index4);

                        Handles.Label((pt1 + pt2 + pt3 + pt4) / 4.0f, str, style);
                    }
                }
            }
        }
    }
}
