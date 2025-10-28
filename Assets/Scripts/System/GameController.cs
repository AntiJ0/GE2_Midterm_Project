using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [Header("게임 시간 설정")]
    [Tooltip("스테이지 시작 시간 (예: 22 → 22시 00분부터 시작)")]
    [Range(0, 23)] public int startHour = 22;
    private int currentHour;
    private int currentMinute;
    private const int endHour = 6;

    [Tooltip("1초당 게임 내 시간 몇 분이 지나가는지")]
    public int minutesPerRealSecond = 5;

    [Serializable]
    public class TimedSpawnerGroup
    {
        [Header("시간 및 스포너 그룹 설정")]
        public int hour;
        public int minute;
        public List<ZombieSpawner> spawnersToActivate = new List<ZombieSpawner>();
    }
    public List<TimedSpawnerGroup> timedSpawnerGroups = new List<TimedSpawnerGroup>();

    private bool hordeTriggered = false;

    [Header("UI")]
    public TMP_Text timeText;
    public GameObject clearPanel;
    public TMP_Text clearGoldText;
    public GameObject pausePanel;
    public TMP_Text pauseGoldText;

    private bool isPaused = false;
    private bool isCleared = false;
    private PlayerController player;

    public bool BlockGameplayInput { get; private set; } = false; 
    private float inputUnscaledBlockRemain = 0f;                  

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();

        currentHour = startHour;
        currentMinute = 0;

        if (clearPanel != null) clearPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        HideCursor();

        StartCoroutine(TimeRoutine());
    }

    private IEnumerator TimeRoutine()
    {
        while (!isCleared)
        {
            yield return new WaitForSeconds(1f);
            AdvanceTime();
            UpdateTimeUI();
            CheckSpawnerActivation();
            CheckHordeEvent();
            CheckStageClear();
        }
    }

    private void AdvanceTime()
    {
        currentMinute += minutesPerRealSecond;
        if (currentMinute >= 60)
        {
            currentHour++;
            currentMinute -= 60;
            if (currentHour >= 24) currentHour -= 24;
        }
    }

    private void UpdateTimeUI()
    {
        if (timeText != null)
            timeText.text = $"{currentHour:00} : {currentMinute:00}";
    }

    private void CheckSpawnerActivation()
    {
        foreach (var group in timedSpawnerGroups)
        {
            if (group.hour == currentHour && group.minute == currentMinute)
            {
                foreach (var spawner in group.spawnersToActivate)
                {
                    if (spawner != null && !spawner.enabled)
                        spawner.enabled = true;
                }
            }
        }
    }

    private void CheckHordeEvent()
    {
        if (!hordeTriggered && currentHour == 4 && currentMinute == 0)
        {
            hordeTriggered = true;

            ZombieSpawner[] allSpawners = FindObjectsOfType<ZombieSpawner>();
            foreach (var spawner in allSpawners)
            {
                spawner.minSpawnInterval *= 0.4f;
                spawner.maxSpawnInterval *= 0.4f;
            }

            Debug.Log("무리 습격 발생! 모든 스포너의 스폰 속도 2.5배 증가");
        }
    }

    private void CheckStageClear()
    {
        if ((currentHour == endHour && currentMinute == 0) && !isCleared)
        {
            isCleared = true;
            Time.timeScale = 0f;

            BlockGameplayInput = true;
            ShowCursor();

            if (clearPanel != null)
            {
                clearPanel.SetActive(true);
                if (clearGoldText != null && player != null)
                    clearGoldText.text = $"획득 골드: {player.stageGold} G";
            }
            Debug.Log("스테이지 클리어!");
        }
    }

    private void Update()
    {
        HandlePauseInput();

        if (inputUnscaledBlockRemain > 0f)
        {
            inputUnscaledBlockRemain -= Time.unscaledDeltaTime;
            if (inputUnscaledBlockRemain <= 0f)
                BlockGameplayInput = isPaused || isCleared; 
        }
    }

    private void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isCleared)
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;

        BlockGameplayInput = true; 
        ShowCursor();

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            if (pauseGoldText != null && player != null)
                pauseGoldText.text = $"획득 골드: {player.stageGold} G";
        }
    }

    public void ResumeGame()
    {
        Input.ResetInputAxes();
        inputUnscaledBlockRemain = 0.1f;
        BlockGameplayInput = true;

        Time.timeScale = 1f;
        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        HideCursor(); 
    }

    public void OnReturnToMainMenu()
    {
        if (player != null)
        {
            player.totalGold -= player.stageGold;
            if (player.totalGold < 0) player.totalGold = 0;
            PlayerPrefs.SetInt("TotalGold", player.totalGold);
            PlayerPrefs.Save();
        }

        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main_Menu");
    }

    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}