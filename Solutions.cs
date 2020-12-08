﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
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
                .Select(s => s.Split(new[] { ' ', '-', ':' }, StringSplitOptions.RemoveEmptyEntries))
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
            return inputs.Select(Day5Parse).Max();
        }

        [TestCase("Day5_problem.txt", ExpectedResult = 597)]
        public long Day5_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);

            var seats2 = inputs
                .Select(Day5Parse)
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

        [TestCase("Day6_test.txt", ExpectedResult = 11)]
        [TestCase("Day6_problem.txt", ExpectedResult = 6297)]
        public long Day6_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var groups = new List<string>();

            var buffer = new StringBuilder();
            foreach (var input in inputs)
            {
                if (string.IsNullOrEmpty(input))
                {
                    groups.Add(buffer.ToString());
                    buffer.Length = 0;
                }
                else
                    buffer.Append(input);
            }
            if (buffer.Length > 0)
                groups.Add(buffer.ToString());

            return groups.Sum(g => g.Distinct().Count());
        }

        [TestCase("Day6_test.txt", ExpectedResult = 6)]
        [TestCase("Day6_problem.txt", ExpectedResult = 3158)]
        public long Day6_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var groups = new List<string>();

            var buffer = new StringBuilder();
            foreach (var input in inputs)
            {
                if (string.IsNullOrEmpty(input))
                {
                    groups.Add(buffer.ToString());
                    buffer.Length = 0;
                }
                else
                    buffer.AppendLine(input);
            }
            if (buffer.Length > 0)
                groups.Add(buffer.ToString());

            var sum = 0;
            foreach (var g in groups)
            {
                var declaration = g.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
                var all = g.Distinct().Where(c => c != '\n' && c != '\r').ToArray();
                sum += all.Count(c => declaration.All(s => s.Contains(c)));
            }

            return sum;
        }

        [TestCase("Day7_test.txt", ExpectedResult = 4)]
        [TestCase("Day7_problem.txt", ExpectedResult = 300)]
        public long Day7_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var graph = new Day7Graph(inputs);

            return graph.Search("shiny gold");
        }

        [TestCase("Day7_test.txt", ExpectedResult = 32)]
        [TestCase("Day7_test2.txt", ExpectedResult = 126)]
        [TestCase("Day7_problem.txt", ExpectedResult = 8030)]
        public long Day7_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var graph = new Day7Graph(inputs);

            return graph.Search2("shiny gold");
        }
        
        [TestCase("Day8_test.txt", ExpectedResult = 5)]
        [TestCase("Day8_problem.txt", ExpectedResult = 1475)]
        public long Day8_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);

            if (Day8_InterpretCode(inputs, out var accumulator))
                return accumulator;

            throw new InvalidOperationException("No infinite loop detected");
        }

        [TestCase("Day8_test.txt", ExpectedResult = 8)]
        [TestCase("Day8_problem.txt", ExpectedResult = 1270)]
        public long Day8_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);

            var ind = 0;
            long accumulator;

            string[] newInput = null;
            do
            {
                newInput = inputs.ToArray();
                var line = newInput[ind];

                if (line.StartsWith("jmp"))
                {
                    newInput[ind] = "nop" + line.Substring(3);
                }
                else if (line.StartsWith("nop"))
                {
                    newInput[ind] = "jmp" + line.Substring(3);
                }

                ind++;
            } while (Day8_InterpretCode(newInput, out accumulator));

            return accumulator;
        }

        private static bool Day8_InterpretCode(string[] inputs, out long accumulator)
        {
            int offset = 0;
            int acc = 0;
            var visited = new HashSet<int>();

            do
            {
                visited.Add(offset);
                var line = inputs[offset];

                var parts = line.Split(' ');
                var arg = int.Parse(parts[1]);
                switch (parts[0])
                {
                    case "jmp":
                        offset += arg;
                        break;
                    case "acc":
                        acc += arg;
                        offset++;
                        break;
                    case "nop":
                        offset++;
                        break;
                }
            } while (!visited.Contains(offset) && offset < inputs.Length);

            accumulator = acc;
            return visited.Contains(offset);
        }

        private sealed class Day7Graph
        {
            private int _count;
            private readonly Dictionary<string, int> _indexes = new Dictionary<string, int>();
            private readonly int[,] _cost = new int[1000, 1000];

            public Day7Graph(string[] inputs)
            {
                foreach (var input in inputs)
                {
                    var (from, to) = ParseLine(input);
                    var (fromIndex, toIndexes) = ConvertToIndexes((from, to));

                    foreach (var index in toIndexes)
                    {
                        _cost[fromIndex, index.index] = index.count;
                    }
                }
            }

            public int Search2(string from)
            {
                var start = _indexes[from];
                var q = new Queue<(int index, int count)>();
                q.Enqueue((start, 1));

                int sum = -1;
                // DFS algo
                while (q.Count > 0)
                {
                    var item = q.Dequeue();
                    sum += item.count;

                    for (int i = 0; i < _count; i++)
                    {
                        if (i != item.index && _cost[item.index, i] > 0)
                            q.Enqueue((i, item.count * _cost[item.index, i]));
                    }
                }

                return sum;
            }

            public int Search(string from)
            {
                var start = _indexes[from];
                var q = new Queue<int>();
                q.Enqueue(start);

                var visited = new bool[_count];
                // DFS algo
                while (q.Count > 0)
                {
                    var item = q.Dequeue();
                    visited[item] = true;

                    for (int i = 0; i < _count; i++)
                    {
                        if (!visited[i] && _cost[i, item] > 0)
                            q.Enqueue(i);
                    }
                }

                return visited.Count(_ => _) - 1;
            }

            private (int from, (int index, int count)[] to) ConvertToIndexes((string @from, string[] to) value)
            {
                var regex = new Regex(@"^((?<count>\d+) )?(?<name>\w+ \w+)$");

                int GetOrAdd(string v)
                {
                    var match = regex.Match(v);
                    var name = match.Groups["name"].Value;
                    if (_indexes.ContainsKey(name))
                        return _indexes[name];
                    _indexes[name] = _count;
                    return _count++;
                }

                int GetCount(string v)
                {
                    var match = regex.Match(v);
                    var countString = match.Groups.ContainsKey("count")
                        ? match.Groups["count"].Value
                        : "1";
                    return int.Parse(countString);
                }

                return (GetOrAdd(value.from), value.to.Select(s => (index: GetOrAdd(s), count: GetCount(s))).ToArray());
            }

            private static (string from, string[] to) ParseLine(string l)
            {
                var regex = new Regex(@"^(?<from>[\w]+ [\w]+) bags? contains?(( no other bags\.)|( (?<to>\d+ \w+ \w+) bags?[,.]?)+)$");
                var match = regex.Match(l);

                var from = match.Groups["from"].Value;
                var to = match.Groups.ContainsKey("to")
                    ? match.Groups["to"].Captures.Select(c => c.Value).ToArray()
                    : Array.Empty<string>();

                return (from, to);
            }
        }

        private int Day5Parse(string b)
        {
            var binaryString = b
                .Replace('F', '0')
                .Replace('B', '1')
                .Replace('L', '0')
                .Replace('R', '1');

            return Convert.ToInt32(binaryString, 2);
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
