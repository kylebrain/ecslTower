using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All user interfaces should inherit from IUserInterface
/// </summary>
public interface IUserInterface
{
    void Show();
    void Hide();
    void Close();
}
