using Terraria.ModLoader;

namespace WireSharkRuntime;

public class WireSharkSystem : ModSystem
{
    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        DLLManager.PostUpdate();
    }
}