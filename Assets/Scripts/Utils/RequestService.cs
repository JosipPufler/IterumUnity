// Assets/Scripts/Utils/RequestService.cs
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Iterum.Scripts.Utils
{
    public static class RequestService
    {
        private sealed class RequestServiceHost : MonoBehaviour { }
        private static RequestServiceHost _host;


        private static void EnsureHost()
        {
            if (_host != null) return;

            var go = new GameObject("[RequestService]");
            UnityEngine.Object.DontDestroyOnLoad(go);
            _host = go.AddComponent<RequestServiceHost>();
        }

#nullable enable
        public static IEnumerator ConstructSimpleWebRequest(
            string endpoint,
            Methods method,
            bool requiresAuthorization,
            object? body,
            Action<UnityWebRequest?> onReady,
            DownloadHandler downloadHandler = null)
        {
            EnsureHost();

            var request = new UnityWebRequest(endpoint, method.ToString())
            {
                downloadHandler = downloadHandler ?? new DownloadHandlerBuffer()
            };

            if (body != null)
            {
                request.uploadHandler = new UploadHandlerRaw(
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body)));
                request.SetRequestHeader("Content-Type", "application/json");
            }
            else
                request.uploadHandler = new UploadHandlerRaw(Array.Empty<byte>());

            

            if (requiresAuthorization)
                request.SetRequestHeader("Authorization", PlayerPrefs.GetString("token"));

            onReady?.Invoke(request);
            yield break;
        }
#nullable disable
    } 
}