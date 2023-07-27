using System;
using System.Collections.Generic;


public class TableRole : TableData
{
	public readonly string filePath = "Assets/EditorAssets/Table/角色表.xlsx";
	public readonly string sheetName = "角色初始";
	public Dictionary<uint, Data> dataDict;


	[Serializable]
	public struct Data
	{
		public uint ID;
		public string RoleName;
		public string PrefabName;
		public string PortraitPath;
		public ushort Star;
	}

	public TableRole()
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
			data.RoleName = rawTable.GetString(i, 1);
			data.PrefabName = rawTable.GetString(i, 2);
			data.PortraitPath = rawTable.GetString(i, 3);
			data.Star = rawTable.GetUShort(i, 4);
			dataDict.Add(data.ID, data);
		}
		rawTable = null;
	}
}