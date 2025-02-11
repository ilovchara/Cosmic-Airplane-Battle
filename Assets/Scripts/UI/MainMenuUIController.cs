using System;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    [SerializeField] Button buttonStartGame;
 
    void OnEnable()
    {
        buttonStartGame.onClick.AddListener(OnStartGameButtononClick);     
    }

    void OnDisable()
    {
        buttonStartGame.onClick.RemoveAllListeners();
    }

    void Start()
    {
        Time.timeScale = 1f;
    }


    private void OnStartGameButtononClick()
    {
        SceneLoader.Instance.LoadGameplayScene();
    }
}
