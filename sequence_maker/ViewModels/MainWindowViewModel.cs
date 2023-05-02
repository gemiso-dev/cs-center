
using Prism.Mvvm;
using System.Windows.Forms;
using System.IO;
using System.Windows.Input;
using Prism.Commands;

namespace sequence_maker.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private FileStream strIn = null;
        private FileStream strOut = null;

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

        private int _copyCount;
        public int CopyCount
        {
            get { return _copyCount; }
            set { SetProperty(ref _copyCount, value); }
        }

        private long _totalProgress;
        public long TotalProgress
        {
            get { return _totalProgress; }
            set { SetProperty(ref _totalProgress, value); }
        }
        #endregion

        #region Command
        public ICommand FindSourceDirCommand { get; set; }
        public ICommand FindTargetDirCommand { get; set; }
        public ICommand CopyCommand { get; set; }
        #endregion

        public MainWindowViewModel()
        {
            FindSourceDirCommand = new DelegateCommand(FindSourceDirCmd);
            FindTargetDirCommand = new DelegateCommand(FindTargetDirCmd);
            CopyCommand = new DelegateCommand(CopyCmd);
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
        private void CopyCmd()
        {
            if (!File.Exists(TargetDir))
            {
            }
            else
            {
                // 타켓 이름이 이미 존재할 때
            }

            long bufferSize = 65536;
            byte[] buf = new byte[bufferSize];

            FileInfo file = new FileInfo(SourceDir);

            // 복사 완료된 파일 용량
            long currentSize = 0;
            // 복사할 파일 용량 * Count = 복사할 전체 용량
            long totalSize = file.Length * CopyCount;

            for(int i = 0; i < CopyCount; i++)
            {
                strIn = new FileStream(SourceDir, FileMode.Open);
                strOut = new FileStream(TargetDir, FileMode.Create);

                // i 번째 copy
                int iSize = 0; // i 번째 파일 복사 완료된 용량
                int iTotalSize = 0; // i 번째 파일 전체 용량

                while (iSize < iTotalSize)
                {
                    // 복사
                    int len = strIn.Read(buf, 0, buf.Length);
                    strOut.Write(buf, 0, len);

                    iSize += len;

                    // 진행률
                    currentSize += len;
                    TotalProgress = (currentSize * 100) / totalSize;
                }

                // stream close
                strOut.Flush();
                strIn.Close();
                strOut.Close();
            }
        }
        #endregion
    }
}
