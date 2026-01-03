using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelectionUI : MonoBehaviour
{
    public GameObject popupPanel;
    public Button closeButton;
    public Button openButton;
    public Button TakeADSCrystalButton;

    public List<SkinSlot> forkliffSlots;
    public List<SkinSlot> magneteSlots;
    public List<SkinSlot> helicopterSlots;
    public List<SkinSlot> TrailSlots;

    public Button trailSellectButton;
    public Button forkliftSellectButton;
    public Button magneteSellectButton;
    public Button helicopterSellectButton;

    public Transform trailConteiner;
    public Transform forkliftConteiner;
    public Transform magneteConteiner;
    public Transform helicopterConteiner;

    public Transform forkliftPreviewModel;
    public Transform magnetePreviewModel;
    public Transform helicopterPreviewModel;

    private void Awake()
    {
        openButton.onClick.AddListener(() => { popupPanel.SetActive(true); trailSellectButton.onClick.Invoke(); });
        closeButton.onClick.AddListener(() => popupPanel.SetActive(false));
    }

    private void Start()
    {
        TakeADSCrystalButton.onClick.AddListener(() =>
        {
            if (!AdManager.Instance.IsRewardedReady())
            {
                Debug.LogWarning("Rewarded ad is not ready.");
                return;
            }
            AdManager.Instance.ShowRewarded(() =>
            {
                LevelManager.Instance.wallet.AddCrystals(30);
                Vector2 uiPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    LevelManager.Instance.wallet.canvas.transform as RectTransform,
                    RectTransformUtility.WorldToScreenPoint(LevelManager.Instance.wallet.canvas.worldCamera, TakeADSCrystalButton.transform.position),
                    LevelManager.Instance.wallet.canvas.worldCamera,
                    out uiPosition
                );
            });
        });
        trailSellectButton.onClick.AddListener(() =>
        {
            trailConteiner.gameObject.SetActive(true);
            forkliftConteiner.gameObject.SetActive(false);
            magneteConteiner.gameObject.SetActive(false);
            helicopterConteiner.gameObject.SetActive(false);
            forkliftPreviewModel.gameObject.SetActive(true);
            magnetePreviewModel.gameObject.SetActive(false);
            helicopterPreviewModel.gameObject.SetActive(false);
            if (AdManager.Instance.IsRewardedReady())
            {
                TakeADSCrystalButton.gameObject.SetActive(true);
            }
            else
            {
                TakeADSCrystalButton.gameObject.SetActive(false);
            }
        });
        forkliftSellectButton.onClick.AddListener(() =>
        {
            trailConteiner.gameObject.SetActive(false);
            forkliftConteiner.gameObject.SetActive(true);
            magneteConteiner.gameObject.SetActive(false);
            helicopterConteiner.gameObject.SetActive(false);
            forkliftPreviewModel.gameObject.SetActive(true);
            magnetePreviewModel.gameObject.SetActive(false);
            helicopterPreviewModel.gameObject.SetActive(false);
            if (AdManager.Instance.IsRewardedReady())
            {
                TakeADSCrystalButton.gameObject.SetActive(true);
            }
            else
            {
                TakeADSCrystalButton.gameObject.SetActive(false);
            }
        });
        magneteSellectButton.onClick.AddListener(() =>
        {
            trailConteiner.gameObject.SetActive(false);
            forkliftConteiner.gameObject.SetActive(false);
            magneteConteiner.gameObject.SetActive(true);
            helicopterConteiner.gameObject.SetActive(false);
            forkliftPreviewModel.gameObject.SetActive(false);
            magnetePreviewModel.gameObject.SetActive(true);
            helicopterPreviewModel.gameObject.SetActive(false);
            if (AdManager.Instance.IsRewardedReady())
            {
                TakeADSCrystalButton.gameObject.SetActive(true);
            }
            else
            {
                TakeADSCrystalButton.gameObject.SetActive(false);
            }
        });
        helicopterSellectButton.onClick.AddListener(() =>
        {
            trailConteiner.gameObject.SetActive(false);
            forkliftConteiner.gameObject.SetActive(false);
            magneteConteiner.gameObject.SetActive(false);
            helicopterConteiner.gameObject.SetActive(true);
            forkliftPreviewModel.gameObject.SetActive(false);
            magnetePreviewModel.gameObject.SetActive(false);
            helicopterPreviewModel.gameObject.SetActive(true);
            if (AdManager.Instance.IsRewardedReady())
            {
                TakeADSCrystalButton.gameObject.SetActive(true);
            }
            else
            {
                TakeADSCrystalButton.gameObject.SetActive(false);
            }
        });
        trailSellectButton.onClick.Invoke();

        InitializeSlots(forkliffSlots, SkinCategory.Forklift);
        InitializeSlots(magneteSlots, SkinCategory.Magnete);
        InitializeSlots(helicopterSlots, SkinCategory.Helicopter);
        InitializeSlots(TrailSlots, SkinCategory.Trail);
    }
    private void InitializeSlots(List<SkinSlot> slots, SkinCategory category)
    {
        List<SkinData> skins = SkinManager.Instance.GetSkinList(category);
        if (skins == null) return;

        int selectedSkinIndex = SkinManager.Instance.GetSelectedSkinIndex(category);

        // Отключаем лишние слоты
        for (int i = skins.Count; i < slots.Count; i++)
        {
            slots[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < skins.Count; i++)
        {
            int index = i;
            SkinSlot slot = slots[i];

            slot.gameObject.SetActive(true); // Убеждаемся, что нужные слоты включены
            if (skins[index].skinID == "Default")
                SkinManager.Instance.PurchaseSkin(category, index, LevelManager.Instance.wallet.Crystals);
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

                for (int j = 0; j < slots.Count; j++)
                {
                    if (j < skins.Count && skins[j].isPurchased)
                    {
                        slots[j].SetPurchased();
                    }
                    else
                        slots[j].UpdateUI(skins[j].isPurchased);
                }
                slot.SetSelected();
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
