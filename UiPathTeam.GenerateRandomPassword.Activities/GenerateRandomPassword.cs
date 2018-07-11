using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;

namespace UiPathTeam.GenerateRandomPassword.Activities
{
    [DisplayName("Generate Random Password")]
    public class GenerateRandomPassword: CodeActivity
    {
        [Category("Input")]
        [DisplayName("Minimum password length")]
        public InArgument<Int32> MinLength { get; set; }
        [Category("Input")]
        [DisplayName("Maximum password length")]
        public InArgument<Int32> MaxLength { get; set; }
        [Category("Input")]
        [DisplayName("Required digits")]
        public InArgument<Int32> RequiredDigits{ get; set; }
        [Category("Input")]
        [DisplayName("Required uppercase letters")]
        public InArgument<Int32> RequiredUpperCaseLetters{ get; set; }
        [Category("Input")]
        [DisplayName("Required lowercase letters")]
        public InArgument<Int32> RequiredLowerCaseLetters { get; set; }
        [Category("Input")]
        [DisplayName("Required special characters")]
        public InArgument<Int32> RequiredSpecialChars { get; set; }

        [Category("Input")]
        [DisplayName("Required special characters")]
        [Description("If not provided, will default to:\" !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~")]
        public InArgument<String> AllowedSpecialChars { get; set; }

        [Category("Output")]
        [DisplayName("Password")]
        public OutArgument<string> Password { get; set; }

        private static string LowerCaseLetters = "abcdefgijkmnopqrstwxyz";
        private static string UpperCaseLetters = "ABCDEFGHJKLMNPQRSTWXYZ";
        private static string Digits = "0123456789";
        private static string SpecialChars = " !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";

        protected override void Execute(CodeActivityContext context)
        {
            var minLength = MinLength.Get(context);
            var maxLength = MaxLength.Get(context);
            var requiredDigits = RequiredDigits.Get(context);
            var requiredLowerCaseLetters = RequiredLowerCaseLetters.Get(context);
            var requiredUpperCaseLetters = RequiredUpperCaseLetters.Get(context);
            var requiredNonAlphaNumericChars = RequiredSpecialChars.Get(context);

            SpecialChars = string.IsNullOrEmpty(AllowedSpecialChars.Get(context)) ? SpecialChars : AllowedSpecialChars.Get(context);

            var allAvailableChars = LowerCaseLetters + UpperCaseLetters + Digits + SpecialChars;

            if (minLength < requiredDigits + requiredLowerCaseLetters + requiredUpperCaseLetters + requiredNonAlphaNumericChars)
            {
                throw new ArgumentException("Min length is lower than total required length");
            }

            var randomBytes = new Byte[4];
            var rngCrypto = new RNGCryptoServiceProvider();
            rngCrypto.GetBytes(randomBytes);

            var randomGenerator = new Random(BitConverter.ToInt32(randomBytes, 0));

            var passwordLength = randomGenerator.Next(minLength, maxLength + 1);
            var selectedChars = new List<Char>();

            for (int i = 0; i < requiredDigits; i++)
            {
                selectedChars.Add(Digits.ToCharArray()[randomGenerator.Next(0, Digits.Length)]);
            }

            for (int i = 0; i < requiredLowerCaseLetters; i++)
            {
                selectedChars.Add(LowerCaseLetters.ToCharArray()[randomGenerator.Next(0, LowerCaseLetters.Length)]);
            }

            for (int i = 0; i < requiredUpperCaseLetters; i++)
            {
                selectedChars.Add(UpperCaseLetters.ToCharArray()[randomGenerator.Next(0, UpperCaseLetters.Length)]);
            }

            for (int i = 0; i < requiredNonAlphaNumericChars; i++)
            {
                selectedChars.Add(SpecialChars.ToCharArray()[randomGenerator.Next(0, SpecialChars.Length)]);
            }

            var remainingChars = passwordLength - selectedChars.Count;
            for (int i = 0; i < remainingChars; i++)
            {
                selectedChars.Add(allAvailableChars.ToCharArray()[randomGenerator.Next(0, allAvailableChars.Length)]);
            }

            var passwordCharArray = new Char[passwordLength];
            var emptyLocations = Enumerable.Range(0, passwordLength).ToList();

            foreach (var selectedChar in selectedChars)
            {
                var emptyLocationIndex = randomGenerator.Next(0, emptyLocations.Count);
                passwordCharArray[emptyLocations[emptyLocationIndex]] = selectedChar;
                emptyLocations.RemoveAt(emptyLocationIndex);
            }

            Password.Set(context, string.Join("", passwordCharArray));
        }
    }
}
