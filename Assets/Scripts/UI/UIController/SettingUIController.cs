using UnityEngine;
using UnityEngine.UI;

public class SettingUIController : MonoBehaviour
{
    [SerializeField] Canvas settingCanvas;  // 设置画布
    [SerializeField] Button exitButton;     // 退出按钮

    [Header("Main Menu")]
    [SerializeField]MainMenuUIController mainMenuUIController; // 引用主菜单控制器

    void OnEnable()
    {
        exitButton.onClick.AddListener(OnExitButtonClicked); // 注册退出按钮点击事件
    }

    void OnDisable()
    {
        exitButton.onClick.RemoveListener(OnExitButtonClicked); // 移除事件监听
    }

    // 显示设置菜单
    public void SettingUIOpen()
    {
        Show();
    }

    // 隐藏设置菜单
    void Hide()
    {
        settingCanvas.enabled = false;
    }

    // 显示设置菜单
    void Show()
    {
        settingCanvas.enabled = true;
    }

    // 退出按钮点击事件
    void OnExitButtonClicked()
    {
        Hide();  // 隐藏设置菜单
        mainMenuUIController.mainMenuCanvas.enabled = true; // 显示主菜单画布
    }
}
