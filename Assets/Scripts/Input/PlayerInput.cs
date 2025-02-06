using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
// 这个类是输入效果，供别的类继承使用这些方法 - 使用的方式是订阅委托
[CreateAssetMenu(menuName = "Player Input")]
public class PlayerInput : ScriptableObject, InputActions.IGameplayActions
{
    InputActions inputActions;
    // 点击后触发的事件
    public event UnityAction<Vector2> onMove = delegate { };

    public event UnityAction onStopMove = delegate { };

    public event UnityAction onFire = delegate { };

    public event UnityAction onStopFire = delegate { };

    public event UnityAction onDodge = delegate { };

    public event UnityAction onOverdrive = delegate { };

    #region Unity Lifecycle Methods

    private void OnEnable()
    {
        inputActions = new InputActions();
        // 将回调事件绑定到当前类的实现方法上。
        inputActions.Gameplay.SetCallbacks(this);
    }

    private void OnDisable()
    {
        DisableAllInputs();
    }

    #endregion

    #region Input Control

    /// <summary>
    /// 禁用所有输入。
    /// </summary>
    public void DisableAllInputs()
    {
        inputActions.Gameplay.Disable();
    }

    /// <summary>
    /// 启用游戏输入并隐藏鼠标。
    /// </summary>
    public void EnableGameplayInput()
    {
        inputActions.Gameplay.Enable();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    #endregion

    #region Input Callbacks

    /// <summary>
    /// 处理移动输入。当输入发生时触发 onMove 或 onStopMove 事件。
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
    /// 处理开火输入。当按下或松开开火键时分别触发 onFire 或 onStopFire 事件。
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
    /// 处理闪避输入。当执行闪避动作时触发 onDodge 事件。
    /// </summary>
    /// <param name="context">输入的回调上下文。</param>
    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onDodge.Invoke();
        }
    }

    public void OnOverdrive(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            onOverdrive.Invoke();
        }
    }

    #endregion
}
