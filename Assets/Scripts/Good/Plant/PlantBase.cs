using System;
using UnityEngine;



/// <summary>
/// 植物类 基类
/// </summary>
public class PlantBase : MonoBehaviour
{
    public bool initFriut;

    [SerializeField][DisplayOnly] protected uint plantID = 1;

    [SerializeField] TablePlant.Data _plantData;
    public TablePlant.Data plantData { get => _plantData; private set => _plantData = value; }

    [SerializeField] private Transform[] _points;
    public Transform[] points { get => _points; protected set => _points = value; }

    public GameObject[] fruits { get; protected set; }

    /// <summary>
    /// 已经生长的时间
    /// </summary>
    public ushort curGrownTime
    {
        get
        {
            ulong curTime = TimeUtility.GetNowTimeSeconds();
            return (ushort)(curTime - startGrownTime);
        }
    }
    public ulong startGrownTime { get; private set; }
    public ulong endGrownTime { get; private set; }
    /// <summary>
    /// 是否成熟
    /// </summary>
    public bool isRipe { get; private set; }

    public event Action onRipeEvent;



    #region Unity Func
    public virtual void Start()
    {
        plantData = TABLE.Get<TablePlant>().dataDict[plantID];

        if (plantData.IsFruit)
        {
            fruits = new GameObject[plantData.FruitNum];
            InitFruits();
            Invoke(nameof(GrowFruit), plantData.TotalTime);
        }
    }

    public virtual void OnEnable()
    {
        if (plantData.IsFruit)
        {
            Invoke(nameof(GrowFruit), plantData.TotalTime);
        }
    }

    public virtual void OnDisable()
    {
        StopAllCoroutines();
        CancelInvoke();
        onRipeEvent = null;
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
        if (!plantData.IsFruit) return;

        isRipe = initFriut;

        for (int i = 0; i < plantData.FruitNum; i++)
        {
            var fruit = uAsset.LoadGameObject(plantData.PrefabName);
            InitFruitGO(fruit, points[i]);

            fruits[i] = fruit;
        }
    }

    private void InitFruitGO(GameObject fruit, Transform parent)
    {
        fruit.SetActive(initFriut);
        fruit.transform.SetParent(parent, false);
        fruit.transform.localScale = GetFruitGOSize(fruit.transform.localScale, parent.localScale);
    }
    #endregion

    private Vector3 GetFruitGOSize(Vector3 fruitSize, Vector3 parentSize)
    {
        return new(fruitSize.x * parentSize.x, fruitSize.y * parentSize.y, fruitSize.z * parentSize.z);
    }

    #region Fruit Func
    protected virtual void GrowFruit()
    {
        if (!plantData.IsFruit)
        {
            isRipe = false;
            return;
        }
        if (isRipe) return;

        isRipe = true;
        GeneratorFruits();
        onRipeEvent?.Invoke();
    }

    public void PickFruit()
    {
        if (!isRipe) return;

        isRipe = false;

        DistoryFruits();
        Invoke(nameof(GrowFruit), plantData.TotalTime);

        startGrownTime = TimeUtility.GetNowTimeSeconds();
        endGrownTime = startGrownTime + plantData.TotalTime;
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
