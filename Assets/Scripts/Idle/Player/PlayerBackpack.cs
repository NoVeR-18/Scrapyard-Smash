using Items;
using Items.Container;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Item;

namespace Player
{
    [RequireComponent(typeof(Player))]
    public class PlayerBackpack : MonoBehaviour
    {
        public ItemsContainer ItemsContainer { get => _itemsContainer; }

        [SerializeField] private ItemsContainer _itemsContainer;
        [SerializeField] private List<Item> itemPrefabs;

        private Player _player;

        private void Awake()
        {
            _player = GetComponent<Player>();
            //_itemsContainer.Init();
        }

        public void SaveBackpack()
        {
            List<ItemData> itemsToSave = new List<ItemData>();

            foreach (Item item in _itemsContainer.GetItems())
            {
                itemsToSave.Add(new ItemData(item));
            }

            string json = JsonUtility.ToJson(new ItemDataWrapper(itemsToSave));
            PlayerPrefs.SetString("PlayerBackpack", json);
            PlayerPrefs.Save();
        }

        public void LoadBackpack()
        {
            if (PlayerPrefs.HasKey("PlayerBackpack"))
            {
                string json = PlayerPrefs.GetString("PlayerBackpack");
                ItemDataWrapper dataWrapper = JsonUtility.FromJson<ItemDataWrapper>(json);

                foreach (ItemData itemData in dataWrapper.items)
                {
                    Item newItem = CreateItem(itemData);

                    _itemsContainer.AddItem(newItem);
                }
            }
        }
        public Item CreateItem(ItemData itemData)
        {
            Item itemPrefab = FindItemPrefabByType(itemData.type);
            if (itemPrefab == null)
            {
                throw new NullReferenceException($"Не найден префаб для предмета типа: {itemData.type}");
            }

            Item newItem = instantiateItem(itemPrefab);


            return newItem;
        }
        private Item FindItemPrefabByType(ItemType type)
        {
            foreach (Item prefab in itemPrefabs)
            {
                if (prefab.Type == type)
                {
                    return prefab;
                }
            }
            return null;
        }
        private Item instantiateItem(Item item)
        {
            Item instance = Instantiate(item);
            instance.transform.position = ItemsContainer.transform.position;
            return instance;
        }
        public void ClearBackpackData()
        {
            PlayerPrefs.DeleteKey("PlayerBackpack");
        }

    }
    [System.Serializable]
    public class ItemDataWrapper
    {
        public List<ItemData> items;

        public ItemDataWrapper(List<ItemData> items)
        {
            this.items = items;
        }
    }
}
