using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A simple resusbale confirmation menu node
/// </summary>
public class MenuConfirmationController : MonoBehaviour
{
    [Tooltip("The selected user option based on the confirmation menu"), SerializeField]
    private UserOption userOption = UserOption.NotSet;
    [Tooltip("The confirmation menu for the main menu"), SerializeField]
    private GameObject confirmationMenuUI = null;
    [Tooltip("Checks if the confrimation menu is active"), SerializeField]
    private bool confrimationMenuIsActive = false;

    public GameObject firstSelectedButton;

    ///<summary> This defines what type of method you're going to call. after confirming</summary>
    public delegate void MethodToCall();
    ///<summary>This is the variable holding the method you're going to call.</summary>
    public MethodToCall confirmationMethod;
    private IEnumerator confirmationCoroutine;

    // Start is called before the first frame update
    private void Start() => this.confirmationMenuUI.SetActive(false);

    /// <summary>
    /// Sets user option to Yes called from button press
    /// </summary>
    public void OnYesPressed() => this.userOption = UserOption.Yes;

    /// <summary>
    /// Sets user option to no called from button press
    /// </summary>
    public void OnNoPressed() => this.userOption = UserOption.No;

    /// <summary>
    /// Gets the value of confrimation menu
    /// </summary>
    public bool GetConfrimationMenuIsActive() => this.confrimationMenuIsActive;

    /// <summary>
    /// Sets the value of confirmation menu is active
    /// <param name="confrimationMenuIsActive">The value of confirmation menu is active</param>
    /// </summary>
    public void SetConfirmationMenuIsActive(bool confrimationMenuIsActive) => this.confrimationMenuIsActive = confrimationMenuIsActive;

    /// <summary>
    /// Sets up the menu presets and displays the confirmation menu to ensure the user makes the right decision
    /// <param name="eventSystem">The current event system the confirmation menu is interacting with</param>
    /// <param name="onYesSelectedMethod">The method to be passed on to the coroutine and called if the user selects yes </param>
    /// <param name="onNoSelectedMethod">The method to be passed on to the coroutine and called if the user selects no </param>
    /// <param name="onEndMethod">The method to be called no matter what the result is, use this for clean up </param>
    /// </summary>
    public void SetConfirmation(EventSystem eventSystem, MethodToCall onYesSelectedMethod, MethodToCall onNoSelectedMethod = null, MethodToCall onEndMethod = null)
    {
        this.confirmationMenuUI.SetActive(true);
        this.SetConfirmationMenuIsActive(true);
        Animator buttonAnimator = this.firstSelectedButton.GetComponent<Animator>();

        if (buttonAnimator != null)
        {
            buttonAnimator.SetTrigger("Normal");
            buttonAnimator.SetTrigger("Selected");
        }

        this.confirmationCoroutine = this.HandleConfirmationMenu(onYesSelectedMethod, onNoSelectedMethod, onEndMethod);
        this.userOption = UserOption.NotSet;
        this.StartCoroutine(this.confirmationCoroutine);
        eventSystem.SetSelectedGameObject(this.firstSelectedButton);
    }

    /// <summary>
    /// Performs the delegated action by the user based on whether they select yes or no
    /// <param name="onYesSelectedMethod">The method called when the yes option is selected </param>
    /// <param name="onNoSelectedMethod">TThe method called when the no option is selected </param>
    /// <param name="onEndMethod">The method called independent of either user option</param>
    /// </summary>
    private IEnumerator HandleConfirmationMenu(MethodToCall onYesSelectMethod, MethodToCall onNoSelectedMethod = null, MethodToCall onEndMethod = null)
    {
        while (true)
        {
            if (this.userOption == UserOption.Yes)
            {
                onYesSelectMethod?.Invoke();

                break;
            }
            else if (this.userOption == UserOption.No)
            {
                onNoSelectedMethod?.Invoke();

                break;
            }

            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }

        this.confirmationMenuUI.SetActive(false);
        this.SetConfirmationMenuIsActive(false);
        this.userOption = UserOption.NotSet;
        onEndMethod?.Invoke();

        yield return null;
    }
}

