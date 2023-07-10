using UnityEngine;

public class PhantomRubyEffectController : MonoBehaviour
{
    [SerializeField]
    private PhantomRubyEffectState phantomRubyEffectState = PhantomRubyEffectState.Idle;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private AudioClip effectAudio;

    private void Reset() => this.animator = this.GetComponent<Animator>();
    // Start is called before the first frame update
    void Start()
    {
        if (this.animator == null)
        {
            this.Reset();
        }
    }

    private void FixedUpdate() => this.animator.SetInteger("State", (int)this.phantomRubyEffectState);


    /// <summary>
    /// Play the audio clip for the phantom ruby
    /// </summary>
    private void PlayEffectAudio() => GMAudioManager.Instance().PlayOneShot(this.effectAudio);
}
