using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CTG2.Content.Functionality
{
    public class MudHoneyCraft : ModSystem
    {
        public override void AddRecipes()
        {
            // Create a new recipe for 1 Mud Block
            Recipe recipe = Recipe.Create(ItemID.MudBlock, 1);
            recipe.AddIngredient(ItemID.DirtBlock, 1);
            recipe.AddCondition(Condition.NearHoney);
            recipe.Register();

            Recipe recipeFish1 = Recipe.Create(ItemID.FrostDaggerfish, 1);
            recipeFish1.AddIngredient(ItemID.AtlanticCod, 1);
            recipeFish1.Register();

            Recipe recipeFish2 = Recipe.Create(ItemID.BombFish, 1);
            recipeFish2.AddIngredient(ItemID.AtlanticCod, 2);
            recipeFish2.Register();

            Recipe recipeFish3 = Recipe.Create(ItemID.Snowball, 1);
            recipeFish3.AddIngredient(ItemID.AtlanticCod, 1);
            recipeFish3.Register();

            Recipe recipeFish4 = Recipe.Create(ItemID.ChumBucket, 1);
            recipeFish4.AddIngredient(ItemID.AtlanticCod, 2);
            recipeFish4.Register();
        }
    }
}