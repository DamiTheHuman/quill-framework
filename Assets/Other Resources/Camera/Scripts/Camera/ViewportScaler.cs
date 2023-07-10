using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class ViewportScaler : MonoBehaviour
{
    private Camera _camera;

    [Tooltip("Set the target aspect ratio.")]
    [SerializeField] private float _targetAspectRatio;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        if (Application.isPlaying)
            ScaleViewport();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (_camera)
            ScaleViewport();
#endif
    }

    private void ScaleViewport()
    {
        // determine the game window's current aspect ratio
        var windowaspect = Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        var scaleheight = windowaspect / _targetAspectRatio;

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1)
        {
            var rect = _camera.rect;

            rect.width = 1;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1 - scaleheight) / 2;

            _camera.rect = rect;
        }
        else // add pillarbox
        {
            var scalewidth = 1 / scaleheight;

            var rect = _camera.rect;

            rect.width = scalewidth;
            rect.height = 1;
            rect.x = (1 - scalewidth) / 2;
            rect.y = 0;

            _camera.rect = rect;
        }
    }

}
