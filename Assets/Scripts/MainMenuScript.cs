using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public AudioSource mainMenuTheme;
    public Camera camera;

    public void OnAwake()
    {
        mainMenuTheme.Play();
    }
    public void StartGameButton()
    {
        camera.gameObject.SetActive(false);
        mainMenuTheme.Stop();
        SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
    }

    public void ExitGameButton()
    {
        Application.Quit();
    }

    public void RestartGameButton()
    {
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
    }
}
