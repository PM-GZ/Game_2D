using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RandomGoodType = TreasureChestBase.RandomGoodType;
using RandomGoodData = TreasureChestBase.RandomGoodData;
using System.Linq;

public class uMapData : DataBase
{
    public struct MapData
    {
        public string sceneName;
        public Dictionary<int, TablePlant.Data> plantDict;
        public Dictionary<int, TableFruit.Data> fruitDict;

        public static MapData Default
        {
            get
            {
                MapData data = new MapData();
                data.sceneName = "";
                data.plantDict = new();
                data.fruitDict = new();
                return data;
            }
        }
    }


    private const string MAP_DATA_NAME = "MapData";
    public Dictionary<string, MapData> mapDataDict { get; private set; }
    private GameObject[] _mapGOs;
    private Dictionary<IGoodRefresh, RandomGoodData> _randomGoodDataDict = new();
    private List<int> idIndexs = new();


    #region override
    public override void InitData()
    {
        ReadMapData();
    }

    public override void InitEvent()
    {
        Main.Scene.onSwitchScene += OnSwitchScene;
    }

    public override void ClearData()
    {
        SaveMapData();
    }

    public override void ClearEvent()
    {
        Main.Scene.onSwitchScene -= OnSwitchScene;
    }
    #endregion

    #region Init
    public void InitMapGoodsData()
    {
        var scene = Main.Scene.curScene;
        _mapGOs = scene.GetRootGameObjects();
    }

    public void InitRandomGoods()
    {
        var randomGoodsDict = TABLE.Get<TableRandomGood>().dataDict;
        foreach (var go in _mapGOs)
        {
            var refreshs = go.GetComponentsInChildren<IGoodRefresh>();
            foreach (var refresh in refreshs)
            {
                var randomGoodData = randomGoodsDict[refresh.randomGoodID];
                SetRandomGoodData(randomGoodData, refresh);
            }
        }
    }

    private void SetRandomGoodData(TableRandomGood.Data randomGoodData, IGoodRefresh refresh)
    {
        var type = randomGoodData.randomType;

        RandomGoodData data = new()
        {
            type = (RandomGoodType)type,
            nums = GetRandomNum(randomGoodData.goodsNum),
            time = (uint)Random.Range(randomGoodData.randomTime[0], randomGoodData.randomTime[1]),
        };

        switch ((RandomGoodType)type)
        {
            case RandomGoodType.FixedGood:
                data.ids = randomGoodData.goodsID;
                data.nums = randomGoodData.goodsNum[0];
                break;
            case RandomGoodType.RandomGood:
                data.ids = GetRandomId(randomGoodData.goodsID);
                break;
            case RandomGoodType.FixedTime:
                data.time = randomGoodData.randomTime[0];
                break;
        }

        data.posArray = GetVecterData(randomGoodData.goodsPos);
        data.rotArray = GetVecterData(randomGoodData.goodsRot);
        data.sizeArray = GetVecterData(randomGoodData.goodsSize);
        _randomGoodDataDict.Add(refresh, data);
    }

    private uint[] GetRandomId(uint[] array)
    {
        idIndexs.Clear();

        uint[] idArray = new uint[Random.Range(0, array.Length)];
        for (int i = 0; i < idArray.Length; i++)
        {
            int index = Random.Range(0, array.Length);
            if (!idArray.Contains(array[index]))
            {
                idArray[i] = array[index];
                idIndexs.Add(index);
            }
        }
        return idArray;
    }

    private uint[] GetRandomNum(uint[][] array)
    {
        uint[] idArray = new uint[Random.Range(0, array.Length)];
        for (int i = 0; i < array.Length; i++)
        {
            idArray[i] = (uint)Random.Range(array[i][0], array[i][1]);
        }
        return idArray;
    }

    private float[][] GetVecterData(float[][] goodsPos)
    {
        float[][] vectors = new float[idIndexs.Count][];
        for (int i = 0; i < idIndexs.Count; i++)
        {
            vectors[i][0] = goodsPos[i][0];
            vectors[i][1] = goodsPos[i][1];
            vectors[i][2] = goodsPos[i][2];
        }
        return vectors;
    }
    #endregion

    #region Event Func
    private void OnSwitchScene()
    {
        UpdateMapData(Main.Scene.curScene);
    }
    #endregion

    public bool TryGetRandomGoodData(IGoodRefresh refresh, out RandomGoodData goodData)
    {
        return _randomGoodDataDict.TryGetValue(refresh, out goodData);
    }

    private void UpdateMapData(Scene scene)
    {
        if (scene.Equals(default) || string.IsNullOrEmpty(scene.name)) return;
        mapDataDict.TryAdd(scene.name, MapData.Default);

        MapData mapData = MapData.Default;
        foreach (var root in scene.GetRootGameObjects())
        {
            var plants = root.GetComponentsInChildren<PlantBase>();
            foreach (var plant in plants)
            {
                int id = plant.gameObject.GetInstanceID();
                mapData.plantDict.Add(id, plant.plantData);
            }
        }
        mapData.sceneName = scene.name;
        mapDataDict[scene.name] = mapData;
    }

    #region 保存/读取 地图数据
    private void ReadMapData()
    {
        StartCoroutine(GetMapData());
    }

    private IEnumerator GetMapData()
    {
        var task = FileUtility.GetFileAsync<Dictionary<string, MapData>>(MAP_DATA_NAME, FileUtility.FileType.MapData);
        while (!task.IsCompleted)
        {
            yield return null;
        }
        if (task == null || task.Result == null)
        {
            mapDataDict = new Dictionary<string, MapData>();
            yield break;
        }

        mapDataDict = task.Result;
    }

    public void SaveMapData()
    {
        FileUtility.WriteFile(MAP_DATA_NAME, FileUtility.FileType.MapData, mapDataDict);
    }
    #endregion
}
