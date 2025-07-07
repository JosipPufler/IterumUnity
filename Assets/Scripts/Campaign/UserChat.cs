using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UserChat : MonoBehaviour
{
    [Header("Chat")]
    public GameObject chatEntryPrefab;
    public GameObject chatContent;
    public TMP_InputField chatInput;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && EventSystem.current.currentSelectedGameObject == chatInput.gameObject && !string.IsNullOrEmpty(chatInput.text)) {
            var entry = Instantiate(chatEntryPrefab, chatContent.transform);
            entry.transform.Find("Name").GetComponent<TMP_Text>().text = $"[{DateTime.Now.ToString("HH:mm")}] {PlayerPrefs.GetString("username")}: {chatInput.text}";
            chatInput.text = "";
        }
    }
}
