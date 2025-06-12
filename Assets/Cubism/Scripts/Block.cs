using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Line[] up;
    public Line[] down;
    public Line[] left;
    public Line[] right;
    public Line[] forward;
    public Line[] back;

    public void DisableLines(Line[] lines)
    {
        foreach (Line l in lines)
        {
            l.gameObject.SetActive(false);
        }
    }
}
