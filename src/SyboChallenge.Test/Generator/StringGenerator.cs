using System;
using System.Collections.Generic;

namespace SyboChallenge.Test.Generator
{
    public static class StringGenerator
    {
        public static string Unique() => Guid.NewGuid().ToString("N");

        public static string Random(int length)
        {
            var chars = "abcdefghijklmnopqrstuvwxyz";
            var stringChars = new char[length];
            var random = new Random(Guid.NewGuid().GetHashCode());

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }

        public static IList<string> RandomMany(int count)
        {
            var words = new List<string>();
            var random = new Random(Guid.NewGuid().GetHashCode());

            for (var i = 0; i < count; ++i)
            {
                words.Add(Random(random.Next(50, 100)));
            }

            return words;
        }
    }
}
