
using Microsoft.Xna.Framework;
using SlugMod.Content.Items.Consumables;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SlugMod.Content.NPCs.Bosses
{
    [AutoloadBossHead]
    public class FivePebbles : ModNPC
    {

        public int MoveTimer = 0;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.width = 46;
            NPC.height = 116;

            NPC.damage = 12;
            NPC.defense = 15;


            NPC.lifeMax = 2000;


            NPC.knockBackResist = 0f;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.noGravity = true;
            NPC.noTileCollide = true;

            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.npcSlots = 10f;

            NPC.aiStyle = -1;

        }


        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<FivePebblesBossBag>()));
        }
        public override void AI()
        {
            // This should almost always be the first code in AI() as it is responsible for finding the proper player target
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }

            Player player = Main.player[NPC.target];

            if (player.dead)
            {
                // If the targeted player is dead, flee
                NPC.velocity.Y -= 0.04f;
                // This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
                NPC.EncourageDespawn(10);
                return;
            }


            float distance = 200; // Distance in pixels behind the player
            Vector2 pos;

            Vector2 fromPlayer = NPC.Center - player.Center;


            float angle = fromPlayer.ToRotation();
            float twelfth = MathHelper.Pi / 6;
            angle += MathHelper.Pi + Main.rand.NextFloat(-twelfth, twelfth);
            if (angle > MathHelper.TwoPi)
            {
                angle -= MathHelper.TwoPi;
            }
            else if (angle < 0)
            {
                angle += MathHelper.TwoPi;
            }
            Vector2 relativeDestination = angle.ToRotationVector2() * distance;
            pos = player.Center + relativeDestination;
            NPC.netUpdate = true;
            


            // Move along the vector
            Vector2 toDestination = pos - NPC.Center;
            Vector2 toDestinationNormalized = toDestination.SafeNormalize(Vector2.UnitY);
            float speed = Math.Min(distance, toDestination.Length());
            NPC.velocity = toDestinationNormalized * speed / 30;

     

            //NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;


        }

    }
}