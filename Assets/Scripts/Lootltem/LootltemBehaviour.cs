using UnityEngine;

/// <summary>
/// 动画状态机行为：当拾取动画结束后，自动禁用掉落物对象。
/// 用于在动画结束时隐藏对象，通常搭配对象池机制使用。
/// </summary>
public class LootItemBehaviour : StateMachineBehaviour
{
    /// <summary>
    /// 状态退出时触发（如拾取动画播放完毕）。
    /// </summary>
    /// <param name="animator">动画器。</param>
    /// <param name="stateInfo">当前状态信息。</param>
    /// <param name="layerIndex">状态层索引。</param>
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.SetActive(false); // 禁用对象（适用于对象池回收）
    }
}
