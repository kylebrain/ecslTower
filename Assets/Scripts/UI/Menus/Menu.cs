using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    protected List<IMenuItem> menuItems = new List<IMenuItem>();

    protected void TransitionTo(IMenuItem menuItem)
    {
        menuItem.Show();
    }

    protected abstract void Quit();
}
