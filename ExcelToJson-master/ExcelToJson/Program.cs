using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using Newtonsoft.Json;
using JsonConfigData;

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
                string outFileName = Path.GetFileName(files[i]);
                ExcelToJson(files[i], outFilePath + outFileName + ".json", indented);
            }
        }

        private static void ExcelToJson(string inFilePath, string outFilePath, Formatting indented)
        {
            var ds = ReadExcelData(inFilePath);

            foreach (DataTable tbl in ds.Tables)
            {
                List<TestData> testData = new List<TestData>();

                for (int i = 1; i < tbl.Rows.Count; i++)
                {
                    TestData d = new TestData();
                    d.Key1 = tbl.Rows[i][0].ToString();
                    d.Key2 = int.Parse(tbl.Rows[i][1].ToString());

                    testData.Add(d);
                }

                var json = JsonConvert.SerializeObject(testData, indented);
                File.WriteAllText(outFilePath, json);
            }
        }

        private static void Readjson(string outFilePath)
        {
            using (StreamReader r = new StreamReader(outFilePath))
            {
                string json = r.ReadToEnd();
                List<TestData> items = JsonConvert.DeserializeObject<List<TestData>>(json);
            }
        }

        static DataSet ReadExcelData(string path)
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
    }
}