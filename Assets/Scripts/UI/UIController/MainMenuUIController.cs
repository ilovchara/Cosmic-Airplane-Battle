using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    [Header(" == CANVAS ==")]
    [SerializeField]public Canvas mainMenuCanvas; // 主菜单画布

    [Header(" == BUTTONS ==")]
    [SerializeField] Button buttonStart;   // 开始按钮
    [SerializeField] Button buttonOptions; // 选项按钮
    [SerializeField] Button buttonQuit;    // 退出按钮

    [Header(" == UI References ==")]
    [SerializeField] SettingUIController settingUIController; // 引用设置菜单控制器

    void OnEnable()
    {
        buttonStart.onClick.AddListener(OnButtonStartClicked);
        buttonOptions.onClick.AddListener(OnButtonOptionsClicked);
        buttonQuit.onClick.AddListener(OnButtonQuitClicked);
    }

    void OnDisable()
    {
        buttonStart.onClick.RemoveListener(OnButtonStartClicked);
        buttonOptions.onClick.RemoveListener(OnButtonOptionsClicked);
        buttonQuit.onClick.RemoveListener(OnButtonQuitClicked);
    }

    void Start()
    {
        Time.timeScale = 1f;
        GameManager.GameState = GameState.Playing;
        UIInput.Instance.SelectUI(buttonStart);
    }

    void OnButtonStartClicked()
    {
        mainMenuCanvas.enabled = false;
        SceneLoader.Instance.LoadGameplayScene();
    }

    // 调用设置菜单
    void OnButtonOptionsClicked()
    {
        mainMenuCanvas.enabled = false; // 隐藏主菜单
        settingUIController.SettingUIOpen(); // 打开设置菜单
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
