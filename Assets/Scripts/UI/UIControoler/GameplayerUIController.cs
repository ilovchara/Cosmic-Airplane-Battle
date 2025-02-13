using NUnit.Framework;
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

    [Header(" == AUDIO DATA == ")]
    [SerializeField] AudioData pauseSFX;
    [SerializeField] AudioData unpauseSFX;

    int ButtonPressedBehaviorID = Animator.StringToHash("Pressed");


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

        ButtonPressedBehavior.buttonFunctionTable.Clear();

    }

    void Pause()
    {

        hUDCanvas.enabled = false;
        menusCanvas.enabled = true;
        GameManager.GameState = GameState.Paused;
        TimeController.Instance.Pause();
        playerInput.EnablePauseMenuInput();
        playerInput.SwitchToDynamicUpdateMode();
        UIInput.Instance.SelectUI(resumeButton);
        AudioManager.Instance.PlaySFX(pauseSFX);
    }

    void OnResumeButtonClick()
    {


        hUDCanvas.enabled = true;
        menusCanvas.enabled = false;
        GameManager.GameState = GameState.Playing;
        playerInput.EnableGameplayInput();
        playerInput.SwitchToFixedUpdateMode();
        TimeController.Instance.Unpause();
    }

    void Unpause()
    {
        resumeButton.Select();
        resumeButton.animator.SetTrigger(ButtonPressedBehaviorID);
        AudioManager.Instance.PlaySFX(unpauseSFX);
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
