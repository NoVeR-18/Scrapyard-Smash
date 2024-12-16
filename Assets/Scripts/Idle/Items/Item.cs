using Items;
using Items.Container;
using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Item : MonoBehaviour
{
    public ItemType Type { get => _itemType; }
    public Action<Item> OnDisappear;

    [SerializeField] private ItemType _itemType;
    private float _itemPositionLerp = 2f;
    [SerializeField] private float _itemRotationLerp = 10f;
    private float _arcHeight = 2f;

    private readonly float _moveEndThreshold = 0.01f;
    private ContainerSlot _currentSlot;
    private Vector3 _startPosition;
    private float _time;
    List<Item> _items;
    public bool CanTake = true;

    public int Cost = 50;
    public BoxCollider phisicCollider;
    private bool moveWithArc = true;

    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player" && CanTake)
        {
            var player = other.GetComponent<Player.Player>();
            if (player == null)
                player = other.GetComponentInParent<Player.Player>();
            if (!player.Backpack.ItemsContainer.CanAddItem())
                return;

            if (player.Backpack.ItemsContainer.AddItem(this))
            {
                CanTake = false;
                GameManager.Instance.Vibrate();
                if (audioSource != null)
                    audioSource?.Play();
                if (player as Forkliff)
                {
                    moveWithArc = true;
                }
                else
                {
                    moveWithArc = false;
                }
                if (phisicCollider != null)
                    phisicCollider.enabled = false;
                Destroy(GetComponent<Rigidbody>());
            }
        }
    }

    public void GoToSlot(ItemsContainer container, List<Item> items)
    {
        _currentSlot = container.AttachItemToSlot(this);
        _items = items;
        _startPosition = transform.position;
        _time = 0f;
    }

    public void Disappear()
    {
        OnDisappear?.Invoke(this);
        Destroy(gameObject);
    }
    public void SetData(Item itemData)
    {
        _itemType = itemData.Type;
    }

    public void OnSlotAttach(ContainerSlot slot)
    {
        var size = transform.localScale;
        transform.SetParent(slot.transform);
        transform.localScale = size;
    }

    public void OnSlotDetach(ContainerSlot slot)
    {
        var size = transform.localScale;
        _currentSlot = null;
        transform.SetParent(null);
        transform.localScale = size;
    }

    private void FixedUpdate()
    {
        if (_currentSlot == null)
        {
            return;
        }
        if (moveWithArc)
            MoveToSlotWithArc();
        else MoveToSlotDirectly();

        float distanceToTarget = Vector3.Distance(transform.position, _currentSlot.transform.position);
        if (distanceToTarget <= _moveEndThreshold)
        {
            transform.position = _currentSlot.transform.position;
            transform.localRotation = Quaternion.identity;

            //_items.Add(this);
            _currentSlot = null;
        }
    }
    private void MoveToSlotDirectly()
    {
        _time += Time.deltaTime * _itemPositionLerp;

        // Линейно интерполируем позицию предмета к позиции слота
        transform.position = Vector3.Lerp(transform.position, _currentSlot.transform.position, _time);

        // Линейно интерполируем вращение предмета к вращению слота
        transform.rotation = Quaternion.Lerp(transform.rotation, _currentSlot.transform.rotation, _itemRotationLerp * Time.deltaTime);

    }
    private void MoveToSlotWithArc()
    {
        _time += Time.deltaTime * _itemPositionLerp;

        Vector3 targetPosition = _currentSlot.transform.position;

        Vector3 straightLinePosition = Vector3.Lerp(_startPosition, targetPosition, _time);

        float heightOffset = Mathf.Sin(_time * Mathf.PI) * _arcHeight;

        transform.position = new Vector3(
            straightLinePosition.x,
            straightLinePosition.y + heightOffset,
            straightLinePosition.z
        );

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            _currentSlot.transform.rotation,
            _itemRotationLerp * Time.deltaTime
        );
    }
    [System.Serializable]
    public class ItemData
    {
        public ItemType type;
        public int amount;

        public ItemData(Item item)
        {
            this.type = item.Type;
        }
    }
}
