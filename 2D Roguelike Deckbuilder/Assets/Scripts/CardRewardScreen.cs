using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;

public class CardRewardScreen : MonoBehaviour
{
    public CanvasGroup rewardScreenCG;
    public CardView[] rewardCardSlots;

    public RoundManager roundManager;

    public CardLibrary cardLibrary;

    public float fadeInDuration = 0.3f; // Animation for screen appearing
    public int numberOfRewards = 3; // Number of cards on screen

    public static CardRewardScreen Instance { get; private set; }
    

    void Awake()
    {
        if (Instance == null) // Sets up singleton
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        if (rewardScreenCG != null) // Start hidden
        {
            rewardScreenCG.alpha = 0;
            rewardScreenCG.interactable = false;
            rewardScreenCG.blocksRaycasts = false;
        }
    }

    void OnEnable()
    {
        RoundManager.enemyDead.AddListener(ShowRewardScreen); // Listens for when the enemy has 0 or lower HP
    }

    void OnDisable()
    {
        RoundManager.enemyDead.RemoveListener(ShowRewardScreen);
    }

    void ShowRewardScreen() // Method that will set up the card reward display
    {
        if (roundManager.encounterNumber == 4) // When pharaoh phase 2 has been killed
        {
            //TODO: Make a 'You Win' Screen appear in this statement

        } else if(roundManager.encounterNumber != 3) // For all combats that are not the pharaoh, give a card reward
        {
            Debug.Log("Enemy defeated! Showing card rewards...");

            List<CardInstance> rewardCards = GenerateRewardCards();

            DisplayRewardCards(rewardCards);

            FadeInRewardScreen();
        } 
        else // Skip the card reward between pharaoh phase 1 and 2
        {
            roundManager.StartNewCombat();
        }
        
    }

    List<CardInstance> GenerateRewardCards() // Generates the number of card rewards as requested by the variable
    {
        //add whatever conditions you want to the first parameter
        return cardLibrary.GetRandomCard(c => c.cardClass == CardClass.BlockStance, numberOfRewards); 
    }

    void DisplayRewardCards(List<CardInstance> cards) // Displays the possible reward cards on screen
    {
        for (int i = 0; i < rewardCardSlots.Length && i < cards.Count; i++)
        {
            if (rewardCardSlots[i] != null)
            {
                rewardCardSlots[i].Setup(cards[i]);
                  
                RewardCardHandler handler = rewardCardSlots[i].GetComponent<RewardCardHandler>(); // Make cards clickable to add them to deck
                
                if (handler == null)
                {
                    handler = rewardCardSlots[i].gameObject.AddComponent<RewardCardHandler>();
                }
                handler.SetCard(cards[i]);
            }
        }
    }

    void FadeInRewardScreen() // Make the reward screen slowly appear rather than pop in instantly
    {
        if (rewardScreenCG != null)
        {
            rewardScreenCG.interactable = true;
            rewardScreenCG.blocksRaycasts = true;
            
            rewardScreenCG.DOFade(1f, fadeInDuration).SetEase(Ease.OutQuad);
        }
    }

    public void HideRewardScreen() // Hide the reward screen after the card is chosen
    {
        if (rewardScreenCG != null)
        {
            rewardScreenCG.DOFade(0f, fadeInDuration)
                .SetEase(Ease.InQuad)
                .OnComplete(() => {
                    rewardScreenCG.interactable = false;
                    rewardScreenCG.blocksRaycasts = false;
                });
        }

        roundManager.StartNewCombat(); // After a card reward is chosen, a new enemy encounter will begin
    }
}
