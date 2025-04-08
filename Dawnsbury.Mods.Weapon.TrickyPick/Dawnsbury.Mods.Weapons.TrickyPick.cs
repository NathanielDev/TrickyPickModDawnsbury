using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Creatures;

namespace Dawnsbury.Mods.Weapons.TrickyPick
{
    public class TrickyPickMod
    {
        //Might want to have these to another file.
        public static readonly Trait ModularBPS = ModManager.RegisterTrait("Modular (B, P or S)", new TraitProperties("Modular (B, P or S)", true, "Implementation works the same as Versatile B, P and S.", relevantForShortBlock: true));

        //this will generate a bug if the player is dual wielding tricky picks and can still interact with the weapon somehow
        public static void SwitchWeaponDmgType (Creature owner, DamageKind newDamageType)
        {
            foreach (Item item in owner.HeldItems)
            {
                if (item.HasTrait(ModularBPS))
                {
                    List<(string,DamageKind)> prevAddDamage = item.WeaponProperties.AdditionalDamage;
                    item.WeaponProperties = new WeaponProperties(item.WeaponProperties.Damage, newDamageType);
                    
                    foreach ((string,DamageKind) addDamage in prevAddDamage)
                    {
                        item.WeaponProperties = item.WeaponProperties.WithAdditionalDamage(addDamage.Item1, addDamage.Item2);
                    }

                }
            }
        }
        
        
        [DawnsburyDaysModMainMethod]
        public static void LoadMod()
        {

            ModManager.RegisterNewItemIntoTheShop("TrickyPick", ItemName =>
            {

                Item TrickyPickItem = new Item(ItemName, new ModdedIllustration("TPAssets/TrickyPick.png"), "tricky pick", 0, 10, Trait.Uncommon, Trait.Backstabber, Trait.FatalD10, Trait.Kobold, ModularBPS, Trait.Advanced, Trait.Pick);
                TrickyPickItem.StateCheckWhenWielded = (wielder, TrickyPick) =>
                {
                    wielder.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                    {
                        ProvideActionIntoPossibilitySection = delegate (QEffect effect, PossibilitySection section)
                        {
                            if (section.PossibilitySectionId == PossibilitySectionId.ItemActions)
                            {
                                // TODO: clean this up, ModdedIllustration definition can be in a separate file along with other commonly used stuff
                                PossibilitySection damageTypesMenu = new PossibilitySection("Damage Type Switch Menu");
                                SubmenuPossibility InteractMenu = new SubmenuPossibility(new ModdedIllustration("TPAssets/TrickyPick.png"), "Adjust Tricky Pick", PossibilitySize.Full);
                                ActionPossibility toBludgeoning = new ActionPossibility(new CombatAction(effect.Owner, new ModdedIllustration("TPAssets/Bludgeoning.png"), "Switch To Bludgeoning Damage", new Trait[1] { Trait.Manipulate }, "Interact with your weapon to switch its damage type to Bludgeoning", Target.Self()).WithActionCost(1).WithEffectOnSelf(self => SwitchWeaponDmgType(self, DamageKind.Bludgeoning)));
                                ActionPossibility toPiercing = new ActionPossibility(new CombatAction(effect.Owner, new ModdedIllustration("TPAssets/Piercing.png"), "Switch To Piercing Damage", new Trait[1] { Trait.Manipulate }, "Interact with your weapon to switch its damage type to Piercing", Target.Self()).WithActionCost(1).WithEffectOnSelf(self => SwitchWeaponDmgType(self, DamageKind.Piercing)));
                                ActionPossibility toSlashing= new ActionPossibility(new CombatAction(effect.Owner, new ModdedIllustration("TPAssets/Slashing.png"), "Switch To Slashing Damage", new Trait[1] { Trait.Manipulate }, "Interact with your weapon to switch its damage type to Slashing", Target.Self()).WithActionCost(1).WithEffectOnSelf(self => SwitchWeaponDmgType(self, DamageKind.Slashing)));
                                switch (TrickyPick.WeaponProperties.DamageKind)
                                {
                                    case DamageKind.Bludgeoning:
                                        damageTypesMenu.AddPossibility(toPiercing);
                                        damageTypesMenu.AddPossibility(toSlashing);
                                        break;
                                    case DamageKind.Piercing:
                                        damageTypesMenu.AddPossibility(toBludgeoning);
                                        damageTypesMenu.AddPossibility(toSlashing);
                                        break;
                                    case DamageKind.Slashing:
                                        damageTypesMenu.AddPossibility(toBludgeoning);
                                        damageTypesMenu.AddPossibility(toPiercing);
                                        break;
                                }
                                
                                
                                InteractMenu.Subsections.Add(damageTypesMenu);
                                
                                
                                return InteractMenu;
                            }
                            return null;



                        }
                    });
                };
                return TrickyPickItem
            .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning))
            .WithDescription("This ingenious kobold pick conceals several hidden traps that the wielder can activate to trick and befuddle foes with a variety of damaging blades and bludgeoning surfaces.");              

            });
        }
    } 
}

