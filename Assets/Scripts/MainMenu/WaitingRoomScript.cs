using Assets.Scripts.Network;
using Iterum.Scripts.UI;
using Mirror;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu
{
    public class WaitingRoomScript : MonoBehaviour
    {
        public TMP_Text lblCode;
        public Button btnStart;

        private void Start()
        {
            if (GameManager.Instance.Session.Host != PlayerPrefs.GetString("username")) { 
                btnStart.gameObject.SetActive(false);
            }
            else
            {
                btnStart.onClick.AddListener(() => {
                    var localRoomPlayer = NetworkClient.connection.identity.GetComponent<CustomRoomPlayer>();
                    if (localRoomPlayer != null)
                    {
                        Debug.Log("Sending CmdStartGame");
                        localRoomPlayer.CmdStartGame();
                    }
                });
            }

            lblCode.text = GameManager.Instance.Session.SessionCode;
#if UNITY_EDITOR
            lblCode.GetComponent<Button>().onClick.AddListener(() => {
                EditorGUIUtility.systemCopyBuffer = lblCode.text;
            });
#endif
        }
    }
}
