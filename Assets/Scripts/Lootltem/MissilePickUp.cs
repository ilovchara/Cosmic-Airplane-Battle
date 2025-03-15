// 目的是让玩家导弹数量+1
public class MissilePickUp : LootItem
{
    protected override void PickUp()
    {
        // 获得导弹
        player.PickUpMissile();
        base.PickUp();
    }
}
