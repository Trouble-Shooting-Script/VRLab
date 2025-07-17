using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class PuzzleBlock : MonoBehaviour
{
    [Serializable]
    public class NeighborFlag
    {
        public bool L;
        public bool R;
        public bool U;
        public bool D;
        public bool F;
        public bool B;
    
        public bool LU;
        public bool LD;
        public bool LF;
        public bool LB;
        public bool RU;
        public bool RD;
        public bool RF;
        public bool RB;
        public bool FU;
        public bool FD;
        public bool BU;
        public bool BD;
    }

    [Serializable]
    public class Outline
    {
        public GameObject LU;
        public GameObject LD;
        public GameObject RU;
        public GameObject RD;
        public GameObject FU;
        public GameObject FD;
        public GameObject BU;
        public GameObject BD;
        public GameObject LF;
        public GameObject RF;
        public GameObject LB;
        public GameObject RB;
    }
    
    public NeighborFlag Flag = new NeighborFlag();
    public Outline Line = new Outline();
    
    public void CleanLines()
    {
        Line.LU.SetActive(!(Flag.L ^ Flag.LU ^ Flag.U));
        Line.LD.SetActive(!(Flag.L ^ Flag.LD ^ Flag.D));
        Line.RU.SetActive(!(Flag.R ^ Flag.RU ^ Flag.U));
        Line.RD.SetActive(!(Flag.R ^ Flag.RD ^ Flag.D));
        Line.FU.SetActive(!(Flag.F ^ Flag.FU ^ Flag.U));
        Line.FD.SetActive(!(Flag.F ^ Flag.FD ^ Flag.D));
        Line.BU.SetActive(!(Flag.B ^ Flag.BU ^ Flag.U));
        Line.BD.SetActive(!(Flag.B ^ Flag.BD ^ Flag.D));
        Line.LF.SetActive(!(Flag.L ^ Flag.LF ^ Flag.F));
        Line.RF.SetActive(!(Flag.R ^ Flag.RF ^ Flag.F));
        Line.LB.SetActive(!(Flag.L ^ Flag.LB ^ Flag.B));
        Line.RB.SetActive(!(Flag.R ^ Flag.RB ^ Flag.B));
    }
}