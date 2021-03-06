﻿/******************************************************************************
 * File         : pLab_QuestDialog.cs            
 * Lisence      : BSD 3-Clause License
 * Copyright    : Lapland University of Applied Sciences
 * Authors      :
 * Note         : Prototype Code, Please Fix
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
/// pLab_QuestDialog
/// </summary>
public class pLab_QuestDialog : MonoBehaviour{

    /// <summary>
    /// quest
    /// </summary>
    [SerializeField] private Quest quest;

    /// <summary>
    /// title
    /// </summary>
    [SerializeField] private Text title;

    /// <summary>
    /// text
    /// </summary>
    [SerializeField] private Text text;


    /// <summary>
    /// text
    /// </summary>
    [SerializeField] private string stringText;

    /// <summary>
    /// qManager
    /// </summary>
    [SerializeField] private pLab_QuestManager qManager;

    /// <summary>
    /// bgImage
    /// </summary>
    [SerializeField] private Image bgImage;

    /// <summary>
    /// characterImage
    /// </summary>
    [SerializeField] private RawImage characterImage;

    /// <summary>
    /// Camera transform
    /// </summary>
    [SerializeField] private Transform rawCameraTransform;

    /// <summary>
    /// playerImage
    /// </summary>
    [SerializeField] private GameObject playerImage;

    /// <summary>
    /// characterDialog
    /// </summary>
    [SerializeField] private GameObject characterDialog;


    /// <summary>
    /// playerDialog
    /// </summary>
    [SerializeField] private GameObject playerDialog;

    /// <summary>
    /// How long to wait before adding a character to text
    /// </summary>
    [SerializeField] private float waitTimePerCharacter = 0.02f;

    /// <summary>
    /// currentNode
    /// </summary>
    private pLab_QuestNode currentNode;

    /// <summary>
    /// currentEndNode
    /// </summary>
    private pLab_QuestNode currentEndNode;

    /// <summary>
    /// returnQuest
    /// </summary>
    private bool returnQuest;

    /// <summary>
    /// 
    /// </summary>
    private IEnumerator coroutine;

    /// <summary>
    /// done
    /// </summary>
    private bool done = true;

    /// <summary>
    /// Currently "loaded" character gameobject with the animator
    /// </summary>
    private GameObject currentAnimatorGo;

    /// <summary>
    /// Currently "loaded" character's animator
    /// </summary>
    private Animator currentAnimator;

    /// <summary>
    /// Wait time cached
    /// </summary>
    private WaitForSeconds waitTimeSeconds;

    public delegate void QuestDialogActivityChangedEvent(bool questDialogActive);
    /// <summary>
    /// Event for when quest dialog activity is changed
    /// </summary>
    public static event QuestDialogActivityChangedEvent OnQuestDialogActivityChangedEvent;


    #region Inherited Methods
    
    private void Awake() {
        waitTimeSeconds = new WaitForSeconds(waitTimePerCharacter);
    }

    private void Start() {
        if (OnQuestDialogActivityChangedEvent != null) {
            OnQuestDialogActivityChangedEvent(gameObject.activeInHierarchy);
        }
    }

    private void OnEnable() {
        coroutine = WaitAndPrint();

        if (OnQuestDialogActivityChangedEvent != null) {
            OnQuestDialogActivityChangedEvent(true);
        }
    }

    private void OnDisable() {
        done = true;

        if (coroutine != null) {
            StopCoroutine(coroutine);
        }

        if (OnQuestDialogActivityChangedEvent != null) {
            OnQuestDialogActivityChangedEvent(false);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// ActivateQuest
    /// </summary>
    /// <param name="aQuest"></param>
    public void ActivateQuest(Quest aQuest){
        returnQuest = false;
        bgImage.sprite = Resources.Load<Sprite>("BGImages/" + aQuest.bgImage);
        quest = aQuest;
        ProgressNode();
    }


    /// <summary>
    /// ReturnQuest
    /// </summary>
    public void ReturnQuest(){
        bgImage.sprite = Resources.Load<Sprite>("BGImages/" + quest.bgEndImage);
        returnQuest = true;

        if (ShowFullText()) {
            return;
        }

        currentEndNode = quest.FinalizeQuest();

        if (currentEndNode == null){
            SetActiveState(false);
            qManager.ChangeQuestStatus(3, quest.questID);
        } else {
            UpdateDialogUIFromNode(currentEndNode);
        }
    }

    /// <summary>
    /// ProgressNode
    /// </summary>
    public void ProgressNode(){

        if (returnQuest){
            ReturnQuest();
            return;
        } else if (ShowFullText()) {
            return;
        }

        currentNode = quest.DoQuest();

        if (currentNode == null){
            if (quest.questType == 1)
            {
                if(quest.endNodes.Count == 0)
                    qManager.ChangeQuestStatus(3, quest.questID);
                else
                    qManager.ChangeQuestStatus(2, quest.questID);
            }
            if (quest.questType == 2){
                qManager.ChangeQuestStatus(2, quest.questID);
            }
            SetActiveState(false);
        } else {
            UpdateDialogUIFromNode(currentNode);
            SetActiveState(true);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Sets quest dialog gameobject active or inactive based on isOn
    /// </summary>
    /// <param name="isOn">Should the gameobject be activated</param>
    private void SetActiveState(bool isOn) {
        gameObject.SetActive(isOn);
    }

    /// <summary>
    /// Updates the dialog window taking info from the node
    /// </summary>
    /// <param name="node"></param>
    private void UpdateDialogUIFromNode(pLab_QuestNode node) {

        //Destroy previous character gameobject
        if (currentAnimatorGo != null) {
            currentAnimator = null;
            Destroy(currentAnimatorGo);
            currentAnimatorGo = null;
        }

        stringText = node.text;
        bool isPlayerNode = node.title == "Pelaaja";

        if (isPlayerNode) {
            currentAnimatorGo = GameObject.Instantiate(Resources.Load<GameObject>("Characters/postikusti"), rawCameraTransform);
        } else {
            currentAnimatorGo = GameObject.Instantiate(Resources.Load<GameObject>("Characters/" + node.image + "_game"), rawCameraTransform);
        }
        
        title.text = isPlayerNode ? pLab_KJPOCSaveGame.Instance.SaveData.playerName : node.title;

        playerDialog.SetActive(isPlayerNode);
        playerImage.SetActive(isPlayerNode);

        characterImage.gameObject.SetActive(!isPlayerNode);
        characterDialog.SetActive(!isPlayerNode);

        currentAnimator = currentAnimatorGo.GetComponent<Animator>();

        if (currentAnimator != null) {
            currentAnimator.SetTrigger("Speak");
        }

        text.text = "";

        StopCoroutine(coroutine);
        coroutine = WaitAndPrint();
        StartCoroutine(coroutine);
    }

    /// <summary>
    /// Show the whole text at once if not already shown
    /// </summary>
    /// <returns>True if the whole text not already shown</returns>
    private bool ShowFullText() {
        bool textWasNotDone = false;

        if (!done) {
            done = true;
            textWasNotDone = true;
        }

        text.text = stringText;

        if (currentAnimator != null) {
            currentAnimator.SetTrigger("Stop");
        }

        if (coroutine != null) {
            StopCoroutine(coroutine);
        }

        return textWasNotDone;
    }

    #endregion

    #region Coroutines & Enumerators

    /// <summary>
    /// Add character to text one by one
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitAndPrint() {
        done = false;
        while (!done) {
            yield return waitTimeSeconds;
            int textCount = text.text.Length;

            if (textCount == stringText.Length - 1) {
                ShowFullText();
            } else {
                text.text = stringText.Substring(0, textCount + 1);
            }
        }
    }

    #endregion
}