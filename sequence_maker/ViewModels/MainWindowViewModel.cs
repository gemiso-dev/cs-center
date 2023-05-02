
using Prism.Mvvm;
using System.Windows.Forms;
using System.IO;
using System.Windows.Input;
using Prism.Commands;

namespace sequence_maker.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Bind
        private string _sourceDir;
        public string SourceDir
        {
            get { return _sourceDir; }
            set { SetProperty(ref _sourceDir, value); }
        }

        private string _targetDir;
        public string TargetDir
        {
            get { return _targetDir; }
            set { SetProperty(ref _targetDir, value); }
        }

        private string _targetName;
        public string TargetName
        {
            get { return _targetName; }
            set { SetProperty(ref _targetName, value); }
        }
        #endregion

        #region Command
        public ICommand FindSourceDirCommand { get; set; }
        public ICommand FindTargetDirCommand { get; set; }
        #endregion

        public MainWindowViewModel()
        {
            FindSourceDirCommand = new DelegateCommand(FindSourceDirCmd);
            FindTargetDirCommand = new DelegateCommand(FindTargetDirCmd);
        }

        #region Cmd
        private void FindSourceDirCmd()
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.InitialDirectory = "C:\\";
                //fileDialog.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    SourceDir = fileDialog.FileName;
                }
                else
                {
                    SourceDir = string.Empty;
                }
                TargetName = Path.GetFileName(fileDialog.SafeFileName);
            }
            //logManager._loggerManage.Log(LogLevel.Error, ($""));
            TargetDir = string.Empty;
        }
        private void FindTargetDirCmd()
        {
            string targetOnlyPath;
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(TargetName))
                {
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        targetOnlyPath = folderDialog.SelectedPath;
                    }
                    else
                    {
                        targetOnlyPath = string.Empty;
                    }
                    TargetDir = Path.Combine(targetOnlyPath, TargetName);
                }
                else
                {
                    //logManager._loggerManage.Log(LogLevel.Error, ($""));
                }
            }
        }
        #endregion
    }
}
