/*



*/

namespace Turbo.Plugins.Razor.Label
{
	using System;
	using System.Collections.Generic;
	//using Gma.System.MouseKeyHook;
	//using Indieteur.GlobalHooks;

	using Turbo.Plugins.Default;
	using Turbo.Plugins.Razor.Util;

	public static class LabelDecorator
	{
		public static IBrush DebugBrush { get; set; }
		public static IBrush DebugBrush2 { get; set; }
		public static IBrush DebugBrush3 { get; set; }
		public static IFont DebugFont { get; set; }
		//public static IFont TooltipFont { get; set; }
		public static UIOverlapHelper Overlay { get; set; }
		//public static ILabelDecorator HoveredLabel { get; set; } = null;
		//private static LabelTooltip Tooltip = null;
		public static LabelController Controller { get; set; }
		//public static IKeyboardMouseEvents GlobalHook { get; set; }
		//public static GlobalMouseHook MouseHook { get; set; }
		


		/*public static void Draw(ILabelDecorator label, float x, float y) //this IRenderController Render,
		{
			if (IsVisible(label))
				label.Paint(x, y, DebugBrush); //recursive
			
			if (label.Hovered)
			{
				HoveredLabel = label;
				SetHint(label);
			}
		}*/
		
		//provide a label from which to find ILabelDecorator.Hint and ILabelDecorator.OnClick
		public static void SetHint(ILabelDecorator label) //this IRenderController Render,
		{
			if (label is object && Controller is object)
			{
				//if (Controller == null)
				//	Controller = label.Hud.GetPlugin<LabelController>();
				Controller.HoveredLabel = label;
			
				var hint = Controller.FindHint(label);
				if (hint is object)
					Controller.HintLabel = hint;
				
				var handler = Controller.FindClickHandler(label);
				if (handler is object)
					Controller.ClickLabel = handler;
				
				/*if (Tooltip == null)
					Tooltip = label.Hud.GetPlugin<LabelTooltip>();
				
				Tooltip.RequestDraw(label);*/
			}
		}
		
		//set Controller.HintLabel directly
		public static void SetHintLabel(ILabelDecorator label)
		{
			if (label is object)
				Controller.HintLabel = label;
		}
		
		public static bool IsVisible(ILabelDecorator label)
		{
			if (label == null)
				return false;
			
			if (!label.Enabled)
				return false;
			
			//if (!label.Visible)
			//	return false;
			
			if (label.Width != 0 && label.Height != 0)
				return true;
			
			label.Resize();
			
			return label.Width != 0 && label.Height != 0;
		}
		
		public static bool IsObstructed(ILabelDecorator label)
		{
			return Overlay is object && Overlay.IsUiObstructingArea(new System.Drawing.RectangleF(label.LastX, label.LastY, label.Width, label.Height), UIOverlapHelper.UIGroup.Prompt, UIOverlapHelper.UIGroup.Clip, UIOverlapHelper.UIGroup.Mail);
		}
		
		public static void DebugWrite(string text, float x, float y)
		{
			if (DebugFont is object && !string.IsNullOrEmpty(text))
				DebugFont.DrawText(DebugFont.GetTextLayout(text), x, y);
		}
	}
}