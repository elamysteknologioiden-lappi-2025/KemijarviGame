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

    private const string GAMESAVE_FILENAME = "gamesave.save";

    private string gamesavePath;

    #endregion

    #region // Class Attributes

    #endregion

    #region // Public Attributes

    /// <summary>
    /// Save data
    /// </summary>
    private pLAB_KJPOCGameData saveData = null;

    /// <summary>
    /// Instance
    /// </summary>
    private static pLab_KJPOCSaveGame instance;

    #endregion

    #region // Protected Attributes

    #endregion

    #region // Set/Get

    public static pLab_KJPOCSaveGame Instance { get { return instance; } }

    public pLAB_KJPOCGameData SaveData { get { return saveData; } }
    
    
    #endregion

    #region // Base Class Methods

    private void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }

        gamesavePath = string.Format("{0}/{1}", Application.persistentDataPath, GAMESAVE_FILENAME);
    }

    /// <summary>
    /// Unity calls this function when application is closing down
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    /// <summary>
    /// Unity calls this function when application is paused
    /// </summary>
    private void OnApplicationPause(bool isPaused) {
        #if !UNITY_EDITOR
        if (isPaused) {
            SaveGame();
        }
        #endif

    }

    #endregion

    #region // Private Methods

    /// <summary>
    /// CreateSaveGameObject
    /// </summary>
    private void CreateSaveGameObject(string playerName)
    {
        saveData = new pLAB_KJPOCGameData();

        saveData.score = 0;
        saveData.playerName = playerName;

        saveData.questSystem = QuestList.LoadQuest("Quests/QuestList");

    }

    #endregion

    #region // Public Methods

    /// <summary>
    /// Create a new blank save data for player
    /// </summary>
    public void CreateNewGame(string playerName)
    {
        CreateSaveGameObject(playerName);
        SaveGame();
    }

    /// <summary>
    /// Save save data to file
    /// </summary>
    public void SaveGame()
    {
        if(null != saveData)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(gamesavePath);
            bf.Serialize(file, saveData);
            file.Close();
        }
    }

    /// <summary>
    /// Check if there is a save file
    /// </summary>
    public bool IsThereSave()
    {
        return File.Exists(gamesavePath);
    }


    /// <summary>
    /// Load save data from file
    /// </summary>
    public void LoadGame()
    {
        if (File.Exists(gamesavePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(gamesavePath, FileMode.Open);
            saveData = (pLAB_KJPOCGameData) bf.Deserialize(file);
            file.Close();
        }
    }

    #endregion
}
