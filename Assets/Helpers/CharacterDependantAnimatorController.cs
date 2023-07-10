using UnityEngine;
/// <summary>
/// Updates the value of the set animator key to that of the current character
/// </summary>
public class CharacterDependantAnimatorController : MonoBehaviour
{
    [SerializeField]
    private string animatorKey = "Character";
    [SerializeField]
    private Animator animator;

    private void Reset() => this.animator = GetComponent<Animator>();

    // Start is called before the first frame update
    private void Start()
    {
        if (this.animator == null)
        {
            this.Reset();
            if (this.animator == null)
            {
                Debug.LogError("Please set an animator for this object " + this.gameObject.name);
                this.enabled = false;
            }
        }
    }

    private void Update() => this.animator.SetInteger(this.animatorKey, (int)GMCharacterManager.Instance().currentCharacter);
}
