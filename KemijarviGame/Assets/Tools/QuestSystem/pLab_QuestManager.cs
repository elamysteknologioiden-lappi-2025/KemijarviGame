/******************************************************************************
 * File         : pLab_QuestManager.cs            
 * Lisence      : BSD 3-Clause License
 * Copyright    : Lapland University of Applied Sciences
 * Authors      : 
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
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

/// <summary>
/// pLab_QuestManager
/// </summary>
public class pLab_QuestManager : MonoBehaviour{

    public delegate void ActivateQuest(int aId, UnityEngine.Events.UnityAction aAction, Quest aCurrentQuest, bool startPoint);
    public static event ActivateQuest OnActivateQuest;

    public delegate void ActivateQuestReturn(int aId, UnityEngine.Events.UnityAction aAction, Quest aCurrentQuest, bool startPoint);
    public static event ActivateQuestReturn OnActivateQuestReturn;

    public delegate void DisableQuest(int aId);
    public static event DisableQuest OnDisableQuest;


    /// <summary>
    /// questDialog
    /// </summary>
    [SerializeField]private GameObject questDialog;

    /// <summary>
    /// endDialog
    /// </summary>
    [SerializeField]private GameObject endDialog;

    /// <summary>
    /// currentQuest
    /// </summary>
    private Quest currentQuest;

    /// <summary>
    /// Start
    /// </summary>
    void Start(){
        CheckQuest();
    }

    /*
    private void OnEnable() {
        //Maybe we should do this here instead
        //so if we disable stuff between mode changes,this stuff will update also
        CheckQuest();
    }
    */

    /// <summary>
    /// CheckQuest
    /// </summary>
    public void CheckQuest() {
        if (pLab_KJPOCSaveGame.instance != null && pLab_KJPOCSaveGame.instance.saveData != null && pLab_KJPOCSaveGame.instance.saveData.questSystem != null) {
            foreach (QuestItem item in pLab_KJPOCSaveGame.instance.saveData.questSystem.nodes) {
                if (item.status == 0 && (item.prevQuest == 0 || pLab_KJPOCSaveGame.instance.saveData.
                    questSystem.nodes.Find(x => x.questID == item.prevQuest).status == 3)){

                    if (item.type == 1){
                        item.status = 2;
                        StartQuest("Quests/" + item.questID + "_Quest");
                    } else if (item.type == 2) {
                        item.status = 1;
                        StartQuestLate("Quests/" + item.questID + "_Quest");
                    } else if (item.type == 6) {
                        item.status = 3;

                        endDialog.SetActive(true);
                    }
                }if (item.status == 1){
                    if (OnActivateQuest != null) {
                        UnityEngine.Events.UnityAction action = () => { Debug.LogError("Herra"); };
                        OnActivateQuest(currentQuest.npcId, action, currentQuest, true);
                    }
                }
                if (item.status == 2 && item.endPoint != 0) {
                    UnityEngine.Events.UnityAction action = () => {  };
                    OnActivateQuestReturn(currentQuest.endPoint, action, currentQuest,false);
                }
            }
        }
        
    }

    /// <summary>
    /// StartQuestLate
    /// </summary>
    /// <param name="aFilePath"></param>
    public void StartQuestLate(string aFilePath){
        TextAsset xml = Resources.Load<TextAsset>(aFilePath);
        XmlSerializer serializer = new XmlSerializer(typeof(Quest));
        StringReader reader = new StringReader(xml.ToString());
        currentQuest = serializer.Deserialize(reader) as Quest;
        reader.Close();
    }

    /// <summary>
    /// StartQuest
    /// </summary>
    /// <param name="aFilePath"></param>
    public void StartQuest(string aFilePath){
        TextAsset xml = Resources.Load<TextAsset>(aFilePath);
        XmlSerializer serializer = new XmlSerializer(typeof(Quest));
        StringReader reader = new StringReader(xml.ToString());
        currentQuest = serializer.Deserialize(reader) as Quest;
        reader.Close();
        questDialog.gameObject.SetActive(true);
        questDialog.GetComponent<pLab_QuestDialog>().ActivateQuest(currentQuest);
    }

    /// <summary>
    /// ChangeQuestStatus
    /// </summary>
    /// <param name="aStatus"></param>
    /// <param name="aQuestID"></param>
    public void ChangeQuestStatus(int aStatus, int aQuestID){

        pLab_KJPOCSaveGame.instance.saveData.
            questSystem.nodes.Find(x => x.questID == aQuestID).status = aStatus;
        if(aStatus == 3) {
            OnDisableQuest(currentQuest.npcId);
            OnDisableQuest(currentQuest.endPoint);
            currentQuest = null;
        }

        CheckQuest();
    }
}
