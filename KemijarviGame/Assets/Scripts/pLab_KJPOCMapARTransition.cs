/******************************************************************************
* File         : pLab_KJPOCMapARTransition.cs
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
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Handles UI and logical transitions between Map and AR-modes
/// </summary>
public class pLab_KJPOCMapARTransition : MonoBehaviour
{

    #region Variables

    private const string AR_BUTTON_ANIMATOR_HIGHLIGHT_PARAMETER = "HighlightEnabled";

    [SerializeField]
    private Button transitionToARButton;

    [SerializeField]
    private Button transitionToMapButton;
    
    [SerializeField]
    [Tooltip("All objects that should be enabled for AR, and disabled otherwise")]
    private List<GameObject> setActiveForAR = new List<GameObject>();

    [SerializeField]
    [Tooltip("All objects that should be enabled for Map, and disabled otherwise")]
    private List<GameObject> setActiveForMap = new List<GameObject>();

    private Animator transitionToARButtonAnimator;

    #endregion

    #region Inherited Methods

    private void Awake() {
        if (transitionToARButton != null) {
            transitionToARButtonAnimator = transitionToARButton.GetComponent<Animator>();
        }
    }

    private void OnEnable() {
        if (transitionToARButton != null) {
            transitionToARButton.onClick.AddListener(TransitionToAR);
        }

        if (transitionToMapButton != null) {
            transitionToMapButton.onClick.AddListener(TransitionToMap);
        }

        pLab_QuestDialog.OnQuestDialogActivityChangedEvent += OnQuestDialogActivityChanged;

        ARSession.stateChanged += OnARSessionStateChanged;

    }

    private void OnDisable() {
        if (transitionToARButton != null) {
            transitionToARButton.onClick.RemoveListener(TransitionToAR);
        }

        if (transitionToMapButton != null) {
            transitionToMapButton.onClick.RemoveListener(TransitionToMap);
        }

        pLab_QuestDialog.OnQuestDialogActivityChangedEvent -= OnQuestDialogActivityChanged;

        ARSession.stateChanged -= OnARSessionStateChanged;
    }

    private void Start() {
        NavigationMode currentNavigationMode = pLab_KJPOCGameManager.Instance != null ? pLab_KJPOCGameManager.Instance.NavigationMode : NavigationMode.Map;
        ChangeNavigationMode(currentNavigationMode);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Transition to AR-mode. Enables AR-specific objects.
    /// </summary>
    public void TransitionToAR() {
        ChangeNavigationMode(NavigationMode.AR);
    }

    /// <summary>
    /// Transition to Map-mode. Disables AR-specific objects
    /// </summary>
    public void TransitionToMap() {
        ChangeNavigationMode(NavigationMode.Map);
    }

    /// <summary>
    /// Changes navigation mode. Handles transition and informs GameManager
    /// </summary>
    /// <param name="navigationMode"></param>
    public void ChangeNavigationMode(NavigationMode navigationMode) {
        bool isARMode = navigationMode == NavigationMode.AR;

        if (pLab_KJPOCGameManager.Instance != null) {
            pLab_KJPOCGameManager.Instance.NavigationMode = navigationMode;
        }

        if (transitionToMapButton != null) {
            transitionToMapButton.gameObject.SetActive(isARMode);
        }

        if (transitionToARButton != null) {
            transitionToARButton.gameObject.SetActive(!isARMode);
        }

        ToggleMapObjects(!isARMode);
        ToggleARObjects(isARMode);

    }

    /// <summary>
    /// Highlights or disables the button based on isHighlighted.
    /// </summary>
    /// <param name="isHighlighted"></param>
    public void ToggleARButtonHighlight(bool isHighlighted) {
        transitionToARButton.interactable = isHighlighted;

        if (transitionToARButtonAnimator != null) {
            transitionToARButtonAnimator.SetBool(AR_BUTTON_ANIMATOR_HIGHLIGHT_PARAMETER, isHighlighted);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Toggle active state of all AR-objects
    /// </summary>
    /// <param name="isOn"></param>
    private void ToggleARObjects(bool isOn) {
        for(int i = 0; i < setActiveForAR.Count; i++) {
            setActiveForAR[i].SetActive(isOn);
        }
    }

    /// <summary>
    /// Toggle active state of all Map-objects
    /// </summary>
    /// <param name="isOn"></param>
    private void ToggleMapObjects(bool isOn) {
        for(int i = 0; i < setActiveForMap.Count; i++) {
            setActiveForMap[i].SetActive(isOn);
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Event handler for ARSessionStateChanged-event. If state is ARSessionState.Unsupported, the option to choose AR-mode is deactivated
    /// </summary>
    /// <param name="evt"></param>
    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs evt)
    {
        if (evt.state == ARSessionState.Unsupported) {
            ChangeNavigationMode(NavigationMode.Map);

            if (transitionToMapButton != null) {
                transitionToMapButton.gameObject.SetActive(false);
            }

            if (transitionToARButton != null) {
                transitionToARButton.gameObject.SetActive(false);
            }

            this.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Event handler for OnQuestDialogActivityChangedEvent. Changes activity of the transition UI-buttons
    /// </summary>
    /// <param name="questDialogActive"></param>
    private void OnQuestDialogActivityChanged(bool questDialogActive)
    {
        //Both buttons inactive if the quest dialog is open
        bool isARButtonActive = false;
        bool isMapButtonActive = false;

        if (!questDialogActive)
        {
            NavigationMode currentMode = pLab_KJPOCGameManager.Instance != null ? pLab_KJPOCGameManager.Instance.NavigationMode : NavigationMode.Map;

            isARButtonActive = currentMode == NavigationMode.Map;
            isMapButtonActive = currentMode == NavigationMode.AR;
        }
        
        if (transitionToMapButton != null) {
            transitionToMapButton.gameObject.SetActive(isMapButtonActive);
        }

        if (transitionToARButton != null) {
            transitionToARButton.gameObject.SetActive(isARButtonActive);
        }

    }
    #endregion
}
