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

    public delegate void ActivateQuest(int aId, Quest aCurrentQuest, bool startPoint);
    public static event ActivateQuest OnActivateQuest;

    public delegate void ActivateQuestReturn(int aId, Quest aCurrentQuest, bool startPoint);
    public static event ActivateQuestReturn OnActivateQuestReturn;

    public delegate void DisableQuest(int aId);
    public static event DisableQuest OnDisableQuest;


    /// <summary>
    /// Quest dialog gameobject
    /// </summary>
    [SerializeField] private GameObject questDialog;

    /// <summary>
    /// End dialog gameobject
    /// </summary>
    [SerializeField] private GameObject endDialog;

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

    /// <summary>
    /// CheckQuest
    /// </summary>
    public void CheckQuest() {
        if (pLab_KJPOCSaveGame.Instance != null && pLab_KJPOCSaveGame.Instance.SaveData != null && pLab_KJPOCSaveGame.Instance.SaveData.questSystem != null) {
            foreach (QuestItem item in pLab_KJPOCSaveGame.Instance.SaveData.questSystem.nodes) {
                if (item.status == 0 && (item.prevQuest == 0 || pLab_KJPOCSaveGame.Instance.SaveData.
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
                }

                //item.status can be changed in the above if-block
                if (item.status == 1) {
                    if (currentQuest == null) {
                        StartQuestLate("Quests/" + item.questID + "_Quest");
                    }

                    if (OnActivateQuest != null) {
                        OnActivateQuest(currentQuest.npcId, currentQuest, true);
                    }
                }

                if (item.status == 2 && item.endPoint != 0) {
                    if (currentQuest == null) {
                        StartQuest("Quests/" + item.questID + "_Quest");
                    }

                    if (OnActivateQuestReturn != null) {
                        OnActivateQuestReturn(currentQuest.endPoint, currentQuest, false);
                    }
                }

                if (item.status == 3 && item.type == 6) {
                    endDialog.SetActive(true);
                }
            }
        }
        
    }

    /// <summary>
    /// Start Quest Late
    /// </summary>
    /// <param name="aFilePath"></param>
    public void StartQuestLate(string aFilePath){
        LoadCurrentQuest(aFilePath);
    }

    /// <summary>
    /// Start Quest
    /// </summary>
    /// <param name="aFilePath"></param>
    public void StartQuest(string aFilePath){
        LoadCurrentQuest(aFilePath);
        questDialog.gameObject.SetActive(true);
        questDialog.GetComponent<pLab_QuestDialog>().ActivateQuest(currentQuest);
    }

    /// <summary>
    /// Load current quest from XML-file
    /// </summary>
    /// <param name="aFilePath">Path to XML-file</param>
    private void LoadCurrentQuest(string aFilePath) {
        TextAsset xml = Resources.Load<TextAsset>(aFilePath);
        XmlSerializer serializer = new XmlSerializer(typeof(Quest));
        StringReader reader = new StringReader(xml.ToString());
        currentQuest = serializer.Deserialize(reader) as Quest;
        reader.Close();
    }

    /// <summary>
    /// ChangeQuestStatus
    /// </summary>
    /// <param name="aStatus"></param>
    /// <param name="aQuestID"></param>
    public void ChangeQuestStatus(int aStatus, int aQuestID){

        QuestItem questItem = pLab_KJPOCSaveGame.Instance.SaveData.
            questSystem.nodes.Find(x => x.questID == aQuestID);

        if (questItem != null) {
            questItem.status = aStatus;
        }
            
        if(aStatus == 3) {

            if (OnDisableQuest != null) {
                OnDisableQuest(currentQuest.npcId);
                OnDisableQuest(currentQuest.endPoint);  
            }

            currentQuest = null;
        }

        CheckQuest();
    }
}
