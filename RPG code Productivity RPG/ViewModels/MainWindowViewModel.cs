using System.Collections.Generic;

namespace RPG_code_Productivity_RPG.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public enum StatType { Strength, Intellect, Social, Level }

        // Основные данные игрока (Сериализуемые)
        [System.Serializable]
        public class PlayerData
        {
            public int currentLevel = 1;
            public int currentExp = 0;
            public int expToNextLevel = 100;

            // Очки развития (закрытая валюта)
            public int availableUpgradePoints = 0;

            // Статистика по направлениям (для карты прокачки)
            public int strengthPoints = 0;
            public int intellectPoints = 0;
            public int socialPoints = 0;

            // Достижения (список ID полученных ачивок)
            public List<string> unlockedAchievements = new List<string>();
        }

        // Данные для конкретной задачи (Тренировка, Книга, Встреча)
        [System.Serializable]
        public class Activity
        {
            public string id;
            public string title;
            public StatType relatedStat; // К какому навыку относится
            public int expReward;         // Опыт для уровня
            public int statReward;        // Очки для карты прокачки (конкретной ветки)
            //public Sprite icon;
        }
    }
}
