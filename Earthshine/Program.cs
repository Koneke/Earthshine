namespace Earthshine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	class Program
	{
		public static Random Random = new Random();

		private static void Main()
		{
			var first = DNA.GenerateRandom();
			var second = DNA.GenerateRandom();
			var joined = DNA.Join(first, second);
			var a = 0;
		}
	}

	public enum Stat
	{
		FIRST,
		Strength,
		Constitution,
		Dexterity,
		Intelligence,
		Wisdom,
		Charisma,
		MAX
	}

	public class Chromosome
	{
		public Stat Stat;
		public float Value;

		public Chromosome(Stat stat, float value)
		{
			this.Stat = stat;
			this.Value = value;
		}

		public override string ToString()
		{
			var statName = Enum.GetName(typeof(Stat), this.Stat);
			statName = statName.Substring(0, 2);
			return $"{statName}: {this.Value}";
		}
	}

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

			for (var stat = Stat.FIRST + 1; stat < Stat.MAX; stat++)
			{
				this.Chromosomes.Add(stat, new List<Chromosome>());
			}
		}

		public override string ToString()
		{
			var stats = new Dictionary<Stat, List<string>>();
			var joined = new List<string>();

			for (var stat = Stat.FIRST + 1; stat < Stat.MAX; stat++)
			{
				stats.Add(stat, new List<string>());

				var chromosomes = new List<Chromosome>(this.Chromosomes[stat]);
				chromosomes.Sort(Comparer);

				foreach (var chromosome in chromosomes)
				{
					stats[stat].Add(chromosome.ToString());
				}

				joined.Add(string.Join(" ", stats[stat]));
			}

			return string.Join("      ", joined);
		}

		public static DNA GenerateRandom()
		{
			var result = new DNA();

			for (var i = 0; i < 18; i++)
			{
				var stat = (Stat)Program.Random.Next(
					(int)(Stat.FIRST + 1),
					(int)Stat.MAX);
				var chromosome = new Chromosome(stat, Program.Random.Next(10, 20));
				result.Chromosomes[stat].Add(chromosome);
			}

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
			for (var stat = Stat.FIRST + 1; stat < Stat.MAX; stat++)
			{
				firstBucket.Add(stat, new List<Chromosome>());
				secondBucket.Add(stat, new List<Chromosome>());

				first.Chromosomes[stat].ForEach(c => firstBucket[stat].Add(c));
				second.Chromosomes[stat].ForEach(c => secondBucket[stat].Add(c));
			}

			for (var stat = Stat.FIRST + 1; stat < Stat.MAX; stat++)
			{
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
			for (var stat = Stat.FIRST + 1; stat < Stat.MAX; stat++)
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
			}

			return result;
		}
	}

	public class Character
	{
		public string Name;
		public DNA Maternal;
		public DNA Paternal;
		public Dictionary<Stat, float> Stats;
	}
}