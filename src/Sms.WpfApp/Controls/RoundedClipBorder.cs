using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sms.WpfApp.Controls;

/// <summary>
/// Бордер, скрывающий все выступающие части дочерних контролов
/// </summary>
public sealed class RoundedClipBorder : Border
{
    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        if (Child is not null)
        {
            var radius = Math.Max(0, CornerRadius.TopLeft - BorderThickness.Left);
            Child.Clip = new RectangleGeometry(new Rect(Child.RenderSize), radius, radius);
        }

        return size;
    }
}
