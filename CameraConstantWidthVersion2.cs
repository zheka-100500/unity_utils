using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
namespace Game
{
    [ExecuteAlways]
    public class CameraConstantWidthVersion2 : MonoBehaviour
    {
        [SerializeField] private float defaultSize = 5f;
        [SerializeField] private float scaledSize = 5f;
        [SerializeField] private Camera cam;

        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        private void Update()
        {
            if(cam == null) return;
            
            
            cam.orthographicSize = Screen.width < Screen.height ? scaledSize : defaultSize;
        }

  
    }
}