using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SelectionAnimation : MonoBehaviour {

    public bool belongsToPlayer = false;
    public Color baseColor;
    public ParticleSystem particleSystem;
    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (belongsToPlayer) GetComponent<SpriteRenderer>().color = PlayerManager.instance.tavern.god.color;
        baseColor = spriteRenderer.color;
        spriteRenderer.enabled = false;
    }

    public void Play(Color color)
    {
        spriteRenderer.enabled = true;
        spriteRenderer.color = color;
        particleSystem.startColor = color;
        particleSystem.Play();
    }

    public void Rewind()
    {
        spriteRenderer.enabled = false;
        particleSystem.Stop();
    }
}
