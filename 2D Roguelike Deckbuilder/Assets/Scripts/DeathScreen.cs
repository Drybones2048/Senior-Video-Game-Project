using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public GameObject deathScreenPanel;
    public GameObject victoryScreenPanel;
    public Button tryAgainButton;   //for restarting the game after a defeat
    public Button playAgainButton;  //for restarting the game after a victory

    public static UnityEvent resetGame = new UnityEvent();
    public static UnityEvent gameWon = new UnityEvent();

    void Awake() {
        Player.playerDead.AddListener(ShowDefeatScreen);

        StartScreen.choseClass.AddListener((_) => HideAll()); // Hides death screen after player confirms a class

        gameWon.AddListener(ShowVictoryScreen);
    }

    void Start()
    {
        HideAll();
    }

    void OnDestroy()
    {
        Player.playerDead.RemoveListener(ShowDefeatScreen);
        StartScreen.choseClass.RemoveListener((_) => HideAll());
        gameWon.RemoveListener(ShowVictoryScreen);
    }

    void ShowDefeatScreen()
    {
        deathScreenPanel.gameObject.SetActive(true);
        RoundManager.instance.EndCombat();
        tryAgainButton.onClick.RemoveAllListeners(); // Prevent duplicate listeners on repeated deaths
        tryAgainButton.onClick.AddListener(buttonClicked);
    }

    void ShowVictoryScreen() {
        victoryScreenPanel.gameObject.SetActive(true);
        RoundManager.instance.EndCombat();
        playAgainButton.onClick.RemoveAllListeners(); // Prevent duplicate listeners on repeated deaths
        playAgainButton.onClick.AddListener(buttonClicked);
    }

    public void HideAll()
    {
        deathScreenPanel.gameObject.SetActive(false);
        victoryScreenPanel.gameObject.SetActive(false);
    }

    void buttonClicked() //called when player clicks restart button after victory or defeat
    {
        resetGame.Invoke();
        HideAll();
    }
}