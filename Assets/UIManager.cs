using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI HealthBar;
    public TextMeshProUGUI Speedometer;
    public PlayerMovement player;

    private void Update()
    {
        // Update speedometer
        Speedometer.text = "Speed: " + Mathf.Round(player.GetSpeed());
    }

    public void UpdateHealth(float healthPercent)
    {
        HealthBar.text = healthPercent.ToString();
    }
}
