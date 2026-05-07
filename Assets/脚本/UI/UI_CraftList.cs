using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class UI_CraftList : MonoBehaviour,IPointerDownHandler
{
    [SerializeField] private Transform craftSlotParent;
    [SerializeField] private GameObject craftSlotPrefab;

    [SerializeField] private List<ItemData_Equipment> craftEquipment;
    //[SerializeField] private List<UI_CraftSlot> craftSlots;
    //protected override void Start()
    //{
    //    base.Start();
    //}
    void Start()
    {
        transform.parent.GetChild(0).GetComponent<UI_CraftList>().SetupCraftList();
        SetupDefaultCraftwindow();
    }

    public void SetupCraftList()
    {
        for (int i = 0; i < craftSlotParent.childCount; i++)
        {
            Destroy(craftSlotParent.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < craftEquipment.Count; i++)
        {
            GameObject newSlot = Instantiate(craftSlotPrefab, craftSlotParent);
            newSlot.GetComponent<UI_CraftSlot>().SetupCraftSlot(craftEquipment[i]);
                
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetupCraftList();
    }
    public void SetupDefaultCraftwindow()
    {
        if (craftEquipment[0] != null)
            GetComponentInParent<UI>().craftWindow.SetupCraftWindow(craftEquipment[0]);
    }
    //public void SetupCraftSlot(ItemData_Equipment _data)
    //{
    //    if (_data == null)
    //        return;

    //    item.data = _data;

    //    itemImage.sprite = _data.itemIcon;
    //    itemText.text = _data.itemName;

    //    if (itemText.text.Length > 12)
    //        itemText.fontSize = itemText.fontSize * .7f;
    //    else
    //        itemText.fontSize = 24;
    //}

    //public override void OnPointerDown(PointerEventData eventData)
    //{
    //    ItemData_Equipment craftData = item.data as ItemData_Equipment;
    //    Inventory.instance.CanCraft(craftData, craftData.craftingMaterials);
    //    //ui.craftWindow.SetupCraftWindow(item.data as ItemData_Equipment);
    //}
}
