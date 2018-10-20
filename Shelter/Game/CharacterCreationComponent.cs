using Game;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class CharacterCreationComponent : MonoBehaviour
{
    public GameObject manager;
    public CreatePart part;

    [UsedImplicitly] // Not sure if it's actually called
    public void nextOption()
    {
        this.manager.GetComponent<CustomCharacterManager>().nextOption(this.part);
    }

    [UsedImplicitly] // Not sure if it's actually called
    public void prevOption()
    {
        this.manager.GetComponent<CustomCharacterManager>().prevOption(this.part);
    }
}

