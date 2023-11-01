using System;
using System.Collections.Generic;
using UnityEngine;

public partial class TableRole : TableData
{
	public readonly string sFilePath = "tTableRoleData";
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
		public string RoleName;
		/// <summary>
		///预制体路径
		/// </summary>
		public string PrefabName;
		/// <summary>
		///icon路径
		/// </summary>
		public string PortraitPath;
		/// <summary>
		///品质
		/// </summary>
		public ushort Star;
		/// <summary>
		///速度
		/// </summary>
		public ushort Speed;
		public bool IsNull
		{
			get 
			{
				return ID <= 0;
			}
		}
	}

	public TableRole()
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
		mTableData.ReadBinary(sFilePath, PACKET_NAME);
	}
	void ParseData()
	{
		mData = new Dictionary<uint, tData>(mTableData._nRows);
		DataList = new List<tData>(mTableData._nRows);
		for (int i = 0; i < mTableData._nRows; i++)
		{
			tData data = new tData();
			data.ID = mTableData.GetUInt(i,0);
			data.RoleName = mTableData.GetString(i,1);
			data.PrefabName = mTableData.GetString(i,2);
			data.PortraitPath = mTableData.GetString(i,3);
			data.Star = mTableData.GetUShort(i,4);
			data.Speed = mTableData.GetUShort(i,5);
			mData.Add(data.ID, data);
			DataList.Add(data);
		}
		 mTableData = null;
	}
}
