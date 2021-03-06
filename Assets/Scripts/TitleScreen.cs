﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
	public Button startButton, controlsButton;
    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(StartRandomBattlefield);
		controlsButton.onClick.AddListener(ShowControls);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void StartRandomBattlefield(){
		int sceneNo = new System.Random().Next(0, SceneManager.sceneCountInBuildSettings - 1) + 1;
		SceneManager.LoadScene(sceneNo, LoadSceneMode.Single);
	}
	
	void ShowControls(){
		
	}
}
