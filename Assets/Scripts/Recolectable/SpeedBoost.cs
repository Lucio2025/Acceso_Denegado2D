using UnityEngine;

public class SpeedBoost : PowerUp
{
    [SerializeField] private float speedMultiplier = 2f;

    protected override void ApplyEffect(Player player)
    {
        player.StartCoroutine(player.SpeedBoostRoutine(duration, speedMultiplier));
    }
}