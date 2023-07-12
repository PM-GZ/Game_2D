using UnityEngine;
using UnityEngine.UI;



[AddComponentMenu("UI/Vertex Image")]
public class VertexImage : Graphic
{
    [SerializeField] private Sprite _Sprite;
    [SerializeField] private float _LineWidth;
    [SerializeField] private float _LineSpace;
    [Header("«„–±")]
    [SerializeField] private float _Incline_Top;
    [SerializeField] private float _Incline_Right;
    private int _Count;


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Vector2 size = rectTransform.sizeDelta;
        _Count = Mathf.FloorToInt(size.x / (_LineWidth + _LineSpace));
        float mod = size.x % (_LineWidth + _LineSpace);

        float x = -size.x / 2;
        float halfY = size.y / 2;

        SetVerts(vh, x, halfY, mod);
        SetTriangle(vh);
    }

    private void SetVerts(VertexHelper vh, float x, float y, float mod)
    {
        for (int i = 0; i < _Count; i++)
        {
            vh.AddVert(new Vector3(x, -y, 0), color, Vector2.zero);
            vh.AddVert(new Vector3(x + _Incline_Top, y, 0), color, Vector2.zero);
            vh.AddVert(new Vector3(x + _LineWidth + _Incline_Top, y + _Incline_Right, 0), color, Vector2.zero);
            vh.AddVert(new Vector3(x + _LineWidth, -y + _Incline_Right, 0), color, Vector2.zero);
            x += _LineWidth + _LineSpace;
        }

        if (mod > 0)
        {
            vh.AddVert(new Vector3(x, -y, 0), color, Vector2.zero);
            vh.AddVert(new Vector3(x + _Incline_Top, y, 0), color, Vector2.zero);
            vh.AddVert(new Vector3(x + mod + _Incline_Top, y + _Incline_Right, 0), color, Vector2.zero);
            vh.AddVert(new Vector3(x + mod, -y + _Incline_Right, 0), color, Vector2.zero);
            _Count++;
        }
    }

    private void SetTriangle(VertexHelper vh)
    {
        for (int i = 0; i < _Count * 4; i += 4)
        {
            vh.AddTriangle(i, i + 1, i + 2);
            vh.AddTriangle(i, i + 2, i + 3);
        }
    }
}
