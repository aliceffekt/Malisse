﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SplashScreenView : View 
{
	#region Members and properties
	// constants
	
	// enums
	
	// public
	public float m_MinDelay = 0.5f;
	public float m_MaxDelay = 3.0f;
	public AudioClip m_SFXClick = null;
	public AudioClip m_Music = null;
	
	// protected
	
	// private
	private float m_CurrentCountdown = 0.0f;
	
	// properties
	#endregion
	
	#region Unity API
	private void Start()
	{
		AudioManager.Instance.PlayMusic(m_Music);
	}

	protected virtual void Update()
	{
		if (m_State != eState.OPENED)
		{
			return;
		}

		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Application.Quit();
		}

		bool success = false;
		if (m_CurrentCountdown < m_MaxDelay)
		{
			m_CurrentCountdown += Time.deltaTime;
			if (m_CurrentCountdown >= m_MaxDelay)
			{
				success = true;
			}
		}

		if (m_CurrentCountdown > m_MinDelay)
		{
			if (ControllerInputManager.Instance.GetButtonDown(ControllerInputManager.eButtonAliases.GRAB.ToString()).Count > 0 || 
			    ControllerInputManager.Instance.GetButtonDown(ControllerInputManager.eButtonAliases.START.ToString()).Count > 0)
			{
				AudioManager.Instance.PlaySFX(m_SFXClick);

				success = true;
			}
		}

		if (success)
		{
			FlowManager.Instance.TriggerAction("GO_TO_MAIN_MENU");
		}
	}
	#endregion
	
	#region Public Methods
	#endregion
	
	#region Protected Methods
	#endregion
	
	#region Private Methods
	#endregion
}
