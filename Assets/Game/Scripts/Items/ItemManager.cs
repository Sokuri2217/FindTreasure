using UnityEngine;

[CreateAssetMenu(menuName ="Items/AddDigArea")]
public class AddDigArea : ItemBase
{
    public int addArea; //Šg‘å”ÍˆÍ

    public override void OnHold(PlayerController player)
    {
        player.dig_width += (addArea * 2);
        player.dig_height += (addArea * 2);
    }
}