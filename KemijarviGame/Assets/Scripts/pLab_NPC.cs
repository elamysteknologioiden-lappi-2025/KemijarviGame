/******************************************************************************
 * File         : pLab_NPC.cs            
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
using UnityEngine.EventSystems;

/// <summary>
/// pLab_NPC
/// </summary>
public class pLab_NPC : MonoBehaviour{

    #region Variables
    /// <summary>
    /// id
    /// </summary>
    [SerializeField]
    private int id;

    [SerializeField]
    [Tooltip("How close the player must be to activate NPC")]
    private float activationRadius = 25f;

    /// <summary>
    /// Icon indicator for available quest
    /// </summary>
    [SerializeField]
    private GameObject questPointSymbol;

    /// <summary>
    /// Icon indicator for quest return
    /// </summary>
    [SerializeField]
    private GameObject returnQuestPointSymbol;


    [SerializeField]
    private GameObject tempHighlightIcon;

    /// <summary>
    /// currentQuest
    /// </summary>
    private Quest currentQuest;

    /// <summary>
    /// questDialog
    /// </summary>
    [SerializeField]
    private pLab_QuestDialog questDialog;

    [SerializeField]
    private Transform playerTransform;

    private bool hasActiveQuest = false;
    private bool hasActiveQuestReturn = false;

    #endregion

    #region Properties
    
    public int Id { get { return id; } }
    public bool HasActiveQuest { get { return hasActiveQuest; } }
    public bool HasActiveQuestReturn { get { return hasActiveQuestReturn; } }
    
    #endregion


    #region Inherited Methods
    /// <summary>
    /// OnEnable
    /// </summary>
    void OnEnable(){
        pLab_QuestManager.OnActivateQuest += ActivateQuest;
        pLab_QuestManager.OnActivateQuestReturn += ActivateQuestReturn;
        pLab_QuestManager.OnDisableQuest += DisableQuest;

        ToggleHighlight((hasActiveQuest || hasActiveQuestReturn) && IsPlayerInsideActivationRadius());
    }


    /// <summary>
    /// OnDisable
    /// </summary>
    void OnDisable(){
        pLab_QuestManager.OnActivateQuest -= ActivateQuest;
        pLab_QuestManager.OnActivateQuestReturn -= ActivateQuestReturn;
        pLab_QuestManager.OnDisableQuest -= DisableQuest;
    }

    private void Start() {
        if (playerTransform == null) {
            GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
            playerTransform = playerGo.transform;
        }
    }

    private void Update() {
        //This could may be done when player actually moves
        ToggleHighlight((hasActiveQuest || hasActiveQuestReturn) && IsPlayerInsideActivationRadius());
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// ActivateQuest
    /// </summary>
    /// <param name="aId"></param>
    /// <param name="aCurrentQuest"></param>
    /// <param name="aFlag"></param>
    public void ActivateQuest(int aId, Quest aCurrentQuest, bool aFlag){
        if (aId == id){
            currentQuest = aCurrentQuest;
            hasActiveQuest = true;
        }
        else{
            hasActiveQuest = false;
            hasActiveQuestReturn = false;
            returnQuestPointSymbol.SetActive(hasActiveQuestReturn);
        }

        questPointSymbol.SetActive(hasActiveQuest);

    }

    /// <summary>
    /// ActivateQuestReturn
    /// </summary>
    /// <param name="aId"></param>
    /// <param name="aCurrentQuest"></param>
    /// <param name="aFlag"></param>
    public void ActivateQuestReturn(int aId, Quest aCurrentQuest, bool aFlag){
        if (aId == id) {
            // Debug.Log("Activate quest return for " + gameObject.name);
            currentQuest = aCurrentQuest;
            hasActiveQuestReturn = true;
        } else {
            hasActiveQuest = false;
            questPointSymbol.SetActive(hasActiveQuest);
            hasActiveQuestReturn = false;
        }

        returnQuestPointSymbol.SetActive(hasActiveQuestReturn);

    }

    /// <summary>
    /// ActivateQuest
    /// </summary>
    /// <param name="aId"></param>
    public void DisableQuest(int aId){
        if (aId == id){
            currentQuest = null;
            hasActiveQuest = false;
            hasActiveQuestReturn = false;
            questPointSymbol.SetActive(hasActiveQuest);
            returnQuestPointSymbol.SetActive(hasActiveQuestReturn);
        }

    }

    /// <summary>
    /// ActivateNPC
    /// </summary>
    public void ActivateNPC(){
        if (currentQuest == null || !IsPlayerInsideActivationRadius()) return;

        questDialog.gameObject.SetActive(true);

        if (currentQuest.endPoint == id) {
            questDialog.gameObject.SetActive(true);
            questDialog.ReturnQuest();
        }
        else {
            questDialog.ActivateQuest(currentQuest);
        }
    }

    /// <summary>
    /// Check if the player is close to this NPC
    /// </summary>
    /// <returns></returns>
    public bool IsPlayerInsideActivationRadius() {
        return playerTransform != null && (playerTransform.position - transform.position).magnitude <= activationRadius;
    }

    /// <summary>
    /// Toggle highlight of this NPC. Indicating the player is close
    /// </summary>
    /// <param name="isOn"></param>
    public void ToggleHighlight(bool isOn) {
        //TODO: Insert the actual highlight here
        if (tempHighlightIcon != null) {
            tempHighlightIcon.SetActive(isOn);
        }
    }

    private void OnMouseUpAsButton() {
        //Chech that there is not a UI element in front
        if (!EventSystem.current.IsPointerOverGameObject()) {
            ActivateNPC();
        }
    }

    #endregion
}
