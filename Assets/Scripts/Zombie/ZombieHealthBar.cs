using UnityEngine;
using UnityEngine.UI;

public class ZombieHealthBar : MonoBehaviour
{
    public Image fillImage;
    private Transform cam;
    private Transform player;
    public float hideDistance = 30f;

    void Start()
    {
        cam = Camera.main.transform;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void LateUpdate()
    {
        if (cam != null)
            transform.LookAt(transform.position + cam.forward);

        if (player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);

            bool shouldShow = dist <= hideDistance;

            if (fillImage != null)
                fillImage.enabled = shouldShow;

            foreach (var img in GetComponentsInChildren<Image>())
                img.enabled = shouldShow;
        }
    }

    public void UpdateHealthBar(float curHP, float maxHP)
    {
        if (fillImage != null)
            fillImage.fillAmount = Mathf.Clamp01(curHP / maxHP);
    }
}