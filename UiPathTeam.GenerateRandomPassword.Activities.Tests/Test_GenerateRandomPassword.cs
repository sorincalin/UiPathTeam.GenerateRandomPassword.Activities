using System;
using System.Activities;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UiPathTeam.GenerateRandomPassword.Activities.Tests
{
    [TestClass]
    public class Test_GenerateRandomPassword
    {
        [TestMethod]
        public void Test_GenerateRandomPassword_RandomInputs()
        {
            var allowedSpecial = "[]()!@";
            var randomBytes = new Byte[4];
            var rngCrypto = new RNGCryptoServiceProvider();
            rngCrypto.GetBytes(randomBytes);
            var randomGenerator = new Random(BitConverter.ToInt32(randomBytes, 0));

            for (int idx = 0; idx < 100000; idx++)
            {
                var minLength = randomGenerator.Next(10, 30 + 1);
                var maxLength = randomGenerator.Next(minLength, minLength + 20 + 1);
                var requiredLC = randomGenerator.Next(0, 5 + 1);
                var requiredUC = randomGenerator.Next(0, 5 + 1);
                var requiredSpecial = randomGenerator.Next(0, 5 + 1);
                var requiredDigits = randomGenerator.Next(0, 5 + 1);

                var generateRandomPassword = new GenerateRandomPassword
                {
                    MinLength = minLength,
                    MaxLength = maxLength,
                    RequiredLowerCaseLetters = requiredLC,
                    RequiredUpperCaseLetters = requiredUC,
                    RequiredSpecialChars = requiredSpecial,
                    RequiredDigits = requiredDigits,
                    AllowedSpecialChars = allowedSpecial
                };

                var generatedPassword = string.Empty;
                try
                {
                    var output = WorkflowInvoker.Invoke(generateRandomPassword);
                    generatedPassword = Convert.ToString(output["Password"]);

                    Assert.IsTrue(minLength <= generatedPassword.Length && generatedPassword.Length <= maxLength);
                    Assert.IsTrue(generatedPassword.Count(Char.IsDigit) >= requiredDigits);
                    Assert.IsTrue(generatedPassword.Count(Char.IsLower) >= requiredLC);
                    Assert.IsTrue(generatedPassword.Count(Char.IsUpper) >= requiredUC);
                    Assert.IsTrue(generatedPassword.Where(c => Char.IsLetterOrDigit(c) == false).All(allowedSpecial.Contains));
                }
                catch (ArgumentException)
                {
                    Assert.IsTrue(minLength < requiredLC + requiredUC + requiredDigits + requiredSpecial);
                }
            }
        }
    }
}
