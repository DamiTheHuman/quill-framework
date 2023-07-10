using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A parent class for all the power ups accessible to the player
/// </summary>
public class HedgePowerUp : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Depnendecies")]
    protected Player player;
    [LastFoldoutItem, SerializeField]
    protected PowerUpManager hedgePowerUpManager;
    [SerializeField]
    protected List<PowerUpSpriteData> powerUpSprites;
    [SerializeField]
    protected GameObject activePowerUp;

    private void Reset()
    {
        this.player = this.GetComponentInParent<Player>();
        this.hedgePowerUpManager = this.GetComponentInParent<PowerUpManager>();
    }

    public virtual void Start()
    {
        if (this.player == null)
        {
            this.Reset();
        }
    }

    /// <summary>
    /// Sets the visibility of the power up
    /// <param name="value">the value to set the status of the power up to</param>
    /// </summary>
    public virtual void SetPowerUpVisbility(bool value)
    {
        if (this.activePowerUp == null)
        {
            return;
        }

        this.activePowerUp.SetActive(value);
    }

    /// <summary>
    /// Removes the parent of the active power up
    /// </summary>
    public virtual void RemovePowerUp()
    {
        this.SetPowerUpVisbility(false);

        if (this.activePowerUp == null)
        {
            return;
        }

        this.activePowerUp.transform.parent = null;
        this.activePowerUp.SetActive(false);
        this.activePowerUp = null;
    }

    /// <summary>
    /// Gets a reference to the active game object
    /// </summary>
    public GameObject GetActivePowerUpGameObject() => this.activePowerUp;

    /// <summary>
    /// Whether the current power up is active
    /// </summary>
    public virtual bool PowerUpIsActive() => this.activePowerUp != null;

    /// <summary>
    /// Gets all the sprites registered to a power up
    /// </summary>
    public void RetriveAllPowerUpSpriteData()
    {
        if (this.activePowerUp == null)
        {
            return;
        }

        this.powerUpSprites = new List<PowerUpSpriteData>();

        foreach (SpriteRenderer spriteRenderer in this.activePowerUp.GetComponentsInChildren<SpriteRenderer>(true))
        {
            this.powerUpSprites.Add(new PowerUpSpriteData(spriteRenderer));
        }
    }

    /// <summary>
    /// Gets all the sprites registered to a power up
    /// <param name="value">the value of the transparency sprite object</param>
    /// </summary>
    public void SetPowerUpSpritesTransparency(float value = 1)
    {
        foreach (PowerUpSpriteData spriteCache in this.powerUpSprites)
        {
            spriteCache.spriteRenderer.color = new Color(1, 1, 1, value * spriteCache.originalColor.a);
        }
    }

    [Serializable]
    public class PowerUpSpriteData
    {
        public PowerUpSpriteData(SpriteRenderer spriteRenderer)
        {
            this.originalColor = spriteRenderer.color;
            this.spriteRenderer = spriteRenderer;
        }

        public Color originalColor;
        public SpriteRenderer spriteRenderer;
    }
}
