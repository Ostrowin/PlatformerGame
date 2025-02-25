using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KeyCombinationManager : MonoBehaviour
{
    private List<KeyCode> currentCombination = new List<KeyCode>();
    private Dictionary<List<KeyCode>, System.Action> registeredCombinations = new Dictionary<List<KeyCode>, System.Action>(new KeyCombinationComparer());

    public void RegisterCombination(KeyCode[] keys, System.Action action)
    {
        registeredCombinations[keys.ToList()] = action;
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            KeyCode pressedKey = GetPressedKey();
            if (pressedKey != KeyCode.None)
            {
                currentCombination.Add(pressedKey);

                foreach (var combination in registeredCombinations.Keys)
                {
                    if (combination.SequenceEqual(currentCombination))
                    {
                        registeredCombinations[combination].Invoke();
                        ResetCombination();
                        return;
                    }
                }
            }
        }

        // Resetowanie kombinacji, jeśli naciśnięto niewłaściwy klawisz
        if (Input.anyKeyDown && !registeredCombinations.Keys.Any(comb => comb.Take(currentCombination.Count).SequenceEqual(currentCombination)))
        {
            ResetCombination();
        }
    }

    private KeyCode GetPressedKey()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                return key;
            }
        }
        return KeyCode.None;
    }

    private void ResetCombination()
    {
        currentCombination.Clear();
    }
}

// Comparer, który pozwala używać List<KeyCode> jako klucza w Dictionary
public class KeyCombinationComparer : IEqualityComparer<List<KeyCode>>
{
    public bool Equals(List<KeyCode> x, List<KeyCode> y)
    {
        return x.SequenceEqual(y);
    }

    public int GetHashCode(List<KeyCode> obj)
    {
        unchecked
        {
            int hash = 17;
            foreach (var key in obj)
            {
                hash = hash * 31 + key.GetHashCode();
            }
            return hash;
        }
    }
}
