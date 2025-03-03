using System;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Player Input")]
public class PlayerInput :
    ScriptableObject,
    InputActions.IGameplayActions,
    InputActions.IPauseMenuActions,
    InputActions.IGameOverScreenActions
{
    InputActions inputActions;

    // 事件定义，供外部订阅
    public event UnityAction<Vector2> onMove = delegate { };
    public event UnityAction onStopMove = delegate { };
    public event UnityAction onFire = delegate { };
    public event UnityAction onStopFire = delegate { };
    public event UnityAction onDodge = delegate { };
    public event UnityAction onOverdrive = delegate { };
    public event UnityAction onPause = delegate { };
    public event UnityAction onUnpause = delegate { };
    public event UnityAction onLaunchMissile = delegate { };
    public event UnityAction onConfirmGameOver = delegate { };

    #region Unity Lifecycle Methods

    /// <summary>
    /// 当脚本激活时，初始化输入系统并绑定回调方法。
    /// </summary>
    private void OnEnable()
    {
        inputActions = new InputActions();
        // 绑定回调事件到当前类的方法
        inputActions.Gameplay.SetCallbacks(this);
        inputActions.PauseMenu.SetCallbacks(this);
        inputActions.GameOverScreen.SetCallbacks(this);
    }

    /// <summary>
    /// 当脚本禁用时，禁用所有输入。
    /// </summary>
    private void OnDisable()
    {
        DisableAllInputs();
    }

    #endregion

    #region Input Control

    /// <summary>
    /// 切换输入控制映射，启用对应的Action Map，并根据是否为UI输入控制鼠标显示与锁定状态。
    /// </summary>
    /// <param name="actionMap">需要启用的输入动作映射</param>
    /// <param name="isUIInput">是否为UI输入</param>
    void SwitchActionMap(InputActionMap actionMap, bool isUIInput)
    {
        inputActions.Disable();
        actionMap.Enable();

        if (isUIInput)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    /// <summary>
    /// 切换到动态更新模式，输入事件会在每帧中被处理。
    /// </summary>
    public void SwitchToDynamicUpdateMode() => InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;

    /// <summary>
    /// 切换到固定更新模式，输入事件会在固定时间步长内处理。
    /// </summary>
    public void SwitchToFixedUpdateMode() => InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;

    /// <summary>
    /// 禁用所有输入。
    /// </summary>
    public void DisableAllInputs() => inputActions.Disable();

    /// <summary>
    /// 启用Gameplay输入。
    /// </summary>
    public void EnableGameplayInput() => SwitchActionMap(inputActions.Gameplay, false);

    /// <summary>
    /// 启用PauseMenu输入。
    /// </summary>
    public void EnablePauseMenuInput() => SwitchActionMap(inputActions.PauseMenu, true);

    /// <summary>
    /// 启用GameOverScreen输入。
    /// </summary>
    public void EnableGameOverScreenInput() => SwitchActionMap(inputActions.GameOverScreen, false);

    #endregion

    #region Input Callbacks

    /// <summary>
    /// 处理移动输入事件。当移动输入被执行时触发 onMove 事件，取消时触发 onStopMove 事件。
    /// </summary>
    /// <param name="context">输入的回调上下文。</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onMove.Invoke(context.ReadValue<Vector2>());
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            onStopMove.Invoke();
        }
    }

    /// <summary>
    /// 处理开火输入事件。按下时触发 onFire，松开时触发 onStopFire。
    /// </summary>
    /// <param name="context">输入的回调上下文。</param>
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onFire.Invoke();
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            onStopFire.Invoke();
        }
    }

    /// <summary>
    /// 处理闪避输入事件。闪避动作执行时触发 onDodge 事件。
    /// </summary>
    /// <param name="context">输入的回调上下文。</param>
    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onDodge.Invoke();
        }
    }

    /// <summary>
    /// 处理过载输入事件。过载动作执行时触发 onOverdrive 事件。
    /// </summary>
    /// <param name="context">输入的回调上下文。</param>
    public void OnOverdrive(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onOverdrive.Invoke();
        }
    }

    /// <summary>
    /// 处理暂停输入事件。当按下暂停键时触发 onPause 事件。
    /// </summary>
    /// <param name="context">输入的回调上下文。</param>
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onPause.Invoke();
        }
    }

    /// <summary>
    /// 处理取消暂停输入事件。当取消暂停时触发 onUnpause 事件。
    /// </summary>
    /// <param name="context">输入的回调上下文。</param>
    public void OnUnpause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onUnpause.Invoke();
        }
    }

    /// <summary>
    /// 处理发射导弹输入事件。当执行发射时触发 onLaunchMissile 事件。
    /// </summary>
    /// <param name="context">输入的回调上下文。</param>
    public void OnLaunchMissile(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onLaunchMissile.Invoke();
        }
    }

    /// <summary>
    /// 处理确认游戏结束输入事件。按下确认键时触发 onConfirmGameOver 事件。
    /// </summary>
    /// <param name="context">输入的回调上下文。</param>
    public void OnConfirmGameOver(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onConfirmGameOver.Invoke();
        }
    }

    #endregion
}
