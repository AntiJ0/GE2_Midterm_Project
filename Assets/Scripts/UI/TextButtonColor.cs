using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TextButtonColorTMP : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("TMP 텍스트")]
    public TMP_Text buttonText;

    [Header("색상 설정")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(0.85f, 0.85f, 0.85f); 
    public Color clickColor = new Color(0.7f, 0.7f, 0.7f);    
    public Color disabledColor = new Color(0.5f, 0.5f, 0.5f); 

    private Button button;
    private bool isPointerOver = false;

    private void Start()
    {
        button = GetComponent<Button>();

        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>();

        UpdateTextColor();
    }

    private void Update()
    {
        UpdateTextColor();
    }

    private void UpdateTextColor()
    {
        if (button == null || buttonText == null) return;

        if (!button.interactable)
            buttonText.color = disabledColor;
        else if (!isPointerOver)
            buttonText.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button == null || !button.interactable) return; 

        isPointerOver = true;
        buttonText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button == null || !button.interactable) return;

        isPointerOver = false;
        buttonText.color = normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button == null || !button.interactable) return;

        buttonText.color = clickColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (button == null || !button.interactable) return;

        buttonText.color = isPointerOver ? hoverColor : normalColor;
    }
}