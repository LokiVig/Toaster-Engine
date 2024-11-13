using DoomNET.Resources;

namespace Resources;

[TestClass]
public class QuaternionTests
{
    private Random rand = new Random();

    // Value 1's
    private float value1_x; // X
    private float value1_y; // Y
    private float value1_z; // Z
    private float value1_w; // W
    // values

    // Value 2's
    private float value2_x; // X
    private float value2_y; // Y
    private float value2_z; // Z
    private float value2_w; // W
    // values

    // Minimum and maximum values for rand.Next(int, int) to use
    int randMin = -250000;
    int randMax = 250000;

    [TestMethod( "Addition" )]
    public void TestAddition()
    {
        int errorCount = 0;
        List<(Quaternion value1, Quaternion value2, Quaternion result, Quaternion expected)> errorList = new();

        Console.WriteLine( "Testing addition...\n" );

        // Do 500 tests
        for (int i = 0; i < 500; i++)
        {
            // Get a random expected addition
            Quaternion expected = new Quaternion
            (
                ( value1_x = rand.Next( randMin, randMax ) ) + ( value2_x = rand.Next( randMin, randMax ) ),
                ( value1_y = rand.Next( randMin, randMax ) ) + ( value2_y = rand.Next( randMin, randMax ) ),
                ( value1_z = rand.Next( randMin, randMax ) ) + ( value2_z = rand.Next( randMin, randMax ) ),
                ( value1_w = rand.Next( randMin, randMax ) ) + ( value2_w = rand.Next( randMin, randMax ) )
            );

            // Assign the values and calculate the result accordingly
            Quaternion value1 = new Quaternion( value1_x, value1_y, value1_z, value1_w );
            Quaternion value2 = new Quaternion( value2_x, value2_y, value2_z, value2_w );
            Quaternion result = value1 + value2;

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) + value2 ({value2}) = {result} (expected: {expected})" );

            if (result != expected) // Returns an error if they aren't equal!
            {
                errorCount++;
                errorList.Add( (value1, value2, result, expected) );
            }
        }

        // Add spacing between comments and errorList
        Console.WriteLine( "\n\n" );

        if (errorCount != 0)
        {
            for (int i = 0; i < errorCount; i++)
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errorList[ i ].value1}) + value2 ({errorList[ i ].value2}) != expected ({errorList[ i ].expected}) (result was {errorList[ i ].result})" );
            }

            throw new Exception( $"errorCount > 0 (errorCount: {errorCount})! This means we have at least *an* error." );
        }
    }

    [TestMethod( "Subtraction" )]
    public void TestSubtraction()
    {
        int errorCount = 0;
        List<(Quaternion value1, Quaternion value2, Quaternion result, Quaternion expected)> errorList = new();

        Console.WriteLine( "Testing subtraction...\n" );

        // Do 500 tests
        for (int i = 0; i < 500; i++)
        {
            // Get a random expected addition
            Quaternion expected = new Quaternion
            (
                ( value1_x = rand.Next( randMin, randMax ) ) - ( value2_x = rand.Next( randMin, randMax ) ),
                ( value1_y = rand.Next( randMin, randMax ) ) - ( value2_y = rand.Next( randMin, randMax ) ),
                ( value1_z = rand.Next( randMin, randMax ) ) - ( value2_z = rand.Next( randMin, randMax ) ),
                ( value1_w = rand.Next( randMin, randMax ) ) - ( value2_w = rand.Next( randMin, randMax ) )
            );

            // Assign the values and calculate the result accordingly
            Quaternion value1 = new Quaternion( value1_x, value1_y, value1_z, value1_w );
            Quaternion value2 = new Quaternion( value2_x, value2_y, value2_z, value2_w );
            Quaternion result = value1 - value2;

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) - value2 ({value2}) = {result} (expected: {expected})" );

            if (result != expected) // Returns an error if they aren't equal!
            {
                errorCount++;
                errorList.Add( (value1, value2, result, expected) );
            }
        }

        // Add spacing between comments and errorList
        Console.WriteLine( "\n\n" );

        if (errorCount != 0)
        {
            for (int i = 0; i < errorCount; i++)
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errorList[ i ].value1}) - value2 ({errorList[ i ].value2}) != expected ({errorList[ i ].expected}) (result was {errorList[ i ].result})" );
            }

            throw new Exception( $"errorCount > 0 (errorCount: {errorCount})! This means we have at least *an* error." );
        }
    }

    [TestMethod( "Multiplication" )]
    public void TestMultiplication()
    {
        int errorCount = 0;
        List<(Quaternion value1, Quaternion value2, Quaternion result, Quaternion expected)> errorList = new();

        Console.WriteLine( "Testing multiplication..." );

        // Do 500 tests
        for (int i = 0; i < 500; i++)
        {
            // Predefine the values, as to not randomize the values every call in the expected value
            value1_x = rand.Next( randMin, randMax );
            value1_y = rand.Next( randMin, randMax );
            value1_z = rand.Next( randMin, randMax );
            value1_w = rand.Next( randMin, randMax );

            value2_x = rand.Next( randMin, randMax );
            value2_y = rand.Next( randMin, randMax );
            value2_z = rand.Next( randMin, randMax );
            value2_w = rand.Next( randMin, randMax );

            // Get a random expected multiplication
            Quaternion expected = new Quaternion
            (
                value1_w * value2_x + value1_x * value2_w + value1_y * value2_z - value1_z * value2_y,
                value1_w * value2_w - value1_x * value2_z + value1_y * value2_w + value1_z * value2_x,
                value1_w * value2_z + value1_x * value2_y - value1_y * value2_x + value1_z * value2_w,
                value1_w * value2_w - value1_x * value2_x - value1_y * value2_y - value1_z * value2_z
            );

            // Assign the values and calculate the result accordingly
            Quaternion value1 = new Quaternion( value1_x, value1_y, value1_z, value1_w );
            Quaternion value2 = new Quaternion( value2_x, value2_y, value2_z, value2_w );
            Quaternion result = value1 * value2;

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) * value2 ({value2}) = {result} (expected: {expected})" );

            if (result != expected) // We have an error!
            {
                errorCount++;
                errorList.Add( (value1, value2, result, expected) );
            }
        }

        // Add spacing between comments and errorList
        Console.WriteLine( "\n\n" );

        if (errorCount != 0)
        {
            for (int i = 0; i < errorCount; i++)
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errorList[ i ].value1}) * value2 ({errorList[ i ].value2}) != expected ({errorList[ i ].expected}) (result was {errorList[ i ].result})" );
            }

            throw new Exception( $"errorCount > 0 (errorCount: {errorCount})! This means we have at least *an* error." );
        }
    }

    [TestMethod( "Multiplication (Float)" )]
    public void TestMultiplicationFloat()
    {
        int errorCount = 0;
        List<(Quaternion value1, float value2, Quaternion result, Quaternion expected)> errorList = new();

        Console.WriteLine( "Testing multiplication against floats..." );

        // Do 500 tests
        for (int i = 0; i < 500; i++)
        {
            // Predefine the values, as to not randomize the values every call in the expected value
            value1_x = rand.Next( randMin, randMax );
            value1_y = rand.Next( randMin, randMax );
            value1_z = rand.Next( randMin, randMax );
            value1_w = rand.Next( randMin, randMax );

            // Since this is just supposed to test against a float, let's only define the x variable,
            // to save on memory (if that even happens)
            value2_x = rand.Next( randMin, randMax );

            // Get a random expected multiplication
            Quaternion expected = new Quaternion
            (
                value1_x * value2_x,
                value1_y * value2_x,
                value1_z * value2_x,
                value1_w * value2_x
            );

            // Assign the values and calculate the result accordingly
            Quaternion value1 = new Quaternion( value1_x, value1_y, value1_z, value1_w );
            float value2 = value2_x;
            Quaternion result = value1 * value2;

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) * value2 ({value2}) = {result} (expected: {expected})" );

            if (result != expected) // We have an error!
            {
                errorCount++;
                errorList.Add( (value1, value2, result, expected) );
            }
        }

        // Add spacing between comments and errorList
        Console.WriteLine( "\n\n" );

        if (errorCount != 0)
        {
            for (int i = 0; i < errorCount; i++)
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errorList[ i ].value1}) * value2 ({errorList[ i ].value2}) != expected ({errorList[ i ].expected}) (result was {errorList[ i ].result})" );
            }

            throw new Exception( $"errorCount > 0 (errorCount: {errorCount})! This means we have at least *an* error." );
        }
    }

    [TestMethod( "Division" )]
    public void TestDivision()
    {
        int errorCount = 0;
        List<(Quaternion value1, Quaternion value2, Quaternion result, Quaternion expected)> errorList = new();

        Console.WriteLine( "Testing division..." );

        // Do 500 tests
        for (int i = 0; i < 500; i++)
        {
            // Predefine the values, as to not randomize the values every call in the expected value
            value1_x = rand.Next( randMin, randMax );
            value1_y = rand.Next( randMin, randMax );
            value1_z = rand.Next( randMin, randMax );
            value1_w = rand.Next( randMin, randMax );

            value2_x = rand.Next( randMin, randMax );
            value2_y = rand.Next( randMin, randMax );
            value2_z = rand.Next( randMin, randMax );
            value2_w = rand.Next( randMin, randMax );

            // Get a random expected multiplication
            Quaternion expected = new Quaternion
            (
                value1_w * -value2_x + value1_x * -value2_w + value1_y * -value2_z - value1_z * -value2_y,
                value1_w * -value2_w - value1_x * -value2_z + value1_y * -value2_w + value1_z * -value2_x,
                value1_w * -value2_z + value1_x * -value2_y - value1_y * -value2_x + value1_z * -value2_w,
                value1_w * -value2_w - value1_x * -value2_x - value1_y * -value2_y - value1_z * -value2_z
            );

            // Assign the values and calculate the result accordingly
            Quaternion value1 = new Quaternion( value1_x, value1_y, value1_z, value1_w );
            Quaternion value2 = new Quaternion( value2_x, value2_y, value2_z, value2_w );
            Quaternion result = value1 / value2;

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) / value2 ({value2}) = {result} (expected: {expected})" );

            if (result != expected) // We have an error!
            {
                errorCount++;
                errorList.Add( (value1, value2, result, expected) );
            }
        }

        // Add spacing between comments and errorList
        Console.WriteLine( "\n\n" );

        if (errorCount != 0)
        {
            for (int i = 0; i < errorCount; i++)
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errorList[ i ].value1}) / value2 ({errorList[ i ].value2}) != expected ({errorList[ i ].expected}) (result was {errorList[ i ].result})" );
            }

            throw new Exception( $"errorCount > 0 (errorCount: {errorCount})! This means we have at least *an* error." );
        }
    }

    [TestMethod( "Division (Float)" )]
    public void TestDivisionFloat()
    {
        int errorCount = 0;
        List<(Quaternion value1, float value2, Quaternion result, Quaternion expected)> errorList = new();

        Console.WriteLine( "Testing division against floats..." );

        // Do 500 tests
        for (int i = 0; i < 500; i++)
        {
            // Predefine the values, as to not randomize the values every call in the expected value
            value1_x = rand.Next( randMin, randMax );
            value1_y = rand.Next( randMin, randMax );
            value1_z = rand.Next( randMin, randMax );
            value1_w = rand.Next( randMin, randMax );

            // Since this is just supposed to test against a float, let's only define the x variable,
            // to save on memory (if that even happens)
            value2_x = rand.Next( randMin, randMax );

            // Get a random expected multiplication
            Quaternion expected = new Quaternion
            (
                value1_x / value2_x,
                value1_y / value2_x,
                value1_z / value2_x,
                value1_w / value2_x
            );

            // Assign the values and calculate the result accordingly
            Quaternion value1 = new Quaternion( value1_x, value1_y, value1_z, value1_w );
            float value2 = value2_x;
            Quaternion result = value1 / value2;

            Console.WriteLine( $"(Test #{i + 1}) value1 ({value1}) / value2 ({value2}) = {result} (expected: {expected})" );

            if (result != expected) // We have an error!
            {
                errorCount++;
                errorList.Add( (value1, value2, result, expected) );
            }
        }

        // Add spacing between comments and errorList
        Console.WriteLine( "\n\n" );

        if (errorCount != 0)
        {
            for (int i = 0; i < errorCount; i++)
            {
                Console.WriteLine( $"(Error #{i + 1}) value1 ({errorList[ i ].value1}) / value2 ({errorList[ i ].value2}) != expected ({errorList[ i ].expected}) (result was {errorList[ i ].result})" );
            }

            throw new Exception( $"errorCount > 0 (errorCount: {errorCount})! This means we have at least *an* error." );
        }
    }
}