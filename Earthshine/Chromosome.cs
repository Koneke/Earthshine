using System;

namespace Earthshine
{
	public class Chromosome
	{
		public Stat Stat;
		public float Value;

		public Chromosome(Stat stat, float value)
		{
			this.Stat = stat;
			this.Value = value;
		}

		public static Chromosome Generate(Stat? specificStat = null)
		{
			var stat = specificStat ?? Stats.GetRandomStat();
			return new Chromosome(stat, Program.Random.Next(10, 20));
		}

		public override string ToString()
		{
			var statName = Enum.GetName(typeof(Stat), this.Stat);
			statName = statName.Substring(0, 2);
			return $"{statName}: {this.Value}";
		}
	}
}