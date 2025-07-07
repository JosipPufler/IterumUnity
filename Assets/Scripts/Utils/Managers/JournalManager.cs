using Iterum.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Iterum.Scripts.Utils
{
    public class JournalManager : MonoBehaviour
    {
        public static JournalManager Instance { get; private set; }

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
                var go = new GameObject("JournalManager");
                Instance = go.AddComponent<JournalManager>();
                DontDestroyOnLoad(go);
            }
        }

        public void GetJournals(Action<IEnumerable<string>> onSuccess, Action<string> onFail)
        {
            StartCoroutine(GetJournalsRequest(onSuccess, onFail));
        }

        private IEnumerator GetJournalsRequest(Action<IEnumerable<string>> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.GetJournals, Methods.GET, true, null, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                IEnumerable<string> response = JsonConvert.DeserializeObject<IEnumerable<string>>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onFail?.Invoke(request.error);
            }
        }

        public void PreviewJournal(string path, Action<JournalDto> onSuccess, Action<string> onFail)
        {
            StartCoroutine(PreviewJournalRequest(path, onSuccess, onFail));
        }

        private IEnumerator PreviewJournalRequest(string path, Action<JournalDto> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.JournalPreview(path), Methods.GET, true, null, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                JournalDto response = JsonConvert.DeserializeObject<JournalDto>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onFail?.Invoke(request.error);
            }
        }

        public void GetJournal(string journalName, Action<JournalDto> onSuccess, Action<string> onFail)
        {
            StartCoroutine(GetJournalRequest(journalName, onSuccess, onFail));
        }

        private IEnumerator GetJournalRequest(string name, Action<JournalDto> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.GetJournal(name), Methods.GET, true, null, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                JournalDto response = JsonConvert.DeserializeObject<JournalDto>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onFail?.Invoke(request.error);
            }
        }

        public void DeleteJournal(string journalName, Action onSuccess, Action<string> onFail)
        {
            StartCoroutine(DeleteJournalRequest(journalName, onSuccess, onFail));
        }

        private IEnumerator DeleteJournalRequest(string name, Action onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.DeleteJournal(name), Methods.DELETE, true, null, result => request = result);

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

        public void SaveJournal(JournalDto journal, Action onSuccess, Action<string> onFail)
        {
            StartCoroutine(SaveJournalRequest(journal, onSuccess, onFail));
        }

        private IEnumerator SaveJournalRequest(JournalDto journal, Action onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.SaveJournal, Methods.POST, true, journal, result => request = result);

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
