using DoomNET.Resources;

namespace Resources;

[TestClass]
public class QuaternionTests
{
    private Random rand = new Random();

    // Value 1's
    private int value1_1; // X
    private int value1_2; // Y
    private int value1_3; // Z
    private int value1_4; // W
    // values

    // Value 2's
    private int value2_1; // X
    private int value2_2; // Y
    private int value2_3; // Z
    private int value2_4; // W
    // values

    // Minimum and maximum values for rand.Next() to use
    int randMin = -250000;
    int randMax = 250000;

    [TestMethod( "Addition" )]
    public void TestAddition()
    {
        int errCount = 0;
        List<(Quaternion value1, Quaternion value2, Quaternion result, Quaternion expected)> errors = new();

        Console.WriteLine( "Testing multiplication...\n" );

        // Do 500 tests
        for (int i = 0; i < 500; i++)
        {
            // Get a random expected addition
            Quaternion expected = new Quaternion
            (
                ( value1_1 = rand.Next( randMin, randMax ) ) + ( value2_1 = rand.Next( randMin, randMax ) ),
                ( value1_2 = rand.Next( randMin, randMax ) ) + ( value2_2 = rand.Next( randMin, randMax ) ),
                ( value1_3 = rand.Next( randMin, randMax ) ) + ( value2_3 = rand.Next( randMin, randMax ) ),
                ( value1_4 = rand.Next( randMin, randMax ) ) + ( value2_4 = rand.Next( randMin, randMax ) )
            );

            // Assign the values and calculate the result accordingly
            Quaternion value1 = new Quaternion( value1_1, value1_2, value1_3, value1_4 );
            Quaternion value2 = new Quaternion( value2_1, value2_2, value2_3, value2_4 );
            Quaternion result = value1 + value2;

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) + value2 ({value2}) = {result} (expected: {expected})" );

            if (result != expected) // Returns an error if they aren't equal!
            {
                errCount++;
                errors.Add( (value1, value2, result, expected) );
            }
        }

        // Add spacing between comments and errors
        Console.WriteLine( "\n\n" );

        if (errCount != 0)
        {
            for (int i = 0; i < errCount; i++)
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errors[ i ].value1}) + value2 ({errors[ i ].value2}) != expected ({errors[ i ].expected}) (result was {errors[ i ].result})" );
            }

            throw new Exception( $"errCount > 0 (errCount: {errCount})! This means we have at least *an* error." );
        }
    }

    [TestMethod( "Subtraction" )]
    public void TestSubtraction()
    {
        int errCount = 0;
        List<(Quaternion value1, Quaternion value2, Quaternion result, Quaternion expected)> errors = new();

        Console.WriteLine( "Testing multiplication...\n" );

        // Do 500 tests
        for (int i = 0; i < 500; i++)
        {
            // Get a random expected addition
            Quaternion expected = new Quaternion
            (
                ( value1_1 = rand.Next( randMin, randMax ) ) - ( value2_1 = rand.Next( randMin, randMax ) ),
                ( value1_2 = rand.Next( randMin, randMax ) ) - ( value2_2 = rand.Next( randMin, randMax ) ),
                ( value1_3 = rand.Next( randMin, randMax ) ) - ( value2_3 = rand.Next( randMin, randMax ) ),
                ( value1_4 = rand.Next( randMin, randMax ) ) - ( value2_4 = rand.Next( randMin, randMax ) )
            );

            // Assign the values and calculate the result accordingly
            Quaternion value1 = new Quaternion( value1_1, value1_2, value1_3, value1_4 );
            Quaternion value2 = new Quaternion( value2_1, value2_2, value2_3, value2_4 );
            Quaternion result = value1 - value2;

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) - value2 ({value2}) = {result} (expected: {expected})" );

            if (result != expected) // Returns an error if they aren't equal!
            {
                errCount++;
                errors.Add( (value1, value2, result, expected) );
            }
        }

        // Add spacing between comments and errors
        Console.WriteLine( "\n\n" );

        if (errCount != 0)
        {
            for (int i = 0; i < errCount; i++)
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errors[ i ].value1}) - value2 ({errors[ i ].value2}) != expected ({errors[ i ].expected}) (result was {errors[ i ].result})" );
            }

            throw new Exception( $"errCount > 0 (errCount: {errCount})! This means we have at least *an* error." );
        }
    }

    [TestMethod( "Multiplication" )]
    public void TestMultiplication()
    {
        int errCount = 0;
        List<(Quaternion value1, Quaternion value2, Quaternion result, Quaternion expected)> errors = new();

        Console.WriteLine( "Testing multiplication..." );

        value1_1 = rand.Next( randMin, randMax );
        value1_2 = rand.Next( randMin, randMax );
        value1_3 = rand.Next(randMin, randMax );
        value1_4 = rand.Next( randMin, randMax );

        value2_1 = rand.Next( randMin, randMax );
        value2_2 = rand.Next( randMin, randMax );
        value2_3 = rand.Next( randMin, randMax );
        value2_4 = rand.Next( randMin, randMax );

        // Do 500 tests
        for (int i = 0; i < 500; i++)
        {
            // Get a random expected addition
            Quaternion expected = new Quaternion
            (
                value1_4 * value2_1 + value1_1 * value2_4 + value1_2 * value2_3 + value1_3 * value2_2,
            );

            // Assign the values and calculate the result accordingly
            Quaternion value1 = new Quaternion( value1_1, value1_2, value1_3, value1_4 );
            Quaternion value2 = new Quaternion( value2_1, value2_2, value2_3, value2_4 );
            Quaternion result = value1 * value2;

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) * value2 ({value2}) = {result} (expected: {expected})" );

            if (result != expected) // We have an error!
            {
                errCount++;
                errors.Add( (value1, value2, result, expected) );
            }
        }

        // Add spacing between comments and errors
        Console.WriteLine( "\n\n" );

        if (errCount != 0)
        {
            for (int i = 0; i < errCount; i++)
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errors[ i ].value1}) * value2 ({errors[ i ].value2}) != expected ({errors[ i ].expected}) (result was {errors[ i ].result})" );
            }

            throw new Exception( $"errCount > 0 (errCount: {errCount})! This means we have at least *an* error." );
        }
    }
}