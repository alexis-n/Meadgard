using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DG.Tweening;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    [SerializeField]
    private RectTransform mainMenuRect, characterSelectRect;

    [SerializeField]
    private float animationDuration = 1f;

    [SerializeField]
    private DOTweenAnimation[] mainMenuAnimations, characterSelectAnimations;

    [SerializeField]
    private Image godIcon, loadingGamePanel, loading;

    [SerializeField]
    private float fadeDuration = 2f;

    [SerializeField]
    private GameObject launchGameButton;
    
    public CursorsBank cursorsBank;

    [SerializeField]
    private Image resumeGameIcon;
    [SerializeField]
    private TextMeshProUGUI dateTime, turn;

    [SerializeField] private CanvasGroup startPanel;

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1;

        GameSaver.instance.LoadGame();

        dateTime.text = GameSaver.instance.gameSave.gameDate + " " + GameSaver.instance.gameSave.gameTime;
        turn.text = "Jour " + GameSaver.instance.gameSave.gameTurn;
        resumeGameIcon.sprite = GameManager.instance.data.gods[GameSaver.instance.gameSave.gameGodIndex].sprite;
    }

    private void Start()
    {
        startPanel.alpha = 1;
        startPanel.DOFade(0, 1f);
    }

    //fonction qui permet de lancer une nouvelle partie
    public void StartNewGame()
    {
        //on cache le menu principal
        for (int i = 0; i < mainMenuAnimations.Length; i++)
        {
            mainMenuAnimations[i].DOPlayBackwards();
        }
        //on affiche le menu de sélection de personnage
        for (int i = 0; i < characterSelectAnimations.Length; i++)
        {
            characterSelectAnimations[i].DORestart();
        }
    }

    //fonction qui reviens au menu principal
    public void BackToMainMenu()
    {
        GameManager.god = null;
        launchGameButton.SetActive(false);

        //on cache le menu de sélection de personnage
        for (int i = 0; i < characterSelectAnimations.Length; i++)
        {
            characterSelectAnimations[i].DOPlayBackwards();
        }
        //on affiche le menu principal
        for (int i = 0; i < mainMenuAnimations.Length; i++)
        {
            mainMenuAnimations[i].DORestart();
        }
    }

    //fonction qui permet de continuer une partie
    public void ResumeGame()
    {
        GameManager.god = GameManager.instance.data.gods[GameSaver.instance.gameSave.gameGodIndex];

        loadingGamePanel.gameObject.SetActive(true);
        loading.DOFade(1, fadeDuration);
        loadingGamePanel.DOFade(1, fadeDuration)
            .OnComplete(() => SceneManager.LoadScene("Ingame"));
        GetComponent<Animation>().DOPlay();
    }

    //fonction qui sélectionne un dieu
    public void SelectGod(int index)
    {
        //on enregistre le choix du joueur dans le Game Manager
        GameManager.god = GameManager.instance.data.gods[index];
        launchGameButton.SetActive(true);
        godIcon.sprite = GameManager.god.sprite;
    }

    //fonction qui lance une partie
    public void LaunchNewGame()
    {
        GameSaver.instance.gameSave = GameSaver.instance.NewGame(System.Array.IndexOf(GameManager.instance.data.gods, GameManager.god));

        AudioManager.instance.StopMusic();
        loadingGamePanel.gameObject.SetActive(true);
        loading.DOFade(1, fadeDuration);
        loadingGamePanel.DOFade(1, fadeDuration)
            .OnComplete(() => SceneManager.LoadScene("Ingame"));
        GetComponent<Animation>().DOPlay();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
