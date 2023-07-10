using System.Collections;
using UnityEngine;

public class SpecialStageHealthManager : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private SpecialStagePlayer player;
    [Tooltip("The current health status of the player"), SerializeField]
    private HealthStatus healthStatus = HealthStatus.Vulnerable;
    [SerializeField]
    [Help("The amount of rings taken on the plahyer per bomb it"), Min(0)]
    private int ringDepletionAmount = 10;
    [Tooltip("How long to flash the player sprite for"), SerializeField]
    private float flashDuration = 120f;
    [Tooltip("How many steps to update the transparency"), SerializeField]
    private float flashTransparencyUpdateStep = 4f;

    private void Reset() => this.player = this.GetComponent<SpecialStagePlayer>();
    private void Start()
    {
        if (this.player == null)
        {
            this.Reset();
        }
    }

    /// <summary>
    /// Set the health status of the player
    /// <param name="healthStatus"> the new health status of the player</param>
    /// </summary>
    public void SetHealthStatus(HealthStatus healthStatus) => this.healthStatus = healthStatus;
    /// <summary>
    /// Get the health status of the player
    /// </summary>
    public HealthStatus GetHealthStatus() => this.healthStatus;
    /// <summary>
    /// Actions performed when the player gets hit
    /// </summary>
    private void GotHit()
    {
        this.SetHealthStatus(HealthStatus.Invulnerable);
        this.StartCoroutine(this.BeginFlashing());
        if (GMSpecialStageScoreManager.Instance().GetRingCount() > 0)
        {
            GMAudioManager.Instance().PlayOneShot(this.player.GetPlayerActionAudio().ringLoss);
        }

        GMAudioManager.Instance().PlayOneShot(this.player.GetPlayerActionAudio().hurt);
        GMSpecialStageScoreManager.Instance().IncrementRingCount(-this.ringDepletionAmount);
        GMSpecialStageManager.Instance().GetSpecialStageSlider().StartDamageStall();
    }

    /// <summary>
    /// Verify the players hit based on their vulnerability
    /// </summary>
    public bool VerifyHit()
    {
        if (this.healthStatus == HealthStatus.Vulnerable)
        {
            this.GotHit();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Flash the playeres sprite
    /// </summary>
    private IEnumerator BeginFlashing()
    {
        this.player.GetSpriteController().SetTransparency(255);

        for (int x = 0; x < this.flashDuration / this.flashTransparencyUpdateStep; x++)
        {
            if (this.player.GetSpriteController().GetTransparency() != 0)
            {
                this.player.GetSpriteController().SetTransparency(0);
            }
            else
            {
                this.player.GetSpriteController().SetTransparency(255);
            }
            yield return new WaitForSeconds(General.StepsToSeconds(this.flashTransparencyUpdateStep));
        }

        this.SetHealthStatus(HealthStatus.Vulnerable);
    }
}
