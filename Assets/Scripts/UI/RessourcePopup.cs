using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class RessourcePopup : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [HideInInspector] public RectTransform rect;
    [SerializeField] private RectTransform animationRect;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image icon, background;
    [SerializeField] private float animationDuration = 0.5f, stayDuration = 0.5f;
    [SerializeField] private Vector2 movementAmount;
    private Vector3 worldPosition;

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
        canvasGroup.alpha = 0;
    }

    private void Update()
    {
        rect.position = Camera.main.WorldToScreenPoint(worldPosition);
    }

    public void Popup(Vector3 position, int valueToDisplay, Color backgroundColor ,Sprite iconOfRessource = null)
    {
        Destroy(this, 5f);

        worldPosition = position;

        text.text = valueToDisplay > 0 ? "+" + valueToDisplay : valueToDisplay.ToString();
        icon.sprite = iconOfRessource == null ? icon.sprite : iconOfRessource;
        background.color = backgroundColor;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(1f, animationDuration))
            .Join(animationRect.DOAnchorPos(movementAmount, animationDuration).SetRelative())
            .AppendInterval(stayDuration)
            .Append(canvasGroup.DOFade(0f, animationDuration))
            .Join(animationRect.DOAnchorPos(-movementAmount, animationDuration).SetRelative());
    }
}
