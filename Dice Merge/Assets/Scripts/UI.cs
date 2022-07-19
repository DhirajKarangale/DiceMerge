using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] Text txtScore;
    [SerializeField] Text txtScoreGameover;
    [SerializeField] Text txtHighScore;
    [SerializeField] GameObject objGameOver;
    private int currScore;

    private void Start()
    {
        currScore = 0;
        txtScore.gameObject.SetActive(true);
        objGameOver.SetActive(false);
        UpdateScore(0);
    }

    public void UpdateScore(int score)
    {
        currScore += score;
        txtScore.text = "Score : " + currScore;
    }

    public void GameOver()
    {
        txtScore.gameObject.SetActive(false);
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (highScore < currScore) highScore = currScore;
        PlayerPrefs.SetInt("HighScore", highScore);

        txtHighScore.text = "High Score : " + highScore;
        txtScoreGameover.text = "Score : " + currScore;
        objGameOver.SetActive(true);
    }
}
