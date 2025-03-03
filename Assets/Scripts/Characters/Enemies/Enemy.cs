using UnityEngine;

public class Enemy : Character
{
    [SerializeField] int deathEnergyBonus = 3;
    // 分数
    [SerializeField] int scorePoint = 100;

    public override void Die()
    {
        ScoreManager.Instance.AddScore(scorePoint);
        PlayerEnergy.Instance.Obtain(deathEnergyBonus);
        EnemyManager.Instance.RemoveFromList(gameObject);
        base.Die();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.Die();
            Die();
        }
    }


}
