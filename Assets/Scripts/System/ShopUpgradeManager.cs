using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUpgradeManager : MonoBehaviour
{
    [Header("�÷��̾� ���� (Stage������ ����)")]
    public PlayerController player;

    [Header("���� UI")]
    public TMP_Text totalGoldText;
    public GameObject notEnoughGoldText;

    [Header("���׷��̵� ��ư �� UI")]
    public Button moveSpeedButton;
    public Button attackButton;
    public Button hpButton;
    public Button reloadButton;

    [Header("���׷��̵� ǥ�� �ؽ�Ʈ")]
    public TMP_Text moveSpeedLevelText;
    public TMP_Text moveSpeedEffectText;
    public TMP_Text moveSpeedCostText;

    public TMP_Text attackLevelText;
    public TMP_Text attackEffectText;
    public TMP_Text attackCostText;

    public TMP_Text hpLevelText;
    public TMP_Text hpEffectText;
    public TMP_Text hpCostText;

    public TMP_Text reloadLevelText;
    public TMP_Text reloadEffectText;
    public TMP_Text reloadCostText;

    [Header("���׷��̵� ��� (1ȸ��)")]
    public int moveSpeedCost = 200;
    public int attackCost = 500;
    public int hpCost = 300;
    public int reloadCost = 100;

    private int moveSpeedLevel;
    private int attackLevel;
    private int hpLevel;
    private int reloadLevel;

    private const int MAX_LEVEL = 10;
    private bool isShowingGoldWarning = false;

    private Color normalButtonColor;
    private Color maxedButtonColor = new Color(0.6f, 0.6f, 0.6f);

    private int totalGoldLocal = 0;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        totalGoldLocal = PlayerPrefs.GetInt("TotalGold", 0);

        if (moveSpeedButton != null)
            normalButtonColor = moveSpeedButton.image.color;

        LoadUpgrades();
        UpdateUI();
    }

    private void Update()
    {
        int currentGold = (player != null) ? player.totalGold : PlayerPrefs.GetInt("TotalGold", 0);
        if (totalGoldText != null)
            totalGoldText.text = $"{currentGold} G";
    }

    private void LoadUpgrades()
    {
        moveSpeedLevel = PlayerPrefs.GetInt("MoveSpeedLevel", 0);
        attackLevel = PlayerPrefs.GetInt("AttackLevel", 0);
        hpLevel = PlayerPrefs.GetInt("HPLevel", 0);
        reloadLevel = PlayerPrefs.GetInt("ReloadLevel", 0);

        if (player != null)
            ApplyUpgradeEffects();
    }

    private void SaveUpgrades()
    {
        PlayerPrefs.SetInt("MoveSpeedLevel", moveSpeedLevel);
        PlayerPrefs.SetInt("AttackLevel", attackLevel);
        PlayerPrefs.SetInt("HPLevel", hpLevel);
        PlayerPrefs.SetInt("ReloadLevel", reloadLevel);
        PlayerPrefs.SetInt("TotalGold", player != null ? player.totalGold : totalGoldLocal);
        PlayerPrefs.Save();
    }

    private void ApplyUpgradeEffects()
    {
        if (player == null) return;

        player.moveSpeed = 5f * (1f + moveSpeedLevel * 0.025f);
        player.attackMultiplier = 1f + attackLevel * 0.1f;
        player.maxHP = 100f * (1f + hpLevel * 0.1f);
        player.reloadSpeedMultiplier = Mathf.Max(0.5f, 1f - 0.05f * reloadLevel);
    }

    private void UpdateUI()
    {
        int gold = player != null ? player.totalGold : totalGoldLocal;
        if (totalGoldText != null)
            totalGoldText.text = $"{gold} G";

        moveSpeedLevelText.text = $"+{moveSpeedLevel}";
        moveSpeedEffectText.text = $"�̵� �ӵ� +{moveSpeedLevel * 2.5f}%";
        moveSpeedCostText.text = moveSpeedLevel >= MAX_LEVEL ? "�ִ� ����" : $"{moveSpeedCost} G";

        attackLevelText.text = $"+{attackLevel}";
        attackEffectText.text = $"���ݷ� +{attackLevel * 10f}%";
        attackCostText.text = attackLevel >= MAX_LEVEL ? "�ִ� ����" : $"{attackCost} G";

        hpLevelText.text = $"+{hpLevel}";
        hpEffectText.text = $"ü�� +{hpLevel * 10f}%";
        hpCostText.text = hpLevel >= MAX_LEVEL ? "�ִ� ����" : $"{hpCost} G";

        reloadLevelText.text = $"+{reloadLevel}";
        reloadEffectText.text = $"�������ӵ� -{reloadLevel * 5f}%";
        reloadCostText.text = reloadLevel >= MAX_LEVEL ? "�ִ� ����" : $"{reloadCost} G";

        UpdateButtonState(moveSpeedButton, moveSpeedLevel >= MAX_LEVEL);
        UpdateButtonState(attackButton, attackLevel >= MAX_LEVEL);
        UpdateButtonState(hpButton, hpLevel >= MAX_LEVEL);
        UpdateButtonState(reloadButton, reloadLevel >= MAX_LEVEL);
    }

    private void UpdateButtonState(Button button, bool isMaxed)
    {
        if (button == null) return;
        button.interactable = !isMaxed;

        if (isMaxed)
            button.image.color = maxedButtonColor;
    }

    public void UpgradeMoveSpeed() => TryUpgrade(ref moveSpeedLevel, moveSpeedCost);
    public void UpgradeAttack() => TryUpgrade(ref attackLevel, attackCost);
    public void UpgradeHP() => TryUpgrade(ref hpLevel, hpCost);
    public void UpgradeReload() => TryUpgrade(ref reloadLevel, reloadCost);

    private void TryUpgrade(ref int level, int cost)
    {
        if (level >= MAX_LEVEL) return;

        int gold = player != null ? player.totalGold : totalGoldLocal;
        if (gold >= cost)
        {
            gold -= cost;
            level++;

            if (player != null)
                player.totalGold = gold;
            else
                totalGoldLocal = gold;

            if (player != null)
                ApplyUpgradeEffects();

            SaveUpgrades();
            UpdateUI();
        }
        else
        {
            if (!isShowingGoldWarning)
                StartCoroutine(ShowGoldWarning());
        }
    }

    private IEnumerator ShowGoldWarning()
    {
        Debug.Log("��� ����!");
        isShowingGoldWarning = true;
        notEnoughGoldText.SetActive(true);
        yield return new WaitForSecondsRealtime(1f); 
        notEnoughGoldText.SetActive(false);
        isShowingGoldWarning = false;
    }

}