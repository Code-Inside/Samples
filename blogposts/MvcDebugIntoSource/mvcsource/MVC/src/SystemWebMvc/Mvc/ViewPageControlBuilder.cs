namespace System.Web.Mvc {
    using System.CodeDom;
    using System.Web.UI;

    internal sealed class ViewPageControlBuilder : FileLevelPageControlBuilder {
        public string PageBaseType {
            get;
            set;
        }

        public override void ProcessGeneratedCode(
            CodeCompileUnit codeCompileUnit,
            CodeTypeDeclaration baseType,
            CodeTypeDeclaration derivedType,
            CodeMemberMethod buildMethod,
            CodeMemberMethod dataBindingMethod) {

            // If we find got a base class string, use it
            if (PageBaseType != null) {
                derivedType.BaseTypes[0] = new CodeTypeReference(PageBaseType);
            }
        }
    }
}
