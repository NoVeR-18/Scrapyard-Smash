using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Items.Container
{
    public class ItemsContainer : MonoBehaviour
    {
        public bool IsFull
        {
            get
            {
                return _items.Count >= _capacity;
            }
        }
        public int Count
        {
            get
            {
                return _items.Count;
            }
        }
        public int Capacity { get => _capacity; }

        private readonly int _maxTotalMultipleSlots = 500;
        [SerializeField] private float _slotSize = 1f;

        [SerializeField] private ItemType _storeItemType;
        [SerializeField, Tooltip("Don't change due runtime!")] private int _capacity = 10;
        [SerializeField] private int _sizeX = 3;
        [SerializeField] private int _sizeZ = 3;

        private List<Item> _items = new List<Item>();
        private ContainerSlot[] _slots;
        [SerializeField] private ContainerSlot _slotPrefab;                         // Markdown #2

        public void Init(ItemType storeItemType)
        {
            _storeItemType = storeItemType;
            _slots = CreateSlots(transform, _capacity, _sizeX, _sizeZ);
        }
        public ContainerSlot[] CreateSlots(Transform parent, int slotsCount, int sizeX = 3, int sizeZ = 3)
        {
            List<ContainerSlot> slots = new List<ContainerSlot>();

            int y = 0;
            while (true)
            {
                for (int x = 0, i = 0; x < sizeX; x++)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        if (slots.Count >= slotsCount)
                        {
                            return slots.ToArray();
                        }
                        if (i >= _maxTotalMultipleSlots)
                        {
                            throw new ArgumentOutOfRangeException($"Too many slots are instantiated. Max: {_maxTotalMultipleSlots}; Current: {_maxTotalMultipleSlots}; Slots count: {slotsCount}");
                        }

                        ContainerSlot slot = instantiateSlotPrefab(parent);
                        slot.transform.localPosition = new Vector3(x - (sizeX / 2), y, z - (sizeZ / 2)) * _slotSize;
                        slot.transform.localRotation = Quaternion.identity;

                        slots.Add(slot);
                        i++;
                    }
                }
                y++;
            }
        }
        private ContainerSlot instantiateSlotPrefab(Transform parent)
        {
            ContainerSlot instance = Instantiate(_slotPrefab, parent);

            instance.transform.localPosition = Vector3.zero;

            return instance;
        }
        public bool AddItem(Item item)
        {
            if (!CanAddItem(item))
            {
                return false;
            }

            _items.Add(item);
            item.GoToSlot(this, _items);

            return true;
        }
        public bool TakeItem(out Item item)
        {
            if (!CanTakeItem())
            {
                item = null;
                return false;
            }

            Item lastItem = _items[_items.Count - 1];
            item = lastItem;

            IEnumerable<ContainerSlot> busySlotsWithItem = _slots.Where(x => x.BusyItem == lastItem);
            if (busySlotsWithItem.Count() != 0)
            {
                busySlotsWithItem?.First().Detach();
            }

            _items.RemoveAt(_items.Count - 1);

            return true;
        }
        public bool TakeItem(out Item item, ItemType type)
        {
            if (!CanTakeItem())
            {
                item = null;
                return false;
            }

            IEnumerable<Item> typedItems = _items.Where(x => x.Type == type);
            if (typedItems.Count() == 0)
            {
                item = null;
                return false;
            }

            Item queryTakeItem = typedItems.Last();
            item = queryTakeItem;

            IEnumerable<ContainerSlot> busySlotsWithItem = _slots.Where(x => x.BusyItem == queryTakeItem);
            if (busySlotsWithItem.Count() != 0)
            {
                busySlotsWithItem?.First().Detach();
            }

            _items.Remove(queryTakeItem);

            return true;
        }


        public int CountItemTypeInSlots(ItemType type)
        {
            if (!CanTakeItem())
            {
                return 0;
            }
            IEnumerable<Item> typedItems = _items.Where(x => x.Type == type);
            if (typedItems.Count() == 0)
            {
                return 0;
            }

            Item queryTakeItem = typedItems.Last();

            IEnumerable<ContainerSlot> busySlotsWithItem = _slots.Where(x => x.BusyItem == queryTakeItem);
            if (busySlotsWithItem.Count() != 0)
            {
                return busySlotsWithItem.Count();
            }
            return 0;
        }

        public bool CanAddItem(Item item)
        {
            if (item == null)
                return false;

            if (_storeItemType != ItemType.Null)
            {
                if (item.Type != _storeItemType)
                {
                    return false;
                }
            }

            return CanAddItem();
        }
        public bool CanAddItem()
        {
            return !IsFull;
        }
        public bool CanTakeItem()
        {
            return _items.Count > 0;
        }

        public ContainerSlot AttachItemToSlot(Item item)
        {
            if (_slots == null || _slots.Length == 0)
                throw new NullReferenceException("Attachment slot is null. Did you initialized this component?");

            IEnumerable<ContainerSlot> availableSlots = _slots.Where(x => x.IsBusy == false);       // Markdown #1
            ContainerSlot availableSlot = availableSlots.FirstOrDefault();
            availableSlot.Attach(item);
            return availableSlot;

            // Somehow, System.Linq thread can be crashed without any exceptions [Markdown #1]
            // In my case in debug mode it shows: "The thread 0x48e32be0 has exited with code 0 (0x0)"
            // I found out that can crash in async-await method if array will be null. Like in my case, 
            // I found bug in my code, that _slots [Markdown #2] was null. I have no idea how can I try-catch this
            // exception, except checking _slots for != null.
        }
        public void DetachItemFromSlot(Item item)
        {
            ContainerSlot itemSlot = _slots.Where(x => x.BusyItem == item).First();
            itemSlot.Detach();
        }
        public List<Item> GetItems()
        {
            return _items;
        }
    }
}
