using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CooldownUI : MonoBehaviour
{
    public GameObject cooldownPrefab;
    public Transform cooldownContainer;

    private Dictionary<string, GameObject> activeCooldowns = new Dictionary<string, GameObject>();

    private void Start()
    {
        FindObjectOfType<CooldownSystem>().CooldownUpdated += UpdateCooldownUI;
        FindObjectOfType<CooldownSystem>().CooldownEnded += RemoveCooldown;
    }

    public void AddCooldown(string abilityName, float duration, Sprite icon)
    {
        if (activeCooldowns.ContainsKey(abilityName)) return;

        GameObject cooldownUI = Instantiate(cooldownPrefab, cooldownContainer);
        cooldownUI.transform.Find("Icon").GetComponent<Image>().sprite = icon;
        activeCooldowns[abilityName] = cooldownUI;
        ReorderCooldowns();
    }

    public void AddCooldown(string abilityName, Sprite icon)
    {
        if (activeCooldowns.ContainsKey(abilityName))
        {
            UpdateCooldownUI(abilityName, 100f);
            return;
        }

        GameObject cooldownUI = Instantiate(cooldownPrefab, cooldownContainer);
        cooldownUI.transform.Find("Icon").GetComponent<Image>().sprite = icon;
        activeCooldowns[abilityName] = cooldownUI;
        UpdateCooldownUI(abilityName, 100f);
        ReorderCooldowns();
    }

    private void UpdateCooldownUI(string abilityName, float percentage)
    {
        if (activeCooldowns.ContainsKey(abilityName))
        {
            activeCooldowns[abilityName].transform.Find("Fill").GetComponent<Image>().fillAmount = percentage;
        }
    }

    private void RemoveCooldown(string abilityName)
    {
        if (activeCooldowns.ContainsKey(abilityName))
        {
            Destroy(activeCooldowns[abilityName]);
            activeCooldowns.Remove(abilityName);
            ReorderCooldowns();
        }
    }

    private void ReorderCooldowns()
    {
        int index = 0;
        foreach (var cooldown in activeCooldowns.Values)
        {
            RectTransform rect = cooldown.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = new Vector2(-index * 70, 0);
            }
            index++;
        }
    }
}
