using UnityEngine;

public readonly struct InteractionContext
{
    public InteractionContext(
        GameObject interactor,
        PlayerCondition playerCondition,
        PlayerInventory playerInventory)
    {
        Interactor = interactor;
        PlayerCondition = playerCondition;
        PlayerInventory = playerInventory;
    }

    public GameObject Interactor { get; }
    public PlayerCondition PlayerCondition { get; }
    public PlayerInventory PlayerInventory { get; }
}