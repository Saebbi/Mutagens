﻿using System;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;

namespace Mitosis
{
	// Token: 0x02000006 RID: 6
	public static class DefOfHelper
	{
		// Token: 0x06000009 RID: 9 RVA: 0x00002214 File Offset: 0x00000414
		public static void RebindAllDefOfs(bool earlyTryMode)
		{
			DefOfHelper.earlyTry = earlyTryMode;
			DefOfHelper.bindingNow = true;
			try
			{
				foreach (Type type in GenTypes.AllTypesWithAttribute<DefOf>())
				{
					DefOfHelper.BindDefsFor(type);
				}
			}
			finally
			{
				DefOfHelper.bindingNow = false;
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000227C File Offset: 0x0000047C
		private static void BindDefsFor(Type type)
		{
			foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Static | BindingFlags.Public))
			{
				Type fieldType = fieldInfo.FieldType;
				if (!typeof(Def).IsAssignableFrom(fieldType))
				{
					Log.Error(fieldType + " is not a Def.", false);
				}
				else if (fieldType == typeof(SoundDef))
				{
					SoundDef soundDef = SoundDef.Named(fieldInfo.Name);
					if (soundDef.isUndefined && !DefOfHelper.earlyTry)
					{
						Log.Error("Could not find SoundDef named " + fieldInfo.Name, false);
					}
					fieldInfo.SetValue(null, soundDef);
				}
				else
				{
					Def def = GenDefDatabase.GetDef(fieldType, fieldInfo.Name, !DefOfHelper.earlyTry);
					fieldInfo.SetValue(null, def);
				}
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002340 File Offset: 0x00000540
		public static void EnsureInitializedInCtor(Type defOf)
		{
			if (!DefOfHelper.bindingNow)
			{
				string text;
				if (DirectXmlToObject.currentlyInstantiatingObjectOfType.Any<Type>())
				{
					text = "DirectXmlToObject is currently instantiating an object of type " + DirectXmlToObject.currentlyInstantiatingObjectOfType.Peek();
				}
				else if (Scribe.mode == LoadSaveMode.LoadingVars)
				{
					text = "curParent=" + Scribe.loader.curParent.ToStringSafe<IExposable>() + " curPathRelToParent=" + Scribe.loader.curPathRelToParent;
				}
				else
				{
					text = string.Empty;
				}
				Log.Warning("Tried to use an uninitialized DefOf of type " + defOf.Name + ". DefOfs are initialized right after all defs all loaded. Uninitialized DefOfs will return only nulls. (hint: don't use DefOfs as default field values in Defs, try to resolve them in ResolveReferences() instead)" + ((!text.NullOrEmpty()) ? (" Debug info: " + text) : string.Empty), false);
			}
		}

		// Token: 0x04000003 RID: 3
		private static bool bindingNow;

		// Token: 0x04000004 RID: 4
		private static bool earlyTry = true;
	}
}
