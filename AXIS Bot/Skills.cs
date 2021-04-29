using System;
using System.Text;

namespace AXIS_Bot
{
    static class Skills
    {
        private static int Agility;
        private static int Constitution;
        private static int Luck;
        private static int Precision;
        private static int Stamina;
        private static int Strength;

        private static double Dodge;
        private static double Parry;
        private static double EvasionChance;

        private static double Health;
        private static double Action;

        private static double EvasionValue;
        private static double CriticalHitChance;
        private static double StrikethroughChance;
        private static double StrikethroughValue;

        private static double BlockChance;

        private static double BlockValue;

        private static double HitChance;

        private static double MeleeDamage;

        public static string AllSkills(string skills)
        {
            AllSkillsTrim(skills);

            AgilitySkills(Agility);
            ConstitutionSkills(Constitution);
            LuckSkills(Luck);
            PrecisionSkills(Precision);
            StaminaSkills(Stamina);
            StrengthSkills(Stamina);

            StringBuilder sb = new StringBuilder();
            sb.Append("Health: +" + Health + "\n");
            sb.Append("Action: +" + Action + "\n\n");

            sb.Append("Dodge: " + Math.Round(Dodge, 1) + "%\n");
            sb.Append("Parry: " + Math.Round(Parry, 1) + "%\n");
            sb.Append("Evasion Chance: " + Math.Round(EvasionChance, 1)  + "%\n");
            sb.Append("Evasion Value: " + Math.Round(EvasionValue, 1) + "\n");
            sb.Append("Block Chance " + Math.Round(BlockChance, 1) + "%\n");
            sb.Append("Block Value: " + Math.Round(BlockValue, 1) + "\n\n");

            sb.Append("Hit Chance: " + Math.Round(HitChance, 1) + "%\n");
            sb.Append("Critical Hit Chance: " + Math.Round(CriticalHitChance, 1) + "%\n");
            sb.Append("Strikethrough Chance: " + Math.Round(StrikethroughChance, 1) + "%\n");
            sb.Append("Strikethrough Value: " + Math.Round(StrikethroughValue, 1) + "%\n\n");

            sb.Append("Melee Damage: +" + MeleeDamage);

            return sb.ToString();
        }

        public static void AgilitySkills(int agi)
        {
            Dodge += agi / 100;
            Parry += agi / 200;
            EvasionChance += agi / 100;
        }

        public static void ConstitutionSkills(int con)
        {
            Health += con * 8;
            Action += con * 2;
        }

        public static void LuckSkills(int luck)
        {
            Dodge += luck / 300;
            EvasionChance += luck / 300;
            EvasionValue += luck / 10;
            CriticalHitChance += Math.Round((double)luck / 300);
            StrikethroughChance += luck / 200;
            StrikethroughValue += luck / 10;
        }

        public static void PrecisionSkills(int precision)
        {
            Parry += precision / 200;
            BlockChance += precision / 200;
            CriticalHitChance += precision / 100;
            StrikethroughChance += precision / 200;
        }

        public static void StaminaSkills(int stam)
        {
            Health += stam * 2;
            Action += stam * 8;
        }

        public static void StrengthSkills(int strength)
        {
            BlockChance += strength / 200;
            BlockValue += strength / 2;
            HitChance += strength / 100;
            MeleeDamage += (strength / 100) * 33;
        }

        public static void ResetSkills()
        {
            Dodge = 0;
            Parry = 0;
            EvasionChance = 0;

            Health = 0;
            Action = 0;

            EvasionValue = 0;
            CriticalHitChance = 0;
            StrikethroughChance = 0;
            StrikethroughValue = 0;

            BlockChance = 0;

            BlockValue = 0;

            HitChance = 0;

            MeleeDamage = 0;
        }
 
        private static void AllSkillsTrim(string skills)
        {
            var _skills = skills.Replace("!skills ", "");

            var words = _skills.Split(' ');

            Strength = Convert.ToInt16(words[0]);
            Constitution = Convert.ToInt16(words[1]);
            Stamina = Convert.ToInt16(words[2]);
            Precision = Convert.ToInt16(words[3]);
            Agility = Convert.ToInt16(words[4]);
            Luck = Convert.ToInt16(words[5]);
        }
    }
}
