using Assets.DTOs;
using Iterum.DTOs;
using Iterum.Scripts.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Utils.Managers
{
    public class SessionManager : MonoBehaviour
    {
        public static SessionManager Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static void EnsureExists()
        {
            if (Instance == null)
            {
                var go = new GameObject("SessionManager");
                Instance = go.AddComponent<SessionManager>();
                DontDestroyOnLoad(go);
            }
        }

        public void CreateSession(Action<SessionDto> onSuccess, Action<string> onFail)
        {
            StartCoroutine(CreateSessionRequest(onSuccess, onFail));
        }

        private IEnumerator CreateSessionRequest(Action<SessionDto> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.CreateSession, Methods.POST, true, null, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                SessionDto response = JsonConvert.DeserializeObject<SessionDto>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onFail?.Invoke(request.error);
            }
        }

        public void EndSession(Action<SessionDto> onSuccess, Action<string> onFail)
        {
            StartCoroutine(EndSessionRequest(onSuccess, onFail));
        }

        private IEnumerator EndSessionRequest(Action<SessionDto> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.EndSession, Methods.POST, true, null, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                SessionDto response = JsonConvert.DeserializeObject<SessionDto>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onFail?.Invoke(request.error);
            }
        }

        public void JoinSession(string sessionCode, Action<SessionDto> onSuccess, Action<string> onFail)
        {
            StartCoroutine(JoinSessionRequest(sessionCode, onSuccess, onFail));
        }

        private IEnumerator JoinSessionRequest(string sessionCode, Action<SessionDto> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.JoinSession(sessionCode), Methods.POST, true, null, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                SessionDto response = JsonConvert.DeserializeObject<SessionDto>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onFail?.Invoke(request.error);
            }
        }
    }
}
