public static class Extensions
{
    public static T[] Populate<T>(this T[] array) where T : new()
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = new T();
        }
        return array;
    }
}