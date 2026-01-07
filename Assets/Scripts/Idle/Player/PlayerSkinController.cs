using System.Collections.Generic;
using UnityEngine;

public class PlayerSkinController : MonoBehaviour
{
    public SkinCategory skinCategory = SkinCategory.Forklift;
    public Renderer playerRenderer;

    private void Start()
    {
        ApplySavedSkin();
    }

    private void ApplySavedSkin()
    {
        if (SkinManager.Instance != null)
        {
            List<SkinData> skins = SkinManager.Instance.GetSkinList(skinCategory);
            if (skins == null || skins.Count == 0) return;

            int selectedSkinIndex = SkinManager.Instance.GetSelectedSkinIndex(skinCategory);
            if (selectedSkinIndex >= 0 && selectedSkinIndex < skins.Count)
            {
                playerRenderer.material = skins[selectedSkinIndex].material;
            }
        }
    }
}
