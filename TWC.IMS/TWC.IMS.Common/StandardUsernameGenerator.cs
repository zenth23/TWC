using TWC.IMS.Common.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common
{
    public static class StandardUsernameGenerator
    {
        /*  RULES
         *  lowercase, maximum character = 255
         *      
         *  If firstname has Maria or Ma. as prefix, prepend "m" to username
         *  If middlename is "de la" "de los" etc., use middlename instead of middle initial
         *  If user has suffix, append suffix to username without the period "."
         *  
         *  (m + firstname initial | firstname initial) + (lastname | lastname + suffix)
         *  (m + firstname initial | firstname initial) + (middlename | middle initial) + (lastname | lastname + suffix)
         *  nickname + (lastname | lastname + suffix)
         */

        private enum RULES
        {
            GENERAL_RULE,
            WITH_MIDDLENAME,
            WITH_NICKNAME
        }

        private static async Task<bool> Validate(string username, int maxlength = 255)
        {
            int length = username.Length;
            bool isUnique = false;
            using (var anuDL = new AspNetUsers())
            {
                isUnique = await anuDL.IsUsernameUniqueAsync(username).ConfigureAwait(false);
            }
            return length <= maxlength && isUnique;
        }

        private static string Generate(string rule, string firstname, string lastname, string middlename = "", string suffix = "", string nickname = "")
        {
            string username = string.Empty;
            var currentRule = (RULES)Enum.Parse(typeof(RULES), rule);
            switch (currentRule)
            {
                case RULES.GENERAL_RULE:
                    if (!string.IsNullOrWhiteSpace(firstname) && !string.IsNullOrWhiteSpace(lastname))
                        username = $"{firstname}{lastname}";
                    break;

                case RULES.WITH_MIDDLENAME:
                    if (!string.IsNullOrWhiteSpace(middlename))
                        username = $"{firstname}{middlename}{lastname}";
                    break;

                case RULES.WITH_NICKNAME:
                    if (!string.IsNullOrEmpty(nickname))
                        username = $"{nickname}{lastname}";
                    break;
            }
            return username;
        }

        public static async Task<string> GenerateUsernameAsync(string firstname, string lastname, string middlename = "", string suffix = "", string nickname = "")
        {
            if (string.IsNullOrWhiteSpace(firstname) || string.IsNullOrWhiteSpace(lastname))
                throw new Exception($"First Name and Last Name fields are required.");

            string username = string.Empty;
            var formattedStr = Format.GetFormattedStrings(firstname, lastname, middlename, suffix, nickname);

            var rules = Enum.GetNames(typeof(RULES));
            foreach (var rule in rules)
            {
                var result = Generate(rule, formattedStr.Item1, formattedStr.Item2, formattedStr.Item3, formattedStr.Item4, formattedStr.Item5);
                var isValid = await Validate(result).ConfigureAwait(false);
                if (isValid)
                {
                    username = result;
                    break;
                }
            }
            return username.ToLower().Trim();
        }
    }

    internal static class Format
    {
        public static Tuple<string, string, string, string, string> GetFormattedStrings(string firstname, string lastname, string middlename = "", string suffix = "", string nickname = "")
        {
            var _firstname = firstname.Split().Length > 1 ? firstname.Substring(0, 3).ToLower() == "ma." || firstname.Split()[0].ToLower() == "maria" ? "m" + firstname.Split()[1][0].ToString().ToLower() : firstname[0].ToString().ToLower() : firstname[0].ToString().ToLower();
            var _lastname = lastname.Replace(" ", string.Empty).ToLower();
            var _middlename = string.IsNullOrWhiteSpace(middlename) ? string.Empty : middlename.Split().Length > 1 ? middlename.Replace(" ", string.Empty).ToLower() : middlename[0].ToString().ToLower();
            var _suffix = string.IsNullOrWhiteSpace(suffix) ? string.Empty : suffix.Replace(".", string.Empty).Replace(" ", string.Empty).ToLower();
            var _nickname = string.IsNullOrWhiteSpace(nickname) ? firstname.Split()[0].ToLower() : nickname.Replace(" ", string.Empty).ToLower();

            if (!string.IsNullOrWhiteSpace(suffix))
                _lastname = _lastname + _suffix;

            return new Tuple<string, string, string, string, string>(_firstname, _lastname, _middlename, _suffix, _nickname);
        }
    }
}