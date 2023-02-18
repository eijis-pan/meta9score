namespace meta9score
{
    public partial class ScoreForm : Form
    {
        private BilliardsModuleEventLogger billiardsModuleEventLogger;
        private ConfigDialog configDialog;

        private Label[] labelPlayers;
        private Label[] labelSinglePlayers;
        private Label[][] labelTeamPlayers;

        private Label[] labelCurrentScores;
        private Label[] labelRemainPoints;

        private Label[] labelBalls;
        private Label[] label9Balls;

        private int goalPoint = 120;

        private string? lastShotPlayer = null; 

        public ScoreForm()
        {
            InitializeComponent();

            labelPlayers = new Label[]
            {
                labelPlayer1, labelPlayer2, labelPlayer3, labelPlayer4
            };

            labelSinglePlayers = new Label[]
            {
                labelPlayer1, labelPlayer3
            };

            labelTeamPlayers = new Label[][]
            {
                new Label[]{ labelPlayer1, labelPlayer2 }, 
                new Label[]{ labelPlayer3, labelPlayer4 }
            };

            labelCurrentScores = new Label[]
            {
                labelCurrentScoreTeam1, labelCurrentScoreTeam2
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
            billiardsModuleEventLogger.OnGameStateReceived += BilliardsModuleEventLogger_OnGameStateReceived;
            billiardsModuleEventLogger.OnRemotePlayersChanged += BilliardsModuleEventLogger_OnRemotePlayersChanged;
            billiardsModuleEventLogger.OnRemoteTurnSimulate += BilliardsModuleEventLogger_OnRemoteTurnSimulate;
            //billiardsModuleEventLogger.OnRemoteBallsPocketedChanged += BilliardsModuleEventLogger_OnRemoteBallsPocketedChanged;
        }

        private void FormScore_Load(object sender, EventArgs e)
        {
            init();
        }

        private void init()
        {
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
        }

        private void scoreReset()
        {
            foreach (var labelScore in labelCurrentScores)
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

            updatePocketed(PoolState.ballProcketedFlags(billiardsModuleEventArgs.poolState.ballsPocketedSynced));
        }

        private void BilliardsModuleEventLogger_OnRemotePlayersChanged(object? sender, EventArgs args)
        {
            var billiardsModuleEventArgs = args as BilliardsModuleEventLoggerEventArgs;
            if (null == billiardsModuleEventArgs || null == billiardsModuleEventArgs.players)
            {
                return;
            }

            for (int i = 0; i < labelPlayers.Length && i < billiardsModuleEventArgs.players.Length; i++)
            {
                var player = billiardsModuleEventArgs.players[i];
                if (!string.IsNullOrEmpty(player))
                {
                    labelPlayers[i].Text = player;
                }
            }
        }

        private void BilliardsModuleEventLogger_OnRemoteTurnSimulate(object? sender, EventArgs args)
        {
            var billiardsModuleEventArgs = args as BilliardsModuleEventLoggerEventArgs;
            if (null == billiardsModuleEventArgs || null == billiardsModuleEventArgs.player)
            {
                return;
            }

            lastShotPlayer = billiardsModuleEventArgs.player;
        }

        private void BilliardsModuleEventLogger_OnRemoteBallsPocketedChanged(object? sender, EventArgs args)
        {
            var billiardsModuleEventArgs = args as BilliardsModuleEventLoggerEventArgs;
            if (null == billiardsModuleEventArgs || null == billiardsModuleEventArgs.ballProcketedFlags)
            {
                return;
            }

            updatePocketed(billiardsModuleEventArgs.ballProcketedFlags);
        }

        private void updatePocketed(bool[] ballProcketedFlags)
        {
            for (int i = 0; i < ballProcketedFlags.Length; i++)
            {
                if (0 < i && i <= labelBalls.Length)
                {
                    var labelBall = labelBalls[i - 1];
                    if (ballProcketedFlags[i])
                    {
                        if (labelBall.Parent == flowLayoutAvailBalls)
                        {
                            if (null != lastShotPlayer && null != labelBall.Tag)
                            {
                                scoreCountUp(int.Parse(labelBall.Tag.ToString()));
                            }
                        }
                        labelBall.Parent = flowLayoutPocketed;
                    }
                    else
                    {
                        labelBall.Parent = flowLayoutAvailBalls;
                    }
                }
            }
        }

        private void scoreCountUp(int point)
        {
            if (null == lastShotPlayer || 0 == point)
            {
                return;
            }

            Label? labelCurrentScore = null;
            Label? labelRemainPoint = null;
            for (int i = 0; i < labelTeamPlayers.Length; i++)
            {
                for (int j = 0; j < labelTeamPlayers[i].Length; j++)
                {
                    if (labelTeamPlayers[i][j].Text == lastShotPlayer)
                    {
                        labelCurrentScore = labelCurrentScores[i];
                        labelRemainPoint = labelRemainPoints[i];
                        break;
                    }
                }
                if (null != labelCurrentScore && null != labelRemainPoint)
                {
                    break;
                }
            }

            if (null == labelCurrentScore || null == labelRemainPoint)
            {
                return;
            }

            var total = int.Parse(labelCurrentScore.Text) + point;
            labelCurrentScore.Text = total.ToString();
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
    }
}