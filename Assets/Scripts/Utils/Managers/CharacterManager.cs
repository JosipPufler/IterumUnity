using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Iterum.DTOs;
using System.Collections;
using Newtonsoft.Json;
using Assets.DTOs;

namespace Iterum.Scripts.Utils.Managers
{
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager Instance { get; private set; }

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
                var go = new GameObject("CharacterManager");
                Instance = go.AddComponent<CharacterManager>();
                DontDestroyOnLoad(go);
            }
        }

        public void GetCharacters(Action<List<CharacterDto>> onSuccess, Action<string> onFail)
        {
            StartCoroutine(GetCharactersRequest(onSuccess, onFail));
        }

        private IEnumerator GetCharactersRequest(Action<List<CharacterDto>> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.Characters, Methods.GET, true, null, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                List<CharacterDto> response = JsonConvert.DeserializeObject<List<CharacterDto>>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onFail?.Invoke(request.error);
            }
        }

        public void CreateCharacter(CharacterDto characterDto, Action<CharacterDto> onSuccess, Action<string> onFail)
        {
            StartCoroutine(CreateCharacterRequest(characterDto, onSuccess, onFail));
        }

        private IEnumerator CreateCharacterRequest(CharacterDto characterDto, Action<CharacterDto> onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.CreateCharacter, Methods.POST, true, characterDto, result => request = result);

            if (request == null)
            {
                onFail?.Invoke("Failed to construct request");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                CharacterDto response = JsonConvert.DeserializeObject<CharacterDto>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onFail?.Invoke(request.error);
            }
        }

        public void UpdateCharacter(CharacterDto characterDto, Action onSuccess, Action<string> onFail)
        {
            StartCoroutine(UpdateCharacterRequest(characterDto, onSuccess, onFail));
        }

        private IEnumerator UpdateCharacterRequest(CharacterDto characterDto, Action onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.UpdateCharacter, Methods.PUT, true, characterDto, result => request = result);

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

        public void DeleteCharacter(string characterId, Action onSuccess, Action<string> onFail)
        {
            StartCoroutine(DeleteCharacterRequest(characterId, onSuccess, onFail));
        }

        private IEnumerator DeleteCharacterRequest(string characterId, Action onSuccess, Action<string> onFail)
        {
            UnityWebRequest request = null;
            yield return RequestService.ConstructSimpleWebRequest(EndpointUtils.DeleteCharacter(characterId), Methods.DELETE, true, null, result => request = result);

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
