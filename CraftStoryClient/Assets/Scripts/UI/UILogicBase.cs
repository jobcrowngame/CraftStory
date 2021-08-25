/// <summary>
/// UILogic ベース
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="K"></typeparam>
public class UILogicBase<T, K> where T : class, new()
{
    private static T entity;
    protected K ui;

    public static T E
    {
        get
        {
            if (entity == null)
                entity = new T();

            return entity;
        }
    }

    public K UI { get => ui; }

    public virtual void Init(K ui)
    {
        this.ui = ui;
    }
}