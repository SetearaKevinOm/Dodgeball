using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public void StartGameButton()
    {
        SceneManager.LoadScene("IPScene", LoadSceneMode.Single);
    }

    public void ExitGameButton()
    {
        Application.Quit();
    }
}
