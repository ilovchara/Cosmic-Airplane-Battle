using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class GameplayerUIController : MonoBehaviour
{
    [Header(" ---- PLAYER INPUT ----")]
    [SerializeField] PlayerInput playerInput; // 玩家输入

    [Header(" --- CANVAS ---")]
    [SerializeField] Canvas hUDCanvas; // HUD画布
    [SerializeField] Canvas menusCanvas; // 菜单画布

    [Header(" --- PLAYER INPUT ---")]
    [SerializeField] Button resumeButton; // 恢复按钮
    [SerializeField] Button optionsButton; // 选项按钮
    [SerializeField] Button mainMenuButton; // 主菜单按钮

    [Header(" == AUDIO DATA == ")]
    [SerializeField] AudioData pauseSFX; // 暂停音效
    [SerializeField] AudioData unpauseSFX; // 恢复音效

    int ButtonPressedBehaviorID = Animator.StringToHash("Pressed"); // 按钮按下的动画ID

    // 当脚本启用时调用
    void OnEnable()
    {
        // 绑定暂停和恢复事件
        playerInput.onPause += Pause; 
        playerInput.onUnpause += Unpause;

        // 将按钮与对应的点击事件绑定
        ButtonPressedBehavior.buttonFunctionTable.Add(resumeButton.gameObject.name, OnResumeButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(optionsButton.gameObject.name, OnOptionsButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(mainMenuButton.gameObject.name, OnMainMenuButtonClick);
    }

    // 当脚本禁用时调用
    void OnDisable()
    {
        // 解绑暂停和恢复事件
        playerInput.onPause -= Pause; 
        playerInput.onUnpause -= Unpause;

        // 清空按钮功能表
        ButtonPressedBehavior.buttonFunctionTable.Clear(); 
    }

    // 暂停游戏
    void Pause()
    {
        hUDCanvas.enabled = false; // 隐藏HUD画布
        menusCanvas.enabled = true; // 显示菜单画布
        GameManager.GameState = GameState.Paused; // 设置游戏状态为暂停
        TimeController.Instance.Pause(); // 暂停时间
        playerInput.EnablePauseMenuInput(); // 启用暂停菜单输入
        playerInput.SwitchToDynamicUpdateMode(); // 切换到动态更新模式
        UIInput.Instance.SelectUI(resumeButton); // 选择恢复按钮
        AudioManager.Instance.PlaySFX(pauseSFX); // 播放暂停音效
    }

    // 恢复按钮点击事件
    void OnResumeButtonClick()
    {
        hUDCanvas.enabled = true; // 显示HUD画布
        menusCanvas.enabled = false; // 隐藏菜单画布
        GameManager.GameState = GameState.Playing; // 设置游戏状态为进行中
        playerInput.EnableGameplayInput(); // 启用游戏输入
        playerInput.SwitchToFixedUpdateMode(); // 切换到固定更新模式
        TimeController.Instance.Unpause(); // 恢复时间
    }

    // 恢复游戏
    void Unpause()
    {
        resumeButton.Select(); // 选择恢复按钮
        resumeButton.animator.SetTrigger(ButtonPressedBehaviorID); // 触发按钮按下动画
        AudioManager.Instance.PlaySFX(unpauseSFX); // 播放恢复音效
    }

    // 选项按钮点击事件
    void OnOptionsButtonClick()
    {
        UIInput.Instance.SelectUI(optionsButton); // 选择选项按钮
        playerInput.EnablePauseMenuInput(); // 启用暂停菜单输入
    }

    // 主菜单按钮点击事件
    void OnMainMenuButtonClick()
    {
        menusCanvas.enabled = false; // 隐藏菜单画布
        SceneLoader.Instance.LoadMainMenuScene(); // 加载主菜单场景
    }
}
