using Iterum.DTOs;
using Iterum.Scripts.Utils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class JournalEdit : MonoBehaviour
{
    [Header("Panels")]
    public GameObject editPanel;
    public GameObject listPanel;

    [Header("Fields")]
    public TMP_Text lblTitle;
    public TMP_InputField input;

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            editPanel.SetActive(false);
            listPanel.SetActive(true);
        }

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
            Input.GetKeyDown(KeyCode.S))
        {
            JournalManager.Instance.SaveJournal(new JournalDto(lblTitle.text, input.text), null, null);
        }
    }
}
