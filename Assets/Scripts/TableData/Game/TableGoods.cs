using System;
using System.Collections.Generic;


public class TableGoods : TableData
{
	public readonly string filePath = "Assets/EditorAssets/Table/物品表.xlsx";
	public readonly string sheetName = "物品表";
	public Dictionary<uint, Data> dataDict;


	[Serializable]
	public struct Data
	{
		public uint ID;
		public string goodName;
		public string PrefabName;
		public string IconPath;
	}

	public TableGoods()
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
			data.goodName = rawTable.GetString(i, 1);
			data.PrefabName = rawTable.GetString(i, 2);
			data.IconPath = rawTable.GetString(i, 3);
			dataDict.Add(data.ID, data);
		}
		rawTable = null;
	}
}