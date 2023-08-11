using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[DisallowMultipleComponent]
public class TileMapInspecterEditor : SerializedMonoBehaviour
{
    public struct TileData
    {
        public RuleTile tile;
        [Range(0, 1)]public float rate;
        public Tilemap map;
    }

    public bool useSeed;
    public int seed;

    public List<TileData> tileDataList;


    [Button("���ɵ�ͼ")]
    private void GeneratorMap()
    {
        if (tileDataList == null) return;

        if (useSeed)
        {
            seed = Random.Range(0, int.MaxValue);
            Random.InitState(seed);
        }
        foreach (var tile in tileDataList)
        {

        }
    }

    [Button("����")]
    private void Save()
    {

    }
}
