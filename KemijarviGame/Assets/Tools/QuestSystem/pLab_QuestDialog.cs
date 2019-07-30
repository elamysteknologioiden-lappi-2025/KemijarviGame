/******************************************************************************
 * File         : pLab_QuestDialog.cs            
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
    [SerializeField]
    private Text title;

    /// <summary>
    /// text
    /// </summary>
    [SerializeField]
    private Text text;

    /// <summary>
    /// qManager
    /// </summary>
    [SerializeField]
    private pLab_QuestManager qManager;

    /// <summary>
    /// currentNode
    /// </summary>
    private pLab_QuestNode currentNode;

    /// <summary>
    /// currentEndNode
    /// </summary>
    private pLab_QuestNode currentEndNode;

    /// <summary>
    /// ActivateQuest
    /// </summary>
    /// <param name="aQuest"></param>
    public void ActivateQuest(Quest aQuest){
       // gameObject.SetActive(true);
        quest = aQuest;
        ProgressNode();

    }

    /// <summary>
    /// ReturnQuest
    /// </summary>
    public void ReturnQuest(){
        currentEndNode = quest.FinalizeQuest();

        if (currentEndNode == null){
            qManager.ChangeQuestStatus(3, quest.questID);
            gameObject.SetActive(false);
            return;
        }
        title.text = currentEndNode.title;
        text.text = currentEndNode.text;
    }

    /// <summary>
    /// ProgressNode
    /// </summary>
    public void ProgressNode(){
        Debug.LogError("---"+ gameObject.activeSelf);
        currentNode = quest.DoQuest();
        if (currentNode == null){
            if (quest.questType == 1)
            {
                Debug.LogError("-ss--");
                qManager.ChangeQuestStatus(3, quest.questID);
            }
            if (quest.questType == 2){
                Debug.LogError("-aa--");
                qManager.ChangeQuestStatus(2, quest.questID);
            }
            Debug.LogError("--sdadsad-");
            gameObject.SetActive(false);
            return;
        }
        Debug.LogError("--dasdasasassaa-"+ currentNode.title + " -- " +gameObject.activeSelf);
        title.text = currentNode.title;
        text.text = currentNode.text;


    }
}