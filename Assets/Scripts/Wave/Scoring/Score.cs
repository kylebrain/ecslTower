using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages Health and Money
/// </summary>
public class Score : MonoBehaviour {

    #region Money Variables
    /// <summary>
    /// Amount of money the level starts with
    /// </summary>
    /// <remarks>
    /// Could be changed to a per Map basis
    /// </remarks>
    public int startingAmount = 0;

    public static int StartingAmount
    {
        get
        {
            return instance.startingAmount;
        }
    }
    /// <summary>
    /// Amount of money the player currently has
    /// </summary>
    public static int Money = 0;
    /// <summary>
    /// Text that displays the value of Money
    /// </summary>
    public Text moneyText;

    #endregion

    #region Health Variables

    /// <summary>
    /// The starting and most health possible
    /// </summary>
    public static readonly int MaxHealth = 100;

    /// <summary>
    /// Current health
    /// </summary>
    /// <remarks>
    /// Set keeps it in the range of [0, MaxHealth]
    /// </remarks>
    public static int Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            if (health > MaxHealth)
            {
                health = MaxHealth;
            }

            if (health < 0)
            {
                health = 0;
            }
        }
    }

    /// <summary>
    /// Local variable for health
    /// </summary>
    private static int health;

    /// <summary>
    /// Text that displays Health
    /// </summary>
    public Text healthText;

    /// <summary>
    /// Slider that displays Health
    /// </summary>
    public Slider healthSlider;

    #endregion

    /// <summary>
    /// Displayed when the game ends
    /// </summary>
    public EndScreen endScreen;

    /// <summary>
    /// Instance of the current Score object
    /// </summary>
    private static Score instance;

    private void Awake()
    {
        instance = this;
        Money = startingAmount;
        Health = MaxHealth;
    }

    /// <summary>
    /// Updates all display variables to display the current value
    /// </summary>
    private void Update()
    {

        //find or set these values explicitly
        if(moneyText != null)
        {
            moneyText.text = "$" + Money + ".00";
        }
        if(healthText != null)
        {
            healthText.text = Health.ToString();
        }
        if(healthSlider != null)
        {
            healthSlider.value = Health;
        }
        
    }

    /// <summary>
    /// The game is lost and the failure screen is brought up
    /// </summary>
    public static void GameLost()
    {
        instance.endScreen.EndGame(false);
    }
}
