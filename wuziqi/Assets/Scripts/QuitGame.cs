using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuitGame : MonoBehaviour
{
    public GameObject exitPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            exitPanel.SetActive(true);
        }
    }

    public void GameQuit(){
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
