using BDFramework.Mgr;


public class UIWidgetAttribute : ManagerAtrribute
{
    public string ResourcePath { get; private set; }

    public UIWidgetAttribute(int tag, string resPath) : base(tag.ToString())
    {
        this.ResourcePath = resPath;
    }
}