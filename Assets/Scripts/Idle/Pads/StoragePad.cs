using Items;
using Items.Container;
using System;
using UnityEngine;

public enum StorageBuildingType
{
    Input,
    Output
}
public class StoragePad : MonoBehaviour
{
    public ItemsContainer ItemsContainer { get => _itemsContainer; }
    public StorageBuildingType Type { get => _storageType; }
    public float InteractionRadius { get => _interactionRadius; }
    public ItemType ItemType { get => _itemType; }

    [SerializeField] private StorageBuildingType _storageType;
    [SerializeField] private ItemsContainer _itemsContainer;
    [SerializeField] private float _interactionRadius = 1.5f;
    [SerializeField] private ItemType _itemType;

    [SerializeField] private Item _itemPrefab;

    public void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (other.tag == "Player")
        {
            Interact(other.gameObject.GetComponent<Player.Player>());
        }
    }
    public void OnTriggerExit(UnityEngine.Collider other)
    {
        if (other.tag == "Player")
        {
            CloseInteract();
        }
    }
    virtual public void CloseInteract()
    {
        StopAllCoroutines();
    }
    virtual public void Interact(Player.Player player)
    {
        Debug.Log("interaction");
        switch (_storageType)
        {
            case StorageBuildingType.Input:
                takeItemsFromPlayer(player);
                break;
            case StorageBuildingType.Output:
                giveItemsToPlayer(player);
                break;
            default:
                throw new NotImplementedException();
        }
    }
    virtual public void giveItemsToPlayer(Player.Player player)
    {
        if (ItemsContainer.Count == 0)
            return;

        if (!player.Backpack.ItemsContainer.CanAddItem())
            return;

        Item takenItem = null;
        if (!ItemsContainer.TakeItem(out takenItem))
            return;

        player.Backpack.ItemsContainer.AddItem(takenItem);
    }
    virtual public void takeItemsFromPlayer(Player.Player player)
    {
        if (!ItemsContainer.CanAddItem())
            return;

        Item takenItem = null;
        if (!player.Backpack.ItemsContainer.TakeItem(out takenItem, ItemType))
            return;

        ItemsContainer.AddItem(takenItem);
    }

    private void Awake()
    {
        _itemsContainer?.Init(_itemType);
    }
    public Item CreateItem()
    {
        if (_itemPrefab == null)
        {
            throw new NullReferenceException($"FrameMachineTab: Can't find item ");
        }

        return instantiateItem(_itemPrefab);
    }
    private Item instantiateItem(Item item)
    {
        Item instance = Instantiate(item);
        instance.transform.position = Vector3.zero;
        return instance;
    }
}

