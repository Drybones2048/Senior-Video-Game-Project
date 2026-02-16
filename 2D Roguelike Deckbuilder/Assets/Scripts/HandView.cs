using UnityEngine;
using UnityEngine.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using DG.Tweening;

public class HandView : MonoBehaviour
{
    public CardView attackPrefab;
    public CardView defendPrefab;

    public Transform handArea;
    public Transform drawPilePosition;
    public Transform discardPilePosition;
    public Vector3 discardPositionOffset = Vector3.zero;

    public float fanRadius = 300f;
    public float fanAngle = 30f;

    // Animation settings
    public float drawDuration = 0.4f;
    public float delayBetweenCards = 0.1f;
    public Ease drawEaseType = Ease.OutQuad;
    
    public AudioClip cardDrawSound; // Optional: drag your sound effect here

    private List<CardView> cardViews = new();
    private bool isDrawing = false;
    private bool hasDrawnInitialHand = false; // Tracks for intial draw

    void OnEnable()
    {
        CardView.cardClicked += OnCardPlayed; // Restore event subscription but with new method
    }

    void OnDisable()
    {
        CardView.cardClicked -= OnCardPlayed;
    }

    // Separate handler for when cards are played
    void OnCardPlayed(List<Card> cards)
    {
        RefreshHandInstantly(cards);
    }

    // Public method called directly from Deck.drawHand()
    public void DisplayHand(List<Card> cards)
    {
        // Only animate if this is the very first draw
        if (!hasDrawnInitialHand)
        {
            if (isDrawing) return;
            hasDrawnInitialHand = true;
            StartCoroutine(DrawCardsSequentially(cards));
        }
        else
        {
            // If called again (shouldn't happen in normal flow, but just in case)
            RefreshHandInstantly(cards);
        }
    }

    // Animated draw sequence - used only for initial hand
    IEnumerator DrawCardsSequentially(List<Card> cards)
    {
        isDrawing = true;
        ClearHand();

        List<CardPositionData> finalPositions = CalculateCardPositions(cards.Count);

        for (int i = 0; i < cards.Count; i++)
        {
            // Wait before drawing (except for the first card)
            if (i > 0)
            {
                yield return new WaitForSeconds(delayBetweenCards);
            }

            CardView view = CreateCard(cards[i]);

            // Set starting position/scale IMMEDIATELY (before adding to list or doing anything else)
            if (drawPilePosition != null)
            {
                view.transform.position = drawPilePosition.position;
                view.transform.rotation = drawPilePosition.rotation;
            }
            else
            {
                view.transform.localPosition = new Vector3(0, -500, 0);
            }
            view.transform.localScale = Vector3.zero; // Start at zero scale (invisible)

            cardViews.Add(view);

            // Play sound effect (optional)
            if (cardDrawSound != null)
            {
                AudioSource.PlayClipAtPoint(cardDrawSound, Camera.main.transform.position, 0.5f);
            }

            AnimateCardToPosition(view, finalPositions[i]);
        }

        isDrawing = false;
    }

    // Instant refresh - used when card is played and hand needs to re-fan
    void RefreshHandInstantly(List<Card> cards)
    {
        // Kill any lingering DOTween animations on cards before destroying them
        foreach (CardView view in cardViews)
        {
            if (view != null)
            {
                view.transform.DOKill();
            }
        }
        
        ClearHand();

        List<CardPositionData> finalPositions = CalculateCardPositions(cards.Count);

        for (int i = 0; i < cards.Count; i++)
        {
            CardView view = CreateCard(cards[i]);
            cardViews.Add(view);

            // Set position instantly (no animation)
            view.transform.localPosition = finalPositions[i].position;
            view.transform.localRotation = finalPositions[i].rotation;
            view.transform.localScale = Vector3.one;
            
            // Ensure no tweens are accidentally playing on the new card
            view.transform.DOKill();
        }
    }

    void AnimateCardToPosition(CardView card, CardPositionData posData)
    {
        // Start the card invisible
        CanvasGroup canvasGroup = card.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = card.gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0;

        Sequence cardSequence = DOTween.Sequence();

        cardSequence.Join(card.transform.DOLocalMove(posData.position, drawDuration).SetEase(drawEaseType));
        cardSequence.Join(card.transform.DOLocalRotate(posData.rotation.eulerAngles, drawDuration).SetEase(drawEaseType));
        cardSequence.Join(card.transform.DOScale(Vector3.one, drawDuration).SetEase(drawEaseType));
        
        // Fade in the card
        cardSequence.Join(canvasGroup.DOFade(1f, drawDuration * 0.3f)); // Fade in quickly at start

        // Optional pop effect
        cardSequence.Append(card.transform.DOScale(Vector3.one * 1.05f, 0.1f));
        cardSequence.Append(card.transform.DOScale(Vector3.one, 0.1f));
    }

    public void AnimateCardToDiscard(CardView card, System.Action onComplete = null)
    {
        // Remove from our tracking list immediately (so hand doesn't try to manage it)
        if (cardViews.Contains(card))
        {
            cardViews.Remove(card);
        }

        // Kill any existing tweens on this card
        card.transform.DOKill();

        // Determine discard pile position
        Vector3 targetPosition;
        if (discardPilePosition != null)
        {
            // If both are in the same Canvas, use local position
            if (card.transform.parent == discardPilePosition.parent)
            {
                targetPosition = discardPilePosition.localPosition + discardPositionOffset;
            }
            else
            {
                // Use world position
                targetPosition = discardPilePosition.position + discardPositionOffset;
            }
        }
        else
        {
            // Default to bottom-right if no discard pile set
            targetPosition = card.transform.localPosition + new Vector3(500, -300, 0);
        }

        // Create animation sequence with longer duration for visibility
        Sequence discardSequence = DOTween.Sequence();

        // Move to discard pile - using Linear easing for straight path
        discardSequence.Join(card.transform.DOLocalMove(targetPosition, 0.3f).SetEase(Ease.Linear));
        
        // Shrink to zero
        discardSequence.Join(card.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InQuad));

        // Rotate while discarding for extra flair
        discardSequence.Join(card.transform.DOLocalRotate(new Vector3(0, 0, 360), 0.3f, RotateMode.FastBeyond360).SetEase(Ease.Linear));

        // When animation completes, destroy the card and call callback
        discardSequence.OnComplete(() =>
        {
            Destroy(card.gameObject);
            onComplete?.Invoke();
        });
    }

    CardView CreateCard(Card card)
    {
        CardView view;

        if (card.cardName == "Attack")
        {
            view = Instantiate(attackPrefab, handArea);
        }
        else
        {
            view = Instantiate(defendPrefab, handArea);
        }

        view.Setup(card);
        return view;
    }

    List<CardPositionData> CalculateCardPositions(int count)
    {
        List<CardPositionData> positions = new List<CardPositionData>();
        float radius = 400f;
        float maxAngle = 20f;

        if (count == 1)
        {
            positions.Add(new CardPositionData
            {
                position = Vector3.zero,
                rotation = Quaternion.identity
            });
            return positions;
        }
        else if (count == 2)
        {
            float smallAngle = 6f;

            for (int i = 0; i < 2; i++)
            {
                float angle = (i == 0) ? -smallAngle : smallAngle;
                float rad = angle * Mathf.Deg2Rad;

                Vector3 pos = new Vector3(
                    Mathf.Sin(rad) * radius,
                    Mathf.Cos(rad) * radius - radius,
                    0
                );

                positions.Add(new CardPositionData
                {
                    position = pos,
                    rotation = Quaternion.Euler(0, 0, -angle)
                });
            }

            return positions;
        }

        float angleStep = (maxAngle * 2) / (count - 1);
        float startAngle = -maxAngle;

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 pos = new Vector3(
                Mathf.Sin(rad) * radius,
                Mathf.Cos(rad) * radius - radius,
                0
            );

            positions.Add(new CardPositionData
            {
                position = pos,
                rotation = Quaternion.Euler(0, 0, -angle)
            });
        }

        return positions;
    }

    void ClearHand()
    {
        foreach (CardView view in cardViews)
        {
            Destroy(view.gameObject);
        }

        cardViews.Clear();
    }

    struct CardPositionData
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}