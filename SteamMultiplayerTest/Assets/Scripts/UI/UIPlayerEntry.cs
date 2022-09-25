using System.Collections;
using System.Collections.Generic;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerEntry : MonoBehaviour
{
    [Header("UI Elements References")]
    [SerializeField] private TextMeshProUGUI playerNameDisplay;
    [SerializeField] private SteamProfileDisplay steamProfileDisplay;
    
    // Variables
    private Player _player;

    public void Initialize(Player player)
    {
        playerNameDisplay.text = player.Name.Value.ToString();
        steamProfileDisplay.Initialize(player.CSteamId.Value);
    }
}
