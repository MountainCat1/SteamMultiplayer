using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerEntry : MonoBehaviour
{
    [Header("UI Elements References")]
    [SerializeField] private TextMeshProUGUI playerNameDisplay;
    [SerializeField] private RawImage playerAvatarDisplay;
    
    // Variables
    private Player _player;

    public void Initialize(Player player)
    {
        playerNameDisplay.text = player.Name.Value.ToString();
    }
}
