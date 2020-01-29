/******************************************************************************
 * File         : pLab_KJPOCMainMenu.cs            
 * Lisence      : BSD 3-Clause License
 * Copyright    : Lapland University of Applied Sciences
 * Authors      : Toni Westerlund (toni.westerlund@lapinamk.fi)
                  Arto Söderström (arto.soderstrom@lapinamk.fi)
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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// MainMenu-script
/// </summary>
public class pLab_KJPOCMainMenu : MonoBehaviour
{
    #region Variables

    [SerializeField] private Button continueButton;

    [SerializeField] private GameObject areUSureDialog;

    [SerializeField] private GameObject nameDialog;

    [SerializeField] private InputField playerNameInputField;

    [SerializeField] private string mainSceneName = "Level_001 AR Kemijarvi";

    #endregion

    #region Inherited Methods

    private void Start(){
        continueButton.interactable = pLab_KJPOCSaveGame.Instance.IsThereSave();
    }

    private void Update(){
        if (Application.platform == RuntimePlatform.Android){
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Application.Quit();
            }
        }
    }

    #endregion

    #region Private Methods

    private void LoadMainScene() {
        SceneManager.LoadScene(mainSceneName);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Starts a new fresh game
    /// </summary>
    public void StartNewGame(){
        string name = playerNameInputField.text.ToString();

        if(name.Length < 1){
            name = "Pelaaja";
        }
        pLab_KJPOCSaveGame.Instance.CreateNewGame(name);
        SceneManager.LoadScene("Level_001 AR");
    }

    /// <summary>
    /// Set player name query element active
    /// </summary>
    public void PlayerNameQuery(){
        areUSureDialog.SetActive(false);
        nameDialog.SetActive(true);
    }

    /// <summary>
    /// Sets "are you sure" dialog active if there is a save. Otherwise starts a new game
    /// </summary>
    public void PrepareToStartNewGame(){
        if (pLab_KJPOCSaveGame.Instance.IsThereSave()) {
            areUSureDialog.SetActive(true);
        }
        else {
            StartNewGame();
        }
    }

    /// <summary>
    /// Loads gamesave and loads the main scene
    /// </summary>
    public void LoadGame() {
        pLab_KJPOCSaveGame.Instance.LoadGame();
        LoadMainScene();
    }

    #endregion
}
