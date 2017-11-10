﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
#if NET452
using System.Security.Cryptography;
#endif
using JetBrains.Annotations;

namespace Saritasa.Tools.Misc.Security
{
    /// <summary>
    /// Passwords generation and strength estimation.
    /// </summary>
    public class PasswordGenerator
    {
        /// <summary>
        /// Minimum password length.
        /// </summary>
        public const int MinPasswordLength = 2;

        /// <summary>
        /// Default password length.
        /// </summary>
        public const int DefaultPasswordLength = 10;

        /// <summary>
        /// Enum specifies what characters to use for password generation.
        /// </summary>
        [Flags]
        public enum CharacterClass
        {
            /// <summary>
            /// Lower letters.
            /// </summary>
            LowerLetters = 1,

            /// <summary>
            /// Upper letters.
            /// </summary>
            UpperLetters = 2,

            /// <summary>
            /// Digits.
            /// </summary>
            Digits = 4,

            /// <summary>
            /// Special characters except space.
            /// </summary>
            SpecialCharacters = 8,

            /// <summary>
            /// Space character.
            /// </summary>
            Space = 16,

            /// <summary>
            /// Combination of LowerLetters and UpperLetters.
            /// </summary>
            AllLetters = LowerLetters | UpperLetters,

            /// <summary>
            /// Combination of AllLetters and Digits.
            /// </summary>
            AlphaNumeric = AllLetters | Digits,

            /// <summary>
            /// Combination of all elements.
            /// </summary>
            All = LowerLetters | UpperLetters | Digits | SpecialCharacters | Space,
        }

        /// <summary>
        /// Enums specifies special processing for password generation.
        /// </summary>
        [Flags]
        public enum GeneratorFlag
        {
            /// <summary>
            /// No special generation flags.
            /// </summary>
            None = 0x0,

            /// <summary>
            /// Exclude conflict characters.
            /// </summary>
            ExcludeLookAlike = 0x1,

            /// <summary>
            /// Shuffle pool characters before generation.
            /// </summary>
            ShuffleChars = 0x2,

            /// <summary>
            /// Makes secure string as read only.
            /// </summary>
            MakeReadOnly = 0x4,
        }

        /// <summary>
        /// Possible additions of standards exceeding.
        /// </summary>
        public enum Addition
        {
            /// <summary>
            /// Number of characters.
            /// </summary>
            NumberOfCharacters,

            /// <summary>
            /// Uppercase letters.
            /// </summary>
            UppercaseLetters,

            /// <summary>
            /// Lowercase letters.
            /// </summary>
            LowercaseLetters,

            /// <summary>
            /// Numbers.
            /// </summary>
            Numbers,

            /// <summary>
            /// Symbols.
            /// </summary>
            Symbols,

            /// <summary>
            /// Middle number or symbols.
            /// </summary>
            MiddleNumbersOrSymbols,

            /// <summary>
            /// Requirements.
            /// </summary>
            Requirements,

            /// <summary>
            /// Letterns only.
            /// </summary>
            LettersOnly,

            /// <summary>
            /// Numbers only.
            /// </summary>
            NumbersOnly,

            /// <summary>
            /// Repeat characters (case insensitive).
            /// </summary>
            RepeatCharacters,

            /// <summary>
            /// Consecutive Uppercase Letters.
            /// </summary>
            ConsecutiveUppercaseLetters,

            /// <summary>
            /// Consecutive Lowercase Letters
            /// </summary>
            ConsecutiveLowercaseLetters,

            /// <summary>
            /// Consecutive Numbers
            /// </summary>
            ConsecutiveNumbers,

            /// <summary>
            /// Sequential Letters (3+).
            /// </summary>
            SequentialLetters,

            /// <summary>
            /// Sequential Numbers (3+).
            /// </summary>
            SequentialNumbers,

            /// <summary>
            /// Sequential Symbols (3+).
            /// </summary>
            SequentialSymbols,
        }

        /// <summary>
        /// Password length. Default is 10.
        /// </summary>
        public int PasswordLength { get; set; }

        /// <summary>
        /// Character classes for password generation. Default is All.
        /// </summary>
        public CharacterClass CharacterClasses { get; set; }

        /// <summary>
        /// Generator flags. Default is None.
        /// </summary>
        public GeneratorFlag GeneratorFlags { get; set; }

        /// <summary>
        /// Instance of random generation class.
        /// </summary>
#if NET452
        public static RandomNumberGenerator RandomService { get; protected set; }
#else
        public static Random RandomService { get; protected set; }
#endif

        /// <summary>
        /// Lock object for RandomService.
        /// </summary>
        private static readonly object randomServiceLock = new object();

        /// <summary>
        /// Characters pool that is use for password generation.
        /// </summary>
        private string CharactersPool { get; set; }

        /// <summary>
        /// Lower case characters pool.
        /// </summary>
        protected const string PoolLowerCase = "abcdefghjkmnpqrstuvwxyz";

        /// <summary>
        /// Lower case conflict characters pool.
        /// </summary>
        protected const string PoolLowerCaseConflict = "ilo";

        /// <summary>
        /// Upper case characters pool.
        /// </summary>
        protected const string PoolUpperCase = "ABCDEFGHJKLMNPQRSTUVWXYZ";

        /// <summary>
        /// Upper case characters conflict pool.
        /// </summary>
        protected const string PoolUpperCaseConflict = "OI";

        /// <summary>
        /// Digits characters pool.
        /// </summary>
        protected const string PoolDigits = "23456789";

        /// <summary>
        /// Digits conflict characters pool.
        /// </summary>
        protected const string PoolDigitsConflict = "10";

        /// <summary>
        /// Special characters pool.
        /// </summary>
        protected const string PoolSpecial = @"~@#$%^&*()_-+=[]|\:;""'<>.?/";

        /// <summary>
        /// Special characters conflict pool.
        /// </summary>
        protected const string PoolSpecialConflict = @"`{}!,";

        /// <summary>
        /// Space character.
        /// </summary>
        protected const string PoolSpace = " ";

        /// <summary>
        /// .ctor
        /// </summary>
        public PasswordGenerator()
        {
            this.PasswordLength = DefaultPasswordLength;
            this.CharacterClasses = CharacterClass.All;
            this.GeneratorFlags = GeneratorFlag.None;
        }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="passwordLength">Password length to generate.</param>
        /// <param name="characterClasses">What characters classes to use.</param>
        /// <param name="generatorFlags">Special generator flags.</param>
        public PasswordGenerator(int passwordLength, CharacterClass characterClasses, GeneratorFlag generatorFlags) : this()
        {
            if (passwordLength < MinPasswordLength)
            {
                throw new ArgumentException(string.Format(Properties.Strings.PasswordLengthMin, MinPasswordLength));
            }

            this.PasswordLength = passwordLength;
            this.CharacterClasses = characterClasses;
            this.GeneratorFlags = generatorFlags;
        }

        /// <summary>
        /// .cctor
        /// </summary>
        static PasswordGenerator()
        {
#if NET452
            RandomService = RandomNumberGenerator.Create();
#else
            RandomService = new Random();
#endif
        }

        /// <summary>
        /// Set characters pool for password generation. If set it will be used for password generation.
        /// Instead default characters pool will be generated.
        /// </summary>
        /// <param name="pool"></param>
        public void SetCharactersPool(string pool)
        {
            if (string.IsNullOrEmpty(pool))
            {
                throw new ArgumentException(Properties.Strings.CharactersPoolEmpty);
            }
            this.CharactersPool = pool;
        }

        /// <summary>
        /// Generates custom characters pool. The pool set by SetCharactersPool will be ignored.
        /// </summary>
        public void UseDefaultCharactersPool()
        {
            this.CharactersPool = string.Empty;
        }

        /// <summary>
        /// Generates new password to String.
        /// </summary>
        /// <returns>Password.</returns>
        public string Generate()
        {
            var pool = string.IsNullOrEmpty(this.CharactersPool) ? CreateCharactersPool() : this.CharactersPool.ToCharArray();
            var sb = new StringBuilder(PasswordLength);

            if (this.GeneratorFlags.HasFlag(GeneratorFlag.ShuffleChars))
            {
                ShuffleCharsArray(pool);
            }

            for (int i = 0; i < PasswordLength; i++)
            {
                int random = GetNextRandom(pool.Length);
                sb.Append(pool[random]);
            }

            return sb.ToString();
        }

#if NET452
        /// <summary>
        /// Generates new password to SecureString.
        /// </summary>
        /// <returns>Password.</returns>
        public SecureString GenerateSecure()
        {
            var pool = string.IsNullOrEmpty(this.CharactersPool) ? CreateCharactersPool() : this.CharactersPool.ToCharArray();
            var secureString = new SecureString();

            if (this.GeneratorFlags.HasFlag(GeneratorFlag.ShuffleChars))
            {
                ShuffleCharsArray(pool);
            }

            for (int i = 0; i < PasswordLength; i++)
            {
                int random = GetNextRandom(pool.Length);
                secureString.AppendChar(pool[random]);
            }

            if (GeneratorFlags.HasFlag(GeneratorFlag.MakeReadOnly))
            {
                secureString.MakeReadOnly();
            }

            return secureString;
        }
#endif

        /// <summary>
        /// Estimate password strength. See documentation for more details.
        /// </summary>
        /// <param name="password">Password to estimate.</param>
        /// <param name="additions">Password strength properties.</param>
        /// <remarks>
        /// The source code has got from http://www.passwordmeter.com .
        /// </remarks>
        /// <returns>Estimated score.</returns>
        public static int EstimatePasswordStrength(
            [NotNull] string password,
            out IDictionary<Addition, int> additions)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException(Properties.Strings.PasswordNotSet);
            }

            var passwordLower = password.ToLowerInvariant();
            int score;
            int alphasUpperCount = 0, alphasLowerCount = 0, digitsCount = 0, symbolsCount = 0, middleCharsCount = 0,
                requirements = 0, alphasOnlyCount = 0, numbersOnlyCount = 0, uniqueCharsCount = 0, repeatCharsCount = 0,
                consequenceAlphasUpperCount = 0, consequenceAlphasLowerCount = 0, consequenceDigitsCount = 0, consequenceSymbolsCount = 0, consequenceCharsTypeCount = 0,
                sequenceAlphasCount = 0, sequenceNumbersCount = 0, sequenceSymbolsCount = 0, sequenceCharsCount = 0, requiredCharsCount = 0;
            double repeatIncrement = 0;
            int tempAlphaUpperIndex = -1, tempAlphaLowerIndex = -1, tempNumberIndex = -1, tempSymbolIndex = -1;

            const int FactorMiddleChar = 2, FactorConsequenceAlphaUpper = 2, FactorConsequenceAlphaLower = 2, FactorConsequenceNumber = 2;
            const int FactorSequenceAlpha = 3, FactorSequenceNumber = 3, FactorSequenceSymbol = 3;
            const int FactorLength = 4, FactorNumber = 4;
            const int FactorSymbol = 6;
            const string PoolAlphas = "abcdefghijklmnopqrstuvwxyz";
            const string PoolNumerics = "01234567890";
            const string PoolSymbols = ")!@#$%^&*()";
            const int MinimumPasswordLength = 8;

            score = password.Length * FactorLength;

            // Loop through password to check for Symbol, Numeric, Lowercase and Uppercase pattern matches.
            for (var a = 0; a < password.Length; a++)
            {
                if (char.IsUpper(password[a]))
                {
                    if (tempAlphaUpperIndex > -1 && tempAlphaUpperIndex + 1 == a)
                    {
                        consequenceAlphasUpperCount++;
                        consequenceCharsTypeCount++;
                    }
                    tempAlphaUpperIndex = a;
                    alphasUpperCount++;
                }
                else if (char.IsLower(password[a]))
                {
                    if (tempAlphaLowerIndex > -1 && tempAlphaLowerIndex + 1 == a)
                    {
                        consequenceAlphasLowerCount++;
                        consequenceCharsTypeCount++;
                    }
                    tempAlphaLowerIndex = a;
                    alphasLowerCount++;
                }
                else if (char.IsNumber(password[a]))
                {
                    if (a > 0 && a < password.Length - 1)
                    {
                        middleCharsCount++;
                    }
                    if (tempNumberIndex > -1 && tempNumberIndex + 1 == a)
                    {
                        consequenceDigitsCount++;
                        consequenceCharsTypeCount++;
                    }
                    tempNumberIndex = a;
                    digitsCount++;
                }
                else if (Regex.IsMatch(password[a].ToString(), @"[^a-zA-Z0-9_]"))
                {
                    if (a > 0 && a < (password.Length - 1))
                    {
                        middleCharsCount++;
                    }
                    if (tempSymbolIndex > -1 && tempSymbolIndex + 1 == a)
                    {
                        consequenceSymbolsCount++;
                        consequenceCharsTypeCount++;
                    }
                    tempSymbolIndex = a;
                    symbolsCount++;
                }

                // Internal loop through password to check for repeat characters.
                var charExists = false;
                for (var b = 0; b < password.Length; b++)
                {
                    // Repeat character exists.
                    if (password[a] == password[b] && a != b)
                    {
                        charExists = true;
                        /* Calculate icrement deduction based on proximity to identical characters
                           Deduction is incremented each time a new match is discovered
                           Deduction amount is based on total password length divided by the
                           difference of distance between currently selected match */
                        repeatIncrement += Math.Abs(password.Length / (b - a));
                    }
                }
                if (charExists)
                {
                    repeatCharsCount++;
                    uniqueCharsCount = password.Length - repeatCharsCount;
                    repeatIncrement = uniqueCharsCount > 0 ? Math.Ceiling(repeatIncrement / uniqueCharsCount) : Math.Ceiling(repeatIncrement);
                }
            }

            // Check for sequential alpha string patterns (forward and reverse).
            for (var s = 0; s < PoolAlphas.Length - 3; s++)
            {
                var forward = PoolAlphas.Substring(s, 3);
                var reverse = Reverse(forward);
                if (passwordLower.IndexOf(forward, StringComparison.Ordinal) != -1 ||
                    passwordLower.IndexOf(reverse, StringComparison.Ordinal) != -1)
                {
                    sequenceAlphasCount++;
                    sequenceCharsCount++;
                }
            }

            // Check for sequential numeric string patterns (forward and reverse).
            for (var s = 0; s < PoolNumerics.Length - 3; s++)
            {
                var forward = PoolNumerics.Substring(s, 3);
                var reverse = Reverse(forward);
                if (passwordLower.IndexOf(forward, StringComparison.Ordinal) != -1 ||
                    passwordLower.IndexOf(reverse, StringComparison.Ordinal) != -1)
                {
                    sequenceNumbersCount++;
                    sequenceCharsCount++;
                }
            }

            // Check for sequential symbol string patterns (forward and reverse).
            for (var s = 0; s < PoolSymbols.Length - 3; s++)
            {
                var forward = PoolSymbols.Substring(s, 3);
                var reverse = Reverse(forward);
                if (passwordLower.IndexOf(forward, StringComparison.Ordinal) != -1 ||
                    passwordLower.IndexOf(reverse, StringComparison.Ordinal) != -1)
                {
                    sequenceSymbolsCount++;
                    sequenceCharsCount++;
                }
            }

            // Modify overall score value based on usage vs requirements.

            // General point assignment.
            if (alphasUpperCount > 0 && alphasUpperCount < password.Length)
            {
                score += (password.Length - alphasUpperCount) * 2;
            }
            if (alphasLowerCount > 0 && alphasLowerCount < password.Length)
            {
                score += (password.Length - alphasLowerCount) * 2;
            }
            if (digitsCount > 0 && digitsCount < password.Length)
            {
                score += digitsCount * FactorNumber;
            }
            if (symbolsCount > 0)
            {
                score += symbolsCount * FactorSymbol;
            }
            if (middleCharsCount > 0)
            {
                score += middleCharsCount * FactorMiddleChar;
            }

            // Point deductions for poor practices.

            // Only Letters.
            if ((alphasLowerCount > 0 || alphasUpperCount > 0) && symbolsCount == 0 && digitsCount == 0)
            {
                score = score - password.Length;
                alphasOnlyCount = password.Length;
            }
            // Only Numbers.
            if (alphasLowerCount == 0 && alphasUpperCount == 0 && symbolsCount == 0 && digitsCount > 0)
            {
                score = score - password.Length;
                numbersOnlyCount = password.Length;
            }
            // Same character exists more than once.
            if (repeatCharsCount > 0)
            {
                score = (int)(score - repeatIncrement);
            }
            // Consecutive uppercase letters exist.
            if (consequenceAlphasUpperCount > 0)
            {
                score -= consequenceAlphasUpperCount * FactorConsequenceAlphaUpper;
            }
            // Consecutive lowercase letters exist.
            if (consequenceAlphasLowerCount > 0)
            {
                score -= consequenceAlphasLowerCount * FactorConsequenceAlphaLower;
            }
            // Consecutive numbers exist.
            if (consequenceDigitsCount > 0)
            {
                score -= consequenceDigitsCount * FactorConsequenceNumber;
            }
            // Sequential alpha strings exist (3 characters or more).
            if (sequenceAlphasCount > 0)
            {
                score -= sequenceAlphasCount * FactorSequenceAlpha;
            }
            // Sequential numeric strings exist (3 characters or more).
            if (sequenceNumbersCount > 0)
            {
                score -= sequenceNumbersCount * FactorSequenceNumber;
            }
            // Sequential symbol strings exist (3 characters or more).
            if (sequenceSymbolsCount > 0)
            {
                score -= sequenceSymbolsCount * FactorSequenceSymbol;
            }

            // Determine if mandatory requirements have been met and set image indicators accordingly.
            var arrChars = new[] { password.Length, alphasUpperCount, alphasLowerCount, digitsCount, symbolsCount };
            for (var c = 0; c < arrChars.Length; c++)
            {
                // Password length.
                int minValue = c == 0 ? MinimumPasswordLength - 1 : 0;
                if (arrChars[c] == minValue + 1 || arrChars[c] > minValue + 1)
                {
                    requiredCharsCount++;
                }
            }
            requirements = requiredCharsCount;
            var minRequiredChars = password.Length >= MinimumPasswordLength ? 3 : 4;

            // One or more required characters exist.
            if (requirements > minRequiredChars)
            {
                score += requirements * 2;
            }

            // Determine if additional bonuses need to be applied and set image indicators accordingly.
            additions = new Dictionary<Addition, int>(10)
            {
                [Addition.MiddleNumbersOrSymbols] = middleCharsCount,
                [Addition.Requirements] = requirements,
                [Addition.NumberOfCharacters] = alphasOnlyCount,
                [Addition.Symbols] = symbolsCount,
                [Addition.UppercaseLetters] = alphasUpperCount,
                [Addition.LowercaseLetters] = alphasLowerCount,
                [Addition.MiddleNumbersOrSymbols] = middleCharsCount,
                [Addition.LettersOnly] = alphasOnlyCount,
                [Addition.NumbersOnly] = numbersOnlyCount,
                [Addition.RepeatCharacters] = repeatCharsCount,
                [Addition.ConsecutiveUppercaseLetters] = consequenceAlphasUpperCount,
                [Addition.ConsecutiveLowercaseLetters] = consequenceAlphasLowerCount,
                [Addition.ConsecutiveNumbers] = consequenceDigitsCount,
                [Addition.SequentialLetters] = sequenceAlphasCount,
                [Addition.SequentialNumbers] = sequenceNumbersCount,
                [Addition.SequentialSymbols] = sequenceSymbolsCount
            };

            score = score > 100 ? 100 : score;
            score = score < 0 ? 0 : score;

            return score;
        }

        /// <summary>
        /// Estimate password strength. See documentation for more details.
        /// </summary>
        /// <param name="password">Password to estimate.</param>
        /// <remarks>
        /// The source code has got from http://www.passwordmeter.com .
        /// </remarks>
        /// <returns>Estimated score.</returns>
        public static int EstimatePasswordStrength([NotNull] string password)
        {
            IDictionary<Addition, int> additions;
            return EstimatePasswordStrength(password, out additions);
        }

        /// <summary>
        /// Get password entropy.
        /// </summary>
        /// <returns>Password's entropy.</returns>
        public double GetEntropy()
        {
            var pool = string.IsNullOrEmpty(this.CharactersPool) ? CreateCharactersPool() : this.CharactersPool.ToCharArray();
            return Math.Log(Math.Pow(this.PasswordLength, pool.Length), 2);
        }

        private char[] CreateCharactersPool()
        {
            var chars = new List<char>(65);

            if (CharacterClasses.HasFlag(CharacterClass.UpperLetters))
            {
                chars.AddRange(PoolUpperCase.ToCharArray());
                if (!GeneratorFlags.HasFlag(GeneratorFlag.ExcludeLookAlike))
                {
                    chars.AddRange(PoolUpperCaseConflict.ToCharArray());
                }
            }
            if (this.CharacterClasses.HasFlag(CharacterClass.LowerLetters))
            {
                chars.AddRange(PoolLowerCase.ToCharArray());
                if (!GeneratorFlags.HasFlag(GeneratorFlag.ExcludeLookAlike))
                {
                    chars.AddRange(PoolLowerCaseConflict.ToCharArray());
                }
            }
            if (this.CharacterClasses.HasFlag(CharacterClass.Digits))
            {
                chars.AddRange(PoolDigits.ToCharArray());
                if (!GeneratorFlags.HasFlag(GeneratorFlag.ExcludeLookAlike))
                {
                    chars.AddRange(PoolDigitsConflict.ToCharArray());
                }
            }
            if (this.CharacterClasses.HasFlag(CharacterClass.SpecialCharacters))
            {
                chars.AddRange(PoolSpecial.ToCharArray());
                if (!GeneratorFlags.HasFlag(GeneratorFlag.ExcludeLookAlike))
                {
                    chars.AddRange(PoolSpecialConflict.ToCharArray());
                }
            }
            if (CharacterClasses.HasFlag(CharacterClass.Space))
            {
                chars.AddRange(PoolSpace.ToCharArray());
            }

            return chars.ToArray();
        }

        static void ShuffleCharsArray(char[] chars)
        {
            for (int i = chars.Length - 1; i >= 1; i--)
            {
                int j = GetNextRandom(i + 1);
                var tmp = chars[i];
                chars[i] = chars[j];
                chars[j] = tmp;
            }
        }

        /// <summary>
        /// Get next random number using RandomService.
        /// </summary>
        /// <param name="maxValue">Maximum value for number.</param>
        /// <returns>The random number between zero and maxValue.</returns>
        static int GetNextRandom(int maxValue)
        {
#if NET452
            var bytes = new byte[4];
            lock (randomServiceLock)
            {
                RandomService.GetBytes(bytes);
            }
            return (int)Math.Round(((double)BitConverter.ToUInt32(bytes, 0) / uint.MaxValue) * (maxValue - 1));
#else
            return RandomService.Next(maxValue);
#endif
        }

        static string Reverse(string target)
        {
            return string.Join(string.Empty, target.Reverse());
        }
    }
}
