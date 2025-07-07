using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager Instance { get; private set; }

    [Header("UI References")]
    public Canvas uiCanvas;
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    public Vector2 offset;
    public float maxTooltipWidth = 300f;

    [Header("Raycast Settings")]
    public LayerMask tokenMask;
    public float hoverGrace = 0.1f;

    private ToolTipTrigger currentHover;
    private float lastHitTime;
    private RectTransform panelRect;
    private Camera cam;

    public CameraController cameraController;
    private bool isUIOverride = false;
    void Awake()
    {
        Instance = this;
        cam = uiCanvas.worldCamera;
        panelRect = tooltipPanel.GetComponent<RectTransform>();
        tooltipPanel.SetActive(false);
    }

    void LateUpdate()
    {
        PositionTooltip();

        if (cameraController.IsCameraMoving)
        {
            HideTooltip();
            currentHover = null;
            return;
        }

        if (isUIOverride)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, tokenMask, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (hit.collider.TryGetComponent<ToolTipTrigger>(out var trig))
            {
                if (trig != currentHover)
                {
                    currentHover = trig;
                    UpdateTooltipText(trig.tooltipText);
                }
                lastHitTime = Time.time;
                ShowTooltip();
                return;
            }
        }

        if (currentHover != null && Time.time - lastHitTime < hoverGrace)
        {
            PositionTooltip();
            return;
        }

        HideTooltip();
        currentHover = null;
    }

    public void ShowTooltip()
    {
        if (!tooltipPanel.activeSelf)
            tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        if (tooltipPanel.activeSelf)
            tooltipPanel.SetActive(false);
    }

    public void UpdateTooltipText(string text)
    {
        tooltipText.textWrappingMode = TextWrappingModes.Normal;
        tooltipText.overflowMode = TextOverflowModes.Overflow;
        tooltipText.text = text;

        if (!tooltipText.TryGetComponent<LayoutElement>(out var le))
            le = tooltipText.gameObject.AddComponent<LayoutElement>();

        float desired = tooltipText.preferredWidth;
        le.preferredWidth = Mathf.Min(desired, maxTooltipWidth);

        tooltipText.ForceMeshUpdate();

        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipText.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
    }

    private void PositionTooltip()
    {
        Vector2 mousePos = Input.mousePosition + (Vector3)offset;

        Vector2 panelSize = panelRect.sizeDelta;
        Vector2 clampedPos = mousePos;

        float canvasScale = uiCanvas.scaleFactor;
        Vector2 screenSize = new(Screen.width, Screen.height);

        clampedPos.x = Mathf.Clamp(clampedPos.x, 0, screenSize.x - panelSize.x * canvasScale);
        clampedPos.y = Mathf.Clamp(clampedPos.y, panelSize.y * canvasScale, screenSize.y);

        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(clampedPos.x, clampedPos.y, uiCanvas.planeDistance));
        panelRect.position = worldPos;
    }

    public void Hide()
    {
        isUIOverride = false;
        tooltipPanel.SetActive(false);
        currentHover = null;
    }

    public void Show(string text)
    {
        isUIOverride = true;
        UpdateTooltipText(text);
        tooltipPanel.SetActive(true);
        PositionTooltip();
    }
}