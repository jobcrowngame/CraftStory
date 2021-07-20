using UnityEngine;

public class CharacterCtl
{
    private PlayerEntity player;

    public void CreateCharacter()
    {
        player = PlayerCtl.E.AddPlayerEntity();
    }
}