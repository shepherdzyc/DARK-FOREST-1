using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegisterView : MonoBehaviour
{
    public InputField playerNameInput;

    public Button startButton;

    private void Start()
    {
        //添加InputField的监听器
        playerNameInput.onEndEdit.AddListener(onEndEdit);
    }

    private void onEndEdit(string playerName)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartGame(playerName);
        }
    }

    private void OnStartGameButtonClick()
    {
        string playerName = playerNameInput.text;
        StartGame(playerName);
    }

    private void StartGame(string playerName)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
