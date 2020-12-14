using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
                    .Split(new[] {'=', ' '}, StringSplitOptions.RemoveEmptyEntries)
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
        [TestCase("Day14_problem.txt", ExpectedResult = 5875750429995)]
        public ulong Day14_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var mem = new Day14Memory2();
            foreach (var line in inputs)
            {
                var parts = line
                    .Split(new[] {'=', ' '}, StringSplitOptions.RemoveEmptyEntries)
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