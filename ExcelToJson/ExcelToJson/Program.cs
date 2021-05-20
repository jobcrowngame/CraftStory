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
                ExcelToJson(files[i], outFilePath + outFileName + ".json", indented);
            }
        }
        private static void ExcelToJson(string inFilePath, string outFilePath, Formatting indented)
        {
            var ds = ReadExcelData(inFilePath);
            if (ds == null)
                return;

            foreach (DataTable tbl in ds.Tables)
            {
                //var json = JsonConvert.SerializeObject(tbl, indented);
                //File.WriteAllText(outFilePath, json);

                var list = ToBlock(tbl);
                var json = JsonConvert.SerializeObject(list, indented);
                File.WriteAllText(outFilePath, json);
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

        private static List<Block> ToBlock(DataTable dt)
        {
            List<Block> blockList = new List<Block>();
            string[] columNames = new string[dt.Columns.Count];

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                columNames[i] = dt.Rows[0].ItemArray[i].ToString();
            }

            for (int i = 1; i < dt.Rows.Count; i++)
            {
                var data = new Block();
                data.ID = Convert.ToInt32(dt.Rows[i].ItemArray[0]);
                data.Name = Convert.ToString(dt.Rows[i].ItemArray[1]);
                data.ResourcesName = Convert.ToString(dt.Rows[i].ItemArray[2]);
                data.Type = Convert.ToInt32(dt.Rows[i].ItemArray[3]);
                data.DestroyTime = Convert.ToInt32(dt.Rows[i].ItemArray[4]);

                blockList.Add(data);
            }

            return blockList;
        }
    }
}