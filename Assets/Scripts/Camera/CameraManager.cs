using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
	[SerializeField]
	private CinemachineVirtualCamera followCamera;
	[SerializeField]
	private float initialCamSize;
	private Tween myTween;
	private void Start()
	{
		followCamera.m_Lens.OrthographicSize = initialCamSize;
		GameManager.Instance.cameraManager = this;
		followCamera.Follow = GameObject.FindWithTag(Tags.T_Player).transform;
	}

	public void ScaleCamera(int scale)
	{
		if(myTween != null)
		{
			myTween.Kill();
        }
        myTween =DOVirtual.Float(followCamera.m_Lens.OrthographicSize, initialCamSize * ((scale - 1) * 0.5f + 1), 1f, (x) =>
		{
			followCamera.m_Lens.OrthographicSize = x;
		}
		);
	}

    private void OnDisable()
    {
		followCamera.m_Lens.OrthographicSize = initialCamSize;
    }
}
