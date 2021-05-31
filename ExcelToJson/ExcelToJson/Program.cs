using System;
using System.Data;
using System.IO;
using ExcelDataReader;
using Newtonsoft.Json;
using JsonConfigData;
using System.Collections.Generic;

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
                    case "Tree": list = Tree(tbl); break;
                    case "Rock": list = Rock(tbl); break;
                    case "TransferGate": list = TransferGate(tbl); break;
                    default: Console.WriteLine("not find fileName function."); break;
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

                data.ID =               ToInt32(tbl.Rows[i]["ID"]);
                data.Name =             ToString(tbl.Rows[i]["Name"]);
                data.ResourcesName =    ToString(tbl.Rows[i]["ResourcesName"]);
                data.Type =             ToInt32(tbl.Rows[i]["Type"]);
                data.DestroyTime =      ToFloat(tbl.Rows[i]["DestroyTime"]);

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

                cell.ID =               ToInt32(tbl.Rows[i]["ID"]);
                cell.Name =             ToString(tbl.Rows[i]["Name"]);
                cell.SizeX =            ToInt32(tbl.Rows[i]["SizeX"]);
                cell.SizeY =            ToInt32(tbl.Rows[i]["SizeY"]);
                cell.SizeZ =            ToInt32(tbl.Rows[i]["SizeZ"]);
                cell.Block01 =          ToInt32(tbl.Rows[i]["Block01"]);
                cell.Block01Height =    ToInt32(tbl.Rows[i]["Block01Height"]);
                cell.Block02 =          ToInt32(tbl.Rows[i]["Block02"]);
                cell.Block02Height =    ToInt32(tbl.Rows[i]["Block02Height"]);
                cell.Block03 =          ToInt32(tbl.Rows[i]["Block03"]);
                cell.Block03Height =    ToInt32(tbl.Rows[i]["Block03Height"]);
                cell.Mountains =        ToString(tbl.Rows[i]["Mountains"]);
                cell.Trees =            ToString(tbl.Rows[i]["Trees"]);
                cell.Rocks =            ToString(tbl.Rows[i]["Rocks"]);
                cell.TransferGateID =   ToInt32(tbl.Rows[i]["TransferGateID"]);
                cell.PlayerPosX =       ToInt32(tbl.Rows[i]["PlayerPosX"]);
                cell.PlayerPosZ =       ToInt32(tbl.Rows[i]["PlayerPosZ"]);

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

                data.ID =               ToInt32(tbl.Rows[i]["ID"]);
                data.StartPosX =        ToInt32(tbl.Rows[i]["StartPosX"]);
                data.StartPosY =        ToInt32(tbl.Rows[i]["StartPosY"]);
                data.StartPosZ =        ToInt32(tbl.Rows[i]["StartPosZ"]);
                data.Height =           ToInt32(tbl.Rows[i]["Height"]);
                data.Wide =             ToInt32(tbl.Rows[i]["Wide"]);

                list.Add(data);
            }
            return list;
        }
        private static object Tree(DataTable tbl)
        {
            List<Tree> list = new List<Tree>();
            for (int i = 1; i < tbl.Rows.Count; i++)
            {
                var data = new Tree();

                data.ID = ToInt32(tbl.Rows[i]["ID"]);
                data.ResourceName = ToString(tbl.Rows[i]["ResourceName"]);
                data.PosX = ToInt32(tbl.Rows[i]["PosX"]);
                data.PosZ = ToInt32(tbl.Rows[i]["PosZ"]);
                data.Angle = ToInt32(tbl.Rows[i]["Angle"]);
                data.OffsetY = ToFloat(tbl.Rows[i]["OffsetY"]);
                data.Scale = ToFloat(tbl.Rows[i]["Scale"]);
                data.Break = ToInt32(tbl.Rows[i]["Break"]);
                data.HP = ToInt32(tbl.Rows[i]["HP"]);
                data.GetResourceID = ToInt32(tbl.Rows[i]["GetResourceID"]);
                data.GetResourceCount = ToInt32(tbl.Rows[i]["GetResourceCount"]);
                data.Count = ToInt32(tbl.Rows[i]["Count"]);

                list.Add(data);
            }
            return list;
        }
        private static object Rock(DataTable tbl)
        {
            List<Rock> list = new List<Rock>();
            for (int i = 1; i < tbl.Rows.Count; i++)
            {
                var data = new Rock();

                data.ID = ToInt32(tbl.Rows[i]["ID"]);
                data.ResourceName = ToString(tbl.Rows[i]["ResourceName"]);
                data.PosX = ToInt32(tbl.Rows[i]["PosX"]);
                data.PosZ = ToInt32(tbl.Rows[i]["PosZ"]);
                data.Angle = ToInt32(tbl.Rows[i]["Angle"]);
                data.OffsetY = ToFloat(tbl.Rows[i]["OffsetY"]);
                data.Scale = ToFloat(tbl.Rows[i]["Scale"]);
                data.Break = ToInt32(tbl.Rows[i]["Break"]);
                data.HP = ToInt32(tbl.Rows[i]["HP"]);
                data.GetResourceID = ToInt32(tbl.Rows[i]["GetResourceID"]);
                data.GetResourceCount = ToInt32(tbl.Rows[i]["GetResourceCount"]);
                data.Count = ToInt32(tbl.Rows[i]["Count"]);

                list.Add(data);
            }
            return list;
        }
        private static object TransferGate(DataTable tbl)
        {
            List<TransferGate> list = new List<TransferGate>();
            for (int i = 1; i < tbl.Rows.Count; i++)
            {
                var data = new TransferGate();

                data.ID = ToInt32(tbl.Rows[i]["ID"]);
                data.ResourcesPath = ToString(tbl.Rows[i]["ResourcesPath"]);
                data.NextMap = ToInt32(tbl.Rows[i]["NextMap"]);
                data.NextMapSceneName = ToString(tbl.Rows[i]["NextMapSceneName"]);
                data.PosX = ToInt32(tbl.Rows[i]["PosX"]);
                data.PosY = ToInt32(tbl.Rows[i]["PosY"]);
                data.PosZ = ToInt32(tbl.Rows[i]["PosZ"]);

                list.Add(data);
            }
            return list;
        }

        private static int ToInt32(object item)
        {
            if (item == DBNull.Value)
                return -1;
            return Convert.ToInt32(item);
        }
        private static float ToFloat(object item)
        {
            if (item == DBNull.Value)
                return -1;
            return (float)Convert.ToDouble(item);
        }
        private static string ToString(object item)
        {
            if (item == DBNull.Value)
                return "N";
            return item.ToString();
        }
    }
}