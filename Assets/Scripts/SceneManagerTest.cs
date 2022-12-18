using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerTest : NetworkBehaviour
{
    //public List<Scene> scenes;
    /*public UnityEditor.SceneAsset SceneAsset;

    private void OnValidate()
    {
        if (SceneAsset != null)
        {
            m_SceneName = SceneAsset.name;
        }
    }

    [SerializeField] private string m_SceneName;
   
    void OnGUI()
    {
        if (GUILayout.Button("Load Scene"))
        {
            //NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Additive);
            var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Additive);
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load {m_SceneName} " +
                                 $"with a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }
    }*/
    
}
