using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace Cloaks
{
	public class NoLongLines : VisualLineElementGenerator
	{
		public override int GetFirstInterestedOffset(int startOffset)
		{
			DocumentLine lastDocumentLine = base.CurrentContext.VisualLine.LastDocumentLine;
			bool flag = lastDocumentLine.Length > 2000;
			if (flag)
			{
				int num = lastDocumentLine.Offset + 2000 - 100 - "< Expand >".Length;
				bool flag2 = startOffset <= num;
				if (flag2)
				{
					return num;
				}
			}
			return -1;
		}

		public override VisualLineElement ConstructElement(int offset)
		{
			return new FormattedTextElement("< Expand >", base.CurrentContext.VisualLine.LastDocumentLine.EndOffset - offset - 100);
		}

		private const int maxLength = 2000;

		private const string ellipsis = "< Expand >";

		private const int charactersAfterEllipsis = 100;
	}
}
