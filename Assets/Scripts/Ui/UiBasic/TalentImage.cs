using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class TalentImage : Graphic
{
    [SerializeField] private Texture _Texture;
    [SerializeField] private bool _Fill;
    [SerializeField][Range(0, 360)] private float _Rotate;
    [SerializeField][Range(3, 10)] private ushort _VertexCount;
    [SerializeField][Range(0, 1)] private float[] _VertexDistance = new float[3];

    [SerializeField] bool _DrawLine;
    [SerializeField][Min(2)] private int _LineCount;
    [SerializeField] private Color _LineColor;
    [SerializeField] private float _LineWidth;

    private float size;
    private float _perRadian;
    private Vector2[] _vertexs;

    public override Texture mainTexture
    {
        get
        {
            return _Texture == null ? s_WhiteTexture : _Texture;
        }
    }
    public Texture texture
    {
        get
        {
            return _Texture;
        }
        set
        {
            if (_Texture == value) return;
            _Texture = value;
            SetVerticesDirty();
            SetMaterialDirty();
        }
    }

    private void Update()
    {
        size = rectTransform.rect.width;
        if (rectTransform.rect.width > rectTransform.rect.height)
            size = rectTransform.rect.height;
        else
            size = rectTransform.rect.width;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (_DrawLine)
        {
            DrawAxis(vh);
            DrawRuling(vh);
        }
        DrawTalentMap(vh);
    }
    private void DrawAxis(VertexHelper vh)
    {
        GetVertexs();
        for (int i = 0; i < _vertexs.Length; i++)
        {
            vh.AddUIVertexQuad(GetQuad(Vector2.zero, _vertexs[i], _LineColor, _LineWidth));
        }
    }

    private void GetVertexs()
    {
        _perRadian = Mathf.PI * 2 / _VertexCount;
        _vertexs = new Vector2[_VertexCount];
        for (int i = 0; i < _VertexCount; i++)
        {
            float radian = _perRadian * i + 90 * Mathf.Deg2Rad;
            Vector2 endPos = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * (size / 2);
            _vertexs[i] = endPos;
        }
    }

    private void DrawRuling(VertexHelper vh)
    {
        float perRadius = size / 2 / (_LineCount - 1);//原点不需要画
        float degrees = 2 * Mathf.PI / _VertexCount;
        for (int i = 1; i < _LineCount; i++)
        {
            for (int j = 0; j < _VertexCount; j++)
            {
                float startRadian = degrees * j + 90 * Mathf.Deg2Rad;
                float endRadian = degrees * (j + 1) + 90 * Mathf.Deg2Rad;
                Vector2 startPos = new Vector2(Mathf.Cos(startRadian), Mathf.Sin(startRadian)) * perRadius * i;
                Vector2 endPos = new Vector2(Mathf.Cos(endRadian), Mathf.Sin(endRadian)) * perRadius * i;
                UIVertex[] newVertexs = GetQuad(startPos, endPos, _LineColor, _LineWidth);
                vh.AddUIVertexQuad(newVertexs);
            }
        }
    }
    private UIVertex[] GetQuad(Vector2 startPos, Vector2 endPos, Color color, float width)
    {
        float dis = Vector2.Distance(startPos, endPos);
        float x = width / 2 * (endPos.y - startPos.y) / dis;//sin
        float y = width / 2 * (endPos.x - startPos.x) / dis;//cos
        if (y <= 0) y = -y;
        else x = -x;
        UIVertex[] vertex = new UIVertex[4];
        vertex[0].position = new Vector3(startPos.x + x, startPos.y + y);
        vertex[1].position = new Vector3(endPos.x + x, endPos.y + y);
        vertex[2].position = new Vector3(endPos.x - x, endPos.y - y);
        vertex[3].position = new Vector3(startPos.x - x, startPos.y - y);
        for (int i = 0; i < vertex.Length; i++)
            vertex[i].color = color;
        return vertex;
    }

    private void DrawTalentMap(VertexHelper vh)
    {
        Vector2 uv0 = new Vector2(0, 1);
        Vector2 uv1 = new Vector2(1, 1);
        Vector2 uv2 = new Vector2(1, 0);
        Vector2 uv3 = new Vector2(0, 0);
        Vector2 prevX = Vector2.zero;
        Vector2 prevY = Vector2.zero;
        Vector2 pos0;
        Vector2 pos1;
        Vector2 pos2;
        Vector2 pos3;

        int vertexs = _VertexCount + 1;
        if (_VertexDistance.Length != vertexs)
        {
            _VertexDistance = new float[vertexs];
            for (int i = 0; i < vertexs - 1; i++)
            {
                _VertexDistance[i] = 1;
            }
        }

        float degrees = 2 * Mathf.PI / _VertexCount;
        _VertexDistance[vertexs - 1] = _VertexDistance[0];
        for (int i = 0; i < vertexs; i++)
        {
            float outer = -rectTransform.pivot.x * size * _VertexDistance[i];
            float rad = i * degrees + _Rotate;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            pos0 = prevX;
            pos1 = new Vector2(outer * cos, outer * sin);
            if (_Fill)
            {
                pos2 = Vector2.zero;
                pos3 = Vector2.zero;
            }
            else
            {
                pos2 = new Vector2(outer * cos, outer * sin);
                pos3 = prevY;
            }
            prevX = pos1;
            prevY = pos2;
            vh.AddUIVertexQuad(SetVertexs(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
        }
    }

    protected UIVertex[] SetVertexs(Vector2[] vertices, Vector2[] uvs)
    {
        UIVertex[] vbo = new UIVertex[4];
        for (int i = 0; i < vertices.Length; i++)
        {
            var vert = UIVertex.simpleVert;
            vert.color = color;
            vert.position = vertices[i];
            vert.uv0 = uvs[i];
            vbo[i] = vert;
        }
        return vbo;
    }

    public void SetRateValue(float[] rates)
    {
        for (int i = 0; i < _VertexDistance.Length && i < rates.Length; i++)
        {
            _VertexDistance[i] = rates[i];
        }
    }
}
