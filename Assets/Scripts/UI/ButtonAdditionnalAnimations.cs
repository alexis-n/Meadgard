using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(EventTrigger))]
public class ButtonAdditionnalAnimations : MonoBehaviour, IPointerClickHandler {

    public UnityEvent MethodToConfirm = new UnityEvent(),
        rightClickMethod = new UnityEvent();

    Button button;
    RectTransform rect;
    Vector3 baseScale, basePosition;
    public float scaleMultiplier = 1f, xMovementAmount = 0, yMovementAmount = 0, zMovementAmount = 0,
        confirmationDuration = 0.5f;
    public bool deactivate = false;
    public Image confirmationFiller;
    Tween fillingTween;

    CursorsBank cursorsBank;

    private void Awake()
    {
        button = GetComponent<Button>();
        rect = GetComponent<RectTransform>();
        baseScale = rect.localScale;
        basePosition = rect.anchoredPosition;
    }

    private void Start()
    {
        if (UIManager.instance != null) cursorsBank = UIManager.instance.cursorsBank;
        else cursorsBank = MainMenu.instance.cursorsBank;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            rightClickMethod.Invoke();
    }

    private void OnDisable()
    {
        rect.localScale = baseScale;
        rect.anchoredPosition = basePosition;
    }

    public void Deactivate()
    {
        deactivate = true;
    }

    public void ResetBase()
    {
        baseScale = rect.localScale;
        basePosition = rect.anchoredPosition;
    }

    public void PointerEnterPositionAnimation()
    {
        if (!deactivate)
        {
            cursorsBank.Hover();
            if (CameraManager.instance != null && !CameraManager.instance.mouseCameraInput)
                rect.DOAnchorPos(new Vector2((basePosition.x + xMovementAmount), (basePosition.y + yMovementAmount)), 0.1f);
            else if (CameraManager.instance == null) rect.DOAnchorPos(new Vector2((basePosition.x + xMovementAmount), (basePosition.y + yMovementAmount)), 0.1f);
        }
            
    }

    public void PointerEnterPositionXAnimation()
    {
        if (!deactivate)
        {
            cursorsBank.Hover();
            if (CameraManager.instance != null && !CameraManager.instance.mouseCameraInput)
                rect.DOAnchorPos(new Vector2((basePosition.x + xMovementAmount), rect.anchoredPosition.y), 0.1f);
            else if (CameraManager.instance == null) rect.DOAnchorPos(new Vector2((basePosition.x + xMovementAmount), rect.anchoredPosition.y), 0.1f);
        }

    }

    public void PointerExitPositionAnimation()
    {
        cursorsBank.Basic();
        rect.DOAnchorPos(basePosition, 0.1f);
    }

    public void PointerExitPositionXAnimation()
    {
        cursorsBank.Basic();
        rect.DOAnchorPos(new Vector2(basePosition.x, rect.anchoredPosition.y), 0.1f);
    }

    public void PointerEnterScaleAnimation()
    {
        if (!deactivate)
        {
            cursorsBank.Hover();
            if (CameraManager.instance != null && !CameraManager.instance.mouseCameraInput)
                rect.DOScale(baseScale * scaleMultiplier, 0.1f);
            else if (CameraManager.instance == null) rect.DOScale(baseScale * scaleMultiplier, 0.1f);
        }
    }

    public void PointerExitScaleAnimation()
    {
        cursorsBank.Basic();
        rect.DOScale(baseScale, 0.1f);
    }

    public void PressingButton()
    {
        if (!deactivate) fillingTween = confirmationFiller.DOFillAmount(1, confirmationDuration).SetEase(Ease.Linear).OnComplete(() => FillingEnd());
    }

    public void FillingEnd()
    {
        fillingTween.Rewind();
        MethodToConfirm.Invoke();
    }

    public void ReleasingButton()
    {
        if (!deactivate) fillingTween.Rewind();
    }
}
