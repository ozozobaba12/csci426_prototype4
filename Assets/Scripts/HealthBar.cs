using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;
    public PlayerController player;

    void Update()
    {
        if (player)
            fillImage.fillAmount = (float)player.health / player.maxHealth;
    }
}