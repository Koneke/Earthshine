using System;
using System.Collections.Generic;
using System.Linq;

namespace Earthshine
{
	public class DNA
	{
		public static Comparer<Chromosome> Comparer = Comparer<Chromosome>.Create((a, b) => a.Value.CompareTo(b.Value));

		public Dictionary<Stat, List<Chromosome>> Chromosomes;

		public int ChromosomeCount =>
			this.Chromosomes
				.Keys
				.Select(k => this.Chromosomes[k].Count)
				.Sum();

		public DNA()
		{
			this.Chromosomes = new Dictionary<Stat, List<Chromosome>>();

			foreach (var stat in Stats.AllStats)
			{
				this.Chromosomes.Add(stat, new List<Chromosome>());
			}
		}

		public static DNA GenerateRandom(int length)
		{
			var result = new DNA();

			if (length < Stats.AllStats.Count)
			{
				throw new Exception("Are you trying to create a cripple??");
			}

			// guarantee atleast one chromosome per stat.
			foreach (var stat in Stats.AllStats)
			{
				result.AddChromosome(Chromosome.Generate(stat));
			}

			// rest we just fill up at random.
			while (result.ChromosomeCount < length)
			{
				result.AddChromosome(Chromosome.Generate());
			}

			// (this is where we'd tweak things later to make it a bit more
			// even and nice).

			return result;
		}

		public static DNA Join(DNA first, DNA second)
		{
			if (first.ChromosomeCount != second.ChromosomeCount)
			{
				throw new Exception("Are you spawning a mutant? What the fuck?");
			}

			var expectedLength = first.ChromosomeCount;
			var result = new DNA();

			var firstBucket = new Dictionary<Stat, List<Chromosome>>();
			var secondBucket = new Dictionary<Stat, List<Chromosome>>();
			var excessBucket = new List<Chromosome>();

			// create our buckets
			foreach (var stat in Stats.AllStats)
			{
				firstBucket.Add(stat, new List<Chromosome>());
				secondBucket.Add(stat, new List<Chromosome>());

				first.Chromosomes[stat].ForEach(c => firstBucket[stat].Add(c));
				second.Chromosomes[stat].ForEach(c => secondBucket[stat].Add(c));

				List<Chromosome> largerPool, smallerPool;

				// identify smallest/largest pool.
				if (firstBucket[stat].Count > secondBucket[stat].Count)
				{
					largerPool = firstBucket[stat];
					smallerPool = secondBucket[stat];
				}
				else if (firstBucket[stat].Count < secondBucket[stat].Count)
				{
					smallerPool = firstBucket[stat];
					largerPool = secondBucket[stat];
				}
				else
				{
					largerPool = Program.Random.NextDouble() > 0.5
						? firstBucket[stat]
						: secondBucket[stat];
					smallerPool = (largerPool == firstBucket[stat])
						? secondBucket[stat]
						: firstBucket[stat];
				}

				// eliminate excess so we can pair evenly.
				// at the moment we just remove the lowest value chrom.
				largerPool.Sort(Comparer);
				smallerPool.Sort(Comparer);

				while (largerPool.Count > smallerPool.Count)
				{
					excessBucket.Add(largerPool[0]);
					largerPool.RemoveAt(0);
				}
			}

			// join the two, now even strands, at random.
			foreach (var stat in Stats.AllStats)
			{
				for (var i = 0; i < firstBucket[stat].Count; i++)
				{
					var chrom = Program.Random.NextDouble() > 0.5
						? firstBucket[stat][i]
						: secondBucket[stat][i];
					result.Chromosomes[stat].Add(chrom);
				}
			}

			// fill up to expected length at random from the excess bucket.
			while (result.ChromosomeCount < expectedLength)
			{
				var chrom = excessBucket[Program.Random.Next(excessBucket.Count)];
				result.Chromosomes[chrom.Stat].Add(chrom);
				excessBucket.Remove(chrom);
			}

			return result;
		}

		public void AddChromosome(Chromosome chromosome)
		{
			this.Chromosomes[chromosome.Stat].Add(chromosome);
		}

		public override string ToString()
		{
			return this.Chromosomes.DictListJoin(
				"      ", " ",
				chromosome => chromosome.Value,
				chromosome => chromosome.ToString());
		}

		public string CSVDump()
		{
			return this.Chromosomes.DictListJoin(
				",", ";",
				chromosome => chromosome.Value,
				chromosome => chromosome.Value);
		}
	}
}