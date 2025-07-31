using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Font Style")]
public class FontStyleData : ScriptableObject
{
    [Header("Main Settings")]
    public TMP_FontAsset fontAsset;
    public int fontSize;
    public Color color;
    public FontStyles fontStyle;

    [Header("Outline Settings")]
    public bool useOutline;
    public Color outlineColor;
    public float outlineWidth;

    [Header("Face Settings")]
    public float softness;
    public float dilate;
}
