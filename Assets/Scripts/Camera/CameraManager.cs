using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
	[SerializeField]
	private CinemachineVirtualCamera followCamera;

	private void Update()
	{
		followCamera.Follow = GameObject.FindWithTag(Tags.T_Player).transform;
	}
}
