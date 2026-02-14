using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrawPile : MonoBehaviour
{
     public TextMeshProUGUI drawPileText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        drawPileText.text = Deck.drawPile.Count.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        drawPileText.text = Deck.drawPile.Count.ToString();
    }
}
