using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] PlayerInput input; // 玩家输入
    [SerializeField] Canvas HUDCanvas; // 游戏HUD画布
    [SerializeField] AudioData confirmGameOverSound; // 游戏结束确认音效

    int exitStateID = Animator.StringToHash("GameOverScreenExit"); // 游戏结束界面退出的动画ID
    Canvas canvas; // 当前GameOver界面画布
    Animator animator; // 动画控制器

    // 初始化
    void Awake()
    {
        canvas = GetComponent<Canvas>(); // 获取Canvas组件
        animator = GetComponent<Animator>(); // 获取Animator组件

        canvas.enabled = false; // 初始时禁用GameOver界面
        animator.enabled = false; // 禁用动画控制器
    }

    // 脚本禁用时解绑事件
    void OnDisable()
    {
        GameManager.onGameOver -= OnGameOver; // 解绑游戏结束事件
        input.onConfirmGameOver -= OnConfirmGameOver; // 解绑确认游戏结束事件
    }

    // 脚本启用时绑定事件
    void OnEnable()
    {
        GameManager.onGameOver += OnGameOver; // 绑定游戏结束事件
        input.onConfirmGameOver += OnConfirmGameOver; // 绑定确认游戏结束事件
    }

    // 游戏结束事件处理
    void OnGameOver()
    {
        HUDCanvas.enabled = false; // 隐藏HUD画布
        canvas.enabled = true; // 显示GameOver画布
        animator.enabled = true; // 启用动画控制器
        input.DisableAllInputs(); // 禁用所有输入
    }

    // 确认游戏结束事件处理
    void OnConfirmGameOver()
    {
        AudioManager.Instance.PlaySFX(confirmGameOverSound); // 播放游戏结束确认音效
        input.DisableAllInputs(); // 禁用所有输入
        animator.Play(exitStateID); // 播放退出动画
        SceneLoader.Instance.LoadScoringScene();  // TODO - 加载积分场景
    }

    // 启用游戏结束画面的输入
    void EnableGameOverScreenInput()
    {
        input.EnableGameOverScreenInput(); // 启用游戏结束画面的输入
    }
    
}
