using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawImage : Graphic
{

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Vector2 size = rectTransform.sizeDelta;

        float width = size.x / 5;
        for (int i = 0; i < 5; i++)
        {
            vh.AddVert(new Vector3(size.x + (i * width) + 10, 0, 0), Color.white, Vector2.zero);
            vh.AddVert(new Vector3(size.x + (i * width) + 10, size.y, 0), Color.white, Vector2.zero);
            vh.AddVert(new Vector3(size.x + (i * width) + width + 10, size.y, 0), Color.white, Vector2.zero);
            vh.AddVert(new Vector3(size.x + (i * width) + width + 10, 0, 0), Color.white, Vector2.zero);
        }

        for (int i = 0; i < 5; i++)
        {
            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }
    }
}
