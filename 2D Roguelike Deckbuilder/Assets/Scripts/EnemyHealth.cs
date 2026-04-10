using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    public Slider slider; //visual for healthbar
    public TextMeshProUGUI healthText; // will be the text that is displayed for the player's health bar

    void Awake() {
        DeathScreen.gameWon.AddListener(DisableHealthBar);
        DeathScreen.resetGame.AddListener(EnableHealthBar);
    }

    void OnDestroy() {
        DeathScreen.gameWon.RemoveListener(DisableHealthBar);
        DeathScreen.resetGame.RemoveListener(EnableHealthBar);
    }

    void DisableHealthBar() {
        slider.gameObject.SetActive(false);
        healthText.gameObject.SetActive(false);
    }

    void EnableHealthBar() {
        slider.gameObject.SetActive(true);
        healthText.gameObject.SetActive(true);
    }

    public void setMaxHealth(int health){ // sets the max health to the passed in value
        healthText.text = health.ToString() + "/" + health.ToString(); // displays the initial health value of the player
        
        slider.maxValue = health;
        slider.value = health; 
    }

    public void setHealth(int health){ //sets the health value every time it is changed in combat
        healthText.text = health.ToString() + "/" + slider.maxValue.ToString();
        
        slider.value = health;
    }
}
