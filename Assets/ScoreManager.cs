using UnityEngine;
using UnityEngine.TextCore.Text;

public class ScoreManager : PersistentSingleton<ScoreManager>
{
    int score;

    public void ResetScore()
    {
        score = 0;
        ScoreDisplay.UpdateText(score);
    }

    public void AddScore(int scorePoint)
    {
        score += scorePoint;
        ScoreDisplay.UpdateText(score);
    }

}
