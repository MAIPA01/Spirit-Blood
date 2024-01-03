using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject deathScreen;
    [SerializeField] TextMeshProUGUI scoreText;
    [HideInInspector] public float score = 0;
    public void DeadScreen()
    {
        deathScreen.SetActive(true);
    }

    public void MenuBtn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void RestartBtn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void Start()
    {
        if (scoreText == null)
        {
            Debug.Log("Pls assign TextMeshPro to me :(");
        }
        else
        {
            scoreText.text = "Score\n" + score;
        }

    }

    public void UpdateScore(float scorePoints)
    {
        if(scoreText != null)
        {
            scoreText.text = "Score\n" + scorePoints;
            score = scorePoints;
        }
    }
}
