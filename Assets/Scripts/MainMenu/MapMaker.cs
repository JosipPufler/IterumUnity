using System.Collections.Generic;
using Iterum.DTOs;
using Iterum.Scripts.Map;
using Iterum.Scripts.UI;
using Iterum.Scripts.Utils.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapMaker : MonoBehaviour
{
    public GameObject entryTemplate;
    public GameObject content;
    public TMP_InputField ifMapName;
    public Button btnCreate;

    public GameObject pnlMapMaker;
    public GameObject pnlMain;

    void Start()
    {
        btnCreate.onClick.AddListener(() => {
            if (!string.IsNullOrEmpty(ifMapName.text)) {
                MapDto newMap = new()
                {
                    Name = ifMapName.text,
                    Hexes = new(),
                    IsFlatTopped = true,
                    MaxX = 20,
                    MaxY = 20,
                };
                MapManager.Instance.CreateMap(newMap, AddEntry, OnError);
                ifMapName.text = "";
            }
        });

        MapManager.Instance.GetMaps(ReloadMaps, OnError);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pnlMain.SetActive(true);
            pnlMapMaker.SetActive(false);
        }
    }

    void ReloadMaps(IEnumerable<MapDto> maps)
    {
        foreach (MapDto map in maps)
        {
            AddEntry(map);
        }
    }

    void AddEntry(MapDto map) {
        var entry = Instantiate(entryTemplate, content.transform);
        Transform leftGroup = entry.transform.Find("LeftGroup");
        
        leftGroup.transform.Find("Name").GetComponent<TMP_Text>().text = map.Name;
        
        var buttonHolder = entry.transform.Find("RightGroup");
        buttonHolder.Find("btnEdit").GetComponent<Button>()
            .onClick.AddListener(() => LoadMapToEdit(map));
        buttonHolder.Find("btnDelete").GetComponent<Button>()
            .onClick.AddListener(() => DeleteMap(map, entry.transform));

        entry.SetActive(true);
    }

    void LoadMapToEdit(MapDto map) { 
        GameManager.Instance.SelectedMap = map;
        SceneManager.LoadScene("MapMaker");
    }

    void DeleteMap(MapDto map, Transform transform) {
        MapManager.Instance.DeleteMap(map.Id, () => {
            MapManager.Instance.GetMaps(ReloadMaps, OnError);
            Destroy(transform.gameObject);
        }, OnError);
    }

    void OnError(string error) { 
        Debug.Log(error);
    }
}
