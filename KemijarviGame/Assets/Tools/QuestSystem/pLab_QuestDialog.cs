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
    public Quest quest;

    /// <summary>
    /// title
    /// </summary>
    [SerializeField]private Text title;

    /// <summary>
    /// text
    /// </summary>
    [SerializeField]private Text text;


    /// <summary>
    /// text
    /// </summary>
    [SerializeField]private string stringText;

    /// <summary>
    /// qManager
    /// </summary>
    [SerializeField]private pLab_QuestManager qManager;

    /// <summary>
    /// bgImage
    /// </summary>
    [SerializeField]private Image bgImage;

    /// <summary>
    /// characterImage
    /// </summary>
    [SerializeField]private RawImage characterImage;

    /// <summary>
    /// rawCamera
    /// </summary>
    [SerializeField]private GameObject rawCamera;

    /// <summary>
    /// playerImage
    /// </summary>
    [SerializeField]private GameObject playerImage;

    /// <summary>
    /// characterDialog
    /// </summary>
    [SerializeField]private GameObject characterDialog;


    /// <summary>
    /// playerDialog
    /// </summary>
    [SerializeField]private GameObject playerDialog;

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
    /// ActivateQuest
    /// </summary>
    /// <param name="aQuest"></param>
    public void ActivateQuest(Quest aQuest){
        returnQuest = false;
        bgImage.sprite = Resources.Load<Sprite>("BGImages/"+aQuest.bgImage);
        quest = aQuest;
        ProgressNode();

    }

    /// <summary>
    /// OnEnable
    /// </summary>
    private void OnEnable() {
        coroutine = WaitAndPrint(0.02f);
    }

    private void OnDisable(){
        done = true;
        StopCoroutine(coroutine);
    }

    /// <summary>
    /// ReturnQuest
    /// TODO: FIX, FAST PROTOTYPE CODE
    /// </summary>
    public void ReturnQuest(){
        bgImage.sprite = Resources.Load<Sprite>("BGImages/" + quest.bgEndImage);
        returnQuest = true; 

        if (done == false && currentEndNode != null){
            if (rawCamera.GetComponentInChildren<Animator>() != null) {
                rawCamera.GetComponentInChildren<Animator>().SetTrigger("Stop");
            }
            done = true;
            title.text = currentEndNode.title;
            text.text = currentEndNode.text;
            StopCoroutine(coroutine);
            return;
        }

        currentEndNode = quest.FinalizeQuest();

        if (currentEndNode == null){
            gameObject.SetActive(false);
            qManager.ChangeQuestStatus(3, quest.questID);
            return;
        }

        Animator[] arraya = rawCamera.GetComponentsInChildren<Animator>();
        foreach (Animator item in arraya) {
            DestroyImmediate(item.gameObject);
        }

        title.text = currentEndNode.title;
        stringText = currentEndNode.text;

        if (currentEndNode.title == "Pelaaja") {
            title.text = pLab_KJPOCSaveGame.instance.saveData.playerName;
            playerImage.SetActive(true);
            characterImage.gameObject.SetActive(false);
            GameObject.Instantiate(Resources.Load<GameObject>("Characters/postikusti"), rawCamera.gameObject.transform);
            rawCamera.GetComponentInChildren<Animator>().SetTrigger("Speak");

            characterDialog.SetActive(false);
            playerDialog.SetActive(true);
        } else {
            playerDialog.SetActive(false);
            characterDialog.SetActive(true);
            playerImage.SetActive(false);
            characterImage.gameObject.SetActive(true);
            GameObject.Instantiate(Resources.Load<GameObject>("Characters/" + currentEndNode.image + "_game"), rawCamera.gameObject.transform);
            rawCamera.GetComponentInChildren<Animator>().SetTrigger("Speak");
        }
        text.text = "";

        StopCoroutine(coroutine);
        coroutine = WaitAndPrint(0.02f);
        StartCoroutine(coroutine);
    }

    /// <summary>
    /// ProgressNode
    /// TODO: FIX, FAST PROTOTYPE CODE
    /// </summary>
    public void ProgressNode(){
        if (returnQuest){
            ReturnQuest();
            return;
        }
        if (done == false && currentNode != null) {
            if(rawCamera.GetComponentInChildren<Animator>() != null)
            rawCamera.GetComponentInChildren<Animator>().SetTrigger("Stop");
            done = true;
            title.text = currentNode.title;
            text.text = currentNode.text;
            StopCoroutine(coroutine);
            return;
        }

        Animator[] arraya = rawCamera.GetComponentsInChildren<Animator>();
        foreach (Animator item in arraya){
            DestroyImmediate(item.gameObject);
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

        title.text = currentNode.title;
        stringText = currentNode.text;

        if (currentNode.title == "Pelaaja") {
            title.text = pLab_KJPOCSaveGame.instance.saveData.playerName;
            playerImage.SetActive(true);
            characterImage.gameObject.SetActive(false);
            GameObject.Instantiate(Resources.Load<GameObject>("Characters/postikusti"), rawCamera.gameObject.transform);
            rawCamera.GetComponentInChildren<Animator>().SetTrigger("Speak");
            characterDialog.SetActive(false);
            playerDialog.SetActive(true);
        } else {
            playerDialog.SetActive(false);
            characterDialog.SetActive(true);
            playerImage.SetActive(false);
            characterImage.gameObject.SetActive(true);

            Animator[] array = rawCamera.GetComponentsInChildren<Animator>();
            foreach (Animator item in array) {
                Debug.LogError(item.gameObject);
                DestroyImmediate(item.gameObject);
            }
            Debug.LogError("Characters/" + currentNode.image + "_game");


            GameObject tmpObj = Resources.Load<GameObject>("Characters/" + currentNode.image + "_game");
            GameObject.Instantiate<GameObject>(tmpObj, rawCamera.gameObject.transform);
            if (rawCamera.GetComponentInChildren<Animator>() != null)
                rawCamera.GetComponentInChildren<Animator>().SetTrigger("Speak");

            Debug.LogError(rawCamera.GetComponentInChildren<Animator>().gameObject.name);

        }
        text.text = "";
        StopCoroutine(coroutine);
        coroutine = WaitAndPrint(0.02f);
        StartCoroutine(coroutine);
        gameObject.SetActive(true);
    }



    private IEnumerator WaitAndPrint(float waitTime) {
        done = false;
        while (!done) {
            yield return new WaitForSeconds(waitTime);
            int textCOunt = text.text.Length;
            if (textCOunt == stringText.Length) {
                if (rawCamera.GetComponentInChildren<Animator>() != null)
                    rawCamera.GetComponentInChildren<Animator>().SetTrigger("Stop");
                done = true;
            } else {
                text.text = stringText.Substring(0, textCOunt + 1);
            }
        }
    }
}