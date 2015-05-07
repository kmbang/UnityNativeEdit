using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class demo : MonoBehaviour {

	public NativeEditBox testNativeEdit;
	public Canvas		mainCanvas;
	private RectTransform rectTrans;

	// Use this for initialization
	void Start () {
		rectTrans = testNativeEdit.transform.FindChild("Text").GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {

	
	}

	private string GetCurObjName()
	{
		string strObjName = "";
		GameObject objSel = EventSystem.current.currentSelectedGameObject;
		if (objSel != null && objSel.transform.parent != null)
		{
			strObjName = objSel.transform.parent.name;
		}

		return strObjName;
	}

	public void OnEditValugChanged(string str)
	{
		Text txt = this.GetComponent<Text>();
		txt.text = string.Format("[{0}] val changed {1}", this.GetCurObjName(), str);
	}

	public void OnEditEnded(string str)
	{
		Text txt = this.GetComponent<Text>();
		txt.text = string.Format("[{0}] edit ended {1}", this.GetCurObjName(), str);
	}

	private bool bTempFocus = false;
	private bool bTempVisible = false;
	public void OnButton1()
	{
		bTempFocus = !bTempFocus;
		Debug.Log("OnButton1 clicked");
		testNativeEdit.SetFocusNative(bTempFocus);
	}

	public void OnButton2()
	{
		Debug.Log("OnButton2 clicked");
		bTempVisible = !bTempVisible;
		testNativeEdit.SetVisible(bTempVisible);
	}

	public void OnButton3()
	{
		rectTrans.sizeDelta = new Vector2(-20.0f, -5.0f);
		testNativeEdit.SetRectNative(rectTrans);
		Debug.Log("OnButton3 clicked");
	}


	public void OnButton4()
	{
		//rectTrans.sizeDelta = new Vector2(20.0f, 5.0f);
		//testNativeEdit.SetRectNative(rectTrans);

		Text txt = this.GetComponent<Text>();
		string sCurText = testNativeEdit.GetTextNative();
		txt.text = string.Format("[{0}] GetText {1}", this.GetCurObjName(), sCurText);
		Debug.Log("OnButton4 clicked");
	}

	public void OnButton5()
	{
		Debug.Log("OnButton5 clicked");
		testNativeEdit.SetTextNative("TestText Set!!@#@5");
	}

	

}
