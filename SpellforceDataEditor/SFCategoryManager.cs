﻿using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor
{
    public class SFSpellDescriptor
    {
        static public string[] get(UInt16 spell_id)
        {
            string[] p = { "", "", "", "", "", "", "", "", "", "" };
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
                    p[3] = "Tick duration (ms)";
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
                    p[0] = "Unknown";
                    p[1] = "Unknown";
                    p[2] = "Unknown";
                    break;
                case 10:  //fog
                    p[0] = "Unknown";
                    p[1] = "Duration (ms)";
                    p[2] = "Unknown";
                    p[3] = "Unknown";
                    break;
                case 11:  //illuminate
                    p[0] = "Unknown";
                    p[1] = "Duration (ms)";
                    p[2] = "Unknown";
                    break;
                case 12:  //fireshield (cast)
                case 15:  //iceshield (cast)
                case 47:  //thornshield
                    p[0] = "Duration (ms)";
                    p[1] = "Sub-effect ID";
                    break;
                case 13:  //fireball (cast)
                    p[0] = "Initial damage";
                    p[1] = "Damage per tick";
                    p[2] = "Tick count";
                    p[3] = "Tick duration (ms)";
                    p[4] = "Sub-effect ID";
                    break;
                case 14:  //icestrike
                case 145: //icestrike (wave)
                case 235: //icestrike (chain effect)
                    p[0] = "Initial damage tick";
                    p[1] = "Time between ticks";
                    p[2] = "Freeze duration";
                    break;
                case 16:  //decay
                    p[0] = "Time between ticks (ms) (?)";
                    p[1] = "Tick count (?)";
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
                    p[0] = "Unknown";
                    p[1] = "Unknown";
                    p[2] = "Unknown";
                    p[3] = "Time between ticks (ms) (?)";
                    p[4] = "Tick count (?)";
                    break;
                case 28:  //area pain
                    p[0] = "Damage";
                    p[1] = "Area radius";
                    break;
                case 30:  //raise dead
                    p[0] = "Unknown";
                    p[1] = "Area radius";
                    p[2] = "Unknown";
                    break;
                case 32:  //death grasp
                    p[0] = "Unknown";
                    p[1] = "Time between ticks (ms) (?)";
                    p[2] = "Unknown";
                    break;
                case 37:  //area slowness
                case 38:  //area inflexibility
                case 40:  //area plague
                case 49:  //area quickness
                case 51:  //area flexibility
                case 53:  //area strength
                    p[0] = "Unknown";
                    p[1] = "Unknown";
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
                    p[0] = "Time between ticks (ms) (?)";
                    p[1] = "1st tick check";
                    p[2] = "2nd tick check";
                    p[3] = "3rd tick check";
                    p[4] = "Tick count";
                    p[5] = "Unknown";
                    p[6] = "Max level affected";
                    break;
                case 54:  //guard (?)
                    p[0] = "Unknown";
                    p[1] = "Duration";
                    break;
                case 55:  //remove curse (?)
                    p[0] = "Unknown";
                    p[1] = "Unknown";
                    p[2] = "Unknown";
                    break;
                case 56:  //regenerate
                    p[0] = "Heal amount";
                    p[1] = "Time between ticks (ms)";
                    p[2] = "Tick count";
                    break;
                case 57: //holy might
                    p[0] = "Unknown";
                    p[1] = "Unknown";
                    p[2] = "Unknown";
                    p[3] = "Unknown";
                    p[4] = "Time between ticks (ms) (?)";
                    p[5] = "Duration (ms) (?)";
                    break;
                case 62:  //forget
                    p[0] = "Unknown";
                    p[1] = "Unknown";
                    p[2] = "Unknown";
                    p[3] = "Unknown";
                    break;
                case 63:  //self illusion
                    p[0] = "Illusions summoned";
                    p[1] = "Unknown";
                    p[2] = "Unknown";
                    p[3] = "Unknown";
                    break;
                case 64:  //retention
                    p[0] = "Unknown";
                    p[1] = "Duration (ms) (?)";
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
                    p[0] = "Unknown";
                    p[1] = "Damage";
                    p[2] = "Unknown";
                    p[3] = "Unknown";
                    break;
                case 70:  //disrupt
                    p[0] = "Unknown";
                    p[1] = "Unknown";
                    p[2] = "Unknown";
                    p[3] = "Unknown";
                    break;
                case 71:  //fear
                    p[0] = "Unknown";
                    p[1] = "Unknown";
                    p[2] = "Unknown";
                    p[3] = "Duration (ms) (?)";
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
                    p[2] = "Unknown";
                    p[3] = "Unknown";
                    p[4] = "Sub-effect ID";
                    break;
                case 75:  //acid cloud
                    p[0] = "Area radius";
                    p[1] = "Time between ticks (ms)";
                    p[2] = "Tick count";
                    p[3] = "Damage per tick";
                    break;
                case 77:  //wall of rocks
                case 78:  //ring of rocks
                    p[0] = "Unknown";
                    p[1] = "Duration (ms) (?)";
                    break;
                case 80:  //extinct
                    p[0] = "Health threshold";
                    p[1] = "Unknown";
                    break;
                case 82:  //detect metal
                case 83:  //detect magic
                    p[0] = "Radius";
                    break;
                case 86:  //invisibility
                    p[0] = "Time between ticks (ms)";
                    p[1] = "Tick count";
                    p[2] = "Unknown";
                    p[3] = "Unknown";
                    p[4] = "Unknown";
                    p[5] = "Unknown";
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
                    p[1] = "Unknown";
                    p[2] = "Aura range";
                    p[3] = "Unknown";
                    p[4] = "Unknown";
                    p[5] = "Aura target type";
                    p[6] = "Sub-effect ID";
                    p[7] = "Unknown";
                    p[8] = "Mana per tick";
                    break;
                case 90:  //suicide death
                    p[0] = "Damage inflicted";
                    p[1] = "Damage received";
                    break;
                case 93:  //feign death
                    p[0] = "Unknown";
                    p[1] = "Unknown";
                    p[2] = "Unknown";
                    p[3] = "Unknown";
                    p[4] = "Unknown";
                    break;
                case 96:  //dispel white aura
                case 112: //dispel black aura
                    p[0] = "Unknown";
                    p[1] = "Unknown";
                    p[2] = "Unknown";
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
                    p[0] = "Unknown";
                    p[1] = "Unknown";
                    break;
                case 123: //befriend
                case 124: //disenchant
                    p[0] = "Max level affected";
                    break;
                case 126: //shock
                    p[0] = "Unknown";
                    p[1] = "Damage";
                    p[2] = "Unknown";
                    p[3] = "Unknown";
                    p[4] = "Area radius";
                    break;
                case 134: //wave of fire
                case 137: //wave of ice
                case 142: //wave of rocks
                    p[0] = "Min projectile amount";
                    p[1] = "Bonus projectile chance";
                    p[2] = "2nd bonus projectile chance";
                    p[3] = "Sub-effect ID";
                    p[4] = "Wave range (?)";
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
                    p[0] = "Unknown";
                    p[1] = "Duration (ms)";
                    break;
                case 165: //unknown?
                    p[0] = "Unknown";
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
                    break;
                case 176: //elemental essence
                case 180: //elemental almightness
                    p[0] = "Effect ID";
                    break;
                case 177: //mental essence
                case 181: //mental almightness
                    p[0] = "Unknown";
                    p[1] = "Unknown";
                    p[2] = "Unknown";
                    p[3] = "Unknown";
                    p[4] = "Unknown";
                    p[5] = "Unknown";
                    break;
                case 182: //elemental almightness
                case 183: //elemental essence
                    p[0] = "Unknown";
                    p[1] = "Unknown";
                    p[2] = "Unknown";
                    p[3] = "Unknown";
                    p[4] = "Unknown";
                    p[5] = "Unknown";
                    p[6] = "Unknown";
                    break;
                case 184: //assistance
                    p[0] = "Unknown";
                    p[1] = "Unknown";
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
                    p[2] = "Unknown";
                    p[3] = "Unknown";
                    p[4] = "Max level affected";
                    p[5] = "Unknown";
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
                    p[1] = "Unknown";
                    p[2] = "Unknown";
                    break;
                case 200: //area of darkness
                    p[0] = "Duration (ms)";
                    p[1] = "Effect strength (%)";
                    p[1] = "Unknown (radius?)";
                    break;
                case 207: //area freeze
                    p[0] = "Freeze duration (ms)";
                    p[1] = "Unknown";
                    p[2] = "Unknown";
                    break;
                case 210: //feet of clay
                    p[0] = "Time between ticks (ms)";
                    p[1] = "Unknown";
                    p[2] = "Unknown";
                    p[3] = "Unknown";
                    p[4] = "Unknown";
                    break;
                case 211: //mirage
                    p[0] = "Unknown";
                    p[1] = "Unknown";
                    p[2] = "Unknown";
                    break;
                case 213: //feedback
                    p[0] = "Unknown";
                    p[1] = "Unknown";
                    break;
                case 215: //area hypnotize
                    p[0] = "Time between ticks (ms)";
                    p[1] = "Unknown";
                    p[2] = "Max level affected";
                    p[3] = "Unknown";
                    p[4] = "Unknown";
                    break;
                case 216: //area confuse
                    p[0] = "Duration";
                    p[1] = "Max level affected";
                    p[2] = "Unknown";
                    p[3] = "Unknown";
                    break;
                case 218: //manashield
                    p[0] = "Unknown";
                    break;
                case 219: //manashift
                    p[0] = "Area radius";
                    p[1] = "Mana per unit";
                    break;
                case 220: //shift life
                    p[0] = "Unknown";
                    p[1] = "Unknown";
                    break;
                case 221: //riposte
                    p[0] = "Unknown";
                    p[1] = "Duration (ms)";
                    break;
                case 222: //critical hits
                    p[0] = "Duration (ms)";
                    p[1] = "Unknown";
                    p[2] = "Unknown";
                    p[3] = "Unknown";
                    p[4] = "Unknown";
                    p[5] = "Unknown";
                    p[6] = "Unknown";
                    p[7] = "Unknown";
                    p[8] = "Unknown";
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
    public class SFCategoryManager
    {
        private SFCategory[] categories;      //array of categories
        private int categoryNumber;           //amount of categories (basically a constant)
        private Byte[] mainHeader;            //gamedata.cff has a main header which is held here
        private SFCategoryRuneHeroes categorySpecial_RuneHeroes;    //intermediary needed to find names of rune heroes

        //constructor, it creates categories
        public SFCategoryManager()
        {
            categoryNumber = 49;
            categories = new SFCategory[categoryNumber];
            for (int i = 1; i <= categoryNumber; i++)
            {
                categories[i - 1] = Assembly.GetExecutingAssembly().CreateInstance("SpellforceDataEditor.SFCategory" + i.ToString()) as SFCategory;
                categories[i - 1].set_manager(this);
            }
            mainHeader = new Byte[20];
            categorySpecial_RuneHeroes = new SFCategoryRuneHeroes();
            categorySpecial_RuneHeroes.set_manager(this);
        }

        //returns category, given its index
        public SFCategory get_category(int index)
        {
            return categories[index];
        }

        //loads gamedata.cff file
        //bar has a maximum value of 10000
        public void load_cff(string filename, ToolStripProgressBar bar)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs, Encoding.Default);

            mainHeader = br.ReadBytes(mainHeader.Length);
            for (int i = 0; i < categoryNumber; i++)
            {
                get_category(i).read(br);
                bar.Value = (i * bar.Maximum) / categoryNumber;
            }
            categorySpecial_RuneHeroes.generate(this);

            br.Close();
            fs.Close();
        }

        //saves gamedata.cff file
        public void save_cff(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            fs.SetLength(0);
            BinaryWriter bw = new BinaryWriter(fs, Encoding.Default);

            bw.Write(mainHeader);
            for (int i = 0; i < categoryNumber; i++)
            {
                get_category(i).write(bw);
            }

            bw.Close();
            fs.Close();
        }

        //returns category count
        public int get_category_number()
        {
            return categoryNumber;
        }

        //using the fact that all text data is saved in category 15 (ind 14)
        //searches for a text with a given ID and in a given language
        public SFCategoryElement find_element_text(int t_index, int t_lang)
        {
            int closest_index = categories[14].find_binary_element_index<UInt16>(0, (UInt16)t_index);
            int cur_index = closest_index;
            int backup_index = -1;
            while (true)
            {
                if (cur_index < 0)
                    break;
                if ((int)(UInt16)categories[14].get_element(cur_index).get_single_variant(0).value != t_index)
                    break;
                if ((int)(Byte)categories[14].get_element(cur_index).get_single_variant(1).value == t_lang)
                    return categories[14].get_element(cur_index);
                if (backup_index == -1)
                    if ((int)(Byte)categories[14].get_element(cur_index).get_single_variant(1).value == 0)
                        backup_index = cur_index;
                cur_index--;
            }
            cur_index = closest_index + 1;
            while (true)
            {
                if (cur_index >= categories[14].get_element_count())
                    break;
                if ((int)(UInt16)categories[14].get_element(cur_index).get_single_variant(0).value != t_index)
                    break;
                if ((int)(Byte)categories[14].get_element(cur_index).get_single_variant(1).value == t_lang)
                    return categories[14].get_element(cur_index);
                if (backup_index == -1)
                    if ((int)(Byte)categories[14].get_element(cur_index).get_single_variant(1).value == 0)
                        backup_index = cur_index;
                cur_index++;
            }
            return categories[14].get_element(backup_index);
        }

        //returns a name of a given effect
        //it can also add the effect level
        public string get_effect_name(UInt16 effect_id, bool effect_level = false)
        {
            SFCategoryElement effect_elem = get_category(0).find_binary_element<UInt16>(0, effect_id);
            if (effect_elem == null)
                return "<no name>";
            UInt16 spell_type = (UInt16)effect_elem.get_single_variant(1).value;
            SFCategoryElement spell_elem = get_category(1).find_binary_element<UInt16>(0, spell_type);
            if (spell_elem == null)
                return "<no name>";
            int text_id = (int)(UInt16)spell_elem.get_single_variant(1).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            if (text_elem == null)
                return "<text missing>";
            string txt = Utility.CleanString(text_elem.get_single_variant(4));
            if (effect_level)
                txt += " level " + effect_elem.get_single_variant(4).value.ToString();
            return txt;
        }

        //returns a name of a given unit
        public string get_unit_name(UInt16 unit_id)
        {
            SFCategoryElement unit_elem = get_category(17).find_binary_element<UInt16>(0, unit_id);
            if (unit_elem == null)
                return "<no name>";
            int text_id = (int)(UInt16)unit_elem.get_single_variant(1).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            if (text_elem == null)
                return "<text missing>";
            string txt = Utility.CleanString(text_elem.get_single_variant(4));
            return txt;
        }

        //used for determining skill name
        private string get_resource_gather(Byte ind)
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
                    return "<no name>";
            }
        }

        //returns a name of a given skill
        public string get_skill_name(Byte skill_major, Byte skill_minor, Byte skill_lvl)
        {
            string txt_major = "";
            string txt_minor = "";
            int major_index = (int)(UInt16)get_category(26).find_element_index<Byte>(0, skill_major);
            if (major_index == -1)
            {
                return "<unknown string>";
            }
            int total_index = major_index + (int)skill_minor;
            int text_id_major = (int)(UInt16)get_category(26).get_element_variant(major_index, 2).value;
            SFCategoryElement txt_elem_major = find_element_text(text_id_major, 1);
            if (txt_elem_major != null)
                txt_major = Utility.CleanString(txt_elem_major.get_single_variant(4));
            if((skill_major == 0)&&(skill_minor != 0))
            {
                txt_major = "";
                txt_minor = get_resource_gather(skill_minor) + " gathering";
            }
            else if (skill_minor != 101)
            {
                int text_id_minor = (int)(UInt16)get_category(26).get_element_variant(total_index, 2).value;
                if ((text_id_minor != 0) && (major_index != total_index))
                {
                    SFCategoryElement txt_elem_minor = find_element_text(text_id_minor, 1);
                    if(txt_elem_minor != null)
                        txt_minor = Utility.CleanString(txt_elem_minor.get_single_variant(4));
                }
            }
            return txt_major + " " + txt_minor + " " + skill_lvl.ToString();
        }

        //returns a name of a given race
        public string get_race_name(Byte race_id)
        {
            SFCategoryElement race_elem = get_category(15).find_binary_element<Byte>(0, race_id);
            if (race_elem == null)
                return "<no name>";
            int text_id = (int)(UInt16)race_elem.get_single_variant(7).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            if (text_elem == null)
                return "<text missing>";
            string txt = Utility.CleanString(text_elem.get_single_variant(4));
            return txt;
        }

        //returns a name of a given item
        public string get_item_name(UInt16 item_id)
        {
            SFCategoryElement item_elem = get_category(6).find_binary_element<UInt16>(0, item_id);
            if (item_elem == null)
                return "<no name>";
            int text_id = (int)(UInt16)item_elem.get_single_variant(3).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            if (text_elem == null)
                return "<text missing>";
            string txt = Utility.CleanString(text_elem.get_single_variant(4));
            return txt;
        }

        //returns a name of a given building
        public string get_building_name(UInt16 building_id)
        {
            SFCategoryElement building_elem = get_category(23).find_binary_element<UInt16>(0, building_id);
            if (building_elem == null)
                return "<no name>";
            int text_id = (int)(UInt16)building_elem.get_single_variant(5).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            if (text_elem == null)
                return "<text missing>";
            string txt = Utility.CleanString(text_elem.get_single_variant(4));
            return txt;
        }

        //returns a name of a given merchant
        public string get_merchant_name(UInt16 merchant_id)
        {
            SFCategoryElement merchant_elem = get_category(28).find_binary_element<UInt16>(0, merchant_id);
            if (merchant_elem == null)
                return "<no name>";
            return get_unit_name((UInt16)merchant_elem.get_single_variant(1).value);
        }

        //returns a name of a given object
        public string get_object_name(UInt16 object_id)
        {
            SFCategoryElement object_elem = get_category(33).find_binary_element<UInt16>(0, object_id);
            if (object_elem == null)
                return "<no name>";
            int text_id = (int)(UInt16)object_elem.get_single_variant(1).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            if (text_elem == null)
                return "<text missing>";
            string txt = Utility.CleanString(text_elem.get_single_variant(4));
            return txt;
        }

        //returns a description given its id
        public string get_description_name(UInt16 desc_id)
        {
            SFCategoryElement desc_elem = get_category(40).find_binary_element<UInt16>(0, desc_id);
            if (desc_elem == null)
                return "<no name>";
            int text_id = (int)(UInt16)desc_elem.get_single_variant(1).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            if (text_elem == null)
                return "<text missing>";
            string txt = Utility.CleanString(text_elem.get_single_variant(4));
            return txt;
        }

        public string get_runehero_name(UInt16 stats_id)
        {
            SFCategoryElement rune_elem = categorySpecial_RuneHeroes.find_binary_element<UInt16>(0, stats_id);
            if (rune_elem == null)
                return "<no name>";
            int text_id = (int)(UInt16)rune_elem.get_single_variant(1).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            if (text_elem == null)
                return "<text missing>";
            return Utility.CleanString(text_elem.get_single_variant(4));
        }

        //returns a list of indices
        //these indices correspond with all elements which contain given value in a given column
        //value is numeric in this query
        public List<int> query_by_column_numeric(int categoryindex, int columnindex, int value)
        {
            List<int> items = new List<int>();
            SFCategory cat = get_category(categoryindex);
            for (int i = 0; i < cat.get_element_count(); i++)
            {
                SFVariant variant = cat.get_element_variant(i, columnindex);
                int current_value = variant.to_int();
                if (current_value == value)
                    items.Add(i);
            }
            return items;
        }

        //returns a list of indices
        //these indices correspond with all elements which contain given value in a given column
        //value is a text in this query
        public List<int> query_by_column_text(int categoryindex, int columnindex, string value)
        {
            List<int> items = new List<int>();
            SFCategory cat = get_category(categoryindex);
            for (int i = 0; i < cat.get_element_count(); i++)
            {
                SFVariant variant = cat.get_element_variant(i, columnindex);
                string current_value = Utility.CleanString(variant);
                if (current_value.Contains(value))
                    items.Add(i);
            }
            return items;
        }

        //frees all data, only empty categories remain
        public void unload_all()
        {
            foreach (SFCategory cat in categories)
                cat.unload();
            categorySpecial_RuneHeroes.unload();
            mainHeader = new Byte[20];
        }
    }
}
