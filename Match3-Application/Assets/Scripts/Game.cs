using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class Game : MonoBehaviour
    {
        public ComponentsModelViewController app { get { return GameObject.FindObjectOfType<ComponentsModelViewController>(); } }
    }
    public class ComponentsModelViewController : MonoBehaviour
    {
        public Model.Model model;
        public View.View view;
        public Controller.Controller controller;
       
    }
}
