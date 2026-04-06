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
    public Button tryAgainButton;

    public static UnityEvent resetGame = new UnityEvent();

    void Start()
    {
        Hide();
        Player.playerDead.AddListener(Show);

        StartScreen.choseClass.AddListener((_) => Hide()); // Hides death screen after player confirms a class
    }

    void OnDestroy()
    {
        Player.playerDead.RemoveListener(Show);
        StartScreen.choseClass.RemoveListener((_) => Hide());
    }

    void Show()
    {
        deathScreenPanel.gameObject.SetActive(true);
        RoundManager.instance.EndCombat();
        tryAgainButton.onClick.RemoveAllListeners(); // Prevent duplicate listeners on repeated deaths
        tryAgainButton.onClick.AddListener(buttonClicked);
    }

    public void Hide()
    {
        deathScreenPanel.gameObject.SetActive(false);
    }

    void buttonClicked() // When the player clicks 'try again', the game will reset using this event
    {
        resetGame.Invoke();
        Hide();
    }
}