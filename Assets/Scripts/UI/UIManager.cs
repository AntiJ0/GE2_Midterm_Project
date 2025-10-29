using System.Collections;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TMP_Text bossMessageText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void ShowBossSpawnMessage()
    {
        if (bossMessageText == null)
        {
            Debug.LogWarning("bossMessageText is null!");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(ShowBossMessageRoutine());
    }

    IEnumerator ShowBossMessageRoutine()
    {
        bossMessageText.text = "거대 좀비가 스폰되었습니다!";
        bossMessageText.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(3f); 

        bossMessageText.gameObject.SetActive(false);
    }
}