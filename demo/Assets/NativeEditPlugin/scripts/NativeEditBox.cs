/*
 * Copyright (c) 2015 Kyungmin Bang
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */


/*
 *  NativeEditBox script should be attached to Unity UI InputField object 
 * 
 *  Limitation
 * 
 * 1. Screen auto rotation is not supported.
 */


using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NativeEditBox : PluginMsgReceiver {
	private struct EditBoxConfig
	{
		public bool multiline;
		public Color textColor;
		public Color backColor;
		public string contentType;
		public string font;
		public float fontSize;
		public string align; 
		public string placeHolder;
	}

	public bool	withDoneButton = true;

	private bool	bNativeEditCreated = false;

	private InputField	objUnityInput;
	private Text		objUnityText;
	
	private static string MSG_CREATE = "CreateEdit";
	private static string MSG_REMOVE = "RemoveEdit";
	private static string MSG_SET_TEXT = "SetText";
	private static string MSG_GET_TEXT = "GetText";
	private static string MSG_SET_RECT = "SetRect";
	private static string MSG_SET_FOCUS = "SetFocus";
	private static string MSG_SET_VISIBLE = "SetVisible";
	private static string MSG_TEXT_CHANGE = "TextChange";
	private static string MSG_TEXT_END_EDIT = "TextEndEdit";
	private static string MSG_ANDROID_KEY_DOWN = "AndroidKeyDown"; // to fix bug Some keys 'back' & 'enter' are eaten by unity and never arrive at plugin

	public static Rect GetScreenRectFromRectTransform(RectTransform rectTransform)
	{
		Vector3[] corners = new Vector3[4];
		
		rectTransform.GetWorldCorners(corners);
		
		float xMin = float.PositiveInfinity;
		float xMax = float.NegativeInfinity;
		float yMin = float.PositiveInfinity;
		float yMax = float.NegativeInfinity;
		
		for (int i = 0; i < 4; i++)
		{
			// For Canvas mode Screen Space - Overlay there is no Camera; best solution I've found
			// is to use RectTransformUtility.WorldToScreenPoint) with a null camera.
			Vector3 screenCoord = RectTransformUtility.WorldToScreenPoint(null, corners[i]);
			
			if (screenCoord.x < xMin)
				xMin = screenCoord.x;
			if (screenCoord.x > xMax)
				xMax = screenCoord.x;
			if (screenCoord.y < yMin)
				yMin = screenCoord.y;
			if (screenCoord.y > yMax)
				yMax = screenCoord.y;
		}
		Rect result = new Rect(xMin, Screen.height - yMax, xMax - xMin, yMax - yMin);
		return result;
	}

	private EditBoxConfig mConfig;

	void Awake()
	{
	//	base.Awake();
	}


	protected new void OnDestroy()
	{
		base.OnDestroy();
	}

	void OnEnable()
	{
		if (bNativeEditCreated) this.SetVisible(true);
	}

	void OnDisable()
	{
		if (bNativeEditCreated) this.SetVisible(false);
	}

	// Use this for initialization
	new void Start () {
		base.Start();

		bNativeEditCreated = false;
		this.PrepareNativeEdit();

		#if (UNITY_IPHONE || UNITY_ANDROID) &&!UNITY_EDITOR 
		this.CreateNativeEdit();
		this.SetTextNative(this.objUnityText.text);
		
		objUnityInput.placeholder.enabled = false;
		objUnityText.enabled = false;
		objUnityInput.enabled = false;
		#endif
	}
	
	// Update is called once per frame
	void Update () {
		this.UpdateForceKeyeventForAndroid();
	}
	
	private void PrepareNativeEdit()
	{
		objUnityInput = this.GetComponent<InputField>();
		if (objUnityInput == null)
		{
			Debug.LogErrorFormat("No InputField found {0} NativeEditBox Error", this.name);
			throw new MissingComponentException();
		}
		
		Graphic placeHolder = objUnityInput.placeholder;
		objUnityText = objUnityInput.textComponent;
		
		mConfig.placeHolder = placeHolder.GetComponent<Text>().text;
		mConfig.font = objUnityText.font.fontNames.Length > 0 ? objUnityText.font.fontNames[0] : "Arial";

		Rect rectScreen = GetScreenRectFromRectTransform(this.objUnityText.rectTransform);
		float fHeightRatio = rectScreen.height / objUnityText.rectTransform.rect.height;
		mConfig.fontSize = ((float) objUnityText.fontSize) * fHeightRatio;

		mConfig.textColor = objUnityText.color;
		mConfig.align = objUnityText.alignment.ToString();
		mConfig.contentType = objUnityInput.contentType.ToString();
		mConfig.backColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		mConfig.multiline = (objUnityInput.lineType == InputField.LineType.SingleLine) ? false : true;
	}

	private void onTextChange(string newText)
	{
		if (this.objUnityInput.onValueChange != null) this.objUnityInput.onValueChange.Invoke(newText);
	}

	private void onTextEditEnd(string newText)
	{
		if (this.objUnityInput.onEndEdit != null) this.objUnityInput.onEndEdit.Invoke(newText);
	}

	public override void OnPluginMsgDirect(JsonObject jsonMsg)
	{
		string msg = jsonMsg.GetString("msg");
		if (msg.Equals(MSG_TEXT_CHANGE))
		{
			string text = jsonMsg.GetString("text");
			this.onTextChange(text);
		}
		else if (msg.Equals(MSG_TEXT_END_EDIT))
		{
			string text = jsonMsg.GetString("text");
			this.onTextEditEnd(text);
		}
	}

	private bool CheckErrorJsonRet(JsonObject jsonRet)
	{
		bool bError = jsonRet.GetBool("bError");
		string strError = jsonRet.GetString("strError");
		if (bError)
		{
			msgHandler.FileLogError(string.Format("NativeEditbox error {0}", strError));
		}
		return bError;
	}

	private void CreateNativeEdit()
	{
		Rect rectScreen = GetScreenRectFromRectTransform(this.objUnityText.rectTransform);

		JsonObject jsonMsg = new JsonObject();

		jsonMsg["msg"] = MSG_CREATE;

		jsonMsg["x"] = rectScreen.x / Screen.width;
		jsonMsg["y"] = rectScreen.y / Screen.height;
		jsonMsg["width"] = rectScreen.width / Screen.width;
		jsonMsg["height"] = rectScreen.height / Screen.height;

		jsonMsg["textColor_r"] = mConfig.textColor.r;
		jsonMsg["textColor_g"] = mConfig.textColor.g;
		jsonMsg["textColor_b"] = mConfig.textColor.b;
		jsonMsg["textColor_a"] = mConfig.textColor.a;
		jsonMsg["backColor_r"] = mConfig.backColor.r;
		jsonMsg["backColor_g"] = mConfig.backColor.g;
		jsonMsg["backColor_b"] = mConfig.backColor.b;
		jsonMsg["backColor_a"] = mConfig.backColor.a;
		jsonMsg["font"] = mConfig.font;
		jsonMsg["fontSize"] = mConfig.fontSize;
		jsonMsg["contentType"] = mConfig.contentType;
		jsonMsg["align"] = mConfig.align;
		jsonMsg["withDoneButton"] = this.withDoneButton;
		jsonMsg["placeHolder"] = mConfig.placeHolder;
		jsonMsg["multiline"] = mConfig.multiline;

		JsonObject jsonRet = this.SendPluginMsg(jsonMsg);
		bNativeEditCreated = !this.CheckErrorJsonRet(jsonRet);
	}

	public void SetTextNative(string newText)
	{
		JsonObject jsonMsg = new JsonObject();
		
		jsonMsg["msg"] = MSG_SET_TEXT;
		jsonMsg["text"] = newText;

		this.SendPluginMsg(jsonMsg);
	}

	public string GetTextNative()
	{
		JsonObject jsonMsg = new JsonObject();
		
		jsonMsg["msg"] = MSG_GET_TEXT;
		JsonObject jsonRet = this.SendPluginMsg(jsonMsg);
		bool bError = this.CheckErrorJsonRet(jsonRet);

		if (bError) return "";

		Debug.Log(string.Format("GetTextNative {0}", jsonRet.GetString("text")));
		return jsonRet.GetString("text");
	}

	private void RemoveNative()
	{
		JsonObject jsonMsg = new JsonObject();
		
		jsonMsg["msg"] = MSG_REMOVE;
		this.SendPluginMsg(jsonMsg);
	}

	public void SetRectNative(RectTransform rectTrans)
	{
		Rect rectScreen = GetScreenRectFromRectTransform(rectTrans);

		JsonObject jsonMsg = new JsonObject();
		
		jsonMsg["msg"] = MSG_SET_RECT;

		jsonMsg["x"] = rectScreen.x / Screen.width;
		jsonMsg["y"] = rectScreen.y / Screen.height;
		jsonMsg["width"] = rectScreen.width / Screen.width;
		jsonMsg["height"] = rectScreen.height / Screen.height;

		this.SendPluginMsg(jsonMsg);
	}

	public void SetFocusNative(bool bFocus)
	{
		JsonObject jsonMsg = new JsonObject();
		
		jsonMsg["msg"] = MSG_SET_FOCUS;
		jsonMsg["isFocus"] = bFocus;
		
		this.SendPluginMsg(jsonMsg);
	}

	public void SetVisible(bool bVisible)
	{
		JsonObject jsonMsg = new JsonObject();
		
		jsonMsg["msg"] = MSG_SET_VISIBLE;
		jsonMsg["isVisible"] = bVisible;
		
		this.SendPluginMsg(jsonMsg);
	}

	void ForceSendKeydown_Android(string key)
	{
		JsonObject jsonMsg = new JsonObject();
		
		jsonMsg["msg"] = MSG_ANDROID_KEY_DOWN;
		jsonMsg["key"] = key;
		this.SendPluginMsg(jsonMsg);
	}


	void UpdateForceKeyeventForAndroid()
	{
		#if UNITY_ANDROID &&!UNITY_EDITOR

		if (Input.anyKeyDown)
		{
			if (Input.GetKeyDown(KeyCode.Backspace))
			{
				this.ForceSendKeydown_Android("backspace");
			}
			else
			{
				foreach(char c in Input.inputString)
				{
					if (c == "\n"[0])
					{
						this.ForceSendKeydown_Android("enter");
					}
				}
			}
		}	
		#endif	
	}

}
