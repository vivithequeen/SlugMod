
using Iced.Intel;
using Microsoft.Build.Tasks;
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
        public int AttackTimer = 0;
        Vector2 targetPosition;
        float angle = MathHelper.Pi;
        bool onSecondPhase = false;

        Random rnd = new Random();
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.width = 46;
            NPC.height = 116;

            NPC.damage = 8;
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
            Player player = Main.player[NPC.target];
            if (!onSecondPhase)
            {
                if (NPC.life < NPC.lifeMax / 2)
                {
                    onSecondPhase = true;
                    SoundEngine.PlaySound(SoundID.Roar, player.position);

                    int type = ModContent.NPCType<LooksToTheMoon>();

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        // If the player is not in multiplayer, spawn directly
                        NPC.SpawnOnPlayer(player.whoAmI, type);
                    }
                    else
                    {
                        // If the player is in multiplayer, request a spawn
                        // This will only work if NPCID.Sets.MPAllowedEnemies[type] is true, which we set in MinionBossBody
                        NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                    }
                }
            }
            // This should almost always be the first code in AI() as it is responsible for finding the proper player target
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }

            

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


            /*float angle = fromPlayer.ToRotation();
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
            
            */
            NPC.netUpdate = true;

            if (MoveTimer >= 120)
            {
                MoveTimer = 0;
                float newAngle = (float)(rnd.NextDouble() * MathHelper.TwoPi);
                while (Math.Abs(newAngle - angle) < MathHelper.Pi / 3.0)
                {
                    newAngle = (float)(rnd.NextDouble() * MathHelper.TwoPi);
                }
                angle = newAngle;
                //make so he cant chose somewhere too close
                //dash?
            }

            else
            {
                MoveTimer += 1;
            }
            if (AttackTimer == 170)
            {
                targetPosition = Main.player[NPC.target].Center;
            }
            if (AttackTimer >= 180)
            {

                if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int attackType = (int)(rnd.NextDouble() * 3);
                    var source = NPC.GetSource_FromAI();
                    Vector2 position = NPC.Center;

                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    if (attackType == 0)
                    {

                        direction.Normalize();
                        float projSpeed = 10f;

                        int type = ProjectileID.HallowBossRainbowStreak;

                        int damage = NPC.damage;

                        Projectile.NewProjectile(source, position, direction * projSpeed, type, damage, 0f, Main.myPlayer);

                    }
                    else if (attackType == 1)
                    {

                        float projSpeed = 10f;

                        int type = ProjectileID.SniperBullet;

                        int damage = NPC.damage;

                        Projectile.NewProjectile(source, position, direction * projSpeed, type, damage, 0f, Main.myPlayer);
                    }
                    else if (attackType == 2)
                    {
                        float projSpeed = 10f;

                        int type = ProjectileID.StarWrath;

                        int damage = NPC.damage;

                        Projectile.NewProjectile(source, position, direction * projSpeed, type, damage, 0f, Main.myPlayer);

                    }
                }
                AttackTimer = 0;

            }
            else
            {
                AttackTimer += 1;
            }
            pos.X = player.Center.X + distance * (float)Math.Cos((double)(angle));
            pos.Y = player.Center.Y + distance * (float)Math.Sin((double)(angle));


            // Move along the vector
            Vector2 toDestination = pos - NPC.Center;
            Vector2 toDestinationNormalized = toDestination.SafeNormalize(Vector2.UnitY);
            float speed = Math.Min(distance, toDestination.Length());
            NPC.velocity = toDestinationNormalized * speed / 10;



            //NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;


        }

    }
}