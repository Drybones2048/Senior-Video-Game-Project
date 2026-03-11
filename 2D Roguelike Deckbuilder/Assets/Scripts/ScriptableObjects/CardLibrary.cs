using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CardLibrary", menuName = "Scriptable Objects/CardLibrary")]
public class CardLibrary : ScriptableObject
{
    [SerializeField] private List<CardData> cardList = new();
    Dictionary<string, CardData> cardDict = new Dictionary<string, CardData>();

    void OnEnable() {
        cardDict.Clear();   //in case CardLibrary were somehow disabled and re-enabled during runtime

        foreach (CardData card in cardList) {
            //safeguard in case two cards are accidentally created with the same ID
            if (cardDict.ContainsKey(card.id))
            {
                Debug.LogError($"Duplicate card id detected: {card.id}", this);
                continue;
            }

            cardDict.Add(card.id, card);
        }
    }

    public CardData GetCardById(string id) {
        if (cardDict.ContainsKey(id))
        {
            return cardDict[id];
        }
        else {
            throw new KeyNotFoundException($"Card name '{id}' was not found in CardLibrary.");
        }
    }

    public CardInstance CreateInstance(string id, bool upgraded = false)
    {
        CardData card = GetCardById(id);
        if (card == null) return null;

        return new CardInstance(card, upgraded);
    }

    public List<CardInstance> GetRandomCard(System.Predicate<CardData> filter, int numCardsRequested)
    {
        var newCardPool = new List<CardData>();

        foreach (var c in cardList)
            if (c != null && filter(c))
                newCardPool.Add(c);

        if (newCardPool.Count == 0)
        {
            Debug.Log("Card requested but no cards match the criteria.");
            return new List<CardInstance>();
        }

        // Shuffle
        for (int i = 0; i < newCardPool.Count; i++)
        {
            int j = RoundManager.instance.RNG.Next(i, newCardPool.Count);
            (newCardPool[i], newCardPool[j]) = (newCardPool[j], newCardPool[i]);
        }

        int numCardsReturning = Mathf.Min(numCardsRequested, newCardPool.Count);    //this would only not equal numCardsRequested if more cards were requested than met the criteria

        var selectedCards = new List<CardInstance>();

        //now that the deck is shuffled, you can just select the number of cards you need starting from index 0. 
        for (int i = 0; i < numCardsReturning; i++)
            selectedCards.Add(new CardInstance(newCardPool[i], false));


        return selectedCards;
    }
}
