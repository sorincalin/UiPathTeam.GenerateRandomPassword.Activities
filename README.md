# UiPathTeam.GenerateRandomPassword.Activities

<b>Summary</b>

Generates a random password with various criteria: min&max length, required number of lowercase&uppercase letters, required number of digits and required number of special characters. 

<b>Benefits</b>

Facilitates password maintenance processes. Some Robots have to generate their own new passwords at expiration time.

<b>Package specifications</b>	

Generates a random password with various criteria: min&max length, required number of lowercase&uppercase letters, required number of digits and required number of special characters. For random number generation it utilizes System.Security.Cryptography.RNGCryptoServiceProvider. All length requirements have to be below 1000 chars.

InArguments
* int Minimum password length
* int Maximum password length
* int Required digits
* int Required uppercase letters
* int Required lowercase letters
* int Required special characters
* string Allowed special characters - If not provided, will default to:\" !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~")]

OutArguments 
* string Password { get; set; }
