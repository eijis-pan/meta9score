using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace meta9score
{
    public class PoolState
    {
        private const int MAX_BALLS = 16;

        public Vector3[] ballsPSynced = new Vector3[MAX_BALLS];

        public Vector3 cueBallVSynced;
        public Vector3 cueBallWSynced;
        public UInt16 ballsPocketedSynced;
        public byte teamIdSynced;
        public byte repositionStateSynced;
        public bool isTableOpenSynced;
        public byte teamColorSynced;
        public byte turnStateSynced;
        public byte gameModeSynced;
        public uint timerSynced;
        public bool teamsSynced;
        public int[] fourBallScoresSynced = new int[2];
        public byte fourBallCueBallSynced;

        public static int[] bitToBallNumber = new int[16]
        {
            0, 8, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15
        };

        /// <summary>
        /// 16bitのpocketedフラグをフラグOnのbit番号のint配列にして返す
        /// </summary>
        /// <param name="ballsPocketed"></param>
        /// <returns></returns>
        public static int[] pocketedBalls(UInt16 ballsPocketed)
        {
            var ballsPocketedList = new List<int>();
            for (int i = 0; i < 16; i++)
            {
                var pocketed = ballsPocketed >> i & 0x1;
                if (0 != pocketed)
                {
                    ballsPocketedList.Add(bitToBallNumber[i]);
                }
            }
            ballsPocketedList.Sort();
            return ballsPocketedList.ToArray();
        }

        /// <summary>
        /// 16bitのpocketedフラグをbool配列にして返す
        /// </summary>
        /// <param name="ballsPocketed"></param>
        /// <returns></returns>
        public static bool[] ballProcketedFlags(UInt16 ballsPocketed)
        {
            var ballProcketedFlags = new bool[16];
            for (int i = 0; i < 16; i++)
            {
                var pocketed = ballsPocketed >>i & 0x1;
                var ballNumber = bitToBallNumber[i];
                ballProcketedFlags[ballNumber] = (pocketed != 0);
            }
            return ballProcketedFlags;
        }

        public PoolState(string gameStateStr)
        {
            if (!this.decordState(gameStateStr))
            {
                throw new ArgumentException();
            }
        }

        public string dump()
        {
            return string.Format(
                "ballsPocketed={2} teamId={3} repositionState={4} isTableOpen={5} teamColor={6} turnState={7} gameMode={8} timer={9} teams={10} fourBallScores={11},{12} fourBallCueBall={13}, cueBallV={0} cueBallW={1}",
                cueBallVSynced,
                cueBallWSynced,
                Convert.ToString(ballsPocketedSynced, 2).PadLeft(16, '0'),
                teamIdSynced,
                repositionStateSynced,
                isTableOpenSynced,
                teamColorSynced,
                turnStateSynced,
                gameModeSynced,
                timerSynced,
                teamsSynced,
                fourBallScoresSynced[0],
                fourBallScoresSynced[1],
                fourBallCueBallSynced
            );
        }

        private bool decordState(string gameStateStr)
        {
            try
            {
                var gameState = System.Convert.FromBase64String(gameStateStr);

                if (gameState.Length != 0x7a) return false;

                for (int i = 0; i < 16; i++)
                {
                    ballsPSynced[i] = decodeVec3Full(gameState, i * 6, 2.5f);
                }
                cueBallVSynced = decodeVec3Full(gameState, 0x60, 50.0f);
                cueBallWSynced = decodeVec3Part(gameState, 0x66, 500.0f);

                ballsPocketedSynced = decodeU16(gameState, 0x6C);
                teamIdSynced = gameState[0x6E];
                repositionStateSynced = gameState[0x6F];
                isTableOpenSynced = gameState[0x70] != 0;
                teamColorSynced = gameState[0x71];
                turnStateSynced = gameState[0x72];
                gameModeSynced = gameState[0x73];
                timerSynced = decodeU16(gameState, 0x74);
                teamsSynced = gameState[0x76] != 0;
                fourBallScoresSynced[0] = gameState[0x77];
                fourBallScoresSynced[1] = gameState[0x78];
                fourBallCueBallSynced = gameState[0x79];
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        private const float I16_MAXf = 32767.0f;

        private ushort decodeU16(byte[] data, int pos)
        {
            return (ushort)(data[pos] | (((uint)data[pos + 1]) << 8));
        }

        private Vector3 decodeVec3Part(byte[] data, int start, float range)
        {
            ushort _x = decodeU16(data, start);
            ushort _y = decodeU16(data, start + 2);

            float x = ((_x - I16_MAXf) / I16_MAXf) * range;
            float y = ((_y - I16_MAXf) / I16_MAXf) * range;

            return new Vector3(x, 0.0f, y);
        }

        private Vector3 decodeVec3Full(byte[] data, int start, float range)
        {
            ushort _x = decodeU16(data, start);
            ushort _y = decodeU16(data, start + 2);
            ushort _z = decodeU16(data, start + 4);

            float x = ((_x - I16_MAXf) / I16_MAXf) * range;
            float y = ((_y - I16_MAXf) / I16_MAXf) * range;
            float z = ((_z - I16_MAXf) / I16_MAXf) * range;

            return new Vector3(x, y, z);
        }
    }
}
