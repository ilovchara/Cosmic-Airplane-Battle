using UnityEngine;
using UnityEngine.UI;

public class GameplayerUIController : MonoBehaviour
{
    [Header(" ---- PLAYER INPUT ----")]
    [SerializeField] PlayerInput playerInput;
    [Header(" --- CANVAS ---")]
    [SerializeField] Canvas hUDCanvas;
    [SerializeField] Canvas menusCanvas;
    [Header(" --- PLAYER INPUT ---")]
    [SerializeField] Button resumeButton;
    [SerializeField] Button optionButton;
    [SerializeField] Button mainMenuButton;
    void OnEnable()
    {
        playerInput.onPause += Pause;
        playerInput.onUnpause += Unpause;

        resumeButton.onClick.AddListener(OnResumeButtonClick);
        optionButton.onClick.AddListener(OnOptionsButtonClick);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClick);
        
    }

    void OnDisable()
    {
        playerInput.onPause -= Pause;
        playerInput.onUnpause -= Unpause;

        resumeButton.onClick.RemoveAllListeners();
        optionButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.RemoveAllListeners();
        
    }

    void Pause()
    {
        Time.timeScale = 0f;
        hUDCanvas.enabled = false;
        menusCanvas.enabled = true;
        playerInput.EnablePauseMenuInput();
        playerInput.SwitchToDynamicUpdateMode();
    }

    void Unpause()
    {
        OnResumeButtonClick();
    }

    void OnResumeButtonClick()
    {
        Time.timeScale = 1f;
        hUDCanvas.enabled = true;
        menusCanvas.enabled = false;
        playerInput.EnableGameplayInput();
        playerInput.SwitchToFixedUpdateMode();
    }

    void OnOptionsButtonClick()
    {

    }

    void OnMainMenuButtonClick()
    {
        menusCanvas.enabled = false;
        SceneLoader.Instance.LoadMainMenuScene();
    }


}
