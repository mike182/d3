namespace Turbo.Plugins.Razor.Label
{
	public interface ILabelDecoratorCollection
	{
		System.Collections.Generic.List<ILabelDecorator> Labels { get; set; }
		ILabelDecorator HoveredLabel { get; }
	}
}