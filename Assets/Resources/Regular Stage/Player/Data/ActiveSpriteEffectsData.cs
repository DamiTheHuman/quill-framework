[System.Serializable]
public class ActiveSpriteEffectData : SpriteEffectsData
{
    /// <summary>
    /// Set the animator float speed multplier variable
    /// </summary>
    public void SetAnimatorSpeedMultiplier(float speed) => this.animator.SetFloat("SpeedMultiplier", General.ConvertAnimationSpeed(this.animator, speed));
}
