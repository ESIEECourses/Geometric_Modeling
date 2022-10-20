using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshGeneratorQuads : MonoBehaviour
{
    delegate Vector3 ComputePosDelegate(float kx, float kz);
    MeshFilter m_Mf;

    [SerializeField] bool m_DisplayMeshInfo = true;
    [SerializeField] bool m_DisplayMeshEdges = true;
    [SerializeField] bool m_DisplayMeshVertices = true;
    [SerializeField] bool m_DisplayMeshFaces = true;

    [SerializeField] AnimationCurve m_Profil;

    private void Start()
    {
        m_Mf = GetComponent<MeshFilter>();
        //m_Mf.mesh = CreateGridXZ( 7, 8, new Vector3(4, 1, 3));
        //Cylindre
        /*m_Mf.mesh = CreateNormalizedGridXZ( 20, 10, 
            (kx, kz) =>
            {
                float rho, theta, y;
                //coordinates mapping de (kx,kz) -> (rhi,theta,y)
                theta = kx * 2 * Mathf.PI;
                y = kz * 6;  //h=6
                rho = m_Profil.Evaluate(kz) * 2;
                return new Vector3(rho * Mathf.Cos(theta), y, rho * Mathf.Sin(theta));
                //return new Vector3(Mathf.Lerp(-1.5f, 5f, kx), 5, Mathf.Lerp(3, -3, kz));
            }
        );*/
        //Sphère
        /*m_Mf.mesh = CreateNormalizedGridXZ(20, 10,
            (kx, kz) =>
            {
                float rho, theta, phi;
                //coordinates mapping de (kx,kz) -> (rhi,theta,phi)
                theta = (1 - kx) * 2 * Mathf.PI;
                phi = kz * Mathf.PI;  //h=6
                //rho = m_Profil.Evaluate(kz) * 2;
                rho = 2 + .55f * Mathf.Cos(kx * 2 * Mathf.PI * 8) * Mathf.Sin(kz * 2 * Mathf.PI * 6);
                return new Vector3(rho * Mathf.Cos(theta) * Mathf.Sin(phi), rho * Mathf.Cos(phi), rho * Mathf.Sin(theta) * Mathf.Sin(phi));
                //return new Vector3(Mathf.Lerp(-1.5f, 5f, kx), 5, Mathf.Lerp(3, -3, kz));
            }
        );
        GUIUtility.systemCopyBuffer = ConvertToCSV("\t");
        Debug.Log(ConvertToCSV("\t"));*/

        //Torus (donut)
        m_Mf.mesh = CreateNormalizedGridXZ(40*6, 20,
            (kx, kz) =>
            {
                float R = 4;
                float r = 1;
                float theta = 2 * Mathf.PI * kx;
                Vector3 OOmega = new Vector3(R * Mathf.Cos(theta), 0, R * Mathf.Sin(theta));
                float alpha = Mathf.PI * 2 * kz;
                Vector3 OmegaP = r * Mathf.Cos(alpha) * OOmega.normalized + r * Mathf.Sin(alpha) * Vector3.up;
                return OOmega + OmegaP;
            }
        );






        //m_Mf.mesh = CreateRegularPolygon(new Vector3(8, 0, 8),20);
        //m_Mf.mesh = CreatePacman(new Vector3(8, 0, 8),20);
    }


    string ConvertToCSV(string separator)
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

    Mesh CreateStrip(int nSegments, Vector3 halfSize)
    {
        Mesh mesh = new Mesh();
        mesh.name = "strip";

        Vector3[] vertices = new Vector3[(nSegments + 1) * 2];
        int[] quads = new int[nSegments * 4];

        int index = 0;
        Vector3 leftTopPos = new Vector3(-halfSize.x, 0, halfSize.z);
        Vector3 rightTopPos = new Vector3(halfSize.x, 0, halfSize.z);
        //bouble for pour remplir vertices
        for (int i = 0; i < nSegments + 1; i++)
        {
            float k = (float)i / nSegments;
            Vector3 tmpPos = Vector3.Lerp(leftTopPos, rightTopPos, k);
            vertices[index++] = tmpPos;
            vertices[index++] = tmpPos - 2 * halfSize.z * Vector3.forward;
        }
        index = 0;
        //boucle for pour remplir triangles
        for (int i = 0; i < nSegments; i++)
        {
            quads[index++] = 2 * i;
            quads[index++] = 2 * i + 2;
            quads[index++] = 2 * i + 3;
            quads[index++] = 2 * i + 1;
        }


        mesh.vertices = vertices;
        mesh.SetIndices(quads, MeshTopology.Quads, 0);

        return mesh;
    }

    Mesh CreateGridXZ(int nSegmentsX, int nSegmentsZ, Vector3 halfSize)
    {
        Mesh mesh = new Mesh();
        mesh.name = "grid";

        Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
        int[] quads = new int[nSegmentsX * nSegmentsZ * 4];

        int index = 0;

        for (int i = 0; i < nSegmentsZ + 1; i++)
        {
            float kz = (float)i / nSegmentsZ;
            for (int j = 0; j < nSegmentsX + 1; j++)
            {
                float kx = (float)j / nSegmentsX;
                vertices[index++] = new Vector3(Mathf.Lerp(-halfSize.x, halfSize.x, kx), 0, Mathf.Lerp(halfSize.z, -halfSize.z, kz));
            }
        }

        /*
        Vector3 leftTopPos = new Vector3(-halfSize.x, 0, halfSize.z);
        Vector3 leftBottomPos = new Vector3(-halfSize.x, 0, -halfSize.z);
        for (int i = 0; i < nSegmentsZ + 1; i++)
        {
            float kz = (float)i / nSegmentsZ;
            Vector3 tmpLeftPos = Vector3.Lerp(leftTopPos, leftBottomPos, kz);
            Vector3 tmpRightPos = tmpLeftPos + 2 * halfSize.x * Vector3.right;
            for (int j = 0; j < nSegmentsX + 1; j++)
            {
                float kx = (float)j / nSegmentsX;
                Vector3 tmpPos = Vector3.Lerp(tmpLeftPos, tmpRightPos, kx);
                vertices[index++] = tmpPos;
            }
        }
        */

        index = 0;
        //boucle for pour remplir triangles
        for (int j = 0; j < nSegmentsZ; j++)
        {
            for (int i = 0; i < nSegmentsX; i++)
            {
                quads[index++] = i + j * (nSegmentsX + 1);
                quads[index++] = i + j * (nSegmentsX + 1) + 1;
                quads[index++] = i + (j + 1) * (nSegmentsX + 1) + 1;
                quads[index++] = i + (j + 1) * (nSegmentsX + 1);
            }
        }
        mesh.vertices = vertices;
        mesh.SetIndices(quads, MeshTopology.Quads, 0);

        return mesh;
    }

    Mesh CreateNormalizedGridXZ(int nSegmentsX, int nSegmentsZ, ComputePosDelegate computePos = null)
    {
        Mesh mesh = new Mesh();
        mesh.name = "normalizedGrid";

        Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
        int[] quads = new int[nSegmentsX * nSegmentsZ * 4];

        int index = 0;

        for (int i = 0; i < nSegmentsZ + 1; i++)
        {
            float kz = (float)i / nSegmentsZ;
            for (int j = 0; j < nSegmentsX + 1; j++)
            {
                float kx = (float)j / nSegmentsX;
                vertices[index++] = computePos != null ? computePos(kx, kz) : new Vector3(kx, 0, kz);
            }
        }


        index = 0;
        //boucle for pour remplir triangles
        for (int j = 0; j < nSegmentsZ; j++)
        {
            for (int i = 0; i < nSegmentsX; i++)
            {
                quads[index++] = i + j * (nSegmentsX + 1);
                quads[index++] = i + j * (nSegmentsX + 1) + 1;
                quads[index++] = i + (j + 1) * (nSegmentsX + 1) + 1;
                quads[index++] = i + (j + 1) * (nSegmentsX + 1);
            }
        }
        mesh.vertices = vertices;
        mesh.SetIndices(quads, MeshTopology.Quads, 0);

        return mesh;
    }

    private void OnDrawGizmos()
    {
        if (!(m_Mf && m_Mf.mesh)) return;

        Mesh mesh = m_Mf.mesh;
        Vector3[] vertices = mesh.vertices;

        int[] quads = mesh.GetIndices(0);

        Gizmos.color = Color.black;
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.red;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(vertices[i]);
            Handles.Label(worldPos, i.ToString(), style);
        }


        style.normal.textColor = Color.magenta;
        for (int i = 0; i < quads.Length / 4; i++)
        {
            int index1 = quads[4 * i];
            int index2 = quads[4 * i + 1];
            int index3 = quads[4 * i + 2];
            int index4 = quads[4 * i + 3];

            Vector3 pt1 = transform.TransformPoint(vertices[index1]);
            Vector3 pt2 = transform.TransformPoint(vertices[index2]);
            Vector3 pt3 = transform.TransformPoint(vertices[index3]);
            Vector3 pt4 = transform.TransformPoint(vertices[index4]);

            Gizmos.DrawLine(pt1, pt2);
            Gizmos.DrawLine(pt2, pt3);
            Gizmos.DrawLine(pt3, pt4);
            Gizmos.DrawLine(pt4, pt1);

            string str = string.Format("{0}:{1},{2},{3},{4}", i, index1, index2, index3, index4);

            Handles.Label((pt1 + pt2 + pt3 + pt4) / 4.0f, str, style);

        }



    }
    Mesh CreateRegularPolygon(Vector3 halfSize, int nSectors)
    {
        Mesh mesh = new Mesh();
        mesh.name = "polygon";

        Vector3[] vertices = new Vector3[2*nSectors +1];
        int[] quads = new int[nSectors*4];
        float deltaAngle = (Mathf.PI * 2) / (nSectors *2);
        for (int i = 0; i < 2 * nSectors; i++)
        {
            vertices[i]= new Vector3(halfSize.x*Mathf.Cos(i*deltaAngle), 0, halfSize.z*Mathf.Sin(i * deltaAngle));
        }
        vertices[vertices.Length - 1] = Vector3.zero;


        int index = 0;

        //boucle for pour remplir quads
        for (int j = 1; j < nSectors+1; j++)
        {
            //12-11-0-1
            quads[index++] = (2 * j - 1) % (vertices.Length -1) ;
            quads[index++] = vertices.Length - 1;
            quads[index++] = ((2 * j - 1) + 2) % (vertices.Length - 1) ;
            quads[index++] = ((2 * j - 1) + 1) % (vertices.Length - 1);
        }

        mesh.vertices = vertices;
        mesh.SetIndices(quads, MeshTopology.Quads, 0);

        return mesh;
    }

    Mesh CreatePacman(Vector3 halfSize, int nSectors, float startAngle = Mathf.PI / 3, float endAngle = 5 * Mathf.PI / 3)
    {
        Mesh mesh = new Mesh();
        mesh.name = "pacman";

        Vector3[] vertices = new Vector3[2 * nSectors + 1];
        int[] quads = new int[nSectors * 4];
        float deltaAngle = (Mathf.PI * 2 -(endAngle - startAngle)) / (nSectors);

        for (int i = 0; i < 2 * nSectors; i++)
        {
            vertices[i] = new Vector3(halfSize.x * Mathf.Cos(i * deltaAngle + startAngle), 0, halfSize.z * Mathf.Sin((i+1) * deltaAngle + startAngle));
        }
        vertices[vertices.Length - 1] = Vector3.zero;


        int index = 0;

        //boucle for pour remplir quads
        print((2 * 0) % (vertices.Length - 1));
        print(vertices.Length - 1);
        print(((2 * 0) + 2) % (vertices.Length - 1));
        print(((2 * 0) + 1) % (vertices.Length - 1));
        for (int j = 0; j < nSectors; j++)
        {
            //12-11-0-1
           
            quads[index++] = (2 * j) ;
            quads[index++] = vertices.Length - 1 ;
            quads[index++] = ((2 * j) + 2);
            quads[index++] = ((2 * j) + 1);
        }

        mesh.vertices = vertices;
        mesh.SetIndices(quads, MeshTopology.Quads, 0);

        return mesh;
    }

}
