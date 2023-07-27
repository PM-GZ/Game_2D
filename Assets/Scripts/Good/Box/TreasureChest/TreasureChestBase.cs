using System;
using UnityEngine;



public class TreasureChestBase : MonoBehaviour, IGoodRefresh
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

    [SerializeField] private uint _randomGoodID;
    public uint randomGoodID { get => _randomGoodID; }

    public RandomGoodData randomGoodData { get; set; }


    private void Start()
    {
        
    }
}
