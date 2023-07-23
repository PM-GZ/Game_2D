using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

    #region Event Func
    private void OnSwitchScene()
    {
        UpdateMapData(Main.Scene.curScene);
    }
    #endregion

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
