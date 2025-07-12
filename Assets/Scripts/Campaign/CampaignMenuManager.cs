using Iterum.models.interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampaignMenuManager : MonoBehaviour
{
    [Header("Stats")]
    public TMP_Text statText;

    [Header("Inventory")]
    public GameObject inventoryContent;
    public GameObject inventoryPrefab;

    [Header("Actions")]
    public GameObject actionPanelContent;
    public GameObject actionEntryPrefab;

    ICreature currentCreature;

    private void Update()
    {
        if (currentCreature != null)
        {
            statText.text = currentCreature.GetStatString();
        }
    }

    public void SetCreature(ICreature creature) {
        currentCreature = creature;

        foreach (Transform child in inventoryContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in actionPanelContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (IAction action in currentCreature.GetActions())
        {
            var entry = Instantiate(actionEntryPrefab, actionPanelContent.transform);

            entry.transform.Find("Name").GetComponent<TMP_Text>().text = action.Name;
            entry.transform.Find("Cost").GetComponent<TMP_Text>().text = action.GetCostString();
            entry.GetComponent<ActionEntryData>().SetAction(action);

            entry.SetActive(true);
        }

        foreach (IItem item in currentCreature.Inventory.Keys)
        {
            var entry = Instantiate(inventoryPrefab, inventoryContent.transform);

            entry.transform.Find("Name").GetComponent<TMP_Text>().text = $"{item.Name} x{currentCreature.Inventory[item]}";
            Button btnUse = entry.transform.Find("btnUse").GetComponent<Button>();

            if (item is IConsumable consumable)
            {
                // To do
                btnUse.onClick.AddListener(() => {

                    consumable.Consume(null, null);
                });
            }
            else
            {
                btnUse.gameObject.SetActive(false);
            }
        }
    }
}
