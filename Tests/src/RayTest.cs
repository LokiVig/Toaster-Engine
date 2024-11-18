using DoomNET;
using DoomNET.Entities;
using DoomNET.Resources;

using SharpGen.Runtime;

namespace Resources;

[TestClass]
public class RayTest
{
    private Random rand = new Random();

    // Origin's
    private float origin_x; // X
    private float origin_y; // Y
    private float origin_z; // Z
    // values

    // Direction's
    private float direction_x; // X
    private float direction_y; // Y
    private float direction_z; // Z
    // values

    // Entity we should look for
    private Entity entity;

    // Minimum and maximum values for rand.Next(int, int) to use
    private const int RAND_MIN = -250000;
    private const int RAND_MAX = 250000;

    [TestMethod( "Trace (Vector3 & Vector3)" )]
    public void TestTrace()
    {
        int errorCount = 0;
        List<(Vector3 origin, Vector3 direction, bool result1, object result2, bool expected1, object epxected2)> errorList = new();

        Vector3 origin = Vector3.Zero;
        Vector3 direction = Vector3.Zero;

        bool result1 = false;
        object? result2 = null;

        bool expected1 = false;
        object? expected2 = null;

        Console.WriteLine( "Testing Ray.Trace (Vector3 & Vector3)..." );

        // Do 5000 tests
        for ( int i = 0; i < 5000; i++ )
        {
            Vector3 entPos = new Vector3( rand.Next( RAND_MIN, RAND_MAX ), rand.Next( RAND_MIN, RAND_MAX ), rand.Next( RAND_MIN, RAND_MAX ) );
            entity = new Entity( entPos, new BBox( new Vector3( 32, 32, 64 ), new Vector3( -32, -32, 0 ) ) );

            // Needed for Ray.Trace!
            DoomNET.DoomNET.currentScene = new Scene();
            DoomNET.DoomNET.currentScene.GetEntities().Add( entity );

            origin = new Vector3( origin_x = rand.Next( RAND_MIN, RAND_MAX ), origin_y = rand.Next( RAND_MIN, RAND_MAX ), origin_z = rand.Next( RAND_MIN, RAND_MAX ) );
            direction = new Vector3( direction_x = rand.Next( RAND_MIN, RAND_MAX ), direction_y = rand.Next( RAND_MIN, RAND_MAX ), direction_z = rand.Next( RAND_MIN, RAND_MAX ) );

            Vector3 rayEnd = ( origin + direction.Normalized() ) * 5000;

            if ( entity.GetBBox().IntersectingWith( rayEnd ) )
            {
                expected1 = true;
                expected2 = entity;
            }

            result1 = Ray.Trace( origin, direction, out result2, RayIgnore.None );

            Console.WriteLine( $"(Test #{i + 1}) value1 ({origin}) & value2 ({direction}) = {result1} && {(result2 == null ? "N/A" : result2)} (expected: {expected1} && {(expected2 == null ? "N/A" : expected2)})" );

            if ( result1 != expected1 || result2 != expected2 )
            {
                errorCount++;
                errorList.Add( (origin, direction, result1, result2, expected1, expected2) );
            }
        }

        if ( errorCount > 0 )
        {
            // Add spacing between comments and errorList
            Console.WriteLine( "\n\n" );

            for ( int i = 0; i < errorCount; i++ )
            {
                Console.WriteLine( $"(Error #{i + 1}) origin ({origin}) & direction ({direction}) != expected ({expected1} && {(expected2 == null ? "N/A" : expected2)}) (result was {result1} && {(result2 == null ? "N/A" : result2)})" );
            }

            throw new Exception( $"errorCount > 0 (errorCount: {errorCount})!" );
        }
    }
}