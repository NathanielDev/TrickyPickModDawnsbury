//using Dawnsbury.Core;
//using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.Weapons.TrickyPick
{
    public class TrickyPickMod
    {
        //Might want to have this in another file.
        public static readonly Trait ModularBPS = ModManager.RegisterTrait("Modular (B, P or S)", new TraitProperties("Modular (B, P or S)", true, "Implementation works the same as Versatile B, P and S.", relevantForShortBlock: true));
        
        [DawnsburyDaysModMainMethod]
        public static void LoadMod()
        {


            ModManager.RegisterNewItemIntoTheShop("TrickyPick", ItemName =>
            new Item(ItemName, new ModdedIllustration("TPAssets/TrickyPick.png"), "tricky pick", 0, 10, Trait.Uncommon, Trait.Backstabber, Trait.FatalD10, Trait.Kobold, ModularBPS, Trait.Advanced)
            .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Piercing))
            .WithDescription("This ingenious kobold pick conceals several hidden traps that the wielder can activate to trick and befuddle foes with a variety of damaging blades and bludgeoning surfaces."));
            // Modular Trait Implementation Idea from SudoDawnsburyMods (https://github.com/SudoProgramming/SudoDawnsburyMods)
            ModManager.RegisterActionOnEachCreature(creature =>
            {
            creature.AddQEffect(new QEffect
            {
                StateCheck = (QEffect self) =>
                {
                    foreach (Item item in self.Owner.HeldItems)
                    {
                        
                        if (item.HasTrait(ModularBPS) && !item.HasTrait(Trait.VersatileB) && !item.HasTrait(Trait.VersatileS))
                        {
                            item.Traits.Add(Trait.VersatileB);
                            item.Traits.Add(Trait.VersatileS);
                        }
                        ;
                    }
                    ;
                }
            });
            });
        }
    }
}
