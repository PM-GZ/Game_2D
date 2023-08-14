using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[DisallowMultipleComponent]
public class TileMapGenerator : SerializedMonoBehaviour
{
    private struct TileData
    {
        public RuleTile tile;
        [Range(0, 1)] public float weight;
        public Tilemap map;
    }

    [SerializeField] private Grid grid;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField][Range(0.01f, 0.5f)] private float lacunarity;
    [SerializeField] private bool useSeed;
    [SerializeField] private int seed;

    private float[,] weightList;
    [SerializeField] private Tilemap[] tilemaps;
    [SerializeField] private List<TileData> tileDataList;


    #region Function method
    [Button("生成地图")]
    private void GeneratorMap()
    {
        if (tileDataList == null) return;

        weightList = new float[width, height];
        tilemaps = grid.GetComponentsInChildren<Tilemap>();
        SetRandomSeed();
        InitMapData();
        foreach (var tile in tileDataList)
        {
            DrawMap(tile);
        }
    }

    [Button("生成边界")]
    private void GeneratorBounds()
    {

    }

    [Button("清除地图")]
    private void Clear()
    {
        tileDataList.Clear();
        foreach (var tile in tileDataList)
        {
            tile.map.ClearAllTiles();
        }
    }

    [Button("保存")]
    private void Save()
    {

    }
    #endregion

    #region Generator Map
    private void SetRandomSeed()
    {
        if (useSeed)
        {
            seed = Random.Range(0, int.MaxValue);
            Random.InitState(seed);
        }
    }

    private void InitMapData()
    {
        float offset = Random.Range(-10000, 10000);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noise = Mathf.PerlinNoise(x * lacunarity + offset, y * lacunarity + offset);
                weightList[x, y] = noise;
            }
        }
    }

    private void DrawMap(TileData tile)
    {
        List<Vector3Int> posArray = new();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (weightList[x, y] >= tile.weight)
                {
                    posArray.Add(new Vector3Int(x, y));
                }
            }
        }
        tile.map.SetTiles(posArray.ToArray(), new TileBase[] { tile.tile });
    }

    private bool Exist(Vector3Int pos)
    {
        foreach (var map in tilemaps)
        {
            var tile = map.GetTile(pos);

            if (tile != null) return true;
        }
        return false;
    }
    #endregion
}
