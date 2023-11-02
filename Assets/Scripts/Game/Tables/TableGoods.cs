using System;
using System.Collections.Generic;
using UnityEngine;

public partial class TableGoods : TableData
{
	public readonly string sFilePath = "tTableItemData";
	public readonly long Position = 67;
	public Dictionary<uint, tData> mData;
	public List<tData> DataList;
	public partial struct tData
	{
		/// <summary>
		///物品id
		/// </summary>
		public uint ID;
		/// <summary>
		///物品名称
		/// </summary>
		public string goodName;
		/// <summary>
		///预制体路径
		/// </summary>
		public string PrefabName;
		/// <summary>
		///icon路径
		/// </summary>
		public string IconPath;
		public bool IsNull
		{
			get 
			{
				return ID <= 0;
			}
		}
	}

	public TableGoods()
	{
		try
		{
			PACKET_NAME = "UiPackage.p";
			ReadTable();
			ParseData();
		}
		catch(Exception exp)
		{
			Debug.Log($"LOAD TABLE ERROR!{exp}");
		}
	}

	void ReadTable()
	{
		if (mTableData == null)
		{
			mTableData = new RawTable();
		}
		mTableData.ReadBinary(sFilePath, Position, PACKET_NAME);
	}
	void ParseData()
	{
		mData = new Dictionary<uint, tData>(mTableData._nRows);
		DataList = new List<tData>(mTableData._nRows);
		for (int i = 0; i < mTableData._nRows; i++)
		{
			tData data = new tData();
			data.ID = mTableData.GetUInt(i,0);
			data.goodName = mTableData.GetString(i,1);
			data.PrefabName = mTableData.GetString(i,2);
			data.IconPath = mTableData.GetString(i,3);
			mData.Add(data.ID, data);
			DataList.Add(data);
		}
		 mTableData = null;
	}
}
