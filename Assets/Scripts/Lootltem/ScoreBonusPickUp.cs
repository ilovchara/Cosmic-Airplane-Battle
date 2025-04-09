using UnityEngine;

/// <summary>
/// 分数加成拾取物：继承自 LootItem，拾取后增加玩家得分。
/// </summary>
public class ScoreBonusPickUp : LootItem
{
    [SerializeField] int scoreBonus; // 拾取后给予的分数加成

    /// <summary>
    /// 重写拾取逻辑：增加分数后播放拾取动画和音效。
    /// </summary>
    protected override void PickUp()
    {
        ScoreManager.Instance.AddScore(scoreBonus); // 增加分数
        base.PickUp(); // 播放动画与音效
    }
}
