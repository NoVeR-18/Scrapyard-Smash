
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Items.Container
{
    public class ItemsContainer : MonoBehaviour
    {
        public bool IsFull
        {
            get
            {
                return Items.Count >= _capacity;
            }
        }
        public int Count
        {
            get
            {
                return Items.Count;
            }
        }
        public int Capacity { get => _capacity; }
        public int CapacityLevel = 0;
        private readonly int _maxTotalMultipleSlots = 500;
        [SerializeField] private float _slotSize = 1f;

        [SerializeField] private ItemType _storeItemType;
        [SerializeField, Tooltip("Don't change due runtime!")] private int _capacity = 10;
        [SerializeField] private int _sizeX = 3;
        [SerializeField] private int _sizeZ = 3;

        public List<Item> Items = new List<Item>();
        private ContainerSlot[] _slots;
        [SerializeField] private ContainerSlot _slotPrefab;                         // Markdown #2

        public Action editCountItems;

        public void Init(ItemType storeItemType)
        {
            _capacity += CapacityLevel;
            _storeItemType = storeItemType;
            _slots = CreateSlots(transform, _capacity, _sizeX, _sizeZ);
        }
        public void Init()
        {
            _capacity += CapacityLevel;
            _slots = CreateSlots(transform, _capacity, _sizeX, _sizeZ);
        }
        public void InitClosely()
        {
            _capacity += CapacityLevel;
            _slots = CreateCloselySlots(transform, _capacity, _sizeX, _sizeZ);
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

        public ContainerSlot[] CreateCloselySlots(Transform parent, int slotsCount, int sizeX = 3, int sizeZ = 3)
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
                        slot.transform.localPosition = new Vector3(Random.Range(-1f, 1f), y, Random.Range(-1f, 1f));
                        slot.transform.localRotation = Quaternion.identity;

                        slots.Add(slot);
                        i++;
                    }
                }
                //y++;
            }
        }

        public void AddContainer(int additionalSlots)
        {
            // Увеличиваем максимальную емкость
            int newCapacity = _capacity + additionalSlots;
            if (newCapacity > _maxTotalMultipleSlots)
            {
                throw new ArgumentOutOfRangeException($"Cannot exceed the maximum number of slots ({_maxTotalMultipleSlots}).");
            }

            List<ContainerSlot> newSlots = new List<ContainerSlot>();
            // Создаем новые слоты
            for (int i = 0; additionalSlots > i; i++)
            {
                int y = _capacity;
                for (int x = 0; x < _sizeX; x++)
                {
                    for (int z = 0; z < _sizeZ; z++)
                    {
                        ContainerSlot slot = instantiateSlotPrefab(transform);
                        slot.transform.localPosition = new Vector3(x - (_sizeX / 2), y, z - (_sizeZ / 2)) * _slotSize;
                        slot.transform.localRotation = Quaternion.identity;
                        newSlots.Add(slot);
                    }
                }
                y++;
            }

            // Объединяем существующие и новые слоты
            _slots = _slots.Concat(newSlots).ToArray();

            // Обновляем текущую емкость
            _capacity = newCapacity;
        }
        public void AddCloselyContainer(int additionalSlots)
        {
            // Увеличиваем максимальную емкость
            int newCapacity = _capacity + additionalSlots;
            if (newCapacity > _maxTotalMultipleSlots)
            {
                throw new ArgumentOutOfRangeException($"Cannot exceed the maximum number of slots ({_maxTotalMultipleSlots}).");
            }

            List<ContainerSlot> newSlots = new List<ContainerSlot>();
            // Создаем новые слоты
            for (int i = 0; additionalSlots > i; i++)
            {
                for (int x = 0; x < _sizeX; x++)
                {
                    for (int z = 0; z < _sizeZ; z++)
                    {
                        ContainerSlot slot = instantiateSlotPrefab(transform);
                        slot.transform.localPosition = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                        slot.transform.localRotation = Quaternion.identity;
                        newSlots.Add(slot);
                    }
                }
            }

            // Объединяем существующие и новые слоты
            _slots = _slots.Concat(newSlots).ToArray();

            // Обновляем текущую емкость
            _capacity = newCapacity;
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
            Items.Add(item);
            item.GoToSlot(this, Items);
            editCountItems?.Invoke();
            return true;
        }
        public bool TakeItem(out Item item)
        {
            if (!CanTakeItem())
            {
                item = null;
                return false;
            }

            Item lastItem = Items[Items.Count - 1];
            item = lastItem;
            if (item == null)
                return false;
            IEnumerable<ContainerSlot> busySlotsWithItem = _slots.Where(x => x.BusyItem == lastItem);
            if (busySlotsWithItem.Count() != 0)
            {
                busySlotsWithItem?.First().Detach();
            }

            Items.RemoveAt(Items.Count - 1);

            editCountItems?.Invoke();
            return true;
        }
        public bool TakeLastItem(out Item item)
        {
            if (!CanTakeItem())
            {
                item = null;
                return false;
            }

            Item lastItem = Items[Items.Count - 1];
            item = lastItem;
            if (item == null)
                return false;
            IEnumerable<ContainerSlot> busySlotsWithItem = _slots.Where(x => x.BusyItem == lastItem);
            if (busySlotsWithItem.Count() != 0)
            {
                busySlotsWithItem?.First().Detach();
            }

            Items.RemoveAt(Items.Count - 1);

            editCountItems?.Invoke();
            return true;
        }
        public bool TakeItem(Item item)
        {
            if (!CanTakeItem())
            {
                return false;
            }
            if (Items.Contains(item))
            {
                if (item == null)
                    return false;
                IEnumerable<ContainerSlot> busySlotsWithItem = _slots.Where(x => x.BusyItem == item);
                if (busySlotsWithItem.Count() != 0)
                {
                    busySlotsWithItem?.First().Detach();
                }

                Items.Remove(item);

                editCountItems?.Invoke();
                return true;
            }
            else
                return false;
        }
        public bool TakeItem(out Item item, ItemType type)
        {
            if (!CanTakeItem())
            {
                item = null;
                return false;
            }

            IEnumerable<Item> typedItems = Items.Where(x => x.Type == type);
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

            Items.Remove(queryTakeItem);

            editCountItems?.Invoke();
            return true;
        }


        public int CountItemTypeInSlots(ItemType type)
        {
            if (!CanTakeItem())
            {
                return 0;
            }
            IEnumerable<Item> typedItems = Items.Where(x => x.Type == type);
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
            Items.RemoveAll(item => item == null);
            return !IsFull;
        }
        public bool CanTakeItem()
        {
            return Items.Count > 0;
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
            return Items;
        }

    }
}
