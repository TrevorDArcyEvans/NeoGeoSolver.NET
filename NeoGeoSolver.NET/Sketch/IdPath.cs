﻿using System.Globalization;
using System.Xml;

namespace NeoGeoSolver.NET.Sketch;

public class IdPath {
	public List<Id> path = new List<Id>();

	static readonly char[] pathSeparator = new char[] {'/'};
	static readonly char[] secondSeparator = new char[] {':'};

	public bool Empty() {
		return path.Count == 0;
	}
	public void Parse(string str) {
		var ids = str.Split(pathSeparator, StringSplitOptions.RemoveEmptyEntries);
		path.Clear();
		for(int i = 0; i < ids.Length; i++) {
			var vals = ids[i].Split(secondSeparator);

			int sign = (vals[0][0] == '-') ? -1 : 1;
			if(sign < 0) vals[0] = vals[0].Remove(0, 1);

			long value = sign * long.Parse(vals[0], NumberStyles.HexNumber);
			long second = 0; 
			if(vals.Length > 1) {
				sign = (vals[1][0] == '-') ? -1 : 1;
				if(sign < 0) vals[1] = vals[1].Remove(0, 1);
				second = sign * long.Parse(vals[1], NumberStyles.HexNumber);
			}
			path.Add(new Id(value, second));
		}

	}

	public IdPath Clone() {
		var result = new IdPath();
		result.path = new List<Id>(path);
		return result;
	}

	public IdPath With(Id id) {
		var result = Clone();
		result.path.Add(id);
		return result;
	}

	public static IdPath From(string str) {
		var result = new IdPath();
		result.Parse(str);
		return result;
	}

	public override string ToString() {
		string result = "";
		for(int i = 0; i < path.Count; i++) {
			if(i != 0) result += pathSeparator[0];
			if(path[i].value < 0) result += "-";
			result += Math.Abs(path[i].value).ToString("X");
			if(path[i].second != 0) {
				result += ":";
				if(path[i].second < 0) result += "-";
				result += Math.Abs(path[i].second).ToString("X");
				// "1-2-3-4:3"
			}
		}
		return result;
	}

	public void Write(XmlTextWriter xml, string name) {
		var data = ToString();
		if(data == "") return;
		xml.WriteStartElement("ref");
		xml.WriteAttributeString("name", name);
		xml.WriteAttributeString("path", data);
		xml.WriteEndElement();
	}

	public void Read(XmlNode xml) {
		path.Clear();
		Parse(xml.Attributes["path"].Value);
	}

	public static bool operator==(IdPath a, IdPath b) {
		if(a.path.Count != b.path.Count) return false;
		for(int i = 0; i < a.path.Count; i++) {
			if(a.path[i] != b.path[i]) return false;
		}
		return true;
		
	}

	public static bool operator!=(IdPath a, IdPath b) {
		return !(a == b);
	}

	public override int GetHashCode() {
		int result = 0;
		int shift = 0;
		for (int i = 0; i < path.Count; i++) {
			shift = (shift + 11) % 21;
			result ^= ((int)path[i].value + 1024) << shift;
		}
		return result;
	}

	public override bool Equals(object obj) {
		return this == obj as IdPath;
	}
}