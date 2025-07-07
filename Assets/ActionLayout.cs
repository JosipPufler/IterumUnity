using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ActionLayout : MonoBehaviour
{
    [Header("References (assign in Inspector)")]
    public TMP_Text leftText;
    public TMP_Text rightText;
    public RectTransform spacer;

    [Header("Settings")]
    public float minSpacer = 15f;

    RectTransform _rt;

    void Awake()
    {
        _rt = (RectTransform)transform;
    }

    void LateUpdate()
    {
        float totalW = ((RectTransform)_rt.parent).rect.width;

        float leftW = leftText.preferredWidth;
        float rightW = rightText.preferredWidth;

        float target = totalW - leftW - rightW - minSpacer;
        if (target < minSpacer) target = minSpacer;

        spacer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, target);
    }
}