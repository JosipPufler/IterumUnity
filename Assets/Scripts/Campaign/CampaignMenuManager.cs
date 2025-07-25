using Assets.Scripts.Campaign;
using Assets.Scripts.GameLogic.models.interfaces;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Mirror;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampaignMenuManager : MonoBehaviour
{
    [Header("Stats")]
    public TMP_Text statText;

    [Header("Skills")]
    public GameObject skillContent;
    public GameObject skillHolderPrefab;

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
        BaseCreature creature = token.creature;
        currentCreature = token;

        InitSkills(creature);

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

        var groupedItems = creature.Inventory
            .GroupBy(item => new { itemName = item.Name, stackable = item.Stackable })
            .ToList();

        foreach (var group in groupedItems)
        {
            var firstItem = group.First();
            int count = group.Count();

            var entry = Instantiate(inventoryPrefab, inventoryContent.transform);

            string displayName = firstItem.Stackable && count > 1
                ? $"{firstItem.Name} x{count}"
                : firstItem.Name;

            entry.transform.Find("Name").GetComponent<TMP_Text>().text = displayName;

            Button btnUse = entry.transform.Find("btnUse").GetComponent<Button>();

            if (firstItem is BaseConsumable consumable)
            {
                btnUse.onClick.AddListener(() =>
                {
                    CampaignActionManager.Instance.CmdSetAction(consumable.ConsumeAction.ID);
                });
            }
            else
            {
                btnUse.gameObject.SetActive(false);
            }
        }
    }

    public void InitSkills(BaseCreature creature) {
        foreach (Transform child in skillContent.transform)
        {
            Destroy(child.gameObject);
        }
        uint netId = currentCreature.GetComponent<NetworkIdentity>().netId;
        foreach (var skill in Skill.GetAllSkills())
        {
            GameObject skillHolder = Instantiate(skillHolderPrefab, skillContent.transform);

            skillHolder.transform.Find("SkillName").GetComponent<TMP_Text>().text = skill.Name;
            string modifierText;
            int modifier = creature.GetSkillModifier(skill);
            if (modifier >= 0)
            {
                modifierText = $"(+{modifier})";
            }
            else
            {
                modifierText = $"({modifier})";
            }
            skillHolder.transform.Find("ModifierBonus").GetComponent<TMP_Text>().text = modifierText;

            skillHolder.GetComponent<Button>().onClick.AddListener(() => CampaignPlayer.LocalPlayer.CmdRollSkillcheck(netId, skill.Attribute));
        }
    }
}
