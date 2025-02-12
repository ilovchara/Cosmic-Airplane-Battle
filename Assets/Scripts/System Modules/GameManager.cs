using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
    public static GameState GameState { get => Instance.gameState; set => Instance.gameState = value; }

    [SerializeField] GameState gameState = GameState.Playing;
}

public enum GameState
{
    Playing,
    Paused,
    GameOver,
}