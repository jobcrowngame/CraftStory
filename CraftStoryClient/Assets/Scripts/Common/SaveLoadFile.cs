using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class SaveLoadFile : Single<SaveLoadFile>
{

	private FileStream fileStream;
	private BinaryFormatter bf;

	public void Save(object data, string path)
	{
		bf = new BinaryFormatter();
		fileStream = null;

		try
		{
			fileStream = File.Create(path);
			bf.Serialize(fileStream, data);
		}
		catch (IOException e1)
		{
			Debug.LogError("ファイルオープンエラー");
			Debug.LogError(e1.Message);
		}
		finally
		{
			if (fileStream != null)
				fileStream.Close();
		}
	}

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
				Debug.LogError("Load fial. Bad save data." + path);
				return null;
			}

			return bf.Deserialize(fileStream);
		}
		catch (FileNotFoundException e1)
		{
			Debug.LogError("ファイルがありません");
			Debug.LogError(e1.Message);
		}
		catch (IOException e2)
		{
			Debug.LogError("ファイルオープンエラー");
			Debug.LogError(e2.Message);
		}
		finally
		{
			if (fileStream != null)
				fileStream.Close();
		}

		Debug.LogError("not find save file." + path);

		return null;
	}
}