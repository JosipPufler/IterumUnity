using Assets.Scripts.Utils;
using Iterum.DTOs;
using Iterum.Scripts.Utils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu
{
    public class JournalList : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject editPanel;
        public GameObject listPanel;
        public GameObject previewPanel;
        public GameObject mainMeuePanel;

        [Header("List fields")]
        public GameObject entryTemplate;
        public GameObject content;
        public TMP_InputField entryName;
        public TMP_InputField previewPath;

        [Header("Buttons")]
        public Button btnCreate;
        public Button btnPreview;

        [Header("Preview fields")]
        public TMP_Text previewText;

        [Header("Edit fields")]
        public TMP_InputField journalEditArea;
        public TMP_Text journalTitle;
        public TMP_Text rendered;

        void Start()
        {
            btnCreate.onClick.AddListener(() => {
                if (!string.IsNullOrEmpty(entryName.text))
                {
                    var text = entryName.text;
                    JournalManager.Instance.SaveJournal(new JournalDto(entryName.text, ""), () => AddEntry(text), OnError);
                    entryName.text = "";
                }
            });
            btnPreview.onClick.AddListener(() => {
                if (!string.IsNullOrEmpty(previewPath.text))
                {
                    JournalManager.Instance.PreviewJournal(previewPath.text, (JournalDto) => { 
                        previewText.text = MarkdownService.Convert(JournalDto.Content);
                        previewPanel.SetActive(true);
                        listPanel.SetActive(false);
                    }, OnError);
                    previewPath.text = "";
                }
            });

            JournalManager.Instance.GetJournals(ReloadJournals, OnError);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (previewPanel.activeSelf)
                {
                    previewPanel.SetActive(false);
                    listPanel.SetActive(true);
                }
                else
                {
                    mainMeuePanel.SetActive(true);
                    listPanel.SetActive(false);
                }
            }
        }

        void ReloadJournals(IEnumerable<string> journals)
        {
            foreach (Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (string journalName in journals)
            {
                AddEntry(journalName);
            }
        }

        void AddEntry(string name)
        {
            var entry = Instantiate(entryTemplate, content.transform);
            Transform leftGroup = entry.transform.Find("LeftGroup");

            leftGroup.transform.Find("Name").GetComponent<TMP_Text>().text = name;

            var buttonHolder = entry.transform.Find("RightGroup");
            buttonHolder.Find("btnEdit").GetComponent<Button>()
                .onClick.AddListener(() => LoadJournal(name));
            buttonHolder.Find("btnDelete").GetComponent<Button>()
                .onClick.AddListener(() => DeleteJournal(name, entry.transform));
            buttonHolder.Find("btnShare").GetComponent<Button>()
                .onClick.AddListener(() => GUIUtility.systemCopyBuffer = $"{SessionData.Username}/journal/{name}.txt");

            entry.SetActive(true);
        }

        void LoadJournal(string name)
        {
            JournalManager.Instance.GetJournal(name, journalDto => { 
                journalEditArea.text = journalDto.Content;
                journalTitle.text = journalDto.Name;
                rendered.text = MarkdownService.Convert(journalDto.Content);

                gameObject.SetActive(false);
                editPanel.SetActive(true);
            }, OnError);
        }

        void DeleteJournal(string name, Transform transform)
        {
            JournalManager.Instance.DeleteJournal(name, () => {
                JournalManager.Instance.GetJournals(ReloadJournals, OnError);
                Destroy(transform.gameObject);
            }, OnError);
        }

        void OnError(string error)
        {
            Debug.Log(error);
        }
    }
}
