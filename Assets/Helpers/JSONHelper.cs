using System;
using System.Text;

/// <summary>
/// Helper to perform operations on JSON Files
/// </summary>
public class JSONHelper
{
    /// <summary>
    /// Encodes our JSON data
    /// </summary>
    public static string EncodeBase64JSONData(string jsonData) => Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonData));

    /// <summary>
    /// Decode our JSON data
    /// </summary>
    public static string DecodeBase64JSONData(string encodedData) => Encoding.UTF8.GetString(Convert.FromBase64String(encodedData));
}
