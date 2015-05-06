using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

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


/// <summary>
///  JsonObject provide safe and type-compatible wrapper to access Json Element
/// Safe means, even if a certain Json doesn't contain 'key', it doesn't crash.
/// </summary>
public class JsonObject : UnityEngine.Object {
	/// <summary>
	/// Supported object type : double, bool, string, List, Array!
	/// </summary>
	public Dictionary<string, System.Object> 	m_dict;

	public JsonObject()
	{
		m_dict = new Dictionary<string, System.Object>();
	}

	public JsonObject(string strJson)
	{
		m_dict = MiniJSON_Min.Json.Deserialize(strJson) as Dictionary<string,object>;
	}	
	
	public JsonObject(Dictionary<string, System.Object> dict)
	{
		m_dict = dict;
	}
	
	public System.Object this[string key]
	{
		get { return m_dict[key]; }
		set { m_dict[key] = value; }
	}
	
	public string getCmd() { return (string) m_dict["cmd"]; }
	
	public string Serialize()
	{
		string strData = MiniJSON_Min.Json.Serialize(m_dict);
		return strData;
	}

	//// Deserialize helper

	private object GetDictValue(string key)
	{
		System.Object obj;
		if (m_dict.TryGetValue(key, out obj)) 
		{
			return ((obj != null) ? obj : "");
		}
		return "";
	}

	public bool keyExist(string key)
	{
		System.Object obj;
		if (m_dict.TryGetValue(key, out obj)) 
		{
			return true;
		}
		return false;
	}
	
	public Dictionary<string, object> GetJsonDict(string key)
	{
		System.Object obj;
		if (m_dict.TryGetValue(key, out obj)) return  (Dictionary<string, object>) obj;
		return new Dictionary<string, object>();
	}

	public JsonObject GetJsonObject(string key)
	{
		Dictionary<string, object> dict = this.GetJsonDict(key);
		return new JsonObject(dict);
	}
	
	public bool GetBool(string key)
	{
		bool val;
		if (bool.TryParse(this.GetDictValue(key).ToString(), out val)) return val;
		return false;
	}
	
	public int GetInt(string key)
	{
		int val;
		if (int.TryParse(this.GetDictValue(key).ToString(), out val)) return val;
		return 0;
	}
	
	public float GetFloat(string key)
	{
		float val;
		if (float.TryParse(this.GetDictValue(key).ToString(), out val)) return val;
		return 0.0f;
	}
	
	public string GetString(string key)
	{
		return (string) this.GetDictValue(key).ToString();
	}
	
	public object GetEnum(System.Type type, string key)
	{
		string obj = (string) this.GetDictValue(key);
		if (obj.Length > 0)
		{
			return System.Enum.Parse(type, obj);
		}
		return 0;
	}
	
	public List<JsonObject> GetListJsonObject(string key)
	{
		System.Object obj;
		List<JsonObject> retList = new List<JsonObject>();
		if (m_dict.TryGetValue(key, out obj)) 
		{
			foreach(object elem in (List<object>) obj)
			{
				retList.Add(new JsonObject( (Dictionary<string, System.Object>) elem));
			}
		}
		return retList;
	}
}
