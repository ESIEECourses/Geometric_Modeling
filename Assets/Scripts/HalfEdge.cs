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
        public List<HalfEdge> GetEdges()
        {
            List<HalfEdge> faceEdges = new List<HalfEdge>();
            HalfEdge currEdge = null;
            HalfEdge startEdge = this.edge;
            faceEdges.Add(startEdge);
            //Edge CW

            currEdge = edge.nextEdge;
            while (currEdge != startEdge)
            {
                faceEdges.Add(currEdge);
                currEdge = currEdge.nextEdge;

            }
            return faceEdges;
        }
        public List<Vertex> GetVertex()
        {
            List<HalfEdge> faceEdges = GetEdges();
            List<Vertex> faceVertices = new List<Vertex>();
            //Vertice CW
            HalfEdge mon_Edge = null;
            for (int i = 0; i < faceEdges.Count; i++)
            {
                mon_Edge = faceEdges[i];
                faceVertices.Add(mon_Edge.sourceVertex);
            }
            return faceVertices;
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
                        HalfEdge twinEdge = new HalfEdge(edges.Count, startVertex, newFace, null, null, halfEdge);
                        halfEdge.twinEdge = twinEdge;
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
            GUIUtility.systemCopyBuffer = ConvertToCSVFormat("\t");
        }
        public Mesh ConvertToFaceVertexMesh()
        {
            Mesh faceVertexMesh = new Mesh();
            // magic happens
            return faceVertexMesh;
        }
        public string ConvertToCSVFormat(string separator = "\t")
        {
            if (this == null) return "";
            Debug.Log("#################      HalfEdgeMesh ConvertTOCSVFormat     #################");

            // Attributs

            string str = "";

            List<string> strings = new List<string>();

            // Récupération des vertices dans le fichier CSV

            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 pos = vertices[i].position;
                strings.Add(vertices[i].index + separator
                    + pos.x.ToString("N03") + " "
                    + pos.y.ToString("N03") + " "
                    + pos.z.ToString("N03") + separator
                    + vertices[i].outgoingEdge.index
                    + separator + separator);
            }

            for (int i = vertices.Count; i < edges.Count; i++)
                strings.Add(separator + separator + separator + separator);

            // Récupération des edges dans le fichier CSV

            for (int i = 0; i < edges.Count; i++)
            {
                strings[i] += edges[i].index + separator
                    + edges[i].sourceVertex.index + separator
                    + edges[i].face.index + separator
                    + edges[i].prevEdge.index + separator
                    + edges[i].nextEdge.index + separator
                    + edges[i].twinEdge.index + separator + separator;
            }

            // Récupération des faces dans le fichier CSV

            for (int i = 0; i < faces.Count; i++)
            {
                List<HalfEdge> faceEdges = faces[i].GetEdges();
                List<Vertex> faceVertex = faces[i].GetVertex();

                List<int> edgesIndex = new List<int>();
                List<int> vertexIndex = new List<int>();
                //Edge CW
                foreach (var edge in faceEdges)
                    edgesIndex.Add(edge.index);
                //Vertice CW
                foreach (var vertice in faceVertex)
                    vertexIndex.Add(vertice.index);


                strings[i] += faces[i].index + separator
                    + faces[i].edge.index + separator
                    + string.Join(" ", edgesIndex) + separator
                    + string.Join(" ", vertexIndex) + separator + separator;
            }

            // Mise en page du fichier CSV

            str = "Vertex" + separator + separator + separator + separator + "HalfEges" + separator + separator + separator + separator + separator + separator + separator + "Faces\n"
                + "Index" + separator + "Position" + separator + "outgoingEdge" + separator + separator +
                "Index" + separator + "sourceVertex" + separator + "Face" + separator + "prevEdge" + separator + "nextEdge" + separator + "twinEdge" + separator + separator +
                "Index" + separator + "Edge" + separator + "CW Edges" + separator + "CW Vertices\n"
                + string.Join("\n", strings);
            Debug.Log(str);
            return str;
        }
        public void DrawGizmos(bool drawVertices, bool drawEdges, bool drawFaces, Transform transform)
        {
            //magic happens
        }
    }
}