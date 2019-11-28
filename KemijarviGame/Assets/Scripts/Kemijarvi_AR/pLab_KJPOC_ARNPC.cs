/******************************************************************************
* File         : pLab_KJPOC_ARNPC.cs
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

/// <summary>
/// NPC-class for AR-mode
/// </summary>
public class pLab_KJPOC_ARNPC : MonoBehaviour
{

    #region Variables

    [SerializeField]
    private int npcId;

    private pLab_NPC npc;

    [SerializeField]
    private GameObject highlightObject;

    #region Debug Variables
    [SerializeField]
    private Image debugImage;
    
    private Color32 normalColor = Color.white;

    private Color32 debugColor = Color.red;
    #endregion Debug Variables

    #endregion

    #region Inherited Methods
    
    private void Awake() {
        //Find the NPC corresponding to npcId
        pLab_NPC[] npcs = GameObject.FindObjectsOfType<pLab_NPC>();

        for(int i = 0; i < npcs.Length; i++) {
            if (npcs[i].Id == npcId) {
                npc = npcs[i];
            }
        }
    }

    private void Update() {
        if (highlightObject != null) {
            ToggleHighlight(npc != null && (npc.HasActiveQuest || npc.HasActiveQuestReturn));
        }
    }

    private void OnMouseUpAsButton() {
        ActivateNPC();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Activate corresponding NPC to get or return a quest. Calls ActivateNPC of the normal NPC
    /// </summary>
    public void ActivateNPC() {
        if (npc != null) {
            DebugHighlight();
            npc.ActivateNPC();
        }
    }
        
    #endregion

    #region Private Methods

    /// <summary>
    /// Toggle highlight of this NPC based on isOn
    /// </summary>
    /// <param name="isOn"></param>
    private void ToggleHighlight(bool isOn) {
        if (highlightObject != null) {
            highlightObject.SetActive(isOn);
        }    
    }

    #endregion
    
    #region Debug

    private void DebugHighlight() {
        if (debugImage != null) {
            Color32 newColor = debugImage.color == normalColor ? debugColor : normalColor;
            debugImage.color = newColor;
        }
    }

    #endregion
}
