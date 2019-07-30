/******************************************************************************
 * File         : pLab_GeoPositionCharacter.cs            
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
using UnityEngine.XR;


/// <summary>
/// GeoCharacterMoveType
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

    #endregion

    #region // From Base Class


    /// <summary>
    /// 
    /// </summary>
    void Start() {
        objectTransform = GetComponent<Transform>();
        XRSettings.enabled = false;
        desiredPos = transform.position;
    }


    private void OnEnable() {
    }


    public Text pos;
    private float startTime;
    private Vector3 startPosition = Vector3.zero;
    public Vector3 desiredPos = Vector3.zero;
    public Vector3 changePos = new Vector3();
    public Vector3 hitPos;
    float speed = 0;
    public Animator characterAnimator;
    float timeDif = 0;
    float distance = 0;
    public Vector3 lastPos = new Vector3();

    Vector3 newPos;


    float timme;
    /// <summary>
    /// 
    /// </summary>
    void Update() {

     /*   if (Input.GetMouseButtonDown(0))
        {


            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                pLab_NPC npc = hit.collider.gameObject.GetComponent<pLab_NPC>();

                if (null != npc)
                {
                    Debug.LogError("AKTIVOI");
                    npc.ActivateNPC();
                }
            }

        }
        return;*/

        if(startPosition == Vector3.zero){
            startPosition = transform.position;
        }
        if (desiredPos == Vector3.zero) {
            desiredPos = transform.position;
        }
        newPos = new Vector3((float)( pLab_GeoLocation.instance.UtmX - pLab_GeoMap.instance.UtmX),
            0f, (float)(pLab_GeoLocation.instance.UtmY - pLab_GeoMap.instance.UtmY));

        // If moved, compute new pos;
        if (Vector3.Distance(newPos, desiredPos) > 0.1f) {

            // Compute new position distance
            distance = Vector3.Distance(newPos, transform.position);

            // Time last update
            timeDif = Time.time - startTime;

            if(timeDif > 0.5f){
                    timeDif = 0.5f;
            }

            speed = distance / timeDif;
            speed = speed * 0.8f;
            if (speed < 0.05f){
                speed = 0.05f;
            }

            startTime = Time.time;

            startPosition = transform.position;
            desiredPos = newPos;
            timme = 0;

            Debug.LogError(startPosition + " speed" + speed);


        }

        
        float distCovereda = (Time.time - startTime) * speed;
        float fracJourneya = distCovereda / distance;

        if(speed == 0)
        {
            fracJourneya = 1;
        }

        if(fracJourneya > 1)
        {
            speed = 0;
        }

        // transform.position = desiredPos;
        timme += Time.deltaTime;
        characterAnimator.SetFloat("Speed", speed);

        transform.position = Vector3.Lerp(startPosition, desiredPos, fracJourneya);

        Vector3 targetDir = newPos - startPosition;
        float angle = Vector3.Angle(targetDir, transform.forward);
        Vector3 localRota = characterAnimator.gameObject.transform.localEulerAngles;
        localRota.y = angle;
        if (targetDir.x < 0)
            localRota.y *= -1;
        characterAnimator.gameObject.transform.localEulerAngles = localRota;

    }


    #endregion

    #region // Private Functions



    #endregion

    #region // Protected Functions
    #endregion

    #region // Public Functions


    public void ChangeMoveType(Slider slider) {
        geoCharacterMoveType = (GeoCharacterMoveType)slider.value;
    }

    /// <summary>
    /// SetObjectToPosition
    /// </summary>
    /// <param name="aPos"></param>
    public void SetObjectToPosition(Vector2 aPos) {

        Ray ray = Camera.main.ScreenPointToRay(aPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f)) {
           // objectTransform.position = hit.point;
        }
        hitPos = hit.point;

    }

    #endregion



}
