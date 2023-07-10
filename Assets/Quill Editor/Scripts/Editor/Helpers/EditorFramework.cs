using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class EditorFramework
{
    internal static bool needToRepaint;
    internal static Event currentEvent;
    internal static float time;

    static EditorFramework() => EditorApplication.update += Updating;

    /// <summary>
    ///  Updating the editor based on conditions
    /// </summary>
    private static void Updating()
    {
        CheckMouse();

        if (needToRepaint)
        {
            time += Time.deltaTime;

            if (time >= 0.3f)
            {
                time -= 0.3f;
                //Unset the repaint flag
                needToRepaint = false;
            }
        }
    }

    /// <summary>
    ///  Keep track of the current mosue movement
    /// </summary>
    private static void CheckMouse()
    {
        Event ev = currentEvent;

        if (ev == null)
        {
            return;
        }
        //If the mouse moves repaint
        if (ev.type == EventType.MouseMove)
        {
            needToRepaint = true;
        }
    }
}