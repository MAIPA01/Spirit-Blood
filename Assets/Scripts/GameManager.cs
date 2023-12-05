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
        // Dopoki nie usuniemy wszedzie zaleznosci od Time Scale
        Time.timeScale = 0;

        GameTimer.StopTime();

        if (score > PlayerPrefs.GetFloat("Highscore", 0.0f))
        {
            PlayerPrefs.SetFloat("Highscore", score);
            foreach (Transform t in deathScreen.GetComponentInChildren<Transform>())
            {
                if (t.name == "Highscore")
                {
                    t.gameObject.SetActive(true);
                    t.GetComponent<TextMeshProUGUI>().text = "New\nHighscore\n*" + score + "*";
                }
            }
        }
        else
        {
            foreach (Transform t in deathScreen.GetComponentInChildren<Transform>())
            {
                if (t.name == "Highscore")
                {
                    t.gameObject.SetActive(false);
                }
            }
        }

        deathScreen.SetActive(true);
    }

    public void MenuBtn()
    {
        // Dopoki nie usuniemy wszedzie zaleznosci od Time Scale
        Time.timeScale = 1;
        GameTimer.StartTime();
        SceneManager.LoadScene(0);
    }

    public void RestartBtn()
    {
        // Dopoki nie usuniemy wszedzie zaleznosci od Time Scale
        Time.timeScale = 1;
        GameTimer.StartTime();
        SceneManager.LoadScene(1);
    }

    public void Start()
    {
        scoreText.text = "Score\n" + score;
    }

    public void UpdateScore(float scorePoints)
    {
        scoreText.text = "Score\n" + scorePoints;
        score = scorePoints;
    }
}
