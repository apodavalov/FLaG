using System.Collections.Immutable;

namespace FLaG.IO.Out
{
    public static class PermutationFinder
    {
        private static Random _Rand = new();

        private sealed record Edge(int U, int V);

        private sealed record Individual(IImmutableList<int> Chromosome, int Fitness)
            : IComparable<Individual>
        {
            public int CompareTo(Individual? other)
            {
                if (other == null)
                {
                    return 1;
                }

                return Fitness.CompareTo(other.Fitness);
            }
        }

        public static List<int> Find(ReadOnlyGraphMatrix graph)
        {
            int populationSize = 100;
            int generations = 500;
            double mutationRate = 0.3;
            int elitismCount = 5;

            ImmutableList<Edge> edges = [.. GetEdges(graph)];
            List<Individual> population = EvaluatePopulation(
                InitializePopulation(populationSize, graph.VertexCount),
                edges
            );

            for (int gen = 0; gen < generations; ++gen)
            {
                population.Sort();

                List<IImmutableList<int>> nextGeneration = [];
                for (int i = 0; i < elitismCount; ++i)
                {
                    nextGeneration.Add(population[i].Chromosome);
                }

                while (nextGeneration.Count < populationSize)
                {
                    Individual parent1 = TournamentSelection(population, 5);
                    Individual parent2 = TournamentSelection(population, 5);

                    List<int> childChromosome = OrderedCrossover(
                        parent1.Chromosome,
                        parent2.Chromosome
                    );

                    if (_Rand.NextDouble() < mutationRate)
                    {
                        int idx1 = _Rand.Next(childChromosome.Count);
                        int idx2 = _Rand.Next(childChromosome.Count);
                        (childChromosome[idx1], childChromosome[idx2]) = (
                            childChromosome[idx2],
                            childChromosome[idx1]
                        );
                    }

                    nextGeneration.Add(childChromosome.ToImmutableList());
                }

                population = EvaluatePopulation(nextGeneration, edges);
            }

            population.Sort();
            return population.FirstOrDefault()?.Chromosome.ToList() ?? [];
        }

        private static List<IImmutableList<int>> InitializePopulation(int size, int vertexCount)
        {
            var pop = new List<IImmutableList<int>>();
            int[] baseGenome = [.. Enumerable.Range(0, vertexCount)];

            for (int i = 0; i < size; ++i)
            {
                _Rand.Shuffle(baseGenome);
                pop.Add([.. baseGenome]);
            }
            return pop;
        }

        private static List<Individual> EvaluatePopulation(
            IReadOnlyList<IImmutableList<int>> population,
            IImmutableList<Edge> edges
        )
        {
            List<Individual> individuals = [];
            foreach (var individual in population)
            {
                individuals.Add(new(individual, CountCrossings(individual, edges)));
            }
            return individuals;
        }

        private static int CountCrossings(IImmutableList<int> chromosome, IReadOnlyList<Edge> edges)
        {
            int crossings = 0;

            int[] positions = new int[chromosome.Count];
            for (int i = 0; i < chromosome.Count; ++i)
            {
                positions[chromosome[i]] = i;
            }

            for (int i = 0; i < edges.Count; ++i)
            {
                for (int j = i + 1; j < edges.Count; ++j)
                {
                    int a = positions[edges[i].U];
                    int b = positions[edges[i].V];
                    int c = positions[edges[j].U];
                    int d = positions[edges[j].V];

                    if (a > b)
                    {
                        (a, b) = (b, a);
                    }
                    if (c > d)
                    {
                        (c, d) = (d, c);
                    }

                    if ((a < c && c < b && b < d) || (c < a && a < d && d < b))
                    {
                        ++crossings;
                    }
                }
            }
            return crossings;
        }

        private static Individual TournamentSelection(
            IReadOnlyList<Individual> population,
            int tournamentSize
        )
        {
            Individual? best = null;
            for (int i = 0; i < tournamentSize; ++i)
            {
                var competitor = population[_Rand.Next(population.Count)];
                if (best == null || competitor.Fitness < best.Fitness)
                {
                    best = competitor;
                }
            }
            return best!;
        }

        private static List<int> OrderedCrossover(
            IImmutableList<int> parent1,
            IImmutableList<int> parent2
        )
        {
            int[] child = new int[parent1.Count];
            Array.Fill(child, -1);

            int start = _Rand.Next(parent1.Count);
            int end = _Rand.Next(parent1.Count);
            if (start > end)
            {
                (start, end) = (end, start);
            }

            for (int i = start; i <= end; ++i)
            {
                child[i] = parent1[i];
            }

            int currentChildIdx = (end + 1) % child.Length;
            int currentParentIdx = (end + 1) % parent2.Count;

            int filledCount = end - start + 1;

            while (filledCount < parent2.Count)
            {
                int item = parent2[currentParentIdx];
                if (!child.Contains(item))
                {
                    child[currentChildIdx] = item;
                    currentChildIdx = (currentChildIdx + 1) % child.Length;
                    ++filledCount;
                }
                currentParentIdx = (currentParentIdx + 1) % parent2.Count;
            }

            return [.. child];
        }

        private static List<Edge> GetEdges(ReadOnlyGraphMatrix graph)
        {
            List<Edge> edges = [];
            for (int i = 0; i < graph.VertexCount; ++i)
            {
                for (int j = 0; j < i; ++j)
                {
                    if (graph.Get(i, j))
                    {
                        edges.Add(new(i, j));
                    }
                }
            }
            return edges;
        }
    }
}
