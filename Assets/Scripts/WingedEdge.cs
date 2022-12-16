using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

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

        public WingedEdge(int index, Vertex startVertex, Vertex endVertex, Face rightFace, Face leftFace, WingedEdge startCWEdge = null, WingedEdge startCCWEdge = null, WingedEdge endCWEdge = null, WingedEdge endCCWEdge = null)
        {
            this.index = index;
            this.startVertex = startVertex;
            this.endVertex = endVertex;
            this.rightFace = rightFace;
            this.leftFace = leftFace;
            this.startCWEdge = startCWEdge;
            this.startCCWEdge = startCCWEdge;
            this.endCWEdge = endCWEdge;
            this.endCCWEdge = endCCWEdge;
            
        }
        public WingedEdge FindEndCWBorder()
        {
            //Nous avons trois cas diff�rents, dont deux o� il n'y a pas de leftface pour la edge courante
            //Le troisi�me cas est d�j� trait�e par la premi�re boucle du cosntructeur

            //Par cons�quent si pas de LeftFace, on skip
            if (this.leftFace != null)
            {
                return null;
            }
            WingedEdge endCW = this.endCCWEdge;
            while(endCW.leftFace!=null)
            {
                if(endCW.endVertex == this.endVertex)
                {
                    endCW = endCW.endCCWEdge;
                }
                else
                {
                    endCW = endCW.startCCWEdge;
                }
            }
            return endCW;

        }
        public WingedEdge FindStartCCWBorder()
        {
            //De m�me que FindEndCWBorder en parcourant dans l'autre sens
            if (this.leftFace != null)
            {
                return null;
            }
            WingedEdge startCCW = this.startCWEdge;
            while (startCCW.leftFace != null)
            {
                if (startCCW.startVertex == this.startVertex)
                {
                    startCCW = startCCW.startCCWEdge;
                }
                else
                {
                    startCCW = startCCW.endCWEdge;
                }
            }
            return startCCW;
        }

        public List<WingedEdge> GetAdjEdges() 
        {
            WingedEdge currentEdge = edge;
            List<WingedEdge> adjEdges = new List<WingedEdge>();
            do
            {
                adjEdges.Add(currentEdge);
                currentEdge = (this == currentEdge.startVertex) ? currentEdge.startCWEdge : currentEdge.endCWEdge;
            } while (currentEdge != edge);

            return adjEdges;
        }

        public List<WingedEdge> GetBorderEdges() 
        {
            List<WingedEdge> borderEdges = new List<WingedEdge>();
            List<WingedEdge> adjEdges = GetAdjEdges();
            for (int i = 0; i < adjEdges.Count; i++)
            {
                if (adjEdges[i].leftFace == null) 
                { 
                    borderEdges.Add(adjEdges[i]); 
                }
            }

            return borderEdges;
        }

    }
    public class Vertex
    {
        public int index;
        public Vector3 position;
        public WingedEdge edge;
        public Vertex(int index, Vector3 position)
        {
            this.index = index;
            this.position = position;
        }
        public Vector3 GetPosition()
        {
            return this.position;
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
        public List<WingedEdge> GetEdges()
        {
            List<WingedEdge> faceEdges = new List<WingedEdge>();
            WingedEdge currEdge = null;
            WingedEdge startEdge = this.edge;
            faceEdges.Add(startEdge);
            if (this == edge.rightFace)
            {

                currEdge = edge.endCCWEdge;
            }
            else
            {
                currEdge = edge.startCCWEdge;
            }
            while (currEdge != startEdge)
            {
                faceEdges.Add(currEdge);
                if (this == currEdge.rightFace)
                {

                    currEdge = currEdge.endCCWEdge;
                }
                else
                {
                    currEdge = currEdge.startCCWEdge;
                }
            }
            return faceEdges;
        }

        public List<Vertex> GetVertex()
        {
            List<WingedEdge> faceEdges = GetEdges();
            List<Vertex> faceVertices = new List<Vertex>();
            WingedEdge mon_Edge = null;
            for (int i = 0; i < faceEdges.Count; i++)
            {
                mon_Edge = faceEdges[i];
                if (mon_Edge.rightFace == this)
                {
                    faceVertices.Add(mon_Edge.startVertex);
                }
                else
                {
                    faceVertices.Add(mon_Edge.endVertex);
                }
            }
            return faceVertices;
        }

        public List<Face> GetAdjFaces()
        {
            List<Face> adjFaces = new List<Face>();
            List<WingedEdge> adjEdges = GetAdjacentEdges();
            for (int i = 0; i < adjEdges.Count; i++)
            {
                //ignore les edges en bordure dirigés vers la vertice courante
                if ( !(adjEdges[i].leftFace == null && this == adjEdges[i].endVertex ))
                {
                    //ajoute la bonne face en fonction de la direction de l'edge par rapport à la vertice courante
                    adjFaces.Add((this == adjEdges[i].startVertex) ? adjEdges[i].rightFace : adjEdges[i].leftFace);
                }
            }
            return adjFaces;
        }
    }
    public class WingedEdgeMesh
    {
        public List<Vertex> vertices = null;
        public List<WingedEdge> edges = null;
        public List<Face> faces = null;
        //MY CODE


        //Recherches des Edges adjacents
        
        public void SubdivideCatmullClark()
        {
            List<Vector3> facePoints = new List<Vector3>();
            List<Vector3> edgePoints = new List<Vector3>();
            List<Vector3> vertexPoints = new List<Vector3>();

            //facePoints.Add(new Vector3(-1.5f,4.0f,.5f));
            CatmullClarkCreateNewPoints(out facePoints,out edgePoints,out vertexPoints);
        }

    




        public void CatmullClarkCreateNewPoints(out List<Vector3> facePoints, out List<Vector3> edgePoints, out List<Vector3> vertexPoints)
        {
            facePoints = new List<Vector3>();
            ////STEP 1/////
            //parcours de toutes les faces de WindegEdgeMesh
            for(int f = 0; f<faces.Count(); ++f){    

                //Recupération des points de la face courante
                List<Vertex> vertexCurrentFace = faces[f].GetVertex();
                
                //Recuperation des coordonnées de la liste de points de la face courante
                List<Vector3> positionsVertexCurrentFace = new List<Vector3>();
                for(int v = 0; v<vertexCurrentFace.Count(); ++v){
                    positionsVertexCurrentFace.Add(vertexCurrentFace[v].GetPosition())   ;
                }
            
                //C0 = Somme de tous les points de la face courante/nb points
                float sumX = 0,sumY = 0,sumZ = 0,nbPoints = 0;
                for(int p = 0; p<positionsVertexCurrentFace.Count(); ++p){
                    sumX += positionsVertexCurrentFace[p].x;
                    sumY += positionsVertexCurrentFace[p].y;
                    sumZ += positionsVertexCurrentFace[p].z;
                    nbPoints += 1;
                }
                Vector3 C0 = new Vector3(   sumX/nbPoints,
                                            sumY/nbPoints,
                                            sumZ/nbPoints);
                
                //Ajout du point C0 (Face point) de la face courante dans la liste  
                facePoints.Add(C0);       
            }
            ///FIN STEP 1////

            List<Vector3> midPoints = new List<Vector3>();
            edgePoints = new List<Vector3>();
            ///STEP 2////
            //parcours de tous les edges de WindegEdgeMesh
            for(int e = 0; e<edges.Count(); ++e){
                
                //E0 = Somme des points de l'edge et des facepoints des faces adjacentes
                float sumX = 0,sumY = 0,sumZ = 0;
                sumX = edges[e].startVertex.GetPosition().x + edges[e].endVertex.GetPosition().x + facePoints[edges[e].leftFace.index].x + facePoints[edges[e].rightFace.index].x;
                sumY = edges[e].startVertex.GetPosition().y + edges[e].endVertex.GetPosition().y + facePoints[edges[e].leftFace.index].y + facePoints[edges[e].rightFace.index].y;
                sumZ = edges[e].startVertex.GetPosition().z + edges[e].endVertex.GetPosition().z + facePoints[edges[e].leftFace.index].z + facePoints[edges[e].rightFace.index].z;
                Vector3 E0 = new Vector3(   sumX/4,
                                            sumY/4,
                                            sumZ/4);
                //Ajout du point E0 (Edge point) de l'edge courant dans la liste
                edgePoints.Add(E0);

                //Calcul mid points
                float midX = 0,midY = 0,midZ = 0;
                midX = edges[e].startVertex.GetPosition().x + edges[e].endVertex.GetPosition().x;
                midY = edges[e].startVertex.GetPosition().y + edges[e].endVertex.GetPosition().y;
                midZ = edges[e].startVertex.GetPosition().z + edges[e].endVertex.GetPosition().z;
                Vector3 m0 = new Vector3(   midX,
                                            midY,
                                            midZ);
                //Ajout du point m0 (Mid point) de l'edge courant dans la liste
                midPoints.Add(m0);
            }
            ///FIN STEP 2////           

            vertexPoints = new List<Vector3>();                       
            ///STEP 3////
            //parcours de tous les vertices de WindegEdgeMesh
            for(int v = 0; v<vertices.Count(); ++v){
                
                
            }

                // //PARCOURS DE LA LISTE DE FACES ADJACENTES 
                // //Récupération des edges des faces adjacantes 
                
                // for(int a = 0 ; a < adjFaces.Count() ; ++a)
                // {
                //     List<WingedEdge> edgesFromAdjFaces = adjFaces[a].GetEdges();
                    
                //     //Récupération des vertex des edges adjacantes
                //     for(int e = 0; e < edgesFromAdjFaces.Count(); ++e)
                //     {
                //         List<Vertex> vertexFromEdgesFromAdjFaces = new List<Vertex>();
                //         if(!vertexFromEdgesFromAdjFaces.Contains(edgesFromAdjFaces.GetFirstVertice())){
                //             vertexFromEdgesFromAdjFaces.Add(edgesFromAdjFaces.GetFirstVertice());
                //         }
                //         //Recuperation des coordonnées de la liste de points de la face courante
                //         List<Vector3> positionsFromVertexFromEdgesFromAdjFaces = new List<Vector3>();
                //         for(int v = 0; v<vertexFromEdgesFromAdjFaces.Count(); ++v){
                //             positionsFromVertexFromEdgesFromAdjFaces.Add(vertexFromEdgesFromAdjFaces[v].GetPosition())   ;
                //         }

                //         //Ci = Somme de tous les points de la face courante/nb points
                //         float sumXx = 0,sumYy = 0,sumZz = 0,nbPts = 0;
                //         for(int p = 0; p<positionsFromVertexFromEdgesFromAdjFaces.Count(); ++p){
                //             sumXx += positionsFromVertexFromEdgesFromAdjFaces[p].x;
                //             sumYy += positionsFromVertexFromEdgesFromAdjFaces[p].y;
                //             sumZz += positionsFromVertexFromEdgesFromAdjFaces[p].z;
                //             nbPts += 1;
                //         }
                //         Vector3 Ci = new Vector3(sumXx/nbPts,sumYy/nbPts,sumZz/nbPts);

                //         //Ajout des points Ci (Face point) de la face courante dans la liste  
                //         vertexPoints.Add(Ci);
                //     }
                // }
                                ///FIN STEP 3: La liste Vertex point pour la face courante est remplie ////
        }   


/*
        public void SplitEdge(WingedEdge edge, Vector3 splittingPoint)
        {
        }
*/



/*
        public void SplitFace(Face face, Vector3 splittingPoint)
        {
        }
        */
        public bool isValid = false;
        public WingedEdgeMesh(Mesh mesh)
        {// constructeur prenant un mesh Vertex-Face en param�tre
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
            edges = new List<WingedEdge>();
            faces = new List<Face>();
            Vector3[] tmpVertices = mesh.vertices;
            for (int i = 0; i < tmpVertices.Length; i++)
            {
                vertices.Add(new Vertex(i, tmpVertices[i]));
            }

            //On r�cup�re les quads et on les parcourt pour trouver les meshs
            int[] indexes = mesh.GetIndices(0);
            Dictionary<ulong, WingedEdge> dicoWingedEdges = new Dictionary<ulong, WingedEdge>();
            List<WingedEdge> faceEdges = new List<WingedEdge>();

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

                //On complete startVertex, endVertex, leftFace et rightFace
                for (int j = 0; j < faceIndexes.Length; j++)
                {
                    int startIndex = faceIndexes[j];
                    int endIndex = faceIndexes[(j + 1) % faceIndexes.Length];
                    Vertex startVertex = vertices[startIndex];
                    Vertex endVertex = vertices[endIndex];

                    // creer un dictionnaire cle a laquell est associ� une seule edge utiliser la m�thode try get Value
                    ulong key = ((ulong)Mathf.Min(startIndex, endIndex)) + (((ulong)Mathf.Max(startIndex, endIndex)) << 32);

                    WingedEdge edge = null;

                    if (!dicoWingedEdges.TryGetValue(key, out edge))
                    {
                        edge = new WingedEdge(edges.Count, startVertex, endVertex, newFace, null);
                        edges.Add(edge);
                        dicoWingedEdges.Add(key, edge);
                        faceEdges.Add(edge);

                        //On ajoute les edges associ�es � la face et aux vertices.
                        if (newFace.edge == null) newFace.edge = edge;
                        if (startVertex.edge == null) startVertex.edge = edge;
                    }
                    else
                    {
                        //Compl�te les info manquante la leftFace 
                        edge.leftFace = newFace;
                        if (newFace.edge == null) newFace.edge = edge;
                        faceEdges.Add(edge);
                    }

                    //Il manque plus qu'� compl�ter les CCW et CW
                }
                for (int w = 0; w < faceEdges.Count; w++)
                {
                    if (newFace == faceEdges[w].rightFace)
                    {
                        faceEdges[w].startCWEdge = faceEdges[(w - 1 + faceEdges.Count) % faceEdges.Count];
                        faceEdges[w].endCCWEdge = faceEdges[(w + 1) % faceEdges.Count];
                    }
                    else if (newFace == faceEdges[w].leftFace)
                    {
                        faceEdges[w].endCWEdge = faceEdges[(w - 1 + faceEdges.Count) % faceEdges.Count];
                        faceEdges[w].startCCWEdge = faceEdges[(w + 1) % faceEdges.Count];
                    }
                }
                faceEdges.Clear();

                //MAJ CCW and CW Edge pour les borderEdges
                for (int w = 0; w < edges.Count; w++)
                {
                    if (edges[w].leftFace == null)
                    {
                        edges[w].startCCWEdge = edges[w].FindStartCCWBorder();
                        edges[w].endCWEdge = edges[w].FindEndCWBorder();
                    }
                }
                for(int w=0; w<faceEdges.Count; w++)
                {
                    if (newFace == faceEdges[w].rightFace)
                    {
                        
                        faceEdges[w].startCWEdge = edges.Find(e => (e.endVertex == faceEdges[w].startVertex && e.rightFace == newFace) || (e.startVertex == faceEdges[w].startVertex && e.leftFace == newFace));
                        faceEdges[w].endCCWEdge = edges.Find(e => (e.startVertex == faceEdges[w].endVertex && e.rightFace == newFace) || (e.endVertex == faceEdges[w].endVertex && e.leftFace == newFace));
                    }
                    else if(newFace == faceEdges[w].leftFace)
                    {
                        faceEdges[w].endCWEdge = edges.Find(e => (e.startVertex == faceEdges[w].startVertex && e.rightFace == newFace) || (e.endVertex == faceEdges[w].startVertex && e.leftFace == newFace));
                        faceEdges[w].startCCWEdge = edges.Find(e => (e.endVertex == faceEdges[w].endVertex && e.rightFace == newFace) || (e.startVertex == faceEdges[w].endVertex && e.leftFace == newFace));
                    }
                }
                faceEdges.Clear();
            }
            ////vertices
            string str = "Vertex - edges : \n";
            foreach (Vertex v in vertices)
            {
                str += "V" + v.index.ToString() + ": " + v.position.ToString() + " | e" + v.edge.index + " \n";
            }
            Debug.Log(str);
            ////faces
            str = "Faces - edges : \n";
            foreach (Face f in faces)
            {
                str += f.index.ToString() + ": F" + f.index.ToString() + " - e "+ f.edge.index + " \n";
            }
            Debug.Log(str);

            //////wingedEdge

            //str = "WingedEdges : \n";
            foreach (WingedEdge e in edges)
            {
                str += $"e{e.index} : V{e.startVertex.index} - V{e.endVertex.index}| {(e.leftFace == null ? "NoLeftFace" : $"F{e.leftFace.index}")} | {(e.rightFace == null ? "NoRightFace" : $"F{e.rightFace.index}")}\n"; //| SCCW : e{e.startCCWEdge.index} - SCW : e{e.startCWEdge.index} - ECW : e{e.endCWEdge.index} - ECCW : e{e.endCCWEdge.index}\n";
            }
            Debug.Log(str);

            GUIUtility.systemCopyBuffer = ConvertToCSV("\t");

        }

        public Mesh ConvertToFaceVertexMesh()
        {
            Mesh faceVertexMesh = new Mesh();
            // magic happens
            Vector3[] m_vertices = new Vector3[vertices.Count];
            int[] m_quads = new int[faces.Count * 4];


            //On doit r�cup�rer les edges d'une face et r�cup�rer les vertices dans le m�me sens.
            //On cree donc deux m�thodes distinctes dans la classe Face GetEdgesFace et GetVertexFace
            //FaceEdges

            //Vertices
            for (int i = 0; i < vertices.Count; i++)
            {
                m_vertices[i] = vertices[i].position;
            }

            int index = 0;

            //Quads
            for (int j = 0; j < faces.Count; j++)
            {
                List<Vertex> maListeVertexFace = faces[j].GetVertex();
                for (int w = 0; w < maListeVertexFace.Count; w++)
                {
                    m_quads[index++] = maListeVertexFace[w].index;
                }
                    
            }

            faceVertexMesh.vertices = m_vertices;
            faceVertexMesh.SetIndices(m_quads, MeshTopology.Quads, 0);
            return faceVertexMesh;
        }
        public string ConvertToCSV(string separator = "\t")
        {

            List<string> strings = new List<string>();

            //Vertices
            foreach (var vertice in vertices)
            {
                List<WingedEdge> edges_vertice = edges.FindAll(edge => edge.startVertex == vertice || edge.endVertex == vertice);
                int[] edges_adj = new int[edges_vertice.Count];
                for (int i = 0; i < edges_vertice.Count; i++)
                {
                    edges_adj[i] = edges_vertice[i].index;
                }
                Vector3 pos = vertice.position;
                strings.Add(vertice.index + separator
                    + pos.x.ToString("N03") + " "
                    + pos.y.ToString("N03") + " "
                    + pos.z.ToString("N03") + separator
                    + string.Join(" ", edges_adj)
                    + separator + separator);
            }
            for (int i = vertices.Count; i < edges.Count; i++)
            {
                strings.Add(separator + separator + separator + separator);
            }
            //Edges
            for (int i = 0; i < edges.Count; i++)
            {
                strings[i] += edges[i].index + separator
                    + edges[i].startVertex.index + separator
                    + edges[i].endVertex.index + separator
                    + $"{(edges[i].leftFace != null ? $"{edges[i].leftFace.index}" : "NULL")}" + separator
                    + $"{(edges[i].rightFace != null ? $"{edges[i].rightFace.index}" : "NULL")}" + separator
                    + $"{(edges[i].startCCWEdge != null ? $"{edges[i].startCCWEdge.index}" : "NULL")}" + separator
                    + $"{(edges[i].startCWEdge != null ? $"{edges[i].startCWEdge.index}" : "NULL")}" + separator
                    + $"{(edges[i].endCWEdge != null ? $"{edges[i].endCWEdge.index}" : "NULL")}" + separator
                    + $"{(edges[i].endCCWEdge != null ? $"{edges[i].endCCWEdge.index}" : "NULL")}" + separator
                    + separator;
            }
            //Faces
            for (int i = 0; i < faces.Count; i++)
            {
                List<WingedEdge> edges_face = faces[i].GetEdges();
                int[] face_edges = new int[edges_face.Count];
                for (int j = 0; j < edges_face.Count; j++)
                {
                    face_edges[j] = edges_face[j].index;
                }
                strings[i] += faces[i].index + separator + string.Join(" ", face_edges) + separator + separator;
            }
            string str = "Vertex" + separator + separator + separator + separator + "WingedEdges" + separator + separator + separator + separator + separator + separator + "Faces\n"
                + "Index" + separator + "Position" + separator + "Edge" + separator + separator +
                "Index" + separator + "Start Vertex" + separator + "End Vertex" + separator + "Left Face" + separator + "Right Face" + separator + "Start CCW Edge" + separator + "Start CW Edge" + separator + "End CW Edge" + separator + "End CCW Edge" + separator + separator +
                "Index" + separator + "Edge\n"
                + string.Join("\n", strings);
            Debug.Log(str);
            return str;
        }
        public void DrawGizmos(bool drawVertices, bool drawEdges, bool drawFaces, Transform transform)
        {

            Gizmos.color = Color.black;
            GUIStyle style = new GUIStyle();
            GUIStyle style2 = new GUIStyle();
            style.fontSize = 12;
            style2.fontSize = 12;

            //vertices
            if (drawVertices)
            {
                style.normal.textColor = Color.red;
                style2.normal.textColor = Color.black;
                for (int i = 0; i < vertices.Count; i++)
                {
                    Vector3 worldPos = transform.TransformPoint(vertices[i].position);
                    Handles.Label(worldPos, "V" + vertices[i].index, style);
                }
            }
            //faces
            if (drawFaces)
            {
                style.normal.textColor = Color.green;
                for (int i = 0; i < faces.Count; i++)
                {
                    List<Vertex> vertices_une_face = faces[i].GetVertex();
                    //Debug.Log(vertices_une_face.Count);
                    Vector3 total = new Vector3(0,0,0);
                    for (int w = 0; w < vertices_une_face.Count; w++)
                    {
                        Vector3 pt = transform.TransformPoint(vertices_une_face[w].position);
                        total += pt;
                    }
   
                    Handles.Label((total) / 4.0f, "F" + faces[i].index, style);
                }
            }
            //edges
            if (drawEdges)
            {
                style.normal.textColor = Color.blue;
                style2.normal.textColor = Color.cyan;
                for (int i = 0; i < edges.Count; i++)
                {
                    Vector3 start = transform.TransformPoint(edges[i].startVertex.position);
                    Vector3 end = transform.TransformPoint(edges[i].endVertex.position);
                    Vector3 pos = Vector3.Lerp(start, end, 0.5f);

                    Handles.Label(pos, "e" + edges[i].index, style);
                }
            }
        }
    }
}