using Mirror;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class HeadlessBoot : MonoBehaviour
    {
        void Start()
        {
            if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
            {
                Debug.Log("Headless build detected. Starting server...");
                NetworkManager.singleton.StartServer();
            }
        }
    }
}
