using System;
using System.Collections.Generic;
using System.Linq;
using CTG2.Content.ServerSide;
using CTG2.Content.Classes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.Chat;
using ReLogic.Graphics;


namespace CTG2.Content.ClientSide;

public class UIManager : ModSystem
{   
    private UserInterface classInterface;
    private ClassUI classUIState;
    
    
    public override void OnWorldLoad()
    {
        // 1) Create your UIState
        classUIState = new ClassUI();

        // 2) Create a UserInterface and attach your state
        classInterface = new UserInterface();
        classInterface.SetState(classUIState);
    }
    
    public override void UpdateUI(GameTime gameTime)
    {
        // Only update the class interface when ShowClassUI is true
        if (Main.LocalPlayer.GetModPlayer<PlayerManager>().ShowClassUI)
        {
            classInterface?.Update(gameTime);
        }
    }
    
    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
        if (index != -1)
        {
            
            if (Main.LocalPlayer.GetModPlayer<PlayerManager>().ShowClassUI)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "CTG2: Class Selection UI",
                    delegate
                    {
                        // Draw your UI
                        classInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
            
            layers.Insert(index + 1, new LegacyGameInterfaceLayer(
                "CTG2: Match Timer",
                delegate
                {
                    if (true)
                    {
                        DrawTopUI();
                        DrawAbilityCooldown();
                    }
                    return true;
                },
                InterfaceScaleType.UI)
            );
        }
    }


    private void DrawTopUI()
    {
        int matchStage = GameInfo.matchStage;

        if (matchStage == 0 || matchStage == 3) return;

        int matchTime = GameInfo.matchTime;
        int secondsElapsed = matchTime / 60 - GameInfo.matchStartTime / 60;
        int minutesElapsed = secondsElapsed / 60;
        int remainder = secondsElapsed % 60;

        int capDifference = GameInfo.blueCaptures - GameInfo.redCaptures;

        // --- Build center time text ---
        string timeText;
        if (matchTime < GameInfo.matchStartTime)
        {
            timeText = $"{(int)(GameInfo.matchStartTime / 60) - matchTime / 60}s";
        }
        else if (Main.LocalPlayer.GetModPlayer<PlayerManager>().classSelectionTimer > 0
            && Main.LocalPlayer.GetModPlayer<PlayerManager>().playerState == PlayerManager.PlayerState.ClassSelection)
        {
            timeText = $"{(int)(Main.LocalPlayer.GetModPlayer<PlayerManager>().classSelectionTimer) / 60}s";
        }
        else if (secondsElapsed > 600 && capDifference != 0)
        {
            timeText = "Kill gem holder!";
        }
        else
        {
            timeText = $"{minutesElapsed}:{remainder.ToString("D2")}";
        }

        // --- Layout constants ---
        DynamicSpriteFont font = FontAssets.MouseText.Value;
        float scale = 1f;
        int boxPaddingX = 14;
        int boxPaddingY = 6;
        int boxGap = 4;
        int topY = 10;

        // Measure each box's text
        Vector2 timeSize   = font.MeasureString(timeText) * scale;
        string redStr      = $"{GameInfo.redCaptures}";
        string blueStr     = $"{GameInfo.blueCaptures}";
        Vector2 redSize    = font.MeasureString(redStr) * scale;
        Vector2 blueSize   = font.MeasureString(blueStr) * scale;

        // Box dimensions
        int timeW   = (int)timeSize.X  + boxPaddingX * 2;
        int timeH   = (int)timeSize.Y  + boxPaddingY * 2;
        int redW    = (int)redSize.X   + boxPaddingX * 2;
        int blueW   = (int)blueSize.X  + boxPaddingX * 2;
        int boxH    = timeH; // all boxes same height

        // Total width, centered on screen
        int totalW  = redW + boxGap + timeW + boxGap + blueW;
        int startX  = (Main.screenWidth - totalW) / 2;

        // Box positions
        Rectangle redBox   = new Rectangle(startX,                        topY, redW,  boxH);
        Rectangle timeBox  = new Rectangle(startX + redW + boxGap,        topY, timeW, boxH);
        Rectangle blueBox  = new Rectangle(startX + redW + boxGap + timeW + boxGap, topY, blueW, boxH);

        // --- Draw boxes ---
        Texture2D pixel = TextureAssets.MagicPixel.Value;

        // Red box background + border
        Main.spriteBatch.Draw(pixel, redBox, Color.DarkRed * 0.85f);
        DrawBorder(Main.spriteBatch, pixel, redBox, Color.Red, 2);

        // White/neutral time box background + border
        Main.spriteBatch.Draw(pixel, timeBox, Color.Black * 0.75f);
        DrawBorder(Main.spriteBatch, pixel, timeBox, Color.White, 2);

        // Blue box background + border
        Main.spriteBatch.Draw(pixel, blueBox, new Color(0, 60, 140) * 0.85f);
        DrawBorder(Main.spriteBatch, pixel, blueBox, new Color(50, 150, 255), 2);

        // --- Draw text centered in each box ---
        Vector2 redTextPos  = new Vector2(redBox.X  + (redBox.Width  - redSize.X)  / 2,
                                        redBox.Y  + (redBox.Height - redSize.Y)  / 2);
        Vector2 timeTextPos = new Vector2(timeBox.X + (timeBox.Width - timeSize.X) / 2,
                                        timeBox.Y + (timeBox.Height - timeSize.Y) / 2);
        Vector2 blueTextPos = new Vector2(blueBox.X + (blueBox.Width - blueSize.X) / 2,
                                        blueBox.Y + (blueBox.Height - blueSize.Y) / 2);

        Utils.DrawBorderString(Main.spriteBatch, redStr,   redTextPos,  Color.White, scale);
        Utils.DrawBorderString(Main.spriteBatch, timeText, timeTextPos, Color.White, scale);
        Utils.DrawBorderString(Main.spriteBatch, blueStr,  blueTextPos, Color.White, scale);
    }

    // Helper to draw a rectangle outline
    private void DrawBorder(SpriteBatch sb, Texture2D pixel, Rectangle rect, Color color, int thickness)
    {
        // Top
        sb.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
        // Bottom
        sb.Draw(pixel, new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness), color);
        // Left
        sb.Draw(pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        // Right
        sb.Draw(pixel, new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height), color);
    }

    private void DrawAbilityCooldown()
    {
        int matchStage = GameInfo.matchStage;
        if (matchStage == 0 || matchStage == 3) return;

        var abilities = Main.LocalPlayer.GetModPlayer<Abilities>();
        int cooldown = abilities.cooldown;

        Texture2D icon = ModContent.Request<Texture2D>("CTG2/Content/ClientSide/AbilityIcon").Value;

        int iconSize = 32;
        int centerX = Main.screenWidth / 2;
        int centerY = Main.screenHeight - 32;
        Rectangle destRect = new Rectangle(centerX - iconSize / 2, centerY - iconSize / 2, iconSize, iconSize);

        if (cooldown <= 0)
        {
            Main.spriteBatch.Draw(icon, destRect, Color.White);
        }
        else
        {
            Main.spriteBatch.Draw(icon, destRect, Color.Gray * 0.6f);

            DynamicSpriteFont font = FontAssets.MouseText.Value;
            string cdText = $"{(int)Math.Ceiling(cooldown / 60f)}";
            Vector2 textSize = font.MeasureString(cdText);
            Vector2 textPos = new Vector2(centerX - textSize.X / 2f, centerY - textSize.Y / 2f);
            Utils.DrawBorderString(Main.spriteBatch, cdText, textPos, Color.White);
        }
    }
}