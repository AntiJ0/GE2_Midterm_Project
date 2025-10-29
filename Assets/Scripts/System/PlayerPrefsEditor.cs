using UnityEngine;

public class PlayerPrefsEditor : MonoBehaviour
{
    [ContextMenu("Add 10000 Gold")]
    void AddGold()
    {
        int newGold = PlayerPrefs.GetInt("TotalGold", 0) + 10000;
        PlayerPrefs.SetInt("TotalGold", newGold);
        PlayerPrefs.Save();

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.totalGold = newGold;
            player.UpdateUI();
        }

        Debug.Log($"��尡 �߰��Ǿ����ϴ�. ���� �� ���: {newGold}");
    }

    [ContextMenu("Reset All PlayerPrefs")]
    void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.totalGold = 0;
            player.UpdateUI();
        }

        Debug.Log("��� PlayerPrefs �����Ͱ� �ʱ�ȭ�Ǿ����ϴ�.");
    }

    [ContextMenu("Set HighestClearedStage to 5")]
    void SetMaxClearedStage()
    {
        PlayerPrefs.SetInt("HighestClearedStage", 5);
        PlayerPrefs.Save();

        Debug.Log("Ŭ������ �ִ� ���������� 5�� �����Ǿ����ϴ�.");
    }
}