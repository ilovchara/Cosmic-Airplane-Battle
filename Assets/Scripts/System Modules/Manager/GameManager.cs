using UnityEngine;

/// <summary>
/// 游戏管理器（单例模式）
/// 负责追踪当前游戏状态，并提供游戏结束事件广播。
/// </summary>
public class GameManager : PersistentSingleton<GameManager>
{
    /// <summary>
    /// 游戏结束事件，可由外部监听以触发相关逻辑（如显示UI、暂停游戏等）
    /// </summary>
    public static System.Action onGameOver;

    /// <summary>
    /// 当前游戏状态（通过静态属性访问内部实例的状态字段）
    /// </summary>
    public static GameState GameState
    {
        get => Instance.gameState;
        set => Instance.gameState = value;
    }

    [SerializeField]
    GameState gameState = GameState.Playing; // 默认游戏状态为进行中
}

/// <summary>
/// 游戏状态枚举
/// Playing：游戏进行中
/// Paused：游戏暂停
/// GameOver：游戏结束
/// Scoring：结算/计分中
/// </summary>
public enum GameState
{
    Playing,
    Paused,
    GameOver,
    Scoring
}
