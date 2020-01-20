/******************************************************************************
 * File         : pLab_ObjectFollow.cs            
 * Lisence      : BSD 3-Clause License
 * Copyright    : Lapland University of Applied Sciences
 * Authors      : 
 * BSD 3-Clause License
 *
 * Copyright (c) 2019, Lapland University of Applied Sciences
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice, this
 *  list of conditions and the following disclaimer.
 *
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *  this list of conditions and the following disclaimer in the documentation
 *  and/or other materials provided with the distribution.
 *
 * 3. Neither the name of the copyright holder nor the names of its
 *  contributors may be used to endorse or promote products derived from
 *  this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera script which follows object. Has touch support for turning and zooming the camera
/// </summary>
public class pLab_ObjectFollow : MonoBehaviour {

    #region Variables
    
    [SerializeField]
    private GameObject followObject;

    [SerializeField]
    private GameObject rings;

    [SerializeField]
    private float zoomValue = 0f;

    [SerializeField]
    private GameObject cameraGameobject;

    private Transform cameraTransform;

    private Camera cameraCam;


    [SerializeField]
    private float cameraMinAngle = 15f;
    [SerializeField]
    private float cameraMaxAngle = 40f;

    [SerializeField]
    private float cameraMinHeight = -75f;
    [SerializeField]
    private float camearaMaxHeight = 0f;

    [SerializeField]
    private float cameraMinDistance = -30f;
    [SerializeField]
    private float cameraMaxDistance = -100f;

    #if UNITY_EDITOR
    private Vector3 prevMousePosition;
    #endif

    #endregion


    #region Inherited Methods

    void Awake() {
        if (cameraGameobject != null) {
            cameraTransform = cameraGameobject.transform;
            cameraCam = cameraGameobject.GetComponent<Camera>();
        }
    }

    // Use this for initialization
    // void Start () {

        //  deltaPosition = Input.mousePosition;

    // }
    

    void LateUpdate () {

        #if UNITY_EDITOR
        //For testing in the editor
        if (Input.GetMouseButtonDown(1)) {
            prevMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(1)) {
            Vector2 deltaPos = prevMousePosition - Input.mousePosition;

            float xDelta = deltaPos.x;

            if (Mathf.Abs(xDelta) > 1f) {
                Vector3 aa = transform.localEulerAngles;
                aa.y += xDelta;
                transform.localEulerAngles = aa;
            }

            prevMousePosition = Input.mousePosition;
        }

        float scrollDeltaY = -Mathf.Clamp(Input.mouseScrollDelta.y, -5, 5);

        if (Mathf.Abs(scrollDeltaY) > 0.1f) {
            zoomValue = Mathf.Clamp01(zoomValue + scrollDeltaY/10f);
        }

        #endif
        

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            Vector2 touchZeroPrevPos = touch.position - touch.deltaPosition;
           // touchZeroPrevPos.z = 0;
            float mmm = touch.deltaPosition.magnitude;


            //Vector2 screenPos = cameraTransform.GetComponent<Camera>().WorldToScreenPoint(transform.position);
            Vector2 screenPos = cameraCam.WorldToScreenPoint(transform.position);
            //screenPos.z = 0;

            Vector3 prevLo = touchZeroPrevPos - screenPos;
            Vector3 nowLo = touch.position - screenPos;


            float angle = Vector3.SignedAngle(prevLo, nowLo, Vector3.forward);

            // float sh = Screen.height;
            // float sw = Screen.width;

            Vector3 aa = transform.localEulerAngles;
            aa.y += angle;
            transform.localEulerAngles = aa;
        }
        
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            zoomValue += (deltaMagnitudeDiff/1000)*-1;
            zoomValue = Mathf.Clamp01(zoomValue);

        }
        
        transform.position = followObject.transform.position;

        cameraTransform.localPosition = new Vector3(0, ((camearaMaxHeight - cameraMinHeight) * zoomValue) + cameraMinHeight, ((cameraMaxDistance - cameraMinDistance) * zoomValue) + cameraMinDistance);

        cameraTransform.localEulerAngles = new Vector3(((cameraMaxAngle - cameraMinAngle) * zoomValue) + cameraMinAngle,0,0);


        Vector3 tasa = new Vector3(0, zoomValue * 0f, 0);
        Vector3 scala = new Vector3(1,1+ zoomValue * 4f, 1);
        rings.transform.localPosition = tasa;
        rings.transform.localScale = scala;
    }

    #endregion
}
