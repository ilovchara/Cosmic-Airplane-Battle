using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 掉落物基类：处理自动追踪玩家、拾取动画与音效播放。
/// 可被血量恢复道具、能量球等继承扩展。
/// </summary>
public class LootItem : MonoBehaviour
{
    [SerializeField] float minSpeed = 5f; // 最小移动速度
    [SerializeField] float maxSpeed = 15f; // 最大移动速度
    [SerializeField] protected AudioData defaultPickUpSFX; // 默认拾取音效
    [SerializeField] protected AudioData fullHealthPickupSFX; // 满血时拾取的音效（子类可用）

    int pickUpStateID = Animator.StringToHash("PickUp"); // 拾取动画的参数 ID

    protected AudioData pickUpSFX; // 当前实际播放的拾取音效
    Animator animator; // 动画控制器
    protected Player player; // 玩家引用
    protected Text lootMessage; // 掉落提示文本（可选）

    /// <summary>
    /// 初始化引用，获取动画器、玩家、拾取音效等。
    /// </summary>
    void Awake()
    {
        animator = GetComponent<Animator>();
        player = FindFirstObjectByType<Player>();
        lootMessage = GetComponentInChildren<Text>(true);
        pickUpSFX = defaultPickUpSFX;
    }

    /// <summary>
    /// 激活时开始自动移动。
    /// </summary>
    void OnEnable()
    {
        StartCoroutine(MoveCoroutine());
    }

    /// <summary>
    /// 触发器检测到玩家则执行拾取逻辑。
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        PickUp();
    }

    /// <summary>
    /// 拾取行为：停止移动、播放动画和音效。
    /// 可被子类重写实现自定义拾取逻辑。
    /// </summary>
    protected virtual void PickUp()
    {
        StopAllCoroutines();
        animator.Play(pickUpStateID);
        AudioManager.Instance.PlayRandomSFX(pickUpSFX);
    }

    /// <summary>
    /// 掉落物自动朝玩家方向移动的协程。
    /// </summary>
    IEnumerator MoveCoroutine()
    {
        float speed = Random.Range(minSpeed, maxSpeed);
        Vector3 direction = Vector3.left;

        while (true)
        {
            if (player.isActiveAndEnabled)
            {
                direction = (player.transform.position - transform.position).normalized;
            }

            transform.Translate(direction * speed * Time.deltaTime);
            yield return null;
        }
    }
}
