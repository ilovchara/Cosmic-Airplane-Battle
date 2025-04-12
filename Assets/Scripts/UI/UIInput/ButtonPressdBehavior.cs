using System.Collections.Generic;
using UnityEngine;

public class ButtonPressedBehavior : StateMachineBehaviour
{
    // 用于存储按钮名称和对应的点击事件（功能）
    public static Dictionary<string, System.Action> buttonFunctionTable;

    // 初始化按钮功能表
    void Awake()
    {
        buttonFunctionTable = new Dictionary<string, System.Action>();
    }

    // 当动画状态进入时调用
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 禁用所有UI输入，防止按钮重复被点击
        UIInput.Instance.DisableAllUIInputs();
    }

    // 当动画状态退出时调用
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 从字典中查找按钮名称并执行对应的点击事件
        buttonFunctionTable[animator.gameObject.name].Invoke();
    }
}
