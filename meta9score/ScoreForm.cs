using System.Drawing;

namespace meta9score
{
    public partial class ScoreForm : Form
    {
        private BilliardsModuleEventLogger billiardsModuleEventLogger;
        private ConfigDialog configDialog;

        private FlowLayoutPanel[] flowLayoutPocketedTeams;
        private FlowLayoutPanel[] flowLayoutNocountTeams;

        private Label[] labelPlayers;
        private Label[] labelSinglePlayers;
        private Label[][] labelTeamPlayers;
        private Color[] colorTeams;

        private stringReference[] currentPoolTeamSidePlayers;
        private stringReference[][] playersByCurrentPoolTeamSide;
        private class stringReference
        {
            public string? value = null;
            public stringReference()
            {

            }

            public stringReference(string? value)
            {
                this.value = value;
            }
        }

        private Label[] labelCurrentScores;
        private Label[] labelTotalScores;
        private Label[] labelRemainPoints;

        private Label[] labelBalls;
        private Label[] label9Balls;

        private int goalPoint = 120;
        private bool teamMemberFixed = false;
        private bool gameModeFixed = false;
        private bool gameEnded = false;
        private int gameMode = 0;
        private string? lastShotPlayer = null;
        private int packetNumber = 0;
        private int stateNumber = 0;
        private PoolState? gameEndState = null;

        public ScoreForm()
        {
            InitializeComponent();

            flowLayoutPocketedTeams = new FlowLayoutPanel[]
            {
                flowLayoutPocketedTeam1, flowLayoutPocketedTeam2
            };

            flowLayoutNocountTeams = new FlowLayoutPanel[]
            {
                flowLayoutNocountTeam1, flowLayoutNocountTeam2
            };

            labelPlayers = new Label[]
            {
                labelPlayer1, labelPlayer2, 
                labelPlayer3, labelPlayer4
            };

            labelSinglePlayers = new Label[]
            {
                labelPlayer1, labelPlayer2
            };

            labelTeamPlayers = new Label[][]
            {
                new Label[]{ labelPlayer1, labelPlayer3 }, 
                new Label[]{ labelPlayer2, labelPlayer4 }
            };

            colorTeams = new Color[]
            {
                labelPlayer1.BackColor, labelPlayer2.BackColor
            };

            currentPoolTeamSidePlayers = new stringReference[]
            {
                new stringReference(string.Empty),
                new stringReference(string.Empty),
                new stringReference(string.Empty),
                new stringReference(string.Empty)
            };

            playersByCurrentPoolTeamSide = new stringReference[][]
            {
                new stringReference[]{ currentPoolTeamSidePlayers[0], currentPoolTeamSidePlayers[2] },
                new stringReference[]{ currentPoolTeamSidePlayers[1], currentPoolTeamSidePlayers[3] }
            };

            labelCurrentScores = new Label[]
            {
                labelCurrentScoreTeam1, labelCurrentScoreTeam2
            };

            labelTotalScores = new Label[]
            {
                labelTotalScoreTeam1, labelTotalScoreTeam2
            };

            labelRemainPoints = new Label[]
            {
                labelRemainPointTeam1, labelRemainPointTeam2
            };

            labelBalls = new Label[]
            {
                labelBall1, labelBall2, labelBall3, labelBall4, labelBall5, labelBall6, labelBall7, labelBall8, 
                labelBall9, labelBall10, labelBall11, labelBall12, labelBall13, labelBall14, labelBall15
            };

            label9Balls = new Label[]
            {
                labelBall1, labelBall2, labelBall3,
                labelBall4, labelBall5, labelBall6,
                labelBall7, labelBall8, labelBall9
            };

            configDialog = new ConfigDialog();

            billiardsModuleEventLogger = new BilliardsModuleEventLogger();
            //billiardsModuleEventLogger.OnLogReceived += BilliardsModuleEventLogger_OnLogReceived;
            billiardsModuleEventLogger.OnStartingGameReceived += BilliardsModuleEventLogger_OnRemoteGameStarted;
            billiardsModuleEventLogger.OnRemoteStateReceived += BilliardsModuleEventLogger_OnRemoteStateReceived;
            billiardsModuleEventLogger.OnGameStateReceived += BilliardsModuleEventLogger_OnGameStateReceived;
            billiardsModuleEventLogger.OnGameResetReceived += BilliardsModuleEventLogger_OnGameResetReceived;
            billiardsModuleEventLogger.OnRemoteLobbyOpened += BilliardsModuleEventLogger_OnRemoteLobbyOpened;
            billiardsModuleEventLogger.OnRemoteGameSettingsUpdated += BilliardsModuleEventLogger_OnRemoteGameSettingsUpdated;
            billiardsModuleEventLogger.OnRemotePlayersChanged += BilliardsModuleEventLogger_OnRemotePlayersChanged;
            billiardsModuleEventLogger.OnRemoteGameStarted += BilliardsModuleEventLogger_OnRemoteGameStarted;
            billiardsModuleEventLogger.OnRemoteTurnSimulate += BilliardsModuleEventLogger_OnRemoteTurnSimulate;
            //billiardsModuleEventLogger.OnRemoteRepositionStateChanged += BilliardsModuleEventLogger_OnRemoteRepositionStateChanged;
            //billiardsModuleEventLogger.OnRemoteBallsPocketedChanged += BilliardsModuleEventLogger_OnRemoteBallsPocketedChanged;
            billiardsModuleEventLogger.OnRemoteGameEnded += BilliardsModuleEventLogger_OnRemoteGameEnded;
        }

        private void FormScore_Load(object sender, EventArgs e)
        {
            init();
            timer.Enabled = true;
        }

        private void init()
        {
            teamMemberFixed = false;
            gameModeFixed = false;
            lastShotPlayer = null;
            playersReset();
            comboBoxGoalPointList.SelectedIndex = (comboBoxGoalPointList.Items.Count - 1);
            scoreReset();
            resetBallToolTips();
        }

        private void playersReset()
        {
            foreach(var labelPlayer in labelPlayers)
            {
                labelPlayer.Visible = false;
                labelPlayer.Text = string.Empty;
            }
            foreach (var labelPlayer in labelSinglePlayers)
            {
                labelPlayer.Visible = true;
                labelPlayer.Text = " ";
            }
            for (int i = 0; i < labelTeamPlayers.Length && i < colorTeams.Length; i++)
            {
                var teamPlayers = labelTeamPlayers[i];
                var color = colorTeams[i];
                foreach (var labelPlayer in teamPlayers)
                {
                    labelPlayer.BackColor = color;
                }
            }

            for (int i = 0; i < currentPoolTeamSidePlayers.Length; i++)
            {
                currentPoolTeamSidePlayers[i].value = string.Empty;
            }
        }

        private void scoreReset()
        {
            foreach (var labelScore in labelCurrentScores)
            {
                labelScore.Text = "0";
            }
            foreach (var labelScore in labelTotalScores)
            {
                labelScore.Text = "0";
            }
            //goalPoint = Convert.ToInt32(comboBoxGoalPointList.SelectedItem);
            foreach (var labelScore in labelRemainPoints)
            {
                labelScore.Text = goalPoint.ToString();
            }
            scoreColorUpdate();
        }

        /*
        private void BilliardsModuleEventLogger_OnLogReceived(object? sender, EventArgs args)
        {
            var billiardsModuleEventArgs = args as BilliardsModuleEventLoggerEventArgs;
            if (null == billiardsModuleEventArgs || null == billiardsModuleEventArgs.text)
            {
                return;
            }
            System.Diagnostics.Debug.WriteLine(billiardsModuleEventArgs.text)
        }
        */

        private void BilliardsModuleEventLogger_OnRemoteStateReceived(object? sender, EventArgs args)
        {
            var billiardsModuleEventArgs = args as BilliardsModuleEventLoggerEventArgs;
            if (null == billiardsModuleEventArgs || null == billiardsModuleEventArgs.intValue || null == billiardsModuleEventArgs.intValue2)
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine("packet={0}, state={1}", billiardsModuleEventArgs.intValue, billiardsModuleEventArgs.intValue2);

            // state が更新されなくなったら Game End 判定
            if (0 < stateNumber && stateNumber == billiardsModuleEventArgs.intValue2)
            {
                gameEnded = true;
            }

            packetNumber = (int)billiardsModuleEventArgs.intValue;
            stateNumber = (int)billiardsModuleEventArgs.intValue2;
        }

        private void BilliardsModuleEventLogger_OnGameStateReceived(object? sender, EventArgs args)
        {
            var billiardsModuleEventArgs = args as BilliardsModuleEventLoggerEventArgs;
            if (null == billiardsModuleEventArgs || null == billiardsModuleEventArgs.poolState)
            {
                return;
            }

            var poolState = billiardsModuleEventArgs.poolState;

            System.Diagnostics.Debug.WriteLine(poolState.dump());

            if (!gameModeFixed)
            {
                gameModeFixed = true;
                changeGameMode(poolState.gameModeSynced);
            }

            if (gameEnded)
            {
                gameEndState = poolState;
            }

            // shot前の状態通知は処理する必要はない
            if (1 == poolState.turnStateSynced)
            {
                return;
            }

            // shot結果の状態でポケット判定する
            var ballProcketedFlags = PoolState.ballProcketedFlags(poolState.ballsPocketedSynced);
            var foul = (poolState.turnStateSynced == 2);
            updatePocketed(ballProcketedFlags, foul);
        }

        private void BilliardsModuleEventLogger_OnGameResetReceived(object? sender, EventArgs args)
        {
            var billiardsModuleEventArgs = args as BilliardsModuleEventLoggerEventArgs;
            if (null == billiardsModuleEventArgs)
            {
                return;
            }

            rollbackCurrentGame();
        }

        private void BilliardsModuleEventLogger_OnRemoteGameSettingsUpdated(object? sender, EventArgs args)
        {
            var billiardsModuleEventArgs = args as BilliardsModuleEventLoggerEventArgs;
            if (null == billiardsModuleEventArgs || null == billiardsModuleEventArgs.intValue || null == billiardsModuleEventArgs.intValue2)
            {
                return;
            }

            changeGameMode((int)billiardsModuleEventArgs.intValue);
        }


        private void BilliardsModuleEventLogger_OnRemoteLobbyOpened(object? sender, EventArgs args)
        {
            var billiardsModuleEventArgs = args as BilliardsModuleEventLoggerEventArgs;
            if (null == billiardsModuleEventArgs)
            {
                return;
            }

            resetSubTotal();
        }

        private void BilliardsModuleEventLogger_OnRemotePlayersChanged(object? sender, EventArgs args)
        {
            var billiardsModuleEventArgs = args as BilliardsModuleEventLoggerEventArgs;
            if (null == billiardsModuleEventArgs || null == billiardsModuleEventArgs.players)
            {
                return;
            }

            //if (teamMemberFixed)
            //{
            //    return;
            //}
                
            for (int i = 0; i < labelPlayers.Length && i < billiardsModuleEventArgs.players.Length; i++)
            {
                var player = billiardsModuleEventArgs.players[i];
                if (!string.IsNullOrEmpty(player))
                {
                    if (!teamMemberFixed)
                    {
                        var labelPlayer = labelPlayers[i];
                        labelPlayer.Text = player;
                        labelPlayer.Visible = true;
                    }

                    currentPoolTeamSidePlayers[i].value = player;
                }
            }
            System.Diagnostics.Debug.WriteLine("currentPoolPlayers: " + string.Join(", ", currentPoolTeamSidePlayers.Select(x => x.value).ToArray()));
        }

        private void BilliardsModuleEventLogger_OnRemoteGameStarted(object? sender, EventArgs args)
        {
            var billiardsModuleEventArgs = args as BilliardsModuleEventLoggerEventArgs;
            if (null == billiardsModuleEventArgs)
            {
                return;
            }

            gameEnded = false;
            gameModeFixed = true;
            teamMemberFixed = true;
            foreach (var labelPlayer in labelPlayers)
            {
                labelPlayer.BackColor = Color.FromKnownColor(KnownColor.Control);
                if (labelPlayer.Text == " ")
                {
                    labelPlayer.Visible = false;
                }
            }

            System.Diagnostics.Debug.WriteLine("labelPlayers: " + string.Join(", ", labelPlayers.Select(x => x.Text).ToArray()));
            System.Diagnostics.Debug.WriteLine("currentPoolPlayers: " + string.Join(", ", currentPoolTeamSidePlayers.Select(x => x.value).ToArray()));
        }

        /*
        private void BilliardsModuleEventLogger_OnRemoteRepositionStateChanged(object? sender, EventArgs args)
        {
            var billiardsModuleEventArgs = args as BilliardsModuleEventLoggerEventArgs;
            if (null == billiardsModuleEventArgs || null == billiardsModuleEventArgs.intValue)
            {
                return;
            }

            var repositionState = billiardsModuleEventArgs.intValue;
            if (2 == repositionState)
            {
                System.Diagnostics.Debug.WriteLine("free ball !!!");
            }
        }
        */

        private void BilliardsModuleEventLogger_OnRemoteTurnSimulate(object? sender, EventArgs args)
        {
            var billiardsModuleEventArgs = args as BilliardsModuleEventLoggerEventArgs;
            if (null == billiardsModuleEventArgs || null == billiardsModuleEventArgs.player)
            {
                return;
            }

            lastShotPlayer = billiardsModuleEventArgs.player;
        }

        /*
        private void BilliardsModuleEventLogger_OnRemoteBallsPocketedChanged(object? sender, EventArgs args)
        {
            var billiardsModuleEventArgs = args as BilliardsModuleEventLoggerEventArgs;
            if (null == billiardsModuleEventArgs || null == billiardsModuleEventArgs.ballProcketedFlags)
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine("OnRemoteBallsPocketedChanged {0}", billiardsModuleEventArgs.ballProcketedFlags);
            //updatePocketed(billiardsModuleEventArgs.ballProcketedFlags);
        }
        */

        private void BilliardsModuleEventLogger_OnRemoteGameEnded(object? sender, EventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("OnRemoteGameEnded");

            var billiardsModuleEventArgs = args as BilliardsModuleEventLoggerEventArgs;
            if (null == billiardsModuleEventArgs || null == billiardsModuleEventArgs.intValue)
            {
                return;
            }

            // resetSubTotal();
            //gameEnded = true;

            // 勝った側
            var winnerTeamCurrentPoolTeamSideIndex = billiardsModuleEventArgs.intValue;
            System.Diagnostics.Debug.WriteLine(string.Format("winnerTeamIndex {0}", winnerTeamCurrentPoolTeamSideIndex));
            if (2 == winnerTeamCurrentPoolTeamSideIndex)
            {
                // リセットの場合
                return;
            }

            System.Diagnostics.Debug.WriteLine(string.Format("lastShotPlayer {0}", lastShotPlayer));

            // 最後に突いた側
            var currentPoolTeamSideIndex = indexOfLastShotCurrentPoolTeamSideByPlayer(lastShotPlayer);
            //if (0 <= teamIndex && teamIndex < labelTeamPlayers.Length)
            //{
            //    lastShotPlayer = null;
            //}
            System.Diagnostics.Debug.WriteLine(string.Format("teamIndex of lastShotPlayer {0}", currentPoolTeamSideIndex));

            // Game End の状態でポケット判定する
            var ballProcketedFlags = PoolState.ballProcketedFlags(gameEndState.ballsPocketedSynced);
            var foul = (winnerTeamCurrentPoolTeamSideIndex != currentPoolTeamSideIndex);
            System.Diagnostics.Debug.WriteLine(string.Format("flag of foul {0}", foul));
            updatePocketed(ballProcketedFlags, foul);

            for (int i = 0; i < currentPoolTeamSidePlayers.Length; i++)
            {
                currentPoolTeamSidePlayers[i].value = string.Empty;
            }
        }

        /*
        private void checkGameMode(bool[] ballProcketedFlags)
        {
            if (gameModeFixed)
            {
                return;
            }

            bool hiBallAlive = false;
            for (int i = 10; i < ballProcketedFlags.Length; i++)
            {
                if (!ballProcketedFlags[i])
                {
                    hiBallAlive = true;
                    break;
                }
            }

            if (hiBallAlive)
            {
                changeTo8ballMode();
            }
            else
            {
                changeTo9ballMode();
            }

            gameModeFixed = true;
        }
        */

        private void changeTo8ballMode()
        {
            for (int i = 9; i <= labelBalls.Length; i++)
            {
                var labelBall = labelBalls[i - 1];
                var point = checkBox8BallCustomPoint.Checked ? (i - 8) : i;
                labelBall.Tag = point.ToString();
                labelBall.Visible = true;
            }
        }

        private void changeTo9ballMode()
        {
            labelBalls[9 - 1].Tag = "9";
            for (int i = 10; i <= labelBalls.Length; i++)
            {
                var labelBall = labelBalls[i - 1];
                labelBall.Tag = "0";
                labelBall.Visible = false;
            }
        }

        private void changeGameMode(int mode)
        {
            gameMode = mode;
            if (gameMode == 0)
            {
                changeTo8ballMode();
            }
            else if (gameMode == 1)
            {
                changeTo9ballMode();
            }

            resetBallToolTips();

            if (gameModeFixed)
            {
                if (gameMode == 0)
                {
                    groupBoxOption.Visible = true;
                }
                else if (gameMode == 1)
                {
                    groupBoxOption.Visible = false;
                }
            }
        }

        private void resetBallToolTips()
        {
            for (int i = 0; i < labelBalls.Length; i++)
            {
                var labelBall = labelBalls[i];
                toolTip.SetToolTip(labelBall, string.Format("{0}点", labelBall.Tag));
            }
        }

        private void rollbackCurrentGame()
        {
            for (int i = 0; i < labelTeamPlayers.Length; i++)
            {
                var labelCurrentScore = labelCurrentScores[i];
                var labelTotalScore = labelTotalScores[i];
                var labelRemainPoint = labelRemainPoints[i];

                var subTotal = int.Parse(labelCurrentScore.Text);
                var total = int.Parse(labelTotalScore.Text) - subTotal;
                labelTotalScore.Text = total.ToString();
                labelRemainPoint.Text = (goalPoint - total).ToString();
            }
        }

        private void resetSubTotal()
        {
            for (int i = 0; i < labelTeamPlayers.Length; i++)
            {
                labelCurrentScores[i].Text = "0";
            }
        }

        private void updatePocketed(bool[] ballProcketedFlags, bool foul)
        {
            var teamIndex = indexOfLastShotTeamByPlayer(lastShotPlayer);
            if (0 <= teamIndex && teamIndex < labelTeamPlayers.Length)
            {
                lastShotPlayer = null;
            }
            for (int i = 0; i < ballProcketedFlags.Length; i++)
            {
                if (0 < i && i <= labelBalls.Length)
                {
                    var labelBall = labelBalls[i - 1];
                    if (ballProcketedFlags[i])
                    {
                        if (labelBall.Parent == flowLayoutAvailBalls)
                        {
                            var moveToParent = flowLayoutPocketedUnknown;
                            if (0 <= teamIndex && teamIndex < labelTeamPlayers.Length)
                            {
                                if (foul)
                                {
                                    moveToParent = flowLayoutNocountTeams[teamIndex];
                                }
                                else
                                {
                                    moveToParent = flowLayoutPocketedTeams[teamIndex];
                                    if (null != labelBall.Tag)
                                    {
                                        scoreCountUp(teamIndex, int.Parse(labelBall.Tag.ToString()));
                                    }
                                }
                            }
                            labelBall.Parent = moveToParent;
                        }
                    }
                    else
                    {
                        labelBall.Parent = flowLayoutAvailBalls;
                    }
                }
            }
        }

        private int indexOfLastShotTeamByPlayer(string lastShotPlayer)
        {
            if (null == lastShotPlayer)
            {
                return -1;
            }

            for (int i = 0; i < labelTeamPlayers.Length; i++)
            {
                for (int j = 0; j < labelTeamPlayers[i].Length; j++)
                {
                    if (labelTeamPlayers[i][j].Text == lastShotPlayer)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private int indexOfLastShotCurrentPoolTeamSideByPlayer(string lastShotPlayer)
        {
            if (null == lastShotPlayer)
            {
                return -1;
            }

            for (int i = 0; i < playersByCurrentPoolTeamSide.Length; i++)
            {
                for (int j = 0; j < playersByCurrentPoolTeamSide[i].Length; j++)
                {
                    if (playersByCurrentPoolTeamSide[i][j].value == lastShotPlayer)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private void scoreCountUp(int teamIndex, int point)
        {
            if (teamIndex < 0 || labelTeamPlayers.Length <= teamIndex || 0 == point)
            {
                return;
            }

            var labelCurrentScore = labelCurrentScores[teamIndex];
            var labelTotalScore = labelTotalScores[teamIndex];
            var labelRemainPoint = labelRemainPoints[teamIndex];

            var subTotal = int.Parse(labelCurrentScore.Text) + point;
            labelCurrentScore.Text = subTotal.ToString();
            var total = int.Parse(labelTotalScore.Text) + point;
            labelTotalScore.Text = total.ToString();
            labelRemainPoint.Text = (goalPoint - total).ToString();

            scoreColorUpdate();
        }

        private void scoreColorUpdate()
        {
            foreach (var labelScore in labelTotalScores)
            {
                var score = int.Parse(labelScore.Text);
                if (goalPoint <= score)
                {
                    labelScore.ForeColor = Color.Red;
                }
                else if (goalPoint <= score - 10)
                {
                    labelScore.ForeColor = Color.RosyBrown;
                }
                else
                {
                    labelScore.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
                }
            }
            foreach (var labelScore in labelRemainPoints)
            {
                var score = int.Parse(labelScore.Text);
                if (score <= 10)
                {
                    labelScore.ForeColor = Color.RosyBrown;
                }
                else if (score <= 0)
                {
                    labelScore.ForeColor = Color.Red;
                }
                else
                {
                    labelScore.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
                }
            }
        }

        private void menuItemLog_Click(object sender, EventArgs e)
        {
            billiardsModuleEventLogger.Show();
        }

        private void menuItemConfig_Click(object sender, EventArgs e)
        {
            configDialog.ShowDialog();
        }

        private void scoreClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            init();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            labelPlayerFix.Text = string.Format(
                "プレイヤー認識 : {0}",
                teamMemberFixed ? "検出済み" : "検出中"
                );
            labelGameMode.Text = string.Format(
                "ゲームモード : {0}",
                gameModeFixed ? (gameMode == 0 ? "8ball" : "9ball") : "検出中"
                );
            labelSource.Text = string.Format(
                "集計ソース : {0}",
                "ログファイル監視"
                );
            labelRemoteState.Text = string.Format(
                "packet = {0}, state = {1}",
                packetNumber, stateNumber
                );
        }

        private void comboBoxGoalPointList_SelectedIndexChanged(object sender, EventArgs e)
        {
            goalPoint = Convert.ToInt32(comboBoxGoalPointList.SelectedItem);
            for (int i = 0; i < labelTotalScores.Length; i++)
            {
                var labelTotalScore = labelTotalScores[i];
                var labelRemainPoint = labelRemainPoints[i];
                var score = int.Parse(labelTotalScore.Text);
                labelRemainPoint.Text = (goalPoint - score).ToString();
            }
            scoreColorUpdate();
        }

        private void checkBox8BallCustomPoint_CheckedChanged(object sender, EventArgs e)
        {
            changeTo8ballMode();
            resetBallToolTips();
        }
    }
}