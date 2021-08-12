using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Match3.Model
{
    public class ModelInput : MonoBehaviour
    {
        [NonSerialized]public List<Token> tokensSelection;
        [NonSerialized]public bool allowed=false;
        [SerializeField]public LayerMask layer;
    }
}
