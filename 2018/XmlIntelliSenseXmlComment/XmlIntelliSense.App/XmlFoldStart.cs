using ICSharpCode.AvalonEdit.Folding;

namespace XmlIntelliSense.App
{
    /// <summary>
    /// Holds information about the start of a fold in an xml string.
    /// </summary>
    sealed class XmlFoldStart : NewFolding
    {
        internal int StartLine;
    }
}