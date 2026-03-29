using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ExtrasensoryPerception.API;
using ExtrasensoryPerception.Utils;
using ExtrasensoryPerception.Utils.Prefabs;
using ProjectM;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ExtrasensoryPerception.ESP;

internal static class Logic
{
	private static Camera? _mainCamera;

	private static readonly Regex CamelCaseRegex = new Regex("(?<=[a-z]|[0-9])([A-Z])", (RegexOptions)8);

	public static Camera MainCamera
	{
		get
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			if ((UnityEngine.Object)(object)_mainCamera != null)
			{
				Camera mainCamera = _mainCamera;
				if (mainCamera != null && ((Behaviour)mainCamera).enabled && ((Behaviour)mainCamera).isActiveAndEnabled)
				{
					goto IL_003e;
				}
			}
			Plugin.Logger.LogWarning((object)"Resetting main camera...");
			_mainCamera = Camera.main;
			goto IL_003e;
			IL_003e:
			return _mainCamera ?? throw new InvalidOperationException();
		}
		set
		{
			_mainCamera = value;
		}
	}

	private static PrefabLookupMap PrefabLookupMap => VWorld.PrefabLookupMap;

	public static void ProcessAllEntities()
	{
		Aimbot.Candidates.Clear();
		ProcessPlayers();
		ProcessVBloodCarriers();
		ProcessMobs();
		ProcessGateBosses();
		if (Config.ESP.Items.Enabled)
		{
			ProcessItems();
		}
		if (Config.ESP.Containers.Enabled)
		{
			ProcessContainers();
		}
		if (Config.ESP.Ores.Enabled)
		{
			ProcessOres();
		}
		if (Config.ESP.Plants.Enabled)
		{
			ProcessPlants();
		}
		if (Config.ESP.FishingSpots.Enabled)
		{
			ProcessFishingSpots();
		}
		if (Config.ESP.Horses.Enabled)
		{
			ProcessHorses();
		}
		if (Config.ESP.Servants.Enabled)
		{
			ProcessServants();
		}
		if (Config.ESP.Carriages.Enabled)
		{
			ProcessCarriages();
		}
	}

	private static void ProcessPlayers()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!Config.ESP.Players.Enabled && !Config.Aimbot.Players.Enabled)
		{
			return;
		}
		EntityList.Players.ForEach(delegate(Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			if (CheckEntity(entity) && entity.TryGetComponent<PlayerCharacter>(out PlayerCharacter componentData) && !(entity == EntityList.LocalPlayer) && entity.TryGetComponent<Blood>(out Blood componentData2) && entity.TryGetComponent<Health>(out Health componentData3) && entity.TryGetComponent<Equipment>(out Equipment componentData4))
			{
				Vector3 position = entity.GetPosition();
				if (GetScreenPoint(position, out var screenPoint))
				{
					bool flag = entity.IsAlly(EntityList.LocalPlayer);
					float distanceFromPlayer = GetDistanceFromPlayer(position);
					if (Config.Aimbot.Players.Enabled && !flag)
					{
						Aimbot.TryAddCandidate(entity, screenPoint, distanceFromPlayer, Aimbot.EntityType.Player);
					}
					if (Config.ESP.Players.Enabled)
					{
						string text = $"[{(int)(componentData4.ArmorLevel.Value + componentData4.WeaponLevel.Value + componentData4.SpellLevel.Value)}] {componentData.Name.ToString()}";
						string text2 = $"HP: {(int)componentData3.Value} | BL: {(int)componentData2.Quality}%";
						Color color = (flag ? Color.green : ColorOptions.GetColor(Config.ESP.Players.Color));
						float num = 20f / Vector3.Distance(((Component)MainCamera).transform.position, position);
						Vector2 val = ItemsSize.DefaultRectSize * num;
						int fontSize = (int)Mathf.Max(ItemsSize.FontSizeMin, ItemsSize.FontSize * num);
						float num2 = Mathf.Max(ItemsSize.FontDistMin, ItemsSize.FontDist * num);
						RenderQueue.String(new Vector2(screenPoint.x, screenPoint.y - val.y - num2), text, color, fontSize);
						if (Config.ESP.Boxes.Enabled)
						{
							RenderQueue.Box(screenPoint - new Vector2(0f, val.y / 2f), val, color);
						}
						RenderQueue.String(screenPoint + new Vector2(0f, num2), text2, color, fontSize);
						RenderQueue.String(screenPoint + new Vector2(0f, num2 * 2f), $"{distanceFromPlayer:F1}m", color, fontSize);
					}
				}
			}
		});
	}

	private static void ProcessVBloodCarriers()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!Config.ESP.VBloodCarriers.Enabled && !Config.Aimbot.Bosses.Enabled)
		{
			return;
		}
		EntityList.VBloodCarriers.ForEach(delegate(Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			if (CheckEntity(entity) && entity.TryGetComponent<VBloodConsumeSource>(out VBloodConsumeSource componentData))
			{
				Vector3 position = entity.GetPosition();
				if (GetScreenPoint(position, out var screenPoint))
				{
					float distanceFromPlayer = GetDistanceFromPlayer(position);
					if (Config.Aimbot.Bosses.Enabled)
					{
						Aimbot.TryAddCandidate(entity, screenPoint, distanceFromPlayer, Aimbot.EntityType.Boss);
					}
					if (Config.ESP.VBloodCarriers.Enabled)
					{
						string name = VBloods.GetName(componentData.Source);
						string text = $"HP: {(int)entity.Read<Health>().Value}";
						Color color = ColorOptions.GetColor(Config.ESP.VBloodCarriers.Color);
						float num = 20f / Vector3.Distance(((Component)MainCamera).transform.position, position);
						Vector2 val = ItemsSize.DefaultRectSize * num;
						int fontSize = (int)Mathf.Max(ItemsSize.FontSizeMin, ItemsSize.FontSize * num);
						float num2 = Mathf.Max(ItemsSize.FontDistMin, ItemsSize.FontDist * num);
						RenderQueue.String(new Vector2(screenPoint.x, screenPoint.y - val.y - num2), name, color, fontSize);
						if (Config.ESP.Boxes.Enabled)
						{
							RenderQueue.Box(screenPoint - new Vector2(0f, val.y / 2f), val, color);
						}
						RenderQueue.String(screenPoint + new Vector2(0f, num2), text, color, fontSize);
						RenderQueue.String(screenPoint + new Vector2(0f, num2 * 2f), $"{distanceFromPlayer:F1}m", color, fontSize);
					}
				}
			}
		});
	}

	private static void ProcessMobs()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!Config.ESP.BloodSources.Enabled && !Config.Aimbot.Mobs.Enabled)
		{
			return;
		}
		EntityList.Mobs.ForEach(delegate(Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			if (CheckEntity(entity) && entity.TryGetComponent<BloodConsumeSource>(out BloodConsumeSource componentData))
			{
				Vector3 position = entity.GetPosition();
				if (GetScreenPoint(position, out var screenPoint))
				{
					float distanceFromPlayer = GetDistanceFromPlayer(position);
					if (Config.Aimbot.Mobs.Enabled)
					{
						Aimbot.TryAddCandidate(entity, screenPoint, distanceFromPlayer, Aimbot.EntityType.Mob);
					}
					if (Config.ESP.BloodSources.Enabled && !(componentData.BloodQuality < Config.ESP.BloodSources.MinimumQuality))
					{
						string text = CleanAndFormatName(entity.GetName(), "CHAR_\\w+_");
						PrefabLookupMap prefabLookupMap = PrefabLookupMap;
						string text2 = prefabLookupMap.GetName(componentData.UnitBloodType).Replace("BloodType_", "");
						string text3 = $"{text2} ({(int)componentData.BloodQuality}%)";
						Color color = ColorOptions.GetColor(Config.ESP.BloodSources.Color);
						float num = 20f / Vector3.Distance(((Component)MainCamera).transform.position, position);
						Vector2 val = ItemsSize.DefaultRectSize * num;
						int fontSize = (int)Mathf.Max(ItemsSize.FontSizeMin, ItemsSize.FontSize * num);
						float num2 = Mathf.Max(ItemsSize.FontDistMin, ItemsSize.FontDist * num);
						RenderQueue.String(new Vector2(screenPoint.x, screenPoint.y - val.y - num2), text, color, fontSize);
						if (Config.ESP.Boxes.Enabled)
						{
							RenderQueue.Box(screenPoint - new Vector2(0f, val.y / 2f), val, color);
						}
						RenderQueue.String(screenPoint + new Vector2(0f, num2), text3, color, fontSize);
						RenderQueue.String(screenPoint + new Vector2(0f, num2 * 2f), $"{distanceFromPlayer:F1}m", color, fontSize);
					}
				}
			}
		});
	}

	private static void ProcessGateBosses()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!Config.ESP.GateBosses.Enabled && !Config.Aimbot.Bosses.Enabled)
		{
			return;
		}
		EntityList.GateBosses.ForEach(delegate(Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			if (CheckEntity(entity))
			{
				Vector3 position = entity.GetPosition();
				if (GetScreenPoint(position, out var screenPoint))
				{
					float distanceFromPlayer = GetDistanceFromPlayer(position);
					if (Config.Aimbot.Bosses.Enabled)
					{
						Aimbot.TryAddCandidate(entity, screenPoint, distanceFromPlayer, Aimbot.EntityType.Boss);
					}
					if (Config.ESP.GateBosses.Enabled)
					{
						string text = CleanAndFormatName(entity.GetName(), "CHAR", "VBlood");
						string text2 = $"HP: {(int)entity.Read<Health>().Value}";
						Color color = ColorOptions.GetColor(Config.ESP.GateBosses.Color);
						float num = 20f / Vector3.Distance(((Component)MainCamera).transform.position, position);
						int fontSize = (int)Mathf.Max(ItemsSize.FontSizeMin, ItemsSize.FontSize * num);
						float num2 = Mathf.Max(ItemsSize.FontDistMin, ItemsSize.FontDist * num);
						RenderQueue.String(screenPoint, text, color);
						RenderQueue.String(screenPoint + new Vector2(0f, num2), text2, color, fontSize);
						RenderQueue.String(screenPoint + new Vector2(0f, num2 * 2f), $"{distanceFromPlayer:F1}m", color, fontSize);
					}
				}
			}
		});
	}

	private static void ProcessContainers()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		EntityList.Containers.ForEach(delegate(Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			if (entity.Exists())
			{
				Vector3 position = entity.GetPosition();
				if (GetScreenPoint(position, out var screenPoint))
				{
					string[] patterns = new string[4] { "TM_", "_Full", "[0-9]", "Resource" };
					string text = CleanAndFormatName(entity.GetName(), patterns);
					float distanceFromPlayer = GetDistanceFromPlayer(position);
					Color color = ColorOptions.GetColor(Config.ESP.Containers.Color);
					float num = 20f / Vector3.Distance(((Component)MainCamera).transform.position, position);
					int fontSize = (int)Mathf.Max(ItemsSize.FontSizeMin, ItemsSize.FontSize * num);
					float num2 = Mathf.Max(ItemsSize.FontDistMin, ItemsSize.FontDist * num);
					RenderQueue.String(screenPoint, text, color, fontSize);
					RenderQueue.String(screenPoint + new Vector2(0f, num2), $"{distanceFromPlayer:F1}m", color, fontSize);
				}
			}
		});
	}

	private static void ProcessItems()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		EntityList.Items.ForEach(delegate(Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			if (entity.Exists())
			{
				Vector3 position = entity.GetPosition();
				if (GetScreenPoint(position, out var screenPoint) && entity.TryGetComponent<ItemPickup>(out ItemPickup componentData))
				{
					PrefabLookupMap prefabLookupMap = PrefabLookupMap;
					string text = Items.CleanName(prefabLookupMap.GetName(componentData.ItemId));
					text += ((!text.Contains("Jewel") && componentData.ItemAmount > 1) ? $" ({componentData.ItemAmount})" : "");
					float distanceFromPlayer = GetDistanceFromPlayer(position);
					Color white = Color.white;
					float num = 20f / Vector3.Distance(((Component)MainCamera).transform.position, position);
					int fontSize = (int)Mathf.Max(ItemsSize.FontSizeMin, ItemsSize.FontSize * num);
					float num2 = Mathf.Max(ItemsSize.FontDistMin, ItemsSize.FontDist * num);
					RenderQueue.String(screenPoint, text, white, fontSize);
					RenderQueue.String(screenPoint + new Vector2(0f, num2), $"{distanceFromPlayer:F1}m", white, fontSize);
				}
			}
		});
	}

	private static void ProcessOres()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		EntityList.Ores.ForEach(delegate(Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			if (entity.Exists())
			{
				string name = entity.GetName();
				if (!name.Contains("Rock") && name.Contains("Resource"))
				{
					Vector3 position = entity.GetPosition();
					if (GetScreenPoint(position, out var screenPoint))
					{
						name = CleanAndFormatName(name, "TM", "[0-9]", "Stage", "Resource");
						float distanceFromPlayer = GetDistanceFromPlayer(position);
						Color color = ColorOptions.GetColor(Config.ESP.Ores.Color);
						float num = 20f / Vector3.Distance(((Component)MainCamera).transform.position, position);
						int fontSize = (int)Mathf.Max(ItemsSize.FontSizeMin, ItemsSize.FontSize * num);
						float num2 = Mathf.Max(ItemsSize.FontDistMin, ItemsSize.FontDist * num);
						RenderQueue.String(screenPoint, name, color, fontSize);
						RenderQueue.String(screenPoint + new Vector2(0f, num2), $"{distanceFromPlayer:F1}m", color, fontSize);
					}
				}
			}
		});
	}

	private static void ProcessPlants()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		EntityList.Plants.ForEach(delegate(Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			if (entity.Exists())
			{
				string name = entity.GetName();
				if (name.Contains("Plant") && !name.Contains("fiber") && !name.Contains("Cotton"))
				{
					Vector3 position = entity.GetPosition();
					if (GetScreenPoint(position, out var screenPoint))
					{
						name = CleanAndFormatName(name, "TM", "BP", "Castle", "Chain", "Plant", "[0-9]", "Pickup");
						float distanceFromPlayer = GetDistanceFromPlayer(position);
						Color color = ColorOptions.GetColor(Config.ESP.Plants.Color);
						float num = 20f / Vector3.Distance(((Component)MainCamera).transform.position, position);
						int fontSize = (int)Mathf.Max(ItemsSize.FontSizeMin, ItemsSize.FontSize * num);
						float num2 = Mathf.Max(ItemsSize.FontDistMin, ItemsSize.FontDist * num);
						RenderQueue.String(screenPoint, name, color, fontSize);
						RenderQueue.String(screenPoint + new Vector2(0f, num2), $"{distanceFromPlayer:F1}m", color, fontSize);
					}
				}
			}
		});
	}

	private static void ProcessFishingSpots()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		EntityList.FishingSpots.ForEach(delegate(Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			if (CheckEntity(entity))
			{
				Vector3 position = entity.GetPosition();
				if (GetScreenPoint(position, out var screenPoint))
				{
					float distanceFromPlayer = GetDistanceFromPlayer(position);
					Color color = ColorOptions.GetColor(Config.ESP.FishingSpots.Color);
					float num = 20f / Vector3.Distance(((Component)MainCamera).transform.position, position);
					int fontSize = (int)Mathf.Max(ItemsSize.FontSizeMin, ItemsSize.FontSize * num);
					float num2 = Mathf.Max(ItemsSize.FontDistMin, ItemsSize.FontDist * num);
					RenderQueue.String(screenPoint, "Fishing Spot", color, fontSize);
					RenderQueue.String(screenPoint + new Vector2(0f, num2), $"{distanceFromPlayer:F1}m", color, fontSize);
				}
			}
		});
	}

	private static void ProcessHorses()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		EntityList.Horses.ForEach(delegate(Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			if (CheckEntity(entity) && entity.TryGetComponent<Mountable>(out Mountable componentData))
			{
				float num = Config.ESP.Horses.MinimumQuality / 100f;
				bool num2 = componentData.Acceleration >= 7f * num && componentData.MaxSpeed >= 11f * num && componentData.RotationSpeed / 10f >= 14f * num;
				Vector3 position = entity.GetPosition();
				if (num2 && GetScreenPoint(position, out var screenPoint))
				{
					string text = CleanAndFormatName(entity.GetName(), "CHAR_", "Mount_");
					string text2 = $"S: {componentData.MaxSpeed} | A: {componentData.Acceleration} | R: {componentData.RotationSpeed / 10f}";
					float distanceFromPlayer = GetDistanceFromPlayer(position);
					Color color = ColorOptions.GetColor(Config.ESP.Horses.Color);
					float num3 = 20f / Vector3.Distance(((Component)MainCamera).transform.position, position);
					int fontSize = (int)Mathf.Max(ItemsSize.FontSizeMin, ItemsSize.FontSize * num3);
					float num4 = Mathf.Max(ItemsSize.FontDistMin, ItemsSize.FontDist * num3);
					RenderQueue.String(screenPoint, text, color, fontSize);
					RenderQueue.String(screenPoint + new Vector2(0f, num4), text2, color, fontSize);
					RenderQueue.String(screenPoint + new Vector2(0f, num4 * 2f), $"{distanceFromPlayer:F1}m", color, fontSize);
				}
			}
		});
	}

	private static void ProcessServants()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		EntityList.Servants.ForEach(delegate(Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			if (CheckEntity(entity) && entity.TryGetComponent<ServantPower>(out ServantPower componentData))
			{
				Vector3 position = entity.GetPosition();
				if (GetScreenPoint(position, out var screenPoint))
				{
					string text = $"[{componentData.GearLevel}] Servant";
					string text2 = $"Expertise: {componentData.Expertise * 100f:F0}% | Power: {componentData.Power:F1}";
					float distanceFromPlayer = GetDistanceFromPlayer(position);
					Color color = ColorOptions.GetColor(Config.ESP.Servants.Color);
					float num = 20f / Vector3.Distance(((Component)MainCamera).transform.position, position);
					Vector2 val = ItemsSize.DefaultRectSize * num;
					int fontSize = (int)Mathf.Max(ItemsSize.FontSizeMin, ItemsSize.FontSize * num);
					float num2 = Mathf.Max(ItemsSize.FontDistMin, ItemsSize.FontDist * num);
					RenderQueue.String(new Vector2(screenPoint.x, screenPoint.y - val.y - num2), text, color, fontSize);
					if (Config.ESP.Boxes.Enabled)
					{
						RenderQueue.Box(screenPoint - new Vector2(0f, val.y / 2f), val, color);
					}
					RenderQueue.String(screenPoint + new Vector2(0f, num2), text2, color, fontSize);
					RenderQueue.String(screenPoint + new Vector2(0f, num2 * 2f), $"{distanceFromPlayer:F1}m", color, fontSize);
				}
			}
		});
	}

	private static void ProcessCarriages()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		EntityList.Carriages.ForEach(delegate(Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			if (CheckEntity(entity))
			{
				Vector3 position = entity.GetPosition();
				if (GetScreenPoint(position, out var screenPoint))
				{
					string text = CleanAndFormatName(entity.GetName(), "CHAR_");
					float distanceFromPlayer = GetDistanceFromPlayer(position);
					Color color = ColorOptions.GetColor(Config.ESP.Carriages.Color);
					float num = 20f / Vector3.Distance(((Component)MainCamera).transform.position, position);
					int fontSize = (int)Mathf.Max(ItemsSize.FontSizeMin, ItemsSize.FontSize * num);
					float num2 = Mathf.Max(ItemsSize.FontDistMin, ItemsSize.FontDist * num);
					RenderQueue.String(screenPoint, text, color, fontSize);
					RenderQueue.String(screenPoint + new Vector2(0f, num2), $"{distanceFromPlayer:F1}m", color, fontSize);
				}
			}
		});
	}

	private static string CleanAndFormatName(string input, params string[] patterns)
	{
		string text = Enumerable.Aggregate<string, string>((System.Collections.Generic.IEnumerable<string>)Enumerable.ToArray<string>(Enumerable.Concat<string>((System.Collections.Generic.IEnumerable<string>)patterns, (System.Collections.Generic.IEnumerable<string>)new string[] { "_" })), input, (Func<string, string, string>)((string current, string pattern) => Regex.Replace(current, pattern, "")));
		return CamelCaseRegex.Replace(text, " $1");
	}

	private static bool CheckEntity(Entity entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Exists() && !entity.IsDisabled() && entity.IsAlive())
		{
			return entity.Has<LocalToWorld>();
		}
		return false;
	}

	private static float GetDistanceFromPlayer(Vector3 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Distance(EntityList.LocalPlayer.GetPosition(), position);
	}

	internal static bool GetScreenPoint(Vector3 position, out Vector2 screenPoint)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = MainCamera.WorldToScreenPoint(position);
		screenPoint = new Vector2(val.x, (float)Screen.height - val.y + 10f);
		Rect val2 = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
		if (val.z > 0f)
		{
			return val2.Contains(screenPoint);
		}
		return false;
	}
}
