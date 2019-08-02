/******************************************************************************
 * File         : public class pLab_KJPOCSaveGame : MonoBehaviour
.cs            
 * Lisence      : BSD 3-Clause License
 * Copyright    : Lapland University of Applied Sciences
 * Authors      : Toni Westerlund (toni.westerlund@lapinamk.fi),
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
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class pLAB_KJPOCGameData
{
    public QuestList questSystem;
    public int score = 0;
    public string playerName;
}

public class pLab_KJPOCSaveGame : MonoBehaviour
{
    #region // SerializeField
    #endregion

    #region // Private Attributes

    /// <summary>
    /// pLAB_KJPOCGameData
    /// </summary>
    public pLAB_KJPOCGameData saveData = null;




    #endregion

    #region // Class Attributes

    #endregion

    #region // Public Attributes

    /// <summary>
    /// pLab_KJPOCSaveGame
    /// </summary>
    public static pLab_KJPOCSaveGame instance;

    #endregion

    #region // Protected Attributes

    #endregion

    #region // Set/Get

    #endregion

    #region // Base Class Methods

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        instance = this;
    }
    void OnDestroy()
    {
#if UNITY_EDITOR
        Debug.Log("On Destroy called");
        OnApplicationQuit();
#endif
    }
    /// <summary>
    /// OnApplicationQuit
    /// </summary>
    private void OnApplicationQuit()
    {

        SaveGame();
    }

    #endregion

    #region // Private Methods

    /// <summary>
    /// CreateSaveGameObject
    /// </summary>
    private void CreateSaveGameObject(string aName)
    {
        saveData = new pLAB_KJPOCGameData();

        saveData.score = 0;
        saveData.playerName = aName;

        saveData.questSystem = QuestList.LoadQuest("Quests/QuestList");
        if(saveData.questSystem == null)
        {
        }

    }


    #endregion

    #region // Public Methods

    /// <summary>
    /// CreateNewGame
    /// </summary>
    public void CreateNewGame(string aName)
    {
        CreateSaveGameObject(aName);
        SaveGame();
    }

    /// <summary>
    /// SaveGame
    /// </summary>
    public void SaveGame()
    {
        if(null == saveData)
        {
            LoadGame();
        }


        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, saveData);
        file.Close();
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsThereSave()
    {
        return File.Exists(Application.persistentDataPath + "/gamesave.save");
    }


    /// <summary>
    /// LoadGame
    /// </summary>
    public void LoadGame()
    {

        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            saveData = (pLAB_KJPOCGameData)bf.Deserialize(file);
            file.Close();

        }
        else
        {
            //CreateNewGame();
        }
    }
    #endregion
}
