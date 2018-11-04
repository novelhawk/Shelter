using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

// ReSharper disable once CheckNamespace
public class StyledItemButtonImageText : StyledItem
{
	public RawImage 	rawImageCtrl;
	public Text   		textCtrl;
	public Button		buttonCtrl;
	
	public class Data
	{
		public string Text { get; }
		public Texture2D Texture { get; }

		public Data(string _text, Texture2D texture)
		{
			Text = _text; 
			Texture = texture;
		}
	}

	public override Button GetButton () { return buttonCtrl; }
	public override Text GetText () { return textCtrl; }
	public override RawImage GetRawImage () { return rawImageCtrl; }

	// we accept a string, a texture2d, or a data object if we want both.
	public override void Populate(object o)
	{
		Texture2D tex = o as Texture2D;
		if (tex != null)
		{
			if (rawImageCtrl != null)
				rawImageCtrl.texture = tex;
			return;
		}

		if (!(o is Data d))
		{
			if (textCtrl != null)
				textCtrl.text = o.ToString();	// string..
			return;
		}
		
		if (rawImageCtrl != null)
			rawImageCtrl.texture = d.Texture;
		
		if (textCtrl != null)
			textCtrl.text = d.Text;
	}
}