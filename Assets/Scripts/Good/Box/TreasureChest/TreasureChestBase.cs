using System;
using System.Collections;
using UnityEngine;



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
        public uint time;
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


    public override void Start()
    {
        base.Start();
        if(Main.Data.Map.TryGetRandomGoodData(this, out var data))
        {
            randomGoodData = data;
        }
        StartCoroutine(InitGoods());
    }

    private IEnumerator InitGoods()
    {
        if (!fixedRefresh || !entity) yield break;

        var goodsDict = TABLE.Get<TableGoods>().dataDict;
        uint[] ids = randomGoodData.ids;
        for (int i = 0; i < ids.Length; i++)
        {
            var goodData = goodsDict[ids[i]];
            var go = uAsset.LoadGameObject(goodData.goodName);
            go.transform.position = new Vector3();
        }
    }
}
