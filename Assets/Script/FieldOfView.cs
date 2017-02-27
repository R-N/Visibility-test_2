using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityStandardAssets.ImageEffects;

public class FieldOfView : MonoBehaviour {
	public Camera darkCam = null;
	public Camera visibleCam = null;
	public Camera frontCam = null;
	public Camera depthCam = null;

	Transform camTrans = null;
	Vector3 camPos = Vector3.zero;
	Vector3 camDir = Vector3.forward;

	public RenderTexture rt = null;
	public Shader maskShad = null;
	public Shader combineShad = null;
	public Shader distShad = null;
	public Shader emptyShad = null;
	public Shader wpShad = null;
	public Shader wpShad2 = null;
	Material maskMat = null;
	Material combineMat = null;
	Material emptyMat = null;

	public Transform topCamTrans = null;
	public Transform camRot = null;

	public BlurOptimized m_blur = null;

	RenderTexture temp = null;

	void Start(){
		camTrans = frontCam.transform;
		camPos = camTrans.position;
		camDir = camTrans.forward;

		darkCam.targetTexture = new RenderTexture (Screen.width, Screen.height, 16);
		//visibleCam.targetTexture = new RenderTexture (Screen.width, Screen.height, 24);
		darkCam.depthTextureMode = DepthTextureMode.None;
		visibleCam.depthTextureMode = DepthTextureMode.None;
		frontCam.depthTextureMode = DepthTextureMode.None;
		depthCam.depthTextureMode = DepthTextureMode.None;
		depthCam.targetTexture = new RenderTexture (Screen.width, Screen.height, 16);
		/*if (SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.R8)) {
			rt = new RenderTexture (Screen.width, Screen.height, 0, RenderTextureFormat.R8);
		} else*/
			rt = new RenderTexture (Screen.width, Screen.height, 0);
		frontCam.targetTexture =  new RenderTexture (Screen.width, Screen.height, 16);
		maskMat = new Material (maskShad);
		combineMat = new Material (combineShad);
		emptyMat = new Material (emptyShad);



		/*frontCam.SetReplacementShader (wpShad, string.Empty);
		depthCam.SetReplacementShader (wpShad2, string.Empty);*/
		frontCam.SetReplacementShader (distShad, string.Empty);
		depthCam.SetReplacementShader (distShad, string.Empty);
	}

	void LateUpdate(){
		if (topCamTrans.hasChanged) {
			topCamTrans.rotation = Quaternion.identity;
			Shader.SetGlobalMatrix ("_XRAMatrix", (GL.GetGPUProjectionMatrix (visibleCam.projectionMatrix, false) * visibleCam.worldToCameraMatrix).inverse);
		}
		if (camTrans.hasChanged){
			camPos = camTrans.position;
			camDir = camTrans.forward;
			Shader.SetGlobalVector ("_PersCamFwd", camDir);
			Shader.SetGlobalVector ("_PersCamFwdXZ", new Vector3(camDir.x, 0, camDir.z).normalized);
			Shader.SetGlobalVector ("_PersCamPos", camPos);
			Matrix4x4 viewMatrix = GL.GetGPUProjectionMatrix (frontCam.projectionMatrix, false) * frontCam.worldToCameraMatrix;
			Shader.SetGlobalMatrix ("_WorldToPersCam", viewMatrix);
			Shader.SetGlobalMatrix ("_XRAMatrix2", viewMatrix.inverse);
		}
		//Shader.SetGlobalMatrix ("_XRAMatrix", visibleCam.cameraToWorldMatrix * GL.GetGPUProjectionMatrix (visibleCam.projectionMatrix, false).inverse);
		//Shader.SetGlobalMatrix("_WorldToLocalWP", frontCam.transform.worldToLocalMatrix);
	//	Shader.SetGlobalMatrix("_WorldToLocalWP2", visibleCam.transform.worldToLocalMatrix);
		//Shader.SetGlobalMatrix("_LocalToWorldWP", frontCam.transform.localToWorldMatrix);
		//Shader.SetGlobalMatrix("_LocalToWorldWP2", visibleCam.transform.localToWorldMatrix);
		temp = RenderTexture.active;
		RenderTexture.active = rt;
		GL.Clear (true, true, Color.black);
		RenderTexture.active = frontCam.targetTexture;
		GL.Clear (true, true, Color.clear);
		RenderTexture.active = depthCam.targetTexture;
		GL.Clear (true, true, Color.clear);
		RenderTexture.active = temp;

	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination){
		//Graphics.Blit (frontCam.targetTexture, destination);

		//frontCam.Render();
		//depthCam.Render ();
		maskMat.SetTexture ("_PersDepthTex", frontCam.targetTexture);
		maskMat.SetTexture ("_CameraDepthTexture2", depthCam.targetTexture);
		//Graphics.Blit (rt, destination, maskMat, 0);
		Graphics.Blit (rt, rt, maskMat, 0);
		//m_blur.OnRenderImage (rt, rt);
		//darkCam.Render ();
		combineMat.SetTexture ("_DarkTex", darkCam.targetTexture);
		combineMat.SetTexture ("_MaskTex", rt);
		Graphics.Blit (source, destination, combineMat, 0);


		//Graphics.Blit (source, destination, mat, 0);
	}
}