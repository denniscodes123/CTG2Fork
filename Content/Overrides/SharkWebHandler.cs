using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using CTG2.Content.Items;
using Terraria.Audio;
using CTG2.Content.ClientSide;
using CTG2.Content.Buffs;
using Microsoft.Xna.Framework;


public class SharkWebHandler : ModPlayer
{
    public override void OnHurt(Player.HurtInfo info)
    {
        // Only run on the local player's own client
        if (Player.whoAmI != Main.myPlayer) return;
        
        var modPlayer = Player.GetModPlayer<PlayerManager>();
        int attackerIndex = info.DamageSource.SourcePlayerIndex;
        int projIndex = info.DamageSource.SourceProjectileLocalIndex;

        if (projIndex >= 0 && projIndex < Main.maxProjectiles)
        {
            Projectile proj = Main.projectile[projIndex];
            if (proj.active)
            {
                if (proj.type == 969)
                {
                    Player.AddBuff(149, 60);
                }
            }
        }
    }
}