using System.Collections.Generic;
using System.Windows.Documents;

namespace LogChecker
{
    public partial class Conclusion
    {
        public Conclusion(IEnumerable<Run> _)
        {
            InitializeComponent();

            var FlowDocument = new FlowDocument();

            Paragraph Paragraph = new Paragraph();

            foreach (Run X in _)
            {
                Paragraph.Inlines.Add(X);
            }

            FlowDocument.Blocks.Add(Paragraph);

            RichTextBox.Document = FlowDocument;
        }
    }
}
