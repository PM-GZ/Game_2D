using System;
using System.Collections.Generic;


public class TableFruit : TableData
{
	public readonly string filePath = "Assets/EditorAssets/Table/物品表.xlsx";
	public readonly string sheetName = "果实表";
	public Dictionary<uint, Data> dataDict;


	public struct Data
	{
		public uint ID;
		public string FruitName;
		public string PrefabName;
		public string IconPath;
		public ushort ShelfLife;
		public uint PutrilageID;
	}

	public TableFruit()
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
			data.FruitName = rawTable.GetString(i, 1);
			data.PrefabName = rawTable.GetString(i, 2);
			data.IconPath = rawTable.GetString(i, 3);
			data.ShelfLife = rawTable.GetUShort(i, 4);
			data.PutrilageID = rawTable.GetUInt(i, 5);
			dataDict.Add(data.ID, data);
		}
		rawTable = null;
	}
}