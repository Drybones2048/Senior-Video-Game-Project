using UnityEngine;
using UnityEngine.UI;

public class ArtManager : MonoBehaviour
{
    void OnEnable()
    {
        CardView.HoverEnter += SetArt; // Listener activates method
    }

    // Sets the art of the magnified card to the art of the card that is being hovered over with mouse
    void SetArt(Card card)
    {
        gameObject.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>(card.sprite);
    }
}
