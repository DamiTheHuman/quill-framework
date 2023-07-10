using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class General : MonoBehaviour
{
    /// <summary>
    /// Converts RGB to basic color e.g 255 = 1
    /// <param name="red">The red value of the color</param>
    /// <param name="blue">The blue value of the color</param>
    /// <param name="green">The green value of the color</param>
    /// <param name="alpha">The transparency of the color</param>
    /// </summary>
    public static Color RGBToColour(int red, int green, int blue, int alpha = 255) => new Color(red / 255f, green / 255f, blue / 255f, alpha / 255f);

    /// <summary>
    /// Calculate the position of a object based on the angle given to it
    /// <param name="position">The current position of the object player </param>
    /// <param name="angleInRadians">The angle of the object</param>
    /// <param name="extraOffset">Offsets applied to the position x and y axis</param>
    /// </summary>
    public static Vector2 CalculateAngledObjectPosition(Vector2 position, float angleInRadians, Vector2 extraOffset)
    {
        float lineXPosition = position.x + (Mathf.Cos(angleInRadians) * extraOffset.x) + (Mathf.Sin(angleInRadians) * -extraOffset.y);
        float lineYPosition = position.y + (Mathf.Cos(angleInRadians) * extraOffset.y) + (Mathf.Sin(angleInRadians) * extraOffset.x);

        return new Vector2(lineXPosition, lineYPosition);
    }

    /// <summary>
    /// Converts Vector 2 to an angle
    /// <param name="vector">The vector2 to convert</param>
    /// </summary>
    public static float Vector2ToAngle(Vector2 vector)
    {
        float angle = Mathf.Atan2(vector.y, vector.x) - (Mathf.PI / 2f);

        if (angle < 0f)
        {
            angle += Mathf.PI * 2f;
        }

        return angle;
    }

    /// <summary>
    /// Returns a unit vector pointing in the specified direction.
    /// </summary>
    /// <param name="angleInRadians">The specified angle, in radians.</param>
    /// </summary>
    public static Vector2 AngleToVector(float angleInRadians) => new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

    /// <summary>
    /// Rounds the value of a float to the specified decimal places
    /// <param name="valueToConvert">The value to round</param>
    /// <param name="decimalPlaces">The decimal place value to convert to</param>
    /// </summary> 
    public static float RoundToDecimalPlaces(float valueToConvert, int decimalPlaces = 2)
    {
        double mult = Mathf.Pow(10.0f, decimalPlaces);
        double result = Math.Truncate(mult * valueToConvert) / mult;

        return (float)result;
    }

    /// <summary>
    /// Calculates whether a vector is to the left or riht of another vector
    /// <param name="directionA">Vector to check as base</param>
    /// <param name="directionB">Vector to check again</param>
    /// </summary> 
    public static int ObjectDirectionHorizontally(Vector2 directionA, Vector2 directionB)
    {
        if (directionB.x >= directionA.x)
        {
            return 1;
        }

        return -1;
    }

    /// <summary>
    /// Rounds the value of a float to nearest decimal value
    /// <param name="valueToConvert">The value to round</param>
    /// <param name="decimalPlaces">The decimal place value to convert to</param>
    /// </summary> 
    public static float RoundToNearestDigit(float valueToConvert, int value) => Mathf.Round(valueToConvert / value) * value;

    /// <summary>
    /// Calculate the absolute difference between two angles
    /// <param name="angle2">The first angle</param>
    /// <param name="angle1">The second angle</param>
    /// </summary> 
    public static float DifferenceBetween2Angles(float angle1, float angle2)
    {
        float dif = (float)Math.Abs(angle1 - angle2) % 360;

        if (dif > 180)
        {
            dif = 360 - dif;
        }

        return dif;
    }

    /// <summary>
    /// Calculate the angle in degrees between two points
    /// <param name="vec1">The first point</param>
    /// <param name="vec2">The second point</param>
    /// </summary> 
    public static float AngleBetweenPointsInDegrees(Vector3 vec1, Vector3 vec2) => AnglesBetweenPointsInRadians(vec1, vec2) * 180 / Mathf.PI;

    /// <summary>
    /// Calculate the angle in radians between two points
    /// <param name="vec1">The first point</param>
    /// <param name="vec2">The second point</param>
    /// </summary> 
    public static float AnglesBetweenPointsInRadians(Vector3 vec1, Vector3 vec2) => Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x);

    /// <summary>
    /// Calculate the angle between two Vecotr 2s
    /// <param name="vec1">The first point</param>
    /// <param name="vec2">The second point</param>
    /// </summary> 
    public static float AngleBetweenVector2(Vector2 vec1, Vector2 vec2) => Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x);

    /// <summary>
    /// Calculate the angle between two polygon points
    /// <param name="vec1">The first point</param>
    /// <param name="vec2">The second point</param>
    /// </summary> 
    public static float AngleBetweenPolygonPoints(Vector2 vec1, Vector2 vec2)
    {
        Vector2 diference = vec2 - vec1;
        float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;

        return Vector2.Angle(Vector2.right, diference) * sign;
    }

    /// <summary>
    /// Converts From Steps to Seconds
    /// <param name="timeInSteps">The time in steps</param>
    /// </summary>
    public static float StepsToSeconds(float timeInSteps)
    {
        float oneSecondInSteps = 59.9999995313f;
        float seconds = timeInSteps / oneSecondInSteps;

        return seconds;
    }

    /// <summary>
    /// Checks if a layermask contains the specified layer
    /// <param name="layerMask">The  active layermask containing layers</param>
    /// <param name="layer">The layer to check for </param>
    /// </summary>
    public static bool ContainsLayer(LayerMask layerMask, int layer) => layerMask == (layerMask | (1 << layer));

    /// <summary>
    /// Checks if the angle is zero
    /// <param name="angleInDegrees">The time in steps</param>
    /// </summary>
    public static bool CheckIfAngleIsZero(float angleInDegrees)
    {
        if (Mathf.Round(angleInDegrees) == 0 || Math.Round(angleInDegrees) == 360)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Converts an Angle to Vector2  From a Transform
    /// </summary>
    public static Vector2 TransformEulerToVector2(float angle, Transform transform, float extraOffsetAngle = 0)
    {
        Vector3 eulerAngle = transform.eulerAngles;
        eulerAngle.z -= extraOffsetAngle;
        eulerAngle.z = RoundToDecimalPlaces(eulerAngle.z, 1);
        Vector2 result = (Vector2)(Quaternion.Euler(eulerAngle) * Vector2.up) * angle;
        result.x = RoundToDecimalPlaces(result.x, 1);
        result.y = RoundToDecimalPlaces(result.y, 1);

        return result;
    }

    /// <summary>
    /// Checks if an angle in degrees is within the specified range clockwise and counter clockwise
    /// <param name="angleToBeChecked">The current angle being checked</param>
    /// <param name="condition1">The first angle to check against</param>
    /// <param name="condition2">The second angle to check against</param>
    /// </summary>
    public static bool CheckAngleIsWithinRange(float angleToBeChecked, float condition1, float condition2)
    {
        float invertedCondition1 = 360 - condition1;
        float invertedCondition2 = 360 - condition2;

        return (angleToBeChecked > condition1 && angleToBeChecked < condition2) || (angleToBeChecked > invertedCondition2 && angleToBeChecked < invertedCondition1);
    }

    /// <summary>
    /// Checks if an angle in degrees is within or equals to the specified range clockwise and counter clockwise
    /// <param name="angleToBeChecked">The current angle being checked</param>
    /// <param name="condition1">The first angle to check against</param>
    /// <param name="condition2">The second angle to check against</param>
    /// </summary>
    public static bool CheckAngleIsWithinOrEqualRange(float angleToBeChecked, float condition1, float condition2)
    {
        float invertedCondition1 = 360 - condition1;
        float invertedCondition2 = 360 - condition2;

        return (angleToBeChecked >= condition1 && angleToBeChecked <= condition2) || (angleToBeChecked >= invertedCondition2 && angleToBeChecked <= invertedCondition1);
    }

    /// <summary>
    /// Applies an empty space to uppercase characters found in a string
    /// <param name="string">The string to convert</param>
    /// </summary>
    public static string TransformSpacesToUpperCaseCharacters(string @string) => string.Concat(@string.Select(x => char.IsUpper(x) || char.IsDigit(x) ? " " + x : x.ToString())).TrimStart(' ');

    /// <summary>
    /// Converts a vector 3 objects values to seconds
    /// <param name="time">The current time in Vector 3 representing x = minutes, y = seconds and z = fraction</param>
    /// </summary>
    public static float ConvertVector3TimeToSeconds(Vector3 time)
    {
        time.z = (int)time.z;
        time.y = (int)time.y;
        int minutes = (int)time.x;
        string seconds = time.y.ToString();
        string fraction = time.z.ToString();
        float value = float.Parse(seconds + "." + fraction);
        value += minutes * 60;

        return value;
    }

    /// <summary>
    /// Performs a copy of a component and its attributes unto another game object
    /// <param name="original">The original component to be cloned</param>
    /// <param name="destination">The game object to set the clone to</param>
    /// </summary>
    public static Component CopyComponent(Component original, GameObject destination)
    {
        if (original == null)
        {
            return null;
        }

        Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        FieldInfo[] fields = type.GetFields();

        foreach (FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }

        return copy;
    }

    /// <summary>
    /// Quick script to allow speed values to be set disregarding sample rate
    /// This conversion also ignores the sample rate
    /// </summary>
    public static float ConvertAnimationSpeed(Animator animator, float gameSpeed)
    {
        float sampleRate = animator.GetCurrentAnimatorClipInfo(0).Length > 0 ? animator.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate : 30;

        return gameSpeed / 50 * 30 / sampleRate;
    }

    /// <summary>
    /// Limits the value of a degree angle between 0 and 360
    /// </summary>
    public static float ClampDegreeAngles(float angleInDegrees)
    {
        if (angleInDegrees >= 360)
        {
            angleInDegrees -= 360;
        }
        else if (angleInDegrees <= 0)
        {
            angleInDegrees += 360;
        }

        return angleInDegrees;
    }

    /// <summary>
    /// Cycle to the next value in an enum based on the direction
    /// </summary>
    public static T CycleEnumValue<T>(T e, int direction = 1)
    {
        T[] all = (T[])Enum.GetValues(typeof(T));
        int i = Array.IndexOf(all, e);

        if (i < 0)
        {
            throw new System.ComponentModel.InvalidEnumArgumentException();
        }

        if (i == all.Length - 1 && direction == 1)
        {
            return all[0];
        }
        else if (i == 0 && direction == -1)
        {
            return all[all.Length - 1];
        }

        return all[i + direction];
    }

    /// <summary>
    /// Logs the exception thrown/ error code in the console in a readable format
    /// <param name="exception">The exception thrown</param>
    /// <param name="errorCode">The error code of the exception</param>
    /// </summary>
    public static void LogErrorMessage(Exception exception, ErrorCode errorCode = ErrorCode.Generic, string message = "") => Debug.LogError("Error Code: " + (int)errorCode + (message != "" ? " Message:" + message : "") + " Exception: " + exception.Message);

    /// <summary>
    /// Gets a game object from the prefab utility 
    /// <param name="go">The game object to fetch/param>
    /// </summary>
    public static GameObject GetPrefabAsset(GameObject go)
    {
        GameObject gameObject = null;
#if UNITY_EDITOR
        string pathToPrefab = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go);
        gameObject = PrefabUtility.GetCorrespondingObjectFromSourceAtPath(go, pathToPrefab);

        //Check up the parent of the prefab to get the most top root component
        if (gameObject != null && gameObject.transform.parent != null)
        {
            int parentsTraversed = 0;
            Transform currentParent = gameObject.transform.parent;
            while (parentsTraversed < 1000)
            {
                if (currentParent.transform.parent != null)
                {
                    currentParent = currentParent.transform.parent;
                }
                else
                {
                    break;
                }
                parentsTraversed++;

                if (parentsTraversed == 1000)
                {
                    Debug.Log("More than 1000 nesting on a prefab, is this intentional?");
                    return gameObject;
                }
            }

            if (currentParent != null)
            {
                return PrefabUtility.GetCorrespondingObjectFromOriginalSource(currentParent.gameObject);
            }

        }
#endif
        return gameObject ? gameObject : go;
    }

    /// <summary>
    /// Force the editor to save the changes
    /// </summary>
    public static void SetDirty(UnityEngine.Object gameObject)
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(gameObject);
#endif
    }
}
