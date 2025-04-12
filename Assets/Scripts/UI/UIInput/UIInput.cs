using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class UIInput : Singleton<UIInput>
{
    [SerializeField] PlayerInput playerInput; // 玩家输入组件

    InputSystemUIInputModule UIInputModule; // 用于UI的输入模块

    // 在脚本启用时调用，初始化UI输入模块
    protected override void Awake()
    {
        base.Awake();
        UIInputModule = GetComponent<InputSystemUIInputModule>(); // 获取UI输入模块组件
        UIInputModule.enabled = false; // 默认禁用UI输入模块
    }

    // 选择并聚焦指定的UI元素
    public void SelectUI(Selectable UIObject)
    {
        UIObject.Select(); // 选择该UI对象
        UIObject.OnSelect(null); // 调用UI对象的选择事件
        UIInputModule.enabled = true; // 启用UI输入模块，允许用户与UI交互
    }

    // 禁用所有UI输入
    public void DisableAllUIInputs()
    {
        playerInput.DisableAllInputs(); // 禁用玩家输入
        UIInputModule.enabled = false; // 禁用UI输入模块
    }
}
