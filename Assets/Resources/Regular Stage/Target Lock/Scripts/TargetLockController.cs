using System.Collections;
using UnityEngine;

public class TargetLockController : MonoBehaviour, IPooledObject
{
    [SerializeField]
    private Player player;
    [SerializeField]
    private GameObject previousTarget = null;
    [SerializeField]
    private HomingAttack homingAttack;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private AudioClip targetLockSound;
    [SerializeField, Tooltip("The material of the sprite renderer")]
    private Material spriteMaterial = null;
    [SerializeField]
    private Color newTargetColor = Color.white;
    [SerializeField]
    private Color targetLockedColor = Color.red;

    private IEnumerator lerpColorOverTime;
    public void OnObjectSpawn() => this.SetPlayer(GMStageManager.Instance().GetPlayer());

    private void Reset()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.animator = this.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (this.player == null || this.homingAttack == null)
        {
            this.gameObject.SetActive(false);
            this.previousTarget = null;

            if (this.lerpColorOverTime != null)
            {
                this.spriteMaterial.SetColor("Color_To_1", this.targetLockedColor);
                this.StopCoroutine(this.lerpColorOverTime);
            }
            return;
        }

        bool shouldShowTarget = this.ShouldShowTargetLock();
        if (!shouldShowTarget)
        {
            this.spriteRenderer.enabled = false;
            this.previousTarget = null;
            if (this.lerpColorOverTime != null)
            {
                this.spriteMaterial.SetColor("Color_To_1", this.targetLockedColor);
                this.StopCoroutine(this.lerpColorOverTime);
            }

            return;
        }

        this.spriteRenderer.enabled = true;
        this.homingAttack = this.player.GetActionManager().GetAction<HomingAttack>() as HomingAttack;
        GameObject currentTarget = this.homingAttack.GetPotentialTargets()[0];

        if (this.previousTarget == null || this.previousTarget != currentTarget)
        {
            this.previousTarget = currentTarget;
            GMAudioManager.Instance().PlayOneShot(this.targetLockSound);
            this.animator.SetTrigger("TargetFound");
            if (this.lerpColorOverTime != null)
            {
                this.spriteMaterial.SetColor("Color_To_1", this.newTargetColor);
                this.StopCoroutine(this.lerpColorOverTime);
            }

            this.lerpColorOverTime = this.LerpColorsOverTime("Color_To_1", this.newTargetColor, this.targetLockedColor, 0.1f);
            this.StartCoroutine(this.lerpColorOverTime);
        }

        this.transform.position = currentTarget.transform.position;
    }

    /// <summary>
    /// Set the <see cref="player"/>
    /// </summary>
    /// <param name="player"></param>
    public void SetPlayer(Player player) => this.player = player;

    /// <summary>
    /// Set the <see cref="homingAttack"/>
    /// </summary>
    /// <param name="homingAttack"></param>
    public void SetHomingAttack(HomingAttack homingAttack) => this.homingAttack = homingAttack;

    /// <summary>
    /// Whether to show the sprite renderer or not
    /// </summary>
    /// <returns></returns>
    private bool ShouldShowTargetLock()
    {
        if (this.homingAttack.GetPotentialTargets().Count == 0)
        {
            return false;
        }

        if (this.player.GetGrounded())
        {
            return false;
        }

        if (this.homingAttack.GetPotentialTargets()[0].activeSelf == false)
        {
            return false;
        }

        if (this.player.GetActionManager().CheckActionIsBeingPerformed<Jump>() == false)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Moves the color to the specified colour within the specified time frame
    /// <param name="materialColor">The material pallete index to change </param>
    /// <param name="startingColor">The color at the beginning of the time frame </param>
    /// <param name="endingColor">The color displayed at the end of the time frame </param>
    /// <param name="time">The time frame to complete this change </param>
    /// </summary>
    private IEnumerator LerpColorsOverTime(string materialColor, Color startingColor, Color endingColor, float time)
    {
        float inversedTime = 1 / time;

        for (float step = 0.0f; step < 1.0f; step += Time.deltaTime * inversedTime)
        {
            this.spriteMaterial.SetColor(materialColor, Color.Lerp(startingColor, endingColor, step));

            yield return null;
        }

        this.spriteMaterial.SetColor("Color_To_1", this.targetLockedColor);

        yield return null;
    }


}
