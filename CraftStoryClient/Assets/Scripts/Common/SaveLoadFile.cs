using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

/// <summary>
/// ローカルのファイルセーブ、ロード
/// </summary>
public class SaveLoadFile : Single<SaveLoadFile>
{

	private FileStream fileStream;
	private BinaryFormatter bf;

	public async Task Save(object data, string path, Action callback = null)
	{
		await Task.Run(() =>
		{
			BinaryFormatter bf = new BinaryFormatter();
			using (FileStream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				bf.Serialize(stream, data);

                if (callback != null)
                {
					callback();
				}
			}
		});
	}

	/// <summary>
	/// ファイルのロード
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public object Load(string path)
	{
		if (!File.Exists(path))
			return null;

		bf = new BinaryFormatter();
		fileStream = null;

		try
		{
			fileStream = File.Open(path, FileMode.Open);
            if (fileStream.Length < 1)
            {
				Logger.Error("Load fial. Bad save data." + path);
				return null;
			}

			return bf.Deserialize(fileStream);
		}
		catch (FileNotFoundException e1)
		{
			Logger.Error("ファイルがありません");
			Logger.Error(e1.Message);
		}
		catch (IOException e2)
		{
			Logger.Error("ファイルオープンエラー");
			Logger.Error(e2.Message);
		}
		finally
		{
			if (fileStream != null)
				fileStream.Close();
		}

		Logger.Error("not find save file." + path);

		return null;
	}
}