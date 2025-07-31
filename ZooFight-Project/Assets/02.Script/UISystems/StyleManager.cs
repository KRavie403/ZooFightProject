using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StyleManager : Singleton<StyleManager>
{
    public List<FontStyleData> styles;

    private Dictionary<string, FontStyleData> _styleMap;

    protected new void Awake()
    {
        base.Awake();
        _styleMap = new Dictionary<string, FontStyleData>();
        foreach (var style in styles)
        {
            _styleMap[style.name] = style;
        }
    }

    public FontStyleData GetStyle(string styleName)
    {
        return _styleMap.TryGetValue(styleName, out var style) ? style : null;
    }
}
