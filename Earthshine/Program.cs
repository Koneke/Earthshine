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
			var first = DNA.GenerateRandom(18);
			var second = DNA.GenerateRandom(18);
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

	public class Character
	{
		public string Name;
		public DNA Maternal;
		public DNA Paternal;
		public Dictionary<Stat, float> Stats;
	}
}