using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RPG_code_Productivity_RPG.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public enum StatType { Strength, Intellect, Social, Level }

        public enum ActivityDifficulty { Easy, Medium, Hard, Epic }

        public enum ActivityCategory { Sport, Literature, Communication }

        // Класс данных игрока
        public partial class PlayerData : ObservableObject
        {
            private int _currentLevel = 1;
            private int _currentExp = 0;
            private int _expToNextLevel = 100;
            private int _availableUpgradePoints = 0;
            private int _strengthPoints = 0;
            private int _intellectPoints = 0;
            private int _socialPoints = 0;
            private List<string> _unlockedAchievements = new List<string>();

            public int CurrentLevel { get => _currentLevel; set => SetProperty(ref _currentLevel, value); }
            public int CurrentExp { get => _currentExp; set => SetProperty(ref _currentExp, value); }
            public int ExpToNextLevel { get => _expToNextLevel; set => SetProperty(ref _expToNextLevel, value); }
            public int AvailableUpgradePoints { get => _availableUpgradePoints; set => SetProperty(ref _availableUpgradePoints, value); }
            public int StrengthPoints { get => _strengthPoints; set => SetProperty(ref _strengthPoints, value); }
            public int IntellectPoints { get => _intellectPoints; set => SetProperty(ref _intellectPoints, value); }
            public int SocialPoints { get => _socialPoints; set => SetProperty(ref _socialPoints, value); }
            public List<string> UnlockedAchievements { get => _unlockedAchievements; set => SetProperty(ref _unlockedAchievements, value); }

            public double ExpProgress => (double)CurrentExp / ExpToNextLevel;
            public int TotalSkillPoints => StrengthPoints + IntellectPoints + SocialPoints;

            public void AddExp(int exp)
            {
                CurrentExp += exp;
                while (CurrentExp >= ExpToNextLevel)
                {
                    CurrentExp -= ExpToNextLevel;
                    CurrentLevel++;
                    ExpToNextLevel = 100 * CurrentLevel;
                    AvailableUpgradePoints += 3;
                }
            }
        }

        // Класс задания
        public class Activity : ObservableObject
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public StatType RelatedStat { get; set; }
            public int ExpReward { get; set; }
            public int StatReward { get; set; }
            public ActivityDifficulty Difficulty { get; set; }
            public ActivityCategory Category { get; set; }
            private bool _isCompleted;
            public bool IsCompleted { get => _isCompleted; set => SetProperty(ref _isCompleted, value); }
            public DateTime? CompletedDate { get; set; }
            public int RequiredMinutes { get; set; }
        }

        public PlayerData CurrentPlayer { get; set; }
        public ObservableCollection<Activity> AllActivities { get; set; }
        public ObservableCollection<Activity> SportActivities { get; set; }
        public ObservableCollection<Activity> LiteratureActivities { get; set; }
        public ObservableCollection<Activity> CommunicationActivities { get; set; }

        // Команды для улучшения навыков
        public ICommand UpgradeStrengthCommand { get; }
        public ICommand UpgradeIntellectCommand { get; }
        public ICommand UpgradeSocialCommand { get; }

        // Команды для выполнения заданий
        public ICommand CompleteSportActivityCommand { get; }
        public ICommand CompleteLiteratureActivityCommand { get; }
        public ICommand CompleteCommunicationActivityCommand { get; }

        // Свойства для управления кнопками улучшения
        public bool CanUpgradeStrength => CurrentPlayer.AvailableUpgradePoints > 0;
        public bool CanUpgradeIntellect => CurrentPlayer.AvailableUpgradePoints > 0;
        public bool CanUpgradeSocial => CurrentPlayer.AvailableUpgradePoints > 0;

        public MainWindowViewModel()
        {
            CurrentPlayer = new PlayerData();
            AllActivities = new ObservableCollection<Activity>();
            SportActivities = new ObservableCollection<Activity>();
            LiteratureActivities = new ObservableCollection<Activity>();
            CommunicationActivities = new ObservableCollection<Activity>();

            InitializeActivities();

            // Инициализация команд
            UpgradeStrengthCommand = new RelayCommand(UpgradeStrength);
            UpgradeIntellectCommand = new RelayCommand(UpgradeIntellect);
            UpgradeSocialCommand = new RelayCommand(UpgradeSocial);

            CompleteSportActivityCommand = new RelayCommand<Activity>(activity => CompleteActivity(activity));
            CompleteLiteratureActivityCommand = new RelayCommand<Activity>(activity => CompleteActivity(activity));
            CompleteCommunicationActivityCommand = new RelayCommand<Activity>(activity => CompleteActivity(activity));
        }

        private void UpgradeStrength()
        {
            if (CurrentPlayer.AvailableUpgradePoints > 0)
            {
                CurrentPlayer.StrengthPoints++;
                CurrentPlayer.AvailableUpgradePoints--;
                OnPropertyChanged(nameof(CanUpgradeStrength));
                OnPropertyChanged(nameof(CanUpgradeIntellect));
                OnPropertyChanged(nameof(CanUpgradeSocial));
                CheckAchievements();
            }
        }

        private void UpgradeIntellect()
        {
            if (CurrentPlayer.AvailableUpgradePoints > 0)
            {
                CurrentPlayer.IntellectPoints++;
                CurrentPlayer.AvailableUpgradePoints--;
                OnPropertyChanged(nameof(CanUpgradeStrength));
                OnPropertyChanged(nameof(CanUpgradeIntellect));
                OnPropertyChanged(nameof(CanUpgradeSocial));
                CheckAchievements();
            }
        }

        private void UpgradeSocial()
        {
            if (CurrentPlayer.AvailableUpgradePoints > 0)
            {
                CurrentPlayer.SocialPoints++;
                CurrentPlayer.AvailableUpgradePoints--;
                OnPropertyChanged(nameof(CanUpgradeStrength));
                OnPropertyChanged(nameof(CanUpgradeIntellect));
                OnPropertyChanged(nameof(CanUpgradeSocial));
                CheckAchievements();
            }
        }

        private void CompleteActivity(Activity activity)
        {
            if (activity.IsCompleted) return;

            // Начисляем опыт
            CurrentPlayer.AddExp(activity.ExpReward);

            // Начисляем очки навыков
            switch (activity.RelatedStat)
            {
                case StatType.Strength:
                    CurrentPlayer.StrengthPoints += activity.StatReward;
                    break;
                case StatType.Intellect:
                    CurrentPlayer.IntellectPoints += activity.StatReward;
                    break;
                case StatType.Social:
                    CurrentPlayer.SocialPoints += activity.StatReward;
                    break;
            }

            activity.IsCompleted = true;
            activity.CompletedDate = DateTime.Now;

            OnPropertyChanged(nameof(CanUpgradeStrength));
            OnPropertyChanged(nameof(CanUpgradeIntellect));
            OnPropertyChanged(nameof(CanUpgradeSocial));

            CheckAchievements();
        }

        private void CheckAchievements()
        {
            var achievements = new List<string>();

            // Достижения по уровням
            if (CurrentPlayer.CurrentLevel >= 5 && !CurrentPlayer.UnlockedAchievements.Contains("Мастер 5-го уровня"))
                achievements.Add("🏆 Мастер 5-го уровня");
            if (CurrentPlayer.CurrentLevel >= 10 && !CurrentPlayer.UnlockedAchievements.Contains("Герой 10-го уровня"))
                achievements.Add("🏆 Герой 10-го уровня");

            // Достижения по навыкам
            if (CurrentPlayer.StrengthPoints >= 50 && !CurrentPlayer.UnlockedAchievements.Contains("Непоколебимая сила"))
                achievements.Add("💪 Непоколебимая сила");
            if (CurrentPlayer.IntellectPoints >= 50 && !CurrentPlayer.UnlockedAchievements.Contains("Великий мыслитель"))
                achievements.Add("🧠 Великий мыслитель");
            if (CurrentPlayer.SocialPoints >= 50 && !CurrentPlayer.UnlockedAchievements.Contains("Душа компании"))
                achievements.Add("💬 Душа компании");

            // Достижение за баланс
            if (CurrentPlayer.StrengthPoints >= 30 && CurrentPlayer.IntellectPoints >= 30 &&
                CurrentPlayer.SocialPoints >= 30 && !CurrentPlayer.UnlockedAchievements.Contains("Гармоничная личность"))
            {
                achievements.Add("⚖️ Гармоничная личность");
                CurrentPlayer.AvailableUpgradePoints += 5;
                OnPropertyChanged(nameof(CanUpgradeStrength));
            }

            foreach (var achievement in achievements)
            {
                CurrentPlayer.UnlockedAchievements.Add(achievement);
            }
        }

        private void InitializeActivities()
        {
            // Спортивные задания
            var sports = new List<Activity>
            {
                new Activity { Id = "s1", Title = "Утренняя зарядка", ExpReward = 25, StatReward = 5,
                              RelatedStat = StatType.Strength, Category = ActivityCategory.Sport, Difficulty = ActivityDifficulty.Easy },
                new Activity { Id = "s2", Title = "Пробежка 3 км", ExpReward = 50, StatReward = 10,
                              RelatedStat = StatType.Strength, Category = ActivityCategory.Sport, Difficulty = ActivityDifficulty.Medium },
                new Activity { Id = "s3", Title = "Силовая тренировка", ExpReward = 75, StatReward = 15,
                              RelatedStat = StatType.Strength, Category = ActivityCategory.Sport, Difficulty = ActivityDifficulty.Hard }
            };

            // Литературные задания
            var literatures = new List<Activity>
            {
                new Activity { Id = "l1", Title = "Чтение 15 минут", ExpReward = 20, StatReward = 5,
                              RelatedStat = StatType.Intellect, Category = ActivityCategory.Literature, Difficulty = ActivityDifficulty.Easy },
                new Activity { Id = "l2", Title = "Чтение главы книги", ExpReward = 40, StatReward = 10,
                              RelatedStat = StatType.Intellect, Category = ActivityCategory.Literature, Difficulty = ActivityDifficulty.Medium },
                new Activity { Id = "l3", Title = "Изучение нового навыка", ExpReward = 60, StatReward = 12,
                              RelatedStat = StatType.Intellect, Category = ActivityCategory.Literature, Difficulty = ActivityDifficulty.Medium }
            };

            // Задания на общение
            var communications = new List<Activity>
            {
                new Activity { Id = "c1", Title = "Звонок другу", ExpReward = 30, StatReward = 8,
                              RelatedStat = StatType.Social, Category = ActivityCategory.Communication, Difficulty = ActivityDifficulty.Easy },
                new Activity { Id = "c2", Title = "Новое знакомство", ExpReward = 75, StatReward = 15,
                              RelatedStat = StatType.Social, Category = ActivityCategory.Communication, Difficulty = ActivityDifficulty.Medium },
                new Activity { Id = "c3", Title = "Встреча с друзьями", ExpReward = 60, StatReward = 12,
                              RelatedStat = StatType.Social, Category = ActivityCategory.Communication, Difficulty = ActivityDifficulty.Medium }
            };

            AddActivities(sports, SportActivities);
            AddActivities(literatures, LiteratureActivities);
            AddActivities(communications, CommunicationActivities);
        }

        private void AddActivities(List<Activity> activities, ObservableCollection<Activity> collection)
        {
            foreach (var activity in activities)
            {
                collection.Add(activity);
                AllActivities.Add(activity);
            }
        }
    }
}