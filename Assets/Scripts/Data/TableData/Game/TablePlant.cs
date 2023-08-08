using System;
using System.Collections.Generic;


public class TablePlant : TableData
{
	public readonly string filePath = "Assets/EditorAssets/Table/物品表.xlsx";
	public readonly string sheetName = "植物表";
	public Dictionary<uint, Data> dataDict;


	[Serializable]
	public struct Data
	{
		public uint ID;
		public string PlantName;
		public string PrefabName;
		public string IconPath;
		public bool IsFruit;
		public byte Season;
		public uint FruitID;
		public ushort FruitNum;
		public ushort TotalTime;
		public ushort FruitPhase;
	}

	public TablePlant()
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
			data.PlantName = rawTable.GetString(i, 1);
			data.PrefabName = rawTable.GetString(i, 2);
			data.IconPath = rawTable.GetString(i, 3);
			data.IsFruit = rawTable.GetBool(i, 4);
			data.Season = rawTable.GetByte(i, 5);
			data.FruitID = rawTable.GetUInt(i, 6);
			data.FruitNum = rawTable.GetUShort(i, 7);
			data.TotalTime = rawTable.GetUShort(i, 8);
			data.FruitPhase = rawTable.GetUShort(i, 9);
			dataDict.Add(data.ID, data);
		}
		rawTable = null;
	}
}