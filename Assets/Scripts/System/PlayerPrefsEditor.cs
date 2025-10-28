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
}