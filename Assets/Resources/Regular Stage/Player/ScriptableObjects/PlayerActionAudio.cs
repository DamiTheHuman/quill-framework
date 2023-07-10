using UnityEngine;
/// <summary>
/// A scriptable object which stores information about the audio when relating to the action
/// Credit - to MarmitoTH for the concept;
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerActionAudio", order = 1)]
public class PlayerActionAudio : ScriptableObject
{
    public AudioClip jump;
    public AudioClip roll;
    public AudioClip spindashCharge;
    public AudioClip spindashRelease;
    public AudioClip dropDash;
    public AudioClip skidding;
    public AudioClip hurt;
    public AudioClip spikeHurt;
    public AudioClip ringLoss;
    public AudioClip land;
    public AudioClip grab;
    public AudioClip gulp;
    public AudioClip drowned;
    public AudioClip superForm;
    public AudioClip homingAttack;
}
