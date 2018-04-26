using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    protected List<IMenuItem> menuItems = new List<IMenuItem>();

    protected virtual void TransitionTo(IMenuItem menuItem)
    {
        menuItem.Show();
    }
}
