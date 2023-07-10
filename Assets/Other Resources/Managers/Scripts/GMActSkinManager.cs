using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GMActSkinManager : MonoBehaviour
{

    [SerializeField]
    private bool noSkin = false;
    [Button("UpdateSpriteSkins")]
    public bool updateSpriteSkins;
    [Button("LoadSpriteSkins")]
    public bool loadSpriteSkins;
    [Help("Assuming skins starts from 'Assets/Resources/</dir>'")]
    public string assetsDirectory = "Regular Stage/Act Skin/Sprites";
    public int index;

    private readonly string noSkinMessage = "No Skin set to true";

    private Object[] textures;

    [System.Serializable]
    public class ActSkin
    {
        [HideInInspector]
        public string name;
        public Texture2D mainTex;
        public List<Sprite> sprites;

        public ActSkin(Texture2D mainTex, List<Sprite> sprites)
        {
            this.sprites = sprites;
            this.mainTex = mainTex;
            this.name = mainTex.name;
        }
    }

    public List<ActSkin> actSkins;


    public void UpdateSpriteSkins()
    {
        foreach (SpriteRenderer sprites in FindObjectsOfType<SpriteRenderer>())
        {
            foreach (ActSkin actSkin in this.actSkins)
            {
                int containsSpriteIndex = actSkin.sprites.FindIndex(s => sprites.sprite == s);

                if (containsSpriteIndex >= 0)
                {
                    Debug.Log(containsSpriteIndex + "- Found");
                    sprites.sprite = this.actSkins[this.index].sprites[containsSpriteIndex];
                }
            }
        }
    }

    public void LoadSpriteSkins()
    {
        if (this.noSkin)
        {
            Debug.Log(this.noSkinMessage);
            return;
        }

        Debug.Log("Load Sprite Skins");
        this.textures = Resources.LoadAll(this.assetsDirectory, typeof(Texture2D));

        this.actSkins.Clear();

        foreach (Object t in this.textures)
        {
            this.actSkins.Add(new ActSkin(Resources.Load(this.assetsDirectory + "/" + t.name) as Texture2D, Resources.LoadAll(this.assetsDirectory + "/" + t.name).OfType<Sprite>().ToList()));
        }
    }


    public void UpdateSkin()
    {
    }
}
