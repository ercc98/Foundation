using UnityEngine;

namespace ErccDev.Foundation.Rendering
{
    public interface IBillboardRenderer
    {
        BillboardMode Mode { get; set; }
        Camera TargetCamera { get; set; }
    }
}
