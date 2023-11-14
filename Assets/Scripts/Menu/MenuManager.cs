using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [HideInInspector] public int scene_id = 0;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject levelScreen;
    [SerializeField] private GameObject settingsScreen;

    public void SetSceneId(int scene_id)
    {
        this.scene_id = scene_id;
    }

    public void StartBtnOnClick()
    {
        startScreen.SetActive(false);
        levelScreen.SetActive(true);
    }
    public void SettingsBtnOnClick()
    {
        startScreen.SetActive(false);
        settingsScreen.SetActive(true);
    }
    public void ExitBtnOnClick()
    {
        Application.Quit();
    }

    public void SelectLevelBtnOnClick()
    {
        SceneManager.LoadScene(scene_id);
    }

    public void BackToMenuBtn()
    {
        levelScreen.SetActive(false);
        settingsScreen.SetActive(false);
        startScreen.SetActive(true);
    }
}
