using System;
using System.IO;
using System.Linq;
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
    }
}
