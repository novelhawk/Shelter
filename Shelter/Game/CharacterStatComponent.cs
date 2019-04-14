using Game;
using Game.Enums;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class CharacterStatComponent : MonoBehaviour
{
    public GameObject manager;
    public CreateStat type;

    public void nextOption()
    {
        this.manager.GetComponent<CustomCharacterManager>().nextStatOption(this.type);
    }

    public void prevOption()
    {
        this.manager.GetComponent<CustomCharacterManager>().prevStatOption(this.type);
    }
}

