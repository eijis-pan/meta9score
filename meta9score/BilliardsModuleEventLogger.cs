using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace meta9score
{

    delegate bool ParseMethod(string logText, int appendStartPos);

    public partial class BilliardsModuleEventLogger : Form
    {
        public string vrcLogDirFromHome = "VRChat\\vrchat"; // C:\Users\[user]\AppData\LocalLow

        public string billiardsLogKeyword = "[BilliardsModule]";

        public event EventHandler? OnLogReceived;

        // local only
        private const string STR_startingGame = "starting game";
        public event EventHandler? OnStartingGameReceived;

        private const string STR_gameState = "game state is";
        public event EventHandler? OnGameStateReceived;

        private const string STR_gameReset = "game reset";
        public event EventHandler? OnGameResetReceived;

        private const string STR_onRemoteLobbyOpened = "onRemoteLobbyOpened";
        public event EventHandler? OnRemoteLobbyOpened;

        private const string STR_onRemoteGameSettingsUpdated = "onRemoteGameSettingsUpdated";
        public event EventHandler? OnRemoteGameSettingsUpdated;

        private const string STR_onRemotePlayersChanged = "onRemotePlayersChanged";
        public event EventHandler? OnRemotePlayersChanged;

        // remote only
        private const string STR_onRemoteGameStarted = "onRemoteGameStarted";
        public event EventHandler? OnRemoteGameStarted;

        private const string STR_onRemoteTurnSimulate = "onRemoteTurnSimulate";
        public event EventHandler? OnRemoteTurnSimulate;

        private const string STR_onRemoteBallsPocketedChanged = "onRemoteBallsPocketedChanged";
        public event EventHandler? OnRemoteBallsPocketedChanged;

        private const string STR_onRemoteRepositionStateChanged = "onRemoteRepositionStateChanged";
        public event EventHandler? OnRemoteRepositionStateChanged;

        private const string STR_onRemoteGameEnded = "onRemoteGameEndedd";
        public event EventHandler? OnRemoteGameEnded;

        string? targetFileName = null;
        long lastReadPosition = 0;

        private ParseMethod[] parseMethods;

        public BilliardsModuleEventLogger()
        {
            InitializeComponent();

            parseMethods = new ParseMethod[] {
                parse_gameState,
                parse_onRemoteTurnSimulate,
                parse_onRemoteRepositionStateChanged,
                parse_onRemoteGameSettingsUpdated,
                parse_onRemotePlayersChanged,
                parse_onRemoteBallsPocketedChanged,
                parse_onRemoteLobbyOpened,
                parse_startingGame,
                parse_OnRemoteGameStarted,
                parse_onRemoteGameEnded,
                parse_gameReset
            };

            fileSystemWatcher.Path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "LocalLow"), vrcLogDirFromHome);
            fileSystemWatcher.Filter = "*.*";
            fileSystemWatcher.IncludeSubdirectories = false;
            fileSystemWatcher.EnableRaisingEvents = false;

            statusStrip.Text = "Scan newer file in dir ...";
            var fileInfo = scanNewFile(fileSystemWatcher.Path, fileSystemWatcher.Filter);
            if (null == fileInfo)
            {
                targetFileName = null;
                lastReadPosition = 0;
            }
            else
            {
                targetFileName = Path.GetFileName(fileInfo.FullName);
                lastReadPosition = fileInfo.Length;
            }

            statusStrip.Text = "Wait for logs ...";
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.RunWorkerAsync();

            // Analysis Tab
        }

        private void BilliardsModuleEventLogger_Load(object sender, EventArgs e)
        {

        }

        private void ClearLog_Click(object sender, EventArgs e)
        {
            planeText.Text = string.Empty;
        }

        private void JumpToTail_Click(object sender, EventArgs e)
        {
            planeText.SelectionStart = planeText.Text.Length;
            planeText.ScrollToCaret();
        }

        private void ClearLog2_Click(object sender, EventArgs e)
        {
            richText.Text = string.Empty;
        }

        private void JumpToTail2_Click(object sender, EventArgs e)
        {
            richText.SelectionStart = richText.Text.Length;
            richText.ScrollToCaret();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker.ReportProgress(0, "Wait for logs ...");

            // 1秒間隔のチェックで問題ないと思われる
            var waitForChangedResult = fileSystemWatcher.WaitForChanged(WatcherChangeTypes.All, 1000);

            // Createdイベントが来ない場合があるのでここでチェックする
            if (waitForChangedResult.TimedOut)
            {
                backgroundWorker.ReportProgress(0, "Scan newer file in dir ...");
                var fileInfo = scanNewFile(fileSystemWatcher.Path, fileSystemWatcher.Filter);
                if (null != fileInfo)
                {                    
                    var newFileName = Path.GetFileName(fileInfo.FullName);
                    if (targetFileName == null || targetFileName != newFileName)
                    {
                        targetFileName = newFileName;
                        lastReadPosition = fileInfo.Length;
                        backgroundWorker.ReportProgress(0, "New file found !");
                    }
                }

            }

            bool tagetChanged = false;
            if (!waitForChangedResult.TimedOut && 
                null != targetFileName && 
                ( waitForChangedResult.Name == targetFileName || waitForChangedResult.OldName == targetFileName))
            {
                tagetChanged = true;
            }

            switch (waitForChangedResult.ChangeType)
            {

                case WatcherChangeTypes.Created:
                    {
                        // Createdイベントが来ない場合があるのでここでの処理はしない
                        break;
                    }
                case WatcherChangeTypes.Deleted:
                    {
                        // 出力監視しているファイルだった場合
                        if (tagetChanged)
                        {
                            // 出力監視を止めて、ファイル作成を監視する状態に戻す
                            targetFileName = null;
                            backgroundWorker.ReportProgress(0, "Target file is deleted ! (Wait for logs ...)");
                        }
                        break;
                    }
                case WatcherChangeTypes.Renamed:
                    {
                        // 出力監視しているファイルだった場合
                        if (tagetChanged)
                        {
                            // そのファイルの出力監視を続ける
                            targetFileName = waitForChangedResult.Name;
                        }
                        break;
                    }
            }

            if (null == targetFileName)
            {
                return;
            }

            try
            {
                var fileStream = new FileStream(Path.Join(fileSystemWatcher.Path, targetFileName), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                try
                {
                    if (lastReadPosition < fileStream.Length)
                    {
                        fileStream.Position = lastReadPosition;
                        var streamReader = new StreamReader(fileStream);
                        try
                        {
                            var readData = streamReader.ReadToEnd();
                            lastReadPosition = fileStream.Position;

                            if (null != OnLogReceived)
                            {
                                OnLogReceived(this, new BilliardsModuleEventLoggerEventArgs(readData));
                            }

                            // System.Diagnostics.Debug.Write(readData);
                            backgroundWorker.ReportProgress(1, readData);
                        }
                        catch
                        {
                            throw;
                        }
                        finally 
                        {
                            streamReader.Close();
                            streamReader.Dispose();
                        }
                    }
                    else
                    {
                        lastReadPosition = fileStream.Length;
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    fileStream.Close();
                    fileStream.Dispose(); 
                }
            }
            catch
            {
                // 出力監視を止めて、ファイル作成を監視する状態に戻す
                targetFileName = null;
                backgroundWorker.ReportProgress(0, "Target file is exception raised ! (Wait for logs ...)");
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (null == e.UserState)
            {
                return;
            }

            if (e.ProgressPercentage == 0)
            {
                statusStrip.Text = e.UserState.ToString();
                return;
            }

            receiveLogText(e.UserState.ToString());
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            statusStrip.Text = "Wait for logs ...";
            backgroundWorker.RunWorkerAsync();
        }

        private FileInfo? scanNewFile(string dir, string pattern)
        {
            var dirInfo = new DirectoryInfo(dir);
            var files = dirInfo.GetFiles(pattern, SearchOption.TopDirectoryOnly).OrderBy(f => f.CreationTime).ToList();
            FileInfo? fileInfo = null;
            if (0 < files.Count)
            {
                fileInfo = files.Last();
            }

            return fileInfo;
        }

        private void receiveLogText(string? logText)
        {
            if (null == logText)
            {
                return;
            }

            toPlaneLog(logText);

            var logLines = logText.Split(new char[] { '\n' });
            foreach( var logLine in logLines )
            {
                if (0 <= logLine.IndexOf(billiardsLogKeyword))
                {
                    parseBilliardsLog(logLine + "\n");
                }
            }
        }

        private void toPlaneLog(string logText)
        {
            bool keepNewer = false;
            if (planeText.SelectionStart == planeText.Text.Length)
            {
                keepNewer = true;
            }

            planeText.AppendText(logText);

            if (2000 < planeText.Lines.Length)
            {
                var newLines = new string[1000];
                Array.Copy(planeText.Lines, 1000, newLines, 0, newLines.Length);
                planeText.Lines = newLines;

                System.Diagnostics.Debug.WriteLine("planeTextは切り詰められました。");
            }

            if (keepNewer)
            {
                planeText.SelectionStart = planeText.Text.Length;
                planeText.ScrollToCaret();
            }
        }

        private void parseBilliardsLog(string logText)
        {
            bool keepNewer = false;
            try
            {
                if (richText.SelectionStart == richText.Text.Length)
                {
                    keepNewer = true;
                }
            }
            catch(ObjectDisposedException)
            {
                // nop
                System.Diagnostics.Debug.WriteLine("richText.SelectionStartが無効");
            }

            var appendStartPos = richText.TextLength;
            richText.AppendText(logText);
            logText = logText.TrimEnd();

            foreach(var parseMethod in parseMethods)
            {
                if (parseMethod(logText, appendStartPos))
                {
                    break;
                }
            }

            if (2000 < richText.Lines.Length)
            {
                var newLines = new string[1000];
                Array.Copy(richText.Lines, 1000, newLines, 0, newLines.Length);
                richText.Lines = newLines;

                System.Diagnostics.Debug.WriteLine("richTextは切り詰められました。");
            }

            if (keepNewer)
            {
                richText.SelectionStart = richText.Text.Length;
                richText.ScrollToCaret();
            }
        }

        // local only
        private bool parse_startingGame(string logText, int appendStartPos)
        {
            var posOfKeyword = logText.IndexOf(STR_startingGame);
            if (posOfKeyword < 0)
            {
                return false;
            }

            richText.SelectionStart = appendStartPos + posOfKeyword;
            richText.SelectionLength = STR_startingGame.Length;
            richText.SelectionColor = Color.Firebrick;

            if (null != OnStartingGameReceived)
            {
                OnStartingGameReceived(this, new BilliardsModuleEventLoggerEventArgs(logText));
            }

            return true;
        }

        private bool parse_gameState(string logText, int appendStartPos)
        {
            var posOfKeyword = logText.IndexOf(STR_gameState);
            if (posOfKeyword < 0)
            {
                return false;
            }

            richText.SelectionStart = appendStartPos + posOfKeyword;
            richText.SelectionLength = STR_gameState.Length;
            richText.SelectionColor = Color.Yellow;

            var partOf_gameState = logText.Split("v2:", 2);
            if (null != partOf_gameState && 1 < partOf_gameState.Length)
            {
                try
                {
                    var poolState = new PoolState(partOf_gameState[1]);

                    richText.SelectionStart = appendStartPos + partOf_gameState[0].Length;
                    richText.SelectionLength = "v2:".Length + partOf_gameState[1].Length;
                    richText.SelectionColor = Color.Yellow;

                    if (null != OnGameStateReceived)
                    {
                        OnGameStateReceived(this, new BilliardsModuleEventLoggerEventArgs(logText, poolState));
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

            return true;
        }

        private bool parse_gameReset(string logText, int appendStartPos)
        {
            var posOfKeyword = logText.IndexOf(STR_gameReset);
            if (posOfKeyword < 0)
            {
                return false;
            }

            richText.SelectionStart = appendStartPos + posOfKeyword;
            richText.SelectionLength = STR_gameReset.Length;
            richText.SelectionColor = Color.Red;

            if (null != OnGameResetReceived)
            {
                OnGameResetReceived(this, new BilliardsModuleEventLoggerEventArgs(logText));
            }

            return true;
        }

        private bool parse_onRemoteLobbyOpened(string logText, int appendStartPos)
        {
            var posOfKeyword = logText.IndexOf(STR_onRemoteLobbyOpened);
            if (posOfKeyword < 0)
            {
                return false;
            }

            richText.SelectionStart = appendStartPos + posOfKeyword;
            richText.SelectionLength = STR_onRemoteLobbyOpened.Length;
            richText.SelectionColor = Color.CadetBlue;

            if (null != OnRemoteLobbyOpened)
            {
                OnRemoteLobbyOpened(this, new BilliardsModuleEventLoggerEventArgs(logText));
            }

            return true;
        }


        private bool parse_onRemoteGameSettingsUpdated(string logText, int appendStartPos)
        {
            var posOfKeyword = logText.IndexOf(STR_onRemoteGameSettingsUpdated);
            if (posOfKeyword < 0)
            {
                return false;
            }

            richText.SelectionStart = appendStartPos + posOfKeyword;
            richText.SelectionLength = STR_onRemoteGameSettingsUpdated.Length;
            richText.SelectionColor = Color.Gainsboro;

            // onRemoteGameSettingsUpdated gameMode=1 timer=0 teams=False guideline=True locking=True
            var keywords = new string[] { "gameMode", "teams" };
            var intValueDict = new Dictionary<string, int>();

            var paramOf_onRemoteGameSettingsUpdated = logText.Substring(posOfKeyword + STR_onRemoteGameSettingsUpdated.Length);
            var partOf_onRemoteGameSettingsUpdated = paramOf_onRemoteGameSettingsUpdated.Split(' ', 5, StringSplitOptions.RemoveEmptyEntries);
            foreach(var keyword in keywords)
            {
                foreach (var param in partOf_onRemoteGameSettingsUpdated)
                {
                    var pair = param.Split('=', 2, StringSplitOptions.TrimEntries);
                    if (null != pair && 1 < pair.Length && pair[0] == keyword)
                    {
                        richText.SelectionStart = appendStartPos + posOfKeyword + STR_onRemoteGameSettingsUpdated.Length + paramOf_onRemoteGameSettingsUpdated.IndexOf(keyword);
                        richText.SelectionLength = param.Length;
                        richText.SelectionColor = Color.Gainsboro;

                        int intValue = int.MinValue;
                        if (!int.TryParse(pair[1], out intValue))
                        {
                            if (bool.TryParse(pair[1], out var boolValue))
                            {
                                intValue = boolValue ? 1 : 0;
                            }
                        }
                        if (intValue != int.MinValue)
                        {
                            intValueDict.Add(keyword, intValue);
                        }
                        break;
                    }
                }

            }

            if (null != OnRemoteGameSettingsUpdated)
            {
                OnRemoteGameSettingsUpdated(this, new BilliardsModuleEventLoggerEventArgs(logText, intValueDict["gameMode"] /*, intValueDict["teams"]*/));
            }

            return true;
        }

        private bool parse_onRemotePlayersChanged(string logText, int appendStartPos)
        {
            var posOfKeyword = logText.IndexOf(STR_onRemotePlayersChanged);
            if (posOfKeyword < 0)
            {
                return false;
            }

            richText.SelectionStart = appendStartPos + posOfKeyword;
            richText.SelectionLength = STR_onRemotePlayersChanged.Length;
            richText.SelectionColor = Color.AliceBlue;

            var players = new string[4];

            var partOf_onRemotePlayersChanged = logText.Split("=", 2);
            if (null != partOf_onRemotePlayersChanged && 1 < partOf_onRemotePlayersChanged.Length)
            {
                // TODO: player名にカンマが入ることがあるのか
                var newPlayers = partOf_onRemotePlayersChanged[1].Split(",", 5);
                if (4 < newPlayers.Length)
                {
                    System.Diagnostics.Debug.WriteLine("player名にカンマが入っている");
                }

                richText.SelectionStart = appendStartPos + partOf_onRemotePlayersChanged[0].Length + "=".Length;
                for (int i = 0; i < players.Length && i < newPlayers.Length; i++)
                {
                    var newPlayer = newPlayers[i];
                    if (newPlayer != "none")
                    {
                        richText.SelectionLength = newPlayer.Length;
                        richText.SelectionColor = Color.AliceBlue;
                        players[i] = newPlayer;
                    }
                    richText.SelectionStart += newPlayer.Length + ",".Length;
                }
            }

            if (null != OnRemotePlayersChanged)
            {
                OnRemotePlayersChanged(this, new BilliardsModuleEventLoggerEventArgs(logText, players));
            }

            return true;
        }

        // remote only
        private bool parse_OnRemoteGameStarted(string logText, int appendStartPos)
        {
            var posOfKeyword = logText.IndexOf(STR_onRemoteGameStarted);
            if (posOfKeyword < 0)
            {
                return false;
            }

            richText.SelectionStart = appendStartPos + posOfKeyword;
            richText.SelectionLength = STR_onRemoteGameStarted.Length;
            richText.SelectionColor = Color.IndianRed;

            if (null != OnRemoteGameStarted)
            {
                OnRemoteGameStarted(this, new BilliardsModuleEventLoggerEventArgs(logText));
            }

            return true;
        }

        private bool parse_onRemoteTurnSimulate(string logText, int appendStartPos)
        {
            var posOfKeyword = logText.IndexOf(STR_onRemoteTurnSimulate);
            if (posOfKeyword < 0)
            {
                return false;
            }

            richText.SelectionStart = posOfKeyword;
            richText.SelectionLength = STR_onRemoteTurnSimulate.Length;
            richText.SelectionColor = Color.Aqua;

            var ownerPlayer = string.Empty;

            var partOf_onRemoteTurnSimulate = logText.Split("owner=", 2);
            if (null != partOf_onRemoteTurnSimulate && 1 < partOf_onRemoteTurnSimulate.Length)
            {
                richText.SelectionStart = appendStartPos + partOf_onRemoteTurnSimulate[0].Length + "owner=".Length;
                richText.SelectionLength = partOf_onRemoteTurnSimulate[1].Length;
                richText.SelectionColor = Color.Aqua;

                ownerPlayer = partOf_onRemoteTurnSimulate[1];
            }

            if (null != OnRemoteTurnSimulate)
            {
                OnRemoteTurnSimulate(this, new BilliardsModuleEventLoggerEventArgs(logText, ownerPlayer));
            }

            return true;
        }

        private bool parse_onRemoteRepositionStateChanged(string logText, int appendStartPos)
        {
            var posOfKeyword = logText.IndexOf(STR_onRemoteRepositionStateChanged);
            if (posOfKeyword < 0)
            {
                return false;
            }

            richText.SelectionStart = posOfKeyword;
            richText.SelectionLength = STR_onRemoteRepositionStateChanged.Length;
            richText.SelectionColor = Color.Honeydew;

            // onRemoteRepositionStateChanged repositionState=1
            int? repositionState = null;

            var partOf_onRemoteRepositionStateChanged = logText.Split("=", 2);
            if (null != partOf_onRemoteRepositionStateChanged && 1 < partOf_onRemoteRepositionStateChanged.Length)
            {
                richText.SelectionStart = appendStartPos + partOf_onRemoteRepositionStateChanged[0].Length + "=".Length;
                richText.SelectionLength = partOf_onRemoteRepositionStateChanged[1].Length;
                richText.SelectionColor = Color.Honeydew;

                repositionState = int.Parse(partOf_onRemoteRepositionStateChanged[1]);
            }

            if (null != OnRemoteRepositionStateChanged)
            {
                OnRemoteRepositionStateChanged(this, new BilliardsModuleEventLoggerEventArgs(logText, repositionState));
            }

            return true;
        }

        private bool parse_onRemoteBallsPocketedChanged(string logText, int appendStartPos)
        {
            var posOfKeyword = logText.IndexOf(STR_onRemoteBallsPocketedChanged);
            if (posOfKeyword < 0)
            {
                return false;
            }

            richText.SelectionStart = posOfKeyword;
            richText.SelectionLength = STR_onRemoteBallsPocketedChanged.Length;
            richText.SelectionColor = Color.AntiqueWhite;

            var ballProcketedFlags = new bool[0];

            var partOf_onRemoteBallsPocketedChanged = logText.Split("=", 2);
            if (null != partOf_onRemoteBallsPocketedChanged && 1 < partOf_onRemoteBallsPocketedChanged.Length)
            {
                richText.SelectionStart = appendStartPos + partOf_onRemoteBallsPocketedChanged[0].Length + "=".Length;
                richText.SelectionLength = partOf_onRemoteBallsPocketedChanged[1].Length;
                richText.SelectionColor = Color.AntiqueWhite;

                var hex4Char = partOf_onRemoteBallsPocketedChanged[1].PadLeft(4, '0');
                var ballsPocketdValue = Convert.ToUInt16(hex4Char, 16);
                ballProcketedFlags = PoolState.ballProcketedFlags(ballsPocketdValue);

                System.Diagnostics.Debug.WriteLine("OnRemoteBallsPocketedChanged {0}", Convert.ToString(ballsPocketdValue, 2).PadLeft(16, '0'));
            }

            if (null != OnRemoteBallsPocketedChanged)
            {
                OnRemoteBallsPocketedChanged(this, new BilliardsModuleEventLoggerEventArgs(logText, ballProcketedFlags));
            }

            return true;
        }

        private bool parse_onRemoteGameEnded(string logText, int appendStartPos)
        {
            var posOfKeyword = logText.IndexOf(STR_onRemoteGameEnded);
            if (posOfKeyword < 0)
            {
                return false;
            }

            richText.SelectionStart = posOfKeyword;
            richText.SelectionLength = STR_onRemoteGameEnded.Length;
            richText.SelectionColor = Color.DarkBlue;

            int? winningTeam = null;

            // onRemoteGameEnded winningTeam=0
            var partOf_onRemoteGameEnded = logText.Split("=", 2);
            if (null != partOf_onRemoteGameEnded && 1 < partOf_onRemoteGameEnded.Length)
            {
                richText.SelectionStart = appendStartPos + partOf_onRemoteGameEnded[0].Length + "=".Length;
                richText.SelectionLength = partOf_onRemoteGameEnded[1].Length;
                richText.SelectionColor = Color.DarkBlue;

                winningTeam = int.Parse(partOf_onRemoteGameEnded[1]);
            }

            if (null != OnRemoteGameEnded)
            {
                OnRemoteGameEnded(this, new BilliardsModuleEventLoggerEventArgs(logText, winningTeam));
            }

            return true;
        }


        private void BilliardsModuleEventLogger_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == BilliardsLog)
            {
                // RichTextBoxの表示が更新されないことがある問題の対処
                richText.SelectionStart = 0;
                richText.SelectionStart = richText.Text.Length;
                richText.ScrollToCaret();
            }
        }
    }
}
