using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    [Header(" == CANVAS ==")]
    [SerializeField] Canvas mainMenuCanvas; // 主菜单画布

    [Header(" == BUTTONS ==")]
    [SerializeField] Button buttonStart;   // 开始按钮
    [SerializeField] Button buttonOptions; // 选项按钮
    [SerializeField] Button buttonQuit;    // 退出按钮

    void OnEnable()
    {
        // 使用 Unity 的 onClick 事件监听
        buttonStart.onClick.AddListener(OnButtonStartClicked);
        buttonOptions.onClick.AddListener(OnButtonOptionsClicked);
        buttonQuit.onClick.AddListener(OnButtonQuitClicked);
    }

    void OnDisable()
    {
        // 移除监听，避免重复注册
        buttonStart.onClick.RemoveListener(OnButtonStartClicked);
        buttonOptions.onClick.RemoveListener(OnButtonOptionsClicked);
        buttonQuit.onClick.RemoveListener(OnButtonQuitClicked);
    }

    void Start()
    {
        Time.timeScale = 1f; // 恢复时间流动
        GameManager.GameState = GameState.Playing; // 设置游戏状态为Playing
        UIInput.Instance.SelectUI(buttonStart); // 选择开始按钮
    }

    // 按钮点击事件处理方法
    void OnButtonStartClicked()
    {
        mainMenuCanvas.enabled = false;
        SceneLoader.Instance.LoadGameplayScene();
    }

    void OnButtonOptionsClicked()
    {
        UIInput.Instance.SelectUI(buttonOptions);
    }

    void OnButtonQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
