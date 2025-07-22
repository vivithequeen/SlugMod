using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SlugMod.Content.Projectiles
{
	public class GrenadeBullet : ModProjectile
	{
		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
		}

		public override void SetDefaults() {
			Projectile.width = 8; // The width of projectile hitbox
			Projectile.height = 8; // The height of projectile hitbox
			Projectile.aiStyle = 1; // The ai style of the projectile, please reference the source code of Terraria
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
			Projectile.penetrate = 1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
			Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.alpha = 255; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
			Projectile.light = 0.5f; // How much light emit around the projectile
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = true; // Can the projectile collide with tiles?
			Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame

			AIType = ProjectileID.Bullet; // Act exactly like default Bullet
		}

		public override bool OnTileCollide(Vector2 oldVelocity) {
			// If collide with tile, reduce the penetrate.
			// So the projectile can reflect at most 5 times
			Projectile.penetrate--;
			if (Projectile.penetrate <= 0) {
				Projectile.Kill();
			}
			else {
				Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
				SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

				// If the projectile hits the left or right side of the tile, reverse the X velocity
				if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
					Projectile.velocity.X = -oldVelocity.X;
				}

				// If the projectile hits the top or bottom side of the tile, reverse the Y velocity
				if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
					Projectile.velocity.Y = -oldVelocity.Y;
				}
			}
			PrepareBombToBlow();
			return false;
		}

		public override bool PreDraw(ref Color lightColor) {
			// Draws an afterimage trail. See https://github.com/tModLoader/tModLoader/wiki/Basic-Projectile#afterimage-trail for more information.

			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = Projectile.oldPos.Length - 1; k > 0; k--) {
				Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			return true;
		}
		public override void PrepareBombToBlow() {
			Projectile.tileCollide = false; // This is important or the explosion will be in the wrong place if the bomb explodes on slopes.
			Projectile.alpha = 255; // Set to transparent. This projectile technically lives as transparent for about 3 frames

			// Change the hitbox size, centered about the original projectile center. This makes the projectile damage enemies during the explosion.
			Projectile.Resize(250, 250);

			Projectile.damage = 250; // Bomb: 100, Dynamite: 250
			Projectile.knockBack = 10f; // Bomb: 8f, Dynamite: 10f
		}
		public override void OnKill(int timeLeft)
		{
			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
			SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
			if (Projectile.owner == Main.myPlayer) {
				int explosionRadius = 2; // Bomb: 4, Dynamite: 7, Explosives & TNT Barrel: 10
				int minTileX = (int)(Projectile.Center.X / 16f - explosionRadius);
				int maxTileX = (int)(Projectile.Center.X / 16f + explosionRadius);
				int minTileY = (int)(Projectile.Center.Y / 16f - explosionRadius);
				int maxTileY = (int)(Projectile.Center.Y / 16f + explosionRadius);

				// Ensure that all tile coordinates are within the world bounds
				Utils.ClampWithinWorld(ref minTileX, ref minTileY, ref maxTileX, ref maxTileY);
 
				// These 2 methods handle actually mining the tiles and walls while honoring tile explosion conditions
				bool explodeWalls = Projectile.ShouldWallExplode(Projectile.Center, explosionRadius, minTileX, maxTileX, minTileY, maxTileY);
				Projectile.ExplodeTiles(Projectile.Center, explosionRadius, minTileX, maxTileX, minTileY, maxTileY, explodeWalls);
			}
		}
	}
}