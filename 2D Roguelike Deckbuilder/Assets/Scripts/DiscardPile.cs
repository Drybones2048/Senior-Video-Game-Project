using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiscardPile : MonoBehaviour
{
     public TextMeshProUGUI discardPileText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        discardPileText.text = Deck.discard.Count.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        discardPileText.text = Deck.discard.Count.ToString();
    }
}
