/******************************************************************************
* File         : pLab_OutOfGeoAreaBoundsUI.cs
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

public class pLab_OutOfGeoAreaBoundsUI : MonoBehaviour
{
    #region Variables

    [SerializeField]
    [Header("Player must be inside this geo area to play")]
    private pLab_GeoArea playableGeoArea;

    [SerializeField]
    private pLab_LocationProvider locationProvider;

    [SerializeField]
    private GameObject alertIndicatorCanvasObj;

    #endregion

    #region Inherited Methods
    
    private void OnEnable() {
        if (locationProvider != null) {
            locationProvider.OnLocationUpdated += OnLocationUpdated;
        }
    }

    private void OnDisable() {
        if (locationProvider != null) {
            locationProvider.OnLocationUpdated -= OnLocationUpdated;
        }
    }


    #endregion

    #region Private Methods

    /// <summary>
    /// Event handler for OnLocationUpdated-event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLocationUpdated(object sender, pLab_LocationUpdatedEventArgs e)
    {
        RecheckIfOutOfBounds(e.location);
    }

    /// <summary>
    /// Recheck if new GPS-coordinates are out of bounds
    /// </summary>
    private void RecheckIfOutOfBounds(pLab_LatLon latLon) {
        bool showAlertIndicator = false;

        if (playableGeoArea != null) {
            showAlertIndicator = playableGeoArea.IsLocationInsideZone(latLon);
        }

        ToggleAlertIndicatorVisibility(showAlertIndicator);

    }

    /// <summary>
    /// Toggle alert indicator visibility
    /// </summary>
    /// <param name="isVisible"></param>
    private void ToggleAlertIndicatorVisibility(bool isVisible) {
        if (alertIndicatorCanvasObj != null) {
            alertIndicatorCanvasObj.SetActive(isVisible);
        }
    }

    #endregion

}
