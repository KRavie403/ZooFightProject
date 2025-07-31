using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StyledText : MonoBehaviour
{
    public FontStyleData style;

    private void Awake()
    {
        ApplyStyle();
    }

    public void ApplyStyle()
    {
        var tmp = GetComponent<TextMeshProUGUI>();
        if (tmp == null || style == null) return;

        tmp.font = style.fontAsset;
        tmp.fontSize = style.fontSize;
        tmp.color = style.color;
        tmp.fontStyle = style.fontStyle;

        tmp.enableVertexGradient = false;
        tmp.outlineColor = style.outlineColor;
        tmp.outlineWidth = style.useOutline ? style.outlineWidth : 0;

        tmp.fontSharedMaterial.SetFloat(ShaderUtilities.ID_OutlineSoftness, style.softness);
        tmp.fontSharedMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, style.dilate);
    }
}
