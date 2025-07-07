using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class HexRenderer : MonoBehaviour
{
    public float innerSize;
    public float outerSize;
    public float height;
    public bool isFlatTopped;
    public float startingY;

    private Mesh m_mesh;
    private MeshFilter m_meshFilter;
    private MeshRenderer m_meshRenderer;
    private MeshCollider m_meshCollider;

    public Material material;

    private List<Face> faces = new();

    public void Initialize(Material mat, float inner, float outer, float height, bool isFlat, float startingY = 0)
    {
        material = mat;
        innerSize = inner;
        outerSize = outer;
        this.height = height;
        isFlatTopped = isFlat;
        this.startingY = startingY;

        m_meshRenderer.material = material;
        DrawMesh();
    }

    private void Awake()
    {
        m_meshFilter = GetComponent<MeshFilter>();
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_meshCollider = GetComponent<MeshCollider>();

        m_mesh = new Mesh();
        m_mesh.name = "Hex";

        m_meshFilter.mesh = m_mesh;
        m_meshRenderer.material = material;
        m_meshCollider.sharedMesh = m_mesh;
    }

    public void DrawMesh() 
    {
        DrawFaces();
        CombineFaces();

        if (m_meshCollider == null)
            m_meshCollider = GetComponent<MeshCollider>();

        m_meshCollider.sharedMesh = m_mesh;
    }

    private void CombineFaces()
    {
        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvs = new();

        for (int i = 0; i < faces.Count; i++) { 
            vertices.AddRange(faces[i].Vertices);
            uvs.AddRange(faces[i].Uvs);

            int offset = (4 * i);
            foreach (int triangle in faces[i].Triangles) { 
                triangles.Add(triangle+offset);
            }
        }

        m_mesh.vertices = vertices.ToArray();
        m_mesh.triangles = triangles.ToArray();
        m_mesh.uv = uvs.ToArray();
        m_mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000f);
        m_mesh.RecalculateNormals();
        m_mesh.RecalculateBounds();
    }

    private void DrawFaces()
    {
        faces = new List<Face>();

        float top = startingY + height;
        float bottom = startingY;

        for (int i = 0; i < 6; i++)
        {
            //Top
            faces.Add(CreateFace(innerSize, outerSize, top, top, i));
            //Bottom
            faces.Add(CreateFace(innerSize, outerSize, bottom, bottom, i, true));
            //Outer wall
            faces.Add(CreateFace(outerSize, outerSize, top, bottom, i, true));
            //Inner side
            faces.Add(CreateFace(innerSize, innerSize, top, bottom, i));
        }
    }

    private Face CreateFace(float innerRad, float outerRad, float heightA, float heightB, int point, bool reverse = false) {
        Vector3 pointA = GetPoint(innerRad, heightB, point);
        Vector3 pointB = GetPoint(innerRad, heightB, point < 5 ? point + 1 : 0);
        Vector3 pointC = GetPoint(outerRad, heightA, point < 5 ? point + 1 : 0);
        Vector3 pointD = GetPoint(outerRad, heightA, point);

        List<Vector3> vertices = new List<Vector3>() {  pointA, pointB, pointC, pointD };
        List<int> triangles = new List<int>() { 0, 1, 2, 2, 3, 0 };
        List<Vector2> uvs = new List<Vector2>() { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
        if (reverse) { 
            vertices.Reverse();
        }

        return new Face(vertices, triangles, uvs);
    }

    protected Vector3 GetPoint(float size, float height, int index) {
        float angleDeg = isFlatTopped ?  60 * index : 60 * index - 30;
        float angleRad = Mathf.PI / 180f * angleDeg;
        return new Vector3(size * Mathf.Cos(angleRad), height, size * Mathf.Sin(angleRad));
    }

    public Vector3[] GetSideCenters()
    {
        Vector3[] centers = new Vector3[6];
        float radius = outerSize;
        float y = startingY + height / 2f;

        for (int i = 0; i < 6; i++)
        {
            float angleDeg = isFlatTopped ? 60 * i : 60 * i - 30;
            float angleRad = Mathf.Deg2Rad * angleDeg;
            Vector3 localOffset = new Vector3(Mathf.Cos(angleRad) * radius, y, Mathf.Sin(angleRad) * radius);
            centers[i] = transform.TransformPoint(localOffset);
        }

        return centers;
    }
}

public struct Face 
{
    public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        Vertices = vertices;
        Triangles = triangles;
        Uvs = uvs;
    }

    public List<Vector3> Vertices { get; private set; }
    public List<int> Triangles { get; private set; }
    public List<Vector2> Uvs { get; private set; }
}