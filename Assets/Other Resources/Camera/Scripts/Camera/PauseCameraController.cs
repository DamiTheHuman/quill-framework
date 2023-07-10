using UnityEngine;

public class PauseCameraController : MonoBehaviour
{
    [SerializeField] private RenderTexture _cachedRenderTexture;
    public bool paused;

    public static PauseCameraController instance;

    private void Awake() => instance = this;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (this._cachedRenderTexture == null)
        {
            this._cachedRenderTexture = new RenderTexture(src.width, src.height, src.depth);
        }

        if (this.paused)
        {
            Graphics.Blit(this._cachedRenderTexture, dest);
        }
        else
        {
            Graphics.CopyTexture(src, this._cachedRenderTexture);
            Graphics.Blit(src, dest);
        }
    }
}
