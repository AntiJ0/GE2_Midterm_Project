using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("이동 관련")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    [Header("플레이어 스탯")]
    public float maxHP = 100f;
    public float curHP;
    public float attackMultiplier = 1f;
    public float reloadSpeedMultiplier = 1f;

    [Header("무기 관련")]
    public int magazineSize = 30;
    public int currentAmmo;
    public float reloadTime = 2f;
    [HideInInspector] public bool isReloading = false;

    [Header("골드 관련")]
    public int totalGold = 0;   
    public int stageGold = 0;   
    public TMP_Text goldText;   

    [Header("UI")]
    public TMP_Text ammoText;
    public TMP_Text reloadTimerText;
    public Image hpBarFill;
    public TMP_Text hpText;
    public GameObject gameOverPanel;

    private Coroutine reloadRoutine;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        curHP = maxHP;
        currentAmmo = magazineSize;

        totalGold = PlayerPrefs.GetInt("TotalGold", 0);
        stageGold = 0;

        ApplyUpgradesFromPrefs();

        if (hpBarFill != null)
        {
            hpBarFill.type = Image.Type.Filled;
            hpBarFill.fillMethod = Image.FillMethod.Horizontal;
            hpBarFill.fillOrigin = (int)Image.OriginHorizontal.Right;
            hpBarFill.fillAmount = 1f;
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        reloadTimerText.gameObject.SetActive(false);
        UpdateUI();
    }

    private void Update()
    {
        if (curHP <= 0) return;

        HandleMovement();
        HandleReloadInput();
        UpdateUI();
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);

        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleReloadInput()
    {
        if (!isReloading && Input.GetKeyDown(KeyCode.R))
        {
            reloadRoutine = StartCoroutine(ReloadCoroutine());
        }

        if (!isReloading && currentAmmo <= 0)
        {
            reloadRoutine = StartCoroutine(ReloadCoroutine());
        }
    }

    public IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        float adjustedReloadTime = reloadTime * reloadSpeedMultiplier; 

        float remainingTime = adjustedReloadTime;
        reloadTimerText.gameObject.SetActive(true);

        while (remainingTime > 0)
        {
            reloadTimerText.text = $"{remainingTime:F1}s";
            yield return new WaitForSeconds(0.1f);
            remainingTime -= 0.1f;
        }

        reloadTimerText.gameObject.SetActive(false);
        currentAmmo = magazineSize;
        isReloading = false;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (hpBarFill != null)
        {
            float ratio = Mathf.Clamp01(curHP / maxHP);
            hpBarFill.fillAmount = ratio;
        }

        if (hpText != null)
            hpText.text = $"{curHP:F0} / {maxHP:F0}";

        if (ammoText != null)
            ammoText.text = $"{currentAmmo} / ∞";

        if (goldText != null)
            goldText.text = $"{stageGold} G";
    }

    public void AddGold(int amount)
    {
        stageGold += amount;
        totalGold += amount;

        PlayerPrefs.SetInt("TotalGold", totalGold);
        PlayerPrefs.Save();

        UpdateUI();
    }

    public void ConsumeAmmo()
    {
        if (!isReloading && currentAmmo > 0)
        {
            currentAmmo--;
            UpdateUI();

            if (currentAmmo <= 0 && !isReloading)
            {
                if (reloadRoutine != null)
                    StopCoroutine(reloadRoutine);

                reloadRoutine = StartCoroutine(ReloadCoroutine());
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (curHP <= 0) return;

        curHP = Mathf.Max(0, curHP - amount);
        UpdateUI();

        if (curHP <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Time.timeScale = 0f;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ApplyUpgradesFromPrefs()
    {
        int moveSpeedLevel = PlayerPrefs.GetInt("MoveSpeedLevel", 0);
        int attackLevel = PlayerPrefs.GetInt("AttackLevel", 0);
        int hpLevel = PlayerPrefs.GetInt("HPLevel", 0);
        int reloadLevel = PlayerPrefs.GetInt("ReloadLevel", 0);

        moveSpeed = 5f * (1f + 0.025f * moveSpeedLevel);
        attackMultiplier = 1f + 0.1f * attackLevel;
        maxHP = 100f * (1f + 0.1f * hpLevel);
        reloadSpeedMultiplier = Mathf.Max(0.5f, 1f - 0.05f * reloadLevel);

        curHP = maxHP;
    }
}