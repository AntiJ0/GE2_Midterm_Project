using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    [Header("스테이지 정보")]
    public int maxStageCount = 5;
    public int highestClearedStage = 0; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadStageData();
    }

    public void SaveStageData()
    {
        PlayerPrefs.SetInt("HighestClearedStage", highestClearedStage);
        PlayerPrefs.Save();
    }

    public void LoadStageData()
    {
        highestClearedStage = PlayerPrefs.GetInt("HighestClearedStage", 0);
    }

    public void ClearStage(int stageNumber)
    {
        if (stageNumber > highestClearedStage)
        {
            highestClearedStage = stageNumber;
            SaveStageData();
        }
    }

    public void LoadStage(int stageNumber)
    {
        string sceneName = "Stage" + stageNumber;
        SceneManager.LoadScene(sceneName);
    }
}