using UnityEngine;

public class Invisibility : PowerUp
{
    protected override void ApplyEffect(Player player)
    {
        player.StartCoroutine(player.InvisibilityRoutine(duration));
    }
}