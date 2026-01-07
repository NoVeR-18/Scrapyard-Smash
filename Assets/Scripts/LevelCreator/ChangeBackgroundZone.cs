using System.Collections.Generic;
using UnityEngine;

public class ChangeBackgroundZone : MonoBehaviour
{
    private List<MeshRenderer> _floors;
    private int _floorsNumber = 0;
    [SerializeField] private List<Material> florsMaterial;

    void Start()
    {
        _floorsNumber = PlayerPrefs.GetInt("floorsNumber", 0);
        _floors = new List<MeshRenderer>();
        foreach (Transform item in gameObject.transform)
        {
            _floors.Add(item.GetComponent<MeshRenderer>());
        }
    }
    public void ChaneMaterial(int numberofmaterial)
    {
        _floorsNumber = (numberofmaterial / 5) % florsMaterial.Count;

        _floorsNumber = Mathf.Min(_floorsNumber, florsMaterial.Count - 1);

        foreach (var item in _floors)
        {
            item.material = florsMaterial[_floorsNumber];
        }
    }
}
