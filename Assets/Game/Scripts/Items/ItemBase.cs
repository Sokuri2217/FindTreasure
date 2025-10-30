using UnityEngine;

[CreateAssetMenu(fileName = "ItemBase", menuName = "Scriptable Objects/ItemBase")]
public class ItemBase : ScriptableObject
{
    public string itemName;
    public string[] description;
    public Sprite icon;

    //Šl“¾Œø‰Ê
    public virtual void OnGet(PlayerController player) { }
    //íİŒø‰Ê
    public virtual void OnHold(PlayerController player) { }
    //”CˆÓŒø‰Ê
    public virtual void OnUse(PlayerController player) { }
    //”jŠü
    public virtual void OnDelete(PlayerController player) { }
}
