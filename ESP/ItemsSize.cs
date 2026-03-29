using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace ExtrasensoryPerception.ESP;

internal abstract record ItemsSize
{
	[CompilerGenerated]
	protected virtual System.Type EqualityContract
	{
		[CompilerGenerated]
		get
		{
			return typeof(ItemsSize);
		}
	}

	private static Vector2 CurrentResolution => new Vector2((float)Screen.width, (float)Screen.height);

	private static float ScaleFactor => Mathf.Min((float)Screen.width / ReferenceResolution.x, (float)Screen.height / ReferenceResolution.y);

	internal static Vector2 DefaultRectSize => new Vector2(40f, 100f) * (CurrentResolution / ReferenceResolution);

	internal static float FontDist => 12f * ScaleFactor;

	internal static float FontDistMin => 7.2f * ScaleFactor;

	internal static float FontSize => 11f * ScaleFactor;

	internal static float FontSizeMin => 8f * ScaleFactor;

	private static readonly Vector2 ReferenceResolution = new Vector2(1920f, 1080f);

	[CompilerGenerated]
	public override string ToString()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		StringBuilder val = new StringBuilder();
		val.Append("ItemsSize");
		val.Append(" { ");
		if (PrintMembers(val))
		{
			val.Append(' ');
		}
		val.Append('}');
		return ((object)val).ToString();
	}

	[CompilerGenerated]
	public virtual bool Equals(ItemsSize? other)
	{
		if ((object)this != other)
		{
			if ((object)other != null)
			{
				return EqualityContract == other.EqualityContract;
			}
			return false;
		}
		return true;
	}

	[CompilerGenerated]
	protected ItemsSize(ItemsSize original)
	{
	}

	protected ItemsSize()
	{
	}
}
