using Network;
using TMPro;
using UnityEngine;

public class UIPlayerEntry : MonoBehaviour
{
    [Header("UI Elements References")]
    [SerializeField] private TextMeshProUGUI playerNameDisplay;
    [SerializeField] private SteamProfileDisplay steamProfileDisplay;
    
    // Variables
    private ClientPlayer _clientPlayer;

    public void Initialize(ClientPlayer clientPlayer)
    {
        playerNameDisplay.text = clientPlayer.Name.Value.ToString();
        steamProfileDisplay.Initialize(clientPlayer.CSteamId.Value);
    }
}
