using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AdventOfCode2020
{
    public sealed class Solutions
    {
        [TestCase("Day1_problem.txt", ExpectedResult = 960075)]
        [TestCase("Day1_test.txt", ExpectedResult = 514579)]
        public int Day1_1(string fileName)
        {
            var input = ReadAllLines(fileName).Select(int.Parse).ToArray();

            for (int i = 0; i < input.Length; i++)
            {
                for (int j = i + 1; j < input.Length; j++)
                    if (input[i] + input[j] == 2020)
                        return input[i] * input[j];
            }

            throw new InvalidOperationException("Solution not found");
        }

        [TestCase("Day1_problem.txt", ExpectedResult = 212900130)]
        [TestCase("Day1_test.txt", ExpectedResult = 241861950)]
        public int Day1_2(string fileName)
        {
            var input = ReadAllLines(fileName).Select(int.Parse).ToArray();

            for (int i = 0; i < input.Length; i++)
            {
                for (int j = i + 1; j < input.Length; j++)
                for (int k = j + 1; k < input.Length; k++)
                    if (input[i] + input[j] + input[k] == 2020)
                        return input[i] * input[j] * input[k];
            }

            throw new InvalidOperationException("Solution not found");
        }

        [TestCase("Day2_problem.txt", ExpectedResult = 550)]
        [TestCase("Day2_test.txt", ExpectedResult = 2)]
        public int Day2_1(string fileName)
        {
            var inputs = ReadAllLines(fileName)
                .Select(s => s.Split(new[] { ' ', '-', ':'}, StringSplitOptions.RemoveEmptyEntries))
                .Select(parts => (from: int.Parse(parts[0]), to: int.Parse(parts[1]), symbol: parts[2].Single(), password: parts[3]))
                .ToArray();

            var correctPasswordsCount = inputs
                .Count(a => a.password.Count(c => c == a.symbol) >= a.from &&
                            a.password.Count(c => c == a.symbol) <= a.to);

            return correctPasswordsCount;
        }

        [TestCase("Day2_problem.txt", ExpectedResult = 634)]
        [TestCase("Day2_test.txt", ExpectedResult = 1)]
        public int Day2_2(string fileName)
        {
            var inputs = ReadAllLines(fileName)
                .Select(s => s.Split(new[] { ' ', '-', ':' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(parts => (pos1: int.Parse(parts[0]), pos2: int.Parse(parts[1]), symbol: parts[2].Single(), password: parts[3]))
                .ToArray();

            var correctPasswordsCount = inputs
                .Where(a => a.password.Length >= a.pos1 && a.password.Length >= a.pos2)
                .Count(a => a.password[a.pos1 - 1] == a.symbol ^ a.password[a.pos2 - 1] == a.symbol);

            return correctPasswordsCount;
        }

        [TestCase("Day3_problem.txt", ExpectedResult = 169)]
        [TestCase("Day3_test.txt", ExpectedResult = 7)]
        public long Day3_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var map = new Day3Map(inputs);

            return map.Trees((x: 3, y: 1));
        }

        [TestCase("Day3_problem.txt", ExpectedResult = 7560370818)]
        [TestCase("Day3_test.txt", ExpectedResult = 336)]
        public long Day3_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var map = new Day3Map(inputs);

            return map.Trees((x: 1, y: 1))
                * map.Trees((x: 3, y: 1))
                * map.Trees((x: 5, y: 1))
                * map.Trees((x: 7, y: 1))
                * map.Trees((x: 1, y: 2));
        }

        [TestCase("Day4_test.txt", ExpectedResult = 2)]
        [TestCase("Day4_problem.txt", ExpectedResult = 192)]
        public long Day4_1(string fileName)
        {
            var inputs = ReadEmptyLineSeparated(fileName);
            var passports = inputs.Select(Day4Passport.Parse).ToArray();

            return passports.Count(p => p.HaveRequiredValues);
        }

        [TestCase("Day4_valid.txt", ExpectedResult = 4)]
        [TestCase("Day4_invalid.txt", ExpectedResult = 0)]
        [TestCase("Day4_test.txt", ExpectedResult = 2)]
        [TestCase("Day4_problem.txt", ExpectedResult = 101)]
        public long Day4_2(string fileName)
        {
            var inputs = ReadEmptyLineSeparated(fileName);
            var passports = inputs.Select(Day4Passport.Parse).ToArray();

            return passports.Count(p => p.HaveValidValues);
        }
        
        [TestCase("Day5_test.txt", ExpectedResult = 357)]
        [TestCase("Day5_problem.txt", ExpectedResult = 801)]
        public long Day5_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            return inputs.Select(Day5Parse).Select(bp => bp.row * 8 + bp.col).Max();
        }

        [TestCase("Day5_problem.txt", ExpectedResult = 597)]
        public long Day5_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);

            var seats2 = inputs
                .Select(Day5Parse)
                .Select(bp => (long)bp.row * 8 + bp.col)
                .Where(s => s > (0 * 8 + 7) && s < (127 * 8 + 0)) // removing first and last row
                .OrderBy(_ => _)
                .ToArray();

            for (int i = 0; i < seats2.Length - 1; i++)
            {
                if (seats2[i + 1] - seats2[i] == 2)
                    return seats2[i] + 1;
            }

            throw new InvalidOperationException("no solution");
        }

        private (int row, int col) Day5Parse(string b)
        {
            var l = 0;
            var r = 128;
            var first = b.Substring(0, 7);
            foreach (var c in first)
            {
                if (c == 'F')
                    r = r - (r - l) / 2;
                if (c == 'B')
                    l = l + (r - l) / 2;
            }
            var row = l;

            l = 0;
            r = 8;
            var second = b.Substring(7, 3);
            foreach (var c in second)
            {
                if (c == 'L')
                    r = r - (r - l) / 2;
                if (c == 'R')
                    l = l + (r - l) / 2;
            }
            var col = l;

            return (row, col);
        }

        private sealed class Day4Passport
        {
            public bool HaveRequiredValues { get; private set; }
            public bool HaveValidValues { get; private set; }

            public static Day4Passport Parse(string line)
            {
                var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var fields = parts.ToDictionary(i => i.Split(":")[0], i => i.Split(":")[1]);

                var result = new Day4Passport
                {
                    HaveRequiredValues = IsHaveRequiredValues(fields),
                    HaveValidValues = IsHaveValidValues(fields)
                };

                return result;
            }

            private static bool IsHaveRequiredValues(IDictionary<string, string> parts)
            {
                if (parts.Count == 8)
                    return true;
                if (parts.Count == 7 && !parts.ContainsKey("cid"))
                    return true;
                return false;
            }

            private static bool IsHaveValidValues(Dictionary<string, string> fields)
            {
                if (!IsHaveRequiredValues(fields))
                    return false;
                var byr = fields["byr"];
                var byrValid = Regex.IsMatch(byr, @"^\d{4}$") && int.TryParse(byr, out var byrValue) &&
                               byrValue >= 1920 && byrValue <= 2002;

                var iyr = fields["iyr"];
                var iyrValid = Regex.IsMatch(iyr, @"^\d{4}$") && int.TryParse(iyr, out var iyrValue) &&
                               iyrValue >= 2010 && iyrValue <= 2020;

                var eyr = fields["eyr"];
                var eyrValid = Regex.IsMatch(eyr, @"^\d{4}$") && int.TryParse(eyr, out var eyrValue) &&
                               eyrValue >= 2020 && eyrValue <= 2030;

                var hgt = fields["hgt"];
                var hgtValid = false;
                var match = Regex.Match(hgt, @"^(\d{2,3})(cm|in)$");
                if (match.Success)
                {
                    var value = int.Parse(match.Groups[1].Value);
                    var isCm = match.Groups[2].Value == "cm";

                    hgtValid = (isCm && value >= 150 && value <= 193) || (!isCm && value >= 59 && value <= 76);
                }

                var hcl = fields["hcl"];
                var hclValid = Regex.IsMatch(hcl, @"^#(\d|[a-z]){6}$");

                var ecl = fields["ecl"];
                var eclValid = "amb blu brn gry grn hzl oth".Split(" ").Any(s => StringComparer.Ordinal.Equals(s, ecl));

                var pid = fields["pid"];
                var pidValid = Regex.IsMatch(pid, @"^\d{9}$");

                return byrValid && iyrValid && eyrValid && hgtValid && hclValid && eclValid && pidValid;
            }
        }

        private sealed class Day3Map
        {
            private readonly string[] _input;
            private readonly int _width;

            public Day3Map(string[] input)
            {
                _input = input;
                _width = input[0].Length;
            }

            private bool IsTree((int x, int y) loc)
            {
                var x = loc.x % _width;
                return _input[loc.y][x] == '#';
            }

            public long Trees((int x, int y) offset)
            {
                var loc = (x: 0, y: 0);
                var trees = 0;
                do
                {
                    if (IsTree(loc))
                        trees++;

                    loc = (loc.x + offset.x, loc.y + offset.y);
                } while (loc.y < _input.Length);
                
                return trees;
            }
        }

        private static string[] ReadAllLines(string fileName)
        {
            var filePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "../../../Input/", fileName);
            return File.ReadAllLines(filePath);
        }

        private static string[] ReadEmptyLineSeparated(string fileName)
        {
            var result = new List<string>();
            var filePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "../../../Input/", fileName);

            using (var reader = new StreamReader(File.OpenRead(filePath)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var current = "";
                    while (!string.IsNullOrWhiteSpace(line))
                    {
                        current += " " + line;
                        line = reader.ReadLine();
                    }
                    result.Add(current);
                }
            }

            return result.ToArray();
        }
    }
}
