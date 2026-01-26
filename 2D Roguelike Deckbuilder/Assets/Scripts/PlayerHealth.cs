using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public Slider slider; //adjusts the visual slider for the player's health bar
    public TextMeshProUGUI healthText; // will be the text that is displayed for the player's health bar

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
