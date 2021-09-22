using UnityEngine;

public class AudioMng : MonoBehaviour
{
    public static AudioMng E
    {
        get => entity;
    }
    private static AudioMng entity;

    private const string AudioRoot = "Audio/";

    private AudioSource BGM { get => transform.GetChild(0).GetComponent<AudioSource>(); }
    private AudioSource SE { get => transform.GetChild(1).GetComponent<AudioSource>(); }

    public static void Init(Transform parent)
    {
        var prefab = Resources.Load("Prefabs/Game/Cor/AudoMng") as GameObject;
        var obj = GameObject.Instantiate(prefab, parent);
        entity = obj.GetComponent<AudioMng>();
        if (entity == null)
        {
            entity = obj.AddComponent<AudioMng>();
        }
    }

    /// <summary>
    /// BGM をプレイ
    /// </summary>
    /// <param name="clipName"></param>
    public void ShowBGM(string clipName)
    {
        BGM.clip = ReadAudioClip(clipName);
        BGM.Play();
    }
    /// <summary>
    /// SE をプレイ
    /// </summary>
    /// <param name="clipName"></param>
    public void ShowSE(string clipName)
    {
        SE.clip = ReadAudioClip(clipName);
        SE.Play();
    }

    /// <summary>
    /// クリップをロード
    /// </summary>
    /// <param name="clipName"></param>
    /// <returns>クリップ</returns>
    private AudioClip ReadAudioClip(string clipName)
    {
        var clip = Resources.Load<AudioClip>(AudioRoot + clipName);
        if (clip == null)
            Logger.Warning("Not find audio " + clipName);

        return clip;
    }
}
