using UnityEngine;

public static class Utils2D
{
	public static bool CheckIfNumDivisibleBy(float numberToCheck, int divisibleBy)
	{
		if (numberToCheck == 0 || divisibleBy == 0) return true;
		
		// Convert to integer by multiplying by 100 to avoid floating point precision issues
		float multiplied = numberToCheck * 100;

		// avoid floating point precision errors
		int asInt = Mathf.RoundToInt(multiplied);

		return asInt % divisibleBy == 0;
	}

	public static float GetNearestNumberDivisibleBy(float valueToSnap, float divisibleBy)
	{
		float snappedValue = Mathf.Round(valueToSnap / divisibleBy) * divisibleBy;
		return snappedValue;
	}
}

