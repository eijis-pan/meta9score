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

        private Label[] labelCurrentScores;
        private Label[] labelTotalScores;
        private Label[] labelRemainPoints;

        private Label[] labelBalls;
        private Label[] label9Balls;

        private int goalPoint = 120;
        private bool teamMemberFixed = false;
        //private bool gameModeFixed = false;
        private string? lastShotPlayer = null; 

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
            billiardsModuleEventLogger.OnGameStateReceived += BilliardsModuleEventLogger_OnGameStateReceived;
            billiardsModuleEventLogger.OnGameResetReceived += BilliardsModuleEventLogger_OnGameResetReceived;
            billiardsModuleEventLogger.OnRemoteLobbyOpened += BilliardsModuleEventLogger_OnRemoteLobbyOpened;
            billiardsModuleEventLogger.OnRemoteGameSettingsUpdated += BilliardsModuleEventLogger_OnRemoteGameSettingsUpdated;
            billiardsModuleEventLogger.OnRemotePlayersChanged += BilliardsModuleEventLogger_OnRemotePlayersChanged;
            billiardsModuleEventLogger.OnRemoteGameStarted += BilliardsModuleEventLogger_OnRemoteGameStarted;
            billiardsModuleEventLogger.OnRemoteTurnSimulate += BilliardsModuleEventLogger_OnRemoteTurnSimulate;
            //billiardsModuleEventLogger.OnRemoteRepositionStateChanged += BilliardsModuleEventLogger_OnRemoteRepositionStateChanged;
            //billiardsModuleEventLogger.OnRemoteBallsPocketedChanged += BilliardsModuleEventLogger_OnRemoteBallsPocketedChanged;
            //billiardsModuleEventLogger.OnRemoteGameEnded += BilliardsModuleEventLogger_OnRemoteGameEnded;
        }

        private void FormScore_Load(object sender, EventArgs e)
        {
            init();
        }

        private void init()
        {
            teamMemberFixed = false;
            //gameModeFixed = false;
            lastShotPlayer = null;
            playersReset();
            comboBoxGoalPointList.SelectedIndex = 0;
            scoreReset();
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
            goalPoint = Convert.ToInt32(comboBoxGoalPointList.SelectedItem);
            foreach (var labelScore in labelRemainPoints)
            {
                labelScore.Text = goalPoint.ToString();
            }
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

        private void BilliardsModuleEventLogger_OnGameStateReceived(object? sender, EventArgs args)
        {
            var billiardsModuleEventArgs = args as BilliardsModuleEventLoggerEventArgs;
            if (null == billiardsModuleEventArgs || null == billiardsModuleEventArgs.poolState)
            {
                return;
            }

            var poolState = billiardsModuleEventArgs.poolState;

            System.Diagnostics.Debug.WriteLine(poolState.dump());

            if (1 == poolState.turnStateSynced)
            {
                return;
            }

            var ballProcketedFlags = PoolState.ballProcketedFlags(poolState.ballsPocketedSynced);
            var foul = poolState.turnStateSynced == 2;
            updatePocketed(ballProcketedFlags, foul);
            //checkGameMode(ballProcketedFlags);
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
            if (null == billiardsModuleEventArgs || null == billiardsModuleEventArgs.intValue /*|| null == billiardsModuleEventArgs.intValue2 */)
            {
                return;
            }

            if (billiardsModuleEventArgs.intValue == 0)
            {
                changeTo8ballMode();
            }
            else if (billiardsModuleEventArgs.intValue == 1)
            {
                changeTo9ballMode();
            }
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

            if (teamMemberFixed)
            {
                return;
            }
                
            for (int i = 0; i < labelPlayers.Length && i < billiardsModuleEventArgs.players.Length; i++)
            {
                var player = billiardsModuleEventArgs.players[i];
                if (!string.IsNullOrEmpty(player))
                {
                    var labelPlayer = labelPlayers[i];
                    labelPlayer.Text = player;
                    labelPlayer.Visible = true;
                }
            }
        }

        private void BilliardsModuleEventLogger_OnRemoteGameStarted(object? sender, EventArgs args)
        {
            var billiardsModuleEventArgs = args as BilliardsModuleEventLoggerEventArgs;
            if (null == billiardsModuleEventArgs)
            {
                return;
            }

            teamMemberFixed = true;
            foreach (var labelPlayer in labelPlayers)
            {
                labelPlayer.BackColor = Color.FromKnownColor(KnownColor.Control);
                if (labelPlayer.Text == " ")
                {
                    labelPlayer.Visible = false;
                }
            }
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

        /*
        private void BilliardsModuleEventLogger_OnRemoteGameEnded(object? sender, EventArgs args)
        {
            var billiardsModuleEventArgs = args as BilliardsModuleEventLoggerEventArgs;
            if (null == billiardsModuleEventArgs || null == billiardsModuleEventArgs.intValue)
            {
                return;
            }

            // resetSubTotal();
        }
        */

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
                labelBall.Tag = (i - 8).ToString();
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
    }
}