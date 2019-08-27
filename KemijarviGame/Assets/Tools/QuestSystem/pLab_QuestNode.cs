/******************************************************************************
 * File         : pLab_QuestNode.cs            
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

using System.Collections.Generic;

using System.Xml.Serialization;
using UnityEngine;

/// <summary>
/// pLab_QuestNode
/// </summary>
public class pLab_QuestNode{
    [XmlElementAttribute("Title")] public string title;

    [XmlElementAttribute("Text")] public string text;

    [XmlElementAttribute("Image")]public string image;
}

/// <summary>
/// Quest
/// </summary>
public class Quest
{
    #region // Class Attributes

    /// <summary>
    /// Nodes
    /// </summary>
    [XmlArray("Nodes")]
    [XmlArrayItem("Node")]
    public List<pLab_QuestNode> nodes;

    /// <summary>
    /// EndNodes
    /// </summary>
    [XmlArray("EndNodes")]
    [XmlArrayItem("EndNode")]
    public List<pLab_QuestNode> endNodes;

    /// <summary>
    /// questTitle
    /// </summary>
    [XmlElementAttribute("QuestTitle")]
    public string questTitle;

    /// <summary>
    /// bgImage
    /// </summary>
    [XmlElementAttribute("BGImage")]
    public string bgImage;

    /// <summary>
    /// bgEndImage
    /// </summary>
    [XmlElementAttribute("BGENDImage")]
    public string bgEndImage;
    
    /// <summary>
    /// questType
    /// </summary>
    [XmlElementAttribute("QuestType")]
    public int questType;

    /// <summary>
    /// questID
    /// </summary>
    [XmlElementAttribute("QuestID")]
    public int questID;


    /// <summary>
    /// npcId
    /// </summary>
    [XmlElementAttribute("NPCID")]
    public int npcId;

    /// <summary>
    /// endPoint
    /// </summary>
    [XmlElementAttribute("EndPoint")]
    public int endPoint;

    /// <summary>
    /// currentNode
    /// </summary>
    [System.NonSerialized]private pLab_QuestNode currentNode;

    /// <summary>
    /// currentEndNode
    /// </summary>
    [System.NonSerialized]private pLab_QuestNode currentEndNode;

    /// <summary>
    /// position
    /// </summary>
    [System.NonSerialized]private int position;

    /// <summary>
    /// endposition
    /// </summary>
    [System.NonSerialized]private int endPosition;
    #endregion

    #region // From Base Class


    #endregion

    #region // Private Functions


    /// <summary>
    /// IsNext
    /// </summary>
    /// <param name="aNodes"></param>
    /// <param name="aPosition"></param>
    /// <returns></returns>
    private bool IsNext(List<pLab_QuestNode> aNodes, int aPosition){
        return aNodes != null && aPosition < aNodes.Count;
    }
    #endregion

    #region // Protected Functions
    #endregion

    #region // Public Functions

    /// <summary>
    /// DoQuest
    /// </summary>
    /// <returns></returns>
    public pLab_QuestNode DoQuest(){
        if (IsNext(nodes, position)){
            currentNode = nodes[position++];
            return currentNode;
        }
        return null;
    }


    /// <summary>
    /// FinalizeQuest()
    /// </summary>
    /// <returns></returns>
    public pLab_QuestNode FinalizeQuest() {
        if (IsNext(endNodes, endPosition)) {
            currentEndNode = endNodes[endPosition++];
            return currentEndNode;
        }
        return null;
    }

    #endregion
}