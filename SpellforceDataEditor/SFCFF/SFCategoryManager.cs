//todo: modernize loading/saving data (chunk-based instead of sequential)

using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFCFF
{
    //helper class which provides nice interface for retrieving spell parameter description
    public class SFSpellDescriptor
    {
        static public string[] get(UInt16 spell_id)
        {
            string[] p = { "", "", "", "", "", "", "", "", "", "", "0000000000" };  //p[10] is for data tracing
            switch(spell_id)
            {
                case 1:   //fireburst
                case 147: //fireball
                case 159: //fireburst (firestarter)
                case 234: //fireburst (chain effect)
                case 239: //fireball (chain effect)
                    p[0] = "Initial damage";
                    p[1] = "Damage per tick";
                    p[2] = "Tick count";
                    p[3] = "Time between ticks (ms)";
                    break;
                case 2:   //healing
                case 45:  //greater healing
                case 144: //healing (white tower)
                case 166: //aura of healing (aura effect)
                    p[0] = "Heal amount";
                    break;
                case 3:   //death
                case 18:  //pain
                case 19:  //lifetap
                case 60:  //fireshield (effect)
                case 61:  //thorn shield (effect)
                case 139: //rock bullet
                case 143: //arrow tower
                case 146: //aura of lifetap (aura effect)
                case 160: //spark (veteran upgrade)
                case 162: //pain (tower of sorcery)
                case 163: //stone tower
                case 170: //lavabullet (thrower upgrade)
                case 231: //lifetap (chain effect)
                case 236: //rock bullet (chain effect)
                case 240: //pain (chain effect)
                    p[0] = "Damage";
                    break;
                case 4:   //slowness
                case 34:  //inflexibility
                case 35:  //weaken
                case 36:  //dark banishing
                case 48:  //quickness
                case 50:  //flexibility
                case 52:  //strength
                case 65:  //brilliance
                case 99:  //suffocation
                case 100: //inability
                case 101: //slow fighting
                case 116: //dexterity
                case 117: //endurance
                case 118: //fast fighting
                case 125: //charisma
                case 130: //enlightenment
                case 135: //melt resistance
                case 138: //chill resistance
                    p[0] = "Effect strength (%)";
                    p[1] = "Duration (ms)";
                    break;
                case 5:   //poison
                case 58:  //hallow
                case 230: //hallow (chain effect)
                    p[0] = "Damage per tick";
                    p[1] = "Tick duration (ms)";
                    p[2] = "Tick count";
                    break;
                case 6:   //invulnerability
                case 9:   //freeze
                case 41:  //reemediless
                case 157: //steelskin
                    p[0] = "Duration (ms)";
                    break;
                case 7:   //cure poison
                case 24:  //cure disease
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = Utility.S_UNKNOWN;
                    break;
                case 10:  //fog
                    p[0] = "Maximum sight range";
                    p[1] = "Time between ticks (ms)";
                    p[2] = "Tick count";
                    p[3] = Utility.S_UNKNOWN;
                    break;
                case 11:  //illuminate
                    p[0] = "Tick count";
                    p[1] = "Time between ticks (ms)";
                    p[2] = "Minimum sight range";
                    break;
                case 12:  //fireshield (cast)
                case 15:  //iceshield (cast)
                case 47:  //thornshield
                    p[0] = "Duration (ms)";
                    p[1] = "Sub-effect ID";
                    p[10] = "0100000000";
                    break;
                case 13:  //fireball (cast)
                    p[0] = "Initial damage";
                    p[1] = "Damage per tick";
                    p[2] = "Tick count";
                    p[3] = "Tick duration (ms)";
                    p[4] = "Sub-effect ID";
                    p[10] = "0000100000";
                    break;
                case 14:  //icestrike
                case 145: //icestrike (wave)
                case 235: //icestrike (chain effect)
                    p[0] = "Initial damage tick";
                    p[1] = "Time between ticks";
                    p[2] = "Freeze duration";
                    break;
                case 16:  //decay
                    p[0] = "Time between ticks (ms)";
                    p[1] = "Tick count";
                    p[2] = "Armor reduction (%)";
                    break;
                case 20:  //summon undead goblin
                case 29:  //summon skeleton
                case 92:  //summon spectre
                case 106: //summon wolf
                case 109: //summon bear
                case 133: //fire elemental
                case 136: //ice elemental
                case 141: //earth elemental
                case 188: //treewraith
                case 198: //summon blade
                case 203: //summon fire golem
                case 206: //summon ice golem
                case 209: //summon stonee golem
                    p[0] = "Time between ticks (ms)";
                    p[1] = "Mana per tick";
                    p[2] = "Unit ID";
                    p[10] = "0020000000";
                    break;
                case 21:  //hypnotize
                case 161: //hypnotize (aura effect)
                case 167: //hypnotize (mindbreaker)
                    p[0] = "Time between ticks (ms)";
                    p[1] = "Tick count";
                    p[2] = "Max level affected";
                    break;
                case 22:  //iceshield (effect)
                    p[0] = "Freeze duration";
                    break;
                case 23:  //pestilence
                    p[0] = "Initial damage";
                    p[1] = "Time between ticks (ms)";
                    p[2] = "Damage increase";
                    p[3] = "Ticks between dmg increase";
                    p[4] = "Max level affected";
                    break;
                case 25:  //petrify
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = Utility.S_UNKNOWN;
                    p[3] = "Time between ticks (ms)";
                    p[4] = "Tick count";
                    break;
                case 28:  //area pain
                    p[0] = "Damage";
                    p[1] = "Area radius";
                    break;
                case 30:  //raise dead
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = "Area radius";
                    p[2] = Utility.S_UNKNOWN;
                    break;
                case 32:  //death grasp
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = "Time between ticks (ms)";
                    p[2] = Utility.S_UNKNOWN;
                    break;
                case 37:  //area slowness
                case 38:  //area inflexibility
                case 40:  //area plague
                case 49:  //area quickness
                case 51:  //area flexibility
                case 53:  //area strength
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = "Duration (ms)";
                    break;
                case 43:  //area healing
                    p[0] = "Area radius (?)";
                    p[1] = "Heal amount";
                    break;
                case 44:  //sentinel healing
                    p[0] = "Area radius";
                    p[1] = "1st unit heal";
                    p[2] = "2nd unit heal";
                    p[3] = "3rd unit heal";
                    p[4] = "4th unit heal";
                    break;
                case 46:  //charm animal
                case 122: //charm
                case 237: //charm (chain effect)
                    p[0] = "Time between ticks (ms)";
                    p[1] = "1st tick check";
                    p[2] = "2nd tick check";
                    p[3] = "3rd tick check";
                    p[4] = "Tick count";
                    p[5] = Utility.S_UNKNOWN;
                    p[6] = "Max level affected";
                    break;
                case 54:  //guard (?)
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = "Duration";
                    break;
                case 55:  //remove curse (?)
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = Utility.S_UNKNOWN;
                    break;
                case 56:  //regenerate
                    p[0] = "Heal amount";
                    p[1] = "Time between ticks (ms)";
                    p[2] = "Tick count";
                    break;
                case 57: //holy might
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = Utility.S_UNKNOWN;
                    p[3] = Utility.S_UNKNOWN;
                    p[4] = "Time between ticks (ms)";
                    p[5] = "Duration (ms)";
                    break;
                case 62:  //forget
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = Utility.S_UNKNOWN;
                    p[3] = Utility.S_UNKNOWN;
                    break;
                case 63:  //self illusion
                    p[0] = "Illusions summoned";
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = Utility.S_UNKNOWN;
                    p[3] = Utility.S_UNKNOWN;
                    break;
                case 64:  //retention
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = "Duration (ms)";
                    break;
                case 66:  //sacrifice mana
                case 67:  //manatap
                case 68:  //manadrain
                case 172: //manatap (aura effect)
                case 232: //manatap (?)
                    p[0] = "Mana amount";
                    break;
                case 69:  //shock
                case 238: //shock (chain efect)
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = "Damage";
                    p[2] = Utility.S_UNKNOWN;
                    p[3] = Utility.S_UNKNOWN;
                    break;
                case 70:  //disrupt
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = Utility.S_UNKNOWN;
                    p[3] = Utility.S_UNKNOWN;
                    break;
                case 71:  //fear
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = Utility.S_UNKNOWN;
                    p[3] = "Duration (ms)";
                    break;
                case 72:  //confuse
                case 79:  //amok
                case 128: //demoralization
                    p[0] = "Duration (ms)";
                    p[1] = "Max level affected";
                    break;
                case 73:  //rain of fire
                case 74:  //blizzard
                case 76:  //stone rain
                    p[0] = "Time between ticks";
                    p[1] = "Tick count";
                    p[2] = Utility.S_UNKNOWN;
                    p[3] = Utility.S_UNKNOWN;
                    p[4] = "Sub-effect ID";
                    p[10] = "0000100000";
                    break;
                case 75:  //acid cloud
                    p[0] = "Area radius";
                    p[1] = "Time between ticks (ms)";
                    p[2] = "Tick count";
                    p[3] = "Damage per tick";
                    break;
                case 77:  //wall of rocks
                case 78:  //ring of rocks
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = "Duration (ms))";
                    break;
                case 80:  //extinct
                    p[0] = "Health threshold";
                    p[1] = Utility.S_UNKNOWN;
                    break;
                case 82:  //detect metal
                case 83:  //detect magic
                    p[0] = "Radius";
                    break;
                case 86:  //invisibility
                    p[0] = "Time between ticks (ms)";
                    p[1] = "Tick count";
                    p[2] = Utility.S_UNKNOWN;
                    p[3] = Utility.S_UNKNOWN;
                    p[4] = Utility.S_UNKNOWN;
                    p[5] = Utility.S_UNKNOWN;
                    break;
                case 88:  //aura of weakness
                case 89:  //aura of suffocation
                case 91:  //aura of lifetap
                case 94:  //aura of slow fighting
                case 95:  //aura of inflexibility
                case 97:  //aura of slow walking
                case 98:  //aura of inability
                case 102: //aura of strength
                case 103: //aura of healing
                case 104: //aura of endurance
                case 107: //aura of regeneration
                case 110: //aura of fast fighting
                case 111: //aura of flexibility
                case 113: //aura of fast walking
                case 114: //aura of light
                case 115: //aura of dexterity
                case 127: //aura of hypnotization
                case 129: //aura of brilliance
                case 131: //aura of manatap
                case 192: //aura of eternity
                case 223: //aura siege human
                case 225: //aura siege elf
                case 226: //aura siege orc
                case 227: //aura siege troll
                case 228: //aura siege darkelf
                    p[0] = "Time between ticks (ms)";
                    p[1] = "Unused";
                    p[2] = "Aura range";
                    p[3] = "Unused";
                    p[4] = "Unused";
                    p[5] = "Aura target type";
                    p[6] = "Sub-effect ID";
                    p[7] = "Unused";
                    p[8] = "Mana per tick";
                    p[10] = "0000001000";
                    break;
                case 90:  //suicide death
                    p[0] = "Damage inflicted";
                    p[1] = "Damage received";
                    break;
                case 93:  //feign death
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = Utility.S_UNKNOWN;
                    p[3] = Utility.S_UNKNOWN;
                    p[4] = Utility.S_UNKNOWN;
                    break;
                case 96:  //dispel white aura
                case 112: //dispel black aura
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = Utility.S_UNKNOWN;
                    p[3] = "Dispel duration (ms)";
                    break;
                case 105: //suicide heal
                    p[0] = "Heal amount";
                    p[1] = "Damage received";
                    break;
                case 108: //dominate animal
                case 120: //dominate
                case 197: //dominate undead
                    p[0] = "Time between ticks (ms)";
                    p[1] = "Mana per tick";
                    p[2] = "Max level affected";
                    break;
                case 119: //distract
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = Utility.S_UNKNOWN;
                    break;
                case 123: //befriend
                case 124: //disenchant
                    p[0] = "Max level affected";
                    break;
                case 126: //shock
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = "Damage";
                    p[2] = Utility.S_UNKNOWN;
                    p[3] = Utility.S_UNKNOWN;
                    p[4] = "Area radius";
                    break;
                case 134: //wave of fire
                case 137: //wave of ice
                case 142: //wave of rocks
                    p[0] = "Min projectile amount";
                    p[1] = "Bonus projectile chance";
                    p[2] = "2nd bonus projectile chance";
                    p[3] = "Sub-effect ID";
                    p[4] = "Wave range";
                    p[10] = "0001000000";
                    break;
                case 140: //conservation
                    p[0] = "Damage shielded";
                    p[1] = "Duration (ms)";
                    break;
                case 148: //war cry
                    p[0] = "Duration (ms)";
                    p[1] = "Damage multiplier (%)";
                    p[2] = "Area radius";
                    p[3] = "Units affected";
                    break;
                case 149: //benefactions
                    p[0] = "Health restored (%)";
                    p[1] = "Area radius";
                    p[2] = "Units affected";
                    break;
                case 150: //patronize
                    p[0] = "Duration (ms)";
                    p[1] = "Resistance multiplier (%)";
                    p[2] = "Area radius";
                    p[3] = "Units affected";
                    break;
                case 151: //endurance
                    p[0] = "Duration (ms)";
                    p[1] = "Damage reduced to (%)";
                    p[2] = "Area radius";
                    p[3] = "Units affected";
                    break;
                case 152: //berserk
                case 156: //trueshot
                    p[0] = "Duration";
                    p[1] = "Damage multiplier (%)";
                    break;
                case 153: //boons
                    p[0] = "Health restored (%)";
                    break;
                case 154: //shelter
                    p[0] = "Resistance multiplier (%)";
                    break;
                case 155: //durability
                    p[0] = "Damage reduced to (%)";
                    break;
                case 158: //salvo
                    p[0] = "Duration";
                    p[1] = "Max targets";
                    p[2] = "Cone radius (degrees)";
                    break;
                case 164: //cloak of nor
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = "Duration (ms)";
                    break;
                case 165: //unknown?
                    p[0] = Utility.S_UNKNOWN;
                    break;
                case 168: //freeze (wind archer upgrade)
                case 169: //freeze (hurler upgrade)
                    p[0] = "Freeze duration (ms)";
                    p[1] = "Initial tick damage";
                    break;
                case 171: //extinct (blact tower)
                    p[0] = "Health threshold";
                    p[1] = "Area radius";
                    break;
                case 173: //firebane
                    p[0] = "Fire resistance granted";
                    p[1] = "Duration (ms)";
                    break;
                case 174: //black essence
                case 178: //black almightness
                    p[0] = "Damage";
                    p[1] = "Lifesteal percentage (?)";
                    p[2] = "Agility loss (%)";
                    p[3] = "Fight speed reduction (%)";
                    p[4] = "Duration";
                    break;
                case 175: //white essence
                case 179: //white almightness
                    p[0] = "Heal amount";
                    p[1] = "Sub-effect ID";
                    p[2] = "Dexterity gain (%)";
                    p[3] = "Fight speed increase (%)";
                    p[4] = "Duration";
                    p[10] = "0100000000";
                    break;
                case 176: //elemental essence
                case 180: //elemental almightness
                    p[0] = "Effect ID";
                    p[10] = "1000000000";
                    break;
                case 177: //mental essence
                case 181: //mental almightness
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = Utility.S_UNKNOWN;
                    p[3] = Utility.S_UNKNOWN;
                    p[4] = Utility.S_UNKNOWN;
                    p[5] = Utility.S_UNKNOWN;
                    break;
                case 182: //elemental almightness
                case 183: //elemental essence
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = Utility.S_UNKNOWN;
                    p[3] = Utility.S_UNKNOWN;
                    p[4] = Utility.S_UNKNOWN;
                    p[5] = Utility.S_UNKNOWN;
                    p[6] = Utility.S_UNKNOWN;
                    break;
                case 184: //assistance
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = "Duration (ms)";
                    break;
                case 186: //revenge
                case 195: //torture
                    p[0] = "Health per corpse";
                    p[1] = "Mana per corpse";
                    p[2] = "Area radius";
                    break;
                case 187: //area roots
                case 189: //roots
                    p[0] = "Time between ticks (ms)";
                    p[1] = "Damage per tick";
                    p[2] = Utility.S_UNKNOWN;
                    p[3] = Utility.S_UNKNOWN;
                    p[4] = "Max level affected";
                    p[5] = Utility.S_UNKNOWN;
                    break;
                case 190: //chain hallow
                case 193: //chain pain
                case 196: //chain lifetap
                case 201: //chain mutation
                case 202: //chain fireburst
                case 204: //chain fireball
                case 205: //chain iceburst
                case 208: //chain rock bullet
                case 212: //chain charm
                case 214: //chain shock
                case 217: //chain manatap
                    p[0] = "Time between bounces (ms)";
                    p[1] = "Bounce radius";
                    p[2] = "Target type";
                    p[3] = "Sub-effect ID";
                    p[4] = "Max bounces";
                    p[10] = "0001000000";
                    break;
                case 191: //reinforcement
                    p[0] = "Area radius";
                    break;
                case 194: //cannibalize
                    p[0] = "Damage received";
                    p[1] = "Mana regained";
                    break;
                case 199: //mutation
                case 233: //mutation (chain effect)
                    p[0] = "Duration (ms)";
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = Utility.S_UNKNOWN;
                    break;
                case 200: //area of darkness
                    p[0] = "Duration (ms)";
                    p[1] = "Effect strength (%)";
                    p[1] = "Unknown (radius?)";
                    break;
                case 207: //area freeze
                    p[0] = "Freeze duration (ms)";
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = Utility.S_UNKNOWN;
                    break;
                case 210: //feet of clay
                    p[0] = "Time between ticks (ms)";
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = Utility.S_UNKNOWN;
                    p[3] = Utility.S_UNKNOWN;
                    p[4] = Utility.S_UNKNOWN;
                    break;
                case 211: //mirage
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = Utility.S_UNKNOWN;
                    break;
                case 213: //feedback
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = Utility.S_UNKNOWN;
                    break;
                case 215: //area hypnotize
                    p[0] = "Time between ticks (ms)";
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = "Max level affected";
                    p[3] = Utility.S_UNKNOWN;
                    p[4] = Utility.S_UNKNOWN;
                    break;
                case 216: //area confuse
                    p[0] = "Duration";
                    p[1] = "Max level affected";
                    p[2] = Utility.S_UNKNOWN;
                    p[3] = Utility.S_UNKNOWN;
                    break;
                case 218: //manashield
                    p[0] = Utility.S_UNKNOWN;
                    break;
                case 219: //manashift
                    p[0] = "Area radius";
                    p[1] = "Mana per unit";
                    break;
                case 220: //shift life
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = Utility.S_UNKNOWN;
                    break;
                case 221: //riposte
                    p[0] = Utility.S_UNKNOWN;
                    p[1] = "Duration (ms)";
                    break;
                case 222: //critical hits
                    p[0] = "Duration (ms)";
                    p[1] = Utility.S_UNKNOWN;
                    p[2] = Utility.S_UNKNOWN;
                    p[3] = Utility.S_UNKNOWN;
                    p[4] = Utility.S_UNKNOWN;
                    p[5] = Utility.S_UNKNOWN;
                    p[6] = Utility.S_UNKNOWN;
                    p[7] = Utility.S_UNKNOWN;
                    p[8] = Utility.S_UNKNOWN;
                    break;
                case 229: //aura of eternity (aura effect)
                    p[0] = "Agility bonus (%)";
                    p[1] = "Dexterity bonus (%)";
                    p[2] = "Walk speed bonus (%)";
                    p[3] = "Fight speed bonus (%)";
                    p[4] = "Duration (ms)";
                    break;
            }
            return p;
        }
    }

    //this class is responsible for category management
    //it provides with general functions to perform on categories as a database
    public static class SFCategoryManager
    {
        /*private static SFCategory[] categories;      //array of categories*/
        private static SFGameData gamedata = new SFGameData() ;
        private static SFCategoryRuneHeroes categorySpecial_RuneHeroes;    //intermediary needed to find names of rune heroes
        /*private static int categoryNumber;           //amount of categories (basically a constant)

        private static Byte[] mainHeader;            //gamedata.cff has a main header which is held here
        private static string gamedata_md5 = "";*/     //currently loaded cff's MD5 hash (as string)

        public static bool ready { get; private set; } = false;

        //constructor, it creates categories
        public static void init()
        {
            categorySpecial_RuneHeroes = new SFCategoryRuneHeroes();
            //categorySpecial_RuneHeroes.set_manager(this);
        }

        //returns category, given its index
        public static SFCategory get_category(int index)
        {
            if((index > -1)&&(index < SFGameData.categoryNumber))
                return gamedata.categories[index];
            return null;
        }

        //returns string representation of file's MD5
        public static string get_data_md5()
        {
            return gamedata.gamedata_md5;
        }

        //loads gamedata.cff file
        public static int load_cff(string filename)
        {
            int result = gamedata.Load(filename);

            if (result == 0)
            {
                categorySpecial_RuneHeroes.generate();
                ready = true;
            }

            return result;
        }

        //saves gamedata.cff file
        public static void save_cff(string filename)
        {
            gamedata.Save(filename);

            //md5 calculation for data diff tool
            //FileStream fs2 = new FileStream(filename, FileMode.Open, FileAccess.Read);

            //MD5 md5_gen = MD5.Create();
            //gamedata_md5 = BitConverter.ToString(md5_gen.ComputeHash(fs2)).Replace("-", "").ToLower();

            //fs2.Close();
        }

        //returns category count
        public static int get_category_number()
        {
            return SFGameData.categoryNumber;
        }

        //using the fact that all text data is saved in category 15 (ind 14)
        //searches for a text with a given ID and in a given language
        //returns a sub-element in a given language which contains text data looked for (or null if it doesnt exist)
        //this is quite fast (O(log n))
        public static SFCategoryElement find_element_text(int t_index, int t_lang)
        {
            int index = gamedata.categories[14].find_binary_element_index<UInt16>(0, (UInt16)t_index);
            if (index == -1)
                return null;

            int lang_index = -1;
            int safe_index = -1;   //will fail if there's no language id 0

            SFCategoryElement e = new SFCategoryElement();
            SFCategoryElement e_found = gamedata.categories[14].get_element(index);
            int elem_num = e_found.get().Count / 5;

            for(int i = 0; i < elem_num; i++)
            {
                if((Byte)e_found.get_single_variant(i*5+1).value == (Byte)t_lang)
                {
                    lang_index = i;
                    break;
                }
                else if((Byte)e_found.get_single_variant(i*5+1).value == 0)
                {
                    safe_index = i;
                }
            }

            if (lang_index == -1)
                lang_index = safe_index;
            if (lang_index == -1)
                return null;

            Object[] vs = new Object[5];
            for (int i = 0; i < 5; i++)
                vs[i] = e_found.get_single_variant(lang_index * 5 + i).value;
            e.set(vs);

            return e;
        }

        //returns a name of a given effect
        //optionally with effect level
        public static string get_effect_name(UInt16 effect_id, bool effect_level = false)
        {
            SFCategoryElement effect_elem = get_category(0).find_binary_element<UInt16>(0, effect_id);
            if (effect_elem == null)
                return Utility.S_NONAME;

            UInt16 spell_type = (UInt16)effect_elem.get_single_variant(1).value;
            SFCategoryElement spell_elem = get_category(1).find_binary_element<UInt16>(0, spell_type);
            if (spell_elem == null)
                return Utility.S_NONAME;

            int text_id = (int)(UInt16)spell_elem.get_single_variant(1).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            if (text_elem == null)
                return Utility.S_MISSING;

            string txt = Utility.CleanString(text_elem.get_single_variant(4));
            if (effect_level)
                txt += " level " + effect_elem.get_single_variant(4).value.ToString();

            return txt;
        }

        //returns a name of a given unit
        public static string get_unit_name(UInt16 unit_id)
        {
            SFCategoryElement unit_elem = get_category(17).find_binary_element<UInt16>(0, unit_id);
            if (unit_elem == null)
                return Utility.S_NONAME; ;

            int text_id = (int)(UInt16)unit_elem.get_single_variant(1).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            if (text_elem == null)
                return Utility.S_MISSING; ;

            string txt = Utility.CleanString(text_elem.get_single_variant(4));

            return txt;
        }

        //used for determining skill name
        private static string get_resource_gather(Byte ind)
        {
            switch (ind)
            {
                case 1:
                    return "Wood";
                case 3:
                    return "Stone";
                case 8:
                    return "Iron";
                case 9:
                    return "Human unique";
                case 10:
                    return "Elf unique";
                case 11:
                    return "Dwarf unique";
                case 12:
                    return "Orc unique";
                case 13:
                    return "Troll unique";
                case 14:
                    return "Dark elf unique";
                default:
                    return Utility.S_NONAME;
            }
        }

        //returns a name of a given skill
        public static string get_skill_name(Byte skill_major, Byte skill_minor, Byte skill_lvl)
        {
            string txt_major = "";
            string txt_minor = "";
            SFCategoryElement skill_elem;
            try
            {
                skill_elem = get_category(26).get_element(skill_major);
            }
            catch(Exception)
            {
                return Utility.S_UNKNOWN;
            }

            int text_id_major = (int)(UInt16)skill_elem.get_single_variant(2).value;
            SFCategoryElement txt_elem_major = find_element_text(text_id_major, 1);
            if (txt_elem_major != null)
                txt_major = Utility.CleanString(txt_elem_major.get_single_variant(4));

            if ((skill_major == 0) && (skill_minor != 0))
            {
                txt_major = "";
                txt_minor = get_resource_gather(skill_minor) + " gathering";
            }
            else if (skill_minor != 101)
            {
                int text_id_minor = (int)(UInt16)skill_elem.get_single_variant(skill_minor*3+2).value;
                if (text_id_minor != 0)
                {
                    SFCategoryElement txt_elem_minor = find_element_text(text_id_minor, 1);
                    if (txt_elem_minor != null)
                        txt_minor = Utility.CleanString(txt_elem_minor.get_single_variant(4));
                }
            }

            return txt_major + " " + txt_minor + " " + skill_lvl.ToString();
        }

        //returns a name of a given race
        public static string get_race_name(Byte race_id)
        {
            SFCategoryElement race_elem = get_category(15).find_binary_element<Byte>(0, race_id);
            if (race_elem == null)
                return Utility.S_NONAME;

            int text_id = (int)(UInt16)race_elem.get_single_variant(7).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            if (text_elem == null)
                return Utility.S_MISSING;

            string txt = Utility.CleanString(text_elem.get_single_variant(4));

            return txt;
        }

        //returns a name of a given item
        public static string get_item_name(UInt16 item_id)
        {
            SFCategoryElement item_elem = get_category(6).find_binary_element<UInt16>(0, item_id);
            if (item_elem == null)
                return Utility.S_NONAME;

            int text_id = (int)(UInt16)item_elem.get_single_variant(3).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            if (text_elem == null)
                return Utility.S_MISSING;

            string txt = Utility.CleanString(text_elem.get_single_variant(4));

            return txt;
        }

        //returns a name of a given building
        public static string get_building_name(UInt16 building_id)
        {
            SFCategoryElement building_elem = get_category(23).find_binary_element<UInt16>(0, building_id);
            if (building_elem == null)
                return Utility.S_NONAME;

            int text_id = (int)(UInt16)building_elem.get_single_variant(5).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            if (text_elem == null)
                return Utility.S_MISSING;

            string txt = Utility.CleanString(text_elem.get_single_variant(4));

            return txt;
        }

        //returns a name of a given merchant
        public static string get_merchant_name(UInt16 merchant_id)
        {
            SFCategoryElement merchant_elem = get_category(28).find_binary_element<UInt16>(0, merchant_id);
            if (merchant_elem == null)
                return Utility.S_NONAME;

            return get_unit_name((UInt16)merchant_elem.get_single_variant(1).value);
        }

        //returns a name of a given object
        public static string get_object_name(UInt16 object_id)
        {
            SFCategoryElement object_elem = get_category(33).find_binary_element<UInt16>(0, object_id);
            if (object_elem == null)
                return Utility.S_NONAME;

            int text_id = (int)(UInt16)object_elem.get_single_variant(1).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            if (text_elem == null)
                return Utility.S_MISSING;

            string txt = Utility.CleanString(text_elem.get_single_variant(4));

            return txt;
        }

        //returns a description given its id
        public static string get_description_name(UInt16 desc_id)
        {
            SFCategoryElement desc_elem = get_category(40).find_binary_element<UInt16>(0, desc_id);
            if (desc_elem == null)
                return Utility.S_NONAME;

            int text_id = (int)(UInt16)desc_elem.get_single_variant(1).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            if (text_elem == null)
                return Utility.S_MISSING;

            string txt = Utility.CleanString(text_elem.get_single_variant(4));

            return txt;
        }

        //returns a rune hero/worker's name given its stats ID
        //this connection is not found in gamedata.cff and instead has to be pre-processed
        public static string get_runehero_name(UInt16 stats_id)
        {
            SFCategoryElement rune_elem = categorySpecial_RuneHeroes.find_binary_element<UInt16>(0, stats_id);
            if (rune_elem == null)
                return Utility.S_NONAME;

            int text_id = (int)(UInt16)rune_elem.get_single_variant(1).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            if (text_elem == null)
                return Utility.S_MISSING;

            return Utility.CleanString(text_elem.get_single_variant(4));
        }

        //returns data value given its address
        public static object get_variant(int cat, int elem, int ind)
        {
            return get_category(cat).get_element_variant(elem, ind).value;
        }

        //frees all data, only empty categories remain
        public static void unload_all()
        {
            gamedata.Unload();
            categorySpecial_RuneHeroes.unload();

            ready = false;
        }
    }
}
