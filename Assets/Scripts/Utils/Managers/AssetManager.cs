using Iterum.DTOs;
using Iterum.Scripts.Utils;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Utils.Managers
{
    public class AssetManager : MonoBehaviour
    {
        public static AssetManager Instance { get; private set; }

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
                var go = new GameObject("AssetManager");
                Instance = go.AddComponent<AssetManager>();
                DontDestroyOnLoad(go);
            }
        }

        public void UploadImage(string path, Action onSuccess, Action<string> onFail)
        {
            StartCoroutine(UploadImageRequest(path, onSuccess, onFail));
        }

        private IEnumerator UploadImageRequest(string path, Action onSuccess, Action<string> onFail)
        {
            byte[] imageBytes = File.ReadAllBytes(path);
            string fileName = Path.GetFileName(path);

            WWWForm form = new WWWForm();
            form.AddBinaryData("image", imageBytes, fileName, "image/png");

            UnityWebRequest request = UnityWebRequest.Post(EndpointUtils.UploadImage, form);
            request.SetRequestHeader("Authorization", SessionData.Token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke();
            }
            else
            {
                onFail?.Invoke(request.error + ": " + request.downloadHandler.text);
            }
        }

        public void GetImages(Action<List<string>> onSuccess, Action<string> onFail)
        {
            StartCoroutine(GetImagesRequest(onSuccess, onFail));
        }

        private IEnumerator GetImagesRequest(Action<List<string>> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.GetImages, Methods.GET, true, null, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                List<string> response = JsonConvert.DeserializeObject<List<string>>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onFail?.Invoke(request.error);
            }
        }

        public void GetImage(string imageName, Action<Texture2D> onSuccess, Action<string> onFail)
        {
            StartCoroutine(GetImageRequest(imageName, onSuccess, onFail));
        }

        private IEnumerator GetImageRequest(string name, Action<Texture2D> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(
                EndpointUtils.GetImage(name),
                Methods.GET,
                true,
                null,
                result => request = result,
                new DownloadHandlerTexture(true)
            );

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                if (texture != null)
                    onSuccess?.Invoke(texture);
                else
                    onFail?.Invoke("Failed to retrieve texture");
            }
            else
            {
                var req = request;
                onFail?.Invoke(request.error);
            }
        }

        public void PreviewImage(string imagePath, Action<Texture2D> onSuccess, Action<string> onFail)
        {
            StartCoroutine(PreviewImageRequest(imagePath, onSuccess, onFail));
        }

        private IEnumerator PreviewImageRequest(string path, Action<Texture2D> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(
                EndpointUtils.ImagePreview(path),
                Methods.GET,
                true,
                null,
                result => request = result,
                new DownloadHandlerTexture(true)
            );

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                if (texture != null)
                    onSuccess?.Invoke(texture);
                else
                    onFail?.Invoke("Failed to retrieve texture");
            }
            else
            {
                onFail?.Invoke(request.error);
            }
        }

        public void DeleteImage(string name, Action onSuccess, Action<string> onFail)
        {
            StartCoroutine(DeleteImageRequest(name, onSuccess, onFail));
        }

        private IEnumerator DeleteImageRequest(string name, Action onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.DeleteImage(name), Methods.DELETE, true, null, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke();
            }
            else
            {
                onFail?.Invoke(request.error);
            }
        }
    }
}
