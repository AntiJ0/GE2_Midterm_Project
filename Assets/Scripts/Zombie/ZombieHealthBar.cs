using UnityEngine;
using UnityEngine.UI;

public class ZombieHealthBar : MonoBehaviour
{
    public Image fillImage;
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (cam != null)
            transform.LookAt(transform.position + cam.forward);
    }

    public void UpdateHealthBar(float curHP, float maxHP)
    {
        if (fillImage != null)
            fillImage.fillAmount = Mathf.Clamp01(curHP / maxHP);
    }
}