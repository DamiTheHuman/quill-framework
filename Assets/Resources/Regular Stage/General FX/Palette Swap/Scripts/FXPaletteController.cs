using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls the active palette of an object with the functionality to cycle between Color sets
/// </summary
public class FXPaletteController : MonoBehaviour
{
    [SerializeField, Tooltip("The index of the current palette being loaded")]
    private int index;
    [SerializeField, Tooltip("A set of available palettes for the player")]
    private List<PaletteScriptableObject> palettes = new List<PaletteScriptableObject>();
    [SerializeField, Tooltip("The material of the sprite renderer")]
    private Material spriteMaterial = null;

    [Tooltip("The time frame each palette lasts before each palette colorSet is swapped"), SerializeField]
    private float palettCycleSpeed = 1f;
    [SerializeField, Button(nameof(UpdatePallete))]
    private bool updatePalette;

    private IEnumerator cycleColorSetsCoroutine;

    // Start is called before the first frame update
    private void Start()
    {
        if (this.palettes.Count == 0)
        {
            Debug.LogWarning("THERE ARE NO PALETTES");

            return;
        }

        this.palettes[this.index].SetSubIndex(0);//Start at 0
        this.UpdatePallete();
    }

    /// <summary>
    /// Sets the active paletee to the specified palette
    /// <param name="paletteScriptableObject">The object containing information of the palette being switched too and between</param>
    /// </summary
    private void SetActivePalette(PaletteScriptableObject paletteScriptableObject)
    {
        if (this.palettes.Count == 0 || this.gameObject.activeSelf == false)
        {
            Debug.LogWarning("THERE ARE NO PALETTES");

            return;
        }

        this.index = Mathf.Clamp(this.index, 0, this.palettes.Count - 1);
        this.palettes[this.index].SetSubIndex(0);//Start at 0
        if (this.palettes[this.index] != null && this.palettes.Count > 0)
        {
            int currentSubindex = this.palettes[this.index].subIndex;
            for (int x = 0; x < paletteScriptableObject.colorSets[currentSubindex].values.Count; x++)
            {
                this.spriteMaterial.SetColor("Color_To_" + (x + 1), paletteScriptableObject.GetActiveColorSet().values[x]);
            }
        }
        if (this.palettes[this.index].colorSetControl is PalletCycleSetting.SwapToNextColorSet or PalletCycleSetting.LerpToNextColorSet)
        {
            if (this.cycleColorSetsCoroutine != null)
            {
                this.StopCoroutine(this.cycleColorSetsCoroutine);
            }

            this.cycleColorSetsCoroutine = this.CycleColorSets();
            this.StartCoroutine(this.cycleColorSetsCoroutine);
        }
    }

    /// <summary>
    /// Loads the next palette
    /// </summary
    public void LoadNextPallete()
    {
        this.index++;

        if (this.index >= this.palettes.Count)
        {
            this.index = 0;
        }

        this.UpdatePallete();
    }

    /// <summary>
    /// Loads the previousPalette palette
    /// </summary
    public void LoadPreviousPallete()
    {
        this.index--;

        if (this.index < 0)
        {
            this.index = this.palettes.Count - 1;
        }

        this.UpdatePallete();
    }

    /// <summary>
    /// Loads the next palettes color set 
    /// </summary>

    public void LoadNextPaletteColorSet() => this.palettes[this.index].GetNextColorSet();
    /// <summary>
    /// Loads the previous palettes color set
    /// </summary>

    public void LoadPreviousPaleteColorSet() => this.palettes[this.index].GetPreviousColorSet();

    /// <summary>
    /// Sets the palette index of the object if it exists
    /// <param name="index">the new index to set te palette to</param>
    /// </summary>
    public void SetPaletteIndex(int index)
    {
        if (index < 0 || index > this.palettes.Count - 1)
        {
            Debug.LogWarning("Palette of index" + index + " could not be found!");
        }

        this.index = index;
        this.UpdatePallete();
    }

    /// <summary>
    /// Updates the palette to the value set at the specified index
    /// </summary
    private void UpdatePallete() => this.SetActivePalette(this.palettes[this.index]);

    /// <summary>
    /// Gets the current index of the palette controller
    /// </summary
    public int GetPaletteIndex() => this.index;

    /// <summary>
    /// Gets the current sprite material
    /// </summary
    public Material GetSpriteMaterial() => this.spriteMaterial;
    /// <summary>
    /// Sets the current sprite material
    /// <param name="spriteMaterial">The new material for the sprite compoment</param>
    /// </summary
    public void SetSpriteMaterial(Material spriteMaterial) => this.spriteMaterial = spriteMaterial;

    /// <summary>
    /// Swaps between palette color sets
    /// </summary>
    private IEnumerator CycleColorSets()
    {
        while (true)
        {
            for (int x = 0; x < this.palettes[this.index].GetActiveColorSet().values.Count; x++)
            {
                if (this.palettes[this.index].colorSetControl == PalletCycleSetting.SwapToNextColorSet)
                {
                    this.spriteMaterial.SetColor("Color_To_" + (x + 1), this.palettes[this.index].GetActiveColorSet().values[x]);
                }
                else if (this.palettes[this.index].colorSetControl == PalletCycleSetting.LerpToNextColorSet)
                {
                    this.StartCoroutine(this.LerpColorsOverTime("Color_To_" + (x + 1), this.spriteMaterial.GetColor("Color_To_" + (x + 1)), this.palettes[this.index].GetActiveColorSet().values[x], this.palettCycleSpeed));
                }
            }
            if (this.palettes[this.index].LastColorSet())
            {
                this.palettes[this.index].SetSubIndex(0);//If at the  last color set go back to the beginning
            }
            else
            {
                this.LoadNextPaletteColorSet();
            }

            yield return new WaitForSeconds(this.palettCycleSpeed);
        }
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
        float initialIndex = this.index;
        float inversedTime = 1 / time;

        for (float step = 0.0f; step < 1.0f; step += Time.deltaTime * inversedTime)
        {
            if (initialIndex != this.index)
            {
                break;
            }
            this.spriteMaterial.SetColor(materialColor, Color.Lerp(startingColor, endingColor, step));

            yield return null;
        }

        yield return null;
    }

    /// <summary>
    /// Swaps data from one palette controller to another
    /// <param name="targetPaletteController">The palette controller to swap to</param>
    /// </summary>
    public void SwapPalleteController(FXPaletteController targetPaletteController)
    {
        this.palettes = targetPaletteController.palettes;
        this.index = targetPaletteController.index;
        this.StopAllCoroutines();
        this.Start();
    }
}
