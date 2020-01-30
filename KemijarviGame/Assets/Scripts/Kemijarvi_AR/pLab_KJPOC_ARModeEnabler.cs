/******************************************************************************
* File         : pLab_KJPOC_ARModeEnabler.cs
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Checks if POIs are close enough to be seen in AR-mode
/// </summary>
public class pLab_KJPOC_ARModeEnabler : MonoBehaviour
{

    #region Variables

    [SerializeField]
    private pLab_PointOfInterestSet pointOfInterestSet;

    [SerializeField]
    private pLab_LocationProvider locationProvider;

    [SerializeField]
    private pLab_KJPOCMapARTransition mapARTransition;

    [SerializeField]
    private float checkInterval = 1f;

    private bool isEnabled = true;

    private float timer = 0f;

    #endregion

    #region Properties

    public float CheckInterval { get { return checkInterval; } set { checkInterval = value; } }
    public bool IsEnabled { get { return isEnabled; } set { isEnabled = value; } }
    public List<pLab_PointOfInterest> PointOfInterests { get { return pointOfInterestSet != null && pointOfInterestSet.PointOfInterests != null ? pointOfInterestSet.PointOfInterests : new List<pLab_PointOfInterest>(); } }
    
    #endregion

    #region Inherited Methods

    private void OnEnable() {
        ARSession.stateChanged += OnARSessionStateChanged;
    }

    private void OnDisable() {
        ARSession.stateChanged -= OnARSessionStateChanged;
    }

    private void Update() {
        if (!isEnabled || locationProvider == null) return;

        timer += Time.deltaTime;

        if (timer >= checkInterval) {
            CheckIfPointOfInterestsClose();
            timer = 0f;
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Checks if any of the point of interests are close to the player, and highlights AR-mode button depending on that.
    /// </summary>
    private void CheckIfPointOfInterestsClose() {
        pLab_LatLon currentPlayerLocation = locationProvider.Location;

        if (currentPlayerLocation != null) {
            bool isClose = false;
            for(int i = 0; i < PointOfInterests.Count; i++) {
                if (PointOfInterests[i] == null) continue;

                float distanceBetween = currentPlayerLocation.DistanceToPointPythagoras(PointOfInterests[i].Coordinates);

                if (distanceBetween <= PointOfInterests[i].TrackingRadius - 5) {
                    // Debug.Log($"{PointOfInterests[i].PoiName} is close ({distanceBetween} m)");
                    isClose = true;
                    break;
                }
            }

            mapARTransition.ToggleARButtonHighlight(isClose);
        }
    }

    /// <summary>
    /// Event handler for ARSessionStateChanged-event. Changes IsEnabled-state based on if AR is supported or not
    /// </summary>
    /// <param name="evt"></param>
    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs evt)
    {
        if (evt.state == ARSessionState.Unsupported) {
            isEnabled = false;
        } else {
            isEnabled = true;
        }
    }

    #endregion
}
