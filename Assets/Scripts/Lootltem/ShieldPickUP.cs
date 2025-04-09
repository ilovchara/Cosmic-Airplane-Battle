using UnityEngine;

/// <summary>
/// 盾牌拾取物：继承自 LootItem，根据玩家当前血量决定拾取效果。
/// 如果玩家血量满，增加分数；否则，恢复玩家一定的血量。
/// </summary>
public class ShieldPickup : LootItem
{
    [SerializeField] int fullHealthScoreBonus = 200; // 玩家血量满时获得的分数加成
    [SerializeField] float shieldBonus = 20f; // 玩家血量未满时恢复的血量

    /// <summary>
    /// 重写拾取逻辑：根据玩家血量决定不同的拾取效果。
    /// </summary>
    protected override void PickUp()
    {
        if (player.IsFullHealth) // 玩家血量满
        {
            pickUpSFX = fullHealthPickupSFX; // 设置满血拾取音效
            lootMessage.text = $"SCORE + {fullHealthScoreBonus}"; // 显示分数加成提示
            ScoreManager.Instance.AddScore(fullHealthScoreBonus); // 增加分数
        }
        else // 玩家血量未满
        {
            pickUpSFX = defaultPickUpSFX; // 设置默认拾取音效
            lootMessage.text = $"SHIELD + {shieldBonus}"; // 显示恢复血量提示
            player.RestoreHealth(shieldBonus); // 恢复血量
        }

        base.PickUp(); // 调用基类的拾取方法，播放动画与音效
    }
}

