using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHud : MonoBehaviour
{
    List<Image> _weapons;

    void Start()
    {
        _weapons = new List<Image>();

        foreach (RectTransform child in transform)
        {
            if (child.TryGetComponent<Image>(out var image))
            {
                _weapons.Add(image);
            }
        }

        EquipWeaponUI(1);
    }

    public void EquipWeaponUI(int weaponIndex)
    {
        if (_weapons == null) return;

        for (int i = 0; i < _weapons.Count; i++)
        {
            if (i + 1 == weaponIndex)
            {
                _weapons[i].color = new Color(1, 1, 1, 1);
            }
            else
            {
                _weapons[i].color = new Color(1, 1, 1, 0.5f);
            }
        }
    }
}
