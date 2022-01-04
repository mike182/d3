/*

Borrowed from Hans Passant
https://stackoverflow.com/questions/21680738/how-to-post-messages-to-an-sta-thread-running-a-message-pump/21684059#21684059
A message pumping thread for running global mouse and keyboard hooks library

*/

namespace Turbo.Plugins.Razor.Click
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Windows.Forms;
	using Gma.System.MouseKeyHook;

	public class GlobalHookThread : IDisposable //STAThread
	{
		public IKeyboardMouseEvents GlobalHook { get; private set; }
		//public Action<IKeyboardMouseEvents> Init { get; set; }
		
		public GlobalHookThread(Action<GlobalHookThread> init = null) //STAThread
		{
			//if (init is object)
			//	Init = init;
			
			using (mre = new ManualResetEvent(false)) {
				thread = new Thread(() => {
					GlobalHook = Hook.GlobalEvents();
					if (init is object)
						init(this);
					
					Application.Idle += Initialize;
					Application.Run();
				});
				thread.IsBackground = true;
				thread.Priority = ThreadPriority.AboveNormal; //Highest;
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
				mre.WaitOne();
			}
		}
		
		public void BeginInvoke(Delegate dlg, params Object[] args)
		{
			if (ctx == null) throw new ObjectDisposedException("STAThread");
			ctx.Post((_) => dlg.DynamicInvoke(args), null);
		}
		public object Invoke(Delegate dlg, params Object[] args)
		{
			if (ctx == null) throw new ObjectDisposedException("STAThread");
			object result = null;
			ctx.Send((_) => result = dlg.DynamicInvoke(args), null);
			return result;
		}
		protected virtual void Initialize(object sender, EventArgs e)
		{
			ctx = SynchronizationContext.Current;
			mre.Set();
			Application.Idle -= Initialize;
		}
		public void Dispose()
		{
			if (GlobalHook is object)
				GlobalHook.Dispose();
			
			if (ctx != null) {
				ctx.Send((_) => Application.ExitThread(), null);
				ctx = null;
			}
		}
		private Thread thread;
		private SynchronizationContext ctx;
		private ManualResetEvent mre;
	}
}