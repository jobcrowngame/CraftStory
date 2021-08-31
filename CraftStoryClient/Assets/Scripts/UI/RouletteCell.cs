using UnityEngine.UI;

public class RouletteUICell : UIBase
{
    Text Text { get => FindChiled<Text>("Text"); }
    private int index;
    private int type;

    /// <summary>
    /// 内容をセット
    /// </summary>
    /// <param name="index">インデックス</param>
    /// <param name="type">結果タイプ　1.あたり 0.はずれ</param>
    public void Set(int index, int type, float angle)
    {
        this.index = index;
        this.type = type;

        Text.text = type == 1 ? "あたり" : "はずれ";
        transform.eulerAngles = new UnityEngine.Vector3(0, 0, angle * index);
    }
}
