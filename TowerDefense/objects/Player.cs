
namespace TowerDefense.objects
{
    class Player
    {
        private int _gold;
        private int _score;
        private int _wave;
        private int _maxwave;
        private int _lives;
        private bool _gameOver;

        public Player(int gold, int score, int wave, int maxwave, int lives)
        {
            Gold = gold;
            Score = score;
            CurrentWave = wave;
            _maxwave = maxwave;
            Lives = lives;
            _gameOver = false;
        }
        public int Gold
        {
            get
            {
                return _gold;
            }

            set
            {
                if (value >= 0) _gold = value;
                else _gold = 0;
            }
        }

        public int Score
        {
            get
            {
                return _score;
            }

            set
            {
                if (value >= 0) _score = value;
                else _score = 0;
            }
        }

        public int CurrentWave
        {

            get
            {
                return _wave;
            }
            set
            {
                if (value >= 0 && value<=_maxwave) _wave = value;
                else _wave = 0;
            }
        }

        public int MaxWave
        {
            get { return _maxwave; }
        }

        public int Lives
        {
            get
            {
                return _lives;
            }

            set
            {
                if (value >= 0) _lives = value;
                else _lives = 0;
            }
        }

        public bool GameOver
        {
            get
            {
                return _gameOver;
            }

            set
            {
                _gameOver = value;
            }
        }

        public void AddScore(int score)
        {
            _score += score;
        }

        public void RemoveLife()
        {
            Lives -= 1;
        }

        public bool IsDead()
        {
            return _lives <= 0;
        }

        public void NextWave()
        {
            _wave++;
        }

        public void AddGold(int gold)
        {
            _gold += gold;
        }

        public bool PayGold(int cost)
        {
            if(cost <= _gold)
            {
                _gold -= cost;
                return true;
            }
            return false;
        }

    }
}
