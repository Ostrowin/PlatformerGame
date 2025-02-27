using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CooldownManager : MonoBehaviour
{
    public GameObject cooldownPrefab; // Prefab pojedynczego cooldownu
    public Transform cooldownContainer; // Kontener na cooldowny

    private Dictionary<string, GameObject> activeCooldowns = new Dictionary<string, GameObject>();

    public void StartCooldown(string abilityName, float duration, Sprite icon)
    {
        if (activeCooldowns.ContainsKey(abilityName)) 
        {
            // Debug.Log($"⚠️ Cooldown {abilityName} już istnieje, nie dodajemy ponownie!");
            return; 
        }

        GameObject cooldownUI = Instantiate(cooldownPrefab, cooldownContainer);
        Transform fillTransform = cooldownUI.transform.Find("Fill");
        cooldownUI.transform.Find("Icon").GetComponent<Image>().sprite = icon;
        if (fillTransform == null)
        {
            // Debug.LogError("❌ Nie znaleziono 'Fill' w CooldownPrefab! Sprawdź nazwę!");
            return;
        }

        Image fillImage = fillTransform.GetComponent<Image>();

        // 🔥 Ustawianie pozycji nowego cooldownu obok poprzednich
        int cooldownCount = activeCooldowns.Count;
        cooldownUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-cooldownCount * 70, 0);

        activeCooldowns[abilityName] = cooldownUI;

        // Debug.Log($"✅ Cooldown {abilityName} rozpoczęty! Aktywne cooldowny przed dodaniem: {activeCooldowns.Count - 1}");
        // Debug.Log($"🔥 Po dodaniu: {activeCooldowns.Count}");

        StartCoroutine(UpdateCooldown(abilityName, fillImage, duration));
    }

    public void StartCooldown(string abilityName, Sprite icon)
    {
        if (activeCooldowns.ContainsKey(abilityName)) return;

        GameObject cooldownUI = Instantiate(cooldownPrefab, cooldownContainer);
        cooldownUI.transform.Find("Icon").GetComponent<Image>().sprite = icon;
        activeCooldowns[abilityName] = cooldownUI;

        ReorderCooldowns(); // 🔄 Przesuń resztę cooldownów
    }


    public IEnumerator UpdateCooldown(string abilityName, Image fillImage, float duration)
    {
        float timePassed = 0;
        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            fillImage.fillAmount = 1 - (timePassed / duration);
            yield return null;
        }

        // Debug.Log($"🔥 Cooldown {abilityName} zakończony! Usuwanie tylko tego cooldownu.");

        // 🔥 Sprawdź, czy cooldown nadal istnieje w Dictionary
        if (activeCooldowns.TryGetValue(abilityName, out GameObject cooldownUI))
        {
            activeCooldowns.Remove(abilityName); // 🛠️ Usuń z listy przed Destroy()
            Destroy(cooldownUI);
            // Debug.Log($"🗑️ Usunięto cooldown {abilityName}. Aktywne cooldowny po usunięciu: {activeCooldowns.Count}");
        }

        ReorderCooldowns(); // 🔄 Przesuń resztę cooldownów
    }

    public void UpdateCooldown(string abilityName, float percentage)
    {
        if (activeCooldowns.ContainsKey(abilityName))
        {
            activeCooldowns[abilityName].transform.Find("Fill").GetComponent<Image>().fillAmount = percentage;
        }
    }

    private void ReorderCooldowns()
    {
        int index = 0;
        foreach (var cooldown in activeCooldowns.Values)
        {
            // Debug.Log("cooldown: " + cooldown);
            RectTransform rect = cooldown.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = new Vector2(-index * 70, 0);
            }
            index++;
            Debug.Log("index: " + index);
        }
    }

    public void RemoveCooldown(string abilityName)
    {
        if (activeCooldowns.ContainsKey(abilityName))
        {
            Destroy(activeCooldowns[abilityName]);
            activeCooldowns.Remove(abilityName);
        }
        ReorderCooldowns();
    }
}
