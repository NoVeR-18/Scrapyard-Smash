using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelectionUI : MonoBehaviour
{
    public GameObject popupPanel;
    public Button closeButton;
    public Button openButton;

    public List<SkinSlot> forkliffSlots;
    public List<SkinSlot> magneteSlots;
    public List<SkinSlot> helicopterSlots;
    public List<SkinSlot> TrailSlots;

    private void Start()
    {
        closeButton.onClick.AddListener(() => popupPanel.SetActive(false));
        //openButton.onClick.AddListener(() => popupPanel.SetActive(true));
        InitializeSlots(forkliffSlots, SkinCategory.Forklift);
        InitializeSlots(magneteSlots, SkinCategory.Magnete);
        InitializeSlots(helicopterSlots, SkinCategory.Helicopter);
        InitializeSlots(TrailSlots, SkinCategory.Trail);
    }

    private void InitializeSlots(List<SkinSlot> slots, SkinCategory category)
    {
        List<SkinData> skins = SkinManager.Instance.GetSkinList(category);
        if (skins == null) return;

        int selectedSkinIndex = SkinManager.Instance.GetSelectedSkinIndex(category); //  Получаем индекс выбранного скина

        for (int i = 0; i < slots.Count; i++)
        {
            int index = i;
            SkinSlot slot = slots[i];

            slot.buyButton.onClick.AddListener(() =>
            {
                if (LevelManager.Instance.wallet.Crystals >= skins[index].price)
                {
                    if (SkinManager.Instance.PurchaseSkin(category, index, LevelManager.Instance.wallet.Crystals))
                    {
                        LevelManager.Instance.wallet.SpendCrystals(skins[index].price);
                        slot.SetPurchased();
                    }
                }
            });

            slot.coastText.text = skins[index].price.ToString();
            if (skins[index].previewIcon != null)
            {
                slot.PreviewSelectedIcon.sprite = skins[index].previewIcon;
                slot.PreviewUnselectedIcon.sprite = skins[index].previewIcon;
            }
            slot.selectButton.onClick.AddListener(() =>
            {
                Renderer vehicleRenderer = FindVehicleRenderer(category);
                SkinManager.Instance.ApplySkin(category, index);

                for (int i = 0; i < slots.Count; i++)
                {
                    if (skins[i].isPurchased)
                    {
                        slots[i].SetPurchased();
                    }
                }
                slot.SetSelected(); //  Выделяем выбранный скин
            });

            slot.UpdateUI(skins[index].isPurchased);

            if (index == selectedSkinIndex)
            {
                slot.SetSelected();
            }
        }
    }

    private Renderer FindVehicleRenderer(SkinCategory category)
    {
        GameObject vehicle = GameObject.Find(category.ToString());
        return vehicle ? vehicle.GetComponent<Renderer>() : null;
    }

    public void OnSkinSelected(SkinCategory category, int skinIndex)
    {
        if (SkinManager.Instance != null)
        {
            SkinManager.Instance.ApplySkin(category, skinIndex);
        }
    }
}
