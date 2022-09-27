using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace Network
{
    [RequireComponent(typeof(RawImage))]
    public class SteamProfileDisplay : MonoBehaviour
    {
        // References
        private RawImage _image;

        // Steam callbacks
        protected Callback<AvatarImageLoaded_t> ImageLoadedCallback;

        // Private variables
        private CSteamID playerSteamId;
        private bool AvatarReceived { get; set; }

        private void Awake()
        {
            _image = GetComponent<RawImage>();
            
            ImageLoadedCallback = Callback<AvatarImageLoaded_t>.Create(OnImageLoadedSteamHandler);
        }
        
        public void Initialize(ulong steamID)
        {
            playerSteamId = (CSteamID)steamID;
            
            int imageId = SteamFriends.GetLargeFriendAvatar(playerSteamId);

            if (imageId == -1)
                return;

            _image.texture = GetSteamImageAsTexture(imageId);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                Initialize((ulong)playerSteamId);
                SteamAPI.RunCallbacks();
            }
            
        }

        private void OnImageLoadedSteamHandler(AvatarImageLoaded_t callback)
        {
            Debug.Log("Steam image received!");
            
            // If its not about our player just return
            if(callback.m_steamID != playerSteamId)
                return;

            _image.texture = GetSteamImageAsTexture(callback.m_iImage);
        }

        private Texture2D GetSteamImageAsTexture(int iImage)
        {
            Texture2D texture = null;

            bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
            if (isValid)
            {
                byte[] image = new byte[width * height * 4];

                isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

                if (isValid)
                {
                    texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                    texture.LoadRawTextureData(image);
                    texture.Apply();
                }
            }
            AvatarReceived = true;
            return texture;
        }
    }
}