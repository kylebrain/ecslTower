using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameButton: MonoBehaviour, IMenuItem {
    public bool MouseIsOver {
        get{
            return mouseIsOver;
        }
        set {

        }
    }
    private bool mouseIsOver = false;

    private void OnMouseEnter() {
        mouseIsOver = true;
    }

    private void OnMouseExit() {
        mouseIsOver = false;
    }

    public abstract void PerformAction();

    #region IUserInterface functions
    public virtual void Close()
    {

    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
    #endregion
}
