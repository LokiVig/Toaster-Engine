using DoomNET.Resources;

namespace Resources;

[TestClass]
public class Vector3Test
{
    private Random rand = new Random();

    // Value 1's
    private float value1_x; // X
    private float value1_y; // Y
    private float value1_z; // Z
    // values

    // Value 2's
    private float value2_x; // X
    private float value2_y; // Y
    private float value2_z; // Z
    // values

    // Minimum and maximum values for rand.Next(int, int) to use
    private const int RAND_MIN = -250000;
    private const int RAND_MAX = 250000;

    [TestMethod( "Addition" )]
    public void TestAddition()
    {
        int errorCount = 0;
        List<(Vector3 value1, Vector3 value2, Vector3 result, Vector3 expected)> errorList = new();

        Console.WriteLine( "Testing addition..." );

        // Do 5000 tests
        for ( int i = 0; i < 5000; i++ )
        {
            Vector3 expected = new Vector3
                (
                    ( value1_x = rand.Next( RAND_MIN, RAND_MAX ) ) + ( value2_x = rand.Next( RAND_MIN, RAND_MAX ) ),
                    ( value1_y = rand.Next( RAND_MIN, RAND_MAX ) ) + ( value2_y = rand.Next( RAND_MIN, RAND_MAX ) ),
                    ( value1_z = rand.Next( RAND_MIN, RAND_MAX ) ) + ( value2_z = rand.Next( RAND_MIN, RAND_MAX ) )
                );

            Vector3 value1 = new Vector3( value1_x, value1_y, value1_z );
            Vector3 value2 = new Vector3( value2_x, value2_y, value2_z );
            Vector3 result = value1 + value2;

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) + value2 ({value2}) = {result} (expected: {expected})" );

            if ( result != expected )
            {
                errorCount++;
                errorList.Add( (value1, value2, result, expected) );
            }
        }

        if ( errorCount > 0 )
        {
            // Add spacing between comments and errorList
            Console.WriteLine( "\n\n" );

            for ( int i = 0; i < errorCount; i++ )
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errorList[ i ].value1}) + value2 ({errorList[ i ].value2}) != expected ({errorList[ i ].expected}) (result was {errorList[ i ].result})" );
            }

            throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
        }
    }

    [TestMethod( "Subtraction" )]
    public void TestSubtraction()
    {
        int errorCount = 0;
        List<(Vector3 value1, Vector3 value2, Vector3 result, Vector3 expected)> errorList = new();

        Console.WriteLine( "Testing subtraction..." );

        // Do 5000 tests
        for ( int i = 0; i < 5000; i++ )
        {
            Vector3 expected = new Vector3
                (
                    ( value1_x = rand.Next( RAND_MIN, RAND_MAX ) ) - ( value2_x = rand.Next( RAND_MIN, RAND_MAX ) ),
                    ( value1_y = rand.Next( RAND_MIN, RAND_MAX ) ) - ( value2_y = rand.Next( RAND_MIN, RAND_MAX ) ),
                    ( value1_z = rand.Next( RAND_MIN, RAND_MAX ) ) - ( value2_z = rand.Next( RAND_MIN, RAND_MAX ) )
                );

            Vector3 value1 = new Vector3( value1_x, value1_y, value1_z );
            Vector3 value2 = new Vector3( value2_x, value2_y, value2_z );
            Vector3 result = value1 - value2;

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) - value2 ({value2}) = {result} (expected: {expected})" );

            if ( result != expected )
            {
                errorCount++;
                errorList.Add( (value1, value2, result, expected) );
            }
        }

        if ( errorCount > 0 )
        {
            // Add spacing between comments and errorList
            Console.WriteLine( "\n\n" );

            for ( int i = 0; i < errorCount; i++ )
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errorList[ i ].value1}) - value2 ({errorList[ i ].value2}) != expected ({errorList[ i ].expected}) (result was {errorList[ i ].result})" );
            }

            throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
        }
    }

    [TestMethod( "Multiplication" )]
    public void TestMultiplication()
    {
        int errorCount = 0;
        List<(Vector3 value1, Vector3 value2, Vector3 result, Vector3 expected)> errorList = new();

        Console.WriteLine( "Testing multiplication..." );

        // Do 5000 tests
        for ( int i = 0; i < 5000; i++ )
        {
            Vector3 expected = new Vector3
                (
                    ( value1_x = rand.Next( RAND_MIN, RAND_MAX ) ) * ( value2_x = rand.Next( RAND_MIN, RAND_MAX ) ),
                    ( value1_y = rand.Next( RAND_MIN, RAND_MAX ) ) * ( value2_y = rand.Next( RAND_MIN, RAND_MAX ) ),
                    ( value1_z = rand.Next( RAND_MIN, RAND_MAX ) ) * ( value2_z = rand.Next( RAND_MIN, RAND_MAX ) )
                );

            Vector3 value1 = new Vector3( value1_x, value1_y, value1_z );
            Vector3 value2 = new Vector3( value2_x, value2_y, value2_z );
            Vector3 result = value1 * value2;

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) * value2 ({value2}) = {result} (expected: {expected})" );

            if ( result != expected )
            {
                errorCount++;
                errorList.Add( (value1, value2, result, expected) );
            }
        }

        if ( errorCount > 0 )
        {
            // Add spacing between comments and errorList
            Console.WriteLine( "\n\n" );

            for ( int i = 0; i < errorCount; i++ )
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errorList[ i ].value1}) * value2 ({errorList[ i ].value2}) != expected ({errorList[ i ].expected}) (result was {errorList[ i ].result})" );
            }

            throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
        }
    }

    [TestMethod( "Multiplication (Float)" )]
    public void TestMultiplicationFloat()
    {
        int errorCount = 0;
        List<(Vector3 value1, Vector3 value2, Vector3 result, Vector3 expected)> errorList = new();

        Console.WriteLine( "Testing multiplication..." );

        // Do 5000 tests
        for ( int i = 0; i < 5000; i++ )
        {
            Vector3 expected = new Vector3
                (
                    ( value1_x = rand.Next( RAND_MIN, RAND_MAX ) ) * ( value2_x = rand.Next( RAND_MIN, RAND_MAX ) ),
                    ( value1_y = rand.Next( RAND_MIN, RAND_MAX ) ) * ( value2_y = rand.Next( RAND_MIN, RAND_MAX ) ),
                    ( value1_z = rand.Next( RAND_MIN, RAND_MAX ) ) * ( value2_z = rand.Next( RAND_MIN, RAND_MAX ) )
                );

            Vector3 value1 = new Vector3( value1_x, value1_y, value1_z );
            Vector3 value2 = new Vector3( value2_x, value2_y, value2_z );
            Vector3 result = value1 * value2;

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) * value2 ({value2}) = {result} (expected: {expected})" );

            if ( result != expected )
            {
                errorCount++;
                errorList.Add( (value1, value2, result, expected) );
            }
        }

        if ( errorCount > 0 )
        {
            // Add spacing between comments and errorList
            Console.WriteLine( "\n\n" );

            for ( int i = 0; i < errorCount; i++ )
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errorList[ i ].value1}) * value2 ({errorList[ i ].value2}) != expected ({errorList[ i ].expected}) (result was {errorList[ i ].result})" );
            }

            throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
        }
    }

    [TestMethod( "Division" )]
    public void TestDivision()
    {
        int errorCount = 0;
        List<(Vector3 value1, Vector3 value2, Vector3 result, Vector3 expected)> errorList = new();

        Console.WriteLine( "Testing division..." );

        // Do 5000 tests
        for ( int i = 0; i < 5000; i++ )
        {
            Vector3 expected = new Vector3
                (
                    ( value1_x = rand.Next( RAND_MIN, RAND_MAX ) ) / ( value2_x = rand.Next( RAND_MIN, RAND_MAX ) ),
                    ( value1_y = rand.Next( RAND_MIN, RAND_MAX ) ) / ( value2_y = rand.Next( RAND_MIN, RAND_MAX ) ),
                    ( value1_z = rand.Next( RAND_MIN, RAND_MAX ) ) / ( value2_z = rand.Next( RAND_MIN, RAND_MAX ) )
                );

            Vector3 value1 = new Vector3( value1_x, value1_y, value1_z );
            Vector3 value2 = new Vector3( value2_x, value2_y, value2_z );
            Vector3 result = value1 / value2;

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) / value2 ({value2}) = {result} (expected: {expected})" );

            if ( result != expected )
            {
                errorCount++;
                errorList.Add( (value1, value2, result, expected) );
            }
        }

        if ( errorCount > 0 )
        {
            // Add spacing between comments and errorList
            Console.WriteLine( "\n\n" );

            for ( int i = 0; i < errorCount; i++ )
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errorList[ i ].value1}) / value2 ({errorList[ i ].value2}) != expected ({errorList[ i ].expected}) (result was {errorList[ i ].result})" );
            }

            throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
        }
    }

    [TestMethod( "Division (Float)" )]
    public void TestDivisionFloat()
    {
        int errorCount = 0;
        List<(Vector3 value1, float value2, Vector3 result, Vector3 expected)> errorList = new();

        Console.WriteLine( "Testing division..." );

        // Do 5000 tests
        for ( int i = 0; i < 5000; i++ )
        {
            value2_x = rand.Next( RAND_MIN, RAND_MAX );

            Vector3 expected = new Vector3
                (
                    ( value1_x = rand.Next( RAND_MIN, RAND_MAX ) ) / value2_x,
                    ( value1_y = rand.Next( RAND_MIN, RAND_MAX ) ) / value2_x,
                    ( value1_z = rand.Next( RAND_MIN, RAND_MAX ) ) / value2_x
                );

            Vector3 value1 = new Vector3( value1_x, value1_y, value1_z );
            float value2 = value2_x;
            Vector3 result = value1 / value2;

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) / value2 ({value2}) = {result} (expected: {expected})" );

            if ( result != expected )
            {
                errorCount++;
                errorList.Add( (value1, value2, result, expected) );
            }
        }

        if ( errorCount > 0 )
        {
            // Add spacing between comments and errorList
            Console.WriteLine( "\n\n" );

            for ( int i = 0; i < errorCount; i++ )
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errorList[ i ].value1}) / value2 ({errorList[ i ].value2}) != expected ({errorList[ i ].expected}) (result was {errorList[ i ].result})" );
            }

            throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
        }
    }

    [TestMethod( "Less Than (<)" )]
    public void TestLessThan()
    {
        int errorCount = 0;
        List<(Vector3 value1, Vector3 value2, bool result, bool expected)> errorList = new();

        Console.WriteLine( "Testing less than (<)..." );

        // Do 5000 tests
        for ( int i = 0; i < 5000; i++ )
        {
            Vector3 value1 = new Vector3( value1_x = rand.Next( RAND_MIN, RAND_MAX ), value1_y = rand.Next( RAND_MIN, RAND_MAX ), value1_z = rand.Next( RAND_MIN, RAND_MAX ) );
            Vector3 value2 = new Vector3( value2_x = rand.Next( RAND_MIN, RAND_MAX ), value2_y = rand.Next( RAND_MIN, RAND_MAX ), value2_z = rand.Next( RAND_MIN, RAND_MAX ) );

            bool expected = (float)Math.Sqrt( value1_x * value1_x + value1_y * value1_y + value1_z * value1_z ) - (float)Math.Sqrt( value2_x * value2_x + value2_y * value2_y + value2_z * value2_z ) < 0;

            bool result = value1 < value2;

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) < value2 ({value2}) ? {result} (expected: {expected})" );

            if ( result != expected )
            {
                errorCount++;
                errorList.Add( (value1, value2, result, expected) );
            }
        }

        if ( errorCount > 0 )
        {
            // Add spacing between comments and errorList
            Console.WriteLine( "\n\n" );

            for ( int i = 0; i < errorCount; i++ )
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errorList[ i ].value1}) < value2 ({errorList[ i ].value2}) != expected ({errorList[ i ].expected}) (result was {errorList[ i ].result})" );
            }

            throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
        }
    }

    [TestMethod( "Greater than (>)" )]
    public void TestGreaterThan()
    {
        int errorCount = 0;
        List<(Vector3 value1, Vector3 value2, bool result, bool expected)> errorList = new();

        Console.WriteLine( "Testing greater than (>)..." );

        // Do 5000 tests
        for ( int i = 0; i < 5000; i++ )
        {
            Vector3 value1 = new Vector3( value1_x = rand.Next( RAND_MIN, RAND_MAX ), value1_y = rand.Next( RAND_MIN, RAND_MAX ), value1_z = rand.Next( RAND_MIN, RAND_MAX ) );
            Vector3 value2 = new Vector3( value2_x = rand.Next( RAND_MIN, RAND_MAX ), value2_y = rand.Next( RAND_MIN, RAND_MAX ), value2_z = rand.Next( RAND_MIN, RAND_MAX ) );

            bool expected = (float)Math.Sqrt( value1_x * value1_x + value1_y * value1_y + value1_z * value1_z ) - (float)Math.Sqrt( value2_x * value2_x + value2_y * value2_y + value2_z * value2_z ) > 0;

            bool result = value1 > value2;

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) > value2 ({value2}) ? {result} (expected: {expected})" );

            if ( result != expected )
            {
                errorCount++;
                errorList.Add( (value1, value2, result, expected) );
            }
        }

        if ( errorCount > 0 )
        {
            // Add spacing between comments and errorList
            Console.WriteLine( "\n\n" );

            for ( int i = 0; i < errorCount; i++ )
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errorList[ i ].value1}) > value2 ({errorList[ i ].value2}) != expected ({errorList[ i ].expected}) (result was {errorList[ i ].result})" );
            }

            throw new Exception($"errorCount > 0 (errorCount: {errorCount})!");
        }
    }
}