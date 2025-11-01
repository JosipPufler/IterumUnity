using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Utils.Managers;
using Iterum.Scripts.Utils;
using SFB;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu
{
    public class AssetList : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject pnlMainMenu;
        public GameObject pnlAssetList;
        public RawImage previewImage;

        [Header("Template")]
        public GameObject entryTemplate;

        [Header("Field")]
        public GameObject content;
        public TMP_InputField imageName;
        public Button uploadButton;

        private Dictionary<string, GameObject> entries = new();

        void Start()
        {
            previewImage.gameObject.SetActive(false);
            uploadButton.onClick.AddListener(OpenFileDialogAndUpload);
            imageName.onValueChanged.AddListener(name => {
                foreach (var item in entries)
                {
                    if (!item.Key.Contains(name))
                    {
                        item.Value.SetActive(false);
                    }
                    else
                    {
                        item.Value.SetActive(true);
                    }
                }
            });
            ReloadAssets();
        }

        public void OpenFileDialogAndUpload()
        {
            string[] paths = StandaloneFileBrowser.OpenFilePanel("Select an Image", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), new[] {
                new ExtensionFilter("Image Files", "png", "jpg", "jpeg")
            }, false);

            if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
            {
                AssetManager.Instance.UploadImage(paths[0], ReloadAssets, error => Debug.Log(error));
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (previewImage.IsActive())
                {
                    previewImage.gameObject.SetActive(false);
                    pnlAssetList.SetActive(true);
                }
                else
                {
                    pnlMainMenu.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
        }

        void ReloadAssets()
        {
            AssetManager.Instance.GetImages((journals) => {
                foreach (var item in entries.Values) {
                    Destroy(item);
                }
                
                entries.Clear();
                foreach (string journalName in journals)
                {
                    AddEntry(journalName);
                }
            }, OnError);
        }

        void AddEntry(string name)
        {
            GameObject entry = Instantiate(entryTemplate, content.transform);
            Transform leftGroup = entry.transform.Find("LeftGroup");

            leftGroup.transform.Find("Name").GetComponent<TMP_Text>().text = name;

            var buttonHolder = entry.transform.Find("RightGroup");
            buttonHolder.Find("btnShow").GetComponent<Button>()
                .onClick.AddListener(() => LoadImage(name));
            buttonHolder.Find("btnDelete").GetComponent<Button>()
                .onClick.AddListener(() => DeleteImage(name, entry.transform));
            entry.SetActive(true);
            entries.Add(name, entry);
        }

        void LoadImage(string name)
        {
            AssetManager.Instance.GetImage(name, imageTexture => {
                previewImage.texture = imageTexture;

                float maxWidth = 1920f;
                float maxHeight = 1080f;

                float imageWidth = imageTexture.width;
                float imageHeight = imageTexture.height;

                float widthRatio = maxWidth / imageWidth;
                float heightRatio = maxHeight / imageHeight;

                float scale = Mathf.Min(widthRatio, heightRatio);

                float width = imageWidth * scale;
                float height = imageHeight * scale;

                RectTransform rectTransform = previewImage.rectTransform;
                rectTransform.sizeDelta = new Vector2(width, height);

                pnlAssetList.SetActive(false);
                previewImage.gameObject.SetActive(true);
            }, OnError);
        }

        void DeleteImage(string name, Transform transform)
        {
            AssetManager.Instance.DeleteImage(name, () => {
                ReloadAssets();
                Destroy(transform.gameObject);
            }, OnError);
        }
        void OnError(string error)
        {
            Debug.Log(error);
        }
    }
}
