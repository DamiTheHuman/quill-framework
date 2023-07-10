using System.Collections.Generic;
/// <summary>
/// Used to perform fast comparisons between objects
/// </summary>
internal class FastComparable : IEqualityComparer<int>
{
    public static FastComparable Default = new FastComparable();
    /// <summary>
    /// Check if the objects are the same
    /// <param name="x"> The first integer </param>
    /// <param name="y"> The second integer </param>
    /// </summary>
    public bool Equals(int x, int y) => x == y;
    /// <summary>
    /// Get the hashcode of an object
    /// <param name="obj"> the object to be retrieve the hashcode from</param>
    /// </summary>
    public int GetHashCode(int obj) => obj.GetHashCode();
}
