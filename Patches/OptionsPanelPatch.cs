namespace ExtrasensoryPerception.Patches;

// [HarmonyPatch(typeof(OptionsPanel_Interface), nameof(OptionsPanel_Interface.Start))]
// public class OptionsPanelPatch
// {
//     [HarmonyPostfix]
// 	private static void Postfix(OptionsPanel_Interface __instance)
// 	{
// 		var header = new Header()
// 			.Panel(__instance)
// 			.Label("ESP")
// 			.Build();
// 		
// 		var enableMod = new Toggle()
// 			.Panel(__instance)
// 			.Label("Enabled")
// 			.Tooltip("<align=\"center\">Enable or disable the mod.</align>")
// 			.DefaultValue(false)
// 			.InitialValue(Config.ModToggle.Value)
// 			.OnValueChanged(OnChange(Config.ModToggle))
// 			.Build();
// 		var drawBoxes = new Toggle()
// 			.Panel(__instance)
// 			.Label("Draw boxes")
// 			.Tooltip("<align=\"center\">Enable or disable boxes drawing.</align>")
// 			.DefaultValue(false)
// 			.InitialValue(Config.DrawBoxes.Value)
// 			.OnValueChanged(OnChange(Config.DrawBoxes))
// 			.Build();
// 		var refreshInterval = new Slider()
// 			.Panel(__instance)
// 			.Label("Refresh Interval")
// 			.MinValue(0.1f)
// 			.MaxValue(5.0f)
// 			.InitialValue(Config.RefreshInterval.Value)
// 			.DefaultValue(3.0f)
// 			.OnValueChanged(OnChange(Config.RefreshInterval))
// 			.Build();
// 		
// 	    var entityConfigs = new Dictionary<string, (Config.EntityConfig Config, bool HasQuality)>
// 	    {
// 	        { "Players", (Config.Players, false) },
// 	        { "VBlood Carriers", (Config.VBloodCarriers, false) },
// 	        { "Blood Source", (Config.Mobs, true) },
// 	        { "GateBosses", (Config.GateBosses, false) },
// 	        { "Containers", (Config.Containers, false) },
// 	        { "Ores", (Config.Ores, false) },
// 	        { "Horses", (Config.Horses, true) },
// 	        { "Servants", (Config.Servants, false) },
// 	        { "Carriages", (Config.Carriages, false) }
// 	    };
// 	    
// 	    foreach (var (name, value) in entityConfigs)
// 	    {
// 		    var config = value.Config;
// 	        bool hasQuality = value.HasQuality;
// 	
// 	        CreateDivider(__instance.ContentNode, name);
// 	        
// 	        var enabled = new Toggle()
// 	            .Panel(__instance)
// 	            .Label("Enabled")
// 	            .Tooltip($"<align=\"center\">Toggle for {name.ToLower()}.</align>")
// 	            .DefaultValue(false)
// 	            .InitialValue(config.Enabled)
// 	            .OnValueChanged(OnChange(config.StatusEntry))
// 	            .Build();
// 	        
// 	        var color = new Dropdown()
// 	            .Panel(__instance)
// 	            .Options(ColorOptions.AllColors)
// 	            .Label("Color")
// 	            .Tooltip($"<align=\"center\">Select {name.ToLower()} color.</align>")
// 	            .DefaultValue(config.Color)
// 	            .InitialValue(config.Color)
// 	            .OnValueChanged(OnChange(config.ColorEntry))
// 	            .Build();
// 	        
// 	        var maxDistance = new Slider()
// 	            .Panel(__instance)
// 	            .Label("Max distance")
// 	            .MinValue(0f)
// 	            .MaxValue(100f)
// 	            .InitialValue(config.MaxDistance)
// 	            .DefaultValue(100f)
// 	            .OnValueChanged(OnChange(config.DistanceEntry))
// 	            .WholeNumbers()
// 	            .Build();
// 	        
// 	        if (hasQuality)
// 	        {
// 	            var minimumQuality = new Slider()
// 	                .Panel(__instance)
// 	                .Label("Minimum quality")
// 	                .MinValue(0f)
// 	                .MaxValue(100f)
// 	                .InitialValue(config.MinimumQuality)
// 	                .DefaultValue(90f)
// 	                .OnValueChanged(OnChange(config.QualityEntry!))
// 	                .WholeNumbers()
// 	                .Build();
// 	        }
// 	    }
// 	}
// 	
// 	static Action<T> OnChange<T>(ConfigEntry<T> option)
// 	{
// 		return value => option.Value = value;
// 	}
// 	
// 	static void CreateDivider(Transform parent, string dividerText)
// 	{
// 		GameObject dividerGameObject = new("Divider");
//
// 		RectTransform dividerTransform = dividerGameObject.AddComponent<RectTransform>();
// 		dividerTransform.SetParent(parent);
// 		dividerTransform.localScale = Vector3.one;
// 		dividerTransform.sizeDelta = new(0f, 28f);
//
// 		Image dividerImage = dividerGameObject.AddComponent<Image>();
// 		dividerImage.color = new(0.12f, 0.152f, 0.2f, 0.15f);
//
// 		LayoutElement dividerLayout = dividerGameObject.AddComponent<LayoutElement>();
// 		dividerLayout.preferredHeight = 28f;
//
// 		GameObject dividerTextGameObject = new("Text");
// 		RectTransform dividerTextTransform = dividerTextGameObject.AddComponent<RectTransform>();
// 		dividerTextTransform.SetParent(dividerGameObject.transform);
// 		dividerTextTransform.localScale = Vector3.one;
//
// 		TextMeshProUGUI textMeshDivider = dividerTextGameObject.AddComponent<TextMeshProUGUI>();
// 		Il2CppArrayBase<TextMeshProUGUI> textMeshArray = parent.GetComponentsInChildren<TextMeshProUGUI>();
// 		TMP_FontAsset fontAsset = textMeshArray.First().font;
//
// 		textMeshDivider.alignment = TextAlignmentOptions.Center;
// 		textMeshDivider.fontStyle = FontStyles.SmallCaps;
// 		textMeshDivider.font = fontAsset;
// 		textMeshDivider.fontSize = 20f;
// 		textMeshDivider.SetText(dividerText);
//
// 		dividerGameObject.SetActive(true);
// 	}
// }