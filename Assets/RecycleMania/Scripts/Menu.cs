using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public void OnPlayBtn() {

        SceneManager.LoadScene("MainScene");

    }

    public void OnQuitBtn() {

        Application.Quit();

    }

}
