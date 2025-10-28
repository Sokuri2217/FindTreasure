using System.Globalization;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemManager/Heal")]
public class Heal : ScriptableObject,ItemInterface
{
    //ÌŒ@‰ñ”‚Ì‰ñ•œ
    public void UseItem(PlayerController player)
    {
        player.digCurrent++;
        if(player.digCurrent>player.digLimit )
        {
            player.digCurrent = player.digLimit;
        }
    }
}

[CreateAssetMenu(menuName = "ItemManager/AddScale1")]
public class AddScale1 : ScriptableObject, ItemInterface
{
    //ÌŒ@”ÍˆÍ‚ÌŠg‘å(‘S•ûŒü +1ƒ}ƒX)
    public void UseItem(PlayerController player)
    {
        player.dig_height += 2;
        player.dig_width += 2;
    }
}
