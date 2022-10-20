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
    }
    public class Vertex
    {
        public int index;
        public Vector3 position;
        public WingedEdge edge;
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
