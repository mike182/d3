/*

If a plugin needs to prevent left clicks for a certain condition, specify it in this handler's property. 
It should be a lightweight function for timely evaluation.

*/

namespace Turbo.Plugins.Razor.Click
{
	public interface ILeftBlockHandler : IPlugin
	{
		System.Func<bool> LeftClickBlockCondition { get; }
	}
}