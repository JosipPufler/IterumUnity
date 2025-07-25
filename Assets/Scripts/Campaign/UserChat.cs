using Assets.Scripts.Campaign;
using Mirror;
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
        if (Input.GetKeyDown(KeyCode.Return) &&
            EventSystem.current.currentSelectedGameObject == chatInput.gameObject &&
            !string.IsNullOrEmpty(chatInput.text))
        {
            var player = NetworkClient.connection.identity.GetComponent<CampaignPlayer>();
            string username = PlayerPrefs.GetString("username");
            player.CmdSendChatMessage(username, chatInput.text);
            chatInput.text = "";
        }
    }

    public void AddEntry(string content, bool timestamp) {
        var entry = Instantiate(chatEntryPrefab, chatContent.transform);
        if (timestamp)
        {
            content = $"[{DateTime.Now:HH:mm}] " + content;
        }
        entry.transform.Find("Text").GetComponent<TMP_Text>().text = content;
    }
}
