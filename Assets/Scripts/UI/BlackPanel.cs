using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BlackPanel : MonoBehaviour
{
    public Image image;
    [SerializeField] private RectTransform dayTextRect, missiveRect;
    [SerializeField] private float dayTextMovement, missiveMovemenent;
    [SerializeField] private GameObject[] missives, buttons;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private CanvasGroup blackScreenGroup, missiveGroup;
    public TextMeshProUGUI[] missiveText, missiveName;
    public Image[] missiveGradient;
    [SerializeField]
    private DOTweenAnimation animation;

    public RecapPanel recapPanel;

    private void Update()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void FadeToPanel()
    {
        CameraManager.instance.controlsEnabled = false;
        animation.gameObject.SetActive(false);
        dayText.alpha = 0;
        image.gameObject.SetActive(true);
        dayTextRect.anchoredPosition = Vector2.zero;
        recapPanel.GetComponent<CanvasGroup>().alpha = 0;
        blackScreenGroup.alpha = 1;
        missiveRect.anchoredPosition = Vector2.zero;
        image.DOFade(1, fadeDuration).OnComplete(() => recapPanel.StartRecap()); 
    }

    public void ChooseMissive(int index)
    {
        Missive.currentMissive = Missive.odinMissives[index];
        FadeToGame();
    }

    public void FadeToMissive()
    {
        dayText.alpha = 0;
        dayText.text = "Jour " + PhaseManager.instance.turn.ToString();
        dayText.DOFade(1, fadeDuration).OnComplete(() => ShowMissive());
    }

    public void Loading()
    {
        animation.gameObject.SetActive(true);
        animation.DOPlay();
    }

    public void FadeToGame()
    {
        UIManager.instance.OpenMenu(UIManager.instance.newsboardMenu);
        animation.gameObject.SetActive(false);
        blackScreenGroup.DOFade(0, fadeDuration);
        image.DOFade(0, fadeDuration)
            .OnComplete(() =>
            {
                image.gameObject.SetActive(false);
                CameraManager.instance.controlsEnabled = true;
            });
        missiveGroup.DOFade(0, fadeDuration*4);
    }

    public void ShowMissive()
    {
        foreach (var item in buttons)
        {
            item.SetActive(true);
        }

        foreach (var item in missives)
        {
            item.SetActive(false);
        }

        if (PlayerManager.instance.tavern.god.name == "Odin")
        {
            missives[1].SetActive(true);
            for (int i = 1; i < missives.Length; i++)
            {
                missiveText[i].text = Missive.odinMissives[i - 1].missiveDescription;
                missiveName[i].text = Missive.odinMissives[i - 1].missiveName;
                missiveGradient[i].color = Missive.odinMissives[i - 1].missiveColor;
            }
        }
        else
        {
            missives[0].SetActive(true);

            missiveText[0].text = Missive.currentMissive.missiveDescription;
            missiveName[0].text = Missive.currentMissive.missiveName;
            missiveGradient[0].color = Missive.currentMissive.missiveColor;
        }

        dayTextRect.DOAnchorPos(new Vector2(dayTextRect.anchoredPosition.x, dayTextRect.anchoredPosition.y + dayTextMovement), 1f);
        missiveRect.DOAnchorPos(new Vector2(missiveRect.anchoredPosition.x, missiveRect.anchoredPosition.y + missiveMovemenent), 1f);
        missiveGroup.DOFade(1, fadeDuration);
    }

    public void DisableAllButtons()
    {
        foreach (var item in buttons)
        {
            item.SetActive(false);
        }
    }
}
