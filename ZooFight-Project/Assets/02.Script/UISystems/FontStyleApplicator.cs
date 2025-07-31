using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FontStyleApplicator : MonoBehaviour
{
    public string styleName;

    void Awake()
    {
        var style = StyleManager.Inst.GetStyle(styleName);
        if (style != null)
        {
            var styledText = GetComponent<StyledText>();
            styledText.style = style;
            styledText.ApplyStyle();
        }
    }
}

