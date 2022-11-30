using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace HalfEdge
{
    public class HalfEdge
    {
        public int index;
        public Vertex sourceVertex;
        public Face face;
        public HalfEdge prevEdge;
        public HalfEdge nextEdge;
        public HalfEdge twinEdge;
        public HalfEdge(int index, Vertex sourceVertex, Face face, HalfEdge prevEdge = null, HalfEdge nextEdge = null, HalfEdge twinEdge = null)
        {
            this.index = index;
            this.sourceVertex = sourceVertex;
            this.face = face;
            this.prevEdge = prevEdge;
            this.nextEdge = nextEdge;
            this.twinEdge = twinEdge;
        }
    }
    public class Vertex
    {
        public int index;
        public Vector3 position;
        public HalfEdge outgoingEdge;
        public Vertex(int index, Vector3 position)
        {
            this.index = index;
            this.position = position;
        }
    }
    public class Face
    {
        public int index;
        public HalfEdge edge;
        public Face(int index)
        {
            this.index = index;
        }
    }
    public class HalfEdgeMesh
    {
        public List<Vertex> vertices;
        public List<HalfEdge> edges;
        public List<Face> faces;

        public bool isValid = false;
        public HalfEdgeMesh(Mesh mesh)
        { // constructeur prenant un mesh Vertex-Face en paramètre
            int nSides;
            switch (mesh.GetTopology(0))
            {
                case MeshTopology.Quads:
                    nSides = 4;
                    break;
                case MeshTopology.Triangles:
                    nSides = 3;
                    break;
                default:
                    isValid = false;
                    return;
            }
            isValid = true;
            vertices = new List<Vertex>();
            edges = new List<HalfEdge>();
            faces = new List<Face>();

            //Oncomplète les vertices de la même manière que WingedEdge
            Vector3[] tmpVertices = mesh.vertices;
            for (int i = 0; i < tmpVertices.Length; i++)
            {
                vertices.Add(new Vertex(i, tmpVertices[i]));
            }

            //On récupère les quads et on les parcourt pour trouver les meshs
            int[] indexes = mesh.GetIndices(0);
            Dictionary<ulong, HalfEdge> dicoHalfEdges = new Dictionary<ulong, HalfEdge>();
            //List<HalfEdge> faceHalfEdges = new List<HalfEdge>();

            for (int i = 0; i < indexes.Length / nSides; i++)
            {
                int[] faceIndexes = new int[nSides];
                for (int k = 0; k < nSides; k++)
                {
                    faceIndexes[k] = indexes[nSides * i + k];
                }

                Face newFace = new Face(faces.Count);
                //On complete la liste de face 
                faces.Add(newFace);

                //On complète d'abord toutes les edges de la face (sourceVertex face twinEdge)
                for (int j = 0; j < faceIndexes.Length; j++)
                {
                    int startIndex = faceIndexes[j];
                    int endIndex = faceIndexes[(j + 1) % faceIndexes.Length];

                    Vertex startVertex = vertices[startIndex];
                    Vertex endVertex = vertices[endIndex];
                    // creer un dictionnaire cle a laquell est associé une seule edge utiliser la méthode try get Value
                    ulong key = ((ulong)Mathf.Min(startIndex, endIndex)) + (((ulong)Mathf.Max(startIndex, endIndex)) << 32);

                    HalfEdge halfEdge = null;

                    if (!dicoHalfEdges.TryGetValue(key, out halfEdge))
                    {
                        halfEdge = new HalfEdge(edges.Count, startVertex, newFace);
                        edges.Add(halfEdge);
                        dicoHalfEdges.Add(key, halfEdge);
                    }
                    else
                    {
                        //Recréer une nouvelle Halfedge, qui sera cette fois une twinEdge
                        halfEdge = new HalfEdge(edges.Count, startVertex, newFace, null, null, halfEdge);
                        halfEdge.twinEdge = halfEdge;
                        edges.Add(halfEdge);
                    }
                    //On ajoute la edge associée à la face.
                    if (newFace.edge == null) newFace.edge = halfEdge;

                    //On complète le outgoingEdge du vertice
                    if (startVertex.outgoingEdge == null) startVertex.outgoingEdge = halfEdge;

                    //On complète maintenant les prevEdges et les nextEdges
                    if (j > 0)
                    {
                        halfEdge.prevEdge = edges[edges.Count - 1];
                        edges[edges.Count - 1].nextEdge = halfEdge;
                    }
                    //C'est la dernière HalfEdge de la face donc la suivante sera la première
                    if (j == 3)
                    {
                        halfEdge.nextEdge = edges[edges.Count - 4];
                        edges[edges.Count - 4].prevEdge = halfEdge;
                    }
                }
            }
        }
        public Mesh ConvertToFaceVertexMesh()
        {
            Mesh faceVertexMesh = new Mesh();
            // magic happens
            return faceVertexMesh;
        }
        public string ConvertToCSVFormat(string separator = "\t")
        {
            string str = "";
            //magic happens
            return str;
        }
        public void DrawGizmos(bool drawVertices, bool drawEdges, bool drawFaces)
        {
            //magic happens
        }
    }
}