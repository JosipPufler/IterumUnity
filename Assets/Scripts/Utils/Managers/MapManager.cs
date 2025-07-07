using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Iterum.DTOs;
using System.Collections;
using Newtonsoft.Json;

namespace Iterum.Scripts.Utils.Managers
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }

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
                var go = new GameObject("MapManager");
                Instance = go.AddComponent<MapManager>();
                DontDestroyOnLoad(go);
            }
        }

        public void GetMaps(Action<List<MapDto>> onSuccess, Action<string> onFail)
        {
            StartCoroutine(GetMapsRequest(onSuccess, onFail));
        }

        private IEnumerator GetMapsRequest(Action<List<MapDto>> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.Maps, Methods.GET, true, null, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                List<MapDto> response = JsonConvert.DeserializeObject<List<MapDto>>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onFail?.Invoke(request.error);
            }
        }

        public void CreateMap(MapDto mapDto, Action<MapDto> onSuccess, Action<string> onFail)
        {
            StartCoroutine(CreateMapRequest(mapDto, onSuccess, onFail));
        }

        private IEnumerator CreateMapRequest(MapDto mapDto, Action<MapDto> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.CreateMap, Methods.POST, true, mapDto, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                MapDto response = JsonConvert.DeserializeObject<MapDto>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onFail?.Invoke(request.error);
            }
        }

        public void UpdateMap(MapDto mapDto, Action onSuccess, Action<string> onFail)
        {
            StartCoroutine(UpdateMapRequest(mapDto, onSuccess, onFail));
        }

        private IEnumerator UpdateMapRequest(MapDto mapDto, Action onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.UpdateMap, Methods.PUT, true, mapDto, result => request = result);

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

        public void DeleteMap(string mapId, Action onSuccess, Action<string> onFail)
        {
            StartCoroutine(DeleteMapRequest(mapId, onSuccess, onFail));
        }

        private IEnumerator DeleteMapRequest(string mapId, Action onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.DeleteMap(mapId), Methods.DELETE, true, null, result => request = result);

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
