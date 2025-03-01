using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CooldownSystem : MonoBehaviour
{
    private Dictionary<string, float> cooldowns = new Dictionary<string, float>();

    public delegate void OnCooldownUpdated(string ability, float percentage);
    public event OnCooldownUpdated CooldownUpdated;

    public delegate void OnCooldownEnded(string ability);
    public event OnCooldownEnded CooldownEnded;

    public void StartCooldown(string abilityName, float duration)
    {
        if (!cooldowns.ContainsKey(abilityName))
        {
            cooldowns[abilityName] = Time.time + duration;
            StartCoroutine(HandleCooldown(abilityName, duration));
        }
    }

    public void StartCooldown(string abilityName)
    {
        if (!cooldowns.ContainsKey(abilityName)) {
            cooldowns[abilityName] = 0f;
            SetCooldownPercentage(abilityName, 100f);
        }
    }


    public bool IsOnCooldown(string abilityName)
    {
        return cooldowns.ContainsKey(abilityName) && cooldowns[abilityName] > Time.time;
    }

    public void SetCooldownPercentage(string abilityName, float percentage)
    {
        if (cooldowns.ContainsKey(abilityName))
        {
            CooldownUpdated?.Invoke(abilityName, percentage);
        }
    }

    public void RemoveCooldown(string abilityName)
    {
        if (cooldowns.ContainsKey(abilityName))
        {
            cooldowns.Remove(abilityName);
            CooldownEnded?.Invoke(abilityName);
        }
    }

    private IEnumerator HandleCooldown(string abilityName, float duration)
    {
        float timePassed = 0;
        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            float percentage = 1 - (timePassed / duration);
            CooldownUpdated?.Invoke(abilityName, percentage);
            yield return null;
        }

        cooldowns.Remove(abilityName);
        CooldownEnded?.Invoke(abilityName);
    }
}
