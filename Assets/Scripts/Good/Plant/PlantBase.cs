using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 植物类 基类
/// </summary>
public class PlantBase : MonoBehaviour
{
    public TablePlant.Data plantData { get; set; }
    [SerializeField] private Transform[] _points;
    public Transform[] points { get => _points; protected set => _points = value; }
    public GameObject[] fruits { get; protected set; }

    /// <summary>
    /// 已经生长的时间
    /// </summary>
    public int grownTime { get; protected set; }
    /// <summary>
    /// 是否成熟
    /// </summary>
    public bool isRipe { get; protected set; }

    public event Action onRipeEvent;



    #region Unity Func
    public virtual void Start()
    {
        plantData = TABLE.Get<TablePlant>().dataDict[1];
        fruits = new GameObject[plantData.FruitNum];
        InitFruits();

        StartCoroutine(GrowFruit());
    }

    public virtual void OnDestroy()
    {
        StopAllCoroutines();
        CancelInvoke();
        onRipeEvent = null;
    }
    #endregion

    #region Init
    private void InitFruits()
    {
        var fruitDataDict = TABLE.Get<TableFruit>().dataDict;
        for (int i = 0; i < plantData.FruitNum; i++)
        {
            var fruitData = fruitDataDict[plantData.FruitID];
            var fruit = uAsset.LoadGameObject(fruitData.PrefabName);
            fruit.SetActive(false);
            fruit.transform.SetParent(points[i].transform, false);

            Vector3 fruitSize = fruit.transform.localScale;
            Vector3 parentSize = points[i].transform.localScale;
            Vector3 size = new(fruitSize.x * parentSize.x, fruitSize.y * parentSize.y, fruitSize.z * parentSize.z);
            fruit.transform.localScale = size;

            fruits[i] = fruit;
        }
    }
    #endregion

    #region Fruit Func
    protected virtual IEnumerator GrowFruit()
    {
        if (isRipe) yield break;

        while (true)
        {
            if (grownTime >= plantData.TotalTime)
            {
                isRipe = true;
                GeneratorFruits();
                onRipeEvent?.Invoke();
                yield break;
            }

            yield return new WaitForSeconds(1);
            grownTime++;
        }
    }

    public void PickFruit()
    {
        if (!isRipe) return;

        Main.Data.Player.SetPackageData(plantData.FruitID, plantData.FruitNum);
        isRipe = false;
        grownTime = 0;
        DistoryFruits();
        StartCoroutine(GrowFruit());
    }

    protected virtual void GeneratorFruits()
    {
        foreach (var fruit in fruits)
        {
            fruit.SetActive(true);
        }
    }

    protected virtual void DistoryFruits()
    {
        foreach (var fruit in fruits)
        {
            fruit.SetActive(false);
        }
    }
    #endregion
}
