﻿/******************************************************************************
* File         : pLab_KJPOC_ARNPCCanvas.cs
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
using UnityEngine.UI;

public class pLab_KJPOC_ARNPCCanvas : MonoBehaviour
{

    #region Variables

    [SerializeField]
    private GameObject questIcon;

    private Image questIconImage;

    private Animator questIconAnimator;

    [SerializeField]
    private Sprite questAvailableSprite;

    [SerializeField]
    private Sprite questReturnSprite;

    private const string ANIMATOR_HIGHLIGHT_PARAMETER = "HighlightEnabled";


    #endregion

    #region Inherited Methods

    private void Awake() {
        if (questIcon != null) {
            questIconImage = questIcon.GetComponent<Image>();
            questIconAnimator = questIcon.GetComponent<Animator>();
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Show available quest icon
    /// </summary>
    /// <param name="isOn"></param>
    public void ShowAvailableQuestIcon() {
        if (questIcon != null) {
            if (questIconImage != null) {
                questIconImage.sprite = questAvailableSprite;
            }
            questIcon.SetActive(true);
        }
    }

    /// <summary>
    /// Show quest return icon
    /// </summary>
    public void ShowQuestReturnIcon() {
        if (questIcon != null) {
            if (questIconImage != null) {
                questIconImage.sprite = questReturnSprite;
            }
            questIcon.SetActive(true);
        }
    }

    /// <summary>
    /// Hides quest icon
    /// </summary>
    public void HideQuestIcon() {
        if (questIcon != null) {
            questIcon.SetActive(false);
        }
    }

    /// <summary>
    /// Toggle highlight visiblity
    /// </summary>
    public void ToggleHighlight(bool isOn) {
        if (questIcon != null) {
            if (questIconAnimator != null) {
                questIconAnimator.SetBool(ANIMATOR_HIGHLIGHT_PARAMETER, isOn);
            }
        }
    }

    #endregion


}
