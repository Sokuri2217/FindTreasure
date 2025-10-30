using UnityEngine;

[CreateAssetMenu(menuName ="Items/AddDigAreaW1H1")]
public class AddDigAreaW1H1 : ItemBase
{
    public int addArea; //Šg‘å”ÍˆÍ

    public override void OnHold(PlayerController player)
    {
        player.dig_width = (player.dig_width + addArea * 2);
        player.dig_height = (player.dig_height + addArea * 2);
    }
    public override void OnDelete(PlayerController player)
    {
        player.dig_width = (player.dig_width - addArea * 2);
        player.dig_height = (player.dig_height - addArea * 2);
    }
}

[CreateAssetMenu(menuName = "Items/AddDigPower1")] 
public class AddDigPower1 : ItemBase
{
    public override void OnHold(PlayerController player)
    {
        player.digPower++;
    }
    public override void OnDelete(PlayerController player)
    {
        player.digPower--;
    }
}

[CreateAssetMenu(menuName = "Items/AddDigCount1")]
public class AddDigCount1 : ItemBase
{
    public override void OnGet(PlayerController player)
    {
        player.digCurrent++;
        if(player.digCurrent > player.digLimit)
        {
            player.digCurrent = player.digLimit;
        }
    }
}