using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main_Menu");
    }

    public void LoadShop()
    {
        SceneManager.LoadScene("Shop_Scene");
    }

    public void LoadStage1()
    {
        SceneManager.LoadScene("Stage_1");
    }

    public void LoadStage2()
    {
        SceneManager.LoadScene("Stage_2");
    }

    public void LoadStage3()
    {
        SceneManager.LoadScene("Stage_3");
    }

    public void LoadStage4()
    {
        SceneManager.LoadScene("Stage_4");
    }

    public void LoadStage5()
    {
        SceneManager.LoadScene("Stage_5");
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료 요청됨");
        Application.Quit();  
    }
}