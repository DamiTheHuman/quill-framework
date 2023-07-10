using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the main logic of the emeralds in special stages
/// </summary>
public class SpecialStageEmeraldController : SpecialStageContactGimmick
{
    [SerializeField]
    private BoxCollider boxCollider;
    [SerializeField]
    private List<GameObject> emeraldList = new List<GameObject>(7);
    [SerializeField, Tooltip("The position of the emerald when spawned")]
    private Transform emeraldHolder;
    [SerializeField, Tooltip("The target position of the player when the stage ends")]
    private Transform endSpecialStagePosition;
    [Tooltip("The debug colour of the collider"), SerializeField]
    private Color debugColor = new Color(1f, 0f, 1f, 0.5f);

    [SerializeField, Tooltip("The emerald created"), FirstFoldOutItem("Emerald Data")]
    private GameObject emerald;
    [SerializeField, Tooltip("The spin speed")]
    private float spinSpeed = 60;
    [SerializeField, Tooltip("The parent object of the emerald"), LastFoldoutItem()]
    private GameObject emeraldParent;
    private IEnumerator MoveSliderToEndPositionCoroutine;

    // Start is called before the first frame update
    private void Start() => this.InstantiateEmerald();

    private void FixedUpdate() => this.UpdateEmeraldRotation();

    /// <summary>
    /// Update the  rotation of the emerald
    /// </summary>
    private void UpdateEmeraldRotation()
    {
        Vector3 rotation = this.emerald.transform.localEulerAngles;
        rotation.y += this.spinSpeed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime;
        this.emerald.transform.localEulerAngles = rotation;
    }

    /// <summary>
    /// Instantiate an emerald based on the player
    /// </summary>
    private void InstantiateEmerald()
    {
        this.emerald = Instantiate(this.emeraldList[GMSpecialStageManager.Instance().GetNextEmeraldNumber()]);
        this.emerald.transform.parent = this.transform;
        this.emerald.transform.position = this.emeraldHolder.transform.position;
        this.emerald.transform.eulerAngles = this.emeraldHolder.transform.eulerAngles;
        this.emeraldHolder.gameObject.SetActive(false);
        this.emeraldParent = new GameObject("Emerald Parent");
        this.emeraldParent.transform.position = this.emerald.transform.position;
        this.emeraldParent.transform.eulerAngles = this.emerald.transform.eulerAngles;
        this.emeraldParent.transform.position = this.emerald.transform.position;
        this.emeraldParent.transform.parent = this.emeraldHolder.transform.parent;
        this.emerald.transform.parent = this.emeraldParent.transform;
    }

    /// <summary>
    /// Checks if the player is below the y of the sphere and grounded
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(SpecialStagePlayer player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;
        triggerAction = GMSpecialStageManager.Instance().GetSpecialStageState() == SpecialStageState.Running;

        return triggerAction;
    }

    /// <summary>
    /// Identify the perform action
    /// <param name="player">The player  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(SpecialStagePlayer player)
    {
        GMSpecialStageManager.Instance().SetSpecialStageState(SpecialStageState.Clear);
        GMSpecialStageManager.Instance().GetSpecialStageSlider().SetTravelSpeed(0);
        this.MoveSliderToEndPositionCoroutine = this.MoveSliderToEndPosition(player);
        player.velocity.x = 0;
        player.GetInputManager().SetInputRestriction(InputRestriction.All);
        this.StartCoroutine(this.MoveSliderToEndPositionCoroutine);
    }

    /// <summary>
    /// Move the slider closer to the end position
    /// </summary>
    private IEnumerator MoveSliderToEndPosition(SpecialStagePlayer player)
    {
        GMSpecialStageManager.Instance().GetSpecialStageSlider().SetIsMoving(true);
        GMSpecialStageManager.Instance().GetSpecialStageSlider().SlideToMiddle();
        SpecialStageSliderController specialStageSlider = GMSpecialStageManager.Instance().GetSpecialStageSlider();

        while (true)
        {
            if ((Vector2)specialStageSlider.transform.position == (Vector2)this.endSpecialStagePosition.position && GMSpecialStageManager.Instance().GetSpecialStageSlider().GetAngle() == 0)
            {
                break;
            }

            float endTravelSpeed = specialStageSlider.GetEndTravelSpeed();
            specialStageSlider.transform.position = Vector2.MoveTowards(specialStageSlider.transform.position, this.endSpecialStagePosition.position, endTravelSpeed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime);
            Vector2 playerPosition = player.transform.localPosition;
            playerPosition.x = Mathf.MoveTowards(playerPosition.x, 0, endTravelSpeed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime);
            player.transform.localPosition = playerPosition;

            yield return new WaitForFixedUpdate();
        }

        GMSpecialStageManager.Instance().GetSpecialStageSlider().SetIsMoving(false);
        GMSpecialStageManager.Instance().SpecialStageActClear();

        yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = this.debugColor;
        Gizmos.DrawCube(this.boxCollider.bounds.center, this.boxCollider.bounds.size);
    }
}
