using System;
using System.Collections;
using Assets.Scripts.Utils;
using Iterum.DTOs;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Iterum.Scripts.Utils
{
    public class UserManager : MonoBehaviour
    {
        public static UserManager Instance { get; private set; }

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
                var go = new GameObject("UserManager");
                Instance = go.AddComponent<UserManager>();
                DontDestroyOnLoad(go);
            }
        }

        private class RefreshRequest
        {
            public string refreshToken;

            public RefreshRequest(string refreshToken)
            {
                this.refreshToken = refreshToken;
            }
        }

        public void Login(LoginForm loginForm, Action<LoginResponse> onSuccess, Action<string> onFail)
        {
            if (string.IsNullOrWhiteSpace(loginForm.password))
            {
                onFail?.Invoke("Invalid password.");
                return;
            }

            if (string.IsNullOrWhiteSpace(loginForm.username))
            {
                onFail?.Invoke("Username invalid.");
                return;
            }

            StartCoroutine(LoginRequest(loginForm, onSuccess, onFail));
        }

        private IEnumerator LoginRequest(LoginForm form, Action<LoginResponse> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.Login, Methods.POST, false, form, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                LoginResponse response = JsonConvert.DeserializeObject<LoginResponse>(request.downloadHandler.text);

                SessionData.Token = "Bearer " + response.JwtToken; 
                
                SessionData.Username = response.Username;
                onSuccess?.Invoke(response);
            }
            else
            {
                onFail?.Invoke(request.error);
            }
        }

        // Create a new user
        public void CreateUser(RegisterForm registerForm, Action<LoginResponse> onSuccess, Action<string> onFail)
        {
            if (string.IsNullOrWhiteSpace(registerForm.password))
            {
                onFail?.Invoke("Invalid password.");
                return;
            }

            if (string.IsNullOrWhiteSpace(registerForm.username))
            {
                onFail?.Invoke("Username invalid.");
                return;
            }

            StartCoroutine(CreateUserRequest(registerForm, onSuccess, onFail));
        }

        private IEnumerator CreateUserRequest(RegisterForm form, Action<LoginResponse> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;

            Debug.Log(JsonUtility.ToJson(form));
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.Register, Methods.POST, false, form, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                LoginResponse response = JsonConvert.DeserializeObject<LoginResponse>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onFail?.Invoke(request.error);
            }
        }

        public void RefreshToken(string refreshToken, Action<LoginResponse> onSuccess, Action<string> onFail)
        {
            StartCoroutine(RefreshTokenRequest(refreshToken, onSuccess, onFail));
        }

        public IEnumerator RefreshTokenRequest(string refreshToken, Action<LoginResponse> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.Refresh, Methods.POST, false, new RefreshRequest(refreshToken), result => request = result);

            if (request == null)
            {
                Debug.LogError("Request was null in RefreshTokenRequest");
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(JsonConvert.DeserializeObject<LoginResponse>(request.downloadHandler.text));
                yield break;
            }
            else
            {
                PlayerPrefs.DeleteKey("refresh-token");
                onFail?.Invoke(request.error);
                yield break;
            }
        }
    }
}
