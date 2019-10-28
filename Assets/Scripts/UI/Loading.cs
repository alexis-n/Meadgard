using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Loading : MonoBehaviour {

    Image image;
    public float speed;
    Sequence sequence;

    // Use this for initialization
    void Start () {
        image = GetComponent<Image>();
        image.fillAmount = 0;
        sequence = DOTween.Sequence();
    }
	
	public void LoadingAnimation()
    {
        image.DOFillAmount(1, 10f);
    }

    public void StopAnimation()
    {
        image.enabled = false;
    }
}
