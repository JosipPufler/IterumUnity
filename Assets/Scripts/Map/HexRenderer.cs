using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class HexRenderer : MonoBehaviour
{
    private const float numberVerticalOffset = 10f;
    public float innerSize;
    public float outerSize;
    public float height;
    public bool isFlatTopped;
    public float startingY;

    private GameObject ringOverlay;
    public Material borderMaterial;
    private GameObject numberCanvasGO;
    private TextMeshProUGUI numberText;

    private Mesh m_mesh;
    private MeshFilter m_meshFilter;
    private MeshRenderer m_meshRenderer;
    private MeshCollider m_meshCollider;

    public Material material;
    private Camera mainCam;
    private List<Face> faces = new();

    public void Initialize(Material mat, Material borderMat, float inner, float outer, float height, bool isFlat, float startingY = 0)
    {
        material = mat;
        borderMaterial = borderMat;
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
        mainCam = Camera.main;
        m_meshFilter = GetComponent<MeshFilter>();
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_meshCollider = GetComponent<MeshCollider>();

        m_mesh = new Mesh
        {
            name = "Hex"
        };

        m_meshFilter.mesh = m_mesh;
        m_meshRenderer.material = material;
        m_meshCollider.sharedMesh = m_mesh;
    }

    private void LateUpdate()
    {
        if (numberCanvasGO != null && numberCanvasGO.activeSelf && mainCam != null)
        {
            Vector3 camForward = mainCam.transform.forward;
            Vector3 camUp = mainCam.transform.up;
            numberCanvasGO.transform.rotation = Quaternion.LookRotation(camForward, camUp);
        }
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

        List<Vector3> vertices = new() { pointA, pointB, pointC, pointD };
        List<int> triangles = new() { 0, 1, 2, 2, 3, 0 };
        List<Vector2> uvs = new() { new(0, 0), new(1, 0), new(1, 1), new(0, 1) };
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
            Vector3 localOffset = new(Mathf.Cos(angleRad) * radius, y, Mathf.Sin(angleRad) * radius);
            centers[i] = transform.TransformPoint(localOffset);
        }

        return centers;
    }

    private void CreateBorderRing(float thickness = 20f)
    {
        if (ringOverlay != null)
            Destroy(ringOverlay);

        ringOverlay = new GameObject("HexBorderOverlay");
        ringOverlay.transform.SetParent(transform, false);
        ringOverlay.transform.localPosition = Vector3.up * 0.01f;

        MeshFilter mf = ringOverlay.AddComponent<MeshFilter>();
        MeshRenderer mr = ringOverlay.AddComponent<MeshRenderer>();
        mr.material = borderMaterial;

        Mesh mesh = new()
        {
            name = "HexBorderRing"
        };

        List<Vector3> vertices = new();
        List<int> triangles = new();

        float outer = outerSize;
        float inner = outerSize - thickness;
        float y = startingY + height + 0.01f;

        for (int i = 0; i < 6; i++)
        {
            Vector3 outerA = GetPoint(outer, y, i);
            Vector3 outerB = GetPoint(outer, y, (i + 1) % 6);
            Vector3 innerA = GetPoint(inner, y, i);
            Vector3 innerB = GetPoint(inner, y, (i + 1) % 6);

            int idx = vertices.Count;
            vertices.Add(innerA);
            vertices.Add(innerB);
            vertices.Add(outerB);
            vertices.Add(outerA);

            triangles.Add(idx);
            triangles.Add(idx + 1);
            triangles.Add(idx + 2);

            triangles.Add(idx + 2);
            triangles.Add(idx + 3);
            triangles.Add(idx);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        mf.mesh = mesh;
        ringOverlay.SetActive(false);
    }
    private void CreateNumberLabel()
    {
        if (numberCanvasGO != null)
            Destroy(numberCanvasGO);

        numberCanvasGO = new GameObject("HexNumberCanvas");
        numberCanvasGO.transform.SetParent(transform, false);
        float verticalOffset = numberVerticalOffset;
        numberCanvasGO.transform.SetLocalPositionAndRotation(new Vector3(0, startingY + height + verticalOffset, 0), Quaternion.Euler(90, 0, 0));
        Canvas canvas = numberCanvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 10;

        CanvasScaler scaler = numberCanvasGO.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;

        RectTransform rect = numberCanvasGO.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(1, 1);

        GameObject textGO = new("Number");
        textGO.transform.SetParent(numberCanvasGO.transform, false);

        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.localPosition = Vector3.zero;
        textRect.sizeDelta = new Vector2(80f, 40f);
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);

        numberText = textGO.AddComponent<TextMeshProUGUI>();
        numberText.alignment = TextAlignmentOptions.Center;
        numberText.fontSize = 40;
        numberText.text = "";
    }

    public void ShowHighlight(int number)
    {
        if (ringOverlay == null) CreateBorderRing();
        if (numberCanvasGO == null) CreateNumberLabel();

        ringOverlay.SetActive(true);
        numberCanvasGO.SetActive(true);
        numberText.text = number.ToString();
    }

    public void HideHighlight()
    {
        if (ringOverlay != null) ringOverlay.SetActive(false);
        if (numberCanvasGO != null) numberCanvasGO.SetActive(false);
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