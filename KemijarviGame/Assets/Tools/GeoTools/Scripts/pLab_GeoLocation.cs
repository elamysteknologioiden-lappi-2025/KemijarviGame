/******************************************************************************
 * File         : pLab_GeoLocation.cs            
 * Lisence      : BSD 3-Clause License
 * Copyright    : Lapland University of Applied Sciences
 * Authors      : Toni Westerlund (toni.westerlund@lapinamk.fi)
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
using UnityEngine.UI;

public class pLab_GeoLocation : MonoBehaviour {

    #region // Class Attributes

    /// <summary>
    /// utm X
    /// </summary>
    private double utmX = 0;

    /// <summary>
    /// Get utm X
    /// </summary>
    public double UtmX { get { return this.utmX; } }

    /// <summary>
    /// utm y
    /// </summary>
    private double utmY = 0;

    /// <summary>
    /// Get utm Y
    /// </summary>
    public double UtmY { get { return this.utmY; } }


    /// <summary>
    /// utm y
    /// </summary>
    private float heading = 0;

    /// <summary>
    /// Get utm Y
    /// </summary>
    public float Heading { get { return this.heading; } }

    /// <summary>
    /// Singleton
    /// </summary>
    public static pLab_GeoLocation instance = null;

    #endregion
    #region // From Base Class

    void Awake() {
        if (instance == null)
            instance = this;
    }

    #endregion

    IEnumerator Start()
    {
        yield return new WaitForSeconds(3);
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {

            yield break;
        }

        Input.compass.enabled = true;


        // Start service before querying location
        Input.location.Start(1, 1f);

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            print("Timed out");

            yield break;
        }
        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");

            yield break;
        }
        else
        {

            // Access granted and location value could be retrieved
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

    }
    private void Update()
    {
        pLAB_GeoUtils.LatLongtoUTM((double)Input.location.lastData.latitude, (double)Input.location.lastData.longitude, out this.utmY, out this.utmX);
        heading = Input.compass.trueHeading;
    }

    #region // Private Functions
    #endregion
    #region // Protected Functions
    #endregion
    #region // Public Functions
    #endregion






}
