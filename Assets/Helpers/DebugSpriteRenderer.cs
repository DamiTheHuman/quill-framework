using UnityEngine;

public class DebugSpriteRenderer : MonoBehaviour
{
    public Sprite[] spriteArray;
    public int index = 0;
    public bool startTimer = false;
    public bool endTimer = false;
    public float timer;
    public float animationSpeed = 0.4258597f;

    // Start is called before the first frame update
    private void Start() => Application.targetFrameRate = 60;

    // Update is called once per frame
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.index--;
            if (this.startTimer)
            {
                //Debug.Log(this.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.frameRate);
                //this.GetComponent<Animator>().speed = animationSpeed;
                this.GetComponent<Animator>().Play("Test");

            }
            else
            {
                //this.GetComponent<Animator>().speed = animationSpeed;
                this.GetComponent<Animator>().Play("Test 2");
                //frameRate / desiredFramerate

            }
            this.startTimer = !this.startTimer;

            this.timer = this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        }
        if (this.startTimer)
        {
        }
        //timer += Time.deltaTime;

        //1/100*60
    }

}
