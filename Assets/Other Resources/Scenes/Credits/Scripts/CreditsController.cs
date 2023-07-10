using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the credits scene
/// </summary>
public class CreditsController : MonoBehaviour
{
    [SerializeField, Tooltip("The JSON file with the credits content"), Help("Please ensure the file inserted is JSON")]
    private TextAsset creditsFile;
    [SerializeField, Tooltip("The credits content")]
    private CreditData credits;
    [SerializeField, Tooltip("The current slide")]
    private int currentSlide = 0;
    [SerializeField, Tooltip("The time between each slide")]
    private float slideInterval = 3;

    [SerializeField, Tooltip("The title for each slide element"), FirstFoldOutItem("Slide Canvas")]
    private TextMeshProUGUI slideTitle;
    [SerializeField, Tooltip("The slide body parent")]
    private GameObject slideBodyLayoutGroup;
    [SerializeField, Tooltip("The prefab for all the slide body text"), LastFoldoutItem()]
    private GameObject slideBodyPrefab;

    private void Start()
    {
        this.ParseCreditsFile();
        this.currentSlide = 0;
        this.LoadSlide(this.currentSlide);
        this.StartCoroutine(this.CycleCreditSlides());
    }

    /// <summary>
    /// Parse the <see cref="creditsFile"/> json file to an array
    /// </summary>
    private void ParseCreditsFile()
    {
        if (this.creditsFile == null)
        {
            Debug.Log("NO CREDITS FILE SET!");
            GMSceneManager.Instance().LoadNextScene();
            return;
        }
        this.credits = JsonUtility.FromJson<CreditData>(this.creditsFile.ToString());
    }

    /// <summary>
    /// Loads the next slide data for the credits file
    /// </summary>
    private void LoadSlide(int slide)
    {
        if (slide > this.credits.credits.Length - 1)
        {
            return;
        }

        CreditSlideData currentSlide = this.credits.credits[slide];
        this.slideTitle.text = currentSlide.title;

        //Destroy all children
        foreach (Transform child in this.slideBodyLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        //Add new children
        foreach (string value in currentSlide.value)
        {
            TextMeshProUGUI bodyText = Instantiate(this.slideBodyPrefab).GetComponent<TextMeshProUGUI>();
            bodyText.name = value;
            bodyText.text = value;
            bodyText.transform.parent = this.slideBodyLayoutGroup.transform;
        }
    }

    /// <summary>
    /// Switch to the next slide
    /// </summary>
    private void SwitchSlides()
    {
        this.currentSlide++;
        this.LoadSlide(this.currentSlide);
    }

    private IEnumerator CycleCreditSlides()
    {
        yield return new WaitForSeconds(GMSceneManager.Instance().GetBlitController().GetFadeOutTime());
        yield return new WaitForSeconds(GMSceneManager.Instance().GetBlitController().GetFadeInTime());
        yield return new WaitForSeconds(this.slideInterval);

        while (true)
        {
            if (this.currentSlide >= this.credits.credits.Length-1)
            {
                GMSceneManager.Instance().LoadNextScene();
                break;
            }

            //Fade out the current slide
            GMSceneManager.Instance().GetBlitController().BeginFadeOut();
            yield return new WaitForSeconds(GMSceneManager.Instance().GetBlitController().GetFadeOutTime());

            //Update the slide
            this.SwitchSlides();

            //Fade in the next slide
            GMSceneManager.Instance().GetBlitController().BeginFadeIn();
            yield return new WaitForSeconds(GMSceneManager.Instance().GetBlitController().GetFadeInTime());
            yield return new WaitForSeconds(this.slideInterval);
        }

        yield return null;
    }
}
