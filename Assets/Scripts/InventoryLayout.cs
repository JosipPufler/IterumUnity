using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class InventoryLayout : MonoBehaviour
{
    [Header("References (assign in Inspector)")]
    public TMP_Text leftText;
    public Button rightButton;
    public RectTransform spacer;

    [Header("Settings")]
    public float minSpacer = 15f;
    public float rightButtonPadding = 30f;

    private RectTransform _rt;
    private TMP_Text rightText;

    void Awake()
    {
        rightText = rightButton.GetComponentInChildren<TMP_Text>();
        _rt = (RectTransform)transform;
    }

    void LateUpdate()
    {
        float totalW = ((RectTransform)_rt.parent).rect.width;

        float leftW = leftText.preferredWidth;
        float rightW = rightText.preferredWidth + rightButtonPadding;

        float target = totalW - leftW - rightW - minSpacer;
        if (target < minSpacer) target = minSpacer;

        spacer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, target);
    }
}