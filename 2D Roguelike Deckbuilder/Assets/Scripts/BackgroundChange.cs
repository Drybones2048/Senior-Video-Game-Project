using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class BackgroundChange : MonoBehaviour
{
    [Header("Background Images")]
    public Image desert1;

    public Image desert2;
    public Image throneRoom1;
    public Image throneRoom2;

    public static UnityEvent changeBackground = new UnityEvent();

    void Awake()
    {
        desert1.gameObject.SetActive(true);
        desert2.gameObject.SetActive(false);
        throneRoom1.gameObject.SetActive(false);
        throneRoom2.gameObject.SetActive(false);
        changeBackground.AddListener(changeCurrentBackground);
    }

    void OnDestroy()
    {
        changeBackground.RemoveListener(changeCurrentBackground);
    }

    void changeCurrentBackground() // A method that is called every time RoundManager.StartNewCombat() is called, changing the background for each battle
    {
        switch (RoundManager.instance.encounterNumber)
        {
            case 1:
                desert1.gameObject.SetActive(true);
                break;
            case 2:
                desert2.gameObject.SetActive(true);
                desert1.gameObject.SetActive(false);
                break;
            case 3:
                desert2.gameObject.SetActive(false);
                throneRoom1.gameObject.SetActive(true);
                break;
            case 4:
                throneRoom1.gameObject.SetActive(false);
                throneRoom2.gameObject.SetActive(true);
                break;
            default:
                desert1.gameObject.SetActive(true);
                desert2.gameObject.SetActive(false);
                throneRoom1.gameObject.SetActive(false);
                throneRoom2.gameObject.SetActive(false);
                break;
        }
    }
}
