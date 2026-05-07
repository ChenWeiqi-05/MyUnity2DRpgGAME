using System.Text;
using UnityEngine;



#if UNITY_EDITOR
using UnityEditor;
#endif
public enum ItemType
{
    //²ÄÁÏ,
    //×°±¸,
    Material,
    Equipment
}
[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public string itemId;
    [Range(0, 100)]
    public float dropChance;
    protected StringBuilder sb = new StringBuilder();
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(itemId))
        {
            string path = AssetDatabase.GetAssetPath(this);
            itemId = AssetDatabase.AssetPathToGUID(path);
        }
#endif
    }
    public virtual string GetDescription()
    {
        return "";
    }

}
