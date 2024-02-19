using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Image icon;

    private void Start()
    {
        icon.gameObject.SetActive(false);
    }
    public void UpdateInfo(Sprite itemIcon)
    {
        icon.gameObject.SetActive(true);
        icon.sprite = itemIcon;
    }
}
