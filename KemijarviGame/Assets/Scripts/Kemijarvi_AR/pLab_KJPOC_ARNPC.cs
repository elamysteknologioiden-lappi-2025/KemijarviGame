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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private pLab_KJPOC_ARNPCCanvas npcCanvas;

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

    private void OnEnable() {
        pLab_QuestManager.OnActivateQuest += OnActivateQuest;
        pLab_QuestManager.OnActivateQuestReturn += OnActivateQuestReturn;
        pLab_QuestManager.OnDisableQuest += OnDisableQuest;

        CheckQuestIconState();
    }


    private void OnDisable() {
        pLab_QuestManager.OnActivateQuest -= OnActivateQuest;
        pLab_QuestManager.OnActivateQuestReturn -= OnActivateQuestReturn;
        pLab_QuestManager.OnDisableQuest -= OnDisableQuest;
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
            npc.ActivateNPC(false);
        }
    }
        
    #endregion

    #region Private Methods

    /// <summary>
    /// Checks quest icon state, and activates or deactivates quest icon
    /// </summary>
    private void CheckQuestIconState() {
        if (npc != null) {
            if (npc.HasActiveQuest) {
                ActivateQuest();
            } else if (npc.HasActiveQuestReturn) {
                ActivateQuestReturn();
            } else {
                DisableQuest();
            }
        }
    }

    /// <summary>
    /// Checks if highlight of this NPC should be on or off
    /// </summary>
    /// <param name="isOn"></param>
    private void CheckHighlight() {
        if (npc != null && npcCanvas != null) {
            bool highlightEnabled = npc.HasActiveQuest || npc.HasActiveQuestReturn;
            npcCanvas.ToggleHighlight(highlightEnabled);
        }
    }

    /// <summary>
    /// Activates available quest icon
    /// </summary>
    private void ActivateQuest()
    {
        if (npcCanvas != null) {
            npcCanvas.ShowAvailableQuestIcon();
        }

        CheckHighlight();
    }
    
    /// <summary>
    /// Activates quest return icon
    /// </summary>
    private void ActivateQuestReturn()
    {
        if (npcCanvas != null) {
            npcCanvas.ShowQuestReturnIcon();
        }

        CheckHighlight();
    }

    /// <summary>
    /// Disables quest icon
    /// </summary>
    private void DisableQuest() {
        if (npcCanvas != null) {
            npcCanvas.HideQuestIcon();
        }
    }

    #endregion


    #region Event Handlers

    /// <summary>
    /// Event handler for OnActivateQuest-event. Activates quest if NPC id matches
    /// </summary>
    /// <param name="aId"></param>
    /// <param name="aCurrentQuest"></param>
    /// <param name="startPoint"></param>
    private void OnActivateQuest(int aId, Quest aCurrentQuest, bool startPoint)
    {
        if (aId == npcId) {
            ActivateQuest();
        }
    }



    /// <summary>
    /// Event handler for OnActivateQuestReturn-event. Activates quest return if NPC id matches
    /// </summary>
    /// <param name="aId"></param>
    /// <param name="aCurrentQuest"></param>
    /// <param name="startPoint"></param>
    private void OnActivateQuestReturn(int aId, Quest aCurrentQuest, bool startPoint) {
        if (aId == npcId) {
            ActivateQuestReturn();
        }
    }


    /// <summary>
    /// Event handler for OnDisableQuest-event. Disabled quest if NPC id matches
    /// </summary>
    /// <param name="aId"></param>
    private void OnDisableQuest(int aId)
    {
        if (aId == npcId) {
            DisableQuest();
        }
    }


    #endregion

}
