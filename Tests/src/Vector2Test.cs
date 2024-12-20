using DoomNET.Resources;

namespace Resources;

[TestClass]
public class Vector2Test
{
	private Random rand = new Random();

	// Value 1's
	private float value1_x; // X
	private float value1_y; // Y
	// values

	// Value 2's
	private float value2_x; // X
	private float value2_y; // Y
	// values

	// Minimum and maximum values for rand.Next(int, int) to use
	private const int RAND_MIN = -250000;
	private const int RAND_MAX = 250000;

	[TestMethod("Addition")]
	public void TestAddition()
	{
		int errorCount = 0;
		List<(Vector2 value1, Vector2 value2, Vector2 result, Vector2 expected)> errorList = new();

		Console.WriteLine("Testing addition...");

		// Do 5000 tests
		for (int i = 0; i < 5000; i++)
		{
			Vector2 expected = new Vector2
			(
				(value1_x = rand.Next(RAND_MIN, RAND_MAX)) + (value2_x = rand.Next(RAND_MIN, RAND_MAX)),
				(value1_y = rand.Next(RAND_MIN, RAND_MAX)) + (value2_y = rand.Next(RAND_MIN, RAND_MAX))
			);

			Vector2 value1 = new Vector2(value1_x, value1_y);
			Vector2 value2 = new Vector2(value2_x, value2_y);
			Vector2 result = value1 + value2;

			Console.WriteLine($"(Test #{i + 1}) value1 ({value1}) + value2 ({value2}) = {result} (expected: {expected})");

			if (result != expected)
			{
				errorCount++;
				errorList.Add((value1, value2, result, expected));
			}
		}

		if (errorCount > 0)
		{
			// Add spacing between comments and errorList
			Console.WriteLine("\n\n");

			for (int i = 0; i < errorCount; i++)
			{
				Console.WriteLine($"(Error #{i + 1}) value1 ({errorList[i].value1}) + value2 ({errorList[i].value2}) != expected ({errorList[i].expected}) (result was {errorList[i].result})");
			}

			throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
		}
	}

	[TestMethod("Subtraction")]
	public void TestSubtraction()
	{
		int errorCount = 0;
		List<(Vector2 value1, Vector2 value2, Vector2 result, Vector2 expected)> errorList = new();

		Console.WriteLine("Testing subtraction...");

		// Do 5000 tests
		for (int i = 0; i < 5000; i++)
		{
			Vector2 expected = new Vector2
			(
				(value1_x = rand.Next(RAND_MIN, RAND_MAX)) - (value2_x = rand.Next(RAND_MIN, RAND_MAX)),
				(value1_y = rand.Next(RAND_MIN, RAND_MAX)) - (value2_y = rand.Next(RAND_MIN, RAND_MAX))
			);

			Vector2 value1 = new Vector2(value1_x, value1_y);
			Vector2 value2 = new Vector2(value2_x, value2_y);
			Vector2 result = value1 - value2;

			Console.WriteLine($"(Test #{i + 1}) value1 ({value1}) - value2 ({value2}) = {result} (expected: {expected})");

			if (result != expected)
			{
				errorCount++;
				errorList.Add((value1, value2, result, expected));
			}
		}

		if (errorCount > 0)
		{
			// Add spacing between comments and errorList
			Console.WriteLine("\n\n");

			for (int i = 0; i < errorCount; i++)
			{
				Console.WriteLine($"(Error #{i + 1}) value1 ({errorList[i].value1}) - value2 ({errorList[i].value2}) != expected ({errorList[i].expected}) (result was {errorList[i].result})");
			}

			throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
		}
	}

	[TestMethod("Multiplication")]
	public void TestMultiplication()
	{
		int errorCount = 0;
		List<(Vector2 value1, Vector2 value2, Vector2 result, Vector2 expected)> errorList = new();

		Console.WriteLine("Testing multiplication...");

		// Do 5000 tests
		for (int i = 0; i < 5000; i++)
		{
			Vector2 expected = new Vector2
			(
				(value1_x = rand.Next(RAND_MIN, RAND_MAX)) * (value2_x = rand.Next(RAND_MIN, RAND_MAX)),
				(value1_y = rand.Next(RAND_MIN, RAND_MAX)) * (value2_y = rand.Next(RAND_MIN, RAND_MAX))
			);

			Vector2 value1 = new Vector2(value1_x, value1_y);
			Vector2 value2 = new Vector2(value2_x, value2_y);
			Vector2 result = value1 * value2;

			Console.WriteLine($"(Test #{i + 1}) value1 ({value1}) * value2 ({value2}) = {result} (expected: {expected})");

			if (result != expected)
			{
				errorCount++;
				errorList.Add((value1, value2, result, expected));
			}
		}

		if (errorCount > 0)
		{
			// Add spacing between comments and errorList
			Console.WriteLine("\n\n");

			for (int i = 0; i < errorCount; i++)
			{
				Console.WriteLine($"(Error #{i + 1}) value1 ({errorList[i].value1}) * value2 ({errorList[i].value2}) != expected ({errorList[i].expected}) (result was {errorList[i].result})");
			}

			throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
		}
	}

	[TestMethod("Multiplication (Float)")]
	public void TestMultiplicationFloat()
	{
		int errorCount = 0;
		List<(Vector2 value1, Vector2 value2, Vector2 result, Vector2 expected)> errorList = new();

		Console.WriteLine("Testing multiplication...");

		// Do 5000 tests
		for (int i = 0; i < 5000; i++)
		{
			Vector2 expected = new Vector2
			(
				(value1_x = rand.Next(RAND_MIN, RAND_MAX)) * (value2_x = rand.Next(RAND_MIN, RAND_MAX)),
				(value1_y = rand.Next(RAND_MIN, RAND_MAX)) * (value2_y = rand.Next(RAND_MIN, RAND_MAX))
			);

			Vector2 value1 = new Vector2(value1_x, value1_y);
			Vector2 value2 = new Vector2(value2_x, value2_y);
			Vector2 result = value1 * value2;

			Console.WriteLine($"(Test #{i + 1}) value1 ({value1}) * value2 ({value2}) = {result} (expected: {expected})");

			if (result != expected)
			{
				errorCount++;
				errorList.Add((value1, value2, result, expected));
			}
		}

		if (errorCount > 0)
		{
			// Add spacing between comments and errorList
			Console.WriteLine("\n\n");

			for (int i = 0; i < errorCount; i++)
			{
				Console.WriteLine($"(Error #{i + 1}) value1 ({errorList[i].value1}) * value2 ({errorList[i].value2}) != expected ({errorList[i].expected}) (result was {errorList[i].result})");
			}

			throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
		}
	}

	[TestMethod("Division")]
	public void TestDivision()
	{
		int errorCount = 0;
		List<(Vector2 value1, Vector2 value2, Vector2 result, Vector2 expected)> errorList = new();

		Console.WriteLine("Testing division...");

		// Do 5000 tests
		for (int i = 0; i < 5000; i++)
		{
			Vector2 expected = new Vector2
			(
				(value1_x = rand.Next(RAND_MIN, RAND_MAX)) / (value2_x = rand.Next(RAND_MIN, RAND_MAX)),
				(value1_y = rand.Next(RAND_MIN, RAND_MAX)) / (value2_y = rand.Next(RAND_MIN, RAND_MAX))
			);

			Vector2 value1 = new Vector2(value1_x, value1_y);
			Vector2 value2 = new Vector2(value2_x, value2_y);
			Vector2 result = value1 / value2;

			Console.WriteLine($"(Test #{i + 1}) value1 ({value1}) / value2 ({value2}) = {result} (expected: {expected})");

			if (result != expected)
			{
				errorCount++;
				errorList.Add((value1, value2, result, expected));
			}
		}

		if (errorCount > 0)
		{
			// Add spacing between comments and errorList
			Console.WriteLine("\n\n");

			for (int i = 0; i < errorCount; i++)
			{
				Console.WriteLine($"(Error #{i + 1}) value1 ({errorList[i].value1}) / value2 ({errorList[i].value2}) != expected ({errorList[i].expected}) (result was {errorList[i].result})");
			}

			throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
		}
	}

	[TestMethod("Division (Float)")]
	public void TestDivisionFloat()
	{
		int errorCount = 0;
		List<(Vector2 value1, float value2, Vector2 result, Vector2 expected)> errorList = new();

		Console.WriteLine("Testing division...");

		// Do 5000 tests
		for (int i = 0; i < 5000; i++)
		{
			// We can't divide by zero, so retry!
			if ((value2_x = rand.Next(RAND_MIN, RAND_MAX)) == 0)
			{
				i--;
				continue;
			}

			Vector2 expected = new Vector2
			(
				(value1_x = rand.Next(RAND_MIN, RAND_MAX)) / value2_x,
				(value1_y = rand.Next(RAND_MIN, RAND_MAX)) / value2_x
			);

			Vector2 value1 = new Vector2(value1_x, value1_y);
			float value2 = value2_x;
			Vector2 result = value1 / value2;

			Console.WriteLine($"(Test #{i + 1}) value1 ({value1}) / value2 ({value2}) = {result} (expected: {expected})");

			if (result != expected)
			{
				errorCount++;
				errorList.Add((value1, value2, result, expected));
			}
		}

		if (errorCount > 0)
		{
			// Add spacing between comments and errorList
			Console.WriteLine("\n\n");

			for (int i = 0; i < errorCount; i++)
			{
				Console.WriteLine($"(Error #{i + 1}) value1 ({errorList[i].value1}) / value2 ({errorList[i].value2}) != expected ({errorList[i].expected}) (result was {errorList[i].result})");
			}

			throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
		}
	}

	[TestMethod("Less Than (<)")]
	public void TestLessThan()
	{
		int errorCount = 0;
		List<(Vector2 value1, Vector2 value2, bool result, bool expected)> errorList = new();

		Console.WriteLine("Testing less than (<)...");

		// Do 5000 tests
		for (int i = 0; i < 5000; i++)
		{
			Vector2 value1 = new Vector2(
				value1_x = rand.Next(RAND_MIN, RAND_MAX),
				value1_y = rand.Next(RAND_MIN, RAND_MAX));
			
			Vector2 value2 = new Vector2(
				value2_x = rand.Next(RAND_MIN, RAND_MAX),
				value2_y = rand.Next(RAND_MIN, RAND_MAX));

			bool expected = 
				(float)Math.Sqrt(value1_x * value1_x + value1_y * value1_y) -
				(float)Math.Sqrt(value2_x * value2_x + value2_y * value2_y) < 0;

			bool result = value1 < value2;

			Console.WriteLine($"(Test #{i + 1}) value1 ({value1}) < value2 ({value2}) ? {result} (expected: {expected})");

			if (result != expected)
			{
				errorCount++;
				errorList.Add((value1, value2, result, expected));
			}
		}

		if (errorCount > 0)
		{
			// Add spacing between comments and errorList
			Console.WriteLine("\n\n");

			for (int i = 0; i < errorCount; i++)
			{
				Console.WriteLine($"(Error #{i + 1}) value1 ({errorList[i].value1}) < value2 ({errorList[i].value2}) != expected ({errorList[i].expected}) (result was {errorList[i].result})");
			}

			throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
		}
	}

	[TestMethod("Greater than (>)")]
	public void TestGreaterThan()
	{
		int errorCount = 0;
		List<(Vector2 value1, Vector2 value2, bool result, bool expected)> errorList = new();

		Console.WriteLine("Testing greater than (>)...");

		// Do 5000 tests
		for (int i = 0; i < 5000; i++)
		{
			Vector2 value1 = new Vector2(
				value1_x = rand.Next(RAND_MIN, RAND_MAX),
				value1_y = rand.Next(RAND_MIN, RAND_MAX));
			
			Vector2 value2 = new Vector2(
				value2_x = rand.Next(RAND_MIN, RAND_MAX),
				value2_y = rand.Next(RAND_MIN, RAND_MAX));

			bool expected = 
				(float)Math.Sqrt(value1_x * value1_x + value1_y * value1_y) -
				(float)Math.Sqrt(value2_x * value2_x + value2_y * value2_y) > 0;

			bool result = value1 > value2;

			Console.WriteLine($"(Test #{i + 1}) value1 ({value1}) > value2 ({value2}) ? {result} (expected: {expected})");

			if (result != expected)
			{
				errorCount++;
				errorList.Add((value1, value2, result, expected));
			}
		}

		if (errorCount > 0)
		{
			// Add spacing between comments and errorList
			Console.WriteLine("\n\n");

			for (int i = 0; i < errorCount; i++)
			{
				Console.WriteLine($"(Error #{i + 1}) value1 ({errorList[i].value1}) > value2 ({errorList[i].value2}) != expected ({errorList[i].expected}) (result was {errorList[i].result})");
			}

			throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
		}
	}
}