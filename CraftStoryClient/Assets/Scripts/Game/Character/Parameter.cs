using JsonConfigData;


public class Parameter
{
    public string Name { get => config.Name; }
    public int Level { get => config.Level; }
    public int MaxHP { get => config.HP + Equipment.HP + skillPar.HP; }
    public int Damage { get => FixDamage + PercentDamage; }
    public int FixDamage { get => config.Damage + Equipment.Damage + skillPar.FixDamage; }
    public int PercentDamage { get => (int)(FixDamage * (skillPar.PercentDamage) * 0.01f); }
    public int Defense { get => config.Defense + Equipment.Defnse + skillPar.Defense; }
    public int SecurityRange { get => config.SecurityRange; }
    public int CallForHelpRange { get => config.CallForHelpRange; }
    public string Skills { get => config.Skills; }
    public float DazeTime { get => config.DazeTime; }
    public float MoveSpeed { get => config.MoveSpeed; }

    public int PondId { get => config.PondId; }
    public int AddExp { get => config.AddExp; }


    public int CurHP { get; set; }

    private Character config { get => ConfigMng.E.Character[mCharacterId]; }
    private int mCharacterId;

    public CharacterEquipment Equipment;
    public ParameterSkill skillPar;

    public Parameter(int characterId)
    {
        mCharacterId = characterId;
        Equipment = new CharacterEquipment(this);
        skillPar = new ParameterSkill(this);
        CurHP = MaxHP;
    }

    public void Refresh()
    {
        CurHP = MaxHP;
    }
}