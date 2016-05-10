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
		Strength,
		Constitution,
		Dexterity,
		Intelligence,
		Wisdom,
		Charisma,
	}

	public static class Stats
	{
		private static List<Stat> GetAllStats()
		{
			return Enum.GetValues(typeof (Stat)).Cast<Stat>().ToList();
		}

		// blah blah blah not entirely perfect optimization wise, watch me not
		// give a fuck (yet).
		public static List<Stat> AllStats => GetAllStats();

		public static Stat GetRandomStat()
		{
			return AllStats[Program.Random.Next(AllStats.Count)];
		}
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

			foreach (var stat in Stats.AllStats)
			{
				this.Chromosomes.Add(stat, new List<Chromosome>());
			}
		}

		public override string ToString()
		{
			var stats = new Dictionary<Stat, List<string>>();
			var joined = new List<string>();

			foreach (var stat in Stats.AllStats)
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
				var stat = Stats.GetRandomStat();
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
			foreach (var stat in Stats.AllStats)
			{
				firstBucket.Add(stat, new List<Chromosome>());
				secondBucket.Add(stat, new List<Chromosome>());

				first.Chromosomes[stat].ForEach(c => firstBucket[stat].Add(c));
				second.Chromosomes[stat].ForEach(c => secondBucket[stat].Add(c));
			}

			foreach (var stat in Stats.AllStats)
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