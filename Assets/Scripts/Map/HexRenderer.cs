using Assets.Scripts;
using Assets.Scripts.GameLogic.models;
using Mirror;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class HexRenderer : NetworkBehaviour
{
    public bool isHighlighted;
    public bool isTargetHighlighted;

    private const float numberVerticalOffset = 10f;
    [SyncVar] public float innerSize;
    [SyncVar] public float outerSize;
    [SyncVar] public float height;
    [SyncVar] public bool isFlatTopped;
    [SyncVar] public float startingY;
    [SyncVar] public GridCoordinate positon;
    
    private GameObject ringOverlay;
    public Material borderMaterial;
    public Material targetMaterial;
    private GameObject numberCanvasGO;
    private TextMeshProUGUI numberText;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    public Material material;
    private Camera mainCam;
    private List<Face> faces = new();
    private MeshRenderer borderMeshRenderer;

    public override void OnStartClient()
    {
        base.OnStartClient();

        CampaignGridLayout.Instance.RegisterHex(positon, gameObject);
        Initialize(material, borderMaterial, targetMaterial, innerSize, outerSize, height, isFlatTopped, startingY);
    }

    public override void OnStopClient()
    {
        CampaignGridLayout.Instance.UnregisterHex(positon);
        base.OnStopClient();
    }

    public void Initialize(Material mat, Material borderMat, Material targetMat, float inner, float outer, float height, bool isFlat, float startingY = 0)
    {
        material = mat;
        borderMaterial = borderMat;
        targetMaterial = targetMat;
        innerSize = inner;
        outerSize = outer;
        this.height = height;
        isFlatTopped = isFlat;
        this.startingY = startingY;

        meshRenderer.material = material;
        DrawMesh();
    }

    private void Awake()
    {
        mainCam = Camera.main;
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        mesh = new Mesh
        {
            name = "Hex"
        };

        meshFilter.mesh = mesh;
        meshRenderer.material = material;
        meshCollider.sharedMesh = mesh;
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
        faces.Clear();
        DrawFaces();
        CombineFaces();

        if (meshCollider == null)
            meshCollider = GetComponent<MeshCollider>();

        meshCollider.sharedMesh = mesh;
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

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000f);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
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
        borderMeshRenderer = ringOverlay.AddComponent<MeshRenderer>();
        borderMeshRenderer.material = borderMaterial;

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

    [Command(requiresAuthority = false)]
    private void CmdShowHighlight(int number)
    {
        ShowHighlight(number);
        RpcShowHighlight(number);
    }

    [Command(requiresAuthority = false)]
    private void CmdShowTargetHighlight()
    {
        ShowTargetHighlight();
        RpcShowTargetHighlight();
    }

    [Command(requiresAuthority = false)]
    private void CmdHideHighlight()
    {
        HideHighlight();
        RpcHideHighlight();
    }

    public void ShowHighlight(int number)
    {
        if (isServer)
        {
            isTargetHighlighted = false;
            isHighlighted = true;
            if (ringOverlay == null) CreateBorderRing();
            if (numberCanvasGO == null) CreateNumberLabel();

            borderMeshRenderer.material = borderMaterial;
            ringOverlay.SetActive(true);
            numberCanvasGO.SetActive(true);
            numberText.text = number.ToString();
        }
        else
        {
            CmdShowHighlight(number);
        }
    }

    public void ShowTargetHighlight() {
        if (isServer)
        {
            isTargetHighlighted = true;
            isHighlighted = false;
            if (ringOverlay == null) CreateBorderRing();
            if (numberCanvasGO == null) CreateNumberLabel();

            numberCanvasGO.SetActive(false);
            borderMeshRenderer.material = targetMaterial;
            ringOverlay.SetActive(true);
        }
        else 
        {
            CmdShowTargetHighlight();
        }
    }

    public void HideHighlight()
    {
        if (isServer)
        {
            isTargetHighlighted = false;
            isHighlighted = false;
            if (ringOverlay != null) ringOverlay.SetActive(false);
            if (numberCanvasGO != null) numberCanvasGO.SetActive(false);
        }
        else 
        {
            CmdHideHighlight();
        }
    }

    [ClientRpc]
    private void RpcShowHighlight(int number)
    {
        isTargetHighlighted = false;
        isHighlighted = true;

        if (ringOverlay == null) CreateBorderRing();
        if (numberCanvasGO == null) CreateNumberLabel();

        borderMeshRenderer.material = borderMaterial;
        ringOverlay.SetActive(true);
        numberCanvasGO.SetActive(true);
        numberText.text = number.ToString();
    }

    [ClientRpc]
    private void RpcShowTargetHighlight()
    {
        isTargetHighlighted = true;
        isHighlighted = false;

        if (ringOverlay == null) CreateBorderRing();
        if (numberCanvasGO == null) CreateNumberLabel();

        numberCanvasGO.SetActive(false);
        borderMeshRenderer.material = targetMaterial;
        ringOverlay.SetActive(true);
    }

    [ClientRpc]
    private void RpcHideHighlight()
    {
        isTargetHighlighted = false;
        isHighlighted = false;

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