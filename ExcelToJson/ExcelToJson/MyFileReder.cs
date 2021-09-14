using System.IO;

namespace ExcelToJson
{
    internal class MyFileReder
    {
        //const string root = "../Excel/Blueprint/";
        const string root = "D:/Project/CraftStory/Config/Excel/Blueprint/";
        public string Read(string path)
        {
            return File.ReadAllText(root + path);
        }
    }
}