using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlingShotArea : MonoBehaviour
{
    [SerializeField] private LayerMask slingShotAreaMask;
    public bool IsWithinSlingShotArea()
    { 
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);

        if (Physics2D.OverlapPoint(worldPosition , slingShotAreaMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
