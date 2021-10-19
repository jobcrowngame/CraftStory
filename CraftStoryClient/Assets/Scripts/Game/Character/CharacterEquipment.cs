using JsonConfigData;
using System.Collections.Generic;

public class CharacterEquipment
{
    public int HP { get => mHP; }
    private int mHP = 0;

    public int Damage { get => mDamage; }
    private int mDamage = 0;

    public int Defnse { get => mDefnse; }
    private int mDefnse = 0;


    Dictionary<int, Equipment> equiped = new Dictionary<int, Equipment>();

    public CharacterEquipment()
    {
        CalculationParameter();
    }

    /// <summary>
    /// 装備する
    /// </summary>
    /// <param name="id"></param>
    public void AddEquiptment(int id)
    {
        equiped[id] = ConfigMng.E.Equipment[id];

        CalculationParameter();
    }

    /// <summary>
    /// 装備消す
    /// </summary>
    /// <param name="id"></param>
    public void RemoveEquipment(int id)
    {
        equiped.Remove(id);

        CalculationParameter();
    }

    /// <summary>
    /// パラメータを計算
    /// </summary>
    private void CalculationParameter()
    {
        mHP = 0;
        mDamage = 0;
        mDefnse = 0;

        foreach (var item in equiped.Values)
        {
            if(item.HP > 0) mHP = item.HP;
            if (item.Damage > 0) mDamage = item.Damage;
            if (item.Defnse > 0) mDefnse = item.Defnse;
        }
    }
}