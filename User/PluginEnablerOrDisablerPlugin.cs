// ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
// *.txt files are not loaded automatically by TurboHUD
// you have to change this file's extension to .cs to enable it
// ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

using Turbo.Plugins.Default;

namespace Turbo.Plugins.User
{

    public class PluginEnablerOrDisablerPlugin : BasePlugin, ICustomizer
    {

        public PluginEnablerOrDisablerPlugin()
        {
            Enabled = true;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
        }

        // "Customize" methods are automatically executed after every plugin is loaded.
        // So these methods can use Hud.GetPlugin<class> to access the plugin instances' public properties (like decorators, Enabled flag, parameters, etc)
        // Make sure you test the return value against null!
        public void Customize()
        {

////////////////////////
// Default Plugins Setup
////////////////////////


	// Root
	// ====
        // @ Turn off F6 bounty summary table
            Hud.TogglePlugin<BountyTablePlugin>(false);
        // @ Turn off elemental % numbers under hp ball 
            Hud.TogglePlugin<DamageBonusPlugin>(false);
        // @ This is false by default. To activate make it "true" for developers
            Hud.TogglePlugin<DebugPlugin>(false);
        // @ Turn off the thin bar showing experience stats just above the skills bar
            Hud.TogglePlugin<ExperienceOverBarPlugin>(false);
        // @ Turn off the Game Clock and the Server Ip Address
            Hud.TogglePlugin<GameInfoPlugin>(true);
        // @ Turn off the Network Latency bar between the Primary Power globe and Skills Bar
            Hud.TogglePlugin<NetworkLatencyPlugin>(false);
        // @ Turn off the Automated Paragon Screen captured saved to disc
            Hud.TogglePlugin<ParagonCapturePlugin>(false);
        // @ ONLY IN MULTIPLAYER this will disable the 4 lines of "DPS dealt to monsters" displayed in the lower section of the portrait
            Hud.TogglePlugin<PortraitBottomStatsPlugin>(true);
        // @ Turn off "Rift Completion" popup when rift progress is greater than 90%
            Hud.TogglePlugin<NotifyAtRiftPercentagePlugin>(false); 
        // @ Turns off the rift percentage and valuation display in the Rift Bar
            Hud.TogglePlugin<RiftPlugin>(true);
                // See CustomizeDefault()
        // @ Turn off the numbers overlaying the health and skill globes
            Hud.TogglePlugin<ResourceOverGlobePlugin>(true);
        // Turn off Act Map normal/special bounty names and completion status
            Hud.TogglePlugin<WaypointQuestsPlugin>(true);


	// Actors
	// ======
        // @ Turn off Minimap chest marker
            Hud.TogglePlugin<ChestPlugin>(true);
        // @ This is usually off and should be set to TRUE. It displays a big X over clickable chests, stashes, stumps, etc.
            //Hud.TogglePlugin<ClickableChestGizmoPlugin>(false); // This is false by default. To activate make it "true"
            //Hud.GetPlugin<ClickableChestGizmoPlugin>().PaintOnlyWhenHarringtonWaistguardIsActive = false;
        // @ Turn off Minimap cursed event chest marker
            Hud.TogglePlugin<CursedEventPlugin>(true);
        // @ Draws white rectangle over clickable dead bodies on minimap - Great in Cow Quest.
            Hud.TogglePlugin<DeadBodyPlugin>(false);  // This is false by default. To activate make it "true"
        // @ Turns off the minimap display of Power Globes and Rift Orbs dropped by elites
            Hud.TogglePlugin<GlobePlugin>(true);
        // @ Turns off Oculus Ground Decorator
            Hud.TogglePlugin<OculusPlugin>(true);
        // @ Turns off the door icon display on minimaps
            Hud.TogglePlugin<PortalPlugin>(true);
        // @ Turn off the weapon rack minimap location dots
            Hud.TogglePlugin<RackPlugin>(false);

	// OLD
            //Hud.GetPlugin<ShrinePlugin>().Enabled = false;

	// BuffLists
	// =========
        // @ Turn off buf icons to right side of mini map dealing with Passive skills that cheat death.
            Hud.TogglePlugin<CheatDeathBuffFeederPlugin>(true);
        // @ Turn off the Convention of Elements timer glyph
            //Hud.TogglePlugin<ConventionOfElementsBuffListPlugin>(false);
        // @ Turn off displays for Shrine and Pylon shaped icons added to the square yellow boxes
            Hud.TogglePlugin<MiniMapLeftBuffListPlugin>(true);
                // See CustomizeDefault()
        // @ Turns off various Passive skill timer countdowns
            Hud.TogglePlugin<MiniMapRightBuffListPlugin>(true);
        // @ Turn off the small bar display above each skill at the bottom
            Hud.TogglePlugin<PlayerBottomBuffListPlugin>(true);
                // See CustomizeDefault()
        // @ Turns off display of timer countdown on various Shrines and Pylons
            Hud.TogglePlugin<PlayerLeftBuffListPlugin>(true);
        // @ This plugin should be set to "true". It paints icons and counters and timers above the skills bar.
            Hud.TogglePlugin<PlayerRightBuffListPlugin>(true);
        // @ This plugin should be set to "true". It paints icons and counters and timers above the skills bar.
            Hud.TogglePlugin<PlayerTopBuffListPlugin>(true);
        // @ This plugin should be set to "true". It paints icons and counters and timers above the skills bar.
            Hud.TogglePlugin<TopLeftBuffListPlugin>(true);
        // @ This plugin should be set to "true". It paints icons and counters and timers above the skills bar.
            Hud.TogglePlugin<TopRightBuffListPlugin>(true);

	// CooldownSoundPlayerPlugin
	// =========================
            //Hud.TogglePlugin<CooldownSoundPlayerPlugin>(false);
                // See CustomizeDefault()

	// Decorators
	// ==========
        // This is not a plugin, just a helper class to display labels on the ground
        // Required for all other plugin decorators to work!
        // @ Turn off the displays of circles under monsters
            //Hud.TogglePlugin<GroundLabelDecoratorPainterPlugin>(true);

	// Inventory
	// =========
        // Numbers under skill bar on the right side - under Inventory
            Hud.TogglePlugin<BloodShardPlugin>(true);
        // Numbers under skill bar on the right side - under Quest and Achievements
            Hud.TogglePlugin<InventoryFreeSpacePlugin>(true);
        // @ Turn off Ancient and Primal markers on items
            Hud.TogglePlugin<InventoryAndStashPlugin>(true);
                // See CustomizeDefault()
        // Cubed Item in Character profile
            Hud.TogglePlugin<InventoryKanaiCubedItemsPlugin>(true);
        // @ Turn off the materials displayed in the extra display bar below the Inventory
            Hud.TogglePlugin<InventoryMaterialCountPlugin>(true);
        // @ Turn off the display when hovered over the Stash tabs 
            Hud.TogglePlugin<StashPreviewPlugin>(false);
        // @ Turn off the numbers on the stash tabs displaying used spaces
            Hud.TogglePlugin<StashUsedSpacePlugin>(true);

	// Items
	// =====
        // Minimap cosmetic item markers
            Hud.TogglePlugin<CosmeticItemsPlugin>(true);
        // @ Turn off the icon displays for Ancients and Primals and item ilvl
            Hud.TogglePlugin<HoveredItemInfoPlugin>(true);
        // Minimap markers for ancient and primal items
        // This plugin defines various item displays and should be used or modified, but not turned off unless it's replaced with a modified one.
            Hud.TogglePlugin<ItemsPlugin>(true);
                // See CustomizeDefault()
        // Show pickup range under player feet
            Hud.TogglePlugin<PickupRangePlugin>(true);

	// LabelLists
	// ==========
        // @ Turn off numbers over skillbar (miscellaneous info about skills and player attributes)
            Hud.TogglePlugin<AttributeLabelListPlugin>(false);
        // @ Turn off the Paragon and Experience boxes at the center top of the screen
            Hud.TogglePlugin<TopExperienceStatistics>(true);

	// Minimap
	// =======
        // Minimap POI markers like bounty or keywarden names
            Hud.GetPlugin<MarkerPlugin>().Enabled = true;
        // ???????????????????????????????????????????????????????????????????????????????????????????
            //Hud.GetPlugin<SceneHintPlugin>().Enabled = true;

	// Monsters
	// ========
        // @ Disable types of monsters on the minimap
            Hud.TogglePlugin<StandardMonsterPlugin>(true); // Elite on minimap
        // Minimap Draws small circle red and name over named dangerous monsters
            Hud.TogglePlugin<DangerousMonsterPlugin>(false);
                // See CustomizeDefault().
        // @ Disable labels showing elite monster powers
            //Hud.TogglePlugin<EliteMonsterAffixPlugin>(false); // Elite label monster affix
        // @ Diable elite monster skill decorators
            Hud.TogglePlugin<EliteMonsterSkillPlugin>(true); // Elite circle ground skill effect
                // See CustomizeDefault()
        // @ Disable explosive monster GroundCircleDecorator
            Hud.TogglePlugin<ExplosiveMonsterPlugin>(true);
        // @ Turn off the various goblin type displays
            Hud.TogglePlugin<GoblinPlugin>(true);
        // Group Elite health bar and draw line between them
            Hud.TogglePlugin<MonsterPackPlugin>(false); // default to false
        // Minimap rift progression monster colors
            Hud.TogglePlugin<MonsterRiftProgressionColoringPlugin>(true);
        // Current monster health bar: show monster health in numbers and in percentage and status ailsment above it.
            Hud.TogglePlugin<TopMonsterHealthBarPlugin>(false);

	// Players
	// =======
        // ??????
            //Hud.TogglePlugin<BannerPlugin>(true);
        // @ Turns off the tumbstone
            Hud.TogglePlugin<HeadStonePlugin>(true);
        // @ Turns off the "Strength in Numbers" buff in multiplayer games
            Hud.TogglePlugin<MultiplayerExperienceRangePlugin>(false);
        // @  Draws label for player name over player character when more than 1 player.
            Hud.TogglePlugin<OtherPlayersPlugin>(true);
        // @ Turn off the popups for skills when they are hovered over
            Hud.TogglePlugin<PlayerSkillPlugin>(false);
                // See CustomizeDefault()
        // Deals with elemental display while hovering over skills
            Hud.TogglePlugin<SkillRangeHelperPlugin>(false);

	// SkillBars
	// =========
        // Skillbar health potion cooldown timer
            Hud.TogglePlugin<OriginalHealthPotionSkillPlugin>(false);
        // @ Turns off the highlighted bonus damage numbers under the skillbar icons and next to the skill panel
            Hud.TogglePlugin<OriginalSkillBarPlugin>(false);
                // See CustomizeDefault()
        // Don't know if this draws anything but disable it anyway!
            //Hud.TogglePlugin<UiHiddenPlayerSkillBarPlugin>(false);



////////////////////////////////
// Default PLugins Customization
////////////////////////////////

/*
            var p = Hud.Sno.SnoPowers;
*/
	// Root 
	// ====
            Hud.RunOnPlugin<RiftPlugin>(plugin =>
            {
                //plugin.NephalemRiftPercentEnabled = true;
                //plugin.GreaterRiftPercentEnabled = true;
                //plugin.GreaterRiftTimerEnabled = true;
                //plugin.ChallengeRiftPercentEnabled = true;
                //plugin.ChallengeRiftTimerEnabled = true;
                plugin.NearMonsterProgressionEnabled = false; // @ Kill all "X% in Y yards" display.
            });
/*
	// Actors
	// ======
            Hud.RunOnPlugin<ChestPlugin>(plugin =>
            {
                plugin.LoreChestDecorator.Radius = 1.2f;            // OG value = 1.2
                plugin.NormalChestDecorator.Radius = 0.75f;         // OG value = 1
                plugin.ResplendentChestDecorator.Radius = 0.75f;    // OG value = 1

                //plugin.LoreChestDecorator.Decorators.Add(new MapLabelDecorator(Hud)
                //{
                //    LabelFont = Hud.Render.CreateFont("tahoma", 6, 255, 235, 120, 0, true, false, false),
                //    RadiusOffset = 14,
                //    Up = true,
                //});
            });
*/
	// BuffLists
	// =========
            Hud.RunOnPlugin<MiniMapLeftBuffListPlugin>(plugin =>
            {
            // Hide most extras - draws buf icons to left side of mini map.
            // You could also remove: ui_default_buffs.xml
                //plugin.RuleCalculator.Rules.Clear();
            // During Greater Rift only.
                //plugin.RuleCalculator.Rules.Add(new BuffRule(266254) { MinimumIconCount = 1 }); // Shield - invincible
                //plugin.RuleCalculator.Rules.Add(new BuffRule(262935) { MinimumIconCount = 1 }); // Power - 5x damage
            });

            Hud.RunOnPlugin<PlayerBottomBuffListPlugin>(plugin =>
            {
                //plugin.BuffPainter.ShowTimeLeftNumbers = true;
                // Set item special skill - centered on the screen!
                //plugin.RuleCalculator.Rules.Clear();
                //plugin.RuleCalculator.Rules.Add(new BuffRule(359583) { IconIndex = 1, ShowStacks = false, ShowTimeLeft = false }); // Focus
                //plugin.RuleCalculator.Rules.Add(new BuffRule(359583) { IconIndex = 2, ShowStacks = false, ShowTimeLeft = false }); // Restraint
                //plugin.RuleCalculator.Rules.Add(new BuffRule(p.TaegukPrimary.Sno) { IconIndex = null, ShowStacks = true, ShowTimeLeft = false });
                //plugin.RuleCalculator.Rules.Add(new BuffRule(p.GogokOfSwiftnessPrimary.Sno) { IconIndex = null, ShowStacks = true, ShowTimeLeft = false });
            });

	// CooldownSoundPlayer
	// ===================

	// Decorators
	// ==========

	// Inventory
	// =========
            Hud.RunOnPlugin<InventoryAndStashPlugin>(plugin =>
            {
                plugin.LooksGoodDisplayEnabled = false;              // pickit -- green mark
                plugin.NotGoodDisplayEnabled = true;               // grey out items in inventory // turn off sell darkening
                plugin.DefinitelyBadDisplayEnabled = false;         // pickit -- red mark
                plugin.HoradricCacheEnabled = true;                 // show act and difficulty on cache
                plugin.CanCubedEnabled = false;                     // disable indicator if an item can be cubed
                plugin.AncientRankEnabled = true;                   // Print A or P for Ancient and Primal
                plugin.CaldesannRankEnabled = true;                 // Print A or P + Augment level
                plugin.SocketedLegendaryGemRankEnabled = true;     // ??
            });

	// Items
	// =====
            Hud.RunOnPlugin<ItemsPlugin>(plugin =>
            {
                plugin.EnableSpeakPrimal = true;
                plugin.EnableSpeakPrimalSet = true;

            // Disable Label
                plugin.LegendaryDecorator.ToggleDecorators<GroundLabelDecorator>(false);
                plugin.AncientDecorator.ToggleDecorators<GroundLabelDecorator>(true);
                plugin.PrimalDecorator.ToggleDecorators<GroundLabelDecorator>(true);
                plugin.SetDecorator.ToggleDecorators<GroundLabelDecorator>(false);
                plugin.AncientSetDecorator.ToggleDecorators<GroundLabelDecorator>(true);
                plugin.PrimalSetDecorator.ToggleDecorators<GroundLabelDecorator>(true);
                //plugin.UtilityDecorator.ToggleDecorators<GroundLabelDecorator>(false);          // ??
                //plugin.NormalKeepDecorator.ToggleDecorators<GroundLabelDecorator>(false);       // default false
                //plugin.MagicKeepDecorator.ToggleDecorators<GroundLabelDecorator>(false);        // default false
                //plugin.RareKeepDecorator.ToggleDecorators<GroundLabelDecorator>(false);         // default false
                //plugin.LegendaryKeepDecorator.ToggleDecorators<GroundLabelDecorator>(false);    // ??
                //plugin.BookDecorator.ToggleDecorators<GroundLabelDecorator>(false);
                plugin.DeathsBreathDecorator.ToggleDecorators<GroundLabelDecorator>(false);
                plugin.InArmorySetDecorator.ToggleDecorators<GroundLabelDecorator>(true);       // Label in armory

            // Disable Circle
                plugin.LegendaryDecorator.ToggleDecorators<GroundCircleDecorator>(false);
                plugin.AncientDecorator.ToggleDecorators<GroundCircleDecorator>(true);
                plugin.PrimalDecorator.ToggleDecorators<GroundCircleDecorator>(true);
                plugin.SetDecorator.ToggleDecorators<GroundCircleDecorator>(false);
                plugin.AncientSetDecorator.ToggleDecorators<GroundCircleDecorator>(true);
                plugin.PrimalSetDecorator.ToggleDecorators<GroundCircleDecorator>(true);
                //plugin.UtilityDecorator.ToggleDecorators<GroundCircleDecorator>(false);
                //plugin.NormalKeepDecorator.ToggleDecorators<GroundCircleDecorator>(false);
                //plugin.MagicKeepDecorator.ToggleDecorators<GroundCircleDecorator>(false);
                //plugin.RareKeepDecorator.ToggleDecorators<GroundCircleDecorator>(false);
                //plugin.LegendaryKeepDecorator.ToggleDecorators<GroundCircleDecorator>(false);
                //plugin.BookDecorator.ToggleDecorators<GroundCircleDecorator>(false);
                plugin.DeathsBreathDecorator.ToggleDecorators<GroundCircleDecorator>(false);
                plugin.InArmorySetDecorator.ToggleDecorators<GroundCircleDecorator>(true);

            // Show ancient and primal items on mini map.
                plugin.AncientDecorator.Decorators.Add(new MapLabelDecorator(Hud)
                {
                    LabelFont = Hud.Render.CreateFont("tahoma", 6, 255, 235, 120, 0, true, false, false),
                    RadiusOffset = 14,
                    Up = true,
                });
                plugin.AncientSetDecorator.Decorators.Add(new MapLabelDecorator(Hud)
                {
                    LabelFont = Hud.Render.CreateFont("tahoma", 6, 255, 0, 170, 0, true, false, false),
                    RadiusOffset = 14,
                    Up = true,
                });
                plugin.PrimalDecorator.Decorators.Add(new MapLabelDecorator(Hud)
                {
                    LabelFont = Hud.Render.CreateFont("tahoma", 7, 255, 240, 20, 0, true, false, false),
                    RadiusOffset = 14,
                    Up = true,
                });
                plugin.PrimalSetDecorator.Decorators.Add(new MapLabelDecorator(Hud)
                {
                    LabelFont = Hud.Render.CreateFont("tahoma", 7, 255, 240, 20, 0, true, false, false),
                    RadiusOffset = 14,
                    Up = true,
                });

            // Add ground circle for Death Breaths.
                plugin.DeathsBreathDecorator.Add(new GroundCircleDecorator(Hud)
                {
                    //Brush = Hud.Render.CreateBrush(192, 102, 202, 177, -2),
                    //Radius = 1.25f,
                });
            });

            Hud.RunOnPlugin<PickupRangePlugin>(plugin =>
            {
                // Make Pickup Range indicator more "prominent". Some builds might find this useful, though.
                //plugin.Enabled = false;
                //plugin.FillBrush = Hud.Render.CreateBrush(4, 255, 255, 255, 0);
                //plugin.OutlineBrush = Hud.Render.CreateBrush(20, 0, 0, 0, 3);
            });

	// LabelLists
	// ==========

	// Minimap
	// =======

	// Monsters
	// ========

/*
            Hud.RunOnPlugin<DangerousMonsterPlugin>(plugin =>   // If you enabled this, here are my customizations!
            {
                plugin.Enable = true;
                plugin.DisableBoss = true;
                plugin.DisableMinions = true;
            });
*/

            Hud.RunOnPlugin<DangerousMonsterPlugin>(plugin =>   // If you enabled this, here are my customizations!
            {
                //plugin.Order = 501;                             // Draw after MyMonsterColoring.
                //plugin.AddNames("Enslaved Nightmare");          // "Terror Demon" is included already.
                //foreach (var decorator in plugin.Decorator.GetDecorators<MapShapeDecorator>())
                //{
                //    decorator.Radius = 3;                       // Increase radius for better visibility.
                //}
            });

            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Arcane);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Desecrator);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Electrified);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Frozen);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.FrozenPulse);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Jailer);
            //Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Juggernaut); // Jugg
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Molten);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Mortar);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Orbiter); // Orb
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Plagued);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Poison);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Reflect);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Thunderstorm);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Waller);
            // Weak
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.ExtraHealth);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.HealthLink);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Fast);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.FireChains);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Knockback);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Nightmarish);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Illusionist);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Shielding);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Teleporter);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Vampiric);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Vortex);
           	Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Wormhole);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Avenger);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.Horde);
            Hud.GetPlugin<EliteMonsterAffixPlugin>().AffixDecorators.Remove(MonsterAffix.MissileDampening);

            //Hud.GetPlugin<EliteMonsterSkillPlugin>().ArcaneDecorator.Enabled = false;
            //Hud.GetPlugin<EliteMonsterSkillPlugin>().ArcaneSpawnDecorator.Enabled = false;
            Hud.GetPlugin<EliteMonsterSkillPlugin>().DesecratorDecorator.Enabled = false;
            //Hud.GetPlugin<EliteMonsterSkillPlugin>().FrozenBallDecorator.Enabled = false;
            //Hud.GetPlugin<EliteMonsterSkillPlugin>().FrozenPulseDecorator.Enabled = false;
            //Hud.GetPlugin<EliteMonsterSkillPlugin>().GhomDecorator.Enabled = false;
            Hud.GetPlugin<EliteMonsterSkillPlugin>().MoltenDecorator.Enabled = false;
            //Hud.GetPlugin<EliteMonsterSkillPlugin>().MoltenExplosionDecorator.Enabled = false;
            Hud.GetPlugin<EliteMonsterSkillPlugin>().PlaguedDecorator.Enabled = false;
            //Hud.GetPlugin<EliteMonsterSkillPlugin>().ThunderstormDecorator.Enabled = false;

            Hud.RunOnPlugin<EliteMonsterSkillPlugin>(plugin =>
            {
                /*
                plugin.ArcaneDecorator.ToggleDecorators<IWorldDecorator>(true);
                plugin.ArcaneSpawnDecorator.ToggleDecorators<IWorldDecorator>(true);
                plugin.DesecratorDecorator.ToggleDecorators<IWorldDecorator>(true);
                plugin.FrozenBallDecorator.ToggleDecorators<IWorldDecorator>(true);
                plugin.FrozenPulseDecorator.ToggleDecorators<IWorldDecorator>(true);
                plugin.GhomDecorator.ToggleDecorators<IWorldDecorator>(true);
                plugin.MoltenDecorator.ToggleDecorators<IWorldDecorator>(true);
                plugin.MoltenExplosionDecorator.ToggleDecorators<IWorldDecorator>(true);
                plugin.PlaguedDecorator.ToggleDecorators<IWorldDecorator>(true);
                plugin.ThunderstormDecorator.ToggleDecorators<IWorldDecorator>(true);
                */
            });

	// Players
	// =======
            Hud.RunOnPlugin<PlayerSkillPlugin>(plugin =>
            {
                //plugin.InnerSanctuarySanctifiedGroundDecorator.Enabled = false;
            });

	// SkillBar
	// ========

            Hud.RunOnPlugin<OriginalSkillBarPlugin>(plugin =>
            {
            	//plugin.SkillPainter.TextureOpacity = 0.1f;
            	plugin.SkillPainter.EnableSkillDpsBar = false;
            	plugin.SkillPainter.EnableDetailedDpsHint = false;
            });

////////////////////////////////////
// Third party Plugins Customization
////////////////////////////////////

    // johnbl
    // ======
			// draws a cursor on the minimap
            Hud.RunOnPlugin<Custom.MinimapCursorPlugin>(plugin =>
            {
                plugin.Enabled = true;
                plugin.ShowInTown = false;
                /*plugin.MiniMapVisorDecorator = new WorldDecoratorCollection(
                new MapShapeDecorator(Hud)
                {
                    Brush = Hud.Render.CreateBrush(255, 255, 255, 255, 1), // Alpha, Red, Green, Blue, Width
                    ShapePainter = new PlusShapePainter(Hud), // List of shapes in plugins\Default\ShapePainters
                    Radius = 10,
                },
   
                new MapShapeDecorator(Hud)
                {
                    Brush = Hud.Render.CreateBrush(255, 255, 255, 255, 1),
                    ShapePainter = new CircleShapePainter(Hud),
                    Radius = 5,
                });*/
            });

    // RuneB
    // ====
			// Shows the remaining cooldown on chosen partymember skills, using small icons in the top of the screen.
			// Skills can be watched by adding their sno's to the WatchedSnos list - there are commented examples.
            Hud.RunOnPlugin<Custom.PartyCooldownsPlugin>(plugin =>
            {
                plugin.Enabled = true;
            });

	// BM
	// ===
			// Shows ShockTower on minimap and on ground.
            Hud.RunOnPlugin<Custom.ShockTowerPlugin>(plugin =>
            {
                plugin.Enabled = true;
            });
	
    // CB
    // ====
			// Add circle under Blue or Yellow Monsters
            Hud.RunOnPlugin<Custom.MonsterCirclePlugin>(plugin =>
            {
                plugin.Enabled = true;
                plugin.ShowMinions = false;
            }); 

	// SHAKE
	// =====

			// This plugin draws a green circle at znec on minimap and feet to helps on rath runs..
			// Also shows your simulacrum remain active time on feet, its possible to see the mages and coe.
            Hud.RunOnPlugin<Custom.RatrunsPlugin>(plugin => 
            {
                plugin.Enabled = true;
            });

            //Hud.RunOnPlugin<Custom.GoodMonsterPlugin>(plugin => 
            //{
            //    plugin.Enabled = false;
            //});

	// JACK
	// ====
			// Display ground symbol & map shape for Doors, Breakable Doors and Bridges.
            Hud.RunOnPlugin<Custom.DoorsPlugin>(plugin =>
            {
                plugin.Enabled = true;
                plugin.Debug = false; // debug
                plugin.DebugEnabled = true; // debug
                plugin.ShowInTown = true;
                plugin.GroundLabelsOnScreen = true;
                plugin.GroundSymbol = "🚪"; // 🚪
                plugin.DoorsDecorators.ToggleDecorators<GroundLabelDecorator>(true);
                plugin.BreakablesDoorsDecorators.ToggleDecorators<GroundLabelDecorator>(true);
                plugin.BridgesDecorators.ToggleDecorators<GroundLabelDecorator>(true);
            }); 

	// Thd3fp
	// ======
            Hud.GetPlugin<Custom.HallOfAgonyShortcutsHints>().Enabled = true;
            Hud.RunOnPlugin<Custom.HallOfAgonyShortcutsHints>(plugin =>
            {
                // disabled the plugin when we in the corner - avoiding overlap
                plugin.ListOverlapPlugin.Add(Hud.GetPlugin<Default.ConventionOfElementsBuffListPlugin>()); 
            });
			
    // DAV
    // ===

			// circle for the skeleton & mage
			// (original plugin zx-necromancerskeletonindicatorplugin.html ([v7.3] [ENGLISH] [ZX] NecromancerSkeletonIndicatorPlugin) )
			// filled circle for skeleton when using active skill
			// add star under target (elite only) of the active command skeleton
			
			// add line on mini-map for target elite
			// target star will not effect on rare minions

			// add buff time of bone ringer (local user only)
			// 2 type of timer will be shown (time for target is within the thud detection range & the time when skill is used, optional)
			// buff time for kill comment will not correct if the buff is not active successfully
            Hud.RunOnPlugin<DAV.DAV_NecroPetPlugin>(plugin => 
            {
                plugin.Enabled = false;
                plugin.ShowSkeleton = false;
                plugin.TargetOnEliteOnly = false;// add decorator for non elite target (optional, default enable)
                plugin.ShowSkeletonOthers = false; // add option to show skeleton of other players
                plugin.ShowMage = false;
                plugin.ShowGolem = false; // add circle for golem
                plugin.ShowSimulacrum = true;
                plugin.ShowSimulacrumOthers = false;
                plugin.Label_Simulacrum = "My Sim";
                plugin.Label_SimulacrumOthers = "Sim";
                plugin.ShowRevive = false; // add circle for revive
                //plugin.SkillPress = false;

                plugin.GRonly = false;
                plugin.Bossonly = false;
                plugin.showTimerShort = true;
                plugin.showTimerLong = true;
            });

			// This plugin show who has not upgrade the gems yet after the GR
            Hud.RunOnPlugin<DAV.DAV_UrshiPlugin>(plugin => 
            {
                plugin.Enabled = true;
            });

	// RESU
	// ====

			// Shows player names above banner in town.
            Hud.RunOnPlugin<Resu.BattleTagAboveBanner>(plugin =>
            {
                plugin.Enabled = true;
                plugin.SeePlayersInTown = false;
            });

			// MUST BE ON FOR DangerPlugin
            Hud.RunOnPlugin<Resu.HotEnablerDisablerPlugin>(plugin =>
            {
                plugin.Enabled = true;
            });

			// Circles around Blood springs on the floor and on the minimap (Paths of the Drowned & Blood Marsh area)(previously BloodSpringsPlugin).
			// Circles around Shock Towers on the floor and on the minimap (from DM's ShockTowerPlugin).
			// Triangles around Demon Forges flames on the floor and indicator on the minimap (heavily modified DM's DemonForgePlugin).
			// Circles around Arcane enchanted / Circles around Demon Mines.
			// Thunderstorm, Plagued, Molten, Morlu's Meteor & Desecrator move! warnings when player is exposed.
			// Crosses on the ground for poison enchanted.
			// SandWasp's Projectile indicator.
            Hud.RunOnPlugin<Resu.DangerPlugin>(plugin =>
            { 
                plugin.BloodSprings = true; 
                plugin.DemonicForge = true;
                plugin.ShockTower = true;
                plugin.Desecrator = true;
                plugin.Thunderstorm = true;
                plugin.Plagued = true;
                plugin.Molten = true;
                plugin.ArcaneEnchanted = true;
                plugin.PoisonEnchanted = true;
                plugin.GasCloud = true; // (Ghom)
                plugin.SandWaspProjectile = true;
                plugin.MorluSpellcasterMeteorPending = true;
                plugin.DemonMine = true;
                plugin.PoisonDeath = true;
                plugin.MoltenExplosion = true;
                plugin.Orbiter = true;
                plugin.GrotesqueExplosion = true;
                plugin.BetrayedPoisonCloud = true;

                plugin.BloodStar = true;
                plugin.ArrowProjectile = true;
                plugin.BogFamilyProjectile = true;
                plugin.bloodGolemProjectile = true;
                plugin.MoleMutantProjectile = true;
                plugin.IcePorcupineProjectile = true;
            });

			// Simple color filter that kills shiny colors and brings a darker Diablo 3.
            Hud.RunOnPlugin<Resu.DarkerDiablo3Plugin>(plugin =>
            {
                plugin.Enabled = false;
            });

			// Displays an advised group GRift level when rift dialogue is open (+ player battletag, Highest solo GR level, class).
			// When Rift or Grift is over and all players are in town, the Nephalem Obelisk tells you you can close.
			// Red circle around 5% of Rift completion monster groups on minimap.
			// "Talk to Urshi" reminder when teleporting after Greater rift.
			// support for Z class (ZDPS, Sup).
            Hud.RunOnPlugin<Resu.GroupGRLevelAdviserPlugin>(plugin =>
            {
                plugin.Enabled = true;
                plugin.RedCircle = true;                  // Set to false if you don't want the red circle
                plugin.PackLeaderLifePercentage = false;   // Set to false if you don't want the life percentage on elites
                plugin.TimeToGRBoss = true;               // Set to false if you don't want the time left to boss fight in Grift bar 
            });

			// Shows EXP percentage with two decimal right to portrait paragon level with EXP/h & time to next paragon level
			// Show highest Greater Rift level with Class, Sheet DPS, EHP and Nemesis Bracers indicator in the hint.
            Hud.RunOnPlugin<Resu.ParagonPercentagePlugin>(plugin =>
            {
                plugin.Enabled = true;
                plugin.ParagonPercentageOnTheRight = false;
                plugin.ShowGreaterRiftMaxLevel = true; // set to true to disable GR level display
                plugin.DisplayParagonPercentage = false; // set to false to disable paragon percentage display
                plugin.NPCDeco = false; // set to false to disable NPC decorator
            });

			// Urshi's gift plugin adds GR level for n% chance of upgrade on the bottom-right
			// of your legendary gems in your stash and your inventory,
			// maxed Gems are labelled "max", hint in itemhovered menu.
            Hud.RunOnPlugin<Resu.UrshisGiftPlugin>(plugin =>
            {
                plugin.Enabled = true;
                plugin.ChanceWantedPercentage = 60;		// % chance wanted : 100; 90; 80; 70; 60; 30; 15; 8; 4; 2; 1;
                plugin.NumberOfAttempts = 3;			// Number of consecutive attempts at this % : 1; 2; 3; (default) 4; (empowered GRift or no-death bonus) 5; (empowered GRift + no-death bonus)
                plugin.InventoryNumbers = true;			// show GRift level advised for the gem in inventory, stash, set to true; or false;
                plugin.HoveredNumbers = true;			// show upgrade hint on item hovered, set to true; or false;
            });

/*
			// Shows 1 circle around your player when you have Zei's stone of vengeance equipped and are fighting
			// it also displays the percentage of damage increased by the legendary gem under each monster
			// (calculated from gem level and distance from your player).
            Hud.RunOnPlugin<Resu.HuntersVengeancePlugin>(plugin => 
            { 
				plugin.Enabled = false;
                plugin.permanentCircle = false;      // Enable permanent circle : Set it to true;
                plugin.ElitesOnlyNumbers = false;    // Enable numbers on elites only : Set it to true;
                plugin.TargetForAll = true;          // Disable cursor on minimap for all : Set it to false;
            });  
*/

/*
			// Shows stacks & the percentage of damage increased by the legendary gem under
			// each monster when you have Bane of the Stricken equipped.
			// There's also a cooldown indicator.
            Hud.RunOnPlugin<Resu.DiadrasFirstGemPlugin>(plugin => 
            {
				plugin.Enabled = false;
                plugin.ElitesnBossOnly = false;
                plugin.BossOnly = false;
                plugin.offsetX = 0;
                plugin.offsetY = 0;
            });
*/

        } // public void Customize()

    } // public class PluginEnablerOrDisablerPlugin : BasePlugin, ICustomizer

} // namespace Turbo.Plugins.User