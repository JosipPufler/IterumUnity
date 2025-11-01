// Assets/Scripts/Utils/ItemManager.cs
using Assets.DTOs;
using Iterum.Scripts.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Utils.Managers
{
    public class ItemManager : MonoBehaviour
    {
        public static ItemManager Instance { get; private set; }

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
                var go = new GameObject("ItemManager");
                Instance = go.AddComponent<ItemManager>();
                DontDestroyOnLoad(go);
            }
        }

        public void GetItem(string id, Action<ItemDto> onSuccess, Action<string> onFail)
        {
            StartCoroutine(GetItemRequest(id, onSuccess, onFail));
        }

        private IEnumerator GetItemRequest(string id, Action<ItemDto> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.ItemById(id), Methods.GET, false, null, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ItemDto response = JsonConvert.DeserializeObject<ItemDto>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onFail?.Invoke(request.error);
            }
        }

        public void GetItems(Action<List<ItemDto>> onSuccess, Action<string> onFail)
        {
            StartCoroutine(GetItemsRequest(onSuccess, onFail));
        }

        private IEnumerator GetItemsRequest(Action<List<ItemDto>> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.Items, Methods.GET, true, null, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                List<ItemDto> response = JsonConvert.DeserializeObject<List<ItemDto>>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onFail?.Invoke(request.error);
            }
        }

        public void CreateItem(ItemDto itemDto, Action<ItemDto> onSuccess, Action<string> onFail)
        {
            StartCoroutine(CreateItemRequest(itemDto, onSuccess, onFail));
        }

        private IEnumerator CreateItemRequest(ItemDto itemDto, Action<ItemDto> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.CreateItem, Methods.POST, true, itemDto, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ItemDto response = JsonConvert.DeserializeObject<ItemDto>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                var newReq = request;
                onFail?.Invoke(request.error);
            }
        }

        public void UpdateItem(ItemDto itemDto, Action onSuccess, Action<string> onFail)
        {
            StartCoroutine(UpdateItemRequest(itemDto, onSuccess, onFail));
        }

        private IEnumerator UpdateItemRequest(ItemDto itemDto, Action onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.UpdateItem, Methods.PUT, true, itemDto, result => request = result);

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

        public void DeleteItem(string itemId, Action onSuccess, Action<string> onFail)
        {
            StartCoroutine(DeleteItemRequest(itemId, onSuccess, onFail));
        }

        private IEnumerator DeleteItemRequest(string itemId, Action onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.DeleteItem(itemId), Methods.DELETE, true, null, result => request = result);

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
