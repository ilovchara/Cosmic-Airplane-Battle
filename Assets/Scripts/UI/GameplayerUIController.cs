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
    [SerializeField] Button optionsButton;
    [SerializeField] Button mainMenuButton;
    void OnEnable()
    {
        playerInput.onPause += Pause;
        playerInput.onUnpause += Unpause;


        ButtonPressedBehavior.buttonFunctionTable.Add(resumeButton.gameObject.name, OnResumeButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(optionsButton.gameObject.name, OnOptionsButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(mainMenuButton.gameObject.name, OnMainMenuButtonClick);

    }

    void OnDisable()
    {
        playerInput.onPause -= Pause;
        playerInput.onUnpause -= Unpause;

    }

    void Pause()
    {
        Time.timeScale = 0f;
        hUDCanvas.enabled = false;
        menusCanvas.enabled = true;
        playerInput.EnablePauseMenuInput();
        playerInput.SwitchToDynamicUpdateMode();
        UIInput.Instance.SelectUI(resumeButton);
    }

    void Unpause()
    {
        resumeButton.Select();
        resumeButton.animator.SetTrigger("Pressed");
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
        UIInput.Instance.SelectUI(optionsButton);
        playerInput.EnablePauseMenuInput();
    }

    void OnMainMenuButtonClick()
    {
        menusCanvas.enabled = false;
        SceneLoader.Instance.LoadMainMenuScene();
    }


}
