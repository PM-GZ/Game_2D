using System;
using System.Collections.Generic;


public class TableRandomGood : TableData
{
	public readonly string filePath = "Assets/EditorAssets/Table/物品表.xlsx";
	public readonly string sheetName = "物品随机表";
	public Dictionary<uint, Data> dataDict;


	[Serializable]
	public struct Data
	{
		public uint ID;
		public byte randomType;
		public uint[] goodsID;
		public uint[][] goodsNum;
		public uint[] randomTime;
		public float[][] goodsPos;
		public float[][] goodsRot;
		public float[][] goodsSize;
	}

	public TableRandomGood()
	{
		if(rawTable == null)
		{
			rawTable = new RawTable();
		}
		rawTable.ReadTable(filePath, sheetName);
		ParseData();
	}

	private void ParseData()
	{
		dataDict = new(rawTable.rowNum - 3);
		for (int i = 0; i < rawTable.rowNum - 3; i++)
		{
			Data data = new();
			data.ID = rawTable.GetUInt(i, 0);
			data.randomType = rawTable.GetByte(i, 1);
			data.goodsID = rawTable.GetUIntArray(i, 2);
			data.goodsNum = rawTable.GetUIntArray2(i, 3);
			data.randomTime = rawTable.GetUIntArray(i, 4);
			data.goodsPos = rawTable.GetFloatArray2(i, 5);
			data.goodsRot = rawTable.GetFloatArray2(i, 6);
			data.goodsSize = rawTable.GetFloatArray2(i, 7);
			dataDict.Add(data.ID, data);
		}
		rawTable = null;
	}
}