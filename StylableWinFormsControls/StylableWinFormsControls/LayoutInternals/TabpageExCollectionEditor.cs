using System.ComponentModel.Design;

namespace StylableWinFormsControls.LayoutInternals;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Roslynator",
    "CA1812: X is an internal class that is apparently never instantiated. " +
        "If so, remove the code from the assembly. If this class is intended to contain only static members, make it 'static' (Module in Visual Basic).",
    Justification = "Is used in annotation.")]
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