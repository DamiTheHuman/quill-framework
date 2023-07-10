using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
[ExecuteInEditMode]
/// <summary>
/// Takes a picture of the current camera render that can be used for thumbnails
/// </summary>
public class ActThumbnailHelper : MonoBehaviour
{
#if UNITY_EDITOR

    [Button(nameof(CaptureCameraImageAndSaveToRendureTexture)), SerializeField]
    private bool captureCameraImage;
    [SerializeField]
    private new Camera camera;
    [SerializeField, Tooltip("The camera layer mask before rendering")]
    private LayerMask thumbnailRenderLayerMask;
    [SerializeField, Tooltip("Merged thumbnail render texture")]
    RenderTexture mergedThumbnailRenderTexture;
    [SerializeField, Tooltip("Left thumbnail render texture")]
    RenderTexture leftThumbnailRenderTexture;
    [SerializeField, Tooltip("Right thumbnail render texture")]
    RenderTexture rightThumbnailRenderTexture;
    [SerializeField, Tooltip("Time between taking the screen")]
    private int cameraScreenShotDelay = 3;
    [SerializeField, Tooltip("How much the camera moves by when taking the right side of the screenshot")]
    private int rightSideOffset = 396;
    [SerializeField, Tooltip("How much the camera moves by when taking the right side of the screenshot")]
    private string saveDirectory = "Assets/Resources/Regular Stage/Act Thumbnail Helper/Render Texture";
    [SerializeField]
    private string fileName = "Act Helper Thumbnail";


    private void Reset() => this.camera = HedgehogCamera.Instance().GetCamera();


    /// <summary>
    /// Updates the thumbnail image based on what the camera is currently rendering
    /// </summary>
    public void CaptureCameraImageAndSaveToRendureTexture() => this.StartCoroutine(this.TakeScreenshot());

    /// <summary>
    /// Take a screen shot of what the camera currently sees from the left side and then the right side
    /// </summary>
    private IEnumerator TakeScreenshot()
    {
        if (this.camera == null)
        {
            this.camera = HedgehogCamera.Instance().GetCamera();
        }

        Material material = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default"));
        CameraMode previousCameraMode = HedgehogCamera.Instance().GetCameraMode();
        LayerMask previousLayerMask = this.camera.cullingMask;
        HedgehogCamera.Instance().SetCameraMode(CameraMode.None);

        this.camera.cullingMask = this.thumbnailRenderLayerMask;

        if (Application.isPlaying)
        {
            for (int x = 0; x < this.cameraScreenShotDelay; x++)
            {
                Debug.Log("Taking left side picture in " + x + " seconds!");

                yield return new WaitForSeconds(1);
            }
        }

        Debug.Log("Left Side Screen shot taken!");

        RenderTexture leftThumbnailRenduerTexture = this.UpdateThumbnailCameraImage();
        Graphics.Blit(leftThumbnailRenduerTexture, this.leftThumbnailRenderTexture);
        Texture2D leftThumbnailTexture2D = this.ConvertRenderTextureToTexture2D(this.leftThumbnailRenderTexture);

        Graphics.Blit(leftThumbnailTexture2D, this.mergedThumbnailRenderTexture, material);//Save the left part
        HedgehogCamera.Instance().SetCameraPosition(HedgehogCamera.Instance().transform.position + new Vector3(this.rightSideOffset, 0));

        if (Application.isPlaying)
        {

            for (int x = 0; x < this.cameraScreenShotDelay; x++)
            {
                Debug.Log("Taking right side picture in " + x + " seconds!");

                yield return new WaitForSeconds(1);
            }
        }

        Debug.Log("Right Side Screen shot taken!");

        RenderTexture rightThumbnailRendurTexture = this.UpdateThumbnailCameraImage();
        Graphics.Blit(rightThumbnailRendurTexture, this.rightThumbnailRenderTexture);
        Texture2D rightThumbnailTexture = this.ConvertRenderTextureToTexture2D(this.rightThumbnailRenderTexture);

        Graphics.BlitMultiTap(rightThumbnailTexture, this.mergedThumbnailRenderTexture, material, new Vector2(this.rightSideOffset, 0));//Save the right part with a offset
        HedgehogCamera.Instance().SetCameraPosition(HedgehogCamera.Instance().transform.position - new Vector3(396, 0));

        this.SaveRenderTextureToPng(this.mergedThumbnailRenderTexture);
        HedgehogCamera.Instance().SetCameraMode(previousCameraMode);
        this.camera.cullingMask = previousLayerMask;

        yield return null;
    }

    /// <summary>
    /// Convert a render texture to texture 2D
    /// <param name="renderTexture">The render texture to convert texture2D </param>
    /// </summary
    private Texture2D ConvertRenderTextureToTexture2D(RenderTexture renderTexture)
    {
        Texture2D texture = new Texture2D(396 * 2, 240, TextureFormat.RGBA32, false);
        texture = this.AddTransparentBackgroundToTexture2D(texture);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        return texture;
    }

    /// <summary>
    /// Add a transparent background to a texture
    /// NOTE: Use this before placing items on a texture
    /// <param name="texture">The texture2D to apply a background layer to</param>
    /// </summary
    private Texture2D AddTransparentBackgroundToTexture2D(Texture2D texture)
    {
        Color fillColor = Color.clear;
        Color[] fillPixels = new Color[texture.width * texture.height];

        for (int i = 0; i < fillPixels.Length; i++)
        {
            fillPixels[i] = fillColor;
        }

        texture.SetPixels(fillPixels);

        return texture;
    }

    /// <summary>
    /// Updates the thumbnail image based on what the camera is currently rendering
    /// </summary>
    private RenderTexture UpdateThumbnailCameraImage()
    {
        RenderTexture renderTexture = new RenderTexture(this.camera.pixelWidth, this.camera.pixelHeight, 24, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm)
        {
            autoGenerateMips = true
        };
        this.camera.targetTexture = renderTexture;
        this.camera.Render();
        this.camera.targetTexture = null;

        return renderTexture;
    }

    /// <summary>
    /// Saves a render to texture to a texture 2D file at the same directory of <see cref="this.renderTexture"/>
    /// <param name="renderTexture">The render texture to save</param>
    /// </summary>
    private void SaveRenderTextureToPng(RenderTexture renderTexture)
    {
        Texture2D savedTexture = this.ConvertRenderTextureToTexture2D(renderTexture);
        Texture2D newTexture = new Texture2D(savedTexture.width, savedTexture.height, TextureFormat.ARGB32, false);
        newTexture.SetPixels(0, 0, savedTexture.width, savedTexture.height, savedTexture.GetPixels());
        newTexture.Apply();
        newTexture.wrapMode = TextureWrapMode.Repeat;
        string timeStamp = Convert.ToString((int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);

        byte[] bytes = newTexture.EncodeToPNG();
        File.WriteAllBytes(this.saveDirectory + "/" + this.fileName + "-" + timeStamp + ".png", bytes);
        DestroyImmediate(savedTexture);
        DestroyImmediate(newTexture);
        AssetDatabase.Refresh();
    }
#endif
}
