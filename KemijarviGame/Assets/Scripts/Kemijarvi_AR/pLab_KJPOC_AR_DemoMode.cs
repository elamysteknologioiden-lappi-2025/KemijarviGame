/******************************************************************************
* File         : pLab_KJPOC_AR_DemoMode.cs
* Lisence      : BSD 3-Clause License
* Copyright    : Lapland University of Applied Sciences
* Authors      : Arto Söderström
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class pLab_KJPOC_AR_DemoMode : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private ARSessionOrigin arSessionOrigin;

    [SerializeField]
    private ARRaycastManager arRaycastManager;

    [SerializeField]
    private List<GameObject> npcPrefabs;

    private List<ARRaycastHit> hitList = new List<ARRaycastHit>();

    private List<GameObject> createdObjects = new List<GameObject>();

    private Transform parentObject;


    #endregion


    #region Inherited Methods

    #if UNITY_EDITOR
    private void Reset() {
        if (arSessionOrigin == null)
            arSessionOrigin = FindObjectOfType<ARSessionOrigin>();

        if (arRaycastManager == null)
            arRaycastManager = FindObjectOfType<ARRaycastManager>();
    }
    #endif

    private void OnEnable() {
        ARSession.stateChanged += OnSessionStateChanged;
    }



    private void OnDisable() {
        ARSession.stateChanged -= OnSessionStateChanged;
        DestroyPrefabs();
    }

    private void Update() {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began) {
                arRaycastManager.Raycast(touch.position, hitList, TrackableType.PlaneWithinPolygon);

                if (hitList.Count > 0) {
                    Pose hitPose = hitList[0].pose;
                    Vector3 fromCameraToPoint = hitPose.position - Camera.main.transform.position;
                    fromCameraToPoint.y = 0;
                    fromCameraToPoint.Normalize();

                    if (createdObjects != null && createdObjects.Count > 0) {
                        MovePrefabs(hitPose.position, fromCameraToPoint);
                    } else {
                        SpawnAllPrefabs(hitPose.position, fromCameraToPoint);
                    }
                }
            }
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Event handler for ARSessionStateChanged-event
    /// </summary>
    /// <param name="evt"></param>
    private void OnSessionStateChanged(ARSessionStateChangedEventArgs evt)
    {
        if (evt.state == ARSessionState.Ready || evt.state == ARSessionState.SessionInitializing) {
            DestroyPrefabs();
        }
    }

    /// <summary>
    /// Spawn all NPC prefabs in a circle around the center point
    /// </summary>
    /// <param name="centerPoint"></param>
    private void SpawnAllPrefabs(Vector3 centerPoint, Vector3 fromCameraToPoint) {
        
        DestroyPrefabs();

        if (parentObject == null) {
            parentObject = new GameObject("Demo AR Parent").transform;
            parentObject.position = Vector3.zero;
        }
        foreach(GameObject prefab in npcPrefabs) {
            GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(parentObject, true);
            createdObjects.Add(go);
        }

        MovePrefabs(centerPoint, fromCameraToPoint);
    }

    /// <summary>
    /// Move the prefabs to around the center point
    /// </summary>
    /// <param name="centerPoint"></param>
    private void MovePrefabs(Vector3 centerPoint, Vector3 fromCameraToPoint) {
        int index = 0;
        Vector3 offset = Vector3.zero;
        Vector3 eulerRot = Vector3.zero;
        parentObject.position = centerPoint;

        foreach(GameObject go in createdObjects) {
            switch(index) {
                case 0:
                offset = new Vector3(0, 0, -0.5f);
                eulerRot.y = 0;
                break;
                case 1:
                offset = new Vector3(0.75f, 0, 0f);
                eulerRot.y = -45f;
                break;
                case 2:
                offset = new Vector3(-0.75f, 0, 0f);
                eulerRot.y = 45f;
                break;
            }

            Vector3 objectPosition = offset;
            go.transform.localPosition = objectPosition;
            go.transform.localRotation = Quaternion.Euler(eulerRot);
            index++;
        }

        parentObject.LookAt(Camera.main.transform);
        Quaternion rot = parentObject.rotation;
        Vector3 parentEulerRot = rot.eulerAngles;
        parentEulerRot.z = 0;
        parentEulerRot.x = 0;
        parentObject.rotation = Quaternion.Euler(parentEulerRot);

    }

    /// <summary>
    /// Destroy all NPC prefabs
    /// </summary>
    private void DestroyPrefabs() {
        foreach(GameObject go in createdObjects) {
            Destroy(go);
        }

        createdObjects.Clear();

        if (parentObject != null) {
            Destroy(parentObject);
        }
    }

    #endregion
}
