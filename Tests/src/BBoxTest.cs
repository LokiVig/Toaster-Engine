using DoomNET.Resources;

namespace Resources;

[TestClass]
public class BBoxTest
{
    private Random rand = new Random();

    // BBox 1's
    // maxs
    private float maxs1_x; // X
    private float maxs1_y; // Y
    private float maxs1_z; // Z
    // values,

    // mins
    private float mins1_x; // X
    private float mins1_y; // Y
    private float mins1_z; // Z
    // values

    // BBox 2's
    // maxs
    private float maxs2_x; // X
    private float maxs2_y; // Y
    private float maxs2_z; // Z
    // values,

    // mins
    private float mins2_x; // X
    private float mins2_y; // Y
    private float mins2_z; // Z
    // values

    // Minimum and maximum values for rand.Next(int, int) to use
    private const int RAND_MIN = -250000;
    private const int RAND_MAX = 250000;

    [TestMethod( "Within (Bounding Box)" )]
    public void TestWithinBoundingBox()
    {
        int errorCount = 0;
        List<(BBox value1, BBox value2, bool result, bool expected)> errorList = new();

        Console.WriteLine( "Testing within (bounding box)..." );

        // Do 5000 tests
        for (int i = 0; i < 5000; i++)
        {
            maxs1_x = rand.Next( RAND_MIN, RAND_MAX ); maxs1_y = rand.Next( RAND_MIN, RAND_MAX ); maxs1_z = rand.Next( RAND_MIN, RAND_MAX );
            mins1_x = rand.Next( RAND_MIN, RAND_MAX ); mins1_y = rand.Next( RAND_MIN, RAND_MAX ); mins1_z = rand.Next( RAND_MIN, RAND_MAX );

            maxs2_x = rand.Next( RAND_MIN, RAND_MAX ); maxs2_y = rand.Next( RAND_MIN, RAND_MAX ); maxs2_z = rand.Next( RAND_MIN, RAND_MAX );
            mins2_x = rand.Next( RAND_MIN, RAND_MAX ); mins2_y = rand.Next( RAND_MIN, RAND_MAX ); mins2_z = rand.Next( RAND_MIN, RAND_MAX );

            Vector3 maxs1 = new Vector3( maxs1_x, maxs1_y, maxs1_z );
            Vector3 mins1 = new Vector3( mins1_x, mins1_y, mins1_z );

            Vector3 maxs2 = new Vector3( maxs2_x, maxs2_y, maxs2_z );
            Vector3 mins2 = new Vector3( mins2_x, mins2_y, mins2_z );

            BBox value1 = new BBox( maxs1, mins1 );
            BBox value2 = new BBox( maxs2, mins2 );

            bool expected = ( mins1 <= maxs2 && maxs1 >= mins2 );

            bool result = value1.IntersectingWith( value2 );

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) within value2 ({value2})? {result} (expected: {expected})" );

            if (result != expected) // We have an error!
            {
                errorCount++;
                errorList.Add( (value1, value2, result, expected) );
            }
        }

        if (errorCount > 0)
        {
            // Add spacing between comments and errorList
            Console.WriteLine( "\n\n" );

            for (int i = 0; i < errorCount; i++)
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errorList[ i ].value1}) within value2 ({errorList[ i ].value2}) != expected ({errorList[ i ].expected}) (result was {errorList[ i ].result})" );
            }

            throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
        }
    }

    [TestMethod( "Within (Point)" )]
    public void WithinPoint()
    {
        int errorCount = 0;
        List<(BBox value1, Vector3 value2, bool result, bool expected)> errorList = new();

        Console.WriteLine( "Testing within (bounding box)..." );

        // Do 5000 tests
        for (int i = 0; i < 5000; i++)
        {
            maxs1_x = rand.Next( RAND_MIN, RAND_MAX ); maxs1_y = rand.Next( RAND_MIN, RAND_MAX ); maxs1_z = rand.Next( RAND_MIN, RAND_MAX );
            mins1_x = rand.Next( RAND_MIN, RAND_MAX ); mins1_y = rand.Next( RAND_MIN, RAND_MAX ); mins1_z = rand.Next( RAND_MIN, RAND_MAX );

            // We don't need mins2, so don't initialize its values
            maxs2_x = rand.Next( RAND_MIN, RAND_MAX ); maxs2_y = rand.Next( RAND_MIN, RAND_MAX ); maxs2_z = rand.Next( RAND_MIN, RAND_MAX );

            Vector3 maxs1 = new Vector3( maxs1_x, maxs1_y, maxs1_z );
            Vector3 mins1 = new Vector3( mins1_x, mins1_y, mins1_z );

            BBox value1 = new BBox( maxs1, mins1 );
            Vector3 value2 = new Vector3( maxs2_x, maxs2_y, maxs2_z );

            bool expected = value2.x >= mins1.x && value2.x <= maxs1.x &&
                            value2.y >= mins1.y && value2.y <= maxs1.y &&
                            value2.z >= mins1.z && value2.z <= maxs1.z;

            bool result = value1.IntersectingWith( value2 );

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) within value2 ({value2})? {result} (expected: {expected})" );

            if (result != expected) // We have an error!
            {
                errorCount++;
                errorList.Add( (value1, value2, result, expected) );
            }
        }

        if (errorCount > 0)
        {
            // Add spacing between comments and errorList
            Console.WriteLine( "\n\n" );

            for (int i = 0; i < errorCount; i++)
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errorList[ i ].value1}) within value2 ({errorList[ i ].value2}) != expected ({errorList[ i ].expected}) (result was {errorList[ i ].result})" );
            }

            throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
        }
    }
}