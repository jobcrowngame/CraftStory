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

            DataTable tbl = ds.Tables[0];

            for (int i = 0; i < tbl.Columns.Count; i++)
            {
                tbl.Columns[i].ColumnName = tbl.Rows[0].ItemArray[i].ToString();
            }

            object list = null;

            switch (fileName)
            {
                case "Block": list = Block(tbl); break;
                case "Bonus": list = Bonus(tbl); break;
                case "Map": list = Map(tbl); break;
                case "MapMountain": list = MapMountain(tbl); break;
                case "Resource": list = Resource(tbl); break;
                case "TransferGate": list = TransferGate(tbl); break;
                case "Item": list = Item(tbl); break;
                case "Craft": list = Craft(tbl); break;
                case "Building": list = Building(tbl); break;
                default: Console.WriteLine("not find fileName function."); break;
            }

            var json = LitJson.JsonMapper.ToJson(list);

            //var json = JsonConvert.SerializeObject(list, indented);
            File.WriteAllText(outputPath + fileName + ".json", json);
        }

        private static object Block(DataTable tbl)
        {
            List<Block> list = new List<Block>();
            for (int i = 1; i < tbl.Rows.Count; i++)
            {
                var data = new Block();

                data.ID =               ToInt32(tbl.Rows[i]["ID"]);
                data.ItemID =               ToInt32(tbl.Rows[i]["ItemID"]);
                data.Name =             ToString(tbl.Rows[i]["Name"]);
                data.ResourcesName =    ToString(tbl.Rows[i]["ResourcesName"]);
                data.Type =             ToInt32(tbl.Rows[i]["Type"]);
                data.DestroyTime =      ToFloat(tbl.Rows[i]["DestroyTime"]);

                list.Add(data);
            }
            return list;
        }
        private static object Bonus(DataTable tbl)
        {
            List<Bonus> list = new List<Bonus>();
            for (int i = 1; i < tbl.Rows.Count; i++)
            {
                var data = new Bonus();

                data.ID = ToInt32(tbl.Rows[i]["ID"]);
                data.Bonus1 = ToInt32(tbl.Rows[i]["Bonus1"]);
                data.BonusCount1 = ToInt32(tbl.Rows[i]["BonusCount1"]);
                data.Bonus2 = ToInt32(tbl.Rows[i]["Bonus2"]);
                data.BonusCount2 = ToInt32(tbl.Rows[i]["BonusCount2"]);
                data.Bonus3 = ToInt32(tbl.Rows[i]["Bonus3"]);
                data.BonusCount3 = ToInt32(tbl.Rows[i]["BonusCount3"]);
                data.Bonus4 = ToInt32(tbl.Rows[i]["Bonus4"]);
                data.BonusCount4 = ToInt32(tbl.Rows[i]["BonusCount4"]);
                data.Bonus5 = ToInt32(tbl.Rows[i]["Bonus5"]);
                data.BonusCount5 = ToInt32(tbl.Rows[i]["BonusCount5"]);
                data.Bonus6 = ToInt32(tbl.Rows[i]["Bonus6"]);
                data.BonusCount6 = ToInt32(tbl.Rows[i]["BonusCount6"]);

                list.Add(data);
            }
            return list;
        }
        private static object Map(DataTable tbl)
        {
            List<Map> list = new List<Map>();
            for (int i = 1; i < tbl.Rows.Count; i++)
            {
                var data = new Map();

                data.ID =               ToInt32(tbl.Rows[i]["ID"]);
                data.Name =             ToString(tbl.Rows[i]["Name"]);
                data.SizeX =            ToInt32(tbl.Rows[i]["SizeX"]);
                data.SizeY =            ToInt32(tbl.Rows[i]["SizeY"]);
                data.SizeZ =            ToInt32(tbl.Rows[i]["SizeZ"]);
                data.Block01 =          ToInt32(tbl.Rows[i]["Block01"]);
                data.Block01Height =    ToInt32(tbl.Rows[i]["Block01Height"]);
                data.Block02 =          ToInt32(tbl.Rows[i]["Block02"]);
                data.Block02Height =    ToInt32(tbl.Rows[i]["Block02Height"]);
                data.Block03 =          ToInt32(tbl.Rows[i]["Block03"]);
                data.Block03Height =    ToInt32(tbl.Rows[i]["Block03Height"]);
                data.Mountains =        ToString(tbl.Rows[i]["Mountains"]);
                data.Resources =         ToString(tbl.Rows[i]["Resources"]);
                data.TransferGateID =   ToInt32(tbl.Rows[i]["TransferGateID"]);
                data.Buildings =        ToString(tbl.Rows[i]["Buildings"]);
                data.PlayerPosX =       ToInt32(tbl.Rows[i]["PlayerPosX"]);
                data.PlayerPosZ =       ToInt32(tbl.Rows[i]["PlayerPosZ"]);
                data.CreatePosOffset =  ToInt32(tbl.Rows[i]["CreatePosOffset"]);

                list.Add(data);
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
        private static object Resource(DataTable tbl)
        {
            List<Resource> list = new List<Resource>();
            for (int i = 1; i < tbl.Rows.Count; i++)
            {
                var data = new Resource();

                data.ID = ToInt32(tbl.Rows[i]["ID"]);
                data.ResourcePath = ToString(tbl.Rows[i]["ResourcePath"]);
                data.Type = ToInt32(tbl.Rows[i]["Type"]);
                data.PosX = ToInt32(tbl.Rows[i]["PosX"]);
                data.PosZ = ToInt32(tbl.Rows[i]["PosZ"]);
                data.Angle = ToInt32(tbl.Rows[i]["Angle"]);
                data.OffsetY = ToFloat(tbl.Rows[i]["OffsetY"]);
                data.CreatePosOffset =  ToInt32(tbl.Rows[i]["CreatePosOffset"]);
                data.Scale = ToFloat(tbl.Rows[i]["Scale"]);
                data.Count = ToInt32(tbl.Rows[i]["Count"]);
                data.CanBreak = ToInt32(tbl.Rows[i]["CanBreak"]);
                data.DestroyTime = ToFloat(tbl.Rows[i]["DestroyTime"]);
                data.BonusID = ToInt32(tbl.Rows[i]["BonusID"]);

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
                data.CreatePosOffset = ToInt32(tbl.Rows[i]["CreatePosOffset"]);

                list.Add(data);
            }
            return list;
        }
        private static object Item(DataTable tbl)
        {
            List<Item> list = new List<Item>();
            for (int i = 1; i < tbl.Rows.Count; i++)
            {
                var data = new Item();

                data.ID = ToInt32(tbl.Rows[i]["ID"]);
                data.Name = ToString(tbl.Rows[i]["Name"]);
                data.IconResourcesPath = ToString(tbl.Rows[i]["IconResourcesPath"]);
                data.Type = ToInt32(tbl.Rows[i]["Type"]);
                data.ReferenceID = ToInt32(tbl.Rows[i]["ReferenceID"]);
                data.MaxCount = ToInt32(tbl.Rows[i]["MaxCount"]);

                list.Add(data);
            }
            return list;
        }
        private static object Craft(DataTable tbl)
        {
            List<Craft> list = new List<Craft>();
            for (int i = 1; i < tbl.Rows.Count; i++)
            {
                var data = new Craft();

                data.ID = ToInt32(tbl.Rows[i]["ID"]);
                data.ItemID = ToInt32(tbl.Rows[i]["ItemID"]);
                data.Type = ToInt32(tbl.Rows[i]["Type"]);
                data.Cost1 = ToInt32(tbl.Rows[i]["Cost1"]);
                data.Cost1Count = ToInt32(tbl.Rows[i]["Cost1Count"]);
                data.Cost2 = ToInt32(tbl.Rows[i]["Cost2"]);
                data.Cost2Count = ToInt32(tbl.Rows[i]["Cost2Count"]);
                data.Cost3 = ToInt32(tbl.Rows[i]["Cost3"]);
                data.Cost3Count = ToInt32(tbl.Rows[i]["Cost3Count"]);
                data.Cost4 = ToInt32(tbl.Rows[i]["Cost4"]);
                data.Cost4Count = ToInt32(tbl.Rows[i]["Cost4Count"]);

                list.Add(data);
            }
            return list;
        }
        private static object Building(DataTable tbl)
        {
            List<Building> list = new List<Building>();
            for (int i = 1; i < tbl.Rows.Count; i++)
            {
                var data = new Building();

                data.ID = ToInt32(tbl.Rows[i]["ID"]);
                data.ItemID = ToInt32(tbl.Rows[i]["ItemID"]);
                data.ResourceName = ToString(tbl.Rows[i]["ResourceName"]);
                data.PosX = ToInt32(tbl.Rows[i]["PosX"]);
                data.PosY = ToInt32(tbl.Rows[i]["PosY"]);
                data.PosZ = ToInt32(tbl.Rows[i]["PosZ"]);
                data.Angle = ToInt32(tbl.Rows[i]["Angle"]);
                data.OffsetY = ToFloat(tbl.Rows[i]["OffsetY"]);
                data.DestroyTime = ToFloat(tbl.Rows[i]["DestroyTime"]);

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