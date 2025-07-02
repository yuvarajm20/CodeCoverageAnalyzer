using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using System.Linq;
using CodeCoverageAnalyzer.Model;
using Microsoft.Win32;
using System.Xml.Linq;

namespace CodeCoverageAnalyzer.ViewModel
{

    public class MainViewModel : ObservableObject
    {
        public ObservableCollection<CoverageNode> CoverageTree { get; } = new();

        private CoverageFile? _selectedFile;
        public CoverageFile? SelectedFile
        {
            get => _selectedFile;
            set { _selectedFile = value; OnPropertyChanged(); }
        }

        private string _coverageTitle = "Code Coverage Analyzer";
        public string CoverageTitle
        {
            get => _coverageTitle;
            set { _coverageTitle = value; OnPropertyChanged(); }
        }

        public ICommand OpenCommand { get; }
        public ICommand CloseCommand { get; }

        public MainViewModel()
        {
            OpenCommand = new RelayCommand(OpenFileDialog);
            CloseCommand = new RelayCommand(CloseCoverageData);
        }

       

        private void OpenFileDialog()
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Coverage XML (*.xml)|*.xml|All files (*.*)|*.*"
            };
            if (ofd.ShowDialog() == true)
            {
                LoadCoverageData(ofd.FileName);
            }
        }

        private void CloseCoverageData()
        {
            CoverageTree.Clear();
            SelectedFile = null;             
            CoverageTitle = "Code Coverage Analyzer";
        }

        private void LoadCoverageData(string xmlPath)
        {
            var doc = XDocument.Load(xmlPath);

            // Get all source_files
            var files = doc.Descendants("source_file")
                .ToDictionary(
                    x => x.Attribute("id")!.Value,
                    x => x.Attribute("path")!.Value);

            var seqPointsByFile = new Dictionary<string, List<SequencePoint>>();

            foreach (var range in doc.Descendants("range"))
            {
                var sourceId = range.Attribute("source_id")!.Value;
                var filePath = files[sourceId];
                var visitCount = range.Attribute("covered")!.Value == "yes" ? 1 : 0;
                var line = int.Parse(range.Attribute("start_line")!.Value);

                if (!seqPointsByFile.ContainsKey(filePath))
                    seqPointsByFile[filePath] = new List<SequencePoint>();

                seqPointsByFile[filePath].Add(new SequencePoint { Line = line, VisitCount = visitCount });
            }

            // Build the hierarchical tree
            CoverageTree.Clear();

            foreach (var kvp in seqPointsByFile)
            {
                var filePath = kvp.Key;
                var fileModel = new CoverageFile { FullPath = filePath, SequencePoints = kvp.Value };
                var parts = filePath.Split(Path.DirectorySeparatorChar);

                var currentLevel = CoverageTree;
                CoverageNode? parentNode = null;

                foreach (var part in parts)
                {
                    var existing = currentLevel.FirstOrDefault(n => n.Name == part);
                    if (existing == null)
                    {
                        existing = new CoverageNode { Name = part };
                        currentLevel.Add(existing);
                    }
                    parentNode = existing;
                    currentLevel = existing.Children;
                }

                parentNode!.File = fileModel;
            }

            int covered = seqPointsByFile.Values.Sum(f => f.Count(sp => sp.VisitCount > 0));
            int total = seqPointsByFile.Values.Sum(f => f.Count);
            CoverageTitle = $"Code Coverage Analyzer: {covered}/{total} blocks covered: {(double)covered / total:P2}";
        }      

    }
}
