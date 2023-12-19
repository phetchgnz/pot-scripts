using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    [SerializeField] GetPlayerData getPlayerData;
    

    public void Playgame()
    {
        if (Time.timeScale == 0) Time.timeScale = 1;
        PlayerPrefs.DeleteAll();

        SceneController.instance.LoadSceneWButton("LoadingScenes");
    }

    public void MainMenu() {
        if (Time.timeScale == 0) Time.timeScale = 1;
        SceneController.instance.LoadScene("Menu");
    }

    public void Settings() {
        SceneController.instance.LoadSceneWButton("Settings");
    }
}
