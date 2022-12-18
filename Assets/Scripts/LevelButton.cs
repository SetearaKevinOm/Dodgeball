using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    public string currentLevel;

    public void SetSelectedScene()
    {
        LobbyManager.instance.selectedLevel = currentLevel;
        LobbyManager.instance.UpdateLevelText(currentLevel);
    }
}
