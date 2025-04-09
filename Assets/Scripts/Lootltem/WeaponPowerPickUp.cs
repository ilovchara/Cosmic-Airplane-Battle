using UnityEngine;

/// <summary>
/// 武器能量拾取物：继承自 LootItem，根据玩家的武器能量状态决定不同的拾取效果。
/// 如果武器能量已满，则增加分数；否则，提升武器能量。
/// </summary>
public class WeaponPowerPickUp : LootItem
{
    [SerializeField] AudioData fullPowerPickUpSFX; // 满能量时的拾取音效
    [SerializeField] int fullPowerScoreBonus = 200; // 满能量时的分数加成

    /// <summary>
    /// 重写拾取逻辑：根据玩家的武器能量状态，选择不同的效果。
    /// </summary>
    protected override void PickUp()
    {
        if (player.IsFullPower) // 如果玩家的武器能量已满
        {
            pickUpSFX = fullPowerPickUpSFX; // 设置满能量拾取音效
            lootMessage.text = $"SCORE + {fullPowerScoreBonus}"; // 显示分数加成提示
            ScoreManager.Instance.AddScore(fullPowerScoreBonus); // 增加分数
        }
        else // 如果武器能量未满
        {
            pickUpSFX = defaultPickUpSFX; // 设置默认拾取音效
            lootMessage.text = "POWER UP!"; // 显示能量提升提示
            player.PowerUp(); // 提升武器能量
        }

        base.PickUp(); // 调用基类的拾取方法，播放动画与音效
    }
}
