using SlugMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SlugMod.Content.Items.Weapons
{
	public class JokeRifle : ModItem
	{
		public override void SetDefaults() {
			// Modders can use Item.DefaultToRangedWeapon to quickly set many common properties, such as: useTime, useAnimation, useStyle, autoReuse, DamageType, shoot, shootSpeed, useAmmo, and noMelee. These are all shown individually here for teaching purposes.

			// Common Properties
			Item.width = 62; // Hitbox width of the item.
			Item.height = 32; // Hitbox height of the item.
			Item.scale = 2.0f;
			Item.rare = ItemRarityID.Green; // The color that the item's name will be in-game.

			// Use Properties
			Item.useTime = 8; // The item's use time in ticks (60 ticks == 1 second.)
			Item.useAnimation = 8; // The length of the item's use animation in ticks (60 ticks == 1 second.)
			Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
			Item.autoReuse = true; // Whether or not you can hold click to automatically use it again.

			
            /*// The sound that this item plays when used.
			Item.UseSound = new SoundStyle($"{nameof(ExampleMod)}/Assets/Sounds/Items/Guns/ExampleGun") {
				Volume = 0.9f,
				PitchVariance = 0.2f,
				MaxInstances = 3,
			};
            */
			// Weapon Properties
			Item.DamageType = DamageClass.Ranged; // Sets the damage type to ranged.
			Item.damage = 20; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
			Item.knockBack = 5f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
			Item.noMelee = true; // So the item's animation doesn't do damage.

			// Gun Properties
			Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns in the vanilla source have this.
			Item.shootSpeed = 10f; // The speed of the projectile (measured in pixels per frame.) This value equivalent to Handgun
			Item.useAmmo = AmmoID.Bullet; // The "ammo Id" of the ammo item that this weapon uses. Ammo IDs are magic numbers that usually correspond to the item id of one item that most commonly represent the ammo type.
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Wood, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}

		// This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
		public override Vector2? HoldoutOffset() {
			return new Vector2(2f, -2f);
		}

	}
}