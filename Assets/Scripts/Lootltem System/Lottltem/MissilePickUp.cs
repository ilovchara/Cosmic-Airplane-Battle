/// <summary>
/// 导弹拾取物：继承自 LootItem，拾取后使玩家导弹数量 +1。
/// </summary>
public class MissilePickUp : LootItem
{
    /// <summary>
    /// 重写拾取逻辑：调用玩家的 PickUpMissile 方法，然后播放拾取动画和音效。
    /// </summary>
    protected override void PickUp()
    {
        player.PickUpMissile(); // 增加玩家导弹数量
        base.PickUp(); // 播放动画与音效
    }
}
