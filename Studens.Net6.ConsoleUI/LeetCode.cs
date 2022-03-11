﻿namespace Studens.Net6.ConsoleUI
{
    public static class LeetCode
    {
        public static void RunBracesProblem()
        {
            var testCases = new[]
            {
                "(){",
                "()[]{}",
                "(]",
                "{[(]]}",
                "{[(n]]}",
                "{[()]}",
                "[[[]",
                "()))",
                "}}}}",
                "[[[]"
            };

            static bool IsValid(string s)
            {
                var length = s.Length;

                // Some base validation
                if (length < 2) return false;
                if (length % 2 != 0) return false;

                var brackets = new Dictionary<char, char>
                {
                    { '(', ')' },
                    { '[', ']' },
                    { '{', '}' },
                };

                // validate last and first chars
                if (!brackets.ContainsKey(s[0]) || !brackets.ContainsValue(s[length - 1]))
                {
                    return false;
                }

                var stack = new Stack<char>();
                stack.Push(s[0]);

                for (int i = 1; i < s.Length; i++)
                {
                    // Is it an open bracket ?
                    if (brackets.ContainsKey(s[i]))
                    {
                        stack.Push(s[i]);
                    }
                    else
                    {
                        var hasOpeningBrackets = stack.TryPop(out char lastOpeningBracket);

                        if (!hasOpeningBrackets || brackets[lastOpeningBracket] != s[i])
                        {
                            return false;
                        }
                    }
                }

                return stack.Count == 0;
            }

            foreach (var test in testCases)
            {
                if (IsValid(test))
                {
                    Console.WriteLine($"Test result for '{test}' is valid.");
                }
                else
                {
                    Console.WriteLine($"Test result for '{test}' is not valid.");
                }
            }
        }

        public static void RunPalindromeProblem()
        {
            var testCases = new[]
            {
                "",
                "b",
                "b(_",
                "anna",
                "car",
                "A man, a plan, a canal: Panama",
                "0P"
            };

            static bool IsPalindrome(string s)
            {
                if (string.IsNullOrEmpty(s)) return true;
                if (s.Length == 1) return true;

                var letters = s.Where(charko => Char.IsDigit(charko) || Char.IsLetter(charko))
                    .Select(charko => Char.ToUpper(charko))
                    .ToArray();

                var len = letters.Length - 1;

                for (int i = 0; i <= len; i++)
                {
                    if (letters[i] != letters[len - i])
                    {
                        return false;
                    }
                }

                return true;
            }

            foreach (var test in testCases)
            {
                if (IsPalindrome(test))
                {
                    Console.WriteLine($"'{test}' is a palindrom.");
                }
                else
                {
                    Console.WriteLine($"'{test}' is not a palindrom.");
                }
            }
        }

        public static void RunShuffledArrayProblem()
        {
            var testCases = new List<int[]>
            {
               new[] { 2, 5, 4, 8 }
            };

            static int[] GetShuffledArray(int[] array)
            {
                var random = new Random();
                var len = array.Length - 1;

                for (int i = 0; i <= len; i++)
                {
                    var randomIndex = random.Next(i == len ? i : i + 1, array.Length);

                    var cpy = array[i];
                    array[i] = array[randomIndex];
                    array[randomIndex] = cpy;
                }

                return array;
            }

            foreach (var test in testCases)
            {
                Console.WriteLine($"Shuffled array for [{string.Join(", ", test)}] is  [{string.Join(", ", GetShuffledArray(test))}]");
            }
        }

        public static void RunLengthOfLastWordProblem()
        {
            var testCases = new List<string>
            {
               "Hello world",
               "Hello a  sd   w sd world      ",
            };           

            static int lengthOfLastWord(string s)
            {
                s = s.TrimEnd();               
                var lastSpaceIndex = s.LastIndexOf(' ');
                return s.Substring(lastSpaceIndex + 1).Length;                
            }

            foreach (var test in testCases)
            {
                Console.WriteLine($"Length of the last word is {lengthOfLastWord(test)} characters.");
            }
        }        
    }
}