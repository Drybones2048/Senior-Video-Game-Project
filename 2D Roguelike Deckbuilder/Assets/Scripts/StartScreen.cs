using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;
using System;

public class StartScreen : MonoBehaviour
{
    [Header("Panel Reference")]
    public GameObject characterSelectPanel;

    [Header("God Art Display")]
    public Image RaArtImage;
    public Image SetArtImage;
    public Image HorusArtImage;

    [Header("Description Text")]
    public TextMeshProUGUI godNameText;
    public TextMeshProUGUI classDescriptionText;

    [Header("Class Buttons")]
    public Button raButton;
    public Button setButton;
    public Button horusButton;

    [Header("RoundManager Reference")]
    public RoundManager roundManager;

    [Header("Death Screen Reference")]
    public DeathScreen deathScreen; // Explicitly hidden when player confirms class selection

    public static UnityEvent<PlayerClass> choseClass = new UnityEvent<PlayerClass>();
    public static UnityEvent afterChosenClass = new UnityEvent();

    private const string RA_NAME        = "RA";
    private const string RA_DESCRIPTION = "If the player is below 50% HP, gain 2 strengthen at the start of the turn.";

    private const string SET_NAME        = "SET";
    private const string SET_DESCRIPTION = "Gain 1 strength at the start of each turn for each active enemy debuff.";

    private const string HORUS_NAME        = "HORUS";
    private const string HORUS_DESCRIPTION = "If incoming damage for a turn is completely blocked, the player deals back 30% of the damage they were dealt.";

    void Start()
    {
        DeathScreen.resetGame.AddListener(ShowForReset);
        Show(); // Show the start screen on first launch
    }

    void OnDestroy()
    {
        DeathScreen.resetGame.RemoveListener(ShowForReset);
    }

    public void Show()
    {
        // Clear all button listeners first to prevent duplicates on re-show
        raButton.onClick.RemoveAllListeners();
        setButton.onClick.RemoveAllListeners();
        horusButton.onClick.RemoveAllListeners();

        // Clear all EventTrigger listeners too
        ClearEventTrigger(raButton);
        ClearEventTrigger(setButton);
        ClearEventTrigger(horusButton);

        // Wire up hover and click events fresh
        AddHoverEvents(raButton,    RaArtImage,    RA_NAME,    RA_DESCRIPTION);
        AddHoverEvents(setButton,   SetArtImage,   SET_NAME,   SET_DESCRIPTION);
        AddHoverEvents(horusButton, HorusArtImage, HORUS_NAME, HORUS_DESCRIPTION);

        raButton.onClick.AddListener(()    => SelectClass(PlayerClass.Ra));
        setButton.onClick.AddListener(()   => SelectClass(PlayerClass.Set));
        horusButton.onClick.AddListener(() => SelectClass(PlayerClass.Horus));

        ResetToDefault();
        characterSelectPanel.SetActive(true);
    }

    private void ShowForReset() // Used to show the screen again after the player loses a run and goes to reset
    {
        Show();
    }

    public void Hide()
    {
        characterSelectPanel.SetActive(false);
        godNameText.text = "";
        classDescriptionText.text = "";
    }

    private void ResetToDefault() // Method that will be called when player is not hovering over a button
    {
        RaArtImage.gameObject.SetActive(true);
        SetArtImage.gameObject.SetActive(false);
        HorusArtImage.gameObject.SetActive(false);

        godNameText.text = "";
        classDescriptionText.text = "Hover over a class to learn more.";
    }

    private void ClearEventTrigger(Button button) // Clears events to avoid infinite loops when resetting
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger != null)
            trigger.triggers.Clear();
    }

    private void AddHoverEvents(Button button, Image godImage, string godName, string description) // Code for when the player hovers over the button
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((_) =>
        {
            RaArtImage.gameObject.SetActive(godName == RA_NAME);
            SetArtImage.gameObject.SetActive(godName == SET_NAME);
            HorusArtImage.gameObject.SetActive(godName == HORUS_NAME);
            godNameText.text = godName;
            classDescriptionText.text = description;
        });
        trigger.triggers.Add(enterEntry);

        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((_) => ResetToDefault());
        trigger.triggers.Add(exitEntry);
    }

    private void SelectClass(PlayerClass selectedClass)
    {
        Debug.Log($"Selected class: {selectedClass}");

        choseClass.Invoke(selectedClass);     // Assigns the class and updates player sprite/deck
        afterChosenClass.Invoke();            // Builds the starting deck

        roundManager.ReinitialiseForNewRun(); // Resets all run state and starts combat

        deathScreen.Hide();

        Hide();
    }
}