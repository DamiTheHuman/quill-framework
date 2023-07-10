using UnityEngine;
/// <summary>
/// The controller class for a hidden spring
/// </summary>
public class HiddenSpringController : SpringController
{
    [Tooltip("How Much offset is added on play so the spring appears hidden while collission stays intact"), FirstFoldOutItem("Hidden Spring Properties"), LastFoldoutItem(), SerializeField]
    private Vector2 hiddenSpringOffset = new Vector2(0, 0);
    public override void Reset()
    {
        base.Reset();
        this.gameObject.layer = 15;
    }

    protected override void Start()
    {
        this.gameObject.layer = 15;
        this.boxCollider2D.offset += this.hiddenSpringOffset;
        base.Start();
    }
}
