using System;

namespace FlappyPlane
{
    public static class Score
    {
        public static Action<int> OnScoreChange;
        private static int currentPoints;

        public static int CurrentScore
        {
            get => currentPoints;
            set
            {
                currentPoints = value;
                OnScoreChange?.Invoke(CurrentScore);
            }
        }

        public static void AddPoints(int value)
        {
            if (value <= 0)
                return;
            CurrentScore += value;
        }

        public static void SubtractPoints(int value)
        {
            if (value <= 0)
                return;
            CurrentScore -= value;
        }

        internal static void ResetScore()
        {
            SubtractPoints(CurrentScore);
        }
    }
}