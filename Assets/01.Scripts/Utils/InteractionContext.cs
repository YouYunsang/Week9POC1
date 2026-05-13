using UnityEngine;

public readonly struct InteractionContext
{
    public InteractionContext(
        GameObject interactor,
        PlayerCondition playerCondition,
        PlayerInventory playerInventory,
        PlayerBattery playerBattery,
        PlayerMovement playerMovement)
    {
        Interactor = interactor;
        PlayerCondition = playerCondition;
        PlayerInventory = playerInventory;
        PlayerBattery = playerBattery;
        PlayerMovement = playerMovement;
    }

    public GameObject Interactor { get; }
    public PlayerCondition PlayerCondition { get; }
    public PlayerInventory PlayerInventory { get; }
    public PlayerBattery PlayerBattery { get; }
    public PlayerMovement PlayerMovement { get; }
}