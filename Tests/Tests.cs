using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AitLab7BeesAlg.Models;
using AitLab7BeesAlg.Models.BeesAlg;
using AitLab7BeesAlg.ViewModels.Builders;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        private const double Tol = 1e-15;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestGeneticAlgModel()
        {
            var random = new Random(17);

            var func = FunctionParser.ParseFunction("4*(x1-5)^2 + (x2 - 6)^2");

            func = (x1, x2) => 4 * Math.Pow(x1 - 5, 2) + Math.Pow(x2 - 6, 2);
            
            var algSettings = new BeesAlgSettings()
            {
                Alpha = 0.95,
                IterationsCount = 50,
                TempInit = 100,
                InitScoutsCount = 20,
                X1Bounds = (-10, 10),
                X2Bounds = (-10, 10),
            };



            var algorithm = new BeesAlgorithm(func, algSettings, random);
            var actualResult = algorithm.FindMinFunction();
            Console.WriteLine($"X1: {actualResult.X1}, X2: {actualResult.X2}, F(X1,X2): {actualResult.FuncValue}");
            Console.WriteLine($"{random.Next()}");
            
            
            // Console.WriteLine($"X1: {actualResult.X1}, X2: {actualResult.X2}, F(X1,X2): {actualResult.FuncValue}");
            //
            //
            // var expectedResult = new Chromosome
            // {
            //     X1 = 5.0000019833704705,
            //     X2 = 5.9969435668879525,
            //     FuncValue = 9.341799103454105E-06
            // };
            //
            // Assert.That(actualResult.X1, Is.EqualTo(expectedResult.X1).Within(Tol));
            // Assert.That(actualResult.X2, Is.EqualTo(expectedResult.X2).Within(Tol));
            // Assert.That(actualResult.FuncValue, Is.EqualTo(expectedResult.FuncValue).Within(Tol));
        }

        [Test]
        public void TestGeneticAlgModelViewModel()
        {
            var random = new Random(18);

            var genAlgBuilderVm = new GeneticAlgorithmBuilderVm(random)
            {
                SelectedFunction = "4*(x1-5)^2 + (x2 - 6)^2", CountGenerations = 100, PopulationSize = 20
            };


            (genAlgBuilderVm.X1Bounds.A, genAlgBuilderVm.X1Bounds.B) = (-10, 10);
            (genAlgBuilderVm.X2Bounds.A, genAlgBuilderVm.X2Bounds.B) = (-10, 10);


            var tournamentSelectionBuilderVm =
                genAlgBuilderVm.Selections.OfType<TournamentSelectionBuilderVm>().First();
            tournamentSelectionBuilderVm.TournamentSize = 3;
            genAlgBuilderVm.SelectedSelection = tournamentSelectionBuilderVm;

            var alphaCrossoverBuilderVm =
                genAlgBuilderVm.Crossovers.OfType<AlphaCrossoverBuilderVm>().First();
            alphaCrossoverBuilderVm.Alpha = 0.5;
            alphaCrossoverBuilderVm.CrossingProbability = 0.98;
            genAlgBuilderVm.SelectedCrossover = alphaCrossoverBuilderVm;

            var uniformMutationBuilderVm =
                genAlgBuilderVm.Mutations.OfType<UniformMutationBuilderVm>().First();
            uniformMutationBuilderVm.MutationProbability = 0.1;
            genAlgBuilderVm.SelectedMutation = uniformMutationBuilderVm;


            var algorithm = genAlgBuilderVm.Build();
            var actualResult = algorithm.FindMinFunction();
            //Console.WriteLine($"X1: {actualResult.X1}, X2: {actualResult.X2}, F(X1,X2): {actualResult.FuncValue}");

            
            var expectedResult = new Chromosome
            {
                X1 = 5.0000019833704705,
                X2 = 5.9969435668879525,
                FuncValue = 9.341799103454105E-06
            };

            Assert.That(actualResult.X1, Is.EqualTo(expectedResult.X1).Within(Tol));
            Assert.That(actualResult.X2, Is.EqualTo(expectedResult.X2).Within(Tol));
            Assert.That(actualResult.FuncValue, Is.EqualTo(expectedResult.FuncValue).Within(Tol));
        }

        [Test]
        public void TestParser()
        {
            var func = FunctionParser.ParseFunction(@"4*(x1 - 5)^2 + (x2 - 6)^2");
            Assert.That(func(1, 2), Is.EqualTo(80).Within(Tol));
        }

        [Test]
        public void TestIEnumerable()
        {
            List<int> list = new() { 1, 2, 3, 4, 5 };
            using var enumerator = list.GetEnumerator();
            enumerator.MoveNext();
            Console.WriteLine(enumerator.Current);
        }
    }
}