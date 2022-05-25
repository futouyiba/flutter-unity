using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public class debugUITrigger : MonoBehaviour
    {
        [SerializeField] protected int clickTimes;
        
        private int clicked = 0;

        [SerializeField] private GameObject debugUI;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        private float idleTime = 0f;
        // Update is called once per frame
        void Update()
        {
            if (clicked > 0)
            {
                idleTime += Time.deltaTime;
                if (idleTime > 3f)
                {
                    ResetClicked();
                }
            }
        }

        private void ResetClicked()
        {
            idleTime = 0f;
            clicked = 0;
        }

        private void OnMouseDown()
        {
            clicked += 1;
            idleTime = 0;
            if (clicked >= clickTimes)
            {
                debugUI.SetActive(!debugUI.activeSelf);
                ResetClicked();
            }
            
            
        }
    }
}
