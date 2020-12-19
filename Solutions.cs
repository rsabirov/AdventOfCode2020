using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;
using NUnit.Framework;

namespace AdventOfCode2020
{
    public sealed class Solutions
    {
        [TestCase("Day01_problem.txt", ExpectedResult = 960075)]
        [TestCase("Day01_test.txt", ExpectedResult = 514579)]
        public int Day01_1(string fileName)
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

        [TestCase("Day01_problem.txt", ExpectedResult = 212900130)]
        [TestCase("Day01_test.txt", ExpectedResult = 241861950)]
        public int Day01_2(string fileName)
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

        [TestCase("Day02_problem.txt", ExpectedResult = 550)]
        [TestCase("Day02_test.txt", ExpectedResult = 2)]
        public int Day02_1(string fileName)
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

        [TestCase("Day02_problem.txt", ExpectedResult = 634)]
        [TestCase("Day02_test.txt", ExpectedResult = 1)]
        public int Day02_2(string fileName)
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

        [TestCase("Day03_problem.txt", ExpectedResult = 169)]
        [TestCase("Day03_test.txt", ExpectedResult = 7)]
        public long Day03_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var map = new Day03Map(inputs);

            return map.Trees((x: 3, y: 1));
        }

        [TestCase("Day03_problem.txt", ExpectedResult = 7560370818)]
        [TestCase("Day03_test.txt", ExpectedResult = 336)]
        public long Day03_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var map = new Day03Map(inputs);

            return map.Trees((x: 1, y: 1))
                * map.Trees((x: 3, y: 1))
                * map.Trees((x: 5, y: 1))
                * map.Trees((x: 7, y: 1))
                * map.Trees((x: 1, y: 2));
        }

        [TestCase("Day04_test.txt", ExpectedResult = 2)]
        [TestCase("Day04_problem.txt", ExpectedResult = 192)]
        public long Day04_1(string fileName)
        {
            var inputs = ReadEmptyLineSeparated(fileName);
            var passports = inputs.Select(Day04Passport.Parse).ToArray();

            return passports.Count(p => p.HaveRequiredValues);
        }

        [TestCase("Day04_valid.txt", ExpectedResult = 4)]
        [TestCase("Day04_invalid.txt", ExpectedResult = 0)]
        [TestCase("Day04_test.txt", ExpectedResult = 2)]
        [TestCase("Day04_problem.txt", ExpectedResult = 101)]
        public long Day04_2(string fileName)
        {
            var inputs = ReadEmptyLineSeparated(fileName);
            var passports = inputs.Select(Day04Passport.Parse).ToArray();

            return passports.Count(p => p.HaveValidValues);
        }

        [TestCase("Day05_test.txt", ExpectedResult = 357)]
        [TestCase("Day05_problem.txt", ExpectedResult = 801)]
        public long Day05_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            return inputs.Select(Day05Parse).Max();
        }

        [TestCase("Day05_problem.txt", ExpectedResult = 597)]
        public long Day05_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);

            var seats2 = inputs
                .Select(Day05Parse)
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

        [TestCase("Day06_test.txt", ExpectedResult = 11)]
        [TestCase("Day06_problem.txt", ExpectedResult = 6297)]
        public long Day06_1(string fileName)
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

        [TestCase("Day06_test.txt", ExpectedResult = 6)]
        [TestCase("Day06_problem.txt", ExpectedResult = 3158)]
        public long Day06_2(string fileName)
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

        [TestCase("Day07_test.txt", ExpectedResult = 4)]
        [TestCase("Day07_problem.txt", ExpectedResult = 300)]
        public long Day07_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var graph = new Day07Graph(inputs);

            return graph.Search("shiny gold");
        }

        [TestCase("Day07_test.txt", ExpectedResult = 32)]
        [TestCase("Day07_test2.txt", ExpectedResult = 126)]
        [TestCase("Day07_problem.txt", ExpectedResult = 8030)]
        public long Day07_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var graph = new Day07Graph(inputs);

            return graph.Search2("shiny gold");
        }

        [TestCase("Day08_test.txt", ExpectedResult = 5)]
        [TestCase("Day08_problem.txt", ExpectedResult = 1475)]
        public long Day08_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);

            if (Day08_InterpretCode(inputs, out var accumulator))
                return accumulator;

            throw new InvalidOperationException("No infinite loop detected");
        }

        [TestCase("Day08_test.txt", ExpectedResult = 8)]
        [TestCase("Day08_problem.txt", ExpectedResult = 1270)]
        public long Day08_2(string fileName)
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
            } while (Day08_InterpretCode(newInput, out accumulator));

            return accumulator;
        }

        [TestCase("Day09_test.txt", 5, ExpectedResult = 127)]
        [TestCase("Day09_problem.txt", 25, ExpectedResult = 3199139634)]
        public long Day09_1(string fileName, int preambleSize)
        {
            var inputs = ReadAllLines(fileName);
            var a = inputs.Select(long.Parse).ToArray();

            return Day09_FindInvalidNumber(preambleSize, a);
        }

        [TestCase("Day09_test.txt", 5, ExpectedResult = 62)]
        [TestCase("Day09_problem.txt", 25, ExpectedResult = 438559930)]
        public long Day09_2(string fileName, int preambleSize)
        {
            var inputs = ReadAllLines(fileName);
            var a = inputs.Select(long.Parse).ToArray();

            var invNumber = Day09_FindInvalidNumber(preambleSize, a);
            for (int size = 2; size < a.Length; size++)
            {
                for (int offset = 0; offset < a.Length - size; offset++)
                {
                    var subset = a.Skip(offset).Take(size);
                    if (subset.Sum() == invNumber)
                    {
                        return subset.Max() + subset.Min();
                    }
                }
            }
            throw new InvalidOperationException("no solution");
        }

        [TestCase("Day10_test.txt", ExpectedResult = 35)]
        [TestCase("Day10_test2.txt", ExpectedResult = 220)]
        [TestCase("Day10_problem.txt", ExpectedResult = 2812)]
        public long Day10_1(string fileName)
        {
            var inputs = ReadAllLines(fileName)
                .Select(int.Parse)
                .Concat(new[] { 0 })
                .OrderBy(_ => _)
                .ToArray();

            var diff1 = 0;
            var diff3 = 1;
            var prev = inputs[0];
            for (int i = 1; i < inputs.Length; i++)
            {
                var diff = inputs[i] - prev;
                if (diff == 1)
                    diff1++;
                else if (diff == 3)
                    diff3++;
                else
                    throw new InvalidOperationException($"Unexpected diff = {diff}");

                prev = inputs[i];
            }

            return diff1 * diff3;
        }

        [TestCase("Day10_test.txt", ExpectedResult = 8)]
        [TestCase("Day10_test2.txt", ExpectedResult = 19208)]
        [TestCase("Day10_problem.txt", ExpectedResult = 386869246296064)]
        public long Day10_2(string fileName)
        {
            var inputs = ReadAllLines(fileName)
                .Select(int.Parse)
                .OrderBy(_ => _)
                .ToList();
            inputs.Add(inputs.Max() + 3);

            var ans = new long[inputs.Max() + 1];
            ans[0] = 1;
            foreach (var val in inputs)
            {
                var curr = ans[val - 1];
                if (val >= 2)
                    curr += ans[val - 2];
                if (val >= 3)
                    curr += ans[val - 3];

                ans[val] = curr;
            }

            return ans[ans.Length - 1];
        }

        [TestCase("Day11_test.txt", ExpectedResult = 37)]
        [TestCase("Day11_problem.txt", ExpectedResult = 2338)]
        public long Day11_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var map = new Day11Map(inputs);

            Console.WriteLine(map.Dump());

            while (map.SimulateOneStep(occupiedFunc: Day11Map.OccupiedSeats, minOccupiedSeats: 4))
            {
                Console.WriteLine(map.Dump());
            }
            Console.WriteLine(map.Dump());

            return map.OccupiedSeats();
        }

        [TestCase("Day11_test.txt", ExpectedResult = 26)]
        [TestCase("Day11_problem.txt", ExpectedResult = 2134)]
        public long Day11_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var map = new Day11Map(inputs);

            Console.WriteLine(map.Dump());
            while (map.SimulateOneStep(occupiedFunc: Day11Map.OccupiedSeats2, minOccupiedSeats: 5))
            {
                Console.WriteLine(map.Dump());
            }

            Console.WriteLine(map.Dump());

            return map.OccupiedSeats();
        }

        [TestCase("Day12_test.txt", ExpectedResult = 25)]
        [TestCase("Day12_problem.txt", ExpectedResult = 590)]
        public long Day12_1(string fileName)
        {
            var dirs = new[]
            {
                (x: +1, y: +0),
                (x: +0, y: +1),
                (x: -1, y: +0),
                (x: +0, y: -1)
            };

            var inputs = ReadAllLines(fileName);
            var pos = (x: 0, y: 0);
            var dir = 0;

            foreach (var input in inputs)
            {
                var c = input[0];
                var p = int.Parse(input.Substring(1));

                switch (c)
                {
                    case 'N':
                        pos.y -= p;
                        break;
                    case 'S':
                        pos.y += p;
                        break;
                    case 'E':
                        pos.x += p;
                        break;
                    case 'W':
                        pos.x -= p;
                        break;
                    case 'L':
                        p = p % 360;
                        p = p / 90;
                        p = p % 4;
                        dir = (4 + (dir - p) % 4) % 4;
                        break;
                    case 'R':
                        p = p % 360;
                        p = p / 90;
                        dir = (dir + p) % 4;
                        break;
                    case 'F':
                        pos = (x: pos.x + dirs[dir].x * p, y: pos.y + dirs[dir].y * p);
                        break;
                }
            }

            return Math.Abs(pos.x) + Math.Abs(pos.y);
        }

        [TestCase("Day12_test.txt", ExpectedResult = 286)]
        [TestCase("Day12_problem.txt", ExpectedResult = 42013)]
        public long Day12_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var pos = (x: 0L, y: 0L);
            var wp = (x: +10L, y: -1L);

            foreach (var input in inputs)
            {
                var c = input[0];
                var p = int.Parse(input.Substring(1));

                switch (c)
                {
                    case 'N':
                        wp.y -= p;
                        break;
                    case 'S':
                        wp.y += p;
                        break;
                    case 'E':
                        wp.x += p;
                        break;
                    case 'W':
                        wp.x -= p;
                        break;
                    case 'L':
                        wp = Rotate(wp, p);
                        break;
                    case 'R':
                        wp = Rotate(wp, -p);
                        break;
                    case 'F':
                        pos = (x: pos.x + p * wp.x, y: pos.y + p * wp.y);
                        break;
                }
            }

            return Math.Abs(pos.x) + Math.Abs(pos.y);


            (long x, long y) Rotate((long x, long y) c, int p)
            {
                var angle = p * Math.PI / 180;
                return (
                    x: (int)Math.Round(c.x * Math.Cos(angle) + c.y * Math.Sin(angle)),
                    y: (int)Math.Round(c.y * Math.Cos(angle) - c.x * Math.Sin(angle)));
            }
        }

        [TestCase("Day13_test.txt", ExpectedResult = 295)]
        [TestCase("Day13_problem.txt", ExpectedResult = 261)]
        public long Day13_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var time = long.Parse(inputs[0]);
            var times = inputs[1]
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Where(i => i != "x")
                .Select(long.Parse)
                .OrderBy(_ => _)
                .ToArray();

            var ordered = times
                .Select(t => new { t = t, left = t - time % t })
                .OrderBy(i => i.left)
                .ToArray();
            var earliest = ordered.First();

            return earliest.left * earliest.t;
        }

        [TestCase("Day13_test.txt", ExpectedResult = 1068781)]
        [TestCase("Day13_problem.txt", ExpectedResult = 807435693182510)]
        public long Day13_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var times = inputs[1]
                .Split(',')
                .Select((item, i) => new { item, i })
                .Where(x => x.item != "x")
                .Select(x => new { bus = int.Parse(x.item), index = x.i })
                .ToArray();

            var r = times.Select(t => t.bus).ToArray();             // base 
            var a = times.Select(t => t.bus - t.index).ToArray();   // presentation

            return (long)Solve(r, a);

            // Chinese remainder theorem solution
            BigInteger Solve(int[] n, int[] a)
            {
                BigInteger prod = n
                    .Select(i => new BigInteger(i))
                    .Aggregate(BigInteger.One, (i, j) => i * j);

                BigInteger sum = 0;
                for (int i = 0; i < n.Length; i++)
                {
                    var p = prod / n[i];
                    sum += a[i] * ModularInverse(p, n[i]) * p;
                }
                return sum % prod;
            }

            int ModularInverse(BigInteger a, long mod)
            {
                var b = a % mod;
                for (int x = 1; x < mod; x++)
                {
                    if (b * x % mod == 1)
                        return x;
                }
                return 1;
            }
        }

        [TestCase("Day14_test.txt", ExpectedResult = 165)]
        [TestCase("Day14_problem.txt", ExpectedResult = 5875750429995)]
        public ulong Day14_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var mem = new Day14Memory();
            foreach (var line in inputs)
            {
                var parts = line
                    .Split(new[] { '=', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToArray();

                if (parts[0] == "mask")
                    mem.SetMask(parts[1]);
                else
                {
                    var index = ulong.Parse(Regex.Match(parts[0], "\\d+").Value);
                    var value = ulong.Parse(parts[1]);
                    mem.SetValue(index, value);
                }
            }

            return mem.GetSum();
        }

        [TestCase("Day14_test2.txt", ExpectedResult = 208)]
        [TestCase("Day14_problem.txt", ExpectedResult = 5272149590143)]
        public ulong Day14_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var mem = new Day14Memory2();
            foreach (var line in inputs)
            {
                var parts = line
                    .Split(new[] { '=', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToArray();

                if (parts[0] == "mask")
                    mem.SetMask(parts[1]);
                else
                {
                    var index = ulong.Parse(Regex.Match(parts[0], "\\d+").Value);
                    var value = ulong.Parse(parts[1]);
                    mem.SetValue(index, value);
                }
            }

            return mem.GetSum();
        }

        [TestCase("Day15_test.txt", 2020, ExpectedResult = 436)]
        [TestCase("Day15_problem.txt", 2020, ExpectedResult = 240)]
        [TestCase("Day15_test.txt", 30_000_000, ExpectedResult = 175594)]
        [TestCase("Day15_problem.txt", 30_000_000, ExpectedResult = 505)]
        public int Day15_1_2(string fileName, int turns)
        {
            var inputs = ReadAllLines(fileName);
            var numbers = inputs[0].Split(',').Select(int.Parse).ToArray();

            var spokenAtTurn = new Dictionary<int, (int recentTurn, int turnBeforeRecent)>();
            var lastSpoken = 0;
            for (int turn = 1; turn <= turns; turn++)
            {
                var spokenNumber = 0;
                if (turn <= numbers.Length)
                {
                    spokenNumber = numbers[turn - 1];
                    spokenAtTurn[spokenNumber] = (turn, -1);
                }
                else
                {
                    if (spokenAtTurn.TryGetValue(lastSpoken, out var lastSpokenInfo))
                    {
                        if (lastSpokenInfo.turnBeforeRecent == -1)
                        {
                            spokenNumber = turn - lastSpokenInfo.recentTurn - 1;
                            if (spokenAtTurn.TryGetValue(spokenNumber, out var infoToUpdate))
                                spokenAtTurn[spokenNumber] = (turn, infoToUpdate.recentTurn);
                            else
                                spokenAtTurn[spokenNumber] = (turn, -1);
                        }
                        else
                        {
                            spokenNumber = lastSpokenInfo.recentTurn - lastSpokenInfo.turnBeforeRecent;
                            if (spokenAtTurn.TryGetValue(spokenNumber, out var infoToUpdate))
                                spokenAtTurn[spokenNumber] = (turn, infoToUpdate.recentTurn);
                            else
                                spokenAtTurn[spokenNumber] = (turn, -1);
                        }
                    }
                    else
                    {
                        spokenAtTurn[lastSpoken] = (turn - 1, 0);
                        spokenNumber = 0;
                    }
                }

                lastSpoken = spokenNumber;
            }

            return lastSpoken;
        }

        [TestCase("Day16_test.txt", ExpectedResult = 71)]
        [TestCase("Day16_problem.txt", ExpectedResult = 25916)]
        public int Day16_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);

            var day16 = new Day16(inputs);
            return day16.CalcErrorRate();
        }

        [TestCase("Day16_test.txt", "class", ExpectedResult = 1)]
        [TestCase("Day16_problem.txt", "departure", ExpectedResult = 2564529489989)]
        public long Day16_2(string fileName, string field)
        {
            var inputs = ReadAllLines(fileName);

            var day16 = new Day16(inputs);
            return day16.GetYourTicketField(field);
        }
















        [TestCase("Day17_test.txt", ExpectedResult = 112)]
        [TestCase("Day17_problem.txt", ExpectedResult = 313)]
        public int Day17_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var cubesMap = new Day17CubeMap(inputs);

            for (int i = 0; i < 6; i++)
                cubesMap.Next(dimension: 3);

            return cubesMap.Active;
        }

        [TestCase("Day17_test.txt", ExpectedResult = 848)]
        [TestCase("Day17_problem.txt", ExpectedResult = 2640)]
        public int Day17_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var cubesMap = new Day17CubeMap(inputs);

            for (int i = 0; i < 6; i++)
                cubesMap.Next(dimension: 4);

            return cubesMap.Active;
        }

        [TestCase("Day18_test.txt", ExpectedResult = "13754")]
        [TestCase("Day18_problem.txt", ExpectedResult = "31142189909908")]
        public string Day18_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            return inputs.Select(s => Day18Evaluator.Evaluate(s, ignorePriorities: true))
                .Aggregate((a, b) => a + b)
                .ToString();
        }

        [TestCase("Day18_test.txt", ExpectedResult = "23622")]
        [TestCase("Day18_problem.txt", ExpectedResult = "323912478287549")]
        public string Day18_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            return inputs.Select(s => Day18Evaluator.Evaluate(s, false))
                .Aggregate((a, b) => a + b)
                .ToString();
        }
        
        [TestCase("Day19_test.txt", ExpectedResult = 2)]
        [TestCase("Day19_problem.txt", ExpectedResult = 190)]
        public int Day19_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);

            var rules = inputs.TakeWhile(s => s != string.Empty).ToArray();
            var processor = new Day19RulesProcessor(rules);

            return inputs.Skip(rules.Length + 1).Count(message => processor.IsValidMessage(message));
        }

        [TestCase("Day19_test2.txt", ExpectedResult = 12)]
        [TestCase("Day19_problem2.txt", ExpectedResult = 311)]
        public int Day19_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);

            var rules = inputs.TakeWhile(s => s != string.Empty).ToArray();
            var processor = new Day19RulesProcessor(rules);

            return inputs.Skip(rules.Length + 1).Count(message => processor.IsValidMessage(message));
        }

        private sealed class Day19RulesProcessor
        {
            private readonly IDictionary<int, Rule> _rules = new Dictionary<int, Rule>();

            public Day19RulesProcessor(string[] rawRules)
            {
                var rules = rawRules.Select(rr => new Rule(rr)).ToList();
                // move constant rules to _rules
                foreach (var rule in rules.Where(r => r.IsConstant).ToArray())
                {
                    rules.Remove(rule);
                    _rules.Add(rule.Number, rule);
                }

                // rules derivation
                var anyMatch = true;
                while (!_rules.ContainsKey(0) && anyMatch)
                {
                    anyMatch = false;
                    foreach (var rule in rules.ToArray())
                    {
                        rule.Apply(_rules);
                        if (rule.IsConstant)
                        {
                            rules.Remove(rule);
                            _rules.Add(rule.Number, rule);
                            anyMatch = true;
                        }
                    }
                }
            }

            public bool IsValidMessage(string message)
            {
                return _rules[0].IsMatch(message);
            }

            private sealed class Rule
            {
                private static readonly Regex IsConstantRegex = new Regex(@"^[a-z\s()|]+$", RegexOptions.Compiled);
                private static readonly Regex IndexRegex = new Regex(@"\d+(?!})", RegexOptions.Compiled);
                // special cases
                private static readonly Regex SingleIndex = new Regex(@"^(?<beforeIndex>[^\d]+)(?<index>\d+)(?<afterIndex>[^\d]+)?$");

                private string _value;

                public int Number { get; }
                public bool IsConstant { get; private set; }

                public Rule(string rawRule)
                {
                    var parts = rawRule.Split(':');
                    var val = parts[1].Replace("\"", "");

                    _value = val.Trim();
                    Number = int.Parse(parts[0]);
                    IsConstant = IsConstantRegex.IsMatch(val);
                }

                public void Apply(IDictionary<int, Rule> rules)
                {
                    var singleIndexMatch = SingleIndex.Match(_value);
                    if (singleIndexMatch.Success && int.Parse(singleIndexMatch.Groups["index"].Value) == Number)
                    {
                        // if self reference is end of the string, then "1: r 1" => "1: r+"
                        if (!singleIndexMatch.Groups["afterIndex"].Success)
                        {
                            _value = $"({singleIndexMatch.Groups["beforeIndex"].Value})+";
                            IsConstant = true;
                            return;
                        }

                        var before = singleIndexMatch.Groups["beforeIndex"].Value;
                        var after = singleIndexMatch.Groups["afterIndex"].Value;


                        // for some reason "Balancing Groups" didn't work
                        // _value = $"?<open>{before})+(?<close-open>{after})+(?(open)(?!)";
                        var parts = Enumerable.Range(1, 5)
                            .Select(i => $"{before}" + "{" + i + "}" + $"{after}" + "{" + i + "}");
                        _value = $"({string.Join("|", parts)})";
                        IsConstant = true;
                        return;
                    }

                    _value = IndexRegex.Replace(_value, match =>
                    {
                        var index = int.Parse(match.Value);
                        return rules.TryGetValue(index, out var rule)
                            ? rule._value.Contains("|") ? $"({rule._value})" : rule._value
                            : match.Value;
                    });

                    IsConstant = !IndexRegex.IsMatch(_value);
                }

                public bool IsMatch(string message)
                {
                    if (!IsConstant)
                        throw new InvalidOperationException("Rule is not constant");

                    var regex = new Regex($"^{_value}$", RegexOptions.IgnorePatternWhitespace);
                    return regex.IsMatch(message);
                }
            }
        }

        private sealed class Day18Evaluator
        {
            private enum TokenType
            {
                Number,
                Operator,
                OpenBrackets,
                CloseBrackets
            }

            public static BigInteger Evaluate(string s, bool ignorePriorities)
            {
                var tokens = Tokenize(s);

                var outQueue = new Queue<Token>();
                var opStack = new Stack<Token>();
                // https://en.wikipedia.org/wiki/Shunting-yard_algorithm
                foreach (var token in tokens)
                {
                    switch (token)
                    {
                        case Token tok when tok.TokenType == TokenType.Number:
                            outQueue.Enqueue(tok);
                            break;

                        case Token tok when tok.TokenType == TokenType.Operator:
                            opStack.TryPeek(out var tok1);
                            while (tok1 != null && tok1.TokenType == TokenType.Operator
                                                && (ignorePriorities ||tok1.Priority > token.Priority))
                            {
                                outQueue.Enqueue(opStack.Pop());
                                opStack.TryPeek(out tok1);
                            }
                            opStack.Push(token);
                            break;

                        case Token tok when tok.TokenType == TokenType.OpenBrackets:
                            opStack.Push(token);
                            break;

                        case Token tok when tok.TokenType == TokenType.CloseBrackets:
                            opStack.TryPop(out var tok2);
                            while (tok2 != null && tok2.TokenType != TokenType.OpenBrackets)
                            {
                                outQueue.Enqueue(tok2);
                                opStack.TryPop(out tok2);
                            }
                            break;

                        default:
                            throw new InvalidOperationException();
                    }
                }
                while (opStack.Count > 0)
                    outQueue.Enqueue(opStack.Pop());

                // calc
                var operands = new Stack<BigInteger>();
                while (outQueue.Count > 0)
                {
                    var a = outQueue.Dequeue();
                    if (a.TokenType == TokenType.Number)
                        operands.Push(a.Num);
                    else
                    {
                        var a1 = operands.Pop();
                        var a2 = operands.Pop();
                        if (a.Op == "+")
                            operands.Push(a1 + a2);
                        else
                            operands.Push(a1 * a2);
                    }
                }

                return operands.Pop();
            }

            private static Token[] Tokenize(string s)
            {
                var regex = new Regex(@"\d+|\+|\*|\(|\)");
                var tokens = regex.Matches(s.Replace(" ", ""))
                    .Select(m => m.Value)
                    .Select(i =>
                    {
                        switch (i)
                        {
                            case var ss when int.TryParse(ss, out var num):
                                return Token.Number(num);
                            case "+":
                            case "*":
                                return Token.Operator(i);
                            case "(":
                                return Token.OpenBrackets();
                            case ")":
                                return Token.CloseBrackets();
                            default:
                                throw new InvalidOperationException($"Not supported '{i}'");
                        }
                    })
                    .ToArray();
                return tokens;
            }

            private sealed class Token
            {
                public TokenType TokenType { get; }
                public int Num { get; }
                public string Op { get; }
                public int Priority { get; }

                public Token(int num)
                {
                    TokenType = TokenType.Number;
                    Num = num;
                }

                private Token(string op)
                {
                    TokenType = TokenType.Operator;
                    Priority = op == "+" ? 1 : 0;
                    Op = op;
                }

                private Token(TokenType tokenType)
                {
                    TokenType = tokenType;
                }

                public static Token Number(int v)
                {
                    return new Token(v);
                }

                public static Token Operator(string op)
                {
                    return new Token(op);
                }

                public static Token OpenBrackets()
                {
                    return new Token(TokenType.OpenBrackets);
                }

                public static Token CloseBrackets()
                {
                    return new Token(TokenType.CloseBrackets);
                }

                public override string ToString()
                {
                    switch (TokenType)
                    {
                        case TokenType.Number:
                            return Num.ToString();
                        case TokenType.Operator:
                            return Op;
                        case TokenType.OpenBrackets:
                            return "(";
                        case TokenType.CloseBrackets:
                            return ")";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private sealed class Day17CubeMap
        {
            private const int Max = 30;
            private const int Mid = 15;
            private bool[,,,] _cube = new bool[Max, Max, Max, Max];

            public Day17CubeMap(string[] inputs)
            {
                var map = inputs.Select(l => l.Select(c => c == '#').ToArray()).ToArray();
                for (int i = 0; i < map.GetLength(0); i++)
                    for (int j = 0; j < map[i].Length; j++)
                    {
                        _cube[Mid + i, Mid + j, Mid, Mid] = map[i][j];
                        Active += map[i][j] ? 1 : 0;
                    }

                // creating diff4d
                Diffs4d = Diffs3d
                    .Concat(
                        Diffs3d.Select(i => (i.x, i.y, i.z, v: +1)).Concat(new[] { (x: 0, y: 0, z: 0, v: +1) })
                    )
                    .Concat(
                        Diffs3d.Select(i => (i.x, i.y, i.z, v: -1)).Concat(new[] { (x: 0, y: 0, z: 0, v: -1) })
                    )
                    .ToArray();
            }

            public int Active { get; private set; }

            public void Next(int dimension)
            {
                Active = 0;
                var newCube = (bool[,,,])_cube.Clone();

                for (int x = 1; x < Max - 1; x++)
                    for (int y = 1; y < Max - 1; y++)
                        for (int z = 1; z < Max - 1; z++)
                            for (int v = 1; v < Max - 1; v++)
                            {
                                var n = Neighbors(_cube, x, y, z, v, dimension);
                                newCube[x, y, z, v] = _cube[x, y, z, v] && (n == 2 || n == 3) || !_cube[x, y, z, v] && n == 3;
                                Active += newCube[x, y, z, v] ? 1 : 0;
                            }

                _cube = newCube;
            }

            private int Neighbors(bool[,,,] cube, int x, int y, int z, int v, int dimension)
            {
                var neighbors = 0;
                var diffs = dimension == 3 ? Diffs3d : Diffs4d;

                foreach (var diff in diffs)
                    if (cube[x + diff.x, y + diff.y, z + diff.z, v + diff.v])
                        neighbors++;

                return neighbors;
            }

            private static (int x, int y, int z, int v)[] Diffs4d;

            private static readonly (int x, int y, int z, int v)[] Diffs3d = new[]
            {
                // slice 0
                (x: -1, y: -1, z: +0, +0),
                (x: +0, y: -1, z: +0, +0),
                (x: +1, y: -1, z: +0, +0),
                (x: +1, y: +0, z: +0, +0),
                (x: +1, y: +1, z: +0, +0),
                (x: +0, y: +1, z: +0, +0),
                (x: -1, y: +1, z: +0, +0),
                (x: -1, y: +0, z: +0, +0),
                // slice -1         , +0
                (x: +0, y: +0, z: -1, +0),
                (x: -1, y: -1, z: -1, +0),
                (x: +0, y: -1, z: -1, +0),
                (x: +1, y: -1, z: -1, +0),
                (x: +1, y: +0, z: -1, +0),
                (x: +1, y: +1, z: -1, +0),
                (x: +0, y: +1, z: -1, +0),
                (x: -1, y: +1, z: -1, +0),
                (x: -1, y: +0, z: -1, +0),
                // slice +1         , +0
                (x: +0, y: +0, z: +1, +0),
                (x: -1, y: -1, z: +1, +0),
                (x: +0, y: -1, z: +1, +0),
                (x: +1, y: -1, z: +1, +0),
                (x: +1, y: +0, z: +1, +0),
                (x: +1, y: +1, z: +1, +0),
                (x: +0, y: +1, z: +1, +0),
                (x: -1, y: +1, z: +1, +0),
                (x: -1, y: +0, z: +1, +0),
            };
        }

        private sealed class Day16
        {
            private readonly Dictionary<string, (int low, int hi)[]> _rules;
            private readonly int[] _yourTicket;
            private readonly int[][] _nearbyTickets;

            public Day16(string[] inputs)
            {
                // parsing the rules
                _rules = new Dictionary<string, (int low, int hi)[]>();
                foreach (var rule in inputs.TakeWhile(s => !string.IsNullOrEmpty(s)))
                {
                    var name = rule.Split(':')[0];
                    var rangesRaw = rule.Split(':')[1].Split(new[] { "or", " " }, StringSplitOptions.RemoveEmptyEntries);
                    var ranges = rangesRaw
                        .Select(r => r.Split('-'))
                        .Select(parts => (low: int.Parse(parts[0]), hi: int.Parse(parts[1])))
                        .ToArray();

                    _rules[name] = ranges;
                }
                // parsing your ticket
                _yourTicket = inputs.SkipWhile(s => s != "your ticket:").Skip(1).Take(1).Single()
                    .Split(',').Select(int.Parse).ToArray();

                // parsing nearby tickets
                _nearbyTickets = inputs.SkipWhile(s => s != "nearby tickets:").Skip(1)
                    .Select(row => row.Split(',').Select(int.Parse).ToArray()).ToArray();
            }

            public int CalcErrorRate()
            {
                var errorRate = 0;
                foreach (var ticket in _nearbyTickets)
                {
                    foreach (var field in ticket)
                    {
                        if (!_rules.Any(r => r.Value.Any(range => range.low <= field && range.hi >= field)))
                            errorRate += field;
                    }
                }

                return errorRate;
            }

            public long GetYourTicketField(string field)
            {
                var fieldsCount = _yourTicket.Length;
                var allTickets = _nearbyTickets.Concat(new[] { _yourTicket }).ToArray();
                var possiblePerIndex = new HashSet<string>[fieldsCount];
                // get matching row per each field
                for (int i = 0; i < fieldsCount; i++)
                {
                    var validRules = allTickets
                        .Select(t => GetMatchingRules(t[i]))
                        .Where(rules => rules.Count > 0)
                        .Aggregate((h1, h2) =>
                        {
                            h1.IntersectWith(h2);
                            return h1;
                        });

                    possiblePerIndex[i] = validRules;
                }

                // clean up unambiguity 
                while (possiblePerIndex.Any(rules => rules.Count != 1))
                {
                    var exactMatches = possiblePerIndex.Where(rules => rules.Count == 1).ToArray();
                    // remove exact matches from other matches
                    foreach (var rules in possiblePerIndex.Where(rules => rules.Count > 1))
                    {
                        foreach (var exactMatch in exactMatches)
                            rules.Remove(exactMatch.Single());
                    }
                }
                // get target field 
                var targetFieldIndexes = Enumerable
                    .Range(0, possiblePerIndex.Length)
                    .Where(i => possiblePerIndex[i].Single().StartsWith(field));

                return targetFieldIndexes
                    .Select(i => (long)_yourTicket[i])
                    .Aggregate(1L, (v1, v2) => v1 * v2);
            }

            private HashSet<string> GetMatchingRules(int field)
            {
                return _rules.Where(r => r.Value.Any(range => range.low <= field && range.hi >= field))
                    .Select(r => r.Key)
                    .ToHashSet();
            }
        }

        private sealed class Day14Memory2
        {
            private readonly IDictionary<ulong, ulong> _map = new Dictionary<ulong, ulong>();
            private ulong _mask1;
            private int[] _floatingMaskIndexes;

            public void SetMask(string mask)
            {
                _mask1 = Convert.ToUInt64(mask.Replace('X', '0'), 2);
                _floatingMaskIndexes = mask.Reverse()
                    .Select((c, i) => (c, i))
                    .Where(item => item.c == 'X')
                    .Select(item => item.i)
                    .ToArray();
            }

            public void SetValue(ulong index, ulong value)
            {
                foreach (var address in GetIndexes(index, _mask1, _floatingMaskIndexes))
                    _map[address] = value;
            }

            private IEnumerable<ulong> GetIndexes(ulong index, ulong mask1, int[] floatingMaskIndexes)
            {
                var size = Pow(2, floatingMaskIndexes.Length);
                for (int i = 0; i < size; i++)
                {
                    ulong fmask0 = 0;
                    ulong fmask1 = 0;
                    for (var index1 = 0; index1 < floatingMaskIndexes.Length; index1++)
                    {
                        var ind = floatingMaskIndexes[index1];
                        if ((i & (1 << index1)) > 0) // "ind" bit is set
                            fmask1 = fmask1 | (1ul << ind);
                        else
                            fmask0 = fmask0 | (1ul << ind);
                    }

                    var value = index | mask1; // applying base mask
                    value = value | fmask1;    // applying floating mask 1
                    value = value & ~fmask0;    // applying floating mask 0

                    yield return value;
                }
            }

            private static int Pow(int bas, int exp)
            {
                return Enumerable
                    .Repeat(bas, exp)
                    .Aggregate(1, (a, b) => a * b);
            }

            public ulong GetSum()
            {
                return _map.Values.Aggregate(0ul, (a, b) => a + b);
            }
        }

        private sealed class Day14Memory
        {
            private readonly IDictionary<ulong, ulong> _map = new Dictionary<ulong, ulong>();
            private ulong _mask0;
            private ulong _mask1;

            public void SetMask(string mask)
            {
                _mask0 = Convert.ToUInt64(mask.Replace('0', 'Y').Replace('X', '0').Replace('1', '0').Replace('Y', '1'), 2);
                _mask1 = Convert.ToUInt64(mask.Replace('X', '0'), 2);
            }

            public void SetValue(ulong index, ulong value)
            {
                value = value | _mask1;

                // val    mask0    res  
                // 1      1        0    
                // 1      0        1    
                // 0      1        0    
                // 0      0        0    
                value = value & ~_mask0;

                _map[index] = value;
            }

            public ulong GetSum()
            {
                return _map.Values.Aggregate(0ul, (a, b) => a + b);
            }
        }

        private sealed class Day11Map
        {
            private static readonly int[] _dx = { -1, +0, +1, +1, +1, +0, -1, -1 };
            private static readonly int[] _dy = { -1, -1, -1, +0, +1, +1, +1, +0 };

            private readonly int _width;
            private readonly int _height;
            private char[,] _map;

            public Day11Map(string[] inputs)
            {
                _width = inputs[0].Length;
                _height = inputs.Length;
                _map = new char[_width + 2, _height + 2]; // adding extra border rows
                for (int x = 1; x <= _width; x++)
                {
                    for (int y = 1; y <= _height; y++)
                        _map[x, y] = inputs[y - 1][x - 1];
                }
            }

            public long OccupiedSeats()
            {
                return Enumerable.Range(1, _width)
                    .Sum(x => Enumerable.Range(1, _height).Count(y => _map[x, y] == '#'));
            }

            public static long OccupiedSeats(char[,] map, int x, int y)
            {
                return Enumerable.Range(0, _dx.Length)
                    .Count(i => map[x + _dx[i], y + _dy[i]] == '#');
            }

            public static long OccupiedSeats2(char[,] map, int x, int y)
            {
                var xLen = map.GetLength(0);
                var yLen = map.GetLength(1);
                var max = Math.Max(xLen, yLen);
                var seen = 0;

                for (int i = 0; i < _dx.Length; i++)
                {
                    for (int d = 1; d <= max; d++)
                    {
                        var nx = x + _dx[i] * d;
                        var ny = y + _dy[i] * d;

                        if (!(nx > 0 && nx < xLen && ny > 0 && ny < yLen))
                            break;
                        if (map[nx, ny] == '#')
                        {
                            seen++;
                            break;
                        }

                        if (map[nx, ny] == 'L')
                            break;
                    }
                }

                return seen;
            }

            public string Dump()
            {
                return Enumerable.Range(1, _height)
                    .Select(y => new string(Enumerable.Range(1, _width).Select(x => _map[x, y]).ToArray()))
                    .Join(Environment.NewLine);
            }

            public bool SimulateOneStep(Func<char[,], int, int, long> occupiedFunc, int minOccupiedSeats)
            {
                var newMap = (char[,])_map.Clone();

                for (int x = 1; x <= _width; x++)
                {
                    for (int y = 1; y <= _height; y++)
                    {
                        var occupiedSeats = occupiedFunc(_map, x, y);
                        if (_map[x, y] == 'L' && occupiedSeats == 0)
                        {
                            newMap[x, y] = '#';
                        }
                        else if (_map[x, y] == '#' && occupiedSeats >= minOccupiedSeats)
                        {
                            newMap[x, y] = 'L';
                        }
                    }
                }

                var changed = false;
                for (int x = 1; x <= _width; x++)
                {
                    for (int y = 1; y <= _height; y++)
                        if (_map[x, y] != newMap[x, y])
                            changed = true;
                }

                _map = newMap;
                return changed;
            }
        }

        private static long Day09_FindInvalidNumber(int preambleSize, long[] a)
        {
            for (int i = preambleSize; i < a.Length; i++)
            {
                var found = false;
                for (int j = i - preambleSize; j < i && !found; j++)
                {
                    var targetSecondElement = a[i] - a[j];
                    if (targetSecondElement != a[j])
                        found = a.Skip(i - preambleSize).Take(preambleSize).Any(item => item == targetSecondElement);
                }

                if (!found)
                    return a[i];
            }

            throw new InvalidOperationException("no solution");
        }

        private static bool Day08_InterpretCode(string[] inputs, out long accumulator)
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

        private sealed class Day07Graph
        {
            private int _count;
            private readonly Dictionary<string, int> _indexes = new Dictionary<string, int>();
            private readonly int[,] _cost = new int[1000, 1000];

            public Day07Graph(string[] inputs)
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

        private int Day05Parse(string b)
        {
            var binaryString = b
                .Replace('F', '0')
                .Replace('B', '1')
                .Replace('L', '0')
                .Replace('R', '1');

            return Convert.ToInt32(binaryString, 2);
        }

        private sealed class Day04Passport
        {
            public bool HaveRequiredValues { get; private set; }
            public bool HaveValidValues { get; private set; }

            public static Day04Passport Parse(string line)
            {
                var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var fields = parts.ToDictionary(i => i.Split(":")[0], i => i.Split(":")[1]);

                var result = new Day04Passport
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

        private sealed class Day03Map
        {
            private readonly string[] _input;
            private readonly int _width;

            public Day03Map(string[] input)
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