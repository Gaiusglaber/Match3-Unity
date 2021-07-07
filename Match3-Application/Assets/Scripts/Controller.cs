using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Controller
{
    public class Controller : ComponentsModelViewController
    {
        Vector2 firstTouchPosition;
        Vector2 finalTouchPosition;
        private void Update()
        {
            if (Physics2D.OverlapPoint(firstTouchPosition, model.layer) && Input.GetMouseButtonDown(0))
            {
                firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                GameObject token = Physics2D.OverlapPoint(firstTouchPosition, model.layer).gameObject;
            }else if (Physics2D.OverlapPoint(firstTouchPosition, model.layer) && Input.GetMouseButtonUp(0))
            {
                finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Debug.Log(ReturnAngle());
            }
        }
        private float ReturnAngle()
        {
            return Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
        }
    }
}
