using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    //Setup canvas to avoid flash UI
    //set up mac dinh cho UI de tranh truong hop bi nhay' hinh
    public virtual void Setup()
    {
    }

    //Open canvas
    //mo canvas
    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

}
