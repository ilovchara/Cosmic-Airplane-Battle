using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    [Header(" == CANVAS ==")]
    [SerializeField] Canvas mainMenuCanvas; // 主菜单画布

    [Header(" == BUTTONS ==")]
    [SerializeField] Button buttonStart; // 开始按钮
    [SerializeField] Button buttonOptions; // 选项按钮
    [SerializeField] Button buttonQuit; // 退出按钮

    // 当脚本启用时调用
    void OnEnable()
    {
        // 将按钮与对应的点击事件绑定
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonStart.gameObject.name, OnButtonStartClicked);
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonOptions.gameObject.name, OnButtonOptionsClicked);
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonQuit.gameObject.name, OnButtonQuitClicked);
    }

    // 当脚本开始时调用
    void Start()
    {
        Time.timeScale = 1f; // 恢复时间流动
        GameManager.GameState = GameState.Playing; // 设置游戏状态为Playing
        UIInput.Instance.SelectUI(buttonStart); // 选择开始按钮
    }

    // 开始按钮点击事件
    void OnButtonStartClicked()
    {
        mainMenuCanvas.enabled = false; // 隐藏主菜单画布
        SceneLoader.Instance.LoadGameplayScene(); // 加载游戏场景
    }

    // 选项按钮点击事件
    void OnButtonOptionsClicked()
    {
        UIInput.Instance.SelectUI(buttonOptions); // 选择选项按钮
    }

    // 退出按钮点击事件
    void OnButtonQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 在编辑器中停止播放
#else
        Application.Quit(); // 退出游戏
#endif
    }
}
