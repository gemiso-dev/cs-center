using Prism.Mvvm;
using System.Windows.Forms;
using System.IO;
using System.Windows.Input;
using Prism.Commands;
using sequence_maker.Services;
using System;
using System.Threading.Tasks;

namespace sequence_maker.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private FileStream strIn = null;
        private FileStream strOut = null;
        private FileStream strAlreadyFile = null;
        private readonly ILogManager _logManager;

        #region Variable
        private string TargetOnlyPath { get; set; }
        #endregion

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

        private int _countNumber;
        public int CountNumber
        {
            get { return _countNumber; }
            set { SetProperty(ref _countNumber, value); }
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

        public MainWindowViewModel(ILogManager logManager)
        {
            _logManager = logManager;

            FindSourceDirCommand = new DelegateCommand(FindSourceDirCmd);
            FindTargetDirCommand = new DelegateCommand(FindTargetDirCmd);
            CopyCommand = new DelegateCommand(CopyCmd);

            _logManager.Logger.Info("sequence maker start");
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
                //TargetName = Path.GetFileName(fileDialog.SafeFileName);
            }
            _logManager.Logger.Info($"Source directory : {SourceDir}");
            TargetDir = string.Empty;
        }
        private void FindTargetDirCmd()
        {

            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(SourceDir))
                {
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        TargetDir = Path.Combine(folderDialog.SelectedPath, Path.GetFileName(SourceDir));
                    }
                    else
                    {
                        TargetDir = string.Empty;
                    }
                    _logManager.Logger.Info($"Target directory : {TargetDir}");
                }
                else
                {
                    _logManager.Logger.Error("Source directory is empty. Find source directory first.");
                }
            }
        }

        private async void CopyCmd()
        {
            if (string.IsNullOrEmpty(SourceDir) || string.IsNullOrEmpty(TargetDir))
            {
                return;
            }

            /*if (!File.Exists(TargetDir))
            {
            }
            else
            {
                // 타켓 이름이 이미 존재할 때
                _logManager.Logger.Info($"{TargetDir} is already exist.");
                return;
            }*/

            await Task.Run(() =>
            {
                long bufferSize = 65536;
                byte[] buf = new byte[bufferSize];

                FileInfo file = new FileInfo(SourceDir);

                // 복사 완료된 파일 용량
                long currentSize = 0;
                // 복사할 파일 용량 * Count = 복사할 전체 용량
                long totalSize = file.Length * CountNumber;

                // countnumber 자릿수
                int CountLength = (int)(Math.Log10(CountNumber) + 1);

                string OnlyFileName = Path.GetFileNameWithoutExtension(TargetDir);
                string OnlyExtention = Path.GetExtension(TargetDir);

                // i 번째 파일 전체 용량
                long iTotalSize = file.Length;
                string tempTargetDir = Path.GetDirectoryName(TargetDir);

                try
                {
                    for (int i = 1; i < CountNumber + 1; i++)
                    {
                        // countnumber로 파일명 재정의
                        string TargetFileName = OnlyFileName + "_" + string.Format("{0:D" + CountLength + "}", i) + OnlyExtention;
                        TargetDir = Path.Combine(tempTargetDir, TargetFileName);

                        // i 번째 copy
                        long iSize = 0; // i 번째 파일 복사 완료된 용량

                        // 복사 파일 이름 존재 시 건너 뛰기
                        if (File.Exists(TargetDir))
                        {
                            strAlreadyFile = new FileStream(TargetDir, FileMode.Open);
                            int AlreadyFileSize = strAlreadyFile.Read(buf, 0, buf.Length);

                            // 이미 존재하는 파일의 용량을 읽어 진행률에 표시
                            iSize += AlreadyFileSize;

                            currentSize += AlreadyFileSize;
                            TotalProgress = (currentSize * 100) / totalSize;

                            strAlreadyFile.Close();

                            continue;
                        }

                        strIn = new FileStream(SourceDir, FileMode.Open);
                        strOut = new FileStream(TargetDir, FileMode.Create);

                        

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
                        _logManager.Logger.Info($"Copy success {i}times");
                    }
                }
                catch (Exception ex)
                {
                    _logManager.Logger.Info($"Copy failed, Error : {ex.Message} ");
                    return;
                }
                _logManager.Logger.Info($"Copy success");

            });
        #endregion
        }
    }
}
