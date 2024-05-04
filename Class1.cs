using System;

public class Class1
{
	public Class1()
	{

		public float CalculatePoints(float weight, int playtime, float placementPercentage)
		{
			float weightFactor = .25 * weight + .75;
			float playtimeFactor = ((-0.00487789 * (Math.Pow(playtime, 2))) + (2.02538 * playtime) - (1.95751)) / 100;
			float placementFactor = placementPercentage;

			float points = 10 * weightFactor * playtimeFactor * placementFactor;
			return points;
		}




	}
}
