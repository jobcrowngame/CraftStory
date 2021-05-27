using System;
using System.Data;
using System.IO;
using ExcelDataReader;
using Newtonsoft.Json;
using JsonConfigData;
using System.Collections.Generic;
using System.Linq;
using System.Data.OleDb;

namespace ExcelToJson
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string inFilePath = args[0];
            string outFilePath = args[1];

            Formatting indented = args.Length > 2 && args[2] == "true"
                ? Formatting.Indented
                : Formatting.None;

            if (Directory.Exists(outFilePath))
                Directory.Delete(outFilePath, true);

            Directory.CreateDirectory(outFilePath);

            string[] files = Directory.GetFiles(inFilePath, "*", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                string outFileName = Path.GetFileNameWithoutExtension(files[i]);
                ExcelToJson(files[i], outFilePath, outFileName, indented);
            }
        }
        private static DataSet ReadExcelData(string path)
        {
            DataSet ds = null;
            var ext = Path.GetExtension(path);

            try
            {
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        ds = reader.AsDataSet();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }

            return ds;
        }

        private static void ExcelToJson(string inFilePath, string outputPath, string fileName, Formatting indented)
        {
            var ds = ReadExcelData(inFilePath);
            if (ds == null)
                return;

            foreach (DataTable tbl in ds.Tables)
            {
                for (int i = 0; i < tbl.Columns.Count; i++)
                {
                    tbl.Columns[i].ColumnName = tbl.Rows[0].ItemArray[i].ToString();
                }

                object list = null;
                switch (fileName)
                {
                    case "Block": list = Block(tbl); break;
                    case "Map": list = Map(tbl); break;
                    case "MapMountain": list = MapMountain(tbl); break;
                    default:
                        break;
                }

                var json = JsonConvert.SerializeObject(list, indented);
                File.WriteAllText(outputPath + fileName + ".json", json);
            }
        }

        private static object Block(DataTable tbl)
        {
            List<Block> list = new List<Block>();
            for (int i = 1; i < tbl.Rows.Count; i++)
            {
                var data = new Block();
                data.ID = ToInt32(tbl.Rows[i].ItemArray[0]);
                data.Name = Convert.ToString(tbl.Rows[i].ItemArray[1]);
                data.ResourcesName = Convert.ToString(tbl.Rows[i].ItemArray[2]);
                data.Type = ToInt32(tbl.Rows[i].ItemArray[3]);
                data.DestroyTime = (float)Convert.ToDouble(tbl.Rows[i].ItemArray[4]);

                list.Add(data);
            }
            return list;
        }
        private static object Map(DataTable tbl)
        {
            List<Map> list = new List<Map>();
            for (int i = 1; i < tbl.Rows.Count; i++)
            {
                var cell = new Map();
                cell.ID = Convert.ToInt32( tbl.Rows[i]["ID"]);
                cell.Name = tbl.Rows[i]["Name"].ToString();
                cell.SizeX = Convert.ToInt32(tbl.Rows[i]["SizeX"]);
                cell.SizeY = Convert.ToInt32(tbl.Rows[i]["SizeY"]);
                cell.SizeZ = Convert.ToInt32(tbl.Rows[i]["SizeZ"]);
                cell.Block01 = Convert.ToInt32(tbl.Rows[i]["Block01"]);
                cell.Block01Height = Convert.ToInt32(tbl.Rows[i]["Block01Height"]);
                cell.Block02 = Convert.ToInt32(tbl.Rows[i]["Block02"]);
                cell.Block02Height = Convert.ToInt32(tbl.Rows[i]["Block02Height"]);
                cell.Block03 = Convert.ToInt32(tbl.Rows[i]["Block03"]);
                cell.Block03Height = Convert.ToInt32(tbl.Rows[i]["Block03Height"]);
                cell.Mountains = tbl.Rows[i]["Mountains"].ToString();
                list.Add(cell);
            }
            return list;
        }
        private static object MapMountain(DataTable tbl)
        {
            List<Mountain> list = new List<Mountain>();
            for (int i = 1; i < tbl.Rows.Count; i++)
            {
                var data = new Mountain();
                data.ID = ToInt32(tbl.Rows[i].ItemArray[0]);
                data.StartPosX = ToInt32(tbl.Rows[i].ItemArray[1]);
                data.StartPosY = ToInt32(tbl.Rows[i].ItemArray[2]);
                data.StartPosZ = ToInt32(tbl.Rows[i].ItemArray[3]);
                data.Height = ToInt32(tbl.Rows[i].ItemArray[4]);
                data.Wide = ToInt32(tbl.Rows[i].ItemArray[5]);

                list.Add(data);
            }
            return list;
        }

        private static int ToInt32(object obj)
        {
            if (obj == DBNull.Value)
                return -1;
            return Convert.ToInt32(obj);
        }
    }
}