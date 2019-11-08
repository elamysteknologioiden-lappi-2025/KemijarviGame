/******************************************************************************
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
    private bool done = false;

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


    #region Inherited Methods
    
    private void Awake() {
        waitTimeSeconds = new WaitForSeconds(waitTimePerCharacter);
    }

    private void OnEnable() {
        coroutine = WaitAndPrint();
    }

    private void OnDisable(){
        done = true;
        if (coroutine != null) {
            StopCoroutine(coroutine);
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
        Debug.Log("Return quest");
        bgImage.sprite = Resources.Load<Sprite>("BGImages/" + quest.bgEndImage);
        returnQuest = true; 

        if (ShowFullText(currentEndNode)) {
            return;
        }

        currentEndNode = quest.FinalizeQuest();

        if (currentEndNode == null){
            gameObject.SetActive(false);
            qManager.ChangeQuestStatus(3, quest.questID);
            return;
        }

        UpdateDialogUIFromNode(currentEndNode);

    }

    /// <summary>
    /// ProgressNode
    /// </summary>
    public void ProgressNode(){

        if (returnQuest){
            ReturnQuest();
            return;
        }

        if (ShowFullText(currentNode)) {
            return;
        }

        currentNode = quest.DoQuest();

        if (currentNode == null){
            if (quest.questType == 1){
                if(quest.endNodes.Count == 0)
                qManager.ChangeQuestStatus(3, quest.questID);
                else
                    qManager.ChangeQuestStatus(2, quest.questID);
            }
            if (quest.questType == 2){
                qManager.ChangeQuestStatus(2, quest.questID);
            }
            gameObject.SetActive(false);
            return;
        }

        UpdateDialogUIFromNode(currentNode);

        
        gameObject.SetActive(true);
    }

    #endregion

    #region Private Methods

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
        
        title.text = isPlayerNode ? pLab_KJPOCSaveGame.instance.saveData.playerName : node.title;

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
    /// <param name="node"></param>
    /// <returns>True if the whole text not already shown</returns>
    private bool ShowFullText(pLab_QuestNode node) {
        bool textWasNotDone = false;

        if (done == false && node != null) {
            if (currentAnimator != null) {
                currentAnimator.SetTrigger("Stop");
            }
            done = true;
            text.text = node.text;
            StopCoroutine(coroutine);
            textWasNotDone = true;;
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
                text.text = stringText;
                if (currentAnimator != null) {
                    currentAnimator.SetTrigger("Stop");
                }

                done = true;
            } else {
                text.text = stringText.Substring(0, textCount + 1);
            }
        }
    }

    #endregion
}