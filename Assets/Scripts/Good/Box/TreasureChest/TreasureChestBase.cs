using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class TreasureChestBase : GoodsBase, IGoodRefresh
{
    public enum RandomGoodType
    {
        FixedGood = 1,
        RandomGood = 2,

        FixedTime = 3,
    }

    [Serializable]
    public struct RandomGoodData
    {
        public RandomGoodType type;
        public uint[] ids;
        public uint[] nums;
        public uint[] time;
        public float[][] posArray;
        public float[][] rotArray;
        public float[][] sizeArray;
    }

    [SerializeField] private bool _fixedRefresh;
    public bool fixedRefresh { get => _fixedRefresh; }

    [SerializeField] private bool _entity = true;
    public bool entity { get => _entity; }

    [SerializeField] private uint _randomGoodID;
    public uint randomGoodID { get => _randomGoodID; }


    public RandomGoodData randomGoodData { get; private set; }
    private List<GameObject> goodList;


    #region Unity Func
    public override void Start()
    {
        base.Start();
        if (Main.Data.Map.TryGetRandomGoodData(this, out var data))
        {
            randomGoodData = data;
        }

        nothingGoods = randomGoodData.ids.Length != 0;
        StartCoroutine(InitGoods());
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    #endregion


    private IEnumerator InitGoods()
    {
        if (!fixedRefresh || !entity) yield break;

        goodList = new List<GameObject>(randomGoodData.ids.Length);
        var goodsDict = TABLE.Get<TableGoods>().dataDict;
        uint[] ids = randomGoodData.ids;
        for (int i = 0; i < ids.Length; i++)
        {
            var goodData = goodsDict[ids[i]];
            var go = uAsset.LoadGameObject(goodData.goodName);

            SetGoodTransform(go.transform, i);

            goodList.Add(go);
        }
    }

    private void SetGoodTransform(Transform go, int index)
    {
        float[] pos = randomGoodData.posArray[index];
        float[] rot = randomGoodData.posArray[index];
        float[] size = randomGoodData.posArray[index];

        go.position = GetVector(pos);
        go.Rotate(GetVector(rot));
        go.localScale = GetVector(size);
    }

    private Vector3 GetVector(float[] vectors)
    {
        Vector3 vector = new Vector3(vectors[0], vectors[1], vectors[2]);
        return vector;
    }

    public uint GetGoodNum(int index)
    {
        return randomGoodData.nums[index];
    }

    public void TakeAll()
    {
        nothingGoods = false;
        DistoryGoods();
        SetGoodsRefresh();
    }

    protected virtual void GeneratorGoods()
    {
        nothingGoods = true;

        if (!entity) return;
        foreach (var item in goodList)
        {
            item.SetActive(true);
        }
    }

    protected virtual void DistoryGoods()
    {
        foreach (var item in goodList)
        {
            if (randomGoodData.type == RandomGoodType.FixedGood)
            {
                Destroy(item);
            }
            else
            {
                item.SetActive(false);
            }
        }
    }

    private void SetGoodsRefresh()
    {
        switch (randomGoodData.type)
        {
            case RandomGoodType.FixedTime:
                Invoke(nameof(GeneratorGoods), randomGoodData.time[0]);
                break;
            case RandomGoodType.RandomGood:
                Invoke(nameof(GeneratorGoods), GetRandomTime());
                break;
        }
    }

    private uint GetRandomTime()
    {
        return (uint)Random.Range(randomGoodData.time[0], randomGoodData.time[1]);
    }
}
