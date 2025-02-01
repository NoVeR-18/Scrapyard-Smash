using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FloatingJoystick : Joystick
{
    private GraphicRaycaster raycaster;
    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
        raycaster = FindObjectOfType<GraphicRaycaster>(); // Находим UI Raycaster
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (IsPointerOverUI(eventData))
            return;
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
    }
    private bool IsPointerOverUI(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);
        return results.Count > 0;
    }
}