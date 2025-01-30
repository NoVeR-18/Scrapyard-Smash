using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance;

    public List<SkinData> forkliffSkins;
    public List<SkinData> magneteSkins;
    public List<SkinData> helicopterSkins;
    public List<SkinData> TrailSkins;

    public List<PlayerSkinController> playerSkinControllers;

    private Dictionary<SkinCategory, int> selectedSkins = new Dictionary<SkinCategory, int>();
    private const string SaveKey = "SkinsData";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadSkins();
    }

    public void ApplySkin(SkinCategory category, int skinIndex)
    {
        List<SkinData> skins = GetSkinList(category);
        if (skins == null || skinIndex >= skins.Count) return;

        if (skins[skinIndex].isPurchased)
        {
            foreach (var pSkin in playerSkinControllers)
            {
                if (pSkin.skinCategory == category)
                {
                    pSkin.playerRenderer.material = skins[skinIndex].material;
                }
            }
            selectedSkins[category] = skinIndex;
            SaveSkins();
        }
    }

    public bool PurchaseSkin(SkinCategory category, int skinIndex, int playerCurrency)
    {
        List<SkinData> skins = GetSkinList(category);
        if (skins == null || skinIndex >= skins.Count || skins[skinIndex].isPurchased) return false;

        if (playerCurrency >= skins[skinIndex].price)
        {
            skins[skinIndex].isPurchased = true;
            SaveSkins();
            return true;
        }
        return false;
    }

    public List<SkinData> GetSkinList(SkinCategory category)
    {
        switch (category)
        {
            case SkinCategory.Forklift: return forkliffSkins;
            case SkinCategory.Magnete: return magneteSkins;
            case SkinCategory.Helicopter: return helicopterSkins;
            case SkinCategory.Trail: return TrailSkins;
            default: return null;
        }
    }

    private void SaveSkins()
    {
        SaveData data = new SaveData(forkliffSkins, magneteSkins, helicopterSkins, TrailSkins, selectedSkins);
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }
    private void OnApplicationQuit()
    {
        SaveSkins();
    }
    private void LoadSkins()
    {
        if (!PlayerPrefs.HasKey(SaveKey)) return;

        string json = PlayerPrefs.GetString(SaveKey);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // Обновляем статус покупки в текущих списках скинов
        UpdateSkinPurchaseStatus(forkliffSkins, data.forkliffSkins);
        UpdateSkinPurchaseStatus(magneteSkins, data.magneteSkins);
        UpdateSkinPurchaseStatus(helicopterSkins, data.helicopterSkins);
        UpdateSkinPurchaseStatus(TrailSkins, data.TrailSkins);

        // Восстанавливаем выбранные скины
        selectedSkins = new Dictionary<SkinCategory, int>();
        if (data.selectedSkins != null)
        {
            foreach (var pair in data.selectedSkins)
            {
                selectedSkins[(SkinCategory)pair.key] = pair.value;
            }
        }
    }


    private void UpdateSkinPurchaseStatus(List<SkinData> currentSkins, List<SkinData> savedSkins)
    {
        if (savedSkins == null || currentSkins == null) return;

        foreach (var savedSkin in savedSkins)
        {
            SkinData currentSkin = currentSkins.Find(s => s.skinID == savedSkin.skinID);
            if (currentSkin != null)
            {
                currentSkin.isPurchased = savedSkin.isPurchased; // Восстанавливаем покупку
            }
        }
    }

    public int GetSelectedSkinIndex(SkinCategory category)
    {
        return selectedSkins.TryGetValue(category, out int skinIndex) ? skinIndex : 0;
    }

    [System.Serializable]
    public class SerializableKeyValuePair
    {
        public int key;
        public int value;

        public SerializableKeyValuePair(int key, int value)
        {
            this.key = key;
            this.value = value;
        }
    }

    [System.Serializable]
    private class SaveData
    {
        public List<SkinData> forkliffSkins;
        public List<SkinData> magneteSkins;
        public List<SkinData> helicopterSkins;
        public List<SkinData> TrailSkins;
        public List<SerializableKeyValuePair> selectedSkins;

        public SaveData(List<SkinData> forkliff, List<SkinData> magnete, List<SkinData> helicopter, List<SkinData> Trail, Dictionary<SkinCategory, int> selected)
        {
            forkliffSkins = forkliff ?? new List<SkinData>();
            magneteSkins = magnete ?? new List<SkinData>();
            helicopterSkins = helicopter ?? new List<SkinData>();
            TrailSkins = Trail ?? new List<SkinData>();

            selectedSkins = new List<SerializableKeyValuePair>();
            if (selected != null)
            {
                foreach (var pair in selected)
                {
                    selectedSkins.Add(new SerializableKeyValuePair((int)pair.Key, pair.Value));
                }
            }
        }
    }
}
public enum SkinCategory
{
    Forklift,
    Magnete,
    Helicopter,
    Trail
}

[System.Serializable]
public class SkinData
{
    public string skinID;
    public Material material;
    public Sprite previewIcon;
    public int price;
    public bool isPurchased;
}
