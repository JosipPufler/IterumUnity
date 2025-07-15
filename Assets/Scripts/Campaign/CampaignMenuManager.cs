using Assets.Scripts.Campaign;
using Iterum.models.interfaces;
using TMPro;
using UnityEditor.Experimental.GraphView;
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

    public static CharacterToken currentCreature;

    private void Update()
    {
        if (currentCreature != null)
        {
            statText.text = currentCreature.creature.GetStatString();
        }
    }

    public void SetCreature(CharacterToken token) {
        ICreature creature = token.creature;
        currentCreature = token;

        foreach (Transform child in inventoryContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in actionPanelContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (IAction action in creature.GetActions())
        {
            var entry = Instantiate(actionEntryPrefab, actionPanelContent.transform);

            entry.transform.Find("Name").GetComponent<TMP_Text>().text = action.Name;
            entry.transform.Find("Cost").GetComponent<TMP_Text>().text = action.GetCostString();
            entry.GetComponent<ActionEntryData>().SetAction(action);

            entry.SetActive(true);
        }

        foreach (IItem item in creature.Inventory.Keys)
        {
            var entry = Instantiate(inventoryPrefab, inventoryContent.transform);

            entry.transform.Find("Name").GetComponent<TMP_Text>().text = $"{item.Name} x{creature.Inventory[item]}";
            Button btnUse = entry.transform.Find("btnUse").GetComponent<Button>();

            if (item is IConsumable consumable)
            {
                btnUse.onClick.AddListener(() => {
                    CampaignActionManager.Instance.SetAction(consumable.ConsumeAction, currentCreature, (actionInfo) => consumable.Consume(creature.Inventory, actionInfo));
                });
            }
            else
            {
                btnUse.gameObject.SetActive(false);
            }
        }
    }
}
