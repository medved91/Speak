namespace Speak.Extensions;

public static class RandomExtensions
{
    /// <summary>
    /// Перемешать элементы массива
    /// </summary>
    public static void Shuffle<T> (this T[] array)
    {
        var random = new Random();
        
        var n = array.Length;
        while (n > 1) 
        {
            var k = random.Next(n--);
            (array[n], array[k]) = (array[k], array[n]);
        }
    }
}