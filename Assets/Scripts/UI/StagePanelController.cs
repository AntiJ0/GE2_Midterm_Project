using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StagePanelController : MonoBehaviour
{
    [Header("�������� ��ư��")]
    public Button[] stageButtons;

    [Header("�ڹ��� �̹��� ������ (�ϳ��� ���)")]
    public GameObject lockPrefab;

    private List<GameObject> lockObjects = new List<GameObject>();

    private void Start()
    {
        CreateLockImages();
        UpdateStageButtons();
    }

    private void OnEnable()
    {
        if (lockObjects == null || lockObjects.Count == 0)
            CreateLockImages();

        UpdateStageButtons();
    }

    private void CreateLockImages()
    {
        foreach (var obj in lockObjects)
        {
            if (obj != null) Destroy(obj);
        }
        lockObjects.Clear();

        if (stageButtons == null || stageButtons.Length == 0)
        {
            Debug.LogError("[StagePanelController] stageButtons�� ��� �ֽ��ϴ�!");
            return;
        }

        if (lockPrefab == null)
        {
            Debug.LogError("[StagePanelController] lockPrefab�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        for (int i = 0; i < stageButtons.Length; i++)
        {
            Button btn = stageButtons[i];
            if (btn == null)
            {
                lockObjects.Add(null);
                continue;
            }

            GameObject lockObj = Instantiate(lockPrefab, btn.transform);
            lockObj.transform.SetAsLastSibling();

            RectTransform rect = lockObj.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;

                float buttonSize = Mathf.Min(btn.GetComponent<RectTransform>().rect.width,
                                             btn.GetComponent<RectTransform>().rect.height);
                rect.sizeDelta = new Vector2(buttonSize * 0.6f, buttonSize * 0.6f);

                rect.localRotation = Quaternion.identity;

                rect.localScale = Vector3.one;
            }

            lockObjects.Add(lockObj);
        }
    }

    public void UpdateStageButtons()
    {
        if (StageManager.Instance == null)
        {
            Debug.LogError("[StagePanelController] StageManager.Instance�� �����ϴ�!");
            return;
        }

        int highest = StageManager.Instance.highestClearedStage;

        for (int i = 0; i < stageButtons.Length; i++)
        {
            int stageNum = i + 1;
            bool canPlay = (stageNum <= highest + 1);

            stageButtons[i].interactable = canPlay;
            if (i < lockObjects.Count && lockObjects[i] != null)
                lockObjects[i].SetActive(!canPlay);
        }
    }

    public void OnStageButtonClicked(int stageNumber)
    {
        if (StageManager.Instance == null) return;

        if (stageNumber <= StageManager.Instance.highestClearedStage + 1)
        {
            StageManager.Instance.LoadStage(stageNumber);
        }
        else
        {
            Debug.Log($"�������� {stageNumber}�� ��� �ֽ��ϴ�.");
        }
    }
}