namespace Turbo.Plugins.Razor.Click
{
	using Gma.System.MouseKeyHook;
	using Turbo.Plugins.Default;

	public static class ClickExtensions
	{
		private static IKeyboardMouseEvents Hook;
		private static GlobalHookThread Thread;
				
		public static GlobalHookThread GetThread(this IInputController input)
		{
			if (Thread == null)
				Thread = new GlobalHookThread();
		
			return Thread;
		}
	}
}