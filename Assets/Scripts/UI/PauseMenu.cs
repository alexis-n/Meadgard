using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public GameObject content, blackPanel;
    [SerializeField] private Slider masterSlider, ambientSlider, SFXSlider, musicSlider, voicesSlider;
    public bool paused = false, controlsEnabled = true;

    private void Awake()
    {
        instance = this;
        if (content.activeSelf) content.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        content.SetActive(true);

        var temp = 0f;
        AudioManager.instance.audioMixer.GetFloat("Master Volume", out temp);
        masterSlider.value = temp;
        AudioManager.instance.audioMixer.GetFloat("Music Volume", out temp);
        musicSlider.value = temp;
        AudioManager.instance.audioMixer.GetFloat("Ambient Volume", out temp);
        ambientSlider.value = temp;
        AudioManager.instance.audioMixer.GetFloat("SFX Volume", out temp);
        SFXSlider.value = temp;
        AudioManager.instance.audioMixer.GetFloat("Voices Volume", out temp);
        voicesSlider.value = temp;

        paused = true;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        content.SetActive(false);
        paused = false;
    }

    public void MainMenu()
    {
        controlsEnabled = false;
        var temp = blackPanel.GetComponent<CanvasGroup>();
        temp.alpha = 0;
        blackPanel.SetActive(true);
        temp.DOFade(1, 1f).SetUpdate(true).OnComplete(() => GameManager.instance.MainMenu());
    }

    public void QuitGame()
    {
        Debug.Log("Application.Quit() called");
        Application.Quit();
    }

    public void SetMasterVolume(float volume)
    {
        AudioManager.instance.audioMixer.SetFloat("Master Volume", volume);
    }
    public void SetMusicVolume(float volume)
    {
        AudioManager.instance.audioMixer.SetFloat("Music Volume", volume);
    }
    public void SetSFXVolume(float volume)
    {
        AudioManager.instance.audioMixer.SetFloat("SFX Volume", volume);
    }
    public void SetAmbientVolume(float volume)
    {
        AudioManager.instance.audioMixer.SetFloat("Ambient Volume", volume);
    }
    public void SetVoicesVolume(float volume)
    {
        AudioManager.instance.audioMixer.SetFloat("Voices Volume", volume);
    }
}
