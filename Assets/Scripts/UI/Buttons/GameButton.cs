using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameButton : MonoBehaviour, IMenuItem
{
    public abstract void PerformAction();

    #region IUserInterface functions
    public virtual void Close()
    {

    }

    public virtual void Hide()
    {
        this.enabled = false;
    }

    public virtual void Show()
    {
        this.enabled = true;
    }
    #endregion
}
