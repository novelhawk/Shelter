// Copyright (c) 2011 - 2018 Bob Berkebile (pixelplacment)
// Please direct any bugs/comments/suggestions to http://pixelplacement.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

/*
TERMS OF USE - EASING EQUATIONS
Open source under the BSD License.
Copyright (c)2001 Robert Penner
All rights reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the name of the author nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Mod;
using UnityEngine;
// ReSharper disable UnusedMember.Global

/// <summary>
/// <para>Version: 2.0.7</para>	 
/// <para>Author: Bob Berkebile (http://pixelplacement.com)</para>
/// <para>Support: http://itween.pixelplacement.com</para>
/// </summary>
// ReSharper disable once CheckNamespace
public class iTween : MonoBehaviour
{
	#region Variables

	private static readonly List<Hashtable> tweens = new List<Hashtable>();

	public string id, type, method;
	public EaseType easeType;
	public float time, delay;
	public LoopType loopType;

	public bool isRunning, isPaused;

	public string _name;
	private float runningTime, percentage;
	private float delayStarted;
	private bool isLocal, loop, reverse, wasPaused, physics;
	private Hashtable tweenArguments;
	private Space space;

	private delegate float EasingFunction(float start, float end, float Value);

	private delegate void ApplyTween();

	private EasingFunction ease;
	private ApplyTween apply;
	private AudioSource audioSource;
	private Vector3[] vector3s;
	private Vector2[] vector2s;
	private Color[,] colors;
	private float[] floats;
	private Rect[] rects;
	private CRSpline path;
	private Vector3 preUpdate;
	private Vector3 postUpdate;
	private NamedValueColor namedcolorvalue;

	private float lastRealTime;
	private bool useRealTime;
	private Transform thisTransform;


	public enum EaseType
	{
		easeInQuad,
		easeOutQuad,
		easeInOutQuad,
		easeInCubic,
		easeOutCubic,
		easeInOutCubic,
		easeInQuart,
		easeOutQuart,
		easeInOutQuart,
		easeInQuint,
		easeOutQuint,
		easeInOutQuint,
		easeInSine,
		easeOutSine,
		easeInOutSine,
		easeInExpo,
		easeOutExpo,
		easeInOutExpo,
		easeInCirc,
		easeOutCirc,
		easeInOutCirc,
		linear,
		spring,

		easeInBounce,
		easeOutBounce,
		easeInOutBounce,

		easeInBack,
		easeOutBack,
		easeInOutBack,

		easeInElastic,
		easeOutElastic,
		easeInOutElastic,

		punch
	}

	public enum LoopType
	{
		none,
		loop,
		pingPong
	}

	private enum NamedValueColor
	{
		_Color,
		_SpecColor,
		_Emission,
		_ReflectColor
	}

	#endregion

	#region Defaults

	private static class Defaults
	{
		public const float time = 1f;
		public const float delay = 0f;
		public const NamedValueColor namedColorValue = NamedValueColor._Color;
		public const LoopType loopType = LoopType.none;
		public const EaseType easeType = EaseType.easeOutExpo;
		public const float lookSpeed = 3f;
		public const bool isLocal = false;
		public const Space space = Space.Self;
		public const bool orientToPath = false;
		public static readonly Color color = Color.white;
		public const float updateTimePercentage = .05f;
		public const float updateTime = 1f * updateTimePercentage;
		public const float lookAhead = .05f;
		public const bool useRealTime = false;
		public static readonly Vector3 up = Vector3.up;
	}

	#endregion

	#region #1 Static Registers

	public static void Init(GameObject target)
	{
		MoveBy(target, Vector3.zero, 0);
	}

	public static void ValueTo(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		if (!args.Contains("onupdate") || !args.Contains("from") || !args.Contains("to"))
		{
			Debug.LogError("iTween Error: ValueTo() requires an 'onupdate' callback function and a 'from' and 'to' property.  The supplied 'onupdate' callback must accept a single argument that is the same type as the supplied 'from' and 'to' properties!");
			return;
		}

		args["type"] = "value";

		switch (args["from"])
		{
			case Vector2 _:
				args["method"] = "vector2";
				break;
			case Vector3 _:
				args["method"] = "vector3";
				break;
			case Rect _:
				args["method"] = "rect";
				break;
			case float _:
				args["method"] = "float";
				break;
			case Color _:
				args["method"] = "color";
				break;
			default:
				Debug.LogError("iTween Error: ValueTo() only works with interpolating Vector3s, Vector2s, floats, ints, Rects and Colors!");
				return;
		}

		if (!args.Contains("easetype"))
			args.Add("easetype", EaseType.linear);

		Launch(target, args);
	}

	public static void FadeFrom(GameObject target, float alpha, float time)
	{
		FadeFrom(target, Hash("alpha", alpha, "time", time));
	}

	public static void FadeFrom(GameObject target, Hashtable args)
	{
		ColorFrom(target, args);
	}

	public static void FadeTo(GameObject target, float alpha, float time)
	{
		FadeTo(target, Hash("alpha", alpha, "time", time));
	}

	public static void FadeTo(GameObject target, Hashtable args)
	{
		ColorTo(target, args);
	}

	public static void ColorFrom(GameObject target, Color color, float time)
	{
		ColorFrom(target, Hash("color", color, "time", time));
	}

	public static void ColorFrom(GameObject target, Hashtable args)
	{
		Color fromColor = new Color();
		Color tempColor = new Color();

		args = CleanArgs(args);

		if (!args.Contains("includechildren") || (bool) args["includechildren"])
		{
			foreach (Transform child in target.transform)
			{
				Hashtable argsCopy = (Hashtable) args.Clone();
				argsCopy["ischild"] = true;
				ColorFrom(child.gameObject, argsCopy);
			}
		}

		if (!args.Contains("easetype"))
		{
			args.Add("easetype", EaseType.linear);
		}

		if (target.GetComponent<Renderer>())
		{
			tempColor = fromColor = target.GetComponent<Renderer>().material.color;
		}
		else if (target.GetComponent<Light>())
		{
			tempColor = fromColor = target.GetComponent<Light>().color;
		}

		if (args.Contains("color"))
		{
			fromColor = (Color) args["color"];
		}
		else
		{
			if (args.Contains("r"))
				fromColor.r = (float) args["r"];

			if (args.Contains("g"))
				fromColor.g = (float) args["g"];

			if (args.Contains("b"))
				fromColor.b = (float) args["b"];

			if (args.Contains("a"))
				fromColor.a = (float) args["a"];
		}

		if (args.Contains("amount"))
		{
			fromColor.a = (float) args["amount"];
			args.Remove("amount");
		}
		else if (args.Contains("alpha"))
		{
			fromColor.a = (float) args["alpha"];
			args.Remove("alpha");
		}

		if (target.GetComponent<Renderer>())
		{
			target.GetComponent<Renderer>().material.color = fromColor;
		}
		else if (target.GetComponent<Light>())
		{
			target.GetComponent<Light>().color = fromColor;
		}

		args["color"] = tempColor;

		args["type"] = "color";
		args["method"] = "to";
		Launch(target, args);
	}

	public static void ColorTo(GameObject target, Color color, float time)
	{
		ColorTo(target, Hash("color", color, "time", time));
	}

	public static void ColorTo(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		if (!args.Contains("includechildren") || (bool) args["includechildren"])
		{
			foreach (Transform child in target.transform)
			{
				Hashtable argsCopy = (Hashtable) args.Clone();
				argsCopy["ischild"] = true;
				ColorTo(child.gameObject, argsCopy);
			}
		}

		if (!args.Contains("easetype"))
		{
			args.Add("easetype", EaseType.linear);
		}

		args["type"] = "color";
		args["method"] = "to";
		Launch(target, args);
	}

	public static void AudioFrom(GameObject target, float volume, float pitch, float time)
	{
		AudioFrom(target, Hash("volume", volume, "pitch", pitch, "time", time));
	}

	public static void AudioFrom(GameObject target, Hashtable args)
	{
		Vector2 tempAudioProperties;
		Vector2 fromAudioProperties;
		AudioSource tempAudioSource;

		args = CleanArgs(args);

		if (args.Contains("audiosource"))
		{
			tempAudioSource = (AudioSource) args["audiosource"];
		}
		else
		{
			if (target.GetComponent<AudioSource>())
			{
				tempAudioSource = target.GetComponent<AudioSource>();
			}
			else
			{
				Debug.LogError("iTween Error: AudioFrom requires an AudioSource.");
				return;
			}
		}

		tempAudioProperties.x = fromAudioProperties.x = tempAudioSource.volume;
		tempAudioProperties.y = fromAudioProperties.y = tempAudioSource.pitch;

		if (args.Contains("volume"))
		{
			fromAudioProperties.x = (float) args["volume"];
		}

		if (args.Contains("pitch"))
		{
			fromAudioProperties.y = (float) args["pitch"];
		}

		tempAudioSource.volume = fromAudioProperties.x;
		tempAudioSource.pitch = fromAudioProperties.y;

		args["volume"] = tempAudioProperties.x;
		args["pitch"] = tempAudioProperties.y;

		if (!args.Contains("easetype"))
		{
			args.Add("easetype", EaseType.linear);
		}

		args["type"] = "audio";
		args["method"] = "to";
		Launch(target, args);
	}

	public static void AudioTo(GameObject target, float volume, float pitch, float time)
	{
		AudioTo(target, Hash("volume", volume, "pitch", pitch, "time", time));
	}

	public static void AudioTo(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		if (!args.Contains("easetype"))
		{
			args.Add("easetype", EaseType.linear);
		}

		args["type"] = "audio";
		args["method"] = "to";
		Launch(target, args);
	}

	public static void Stab(GameObject target, AudioClip audioclip, float delay)
	{
		Stab(target, Hash("audioclip", audioclip, "delay", delay));
	}

	public static void Stab(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		args["type"] = "stab";
		Launch(target, args);
	}

	public static void LookFrom(GameObject target, Vector3 looktarget, float time)
	{
		LookFrom(target, Hash("looktarget", looktarget, "time", time));
	}

	public static void LookFrom(GameObject target, Hashtable args)
	{
		Vector3 tempRotation;
		Vector3 tempRestriction;

		args = CleanArgs(args);

		tempRotation = target.transform.eulerAngles;
		if (args["looktarget"].GetType() == typeof(Transform))
		{
			target.transform.LookAt((Transform) args["looktarget"], (Vector3?) args["up"] ?? Defaults.up);
		}
		else if (args["looktarget"] is Vector3)
		{
			target.transform.LookAt((Vector3) args["looktarget"], (Vector3?) args["up"] ?? Defaults.up);
		}

		if (args.Contains("axis"))
		{
			tempRestriction = target.transform.eulerAngles;
			switch ((string) args["axis"])
			{
				case "x":
					tempRestriction.y = tempRotation.y;
					tempRestriction.z = tempRotation.z;
					break;
				case "y":
					tempRestriction.x = tempRotation.x;
					tempRestriction.z = tempRotation.z;
					break;
				case "z":
					tempRestriction.x = tempRotation.x;
					tempRestriction.y = tempRotation.y;
					break;
			}

			target.transform.eulerAngles = tempRestriction;
		}

		args["rotation"] = tempRotation;

		args["type"] = "rotate";
		args["method"] = "to";
		Launch(target, args);
	}

	public static void LookTo(GameObject target, Vector3 looktarget, float time)
	{
		LookTo(target, Hash("looktarget", looktarget, "time", time));
	}

	public static void LookTo(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		if (args.Contains("looktarget"))
		{
			if (args["looktarget"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform) args["looktarget"];
				args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
				args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y,
					transform.eulerAngles.z);
			}
		}

		args["type"] = "look";
		args["method"] = "to";
		Launch(target, args);
	}

	public static void MoveTo(GameObject target, Vector3 position, float time)
	{
		MoveTo(target, Hash("position", position, "time", time));
	}

	public static void MoveTo(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		if (args.Contains("position"))
		{
			if (args["position"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform) args["position"];
				args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
				args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y,
					transform.eulerAngles.z);
				args["scale"] = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
			}
		}

		args["type"] = "move";
		args["method"] = "to";
		Launch(target, args);
	}

	public static void MoveFrom(GameObject target, Vector3 position, float time)
	{
		MoveFrom(target, Hash("position", position, "time", time));
	}

	public static void MoveFrom(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		bool tempIsLocal;

		if (args.Contains("islocal"))
		{
			tempIsLocal = (bool) args["islocal"];
		}
		else
		{
			tempIsLocal = Defaults.isLocal;
		}

		if (args.Contains("path"))
		{
			Vector3[] fromPath;
			Vector3[] suppliedPath;
			if (args["path"].GetType() == typeof(Vector3[]))
			{
				Vector3[] temp = (Vector3[]) args["path"];
				suppliedPath = new Vector3[temp.Length];
				Array.Copy(temp, suppliedPath, temp.Length);
			}
			else
			{
				Transform[] temp = (Transform[]) args["path"];
				suppliedPath = new Vector3[temp.Length];
				for (int i = 0; i < temp.Length; i++)
				{
					suppliedPath[i] = temp[i].position;
				}
			}

			if (suppliedPath[suppliedPath.Length - 1] != target.transform.position)
			{
				fromPath = new Vector3[suppliedPath.Length + 1];
				Array.Copy(suppliedPath, fromPath, suppliedPath.Length);
				if (tempIsLocal)
				{
					fromPath[fromPath.Length - 1] = target.transform.localPosition;
					target.transform.localPosition = fromPath[0];
				}
				else
				{
					fromPath[fromPath.Length - 1] = target.transform.position;
					target.transform.position = fromPath[0];
				}

				args["path"] = fromPath;
			}
			else
			{
				if (tempIsLocal)
				{
					target.transform.localPosition = suppliedPath[0];
				}
				else
				{
					target.transform.position = suppliedPath[0];
				}

				args["path"] = suppliedPath;
			}
		}
		else
		{
			Vector3 tempPosition;
			Vector3 fromPosition;

			if (tempIsLocal)
			{
				tempPosition = fromPosition = target.transform.localPosition;
			}
			else
			{
				tempPosition = fromPosition = target.transform.position;
			}

			if (args.Contains("position"))
			{
				if (args["position"].GetType() == typeof(Transform))
				{
					Transform trans = (Transform) args["position"];
					fromPosition = trans.position;
				}
				else if (args["position"] is Vector3)
				{
					fromPosition = (Vector3) args["position"];
				}
			}
			else
			{
				if (args.Contains("x"))
				{
					fromPosition.x = (float) args["x"];
				}

				if (args.Contains("y"))
				{
					fromPosition.y = (float) args["y"];
				}

				if (args.Contains("z"))
				{
					fromPosition.z = (float) args["z"];
				}
			}

			if (tempIsLocal)
			{
				target.transform.localPosition = fromPosition;
			}
			else
			{
				target.transform.position = fromPosition;
			}

			args["position"] = tempPosition;
		}

		args["type"] = "move";
		args["method"] = "to";
		Launch(target, args);
	}

	public static void MoveAdd(GameObject target, Vector3 amount, float time)
	{
		MoveAdd(target, Hash("amount", amount, "time", time));
	}

	public static void MoveAdd(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		args["type"] = "move";
		args["method"] = "add";
		Launch(target, args);
	}

	public static void MoveBy(GameObject target, Vector3 amount, float time)
	{
		MoveBy(target, Hash("amount", amount, "time", time));
	}

	public static void MoveBy(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		args["type"] = "move";
		args["method"] = "by";
		Launch(target, args);
	}

	public static void ScaleTo(GameObject target, Vector3 scale, float time)
	{
		ScaleTo(target, Hash("scale", scale, "time", time));
	}

	public static void ScaleTo(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		if (args.Contains("scale"))
		{
			if (args["scale"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform) args["scale"];
				args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
				args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y,
					transform.eulerAngles.z);
				args["scale"] = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
			}
		}

		args["type"] = "scale";
		args["method"] = "to";
		Launch(target, args);
	}

	public static void ScaleFrom(GameObject target, Vector3 scale, float time)
	{
		ScaleFrom(target, Hash("scale", scale, "time", time));
	}

	public static void ScaleFrom(GameObject target, Hashtable args)
	{
		Vector3 tempScale;
		Vector3 fromScale;

		args = CleanArgs(args);

		tempScale = fromScale = target.transform.localScale;

		if (args.Contains("scale"))
		{
			if (args["scale"].GetType() == typeof(Transform))
			{
				Transform trans = (Transform) args["scale"];
				fromScale = trans.localScale;
			}
			else if (args["scale"] is Vector3)
			{
				fromScale = (Vector3) args["scale"];
			}
		}
		else
		{
			if (args.Contains("x"))
			{
				fromScale.x = (float) args["x"];
			}

			if (args.Contains("y"))
			{
				fromScale.y = (float) args["y"];
			}

			if (args.Contains("z"))
			{
				fromScale.z = (float) args["z"];
			}
		}

		target.transform.localScale = fromScale;

		args["scale"] = tempScale;

		args["type"] = "scale";
		args["method"] = "to";
		Launch(target, args);
	}

	public static void ScaleAdd(GameObject target, Vector3 amount, float time)
	{
		ScaleAdd(target, Hash("amount", amount, "time", time));
	}

	public static void ScaleAdd(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		args["type"] = "scale";
		args["method"] = "add";
		Launch(target, args);
	}

	public static void ScaleBy(GameObject target, Vector3 amount, float time)
	{
		ScaleBy(target, Hash("amount", amount, "time", time));
	}

	public static void ScaleBy(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		args["type"] = "scale";
		args["method"] = "by";
		Launch(target, args);
	}

	public static void RotateTo(GameObject target, Vector3 rotation, float time)
	{
		RotateTo(target, Hash("rotation", rotation, "time", time));
	}

	public static void RotateTo(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		if (args.Contains("rotation"))
		{
			if (args["rotation"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform) args["rotation"];
				args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
				args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y,
					transform.eulerAngles.z);
				args["scale"] = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
			}
		}

		args["type"] = "rotate";
		args["method"] = "to";
		Launch(target, args);
	}

	public static void RotateFrom(GameObject target, Vector3 rotation, float time)
	{
		RotateFrom(target, Hash("rotation", rotation, "time", time));
	}

	public static void RotateFrom(GameObject target, Hashtable args)
	{
		Vector3 tempRotation;
		Vector3 fromRotation;
		bool tempIsLocal;

		args = CleanArgs(args);

		if (args.Contains("islocal"))
		{
			tempIsLocal = (bool) args["islocal"];
		}
		else
		{
			tempIsLocal = Defaults.isLocal;
		}

		if (tempIsLocal)
		{
			tempRotation = fromRotation = target.transform.localEulerAngles;
		}
		else
		{
			tempRotation = fromRotation = target.transform.eulerAngles;
		}

		if (args.Contains("rotation"))
		{
			if (args["rotation"].GetType() == typeof(Transform))
			{
				Transform trans = (Transform) args["rotation"];
				fromRotation = trans.eulerAngles;
			}
			else if (args["rotation"] is Vector3)
			{
				fromRotation = (Vector3) args["rotation"];
			}
		}
		else
		{
			if (args.Contains("x"))
			{
				fromRotation.x = (float) args["x"];
			}

			if (args.Contains("y"))
			{
				fromRotation.y = (float) args["y"];
			}

			if (args.Contains("z"))
			{
				fromRotation.z = (float) args["z"];
			}
		}

		if (tempIsLocal)
		{
			target.transform.localEulerAngles = fromRotation;
		}
		else
		{
			target.transform.eulerAngles = fromRotation;
		}

		args["rotation"] = tempRotation;

		args["type"] = "rotate";
		args["method"] = "to";
		Launch(target, args);
	}

	public static void RotateAdd(GameObject target, Vector3 amount, float time)
	{
		RotateAdd(target, Hash("amount", amount, "time", time));
	}

	public static void RotateAdd(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		args["type"] = "rotate";
		args["method"] = "add";
		Launch(target, args);
	}

	public static void RotateBy(GameObject target, Vector3 amount, float time)
	{
		RotateBy(target, Hash("amount", amount, "time", time));
	}

	public static void RotateBy(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		args["type"] = "rotate";
		args["method"] = "by";
		Launch(target, args);
	}

	public static void ShakePosition(GameObject target, Vector3 amount, float time)
	{
		ShakePosition(target, Hash("amount", amount, "time", time));
	}

	public static void ShakePosition(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		args["type"] = "shake";
		args["method"] = "position";
		Launch(target, args);
	}

	public static void ShakeScale(GameObject target, Vector3 amount, float time)
	{
		ShakeScale(target, Hash("amount", amount, "time", time));
	}

	public static void ShakeScale(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		args["type"] = "shake";
		args["method"] = "scale";
		Launch(target, args);
	}

	public static void ShakeRotation(GameObject target, Vector3 amount, float time)
	{
		ShakeRotation(target, Hash("amount", amount, "time", time));
	}

	public static void ShakeRotation(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		args["type"] = "shake";
		args["method"] = "rotation";
		Launch(target, args);
	}

	public static void PunchPosition(GameObject target, Vector3 amount, float time)
	{
		PunchPosition(target, Hash("amount", amount, "time", time));
	}

	public static void PunchPosition(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		args["type"] = "punch";
		args["method"] = "position";
		args["easetype"] = EaseType.punch;
		Launch(target, args);
	}

	public static void PunchRotation(GameObject target, Vector3 amount, float time)
	{
		PunchRotation(target, Hash("amount", amount, "time", time));
	}

	public static void PunchRotation(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		args["type"] = "punch";
		args["method"] = "rotation";
		args["easetype"] = EaseType.punch;
		Launch(target, args);
	}

	public static void PunchScale(GameObject target, Vector3 amount, float time)
	{
		PunchScale(target, Hash("amount", amount, "time", time));
	}

	public static void PunchScale(GameObject target, Hashtable args)
	{
		args = CleanArgs(args);

		args["type"] = "punch";
		args["method"] = "scale";
		args["easetype"] = EaseType.punch;
		Launch(target, args);
	}

	#endregion

	#region #2 Generate Method Targets

	private void GenerateTargets()
	{
		switch (type)
		{
			case "value":
				switch (method)
				{
					case "float":
						GenerateFloatTargets();
						apply = ApplyFloatTargets;
						break;
					case "vector2":
						GenerateVector2Targets();
						apply = ApplyVector2Targets;
						break;
					case "vector3":
						GenerateVector3Targets();
						apply = ApplyVector3Targets;
						break;
					case "color":
						GenerateColorTargets();
						apply = ApplyColorTargets;
						break;
					case "rect":
						GenerateRectTargets();
						apply = ApplyRectTargets;
						break;
				}

				break;
			case "color":
				switch (method)
				{
					case "to":
						GenerateColorToTargets();
						apply = ApplyColorToTargets;
						break;
				}

				break;
			case "audio":
				switch (method)
				{
					case "to":
						GenerateAudioToTargets();
						apply = ApplyAudioToTargets;
						break;
				}

				break;
			case "move":
				switch (method)
				{
					case "to":
						if (tweenArguments.Contains("path"))
						{
							GenerateMoveToPathTargets();
							apply = ApplyMoveToPathTargets;
						}
						else
						{
							GenerateMoveToTargets();
							apply = ApplyMoveToTargets;
						}

						break;
					case "by":
					case "add":
						GenerateMoveByTargets();
						apply = ApplyMoveByTargets;
						break;
				}

				break;
			case "scale":
				switch (method)
				{
					case "to":
						GenerateScaleToTargets();
						apply = ApplyScaleToTargets;
						break;
					case "by":
						GenerateScaleByTargets();
						apply = ApplyScaleToTargets;
						break;
					case "add":
						GenerateScaleAddTargets();
						apply = ApplyScaleToTargets;
						break;
				}

				break;
			case "rotate":
				switch (method)
				{
					case "to":
						GenerateRotateToTargets();
						apply = ApplyRotateToTargets;
						break;
					case "add":
						GenerateRotateAddTargets();
						apply = ApplyRotateAddTargets;
						break;
					case "by":
						GenerateRotateByTargets();
						apply = ApplyRotateAddTargets;
						break;
				}

				break;
			case "shake":
				switch (method)
				{
					case "position":
						GenerateShakePositionTargets();
						apply = ApplyShakePositionTargets;
						break;
					case "scale":
						GenerateShakeScaleTargets();
						apply = ApplyShakeScaleTargets;
						break;
					case "rotation":
						GenerateShakeRotationTargets();
						apply = ApplyShakeRotationTargets;
						break;
				}

				break;
			case "punch":
				switch (method)
				{
					case "position":
						GeneratePunchPositionTargets();
						apply = ApplyPunchPositionTargets;
						break;
					case "rotation":
						GeneratePunchRotationTargets();
						apply = ApplyPunchRotationTargets;
						break;
					case "scale":
						GeneratePunchScaleTargets();
						apply = ApplyPunchScaleTargets;
						break;
				}

				break;
			case "look":
				switch (method)
				{
					case "to":
						GenerateLookToTargets();
						apply = ApplyLookToTargets;
						break;
				}

				break;
			case "stab":
				GenerateStabTargets();
				apply = ApplyStabTargets;
				break;
		}
	}

	#endregion

	#region #3 Generate Specific Targets

	private void GenerateRectTargets()
	{
		rects = new Rect[3];

		rects[0] = (Rect) tweenArguments["from"];
		rects[1] = (Rect) tweenArguments["to"];
	}

	private void GenerateColorTargets()
	{
		colors = new Color[1, 3];

		colors[0, 0] = (Color) tweenArguments["from"];
		colors[0, 1] = (Color) tweenArguments["to"];
	}

	private void GenerateVector3Targets()
	{
		vector3s = new Vector3[3];

		vector3s[0] = (Vector3) tweenArguments["from"];
		vector3s[1] = (Vector3) tweenArguments["to"];

		if (tweenArguments.Contains("speed"))
		{
			float distance = Math.Abs(Vector3.Distance(vector3s[0], vector3s[1]));
			time = distance / (float) tweenArguments["speed"];
		}
	}

	private void GenerateVector2Targets()
	{
		vector2s = new Vector2[3];

		vector2s[0] = (Vector2) tweenArguments["from"];
		vector2s[1] = (Vector2) tweenArguments["to"];

		if (tweenArguments.Contains("speed"))
		{
			Vector3 fromV3 = new Vector3(vector2s[0].x, vector2s[0].y, 0);
			Vector3 toV3 = new Vector3(vector2s[1].x, vector2s[1].y, 0);
			float distance = Math.Abs(Vector3.Distance(fromV3, toV3));
			time = distance / (float) tweenArguments["speed"];
		}
	}

	private void GenerateFloatTargets()
	{
		floats = new float[3];

		floats[0] = (float) tweenArguments["from"];
		floats[1] = (float) tweenArguments["to"];

		if (tweenArguments.Contains("speed"))
		{
			float distance = Math.Abs(floats[0] - floats[1]);
			time = distance / (float) tweenArguments["speed"];
		}
	}

	private void GenerateColorToTargets()
	{
		if (GetComponent<Renderer>())
		{
			colors = new Color[GetComponent<Renderer>().materials.Length, 3];
			for (int i = 0; i < GetComponent<Renderer>().materials.Length; i++)
			{
				colors[i, 0] = GetComponent<Renderer>().materials[i].GetColor(namedcolorvalue.ToString());
				colors[i, 1] = GetComponent<Renderer>().materials[i].GetColor(namedcolorvalue.ToString());
			}
		}
		else if (GetComponent<Light>())
		{
			colors = new Color[1, 3];
			colors[0, 0] = colors[0, 1] = GetComponent<Light>().color;
		}
		else
		{
			colors = new Color[1, 3];
		}

		if (tweenArguments.Contains("color"))
		{
			for (int i = 0; i < colors.GetLength(0); i++)
			{
				colors[i, 1] = (Color) tweenArguments["color"];
			}
		}
		else
		{
			if (tweenArguments.Contains("r"))
			{
				for (int i = 0; i < colors.GetLength(0); i++)
				{
					colors[i, 1].r = (float) tweenArguments["r"];
				}
			}

			if (tweenArguments.Contains("g"))
			{
				for (int i = 0; i < colors.GetLength(0); i++)
				{
					colors[i, 1].g = (float) tweenArguments["g"];
				}
			}

			if (tweenArguments.Contains("b"))
			{
				for (int i = 0; i < colors.GetLength(0); i++)
				{
					colors[i, 1].b = (float) tweenArguments["b"];
				}
			}

			if (tweenArguments.Contains("a"))
			{
				for (int i = 0; i < colors.GetLength(0); i++)
				{
					colors[i, 1].a = (float) tweenArguments["a"];
				}
			}
		}

		if (tweenArguments.Contains("amount"))
		{
			for (int i = 0; i < colors.GetLength(0); i++)
			{
				colors[i, 1].a = (float) tweenArguments["amount"];
			}
		}
		else if (tweenArguments.Contains("alpha"))
		{
			for (int i = 0; i < colors.GetLength(0); i++)
			{
				colors[i, 1].a = (float) tweenArguments["alpha"];
			}
		}
	}

	private void GenerateAudioToTargets()
	{
		vector2s = new Vector2[3];

		if (tweenArguments.Contains("audiosource"))
		{
			audioSource = (AudioSource) tweenArguments["audiosource"];
		}
		else
		{
			if (GetComponent<AudioSource>())
			{
				audioSource = GetComponent<AudioSource>();
			}
			else
			{
				Debug.LogError("iTween Error: AudioTo requires an AudioSource.");
				Dispose();
			}
		}

		vector2s[0] = vector2s[1] = new Vector2(audioSource.volume, audioSource.pitch);

		if (tweenArguments.Contains("volume"))
		{
			vector2s[1].x = (float) tweenArguments["volume"];
		}

		if (tweenArguments.Contains("pitch"))
		{
			vector2s[1].y = (float) tweenArguments["pitch"];
		}
	}

	private void GenerateStabTargets()
	{
		if (tweenArguments.Contains("audiosource"))
		{
			audioSource = (AudioSource) tweenArguments["audiosource"];
		}
		else
		{
			if (GetComponent<AudioSource>())
			{
				audioSource = GetComponent<AudioSource>();
			}
			else
			{
				gameObject.AddComponent<AudioSource>();
				audioSource = GetComponent<AudioSource>();
				audioSource.playOnAwake = false;
			}
		}

		audioSource.clip = (AudioClip) tweenArguments["audioclip"];

		if (tweenArguments.Contains("pitch"))
		{
			audioSource.pitch = (float) tweenArguments["pitch"];
		}

		if (tweenArguments.Contains("volume"))
		{
			audioSource.volume = (float) tweenArguments["volume"];
		}

		time = audioSource.clip.length / audioSource.pitch;
	}

	private void GenerateLookToTargets()
	{
		vector3s = new Vector3[3];

		vector3s[0] = thisTransform.eulerAngles;

		if (tweenArguments.Contains("looktarget"))
		{
			if (tweenArguments["looktarget"].GetType() == typeof(Transform))
			{
				thisTransform.LookAt((Transform) tweenArguments["looktarget"],
					(Vector3?) tweenArguments["up"] ?? Defaults.up);
			}
			else if (tweenArguments["looktarget"] is Vector3)
			{
				thisTransform.LookAt((Vector3) tweenArguments["looktarget"],
					(Vector3?) tweenArguments["up"] ?? Defaults.up);
			}
		}
		else
		{
			Debug.LogError("iTween Error: LookTo needs a 'looktarget' property!");
			Dispose();
		}

		vector3s[1] = thisTransform.eulerAngles;
		thisTransform.eulerAngles = vector3s[0];

		if (tweenArguments.Contains("axis"))
		{
			switch ((string) tweenArguments["axis"])
			{
				case "x":
					vector3s[1].y = vector3s[0].y;
					vector3s[1].z = vector3s[0].z;
					break;
				case "y":
					vector3s[1].x = vector3s[0].x;
					vector3s[1].z = vector3s[0].z;
					break;
				case "z":
					vector3s[1].x = vector3s[0].x;
					vector3s[1].y = vector3s[0].y;
					break;
			}
		}

		vector3s[1] = new Vector3(clerp(vector3s[0].x, vector3s[1].x, 1), clerp(vector3s[0].y, vector3s[1].y, 1),
			clerp(vector3s[0].z, vector3s[1].z, 1));

		if (tweenArguments.Contains("speed"))
		{
			float distance = Math.Abs(Vector3.Distance(vector3s[0], vector3s[1]));
			time = distance / (float) tweenArguments["speed"];
		}
	}

	private void GenerateMoveToPathTargets()
	{
		Vector3[] suppliedPath;

		if (tweenArguments["path"].GetType() == typeof(Vector3[]))
		{
			Vector3[] temp = (Vector3[]) tweenArguments["path"];
			if (temp.Length == 1)
			{
				Debug.LogError("iTween Error: Attempting a path movement with MoveTo requires an array of more than 1 entry!");
				Dispose();
			}

			suppliedPath = new Vector3[temp.Length];
			Array.Copy(temp, suppliedPath, temp.Length);
		}
		else
		{
			Transform[] temp = (Transform[]) tweenArguments["path"];
			if (temp.Length == 1)
			{
				Debug.LogError("iTween Error: Attempting a path movement with MoveTo requires an array of more than 1 entry!");
				Dispose();
			}

			suppliedPath = new Vector3[temp.Length];
			for (int i = 0; i < temp.Length; i++)
			{
				suppliedPath[i] = temp[i].position;
			}
		}

		bool plotStart;
		int offset;
		if (thisTransform.position != suppliedPath[0])
		{
			if (!tweenArguments.Contains("movetopath") || (bool) tweenArguments["movetopath"])
			{
				plotStart = true;
				offset = 3;
			}
			else
			{
				plotStart = false;
				offset = 2;
			}
		}
		else
		{
			plotStart = false;
			offset = 2;
		}

		vector3s = new Vector3[suppliedPath.Length + offset];
		if (plotStart)
		{
			vector3s[1] = thisTransform.position;
			offset = 2;
		}
		else
		{
			offset = 1;
		}

		Array.Copy(suppliedPath, 0, vector3s, offset, suppliedPath.Length);

		vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
		vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] +
		                                (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);

		if (vector3s[1] == vector3s[vector3s.Length - 2])
		{
			Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
			Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
			tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
			tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
			vector3s = new Vector3[tmpLoopSpline.Length];
			Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
		}

		path = new CRSpline(vector3s);

		if (tweenArguments.Contains("speed"))
		{
			float distance = PathLength(vector3s);
			time = distance / (float) tweenArguments["speed"];
		}
	}

	private void GenerateMoveToTargets()
	{
		vector3s = new Vector3[3];

		if (isLocal)
		{
			vector3s[0] = vector3s[1] = thisTransform.localPosition;
		}
		else
		{
			vector3s[0] = vector3s[1] = thisTransform.position;
		}

		if (tweenArguments.Contains("position"))
		{
			if (tweenArguments["position"].GetType() == typeof(Transform))
			{
				Transform trans = (Transform) tweenArguments["position"];
				vector3s[1] = trans.position;
			}
			else if (tweenArguments["position"] is Vector3)
			{
				vector3s[1] = (Vector3) tweenArguments["position"];
			}
		}
		else
		{
			if (tweenArguments.Contains("x"))
			{
				vector3s[1].x = (float) tweenArguments["x"];
			}

			if (tweenArguments.Contains("y"))
			{
				vector3s[1].y = (float) tweenArguments["y"];
			}

			if (tweenArguments.Contains("z"))
			{
				vector3s[1].z = (float) tweenArguments["z"];
			}
		}

		if (tweenArguments.Contains("orienttopath") && (bool) tweenArguments["orienttopath"])
		{
			tweenArguments["looktarget"] = vector3s[1];
		}

		if (tweenArguments.Contains("speed"))
		{
			float distance = Math.Abs(Vector3.Distance(vector3s[0], vector3s[1]));
			time = distance / (float) tweenArguments["speed"];
		}
	}

	private void GenerateMoveByTargets()
	{
		vector3s = new Vector3[6];

		vector3s[4] = thisTransform.eulerAngles;

		vector3s[0] = vector3s[1] = vector3s[3] = thisTransform.position;

		if (tweenArguments.Contains("amount"))
		{
			vector3s[1] = vector3s[0] + (Vector3) tweenArguments["amount"];
		}
		else
		{
			if (tweenArguments.Contains("x"))
			{
				vector3s[1].x = vector3s[0].x + (float) tweenArguments["x"];
			}

			if (tweenArguments.Contains("y"))
			{
				vector3s[1].y = vector3s[0].y + (float) tweenArguments["y"];
			}

			if (tweenArguments.Contains("z"))
			{
				vector3s[1].z = vector3s[0].z + (float) tweenArguments["z"];
			}
		}

		thisTransform.Translate(vector3s[1], space);
		vector3s[5] = thisTransform.position;
		thisTransform.position = vector3s[0];

		if (tweenArguments.Contains("orienttopath") && (bool) tweenArguments["orienttopath"])
		{
			tweenArguments["looktarget"] = vector3s[1];
		}

		if (tweenArguments.Contains("speed"))
		{
			float distance = Math.Abs(Vector3.Distance(vector3s[0], vector3s[1]));
			time = distance / (float) tweenArguments["speed"];
		}
	}

	private void GenerateScaleToTargets()
	{
		vector3s = new Vector3[3];

		vector3s[0] = vector3s[1] = thisTransform.localScale;

		if (tweenArguments.Contains("scale"))
		{
			if (tweenArguments["scale"].GetType() == typeof(Transform))
			{
				Transform trans = (Transform) tweenArguments["scale"];
				vector3s[1] = trans.localScale;
			}
			else if (tweenArguments["scale"] is Vector3)
			{
				vector3s[1] = (Vector3) tweenArguments["scale"];
			}
		}
		else
		{
			if (tweenArguments.Contains("x"))
			{
				vector3s[1].x = (float) tweenArguments["x"];
			}

			if (tweenArguments.Contains("y"))
			{
				vector3s[1].y = (float) tweenArguments["y"];
			}

			if (tweenArguments.Contains("z"))
			{
				vector3s[1].z = (float) tweenArguments["z"];
			}
		}

		if (tweenArguments.Contains("speed"))
		{
			float distance = Math.Abs(Vector3.Distance(vector3s[0], vector3s[1]));
			time = distance / (float) tweenArguments["speed"];
		}
	}

	private void GenerateScaleByTargets()
	{
		vector3s = new Vector3[3];

		vector3s[0] = vector3s[1] = thisTransform.localScale;

		if (tweenArguments.Contains("amount"))
		{
			vector3s[1] = Vector3.Scale(vector3s[1], (Vector3) tweenArguments["amount"]);
		}
		else
		{
			if (tweenArguments.Contains("x"))
			{
				vector3s[1].x *= (float) tweenArguments["x"];
			}

			if (tweenArguments.Contains("y"))
			{
				vector3s[1].y *= (float) tweenArguments["y"];
			}

			if (tweenArguments.Contains("z"))
			{
				vector3s[1].z *= (float) tweenArguments["z"];
			}
		}

		if (tweenArguments.Contains("speed"))
		{
			float distance = Math.Abs(Vector3.Distance(vector3s[0], vector3s[1]));
			time = distance / (float) tweenArguments["speed"];
		}
	}

	private void GenerateScaleAddTargets()
	{
		vector3s = new Vector3[3];

		vector3s[0] = vector3s[1] = thisTransform.localScale;

		if (tweenArguments.Contains("amount"))
		{
			vector3s[1] += (Vector3) tweenArguments["amount"];
		}
		else
		{
			if (tweenArguments.Contains("x"))
			{
				vector3s[1].x += (float) tweenArguments["x"];
			}

			if (tweenArguments.Contains("y"))
			{
				vector3s[1].y += (float) tweenArguments["y"];
			}

			if (tweenArguments.Contains("z"))
			{
				vector3s[1].z += (float) tweenArguments["z"];
			}
		}

		if (tweenArguments.Contains("speed"))
		{
			float distance = Math.Abs(Vector3.Distance(vector3s[0], vector3s[1]));
			time = distance / (float) tweenArguments["speed"];
		}
	}

	private void GenerateRotateToTargets()
	{
		vector3s = new Vector3[3];

		if (isLocal)
		{
			vector3s[0] = vector3s[1] = thisTransform.localEulerAngles;
		}
		else
		{
			vector3s[0] = vector3s[1] = thisTransform.eulerAngles;
		}

		if (tweenArguments.Contains("rotation"))
		{
			if (tweenArguments["rotation"].GetType() == typeof(Transform))
			{
				Transform trans = (Transform) tweenArguments["rotation"];
				vector3s[1] = trans.eulerAngles;
			}
			else if (tweenArguments["rotation"] is Vector3)
			{
				vector3s[1] = (Vector3) tweenArguments["rotation"];
			}
		}
		else
		{
			if (tweenArguments.Contains("x"))
			{
				vector3s[1].x = (float) tweenArguments["x"];
			}

			if (tweenArguments.Contains("y"))
			{
				vector3s[1].y = (float) tweenArguments["y"];
			}

			if (tweenArguments.Contains("z"))
			{
				vector3s[1].z = (float) tweenArguments["z"];
			}
		}

		vector3s[1] = new Vector3(clerp(vector3s[0].x, vector3s[1].x, 1), clerp(vector3s[0].y, vector3s[1].y, 1),
			clerp(vector3s[0].z, vector3s[1].z, 1));

		if (tweenArguments.Contains("speed"))
		{
			float distance = Math.Abs(Vector3.Distance(vector3s[0], vector3s[1]));
			time = distance / (float) tweenArguments["speed"];
		}
	}

	private void GenerateRotateAddTargets()
	{
		vector3s = new Vector3[5];

		vector3s[0] = vector3s[1] = vector3s[3] = thisTransform.eulerAngles;

		if (tweenArguments.Contains("amount"))
		{
			vector3s[1] += (Vector3) tweenArguments["amount"];
		}
		else
		{
			if (tweenArguments.Contains("x"))
			{
				vector3s[1].x += (float) tweenArguments["x"];
			}

			if (tweenArguments.Contains("y"))
			{
				vector3s[1].y += (float) tweenArguments["y"];
			}

			if (tweenArguments.Contains("z"))
			{
				vector3s[1].z += (float) tweenArguments["z"];
			}
		}

		if (tweenArguments.Contains("speed"))
		{
			float distance = Math.Abs(Vector3.Distance(vector3s[0], vector3s[1]));
			time = distance / (float) tweenArguments["speed"];
		}
	}

	private void GenerateRotateByTargets()
	{
		vector3s = new Vector3[4];

		vector3s[0] = vector3s[1] = vector3s[3] = thisTransform.eulerAngles;

		if (tweenArguments.Contains("amount"))
		{
			vector3s[1] += Vector3.Scale((Vector3) tweenArguments["amount"], new Vector3(360, 360, 360));
		}
		else
		{
			if (tweenArguments.Contains("x"))
			{
				vector3s[1].x += 360 * (float) tweenArguments["x"];
			}

			if (tweenArguments.Contains("y"))
			{
				vector3s[1].y += 360 * (float) tweenArguments["y"];
			}

			if (tweenArguments.Contains("z"))
			{
				vector3s[1].z += 360 * (float) tweenArguments["z"];
			}
		}

		if (tweenArguments.Contains("speed"))
		{
			float distance = Math.Abs(Vector3.Distance(vector3s[0], vector3s[1]));
			time = distance / (float) tweenArguments["speed"];
		}
	}

	private void GenerateShakePositionTargets()
	{
		vector3s = new Vector3[4];

		vector3s[3] = thisTransform.eulerAngles;

		vector3s[0] = thisTransform.position;

		if (tweenArguments.Contains("amount"))
		{
			vector3s[1] = (Vector3) tweenArguments["amount"];
		}
		else
		{
			if (tweenArguments.Contains("x"))
			{
				vector3s[1].x = (float) tweenArguments["x"];
			}

			if (tweenArguments.Contains("y"))
			{
				vector3s[1].y = (float) tweenArguments["y"];
			}

			if (tweenArguments.Contains("z"))
			{
				vector3s[1].z = (float) tweenArguments["z"];
			}
		}
	}

	private void GenerateShakeScaleTargets()
	{
		vector3s = new Vector3[3];

		vector3s[0] = thisTransform.localScale;

		if (tweenArguments.Contains("amount"))
		{
			vector3s[1] = (Vector3) tweenArguments["amount"];
		}
		else
		{
			if (tweenArguments.Contains("x"))
			{
				vector3s[1].x = (float) tweenArguments["x"];
			}

			if (tweenArguments.Contains("y"))
			{
				vector3s[1].y = (float) tweenArguments["y"];
			}

			if (tweenArguments.Contains("z"))
			{
				vector3s[1].z = (float) tweenArguments["z"];
			}
		}
	}

	private void GenerateShakeRotationTargets()
	{
		vector3s = new Vector3[3];

		vector3s[0] = thisTransform.eulerAngles;

		if (tweenArguments.Contains("amount"))
		{
			vector3s[1] = (Vector3) tweenArguments["amount"];
		}
		else
		{
			if (tweenArguments.Contains("x"))
			{
				vector3s[1].x = (float) tweenArguments["x"];
			}

			if (tweenArguments.Contains("y"))
			{
				vector3s[1].y = (float) tweenArguments["y"];
			}

			if (tweenArguments.Contains("z"))
			{
				vector3s[1].z = (float) tweenArguments["z"];
			}
		}
	}

	private void GeneratePunchPositionTargets()
	{
		vector3s = new Vector3[5];

		vector3s[4] = thisTransform.eulerAngles;

		vector3s[0] = thisTransform.position;
		vector3s[1] = vector3s[3] = Vector3.zero;

		if (tweenArguments.Contains("amount"))
		{
			vector3s[1] = (Vector3) tweenArguments["amount"];
		}
		else
		{
			if (tweenArguments.Contains("x"))
			{
				vector3s[1].x = (float) tweenArguments["x"];
			}

			if (tweenArguments.Contains("y"))
			{
				vector3s[1].y = (float) tweenArguments["y"];
			}

			if (tweenArguments.Contains("z"))
			{
				vector3s[1].z = (float) tweenArguments["z"];
			}
		}
	}

	private void GeneratePunchRotationTargets()
	{
		vector3s = new Vector3[4];

		vector3s[0] = thisTransform.eulerAngles;
		vector3s[1] = vector3s[3] = Vector3.zero;

		if (tweenArguments.Contains("amount"))
		{
			vector3s[1] = (Vector3) tweenArguments["amount"];
		}
		else
		{
			if (tweenArguments.Contains("x"))
			{
				vector3s[1].x = (float) tweenArguments["x"];
			}

			if (tweenArguments.Contains("y"))
			{
				vector3s[1].y = (float) tweenArguments["y"];
			}

			if (tweenArguments.Contains("z"))
			{
				vector3s[1].z = (float) tweenArguments["z"];
			}
		}
	}

	private void GeneratePunchScaleTargets()
	{
		vector3s = new Vector3[3];

		vector3s[0] = thisTransform.localScale;
		vector3s[1] = Vector3.zero;

		if (tweenArguments.Contains("amount"))
		{
			vector3s[1] = (Vector3) tweenArguments["amount"];
		}
		else
		{
			if (tweenArguments.Contains("x"))
			{
				vector3s[1].x = (float) tweenArguments["x"];
			}

			if (tweenArguments.Contains("y"))
			{
				vector3s[1].y = (float) tweenArguments["y"];
			}

			if (tweenArguments.Contains("z"))
			{
				vector3s[1].z = (float) tweenArguments["z"];
			}
		}
	}

	#endregion

	#region #4 Apply Targets

	private void ApplyRectTargets()
	{
		rects[2].x = ease(rects[0].x, rects[1].x, percentage);
		rects[2].y = ease(rects[0].y, rects[1].y, percentage);
		rects[2].width = ease(rects[0].width, rects[1].width, percentage);
		rects[2].height = ease(rects[0].height, rects[1].height, percentage);

		tweenArguments["onupdateparams"] = rects[2];

		if (percentage == 1)
		{
			tweenArguments["onupdateparams"] = rects[1];
		}
	}

	private void ApplyColorTargets()
	{
		colors[0, 2].r = ease(colors[0, 0].r, colors[0, 1].r, percentage);
		colors[0, 2].g = ease(colors[0, 0].g, colors[0, 1].g, percentage);
		colors[0, 2].b = ease(colors[0, 0].b, colors[0, 1].b, percentage);
		colors[0, 2].a = ease(colors[0, 0].a, colors[0, 1].a, percentage);

		tweenArguments["onupdateparams"] = colors[0, 2];

		if (percentage == 1)
		{
			tweenArguments["onupdateparams"] = colors[0, 1];
		}
	}

	private void ApplyVector3Targets()
	{
		vector3s[2].x = ease(vector3s[0].x, vector3s[1].x, percentage);
		vector3s[2].y = ease(vector3s[0].y, vector3s[1].y, percentage);
		vector3s[2].z = ease(vector3s[0].z, vector3s[1].z, percentage);

		tweenArguments["onupdateparams"] = vector3s[2];

		if (percentage == 1)
		{
			tweenArguments["onupdateparams"] = vector3s[1];
		}
	}

	private void ApplyVector2Targets()
	{
		vector2s[2].x = ease(vector2s[0].x, vector2s[1].x, percentage);
		vector2s[2].y = ease(vector2s[0].y, vector2s[1].y, percentage);

		tweenArguments["onupdateparams"] = vector2s[2];

		if (percentage == 1)
		{
			tweenArguments["onupdateparams"] = vector2s[1];
		}
	}

	private void ApplyFloatTargets()
	{
		floats[2] = ease(floats[0], floats[1], percentage);

		tweenArguments["onupdateparams"] = floats[2];

		if (percentage == 1)
		{
			tweenArguments["onupdateparams"] = floats[1];
		}
	}

	private void ApplyColorToTargets()
	{
		for (int i = 0; i < colors.GetLength(0); i++)
		{
			colors[i, 2].r = ease(colors[i, 0].r, colors[i, 1].r, percentage);
			colors[i, 2].g = ease(colors[i, 0].g, colors[i, 1].g, percentage);
			colors[i, 2].b = ease(colors[i, 0].b, colors[i, 1].b, percentage);
			colors[i, 2].a = ease(colors[i, 0].a, colors[i, 1].a, percentage);
		}


		if (GetComponent<Renderer>())
		{
			for (int i = 0; i < colors.GetLength(0); i++)
			{
				GetComponent<Renderer>().materials[i].SetColor(namedcolorvalue.ToString(), colors[i, 2]);
			}
		}
		else if (GetComponent<Light>())
		{
			GetComponent<Light>().color = colors[0, 2];
		}

		if (percentage == 1)
		{
			if (GetComponent<Renderer>())
			{
				for (int i = 0; i < colors.GetLength(0); i++)
				{
					GetComponent<Renderer>().materials[i].SetColor(namedcolorvalue.ToString(), colors[i, 1]);
				}
			}
			else if (GetComponent<Light>())
			{
				GetComponent<Light>().color = colors[0, 1];
			}
		}
	}

	private void ApplyAudioToTargets()
	{
		vector2s[2].x = ease(vector2s[0].x, vector2s[1].x, percentage);
		vector2s[2].y = ease(vector2s[0].y, vector2s[1].y, percentage);

		audioSource.volume = vector2s[2].x;
		audioSource.pitch = vector2s[2].y;

		if (percentage == 1)
		{
			audioSource.volume = vector2s[1].x;
			audioSource.pitch = vector2s[1].y;
		}
	}

	private void ApplyStabTargets()
	{
	}

	private void ApplyMoveToPathTargets()
	{
		preUpdate = thisTransform.position;
		float t = ease(0, 1, percentage);
		float lookAheadAmount;

		if (isLocal)
		{
			thisTransform.localPosition = path.Interp(Mathf.Clamp(t, 0, 1));
		}
		else
		{
			thisTransform.position = path.Interp(Mathf.Clamp(t, 0, 1));
		}

		if (tweenArguments.Contains("orienttopath") && (bool) tweenArguments["orienttopath"])
		{
			float tLook;
			if (tweenArguments.Contains("lookahead"))
			{
				lookAheadAmount = (float) tweenArguments["lookahead"];
			}
			else
			{
				lookAheadAmount = Defaults.lookAhead;
			}

			tLook = ease(0, 1, Mathf.Min(1f, percentage + lookAheadAmount));

			tweenArguments["looktarget"] = path.Interp(Mathf.Clamp(tLook, 0, 1));
		}

		postUpdate = thisTransform.position;
		if (physics)
		{
			thisTransform.position = preUpdate;
			GetComponent<Rigidbody>().MovePosition(postUpdate);
		}
	}

	private void ApplyMoveToTargets()
	{
		preUpdate = thisTransform.position;


		vector3s[2].x = ease(vector3s[0].x, vector3s[1].x, percentage);
		vector3s[2].y = ease(vector3s[0].y, vector3s[1].y, percentage);
		vector3s[2].z = ease(vector3s[0].z, vector3s[1].z, percentage);

		if (isLocal)
		{
			thisTransform.localPosition = vector3s[2];
		}
		else
		{
			thisTransform.position = vector3s[2];
		}

		if (percentage == 1)
		{
			if (isLocal)
			{
				thisTransform.localPosition = vector3s[1];
			}
			else
			{
				thisTransform.position = vector3s[1];
			}
		}

		postUpdate = thisTransform.position;
		if (physics)
		{
			thisTransform.position = preUpdate;
			GetComponent<Rigidbody>().MovePosition(postUpdate);
		}
	}

	private void ApplyMoveByTargets()
	{
		preUpdate = thisTransform.position;

		Vector3 currentRotation = new Vector3();

		if (tweenArguments.Contains("looktarget"))
		{
			currentRotation = thisTransform.eulerAngles;
			thisTransform.eulerAngles = vector3s[4];
		}

		vector3s[2].x = ease(vector3s[0].x, vector3s[1].x, percentage);
		vector3s[2].y = ease(vector3s[0].y, vector3s[1].y, percentage);
		vector3s[2].z = ease(vector3s[0].z, vector3s[1].z, percentage);

		thisTransform.Translate(vector3s[2] - vector3s[3], space);

		vector3s[3] = vector3s[2];

		if (tweenArguments.Contains("looktarget"))
		{
			thisTransform.eulerAngles = currentRotation;
		}


		postUpdate = thisTransform.position;
		if (physics)
		{
			thisTransform.position = preUpdate;
			GetComponent<Rigidbody>().MovePosition(postUpdate);
		}
	}

	private void ApplyScaleToTargets()
	{
		vector3s[2].x = ease(vector3s[0].x, vector3s[1].x, percentage);
		vector3s[2].y = ease(vector3s[0].y, vector3s[1].y, percentage);
		vector3s[2].z = ease(vector3s[0].z, vector3s[1].z, percentage);

		thisTransform.localScale = vector3s[2];

		if (percentage == 1)
		{
			thisTransform.localScale = vector3s[1];
		}
	}

	private void ApplyLookToTargets()
	{
		vector3s[2].x = ease(vector3s[0].x, vector3s[1].x, percentage);
		vector3s[2].y = ease(vector3s[0].y, vector3s[1].y, percentage);
		vector3s[2].z = ease(vector3s[0].z, vector3s[1].z, percentage);

		if (isLocal)
		{
			thisTransform.localRotation = Quaternion.Euler(vector3s[2]);
		}
		else
		{
			thisTransform.rotation = Quaternion.Euler(vector3s[2]);
		}
	}

	private void ApplyRotateToTargets()
	{
		preUpdate = thisTransform.eulerAngles;

		vector3s[2].x = ease(vector3s[0].x, vector3s[1].x, percentage);
		vector3s[2].y = ease(vector3s[0].y, vector3s[1].y, percentage);
		vector3s[2].z = ease(vector3s[0].z, vector3s[1].z, percentage);

		if (isLocal)
		{
			thisTransform.localRotation = Quaternion.Euler(vector3s[2]);
		}
		else
		{
			thisTransform.rotation = Quaternion.Euler(vector3s[2]);
		}

		if (percentage == 1)
		{
			if (isLocal)
			{
				thisTransform.localRotation = Quaternion.Euler(vector3s[1]);
			}
			else
			{
				thisTransform.rotation = Quaternion.Euler(vector3s[1]);
			}
		}

		postUpdate = thisTransform.eulerAngles;
		if (physics)
		{
			thisTransform.eulerAngles = preUpdate;
			GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(postUpdate));
		}
	}

	private void ApplyRotateAddTargets()
	{
		preUpdate = thisTransform.eulerAngles;

		vector3s[2].x = ease(vector3s[0].x, vector3s[1].x, percentage);
		vector3s[2].y = ease(vector3s[0].y, vector3s[1].y, percentage);
		vector3s[2].z = ease(vector3s[0].z, vector3s[1].z, percentage);

		thisTransform.Rotate(vector3s[2] - vector3s[3], space);

		vector3s[3] = vector3s[2];

		postUpdate = thisTransform.eulerAngles;
		if (physics)
		{
			thisTransform.eulerAngles = preUpdate;
			GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(postUpdate));
		}
	}

	private void ApplyShakePositionTargets()
	{
		if (isLocal)
		{
			preUpdate = thisTransform.localPosition;
		}
		else
		{
			preUpdate = thisTransform.position;
		}

		Vector3 currentRotation = new Vector3();

		if (tweenArguments.Contains("looktarget"))
		{
			currentRotation = thisTransform.eulerAngles;
			thisTransform.eulerAngles = vector3s[3];
		}

		if (percentage == 0)
		{
			thisTransform.Translate(vector3s[1], space);
		}

		if (isLocal)
		{
			thisTransform.localPosition = vector3s[0];
		}
		else
		{
			thisTransform.position = vector3s[0];
		}

		float diminishingControl = 1 - percentage;
		vector3s[2].x =
			UnityEngine.Random.Range(-vector3s[1].x * diminishingControl, vector3s[1].x * diminishingControl);
		vector3s[2].y =
			UnityEngine.Random.Range(-vector3s[1].y * diminishingControl, vector3s[1].y * diminishingControl);
		vector3s[2].z =
			UnityEngine.Random.Range(-vector3s[1].z * diminishingControl, vector3s[1].z * diminishingControl);

		if (isLocal)
		{
			thisTransform.localPosition += vector3s[2];
		}
		else
		{
			thisTransform.position += vector3s[2];
		}

		if (tweenArguments.Contains("looktarget"))
		{
			thisTransform.eulerAngles = currentRotation;
		}

		postUpdate = thisTransform.position;
		if (physics)
		{
			thisTransform.position = preUpdate;
			GetComponent<Rigidbody>().MovePosition(postUpdate);
		}
	}

	private void ApplyShakeScaleTargets()
	{
		if (percentage == 0)
		{
			thisTransform.localScale = vector3s[1];
		}

		thisTransform.localScale = vector3s[0];

		float diminishingControl = 1 - percentage;
		vector3s[2].x =
			UnityEngine.Random.Range(-vector3s[1].x * diminishingControl, vector3s[1].x * diminishingControl);
		vector3s[2].y =
			UnityEngine.Random.Range(-vector3s[1].y * diminishingControl, vector3s[1].y * diminishingControl);
		vector3s[2].z =
			UnityEngine.Random.Range(-vector3s[1].z * diminishingControl, vector3s[1].z * diminishingControl);

		thisTransform.localScale += vector3s[2];
	}

	private void ApplyShakeRotationTargets()
	{
		preUpdate = thisTransform.eulerAngles;

		if (percentage == 0)
		{
			thisTransform.Rotate(vector3s[1], space);
		}

		thisTransform.eulerAngles = vector3s[0];

		float diminishingControl = 1 - percentage;
		vector3s[2].x =
			UnityEngine.Random.Range(-vector3s[1].x * diminishingControl, vector3s[1].x * diminishingControl);
		vector3s[2].y =
			UnityEngine.Random.Range(-vector3s[1].y * diminishingControl, vector3s[1].y * diminishingControl);
		vector3s[2].z =
			UnityEngine.Random.Range(-vector3s[1].z * diminishingControl, vector3s[1].z * diminishingControl);

		thisTransform.Rotate(vector3s[2], space);

		postUpdate = thisTransform.eulerAngles;
		if (physics)
		{
			thisTransform.eulerAngles = preUpdate;
			GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(postUpdate));
		}
	}

	private void ApplyPunchPositionTargets()
	{
		preUpdate = thisTransform.position;

		Vector3 currentRotation = new Vector3();

		if (tweenArguments.Contains("looktarget"))
		{
			currentRotation = thisTransform.eulerAngles;
			thisTransform.eulerAngles = vector3s[4];
		}

		if (vector3s[1].x > 0)
		{
			vector3s[2].x = punch(vector3s[1].x, percentage);
		}
		else if (vector3s[1].x < 0)
		{
			vector3s[2].x = -punch(Mathf.Abs(vector3s[1].x), percentage);
		}

		if (vector3s[1].y > 0)
		{
			vector3s[2].y = punch(vector3s[1].y, percentage);
		}
		else if (vector3s[1].y < 0)
		{
			vector3s[2].y = -punch(Mathf.Abs(vector3s[1].y), percentage);
		}

		if (vector3s[1].z > 0)
		{
			vector3s[2].z = punch(vector3s[1].z, percentage);
		}
		else if (vector3s[1].z < 0)
		{
			vector3s[2].z = -punch(Mathf.Abs(vector3s[1].z), percentage);
		}

		thisTransform.Translate(vector3s[2] - vector3s[3], space);

		vector3s[3] = vector3s[2];

		if (tweenArguments.Contains("looktarget"))
		{
			thisTransform.eulerAngles = currentRotation;
		}


		postUpdate = thisTransform.position;
		if (physics)
		{
			thisTransform.position = preUpdate;
			GetComponent<Rigidbody>().MovePosition(postUpdate);
		}
	}

	private void ApplyPunchRotationTargets()
	{
		preUpdate = thisTransform.eulerAngles;

		if (vector3s[1].x > 0)
		{
			vector3s[2].x = punch(vector3s[1].x, percentage);
		}
		else if (vector3s[1].x < 0)
		{
			vector3s[2].x = -punch(Mathf.Abs(vector3s[1].x), percentage);
		}

		if (vector3s[1].y > 0)
		{
			vector3s[2].y = punch(vector3s[1].y, percentage);
		}
		else if (vector3s[1].y < 0)
		{
			vector3s[2].y = -punch(Mathf.Abs(vector3s[1].y), percentage);
		}

		if (vector3s[1].z > 0)
		{
			vector3s[2].z = punch(vector3s[1].z, percentage);
		}
		else if (vector3s[1].z < 0)
		{
			vector3s[2].z = -punch(Mathf.Abs(vector3s[1].z), percentage);
		}

		thisTransform.Rotate(vector3s[2] - vector3s[3], space);

		vector3s[3] = vector3s[2];


		postUpdate = thisTransform.eulerAngles;
		if (physics)
		{
			thisTransform.eulerAngles = preUpdate;
			GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(postUpdate));
		}
	}

	private void ApplyPunchScaleTargets()
	{
		if (vector3s[1].x > 0)
		{
			vector3s[2].x = punch(vector3s[1].x, percentage);
		}
		else if (vector3s[1].x < 0)
		{
			vector3s[2].x = -punch(Mathf.Abs(vector3s[1].x), percentage);
		}

		if (vector3s[1].y > 0)
		{
			vector3s[2].y = punch(vector3s[1].y, percentage);
		}
		else if (vector3s[1].y < 0)
		{
			vector3s[2].y = -punch(Mathf.Abs(vector3s[1].y), percentage);
		}

		if (vector3s[1].z > 0)
		{
			vector3s[2].z = punch(vector3s[1].z, percentage);
		}
		else if (vector3s[1].z < 0)
		{
			vector3s[2].z = -punch(Mathf.Abs(vector3s[1].z), percentage);
		}

		thisTransform.localScale = vector3s[0] + vector3s[2];
	}

	#endregion

	#region #5 Tween Steps

	private IEnumerator TweenDelay()
	{
		delayStarted = Time.time;
		yield return new WaitForSeconds(delay);
		if (wasPaused)
		{
			wasPaused = false;
			TweenStart();
		}
	}

	private void TweenStart()
	{
		CallBack("onstart");

		if (!loop)
		{
			ConflictCheck();
			GenerateTargets();
		}

		if (type == "stab")
		{
			audioSource.PlayOneShot(audioSource.clip);
		}

		if (type == "move" || type == "scale" || type == "rotate" || type == "punch" || type == "shake" ||
		    type == "curve" || type == "look")
		{
			EnableKinematic();
		}

		isRunning = true;
	}

	private IEnumerator TweenRestart()
	{
		if (delay > 0)
		{
			delayStarted = Time.time;
			yield return new WaitForSeconds(delay);
		}

		loop = true;
		TweenStart();
	}

	private void TweenUpdate()
	{
		apply();
		CallBack("onupdate");
		UpdatePercentage();
	}

	private void TweenComplete()
	{
		isRunning = false;

		if (percentage > .5f)
		{
			percentage = 1f;
		}
		else
		{
			percentage = 0;
		}

		apply();
		if (type == "value")
		{
			CallBack("onupdate");
		}

		if (loopType == LoopType.none)
		{
			Dispose();
		}
		else
		{
			TweenLoop();
		}

		CallBack("oncomplete");
	}

	private void TweenLoop()
	{
		DisableKinematic();
		switch (loopType)
		{
			case LoopType.loop:
				percentage = 0;
				runningTime = 0;
				apply();

				StartCoroutine("TweenRestart");
				break;
			case LoopType.pingPong:
				reverse = !reverse;
				runningTime = 0;

				StartCoroutine("TweenRestart");
				break;
		}
	}

	#endregion

	#region #6 Update Callable

	public static Rect RectUpdate(Rect currentValue, Rect targetValue, float speed)
	{
		Rect diff = new Rect(FloatUpdate(currentValue.x, targetValue.x, speed),
			FloatUpdate(currentValue.y, targetValue.y, speed),
			FloatUpdate(currentValue.width, targetValue.width, speed),
			FloatUpdate(currentValue.height, targetValue.height, speed));
		return (diff);
	}

	public static Vector3 Vector3Update(Vector3 currentValue, Vector3 targetValue, float speed)
	{
		Vector3 diff = targetValue - currentValue;
		currentValue += (diff * speed) * Time.deltaTime;
		return (currentValue);
	}

	public static Vector2 Vector2Update(Vector2 currentValue, Vector2 targetValue, float speed)
	{
		Vector2 diff = targetValue - currentValue;
		currentValue += (diff * speed) * Time.deltaTime;
		return (currentValue);
	}

	public static float FloatUpdate(float currentValue, float targetValue, float speed)
	{
		float diff = targetValue - currentValue;
		currentValue += (diff * speed) * Time.deltaTime;
		return (currentValue);
	}

	public static void FadeUpdate(GameObject target, Hashtable args)
	{
		args["a"] = args["alpha"];
		ColorUpdate(target, args);
	}

	public static void FadeUpdate(GameObject target, float alpha, float time)
	{
		FadeUpdate(target, Hash("alpha", alpha, "time", time));
	}

	public static void ColorUpdate(GameObject target, Hashtable args)
	{
		CleanArgs(args);

		float time;
		Color[] colors = new Color[4];

		if (!args.Contains("includechildren") || (bool) args["includechildren"])
		{
			foreach (Transform child in target.transform)
			{
				ColorUpdate(child.gameObject, args);
			}
		}

		if (args.Contains("time"))
		{
			time = (float) args["time"];
			time *= Defaults.updateTimePercentage;
		}
		else
		{
			time = Defaults.updateTime;
		}

		if (target.GetComponent<Renderer>())
		{
			colors[0] = colors[1] = target.GetComponent<Renderer>().material.color;
		}
		else if (target.GetComponent<Light>())
		{
			colors[0] = colors[1] = target.GetComponent<Light>().color;
		}

		if (args.Contains("color"))
		{
			colors[1] = (Color) args["color"];
		}
		else
		{
			if (args.Contains("r"))
			{
				colors[1].r = (float) args["r"];
			}

			if (args.Contains("g"))
			{
				colors[1].g = (float) args["g"];
			}

			if (args.Contains("b"))
			{
				colors[1].b = (float) args["b"];
			}

			if (args.Contains("a"))
			{
				colors[1].a = (float) args["a"];
			}
		}

		colors[3].r = Mathf.SmoothDamp(colors[0].r, colors[1].r, ref colors[2].r, time);
		colors[3].g = Mathf.SmoothDamp(colors[0].g, colors[1].g, ref colors[2].g, time);
		colors[3].b = Mathf.SmoothDamp(colors[0].b, colors[1].b, ref colors[2].b, time);
		colors[3].a = Mathf.SmoothDamp(colors[0].a, colors[1].a, ref colors[2].a, time);

		if (target.GetComponent<Renderer>())
		{
			target.GetComponent<Renderer>().material.color = colors[3];
		}
		else if (target.GetComponent<Light>())
		{
			target.GetComponent<Light>().color = colors[3];
		}
	}

	public static void ColorUpdate(GameObject target, Color color, float time)
	{
		ColorUpdate(target, Hash("color", color, "time", time));
	}

	public static void AudioUpdate(GameObject target, Hashtable args)
	{
		CleanArgs(args);

		AudioSource audioSource;
		float time;
		Vector2[] vector2s = new Vector2[4];

		if (args.Contains("time"))
		{
			time = (float) args["time"];
			time *= Defaults.updateTimePercentage;
		}
		else
		{
			time = Defaults.updateTime;
		}

		if (args.Contains("audiosource"))
		{
			audioSource = (AudioSource) args["audiosource"];
		}
		else
		{
			if (target.GetComponent<AudioSource>())
			{
				audioSource = target.GetComponent<AudioSource>();
			}
			else
			{
				Debug.LogError("iTween Error: AudioUpdate requires an AudioSource.");
				return;
			}
		}

		vector2s[0] = vector2s[1] = new Vector2(audioSource.volume, audioSource.pitch);

		if (args.Contains("volume"))
		{
			vector2s[1].x = (float) args["volume"];
		}

		if (args.Contains("pitch"))
		{
			vector2s[1].y = (float) args["pitch"];
		}

		vector2s[3].x = Mathf.SmoothDampAngle(vector2s[0].x, vector2s[1].x, ref vector2s[2].x, time);
		vector2s[3].y = Mathf.SmoothDampAngle(vector2s[0].y, vector2s[1].y, ref vector2s[2].y, time);

		audioSource.volume = vector2s[3].x;
		audioSource.pitch = vector2s[3].y;
	}

	public static void AudioUpdate(GameObject target, float volume, float pitch, float time)
	{
		AudioUpdate(target, Hash("volume", volume, "pitch", pitch, "time", time));
	}

	public static void RotateUpdate(GameObject target, Hashtable args)
	{
		CleanArgs(args);

		bool isLocal;
		float time;
		Vector3[] vector3s = new Vector3[4];
		Vector3 preUpdate = target.transform.eulerAngles;

		if (args.Contains("time"))
		{
			time = (float) args["time"];
			time *= Defaults.updateTimePercentage;
		}
		else
		{
			time = Defaults.updateTime;
		}

		if (args.Contains("islocal"))
		{
			isLocal = (bool) args["islocal"];
		}
		else
		{
			isLocal = Defaults.isLocal;
		}

		if (isLocal)
		{
			vector3s[0] = target.transform.localEulerAngles;
		}
		else
		{
			vector3s[0] = target.transform.eulerAngles;
		}

		if (args.Contains("rotation"))
		{
			if (args["rotation"].GetType() == typeof(Transform))
			{
				Transform trans = (Transform) args["rotation"];
				vector3s[1] = trans.eulerAngles;
			}
			else if (args["rotation"] is Vector3)
			{
				vector3s[1] = (Vector3) args["rotation"];
			}
		}

		vector3s[3].x = Mathf.SmoothDampAngle(vector3s[0].x, vector3s[1].x, ref vector3s[2].x, time);
		vector3s[3].y = Mathf.SmoothDampAngle(vector3s[0].y, vector3s[1].y, ref vector3s[2].y, time);
		vector3s[3].z = Mathf.SmoothDampAngle(vector3s[0].z, vector3s[1].z, ref vector3s[2].z, time);

		if (isLocal)
		{
			target.transform.localEulerAngles = vector3s[3];
		}
		else
		{
			target.transform.eulerAngles = vector3s[3];
		}

		if (target.GetComponent<Rigidbody>() != null)
		{
			Vector3 postUpdate = target.transform.eulerAngles;
			target.transform.eulerAngles = preUpdate;
			target.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(postUpdate));
		}
	}

	public static void RotateUpdate(GameObject target, Vector3 rotation, float time)
	{
		RotateUpdate(target, Hash("rotation", rotation, "time", time));
	}

	public static void ScaleUpdate(GameObject target, Hashtable args)
	{
		CleanArgs(args);

		float time;
		Vector3[] vector3s = new Vector3[4];

		if (args.Contains("time"))
		{
			time = (float) args["time"];
			time *= Defaults.updateTimePercentage;
		}
		else
		{
			time = Defaults.updateTime;
		}

		vector3s[0] = vector3s[1] = target.transform.localScale;

		if (args.Contains("scale"))
		{
			if (args["scale"].GetType() == typeof(Transform))
			{
				Transform trans = (Transform) args["scale"];
				vector3s[1] = trans.localScale;
			}
			else if (args["scale"] is Vector3)
			{
				vector3s[1] = (Vector3) args["scale"];
			}
		}
		else
		{
			if (args.Contains("x"))
			{
				vector3s[1].x = (float) args["x"];
			}

			if (args.Contains("y"))
			{
				vector3s[1].y = (float) args["y"];
			}

			if (args.Contains("z"))
			{
				vector3s[1].z = (float) args["z"];
			}
		}

		vector3s[3].x = Mathf.SmoothDamp(vector3s[0].x, vector3s[1].x, ref vector3s[2].x, time);
		vector3s[3].y = Mathf.SmoothDamp(vector3s[0].y, vector3s[1].y, ref vector3s[2].y, time);
		vector3s[3].z = Mathf.SmoothDamp(vector3s[0].z, vector3s[1].z, ref vector3s[2].z, time);

		target.transform.localScale = vector3s[3];
	}

	public static void ScaleUpdate(GameObject target, Vector3 scale, float time)
	{
		ScaleUpdate(target, Hash("scale", scale, "time", time));
	}

	public static void MoveUpdate(GameObject target, Hashtable args)
	{
		CleanArgs(args);

		float time;
		Vector3[] vector3s = new Vector3[4];
		bool isLocal;
		Vector3 preUpdate = target.transform.position;

		if (args.Contains("time"))
		{
			time = (float) args["time"];
			time *= Defaults.updateTimePercentage;
		}
		else
		{
			time = Defaults.updateTime;
		}

		if (args.Contains("islocal"))
		{
			isLocal = (bool) args["islocal"];
		}
		else
		{
			isLocal = Defaults.isLocal;
		}

		if (isLocal)
		{
			vector3s[0] = vector3s[1] = target.transform.localPosition;
		}
		else
		{
			vector3s[0] = vector3s[1] = target.transform.position;
		}

		if (args.Contains("position"))
		{
			if (args["position"].GetType() == typeof(Transform))
			{
				Transform trans = (Transform) args["position"];
				vector3s[1] = trans.position;
			}
			else if (args["position"] is Vector3)
			{
				vector3s[1] = (Vector3) args["position"];
			}
		}
		else
		{
			if (args.Contains("x"))
				vector3s[1].x = (float) args["x"];

			if (args.Contains("y"))
				vector3s[1].y = (float) args["y"];

			if (args.Contains("z"))
				vector3s[1].z = (float) args["z"];
		}

		vector3s[3].x = Mathf.SmoothDamp(vector3s[0].x, vector3s[1].x, ref vector3s[2].x, time);
		vector3s[3].y = Mathf.SmoothDamp(vector3s[0].y, vector3s[1].y, ref vector3s[2].y, time);
		vector3s[3].z = Mathf.SmoothDamp(vector3s[0].z, vector3s[1].z, ref vector3s[2].z, time);

		if (args.Contains("orienttopath") && (bool) args["orienttopath"])
			args["looktarget"] = vector3s[3];

		if (args.Contains("looktarget"))
			LookUpdate(target, args);

		if (isLocal)
			target.transform.localPosition = vector3s[3];
		else
			target.transform.position = vector3s[3];

		if (target.GetComponent<Rigidbody>() != null)
		{
			Vector3 postUpdate = target.transform.position;
			target.transform.position = preUpdate;
			target.GetComponent<Rigidbody>().MovePosition(postUpdate);
		}
	}

	public static void MoveUpdate(GameObject target, Vector3 position, float time)
	{
		MoveUpdate(target, Hash("position", position, "time", time));
	}

	public static void LookUpdate(GameObject target, Hashtable args)
	{
		CleanArgs(args);

		float time;
		Vector3[] vector3s = new Vector3[5];

		if (args.Contains("looktime"))
		{
			time = (float) args["looktime"];
			time *= Defaults.updateTimePercentage;
		}
		else if (args.Contains("time"))
		{
			time = (float) args["time"] * .15f;
			time *= Defaults.updateTimePercentage;
		}
		else
		{
			time = Defaults.updateTime;
		}

		vector3s[0] = target.transform.eulerAngles;

		if (args.Contains("looktarget"))
		{
			if (args["looktarget"].GetType() == typeof(Transform))
			{
				target.transform.LookAt((Transform) args["looktarget"], (Vector3?) args["up"] ?? Defaults.up);
			}
			else if (args["looktarget"] is Vector3)
			{
				target.transform.LookAt((Vector3) args["looktarget"], (Vector3?) args["up"] ?? Defaults.up);
			}
		}
		else
		{
			Debug.LogError("iTween Error: LookUpdate needs a 'looktarget' property!");
			return;
		}

		vector3s[1] = target.transform.eulerAngles;
		target.transform.eulerAngles = vector3s[0];

		vector3s[3].x = Mathf.SmoothDampAngle(vector3s[0].x, vector3s[1].x, ref vector3s[2].x, time);
		vector3s[3].y = Mathf.SmoothDampAngle(vector3s[0].y, vector3s[1].y, ref vector3s[2].y, time);
		vector3s[3].z = Mathf.SmoothDampAngle(vector3s[0].z, vector3s[1].z, ref vector3s[2].z, time);

		target.transform.eulerAngles = vector3s[3];

		if (args.Contains("axis"))
		{
			vector3s[4] = target.transform.eulerAngles;
			switch ((string) args["axis"])
			{
				case "x":
					vector3s[4].y = vector3s[0].y;
					vector3s[4].z = vector3s[0].z;
					break;
				case "y":
					vector3s[4].x = vector3s[0].x;
					vector3s[4].z = vector3s[0].z;
					break;
				case "z":
					vector3s[4].x = vector3s[0].x;
					vector3s[4].y = vector3s[0].y;
					break;
			}

			target.transform.eulerAngles = vector3s[4];
		}
	}

	public static void LookUpdate(GameObject target, Vector3 looktarget, float time)
	{
		LookUpdate(target, Hash("looktarget", looktarget, "time", time));
	}

	#endregion

	#region #7 External Utilities

	public static float PathLength(Transform[] path)
	{
		Vector3[] suppliedPath = new Vector3[path.Length];
		float pathLength = 0;

		for (int i = 0; i < path.Length; i++)
		{
			suppliedPath[i] = path[i].position;
		}

		Vector3[] vector3s = PathControlPointGenerator(suppliedPath);

		Vector3 prevPt = Interp(vector3s, 0);
		int SmoothAmount = path.Length * 20;
		for (int i = 1; i <= SmoothAmount; i++)
		{
			float pm = (float) i / SmoothAmount;
			Vector3 currPt = Interp(vector3s, pm);
			pathLength += Vector3.Distance(prevPt, currPt);
			prevPt = currPt;
		}

		return pathLength;
	}

	public static float PathLength(Vector3[] path)
	{
		float pathLength = 0;

		Vector3[] vector3s = PathControlPointGenerator(path);

		Vector3 prevPt = Interp(vector3s, 0);
		int SmoothAmount = path.Length * 20;
		for (int i = 1; i <= SmoothAmount; i++)
		{
			float pm = (float) i / SmoothAmount;
			Vector3 currPt = Interp(vector3s, pm);
			pathLength += Vector3.Distance(prevPt, currPt);
			prevPt = currPt;
		}

		return pathLength;
	}


	public static void PutOnPath(GameObject target, Vector3[] path, float percent)
	{
		target.transform.position = Interp(PathControlPointGenerator(path), percent);
	}

	public static void PutOnPath(Transform target, Vector3[] path, float percent)
	{
		target.position = Interp(PathControlPointGenerator(path), percent);
	}

	public static void PutOnPath(GameObject target, Transform[] path, float percent)
	{
		Vector3[] suppliedPath = new Vector3[path.Length];
		for (int i = 0; i < path.Length; i++)
		{
			suppliedPath[i] = path[i].position;
		}

		target.transform.position = Interp(PathControlPointGenerator(suppliedPath), percent);
	}

	public static void PutOnPath(Transform target, Transform[] path, float percent)
	{
		Vector3[] suppliedPath = new Vector3[path.Length];
		for (int i = 0; i < path.Length; i++)
		{
			suppliedPath[i] = path[i].position;
		}

		target.position = Interp(PathControlPointGenerator(suppliedPath), percent);
	}

	public static Vector3 PointOnPath(Transform[] path, float percent)
	{
		Vector3[] suppliedPath = new Vector3[path.Length];
		for (int i = 0; i < path.Length; i++)
		{
			suppliedPath[i] = path[i].position;
		}

		return (Interp(PathControlPointGenerator(suppliedPath), percent));
	}

	public static void DrawLine(Vector3[] line)
	{
		if (line.Length > 0)
		{
			DrawLineHelper(line, Defaults.color, "gizmos");
		}
	}

	public static void DrawLine(Vector3[] line, Color color)
	{
		if (line.Length > 0)
		{
			DrawLineHelper(line, color, "gizmos");
		}
	}

	public static void DrawLine(Transform[] line)
	{
		if (line.Length > 0)
		{
			Vector3[] suppliedLine = new Vector3[line.Length];
			for (int i = 0; i < line.Length; i++)
			{
				suppliedLine[i] = line[i].position;
			}

			DrawLineHelper(suppliedLine, Defaults.color, "gizmos");
		}
	}

	public static void DrawLine(Transform[] line, Color color)
	{
		if (line.Length > 0)
		{
			Vector3[] suppliedLine = new Vector3[line.Length];
			for (int i = 0; i < line.Length; i++)
			{
				suppliedLine[i] = line[i].position;
			}

			DrawLineHelper(suppliedLine, color, "gizmos");
		}
	}

	public static void DrawLineGizmos(Vector3[] line)
	{
		if (line.Length > 0)
		{
			DrawLineHelper(line, Defaults.color, "gizmos");
		}
	}

	public static void DrawLineGizmos(Vector3[] line, Color color)
	{
		if (line.Length > 0)
		{
			DrawLineHelper(line, color, "gizmos");
		}
	}

	public static void DrawLineGizmos(Transform[] line)
	{
		if (line.Length > 0)
		{
			Vector3[] suppliedLine = new Vector3[line.Length];
			for (int i = 0; i < line.Length; i++)
			{
				suppliedLine[i] = line[i].position;
			}

			DrawLineHelper(suppliedLine, Defaults.color, "gizmos");
		}
	}

	public static void DrawLineGizmos(Transform[] line, Color color)
	{
		if (line.Length > 0)
		{
			Vector3[] suppliedLine = new Vector3[line.Length];
			for (int i = 0; i < line.Length; i++)
			{
				suppliedLine[i] = line[i].position;
			}

			DrawLineHelper(suppliedLine, color, "gizmos");
		}
	}

	public static void DrawLineHandles(Vector3[] line)
	{
		if (line.Length > 0)
		{
			DrawLineHelper(line, Defaults.color, "handles");
		}
	}

	public static void DrawLineHandles(Vector3[] line, Color color)
	{
		if (line.Length > 0)
		{
			DrawLineHelper(line, color, "handles");
		}
	}

	public static void DrawLineHandles(Transform[] line)
	{
		if (line.Length > 0)
		{
			Vector3[] suppliedLine = new Vector3[line.Length];
			for (int i = 0; i < line.Length; i++)
			{
				suppliedLine[i] = line[i].position;
			}

			DrawLineHelper(suppliedLine, Defaults.color, "handles");
		}
	}

	public static void DrawLineHandles(Transform[] line, Color color)
	{
		if (line.Length > 0)
		{
			Vector3[] suppliedLine = new Vector3[line.Length];
			for (int i = 0; i < line.Length; i++)
			{
				suppliedLine[i] = line[i].position;
			}

			DrawLineHelper(suppliedLine, color, "handles");
		}
	}

	public static Vector3 PointOnPath(Vector3[] path, float percent)
	{
		return (Interp(PathControlPointGenerator(path), percent));
	}

	public static void DrawPath(Vector3[] path)
	{
		if (path.Length > 0)
		{
			DrawPathHelper(path, Defaults.color, "gizmos");
		}
	}

	public static void DrawPath(Vector3[] path, Color color)
	{
		if (path.Length > 0)
		{
			DrawPathHelper(path, color, "gizmos");
		}
	}

	public static void DrawPath(Transform[] path)
	{
		if (path.Length > 0)
		{
			Vector3[] suppliedPath = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				suppliedPath[i] = path[i].position;
			}

			DrawPathHelper(suppliedPath, Defaults.color, "gizmos");
		}
	}

	public static void DrawPath(Transform[] path, Color color)
	{
		if (path.Length > 0)
		{
			Vector3[] suppliedPath = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				suppliedPath[i] = path[i].position;
			}

			DrawPathHelper(suppliedPath, color, "gizmos");
		}
	}

	public static void DrawPathGizmos(Vector3[] path)
	{
		if (path.Length > 0)
		{
			DrawPathHelper(path, Defaults.color, "gizmos");
		}
	}

	public static void DrawPathGizmos(Vector3[] path, Color color)
	{
		if (path.Length > 0)
		{
			DrawPathHelper(path, color, "gizmos");
		}
	}

	public static void DrawPathGizmos(Transform[] path)
	{
		if (path.Length > 0)
		{
			Vector3[] suppliedPath = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				suppliedPath[i] = path[i].position;
			}

			DrawPathHelper(suppliedPath, Defaults.color, "gizmos");
		}
	}

	public static void DrawPathGizmos(Transform[] path, Color color)
	{
		if (path.Length > 0)
		{
			Vector3[] suppliedPath = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				suppliedPath[i] = path[i].position;
			}

			DrawPathHelper(suppliedPath, color, "gizmos");
		}
	}

	public static void DrawPathHandles(Vector3[] path)
	{
		if (path.Length > 0)
		{
			DrawPathHelper(path, Defaults.color, "handles");
		}
	}

	public static void DrawPathHandles(Vector3[] path, Color color)
	{
		if (path.Length > 0)
		{
			DrawPathHelper(path, color, "handles");
		}
	}

	public static void DrawPathHandles(Transform[] path)
	{
		if (path.Length > 0)
		{
			Vector3[] suppliedPath = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				suppliedPath[i] = path[i].position;
			}

			DrawPathHelper(suppliedPath, Defaults.color, "handles");
		}
	}

	public static void DrawPathHandles(Transform[] path, Color color)
	{
		if (path.Length > 0)
		{
			Vector3[] suppliedPath = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				suppliedPath[i] = path[i].position;
			}

			DrawPathHelper(suppliedPath, color, "handles");
		}
	}


	public static void Resume(GameObject target)
	{
		foreach (var item in target.GetComponents<iTween>())
		{
			item.enabled = true;
		}
	}

	public static void Resume(GameObject target, bool includechildren)
	{
		Resume(target);
		if (includechildren)
		{
			foreach (Transform child in target.transform)
			{
				Resume(child.gameObject, true);
			}
		}
	}

	public static void Resume(GameObject target, string type)
	{
		foreach (iTween item in target.GetComponents<iTween>())
		{
			string targetType = item.type + item.method;
			targetType = targetType.Substring(0, type.Length);
			if (targetType.EqualsIgnoreCase(type))
			{
				item.enabled = true;
			}
		}
	}

	public static void Resume(GameObject target, string type, bool includechildren)
	{
		foreach (iTween item in target.GetComponents<iTween>())
		{
			string targetType = item.type + item.method;
			targetType = targetType.Substring(0, type.Length);
			if (targetType.EqualsIgnoreCase(type))
			{
				item.enabled = true;
			}
		}

		if (includechildren)
		{
			foreach (Transform child in target.transform)
			{
				Resume(child.gameObject, type, true);
			}
		}
	}

	public static void Resume()
	{
		for (int i = 0; i < tweens.Count; i++)
		{
			Hashtable currentTween = tweens[i];
			GameObject target = (GameObject) currentTween["target"];
			Resume(target);
		}
	}

	public static void Resume(string type)
	{
		ArrayList resumeArray = new ArrayList();

		for (int i = 0; i < tweens.Count; i++)
		{
			Hashtable currentTween = tweens[i];
			GameObject target = (GameObject) currentTween["target"];
			resumeArray.Insert(resumeArray.Count, target);
		}

		for (int i = 0; i < resumeArray.Count; i++)
		{
			Resume((GameObject) resumeArray[i], type);
		}
	}


	public static void Pause(GameObject target)
	{
		foreach (iTween item in target.GetComponents<iTween>())
		{
			if (item.delay > 0)
			{
				item.delay -= Time.time - item.delayStarted;
				item.StopCoroutine(nameof(TweenDelay));
			}

			item.isPaused = true;
			item.enabled = false;
		}
	}

	public static void Pause(GameObject target, bool includechildren)
	{
		Pause(target);
		if (includechildren)
		{
			foreach (Transform child in target.transform)
			{
				Pause(child.gameObject, true);
			}
		}
	}

	public static void Pause(GameObject target, string type)
	{
		foreach (iTween item in target.GetComponents<iTween>())
		{
			string targetType = item.type + item.method;
			targetType = targetType.Substring(0, type.Length);
			if (targetType.EqualsIgnoreCase(type))
			{
				if (item.delay > 0)
				{
					item.delay -= Time.time - item.delayStarted;
					item.StopCoroutine(nameof(TweenDelay));
				}

				item.isPaused = true;
				item.enabled = false;
			}
		}
	}

	public static void Pause(GameObject target, string type, bool includechildren)
	{
		foreach (iTween item in target.GetComponents<iTween>())
		{
			string targetType = item.type + item.method;
			targetType = targetType.Substring(0, type.Length);
			if (targetType.EqualsIgnoreCase(type))
			{
				if (item.delay > 0)
				{
					item.delay -= Time.time - item.delayStarted;
					item.StopCoroutine(nameof(TweenDelay));
				}

				item.isPaused = true;
				item.enabled = false;
			}
		}

		if (includechildren)
		{
			foreach (Transform child in target.transform)
			{
				Pause(child.gameObject, type, true);
			}
		}
	}

	public static void Pause()
	{
		for (int i = 0; i < tweens.Count; i++)
		{
			Hashtable currentTween = tweens[i];
			GameObject target = (GameObject) currentTween["target"];
			Pause(target);
		}
	}

	public static void Pause(string type)
	{
		ArrayList pauseArray = new ArrayList();

		for (int i = 0; i < tweens.Count; i++)
		{
			Hashtable currentTween = tweens[i];
			GameObject target = (GameObject) currentTween["target"];
			pauseArray.Insert(pauseArray.Count, target);
		}

		for (int i = 0; i < pauseArray.Count; i++)
		{
			Pause((GameObject) pauseArray[i], type);
		}
	}


	public static int Count()
	{
		return (tweens.Count);
	}

	public static int Count(string type)
	{
		int tweenCount = 0;

		for (int i = 0; i < tweens.Count; i++)
		{
			Hashtable currentTween = tweens[i];
			string targetType = (string) currentTween["type"] + (string) currentTween["method"];
			targetType = targetType.Substring(0, type.Length);
			if (targetType.ToLower() == type.ToLower())
			{
				tweenCount++;
			}
		}

		return (tweenCount);
	}

	public static int Count(GameObject target)
	{
		return target.GetComponents<iTween>().Length;
	}

	public static int Count(GameObject target, string type)
	{
		int tweenCount = 0;
		foreach (iTween item in target.GetComponents<iTween>())
		{
			string targetType = item.type + item.method;
			targetType = targetType.Substring(0, type.Length);
			if (targetType.ToLower() == type.ToLower())
			{
				tweenCount++;
			}
		}

		return (tweenCount);
	}


	public static void Stop()
	{
		for (int i = 0; i < tweens.Count; i++)
		{
			Hashtable currentTween = tweens[i];
			GameObject target = (GameObject) currentTween["target"];
			Stop(target);
		}

		tweens.Clear();
	}

	public static void Stop(string type)
	{
		ArrayList stopArray = new ArrayList();

		for (int i = 0; i < tweens.Count; i++)
		{
			Hashtable currentTween = tweens[i];
			GameObject target = (GameObject) currentTween["target"];
			stopArray.Insert(stopArray.Count, target);
		}

		for (int i = 0; i < stopArray.Count; i++)
		{
			Stop((GameObject) stopArray[i], type);
		}
	}


	public static void StopByName(string name)
	{
		ArrayList stopArray = new ArrayList();

		for (int i = 0; i < tweens.Count; i++)
		{
			Hashtable currentTween = tweens[i];
			GameObject target = (GameObject) currentTween["target"];
			stopArray.Insert(stopArray.Count, target);
		}

		for (int i = 0; i < stopArray.Count; i++)
		{
			StopByName((GameObject) stopArray[i], name);
		}
	}


	public static void Stop(GameObject target)
	{
		foreach (iTween item in target.GetComponents<iTween>())
		{
			item.Dispose();
		}
	}

	public static void Stop(GameObject target, bool includechildren)
	{
		Stop(target);
		if (includechildren)
		{
			foreach (Transform child in target.transform)
			{
				Stop(child.gameObject, true);
			}
		}
	}

	public static void Stop(GameObject target, string type)
	{
		foreach (var component in target.GetComponents<iTween>())
		{
			var item = component;
			string targetType = item.type + item.method;
			targetType = targetType.Substring(0, type.Length);
			if (targetType.EqualsIgnoreCase(type))
			{
				item.Dispose();
			}
		}
	}


	public static void StopByName(GameObject target, string name)
	{
		foreach (var item in target.GetComponents<iTween>())
		{
			if (item._name == name)
			{
				item.Dispose();
			}
		}
	}


	public static void Stop(GameObject target, string type, bool includechildren)
	{
		foreach (var item in target.GetComponents<iTween>())
		{
			string targetType = item.type + item.method;
			targetType = targetType.Substring(0, type.Length);
			if (targetType.EqualsIgnoreCase(type))
			{
				item.Dispose();
			}
		}

		if (includechildren)
		{
			foreach (Transform child in target.transform)
			{
				Stop(child.gameObject, type, true);
			}
		}
	}


	public static void StopByName(GameObject target, string name, bool includechildren)
	{
		foreach (var item in target.GetComponents<iTween>())
		{
			if (item._name == name)
			{
				item.Dispose();
			}
		}

		if (includechildren)
		{
			foreach (Transform child in target.transform)
			{
				StopByName(child.gameObject, name, true);
			}
		}
	}


	public static Hashtable Hash(params object[] args)
	{
		Hashtable hashTable = new Hashtable(args.Length / 2);
		if (args.Length % 2 != 0)
		{
			Debug.LogError("Tween Error: Hash requires an even number of arguments!");
			return null;
		}
		else
		{
			int i = 0;
			while (i < args.Length - 1)
			{
				hashTable.Add(args[i], args[i + 1]);
				i += 2;
			}

			return hashTable;
		}
	}

	#endregion

	#region Component Segments

	private iTween(Hashtable h)
	{
		tweenArguments = h;
	}

	private void Awake()
	{
		thisTransform = transform;

		RetrieveArgs();
		lastRealTime = Time.realtimeSinceStartup;
	}

	private IEnumerator Start()
	{
		if (delay > 0)
		{
			yield return StartCoroutine("TweenDelay");
		}

		TweenStart();
	}

	private void Update()
	{
		if (isRunning && !physics)
		{
			if (!reverse)
			{
				if (percentage < 1f)
				{
					TweenUpdate();
				}
				else
				{
					TweenComplete();
				}
			}
			else
			{
				if (percentage > 0)
				{
					TweenUpdate();
				}
				else
				{
					TweenComplete();
				}
			}
		}
	}

	private void FixedUpdate()
	{
		if (isRunning && physics)
		{
			if (!reverse)
			{
				if (percentage < 1f)
				{
					TweenUpdate();
				}
				else
				{
					TweenComplete();
				}
			}
			else
			{
				if (percentage > 0)
				{
					TweenUpdate();
				}
				else
				{
					TweenComplete();
				}
			}
		}
	}

	private void LateUpdate()
	{
		if (tweenArguments.Contains("looktarget") && isRunning)
		{
			if (type == "move" || type == "shake" || type == "punch")
			{
				LookUpdate(gameObject, tweenArguments);
			}
		}
	}

	private void OnEnable()
	{
		if (isRunning)
		{
			EnableKinematic();
		}

		if (isPaused)
		{
			isPaused = false;
			if (delay > 0)
			{
				wasPaused = true;
				ResumeDelay();
			}
		}
	}

	private void OnDisable()
	{
		DisableKinematic();
	}

	#endregion

	#region Internal Helpers

	private static void DrawLineHelper(Vector3[] line, Color color, string method)
	{
		Gizmos.color = color;
		for (int i = 0; i < line.Length - 1; i++)
		{
			if (method == "gizmos")
			{
				Gizmos.DrawLine(line[i], line[i + 1]);
			}
			else if (method == "handles")
			{
				Debug.LogError(
					"iTween Error: Drawing a line with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
			}
		}
	}

	private static void DrawPathHelper(Vector3[] path, Color color, string method)
	{
		Vector3[] vector3s = PathControlPointGenerator(path);

		Vector3 prevPt = Interp(vector3s, 0);
		Gizmos.color = color;
		int SmoothAmount = path.Length * 20;
		for (int i = 1; i <= SmoothAmount; i++)
		{
			float pm = (float) i / SmoothAmount;
			Vector3 currPt = Interp(vector3s, pm);
			if (method == "gizmos")
			{
				Gizmos.DrawLine(currPt, prevPt);
			}
			else if (method == "handles")
			{
				Debug.LogError(
					"iTween Error: Drawing a path with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
			}

			prevPt = currPt;
		}
	}

	private static Vector3[] PathControlPointGenerator(Vector3[] path)
	{
		Vector3[] suppliedPath;
		Vector3[] vector3s;

		suppliedPath = path;

		int offset = 2;
		vector3s = new Vector3[suppliedPath.Length + offset];
		Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);

		vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
		vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] +
		                                (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);

		if (vector3s[1] == vector3s[vector3s.Length - 2])
		{
			Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
			Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
			tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
			tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
			vector3s = new Vector3[tmpLoopSpline.Length];
			Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
		}

		return (vector3s);
	}

	private static Vector3 Interp(Vector3[] pts, float t)
	{
		int numSections = pts.Length - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * numSections), numSections - 1);
		float u = t * numSections - currPt;

		Vector3 a = pts[currPt];
		Vector3 b = pts[currPt + 1];
		Vector3 c = pts[currPt + 2];
		Vector3 d = pts[currPt + 3];

		return .5f * (
			       (-a + 3f * b - 3f * c + d) * (u * u * u)
			       + (2f * a - 5f * b + 4f * c - d) * (u * u)
			       + (-a + c) * u
			       + 2f * b
		       );
	}

	private class CRSpline
	{
		public Vector3[] pts;

		public CRSpline(params Vector3[] pts)
		{
			this.pts = new Vector3[pts.Length];
			Array.Copy(pts, this.pts, pts.Length);
		}


		public Vector3 Interp(float t)
		{
			int numSections = pts.Length - 3;
			int currPt = Mathf.Min(Mathf.FloorToInt(t * numSections), numSections - 1);
			float u = t * numSections - currPt;
			Vector3 a = pts[currPt];
			Vector3 b = pts[currPt + 1];
			Vector3 c = pts[currPt + 2];
			Vector3 d = pts[currPt + 3];
			return .5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) + (2f * a - 5f * b + 4f * c - d) * (u * u) +
			              (-a + c) * u + 2f * b);
		}
	}

	private static void Launch(GameObject target, Hashtable args)
	{
		if (!args.Contains("id"))
		{
			args["id"] = GenerateID();
		}

		if (!args.Contains("target"))
		{
			args["target"] = target;
		}

		tweens.Insert(0, args);
		target.AddComponent<iTween>();
	}

	private static Hashtable CleanArgs(Hashtable args)
	{
		Hashtable argsCopy = new Hashtable(args.Count);
		Hashtable argsCaseUnified = new Hashtable(args.Count);

		foreach (DictionaryEntry item in args)
		{
			argsCopy.Add(item.Key, item.Value);
		}

		foreach (DictionaryEntry item in argsCopy)
		{
			if (item.Value is int)
			{
				int original = (int) item.Value;
				float casted = original;
				args[item.Key] = casted;
			}

			if (item.Value is double)
			{
				double original = (double) item.Value;
				float casted = (float) original;
				args[item.Key] = casted;
			}
		}

		foreach (DictionaryEntry item in args)
		{
			argsCaseUnified.Add(item.Key.ToString().ToLower(), item.Value);
		}

		args = argsCaseUnified;

		return args;
	}

	private static string GenerateID()
	{
		return Guid.NewGuid().ToString();
	}

	private void RetrieveArgs()
	{
		foreach (Hashtable item in tweens)
		{
			if ((GameObject) item["target"] == gameObject)
			{
				tweenArguments = item;
				break;
			}
		}

		id = (string) tweenArguments["id"];
		type = (string) tweenArguments["type"];

		_name = (string) tweenArguments["name"];

		method = (string) tweenArguments["method"];

		if (tweenArguments.Contains("time"))
		{
			time = (float) tweenArguments["time"];
		}
		else
		{
			time = Defaults.time;
		}

		if (GetComponent<Rigidbody>() != null)
		{
			physics = true;
		}

		if (tweenArguments.Contains("delay"))
		{
			delay = (float) tweenArguments["delay"];
		}
		else
		{
			delay = Defaults.delay;
		}

		if (tweenArguments.Contains("namedcolorvalue"))
		{
			if (tweenArguments["namedcolorvalue"].GetType() == typeof(NamedValueColor))
			{
				namedcolorvalue = (NamedValueColor) tweenArguments["namedcolorvalue"];
			}
			else
			{
				try
				{
					namedcolorvalue = (NamedValueColor) Enum.Parse(typeof(NamedValueColor),
						(string) tweenArguments["namedcolorvalue"], true);
				}
				catch
				{
					Debug.LogWarning("iTween: Unsupported namedcolorvalue supplied! Default will be used.");
					namedcolorvalue = NamedValueColor._Color;
				}
			}
		}
		else
		{
			namedcolorvalue = Defaults.namedColorValue;
		}

		if (tweenArguments.Contains("looptype"))
		{
			if (tweenArguments["looptype"].GetType() == typeof(LoopType))
			{
				loopType = (LoopType) tweenArguments["looptype"];
			}
			else
			{
				try
				{
					loopType = (LoopType) Enum.Parse(typeof(LoopType), (string) tweenArguments["looptype"], true);
				}
				catch
				{
					Debug.LogWarning("iTween: Unsupported loopType supplied! Default will be used.");
					loopType = LoopType.none;
				}
			}
		}
		else
		{
			loopType = LoopType.none;
		}

		if (tweenArguments.Contains("easetype"))
		{
			if (tweenArguments["easetype"].GetType() == typeof(EaseType))
			{
				easeType = (EaseType) tweenArguments["easetype"];
			}
			else
			{
				try
				{
					easeType = (EaseType) Enum.Parse(typeof(EaseType), (string) tweenArguments["easetype"], true);
				}
				catch
				{
					Debug.LogWarning("iTween: Unsupported easeType supplied! Default will be used.");
					easeType = Defaults.easeType;
				}
			}
		}
		else
		{
			easeType = Defaults.easeType;
		}

		if (tweenArguments.Contains("space"))
		{
			if (tweenArguments["space"].GetType() == typeof(Space))
			{
				space = (Space) tweenArguments["space"];
			}
			else
			{
				try
				{
					space = (Space) Enum.Parse(typeof(Space), (string) tweenArguments["space"], true);
				}
				catch
				{
					Debug.LogWarning("iTween: Unsupported space supplied! Default will be used.");
					space = Defaults.space;
				}
			}
		}
		else
		{
			space = Defaults.space;
		}

		if (tweenArguments.Contains("islocal"))
		{
			isLocal = (bool) tweenArguments["islocal"];
		}
		else
		{
			isLocal = Defaults.isLocal;
		}

		if (tweenArguments.Contains("ignoretimescale"))
		{
			useRealTime = (bool) tweenArguments["ignoretimescale"];
		}
		else
		{
			useRealTime = Defaults.useRealTime;
		}

		GetEasingFunction();
	}

	private void GetEasingFunction()
	{
		switch (easeType)
		{
			case EaseType.easeInQuad:
				ease = easeInQuad;
				break;
			case EaseType.easeOutQuad:
				ease = easeOutQuad;
				break;
			case EaseType.easeInOutQuad:
				ease = easeInOutQuad;
				break;
			case EaseType.easeInCubic:
				ease = easeInCubic;
				break;
			case EaseType.easeOutCubic:
				ease = easeOutCubic;
				break;
			case EaseType.easeInOutCubic:
				ease = easeInOutCubic;
				break;
			case EaseType.easeInQuart:
				ease = easeInQuart;
				break;
			case EaseType.easeOutQuart:
				ease = easeOutQuart;
				break;
			case EaseType.easeInOutQuart:
				ease = easeInOutQuart;
				break;
			case EaseType.easeInQuint:
				ease = easeInQuint;
				break;
			case EaseType.easeOutQuint:
				ease = easeOutQuint;
				break;
			case EaseType.easeInOutQuint:
				ease = easeInOutQuint;
				break;
			case EaseType.easeInSine:
				ease = easeInSine;
				break;
			case EaseType.easeOutSine:
				ease = easeOutSine;
				break;
			case EaseType.easeInOutSine:
				ease = easeInOutSine;
				break;
			case EaseType.easeInExpo:
				ease = easeInExpo;
				break;
			case EaseType.easeOutExpo:
				ease = easeOutExpo;
				break;
			case EaseType.easeInOutExpo:
				ease = easeInOutExpo;
				break;
			case EaseType.easeInCirc:
				ease = easeInCirc;
				break;
			case EaseType.easeOutCirc:
				ease = easeOutCirc;
				break;
			case EaseType.easeInOutCirc:
				ease = easeInOutCirc;
				break;
			case EaseType.linear:
				ease = linear;
				break;
			case EaseType.spring:
				ease = spring;
				break;


			case EaseType.easeInBounce:
				ease = easeInBounce;
				break;
			case EaseType.easeOutBounce:
				ease = easeOutBounce;
				break;
			case EaseType.easeInOutBounce:
				ease = easeInOutBounce;
				break;

			case EaseType.easeInBack:
				ease = easeInBack;
				break;
			case EaseType.easeOutBack:
				ease = easeOutBack;
				break;
			case EaseType.easeInOutBack:
				ease = easeInOutBack;
				break;


			case EaseType.easeInElastic:
				ease = easeInElastic;
				break;
			case EaseType.easeOutElastic:
				ease = easeOutElastic;
				break;
			case EaseType.easeInOutElastic:
				ease = easeInOutElastic;
				break;
		}
	}

	private void UpdatePercentage()
	{
		if (useRealTime)
		{
			runningTime += (Time.realtimeSinceStartup - lastRealTime);
		}
		else
		{
			runningTime += Time.deltaTime;
		}

		if (reverse)
		{
			percentage = 1 - runningTime / time;
		}
		else
		{
			percentage = runningTime / time;
		}

		lastRealTime = Time.realtimeSinceStartup;
	}

	private void CallBack(string callbackType)
	{
		if (tweenArguments.Contains(callbackType) && !tweenArguments.Contains("ischild"))
		{
			GameObject target;
			if (tweenArguments.Contains(callbackType + "target"))
			{
				target = (GameObject) tweenArguments[callbackType + "target"];
			}
			else
			{
				target = gameObject;
			}

			if (tweenArguments[callbackType] is string)
			{
				target.SendMessage((string) tweenArguments[callbackType],
					tweenArguments[callbackType + "params"], SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				Debug.LogError("iTween Error: Callback method references must be passed as a String!");
				Destroy(this);
			}
		}
	}

	private void Dispose()
	{
		for (int i = 0; i < tweens.Count; i++)
		{
			Hashtable tweenEntry = tweens[i];
			if ((string) tweenEntry["id"] == id)
			{
				tweens.RemoveAt(i);
				break;
			}
		}

		Destroy(this);
	}

	private void ConflictCheck()
	{
		foreach (var component in GetComponents<iTween>())
		{
			var item = component;
			if (item.type == "value")
			{
				return;
			}
			else if (item.isRunning && item.type == type)
			{
				if (item.method != method)
				{
					return;
				}

				if (item.tweenArguments.Count != tweenArguments.Count)
				{
					item.Dispose();
					return;
				}

				foreach (DictionaryEntry currentProp in tweenArguments)
				{
					if (!item.tweenArguments.Contains(currentProp.Key))
					{
						item.Dispose();
						return;
					}
					else
					{
						if (!item.tweenArguments[currentProp.Key].Equals(tweenArguments[currentProp.Key]) &&
						    (string) currentProp.Key != "id")
						{
							item.Dispose();
							return;
						}
					}
				}

				Dispose();
			}
		}
	}

	private void EnableKinematic()
	{
	}

	private void DisableKinematic()
	{
	}

	private void ResumeDelay()
	{
		StartCoroutine("TweenDelay");
	}

	#endregion

	#region Easing Curves

	private float linear(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, value);
	}

	private float clerp(float start, float end, float value)
	{
		float min = 0.0f;
		float max = 360.0f;
		float half = Mathf.Abs((max - min) * 0.5f);
		float retval = 0.0f;
		float diff = 0.0f;
		if ((end - start) < -half)
		{
			diff = ((max - start) + end) * value;
			retval = start + diff;
		}
		else if ((end - start) > half)
		{
			diff = -((max - end) + start) * value;
			retval = start + diff;
		}
		else retval = start + (end - start) * value;

		return retval;
	}

	private float spring(float start, float end, float value)
	{
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) +
		         value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}

	private float easeInQuad(float start, float end, float value)
	{
		end -= start;
		return end * value * value + start;
	}

	private float easeOutQuad(float start, float end, float value)
	{
		end -= start;
		return -end * value * (value - 2) + start;
	}

	private float easeInOutQuad(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * value * value + start;
		value--;
		return -end * 0.5f * (value * (value - 2) - 1) + start;
	}

	private float easeInCubic(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value + start;
	}

	private float easeOutCubic(float start, float end, float value)
	{
		value--;
		end -= start;
		return end * (value * value * value + 1) + start;
	}

	private float easeInOutCubic(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * value * value * value + start;
		value -= 2;
		return end * 0.5f * (value * value * value + 2) + start;
	}

	private float easeInQuart(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value * value + start;
	}

	private float easeOutQuart(float start, float end, float value)
	{
		value--;
		end -= start;
		return -end * (value * value * value * value - 1) + start;
	}

	private float easeInOutQuart(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * value * value * value * value + start;
		value -= 2;
		return -end * 0.5f * (value * value * value * value - 2) + start;
	}

	private float easeInQuint(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value * value * value + start;
	}

	private float easeOutQuint(float start, float end, float value)
	{
		value--;
		end -= start;
		return end * (value * value * value * value * value + 1) + start;
	}

	private float easeInOutQuint(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * value * value * value * value * value + start;
		value -= 2;
		return end * 0.5f * (value * value * value * value * value + 2) + start;
	}

	private float easeInSine(float start, float end, float value)
	{
		end -= start;
		return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
	}

	private float easeOutSine(float start, float end, float value)
	{
		end -= start;
		return end * Mathf.Sin(value * (Mathf.PI * 0.5f)) + start;
	}

	private float easeInOutSine(float start, float end, float value)
	{
		end -= start;
		return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
	}

	private float easeInExpo(float start, float end, float value)
	{
		end -= start;
		return end * Mathf.Pow(2, 10 * (value - 1)) + start;
	}

	private float easeOutExpo(float start, float end, float value)
	{
		end -= start;
		return end * (-Mathf.Pow(2, -10 * value) + 1) + start;
	}

	private float easeInOutExpo(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
		value--;
		return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
	}

	private float easeInCirc(float start, float end, float value)
	{
		end -= start;
		return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
	}

	private float easeOutCirc(float start, float end, float value)
	{
		value--;
		end -= start;
		return end * Mathf.Sqrt(1 - value * value) + start;
	}

	private float easeInOutCirc(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1) return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
		value -= 2;
		return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
	}


	private float easeInBounce(float start, float end, float value)
	{
		end -= start;
		float d = 1f;
		return end - easeOutBounce(0, end, d - value) + start;
	}


	private float easeOutBounce(float start, float end, float value)
	{
		value /= 1f;
		end -= start;
		if (value < (1 / 2.75f))
		{
			return end * (7.5625f * value * value) + start;
		}
		else if (value < (2 / 2.75f))
		{
			value -= (1.5f / 2.75f);
			return end * (7.5625f * (value) * value + .75f) + start;
		}
		else if (value < (2.5 / 2.75))
		{
			value -= (2.25f / 2.75f);
			return end * (7.5625f * (value) * value + .9375f) + start;
		}
		else
		{
			value -= (2.625f / 2.75f);
			return end * (7.5625f * (value) * value + .984375f) + start;
		}
	}


	private float easeInOutBounce(float start, float end, float value)
	{
		end -= start;
		float d = 1f;
		if (value < d * 0.5f) return easeInBounce(0, end, value * 2) * 0.5f + start;
		else return easeOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
	}


	private float easeInBack(float start, float end, float value)
	{
		end -= start;
		value /= 1;
		float s = 1.70158f;
		return end * (value) * value * ((s + 1) * value - s) + start;
	}

	private float easeOutBack(float start, float end, float value)
	{
		float s = 1.70158f;
		end -= start;
		value = (value) - 1;
		return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
	}

	private float easeInOutBack(float start, float end, float value)
	{
		float s = 1.70158f;
		end -= start;
		value /= .5f;
		if ((value) < 1)
		{
			s *= (1.525f);
			return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
		}

		value -= 2;
		s *= (1.525f);
		return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
	}

	private float punch(float amplitude, float value)
	{
		float s = 9;
		if (value == 0)
		{
			return 0;
		}
		else if (value == 1)
		{
			return 0;
		}

		float period = 1 * 0.3f;
		s = period / (2 * Mathf.PI) * Mathf.Asin(0);
		return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
	}


	private float easeInElastic(float start, float end, float value)
	{
		end -= start;

		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;

		if (value == 0) return start;

		if ((value /= d) == 1) return start + end;

		if (a == 0f || a < Mathf.Abs(end))
		{
			a = end;
			s = p / 4;
		}
		else
		{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}

		return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
	}


	private float easeOutElastic(float start, float end, float value)
	{
		end -= start;

		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;

		if (value == 0) return start;

		if ((value /= d) == 1) return start + end;

		if (a == 0f || a < Mathf.Abs(end))
		{
			a = end;
			s = p * 0.25f;
		}
		else
		{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}

		return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
	}


	private float easeInOutElastic(float start, float end, float value)
	{
		end -= start;

		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;

		if (value == 0) return start;

		if ((value /= d * 0.5f) == 2) return start + end;

		if (a == 0f || a < Mathf.Abs(end))
		{
			a = end;
			s = p / 4;
		}
		else
		{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}

		if (value < 1)
			return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) +
			       start;
		return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end +
		       start;
	}

	#endregion
} 
