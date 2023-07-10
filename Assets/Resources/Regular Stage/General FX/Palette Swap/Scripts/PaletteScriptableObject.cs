using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Palette", order = 3)]
public class PaletteScriptableObject : ScriptableObject
{
    public PalletCycleSetting colorSetControl = PalletCycleSetting.Static;
    public int subIndex = 0;
    public List<Palette> colorSets;

    [SerializeField, Button(nameof(AutoGenerateColorSets)), Tooltip("Use permutations of the first color set to create new ones")]
    private bool autoGenerateColorSets;

    /// <summary>
    /// A class which contains the data of possible palettes for the user to select from
    /// </summary>
    [System.Serializable]
    public class Palette
    {
        public string description;
        public List<Color> values = new List<Color>(18);
    }

    /// <summary>
    /// Loads the next palette
    /// </summary
    public void GetNextColorSet()
    {
        this.subIndex++;

        if (this.subIndex >= this.colorSets.Count)
        {
            this.subIndex = this.colorSets.Count - 1;
        }
    }

    /// <summary>
    /// Loads the previousPalette palette
    /// </summary
    public void GetPreviousColorSet()
    {
        this.subIndex--;

        if (this.subIndex < 0)
        {
            this.subIndex = 0;
        }
    }

    /// <summary>
    /// Checks if the current sub index is at the last color set
    /// </summary
    public bool LastColorSet() => this.subIndex >= this.colorSets.Count - 1;
    /// <summary>
    /// Sets the sub index pointer for the palette to the specified value
    /// </summary
    public void SetSubIndex(int subIndex)
    {
        subIndex = Mathf.Clamp(subIndex, 0, this.colorSets.Count - 1);
        this.subIndex = subIndex;
    }

    public Palette GetActiveColorSet() => this.colorSets[this.subIndex];
    /// <summary>
    /// Loads the preset of colors from the material that has been passed in
    /// </summary
    public void LoadColorsFromMaterial(Material material, int index = 0)
    {
        //Ensures there is always a palette
        if (index > this.colorSets.Count - 1)
        {
            this.colorSets.Add(new Palette());
            index = this.colorSets.Count - 1;
        }

        this.colorSets[index].values = new List<Color>();

        for (int x = 0; x < 18; x++)
        {
            this.colorSets[index].values.Add(material.GetColor("Color_To_" + (x + 1)));
        }
    }

    private void AutoGenerateColorSets()
    {
        if (this.colorSets[0].values != null)
        {
            for (int x = 1; x < this.colorSets[0].values.Count; x++)
            {
                Palette newColor = new Palette
                {
                    values = this.ShiftRight(this.colorSets[this.colorSets.Count - 1].values),
                    description = "Color " + x
                };

                this.colorSets.Add(newColor);
            }
        }
    }

    public List<Color> ShiftRight(List<Color> array) => array.Skip(array.Count - 1).Concat(array.Take(array.Count - 1)).ToList();
}
