using System.ComponentModel.Design;

namespace StylableWinFormsControls.LayoutInternals;

internal class TabpageExCollectionEditor : CollectionEditor
{
    public TabpageExCollectionEditor(Type type) : base(type)
    {
    }

    protected override Type CreateCollectionItemType()
    {
        return typeof(TabPage);
    }
}