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

    public void SetCreature(ICreature creature) {
        statText.text = creature.GetStatString();

        foreach (IAction action in creature.GetActions())
        {
            var entry = Instantiate(actionEntryPrefab, actionPanelContent.transform);

            entry.transform.Find("Name").GetComponent<TMP_Text>().text = action.Name;
            entry.transform.Find("Cost").GetComponent<TMP_Text>().text = action.GetCostString();
            entry.GetComponent<ActionEntryData>().SetAction(action);

            entry.SetActive(true);
        }

        foreach (IItem item in creature.Inventory.Keys) {
            var entry = Instantiate(inventoryPrefab, inventoryContent.transform);

            entry.transform.Find("Name").GetComponent<TMP_Text>().text = $"{item.Name} x{creature.Inventory[item]}";
            Button btnUse = entry.transform.Find("btnUse").GetComponent<Button>();

            if (item is IConsumable consumable) {
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
