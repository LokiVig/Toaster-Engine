using Toast.Engine.Resources;

namespace Resources;

[TestClass]
public class BBoxTest
{
	private Random rand = new Random();

	// BBox 1's
	// mins
	private float mins1_x; // X
	private float mins1_y; // Y
	private float mins1_z; // Z
	// values,

	// maxs
	private float maxs1_x; // X
	private float maxs1_y; // Y
	private float maxs1_z; // Z
	// values

	// BBox 2's
	// mins
	private float mins2_x; // X
	private float mins2_y; // Y
	private float mins2_z; // Z
	// values,

	// maxs
	private float maxs2_x; // X
	private float maxs2_y; // Y
	private float maxs2_z; // Z
	// values

	// Minimum and maximum values for rand.Next(int, int) to use
	private const int RAND_MIN = -250000;
	private const int RAND_MAX = 250000;

	[TestMethod("Within (Bounding Box)")]
	public void WithinBoundingBox()
	{
		int errorCount = 0;
		List<(BBox value1, BBox value2, bool result, bool expected)> errorList = new();

		Console.WriteLine("Testing within (bounding box)...");

		// Do 5000 tests
		for (int i = 0; i < 5000; i++)
		{
			maxs1_x = rand.Next(RAND_MIN, RAND_MAX);
			maxs1_y = rand.Next(RAND_MIN, RAND_MAX);
			maxs1_z = rand.Next(RAND_MIN, RAND_MAX);
			mins1_x = rand.Next(RAND_MIN, RAND_MAX);
			mins1_y = rand.Next(RAND_MIN, RAND_MAX);
			mins1_z = rand.Next(RAND_MIN, RAND_MAX);

			maxs2_x = rand.Next(RAND_MIN, RAND_MAX);
			maxs2_y = rand.Next(RAND_MIN, RAND_MAX);
			maxs2_z = rand.Next(RAND_MIN, RAND_MAX);
			mins2_x = rand.Next(RAND_MIN, RAND_MAX);
			mins2_y = rand.Next(RAND_MIN, RAND_MAX);
			mins2_z = rand.Next(RAND_MIN, RAND_MAX);

			Vector3 maxs1 = new Vector3(maxs1_x, maxs1_y, maxs1_z);
			Vector3 mins1 = new Vector3(mins1_x, mins1_y, mins1_z);

			Vector3 maxs2 = new Vector3(maxs2_x, maxs2_y, maxs2_z);
			Vector3 mins2 = new Vector3(mins2_x, mins2_y, mins2_z);

			BBox value1 = new BBox(mins1, maxs1);
			BBox value2 = new BBox(mins2, maxs2);

			bool expected = mins1_x <= maxs2_x && maxs1_x >= mins2_x &&
			                mins1_y <= maxs2_y && maxs1_y >= mins2_y &&
			                mins1_z <= maxs2_z && maxs1_z >= mins2_z;

			bool result = value1.IntersectingWith(value2);

			Console.WriteLine($"(Test #{i + 1}) value2 ({value2}) within value1 ({value1}) ? {result} (expected: {expected})");

			if (result != expected) // We have an error!
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
				Console.WriteLine($"(Error #{i + 1}) value2 ({errorList[i].value2}) within value1 ({errorList[i].value1}) != expected ({errorList[i].expected}) (result was {errorList[i].result})");
			}

			throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
		}
	}

	[TestMethod("Within (Point)")]
	public void WithinPoint()
	{
		int errorCount = 0;
		List<(BBox value1, Vector3 value2, bool result, bool expected)> errorList = new();

		Console.WriteLine("Testing within (bounding box)...");

		// Do 5000 tests
		for (int i = 0; i < 5000; i++)
		{
			maxs1_x = rand.Next(RAND_MIN, RAND_MAX);
			maxs1_y = rand.Next(RAND_MIN, RAND_MAX);
			maxs1_z = rand.Next(RAND_MIN, RAND_MAX);
			mins1_x = rand.Next(RAND_MIN, RAND_MAX);
			mins1_y = rand.Next(RAND_MIN, RAND_MAX);
			mins1_z = rand.Next(RAND_MIN, RAND_MAX);

			// We don't need mins2, so don't initialize its values
			maxs2_x = rand.Next(RAND_MIN, RAND_MAX);
			maxs2_y = rand.Next(RAND_MIN, RAND_MAX);
			maxs2_z = rand.Next(RAND_MIN, RAND_MAX);

			Vector3 maxs1 = new Vector3(maxs1_x, maxs1_y, maxs1_z);
			Vector3 mins1 = new Vector3(mins1_x, mins1_y, mins1_z);

			BBox value1 = new BBox(mins1, maxs1);
			Vector3 value2 = new Vector3(maxs2_x, maxs2_y, maxs2_z);

			bool expected = value2.x >= mins1.x && value2.x <= maxs1.x &&
			                value2.y >= mins1.y && value2.y <= maxs1.y &&
			                value2.z >= mins1.z && value2.z <= maxs1.z;

			bool result = value1.IntersectingWith(value2);

			Console.WriteLine($"(Test #{i + 1}) value2 ({value2}) within value1 ({value1}) ? {result} (expected: {expected})");

			if (result != expected) // We have an error!
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
				Console.WriteLine($"(Error #{i + 1}) value2 ({errorList[i].value2}) within value1 ({errorList[i].value1}) != expected ({errorList[i].expected}) (result was {errorList[i].result})");
			}

			throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
		}
	}

	// [TestMethod("Traces")]
	// public void Traces()
	// {
	// 	int errorCount = 0;
	// 	List<(BBox value1, (Vector3 origin, Vector3 direction, float length) ray, bool result, bool expected)> errorList = new();
	//
	// 	Console.WriteLine("Testing traces...");
	//
	// 	// Do 5000 tests
	// 	for (int i = 0; i < 5000; i++)
	// 	{
	// 		maxs1_x = rand.Next(RAND_MIN, RAND_MAX);
	// 		maxs1_y = rand.Next(RAND_MIN, RAND_MAX);
	// 		maxs1_z = rand.Next(RAND_MIN, RAND_MAX);
	// 		mins1_x = rand.Next(RAND_MIN, RAND_MAX);
	// 		mins1_y = rand.Next(RAND_MIN, RAND_MAX);
	// 		mins1_z = rand.Next(RAND_MIN, RAND_MAX);
	// 		
	// 		// Mins can't be larger than maxs
	// 		if (mins1_x > maxs1_x)
	// 		{
	// 			(mins1_x, maxs1_x) = (maxs1_x, mins1_x);
	// 		}
	//
	// 		if (mins1_y > maxs1_y)
	// 		{
	// 			(mins1_y, maxs1_y) = (maxs1_y, mins1_y);
	// 		}
	//
	// 		if (mins1_z > maxs1_z)
	// 		{
	// 			(mins1_z, maxs1_z) = (maxs1_z, mins1_z);
	// 		}
	// 		
	// 		// In this case, mins2_* means the origin position of a trace...
	// 		mins2_x = rand.Next(RAND_MIN, RAND_MAX);
	// 		mins2_y = rand.Next(RAND_MIN, RAND_MAX);
	// 		mins2_z = rand.Next(RAND_MIN, RAND_MAX);
	// 		
	// 		// while maxs2_* means the end position of a trace
	// 		maxs2_x = rand.Next(RAND_MIN, RAND_MAX);
	// 		maxs2_y = rand.Next(RAND_MIN, RAND_MAX);
	// 		maxs2_z = rand.Next(RAND_MIN, RAND_MAX);
	//
	// 		Vector3 maxs1 = new Vector3(maxs1_x, maxs1_y, maxs1_z);
	// 		Vector3 mins1 = new Vector3(mins1_x, mins1_y, mins1_z);
	//
	// 		BBox bbox = new BBox(mins1, maxs1);
	// 		(Vector3 origin, Vector3 direction, float length) ray = (new Vector3(mins2_x, mins2_y, mins2_z), new Vector3(maxs2_x, maxs2_y, maxs2_z), 5000.0f);
	//
	// 		bool expected;
	// 		
	// 		// Do the ray intersection calculations locally, as to allow us to see if the results match with if we were
	// 		// to just call the function regularly
	// 		{
	// 			// Avoid division by zero; use an epsilon value
	// 			const float epsilon = 1e-6f;
	//
	// 			float tMin = 0; // Start of intersection interval
	// 			float tMax = ray.length; // End of intersection interval
	//
	// 			// Check each axis
	// 			for (int j = 0; j < 3; j++)
	// 			{
	// 				float origin = ray.origin[j];
	// 				float direction = ray.direction[j];
	// 				float min = bbox.mins[j];
	// 				float max = bbox.maxs[j];
	//
	// 				if (Math.Abs(direction) < epsilon)
	// 				{
	// 					// Ray is parallel to the object (no intersection if origin is outside the object)
	// 					if (origin < min || origin > max)
	// 					{
	// 						expected = false;
	// 					}
	// 				}
	// 				else
	// 				{
	// 					// Calculate intersection t-values for the near and far planes
	// 					float t1 = (min - origin) / direction;
	// 					float t2 = (max - origin) / direction;
	//
	// 					// Swap t1 and t2 if needed to ensure t1 is the near intersection
	// 					if (t1 > t2)
	// 					{
	// 						(t1, t2) = (t2, t1);
	// 					}
	//
	// 					// Update the intersection interval
	// 					tMin = Math.Max(tMin, t1);
	// 					tMax = Math.Min(tMax, t2);
	//
	// 					// If the interval is invalid, there's no intersection
	// 					if (tMin > tMax)
	// 					{
	// 						expected = false;
	// 					}
	// 				}
	// 			}
	//
	// 			// If we reach this point, the ray intersects the bounding box within the valid range
	// 			expected = true;
	// 		}
	// 		
	// 		// Do the calculations of the thingamajig
	//
	// 		bool result = bbox.RayIntersects(ray.origin, ray.direction, ray.length);
	//
	// 		Console.WriteLine($"(Test #{i + 1}) ray (origin: {ray.origin}, direction: {ray.direction}, length: {ray.length}) hit value1 ({bbox}) ? {result} (expected: {expected})");
	//
	// 		if (result != expected) // We have an error!
	// 		{
	// 			errorCount++;
	// 			errorList.Add((bbox, ray, result, expected));
	// 		}
	// 	}
	//
	// 	if (errorCount > 0)
	// 	{
	// 		// Add spacing between comments and errorList
	// 		Console.WriteLine("\n\n");
	//
	// 		for (int i = 0; i < errorCount; i++)
	// 		{
	// 			Console.WriteLine($"(Error #{i + 1}) ray (origin: {errorList[i].ray.origin}, direction: {errorList[i].ray.direction}, length: {errorList[i].ray.length}) hit value1 ({errorList[i].value1}) != expected ({errorList[i].expected}) (result was {errorList[i].result})");
	// 		}
	//
	// 		throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
	// 	}
	// }
}