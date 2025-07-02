using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows;
using System.IO;
using CodeCoverageAnalyzer.Model;
using System.Windows.Media;

namespace CodeCoverageAnalyzer.ViewModel
{

    public class CoverageFileToDocumentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var file = value as CoverageFile;
            var doc = new FlowDocument();

            if (file == null || string.IsNullOrWhiteSpace(file.FullPath))
            {
                doc.Blocks.Add(new Paragraph(new Run("No file or source code found")));
                return doc;
            }

            var coveredLines = new HashSet<int>(file.SequencePoints
                                                   .Where(sp => sp.VisitCount > 0)
                                                   .Select(sp => sp.Line));
            var uncoveredLines = new HashSet<int>(file.SequencePoints
                                                     .Where(sp => sp.VisitCount == 0)
                                                     .Select(sp => sp.Line));

            var allLines = File.ReadAllLines(file.FullPath);
            for (int i = 0; i < allLines.Length; i++)
            {
                var lineNum = i + 1;
                var para = new Paragraph(new Run(allLines[i]));

                if (coveredLines.Contains(lineNum))
                    para.Background = Brushes.LightBlue;
                else if (uncoveredLines.Contains(lineNum))
                    para.Background = Brushes.Orange;

                doc.Blocks.Add(para);
            }

            return doc;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null!;
    }
}
