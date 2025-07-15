using Assets.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Iterum.Scripts.Utils.Managers
{
    public class ActionManager : MonoBehaviour
    {
        public static ActionManager Instance { get; private set; }

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
                var go = new GameObject("ActionManager");
                Instance = go.AddComponent<ActionManager>();
                DontDestroyOnLoad(go);
            }
        }

        public void GetActions(Action<List<ActionDto>> onSuccess, Action<string> onFail)
        {
            StartCoroutine(GetActionsRequest(onSuccess, onFail));
        }

        private IEnumerator GetActionsRequest(Action<List<ActionDto>> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.Actions, Methods.GET, true, null, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                List<ActionDto> response = JsonConvert.DeserializeObject<List<ActionDto>>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onFail?.Invoke(request.error);
            }
        }

        public void CreateAction(ActionDto actionDto, Action<ActionDto> onSuccess, Action<string> onFail)
        {
            StartCoroutine(CreateActionRequest(actionDto, onSuccess, onFail));
        }

        private IEnumerator CreateActionRequest(ActionDto actionDto, Action<ActionDto> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.CreateAction, Methods.POST, true, actionDto, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ActionDto response = JsonConvert.DeserializeObject<ActionDto>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                var newReq = request;
                onFail?.Invoke(request.error);
            }
        }

        public void UpdateAction(ActionDto actionDto, Action onSuccess, Action<string> onFail)
        {
            StartCoroutine(UpdateActionRequest(actionDto, onSuccess, onFail));
        }

        private IEnumerator UpdateActionRequest(ActionDto characterDto, Action onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.UpdateAction, Methods.PUT, true, characterDto, result => request = result);

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

        public void DeleteAction(string actionId, Action onSuccess, Action<string> onFail)
        {
            StartCoroutine(DeleteActionRequest(actionId, onSuccess, onFail));
        }

        private IEnumerator DeleteActionRequest(string actionId, Action onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.DeleteAction(actionId), Methods.DELETE, true, null, result => request = result);

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
