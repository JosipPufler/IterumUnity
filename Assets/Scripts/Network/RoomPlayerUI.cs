using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Network
{
    public class RoomPlayerUI : MonoBehaviour
    {
        public Text playerNameLabel;
        public Text characterNameLabel;
        public RawImage characterImage;

        public void SetName(string name) { 
            playerNameLabel.text = name;
        }

        public void SetCharacterName(string name)
        {
            characterNameLabel.text = name;
        }

        public void SetCharacterImage(string imagePath) {
            Debug.Log(imagePath);
            TextureMemorizer.LoadTexture(imagePath, texture => characterImage.texture = texture);
        }
    }
}
