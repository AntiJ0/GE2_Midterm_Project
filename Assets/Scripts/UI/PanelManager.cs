using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject panel;

    public void OpenPanel()
    {
        panel.SetActive(true);
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
    }
}