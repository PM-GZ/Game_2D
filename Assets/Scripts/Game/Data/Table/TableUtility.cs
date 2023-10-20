using System.IO;
using System.Linq;
using OfficeOpenXml;


public class TableUtility
{
    public static byte[] GetFile(string tableName)
    {
        string pattern = Path.GetExtension(tableName);
        string[] files = Directory.GetFiles($"{Constent.TABLE_CONFIG_PATH}", $"*{pattern}", SearchOption.AllDirectories);
        string fileName = Path.GetFileName(tableName);
        foreach (var item in files)
        {
            if (string.Equals(Path.GetFileName(item), fileName))
            {
                return File.ReadAllBytes(item.Replace('\\', '/'));
            }
        }
        return null;
    }

    public static ExcelWorksheet GetTable(string tableName, string sheetName)
    {
        FileInfo fileInfo = new FileInfo(tableName);
        ExcelPackage excel = new ExcelPackage(fileInfo);

        return excel.Workbook.Worksheets[sheetName];
    }

    public static bool IsRowEmpty(ExcelWorksheet sheet, int startRow, int endRow)
    {
        int endCol = sheet.Cells.End.Column;
        return sheet.Cells[startRow, 1, endRow, endCol].All(c => c.Value == null);
    }

    public static void RemoveEmptyRow(ExcelWorksheet sheet)
    {
        for (int i = sheet.Dimension.Rows; i >= 1; i--)
        {
            for (int j = sheet.Dimension.Columns; j >= 1; j--)
            {
                if(IsRowEmpty(sheet, i, i))
                {
                    sheet.DeleteRow(i);
                }
            }
        }
    }
}
