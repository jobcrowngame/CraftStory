using UnityEngine;

/// <summary>
/// キャラクタコンソール
/// </summary>
public class CharacterCtl
{
    private PlayerEntity player;

    public void CreateCharacter()
    {
        player = PlayerCtl.E.AddPlayerEntity();
    }
}