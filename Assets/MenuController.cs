using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuController : MonoBehaviour
{
    public void OnClickRestartButton()
    {
        SceneManager.LoadScene("Main Menu");
    }
    public void OnClickQuitButton()
    {
        Application.Quit();
    }
    public void OnClickStartButton()
    {
        SceneManager.LoadScene("Level1.1");
    }
    public void OnClickRulesButton()
    {
        SceneManager.LoadScene("Rules");
    }
    public void OnClickBackButotn()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
