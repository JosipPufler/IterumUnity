namespace Assets.Scripts.Utils
{
    using Mirror;
    using UnityEditor;
    using UnityEngine;

    public class NetworkIdentityLogger : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Tools/Log NetworkIdentity Scene IDs")]
        static void LogSceneNetworkIdentities()
        {
            var identities = FindObjectsByType<NetworkIdentity>(FindObjectsSortMode.None);
            foreach (var identity in identities)
            {
                if (identity.sceneId != 0)
                {
                    Debug.Log($"{identity.gameObject.name} has sceneId: {identity.sceneId.ToString("X")}", identity.gameObject);
                }
            }
        }
#endif
    }
}
