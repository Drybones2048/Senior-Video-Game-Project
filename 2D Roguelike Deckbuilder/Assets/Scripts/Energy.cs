using UnityEngine;
using TMPro;

public class Energy : MonoBehaviour
{
    public TMP_Text text;

    void Awake() {
        RoundManager.energyChanged.AddListener(displayEnergy);
    }

    void Start() {
        //only needed if the energy somehow wasn't initialized when it was created
        if (string.IsNullOrWhiteSpace(text.text))
        {
            displayEnergy(RoundManager.instance.currentEnergy);
        }
    }

    void OnDestroy() {
        RoundManager.energyChanged.RemoveListener(displayEnergy);
    }

    void displayEnergy(int currentEnergy) {
        text.text = currentEnergy + "/" + RoundManager.instance.maxEnergy;
    }
}
