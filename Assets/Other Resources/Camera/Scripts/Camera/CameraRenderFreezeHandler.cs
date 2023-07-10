using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;
/// <summary>
/// Caches the current view on the camera and displays the image untop of the camera
/// Useful when making freeze kind of effects without disabling the camera
/// </summary>
public class CameraRenderFreezeHandler : MonoBehaviour
{
    [Button(nameof(UpdateFreezeCameraImage)), SerializeField]
    private bool updateFreezeCameraImage;
    [SerializeField]
    private new Camera camera;
    [SerializeField]
    private PixelPerfectCamera pixelPerfectCamera;
    [LayerList, SerializeField, Tooltip("The layer to render the UI")]
    private int renderLayer = 5;
    [SerializeField, Tooltip("The canvas which holds our raw image")]
    private Canvas freezeRenderCanvas;
    [SerializeField, Tooltip("The raw image to display our texture on")]
    private RawImage freezeRenderRawImage;
    [SerializeField, Tooltip("The camera layer mask before rendering")]
    private LayerMask prevCameraLayerMask;

    private void Reset()
    {
        this.camera = this.GetComponent<Camera>();
        this.TryGetComponent(out this.pixelPerfectCamera);
    }

    private void Start()
    {
        if (this.camera == null)
        {
            this.Reset();
        }

        Vector3 cameraRotation;

        HedgehogCamera hedgehogCamera = HedgehogCamera.Instance();
        cameraRotation = hedgehogCamera.transform.eulerAngles;
        hedgehogCamera.transform.eulerAngles = Vector3.zero;
        this.freezeRenderCanvas = new GameObject("Freeze Render Canvas").AddComponent<Canvas>();
        this.freezeRenderCanvas.transform.SetParent(this.transform);
        this.freezeRenderCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        this.freezeRenderCanvas.sortingOrder = 999999;
        this.freezeRenderCanvas.worldCamera = this.camera;
        this.freezeRenderCanvas.gameObject.AddComponent<CanvasScaler>();
        this.freezeRenderCanvas.sortingLayerName = "HUD Layer";
        this.freezeRenderRawImage = new GameObject("Freeze Render Image").AddComponent<RawImage>();
        this.freezeRenderRawImage.transform.SetParent(this.freezeRenderCanvas.transform);
        this.freezeRenderRawImage.transform.localPosition = new Vector3(0, 0, 0);

        this.freezeRenderCanvas.transform.gameObject.layer = this.renderLayer;
        RectTransform rawImageRect = this.freezeRenderRawImage.GetComponent<RectTransform>();

        rawImageRect.anchorMin = new Vector2(0, 0);
        rawImageRect.anchorMax = new Vector2(1, 1);
        rawImageRect.pivot = new Vector2(0.5f, 0.5f);
        rawImageRect.localScale = new Vector3(1, 1, 1);
        rawImageRect.sizeDelta = new Vector2(0, 0);
        this.prevCameraLayerMask = this.camera.cullingMask;

        this.SetFreezeRenderImageVisibility(false);
        hedgehogCamera.transform.eulerAngles = cameraRotation;
    }

    private void LateUpdate()
    {
        RectTransform rawImageRect = this.freezeRenderRawImage.GetComponent<RectTransform>();
    }

    /// <summary>
    /// Sets the visibility for the freeze render image
    /// <param name="value">The display mode of the cached image </param>
    /// </summary>
    public void SetFreezeRenderImageVisibility(bool value)
    {
        this.freezeRenderCanvas.gameObject.SetActive(value);

        if (value)
        {
            //Add the render layer and disbale all other rendering
            this.prevCameraLayerMask = this.camera.cullingMask;
            this.camera.cullingMask = 0;
            HedgehogCamera.Instance().AddLayerToCullingMask(this.renderLayer);
        }
        else
        {
            HedgehogCamera.Instance().RemoveLayerFromCullingMask(this.renderLayer);
            this.camera.cullingMask = this.prevCameraLayerMask;
        }
    }

    /// <summary>
    /// Updates the freeze image based on what the camera is currently rendering
    /// </summary>
    public void UpdateFreezeCameraImage()
    {
        RenderTexture renderTexture = new RenderTexture(this.camera.pixelWidth, this.camera.pixelHeight, 24, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm)
        {
            autoGenerateMips = true
        };

        Rect previousRect = this.camera.rect;
        this.camera.targetTexture = renderTexture;

        if (this.pixelPerfectCamera != null)
        {
            this.camera.rect = previousRect;
            this.pixelPerfectCamera.enabled = false;

        }

        this.camera.Render();
        this.camera.targetTexture = null;

        this.freezeRenderRawImage.texture = renderTexture;
        this.camera.rect = previousRect;

        if (this.pixelPerfectCamera != null)
        {
            this.pixelPerfectCamera.enabled = true;
        }
    }
}
