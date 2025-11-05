using JetBrains.Annotations;
using UnityEngine;

//ŠÖ”–¼‚Ì––‚É•ÏX“à—e‚ğ‘‚­
// W = ‰¡
// H = c
// _ = ƒ}ƒCƒiƒX
//turnX = Xƒ^[ƒ“‘±


[CreateAssetMenu(menuName ="Items/AddDigAreaW1H1")]
public class AddDigAreaW1H1 : ItemBase
{
    public int addArea = 1; //Šg‘å”ÍˆÍ

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

[CreateAssetMenu(menuName = "Items/AddDigAreaW2H_1")]
public class AddDigAreaW2H_1 : ItemBase
{
    public int addArea = 2; //Šg‘å”ÍˆÍ
    public int subArea = 1; //k¬”ÍˆÍ

    public override void OnHold(PlayerController player)
    {
        player.dig_width = (player.dig_width + addArea * 2);
        player.dig_height = (player.dig_height - subArea * 2);
    }
    public override void OnDelete(PlayerController player)
    {
        player.dig_width = (player.dig_width - addArea * 2);
        player.dig_height = (player.dig_height + subArea * 2);
    }
}

[CreateAssetMenu(menuName = "Items/AddDigAreaW_1H2")]
public class AddDigAreaW_1H2 : ItemBase
{
    public int addArea = 2; //Šg‘å”ÍˆÍ
    public int subArea = 1; //k¬”ÍˆÍ

    public override void OnHold(PlayerController player)
    {
        player.dig_width = (player.dig_width - subArea * 2);
        player.dig_height = (player.dig_height + addArea * 2);
    }
    public override void OnDelete(PlayerController player)
    {
        player.dig_width = (player.dig_width + subArea * 2);
        player.dig_height = (player.dig_height - addArea * 2);
    }
}

[CreateAssetMenu(menuName = "Items/AddDigLimit1turn3")] 
public class AddDigLimit1turn3 : ItemBase
{
    public int turn = 3;

    public override void OnUse(PlayerController player)
    {
        player.digLimit++;
        player.isActiveItems.Add(this);
    }

    public override void OnDelete(PlayerController player)
    {
        player.digLimit--;
        player.isActiveItems.Remove(this);
    }
}