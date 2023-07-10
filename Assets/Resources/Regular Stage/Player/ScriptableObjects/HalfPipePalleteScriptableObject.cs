using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/HalfPipePalettes", order = 3)]

public class HalfPipePalleteScriptableObject : ScriptableObject
{
    [System.Serializable]
    public class EmeraldPalette
    {
        public Color color1 = General.RGBToColour(226, 136, 0);
        public Color color2 = General.RGBToColour(102, 126, 0);
    }

    public Material halfPipeMaterial;
    public List<EmeraldPalette> emeraldPalettes = new List<EmeraldPalette>();

    public void UpdatePaletteMaterial(int index)
    {
        this.halfPipeMaterial.SetColor("Color_To_1", this.emeraldPalettes[index].color1);
        this.halfPipeMaterial.SetColor("Color_To_2", this.emeraldPalettes[index].color2);
    }
}
