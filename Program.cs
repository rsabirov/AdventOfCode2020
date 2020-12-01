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
            var filePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "../../../Input/", fileName);
            var input = File.ReadAllLines(filePath).Select(int.Parse).ToArray();

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
            var filePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "../../../Input/", fileName);
            var input = File.ReadAllLines(filePath).Select(int.Parse).ToArray();

            for (int i = 0; i < input.Length; i++)
            {
                for (int j = i + 1; j < input.Length; j++)
                for (int k = j + 1; k < input.Length; k++)
                    if (input[i] + input[j] + input[k] == 2020)
                        return input[i] * input[j] * input[k];
            }

            throw new InvalidOperationException("Solution not found");
        }

    }
}
