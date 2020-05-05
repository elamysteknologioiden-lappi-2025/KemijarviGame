/******************************************************************************
 * File         : pLab_GeoPositionCharacter.cs            
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


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;


/// <summary>
/// Type of character movement
/// </summary>
enum GeoCharacterMoveType {
    ENone,
    EGps,
    EDirectPoint
}

/// <summary>
/// pLab_GeoPositionCharacter
/// </summary>
public class pLab_GeoPositionCharacter : MonoBehaviour {

    #region // Class Attributes

    /// <summary>
    /// objectTransform
    /// </summary>
    private Transform objectTransform;

    /// <summary>
    /// geoCharacterMoveType
    /// </summary>
    [SerializeField]
    private GeoCharacterMoveType geoCharacterMoveType = GeoCharacterMoveType.EGps;

    [SerializeField]
    private pLab_LocationProvider locationProvider;

    private const string ANIMATOR_SPEED_PARAMETER = "MovementSpeed";

    private float startTime = 0f;
    private Vector3 startPosition = Vector3.zero;
    private Vector3 desiredPos = Vector3.zero;
    private Vector3 changePos = Vector3.zero;
    private Vector3 newPos = Vector3.zero;
    private float speed = 0;
    private float timeDif = 0;
    private float distance = 0;

    [SerializeField]
    private Animator characterAnimator;

    private Transform characterTransform;

    bool initialPositioningDone = false;

    private float lastTimeClicked = 0;



    #endregion

    #region // From Base Class

    void Awake() {
        if (characterTransform == null) {
            characterTransform = characterAnimator.gameObject.transform;
        }
    }

    void Start() {
        objectTransform = transform;
        XRSettings.enabled = false;
        desiredPos = objectTransform.position;

        if(startPosition == Vector3.zero){
            startPosition = transform.position;
        }

        if (desiredPos == Vector3.zero) {
            desiredPos = transform.position;
        }

        if (newPos == Vector3.zero) {
            newPos = transform.position;
        }
    }


    private void OnEnable() {
        if (locationProvider != null) {
            locationProvider.OnLocationUpdated += OnLocationUpdated;

            if (geoCharacterMoveType == GeoCharacterMoveType.EGps) {
                TeleportToLocation(locationProvider.Location);
            }

        }

    }


    private void OnDisable() {
        if (locationProvider != null) {
            locationProvider.OnLocationUpdated -= OnLocationUpdated;
        }

        initialPositioningDone = false;
    }

    void Update() {

        if (geoCharacterMoveType == GeoCharacterMoveType.EDirectPoint) {
            if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject()) {
                bool castRay = false;

                if (lastTimeClicked != 0) {
                    if (Time.time - lastTimeClicked < 0.4f) {
                        //Double click
                        castRay = true;
                    }
                }

                if (Input.touchCount >= 2) castRay = false;

                if (castRay) {
                    RaycastHit rayHit;

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out rayHit, 400)) {
                        bool hitGround = true;
                        string hitTag = rayHit.transform.gameObject.tag;
                        string hitLayer = LayerMask.LayerToName(rayHit.transform.gameObject.layer);

                        if (hitTag.Equals("NPC") || hitLayer.Equals("Water")) {
                            hitGround = false;
                        }


                        if (hitGround) {
                            Vector3 rayHitPoint = rayHit.point;
                            rayHitPoint.y = 0f;
                            newPos = rayHitPoint;
                        }
                    }
                }

                lastTimeClicked = Time.time;
            }
        }

        // If moved, compute new pos;
        if (Vector3.Distance(newPos, desiredPos) > 0.1f) {

            desiredPos = newPos;
            startPosition = transform.position;

            // Compute new position distance
            distance = Vector3.Distance(startPosition, desiredPos);

            if (distance > 0) {
                if (distance > 500) {
                    TeleportToPosition(desiredPos);
                }
                speed = Mathf.Clamp(distance, 0.05f, 25f);
            } else {
                speed = 0;
            }

            characterAnimator.SetFloat(ANIMATOR_SPEED_PARAMETER, speed);

            startTime = Time.time;

            //Calculate and update the angle
            Vector3 targetDir = desiredPos - startPosition;
            float angle = Vector3.Angle(targetDir, transform.forward);
            Vector3 localRot = characterTransform.localEulerAngles;

            localRot.y = angle;

            if (targetDir.x < 0)
                localRot.y *= -1;

            characterTransform.localEulerAngles = localRot;
        }

        if (speed >= 0.05f) {
            float distCovered = (Time.time - startTime) * speed;
            
            float distanceCoveredRatio = distCovered / distance;

            // if(speed == 0)
            // {
            //     distanceCoveredRatio = 1;
            // }

            if(distanceCoveredRatio > 1)
            {
                distanceCoveredRatio = 1;
                speed = 0;
                characterAnimator.SetFloat(ANIMATOR_SPEED_PARAMETER, speed);
            }


            transform.position = Vector3.Lerp(startPosition, desiredPos, distanceCoveredRatio);
            //transform.position = Vector3.Lerp(startPosition, desiredPos, Time.deltaTime);
            // transform.position = transform.position + ((desiredPos - transform.position).normalized * speed * Time.deltaTime);
        }
    }


    #endregion

    #region // Private Functions

    /// <summary>
    /// Event handler for OnLocationUpdated-event. Calculates new position for the character
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLocationUpdated(object sender, pLab_LocationUpdatedEventArgs e)
    {
        if (geoCharacterMoveType == GeoCharacterMoveType.EGps)
            CalculateNewPosition(e.location);
    }

    /// <summary>
    /// Calculates characters new position from LatLon coordinates
    /// </summary>
    /// <param name="latLon"></param>
    private void CalculateNewPosition(pLab_LatLon latLon) {
        if (latLon == null) return;
        pLab_UTMCoordinates coordinates = new pLab_UTMCoordinates(latLon);
        newPos = new Vector3((float)(coordinates.UTMX - pLab_GeoMap.instance.UtmX),
            0f, (float)(coordinates.UTMY - pLab_GeoMap.instance.UtmY));

        if (!initialPositioningDone) {
            TeleportToPosition(newPos);
            initialPositioningDone = true;
        }
    }

    /// <summary>
    /// Teleport character to a location (GPS-coordinates)
    /// </summary>
    /// <param name="location"></param>
    private void TeleportToLocation(pLab_LatLon location) {
        if (location == null) return;
        
        pLab_UTMCoordinates coordinates = new pLab_UTMCoordinates(location);
        Vector3 newPosition = new Vector3((float)(coordinates.UTMX - pLab_GeoMap.instance.UtmX),
            0f, (float)(coordinates.UTMY - pLab_GeoMap.instance.UtmY));
        
        if (!initialPositioningDone) {
            TeleportToPosition(newPosition);
            initialPositioningDone = true;
        }
    }
    /// <summary>
    /// Teleport character to a position (World coordinates)
    /// </summary>
    /// <param name="pos"></param>
    private void TeleportToPosition(Vector3 pos) {
        transform.position = pos;
    }

    #endregion

    #region // Protected Functions
    #endregion

    #region // Public Functions


    #endregion



}
