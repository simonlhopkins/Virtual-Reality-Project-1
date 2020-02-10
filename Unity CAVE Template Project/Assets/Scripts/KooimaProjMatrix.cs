using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class KooimaProjMatrix : MonoBehaviour {


	public Vector3[] corners;
    public int wallNumber;

	public enum eyeOffset{left, right};
	public eyeOffset whichEyeIfStereo = eyeOffset.left;

	Transform trackerPosition;
	bool stereo;
	float interpupillaryDistance;
	Vector3 viewingLocation;


    public void Update () {
		Camera cam = GetComponent<Camera>();

		//get new tracker position (as updated over the network)
		//getting it off the "ViewUpdate" component on the projection root (parent), along with IPD if needed
		trackerPosition = GameObject.FindObjectOfType<OSCEventListener>().projectionRoot;
		stereo = CustomNetworkManager.stereo;
		interpupillaryDistance = CustomNetworkManager.interpupillaryDistance;

		if (stereo){
			if (whichEyeIfStereo == eyeOffset.left){
				//cam.transform.localPosition = new Vector3(-interpupillaryDistance/2, 0, 0);

				viewingLocation = new Vector3( trackerPosition.localPosition.x-interpupillaryDistance/2, trackerPosition.localPosition.y, trackerPosition.localPosition.z);
				cam.transform.localPosition = viewingLocation;
			}
			else{
				//cam.transform.localPosition = new Vector3(+interpupillaryDistance/2, 0, 0);

				viewingLocation =  new Vector3( trackerPosition.localPosition.x+interpupillaryDistance/2, trackerPosition.localPosition.y, trackerPosition.localPosition.z);
				cam.transform.localPosition = viewingLocation;
			}
		}

		//Vector3 offset = new Vector3(0.0f, trackerPosition.localPosition.y, trackerPosition.localPosition.z);
		//Vector3 offset = cam.transform.localPosition;

		Matrix4x4 generatedProjection = Kooima2(
			corners[0], corners[1], 
			corners[2], viewingLocation, 
			cam.nearClipPlane, cam.farClipPlane);
		/*KooimaPerspectiveProjection(
                BottomLeftScreenCorner, BottomRightScreenCorner, 
                TopLeftScreenCorner, viewingLocation, 
                cam.nearClipPlane, cam.farClipPlane);*/


		cam.projectionMatrix = generatedProjection;  
		//cam.cullingMatrix = generatedProjection;

	}

	public Matrix4x4 Kooima2(Vector3 pa, Vector3 pb, Vector3 pc, Vector3 pe, float n, float f) {
		Vector3 va, vb, vc;
		Vector3 vr, vu, vn;
		float l, r, b, t, d;
		Matrix4x4 M;


		//Deal with reversed Z for left-handed coordinate system

		pa.z = -pa.z;
		pb.z = -pb.z;
		pc.z = -pc.z;
		pe.z = -pe.z;


		// compute orthonormal basis for the screen

		vr = pb - pa;
		vu = pc - pa;

		vr.Normalize();
		vu.Normalize();
		vn = Vector3.Cross(vr, vu);
		vn.Normalize();

		// compute screen corner vectors

		va = pa - pe;
		vb = pb - pe;
		vc = pc - pe;

		// find distance from eye to screen plane

		d = -Vector3.Dot(va, vn);

		// find the extent of the perpendicular projection

		l = Vector3.Dot(vr, va) * n / d;
		r = Vector3.Dot(vr, vb) * n / d;
		b = Vector3.Dot(vu, va) * n / d;
		t = Vector3.Dot(vu, vc) * n / d;

		// load the perpendicular projection

		Matrix4x4 final = Matrix4x4.identity;
		final = final * Matrix4x4.Frustum(l, r, b, t, n, f);

		// rotate the projection to be non-perpendicular

		M = Matrix4x4.zero;
		M[0] = vr[0]; M[4] = vr[1]; M[ 8] = vr[2];
		M[1] = vu[0]; M[5] = vu[1]; M[ 9] = vu[2];
		M[2] = vn[0]; M[6] = vn[1]; M[10] = vn[2];
		M[15] = 1;

		final = final * M;

		// move the apex of the frustum to the origin

		final = final * Matrix4x4.Translate(pe);

		return final;
	}


	public Matrix4x4 KooimaPerspectiveProjection(Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 eyePos, float near, float far){
		Vector3 eyeToScreenBottomLeft, eyeToScreenBottomRight, eyeToScreenTopLeft;
		Vector3 orthoR, orthoU, orthoN;

		float left, right, bottom, top, eyedistance;     

		Matrix4x4 transformMatrix;
		Matrix4x4 projectionM;
		Matrix4x4 eyeTranslateM;
		Matrix4x4 finalProjection;

		//Deal with reversed Z for left-handed coordinate system

		bottomLeft.z = -bottomLeft.z;
		bottomRight.z = -bottomRight.z;
		topLeft.z = -topLeft.z;
		eyePos.z = -eyePos.z;

		///Calculate the orthonormal basis for the screen
		orthoR = bottomRight - bottomLeft;
		orthoR.Normalize();
		orthoU = topLeft - bottomLeft;
		orthoU.Normalize();
		orthoN = Vector3.Cross(orthoR, orthoU);
		orthoN.Normalize();

		//Screen corner vecs (frustum extent)
		eyeToScreenBottomLeft = bottomLeft-eyePos;
		eyeToScreenBottomRight = bottomRight-eyePos;
		eyeToScreenTopLeft = topLeft-eyePos;

		//Distance from the eye to the screen plane
		eyedistance = -(Vector3.Dot(eyeToScreenBottomLeft, orthoN));


		//Extent of the perpendicular projection
		left   = (Vector3.Dot(orthoR, eyeToScreenBottomLeft )*near)/eyedistance;
		right  = (Vector3.Dot(orthoR, eyeToScreenBottomRight)*near)/eyedistance;
		bottom = (Vector3.Dot(orthoU, eyeToScreenBottomLeft )*near)/eyedistance;
		top    = (Vector3.Dot(orthoU, eyeToScreenTopLeft    )*near)/eyedistance;

		GameObject.Find("Info").GetComponent<Text>().text =
			"left: " + left + "\n" +
			"right: " + right + "\n" +
			"bottom: " + bottom + "\n" +
			"top: " + top + "\n" +
			"eyedistance: " + eyedistance + "\n" +
			"orthoR: " + orthoR + "\n" +
			"orthoU: " + orthoU + "\n" +
			"orthoN: " + orthoN + "\n" +
			"bottom left: " + bottomLeft + "\n" +
			"bottom right: " + bottomRight + "\n" +
			"top left: " + topLeft + "\n";


		//"Load" projection using Unity's oblique projection
		projectionM = PerspectiveOffCenter(left, right, bottom, top, near, far);

		//Fill in the transform matrix with the orthonormal basis
		transformMatrix = new Matrix4x4();
		transformMatrix = Matrix4x4.identity;
		transformMatrix[0, 0] = orthoR.x;
		transformMatrix[0, 1] = orthoR.y;
		transformMatrix[0, 2] = orthoR.z;
		transformMatrix[1, 0] = orthoU.x;
		transformMatrix[1, 1] = orthoU.y;
		transformMatrix[1, 2] = orthoU.z;
		transformMatrix[2, 0] = orthoN.x;
		transformMatrix[2, 1] = orthoN.y;
		transformMatrix[2, 2] = orthoN.z;

		//Apex of frustum to eye transform
		eyeTranslateM = new Matrix4x4();
		eyeTranslateM = Matrix4x4.identity;
		eyeTranslateM[0, 3] = -eyePos.x;
		eyeTranslateM[1, 3] = -eyePos.y;
		eyeTranslateM[2, 3] = -eyePos.z;

		//Concatenate everything
		finalProjection = new Matrix4x4();
		finalProjection = Matrix4x4.identity * projectionM * transformMatrix * eyeTranslateM;

		return finalProjection;
	}

	static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far) {

		/*
		 the classic projection matrix, generalized to allow off-center viewing
		 
		 2n/(r-l)	0			(r+l)/(r-l)		0
		 0			2n/(t-b)	(t+b)/(t-b)		0
		 0			0			-(f+n)/(f-n)	-2nf/(f-n)
		 0			0			-1				0
		 */

		Matrix4x4 m = new Matrix4x4();
		m = Matrix4x4.identity;
		m[0, 0] = 2.0F * near / (right - left);
		m[0, 2] = (right + left) / (right - left);
		m[1, 1] = 2.0F * near / (top - bottom);
		m[1, 2] = (top + bottom) / (top - bottom);
		m[2, 2] = -(far + near) / (far - near);
		m[2, 3] = -(2.0F * far * near) / (far - near);
		m[3, 2] = -1.0f;
		m[3, 3] = 0;
		return m;
	}
}
