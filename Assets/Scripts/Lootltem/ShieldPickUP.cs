using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPickup : LootItem
{
    [SerializeField] int fullHealthScoreBonus = 200;
    [SerializeField] float shieldBonus = 20f;

    protected override void PickUp()
    {
        if (player.IsFullHealth)
        {
            pickUpSFX = fullHealthPickupSFX;
            lootMessage.text = $"SCORE + {fullHealthScoreBonus}";
            ScoreManager.Instance.AddScore(fullHealthScoreBonus);
        }
        else
        {
            pickUpSFX = defaultPickUpSFX;
            lootMessage.text = $"SHIELD + {shieldBonus}";
            player.RestoreHealth(shieldBonus);
        }
        base.PickUp();
    }
}
