using JsonConfigData;
using UnityEngine;

public class EntityBase : MonoBehaviour
{
    private int id;
    private Vector3Int pos;
    private DirectionType directionType;
    private float clickingTime;

    public int EntityID { get => id; set => id = value; }
    public Entity EConfig { get => ConfigMng.E.Entity[id]; }
    public Vector3Int Pos { get => pos; set => pos = value; }
    public Vector3Int Scale { get => new Vector3Int(EConfig.ScaleX, EConfig.ScaleY, EConfig.ScaleZ); }
    public EntityType Type { get => (EntityType)EConfig.Type; }
    public DirectionType DirectionType { get => directionType; set => directionType = value; }

    public GameObject obj { get; set; }
   
    public string ToStringData()
    {
        return string.Format("{0}^{1}^{2}^{3}", id, pos.x, pos.y, pos.z);
    }
    public virtual EntityBase Active(bool b, Transform parent = null)
    {
        if (b)
        {
            if (obj == null)
            {
                obj = CommonFunction.Instantiate(EConfig.Resources, WorldMng.E.MapCtl.CellParent, Pos);
            }
            else
                obj.gameObject.SetActive(b);
        }
        else
        {
            if (obj != null)
                obj.gameObject.SetActive(false);
        }

        return this;
    }
    public void ClearObj()
    {
        obj = null;
    }

    public virtual void OnClicking(float time)
    {
        if (clickingTime == 0)
            EffectMng.E.AddDestroyEffect(transform.position);

        clickingTime += time;

        if (clickingTime > EConfig.DestroyTime)
        {
            clickingTime = 0;

            ClickingEnd();

            EffectMng.E.RemoveDestroyEffect();
        }
    }
    public virtual void CancelClicking() { clickingTime = 0; }
    public virtual void ClickingEnd() { }
    public virtual void SetTouchType(DirectionType tType) { }
}

public enum EntityType
{
    None = 0,
    Block = 1,
    Block2 = 2, // 半透明ブロック
    Block99 = 99, // 壊れないブロック
    Resources = 100,
    Workbench = 1000,
    Kamado = 1001,
    Door = 2000,
    Torch = 2100,
    TransferGate = 9999,
    Obstacle = 10000,
}