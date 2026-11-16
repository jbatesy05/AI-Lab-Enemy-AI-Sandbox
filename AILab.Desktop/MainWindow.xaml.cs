using AILab.Core.Simulation;
using AILab.Core.Simulation.Entities;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Collections.Generic;

namespace AILab.Desktop
{
    public partial class MainWindow : Window
    {
        private readonly SimulationEngine _engine;
        private readonly Player _player;

        private readonly DispatcherTimer _timer;
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        private double _lastUpdateTime;

        public MainWindow()
        {
            InitializeComponent();

            _player = new Player(new Vector2(8, 0));
            _playerTargetPosition = _player.Position;

            Enemy enemy1 = new Enemy(
                new Vector2(-8, 0), 2f, 8f, 2f
            );

            Enemy enemy2 = new Enemy(
                new Vector2(0, 5), 2f, 10f, 2f
            );

            EnemySimulation simulation1 = new EnemySimulation(
                enemy1, _player
            );

            EnemySimulation simulation2 = new EnemySimulation(
                enemy2, _player
            );

            _engine = new SimulationEngine(simulation1, "Patroller");

            _engine.AddEnemy(simulation2, "Chaser");

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };

            _timer.Tick += UpdateSimulation;

            _timer.Start();

            Closed += (sender, args) => _timer.Stop(); }
            private Vector2 _playerTargetPosition;
            private Vector2 _mouseWorldPosition;

            private bool _isMouseOverArena;

        private int _spawnedEnemyCount = 2;
        private void SimulationCanvas_MouseLeftButtonDown(
    object sender,
    MouseButtonEventArgs e)
        {
            Point clickPosition = e.GetPosition(SimulationCanvas);

            _playerTargetPosition = ScreenToWorld(clickPosition);

            _mouseWorldPosition = _playerTargetPosition;
            _isMouseOverArena = true;
        }
        private void SimulationCanvas_MouseMove(
    object sender,
    MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(SimulationCanvas);

            _mouseWorldPosition = ScreenToWorld(mousePosition);
            _isMouseOverArena = true;
        }

        private void SimulationCanvas_MouseLeave(
            object sender,
            MouseEventArgs e)
        {
            _isMouseOverArena = false;
        }

        private Vector2 ScreenToWorld(Point position)
        {
            float worldX = (float)(
                position.X / SimulationCanvas.Width * 20 - 10
            );

            float worldY = (float)(
                10 - position.Y / SimulationCanvas.Height * 20
            );

            return new Vector2(worldX, worldY);
        }
        private void Window_PreviewKeyDown(
    object sender,
    KeyEventArgs e)
        {
            if (e.Key == Key.Space && _isMouseOverArena)
            {
                SpawnEnemy();
                e.Handled = true;
            }
        }
        private void SpawnEnemy()
        {
            Vector2 spawnPosition = FindSpawnPosition(
                _mouseWorldPosition
            );

            Enemy enemy = new Enemy(
                spawnPosition,
                2f,
                8f,
                2f
            );

            EnemySimulation simulation = new EnemySimulation(
                enemy,
                _player
            );

            _spawnedEnemyCount++;

            _engine.AddEnemy(
                simulation,
                $"Enemy {_spawnedEnemyCount}"
            );
        }

        private Vector2 FindSpawnPosition(Vector2 desiredPosition)
        {
            const float minimumDistance = 1.5f;

            bool IsPositionAvailable(Vector2 position)
            {
                foreach (EnemyAgent agent in _engine.Agents)
                {
                    float distance = Vector2.Distance(
                        position,
                        agent.Simulation.Enemy.Position
                    );

                    if (distance < minimumDistance)
                    {
                        return false;
                    }
                }

                return true;
            }

            if (IsPositionAvailable(desiredPosition))
            {
                return desiredPosition;
            }

            // Search increasingly larger circles around the cursor.
            for (int ring = 1; ring <= 10; ring++)
            {
                float radius = ring * minimumDistance;

                for (int step = 0; step < 16; step++)
                {
                    float angle = step * MathF.Tau / 16;

                    Vector2 candidate = new Vector2(
                        Math.Clamp(
                            desiredPosition.X + MathF.Cos(angle) * radius,
                            -9f,
                            9f
                        ),
                        Math.Clamp(
                            desiredPosition.Y + MathF.Sin(angle) * radius,
                            -9f,
                            9f
                        )
                    );

                    if (IsPositionAvailable(candidate))
                    {
                        return candidate;
                    }
                }
            }

            // Fallback if the arena is too crowded.
            return desiredPosition;
        }
        private void DrawPatrolRoute(EnemyAgent agent)
        {
            IReadOnlyList<Vector2> points = agent.Simulation.PatrolPoints;

            if (points.Count < 2)
                return;

            // Draw a line connecting each patrol point.
            for (int i = 0; i < points.Count - 1; i++)
            {
                Point start = new Point(
                    (points[i].X + 10) / 20 * SimulationCanvas.Width,
                    (10 - points[i].Y) / 20 * SimulationCanvas.Height
                );

                Point end = new Point(
                    (points[i + 1].X + 10) / 20 * SimulationCanvas.Width,
                    (10 - points[i + 1].Y) / 20 * SimulationCanvas.Height
                );

                Line routeLine = new Line
                {
                    X1 = start.X,
                    Y1 = start.Y,
                    X2 = end.X,
                    Y2 = end.Y,
                    Stroke = Brushes.LightSkyBlue,
                    StrokeThickness = 2,
                    Opacity = 0.5,
                    StrokeDashArray = new DoubleCollection { 4, 4 }
                };

                SimulationCanvas.Children.Add(routeLine);
            }

            // Draw a marker at each patrol point.
            foreach (Vector2 patrolPoint in points)
            {
                double x = (patrolPoint.X + 10) / 20 * SimulationCanvas.Width;
                double y = (10 - patrolPoint.Y) / 20 * SimulationCanvas.Height;

                Ellipse marker = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.LightSkyBlue,
                    Stroke = Brushes.White,
                    StrokeThickness = 1
                };

                Canvas.SetLeft(marker, x - 5);
                Canvas.SetTop(marker, y - 5);

                SimulationCanvas.Children.Add(marker);
            }
        }
        private void UpdateSimulation(object? sender, EventArgs e)
        {
            double currentTime = _stopwatch.Elapsed.TotalSeconds;

            float deltaTime = (float)(
                currentTime - _lastUpdateTime
            );

            _lastUpdateTime = currentTime;

            if (!_engine.IsPaused)
            {
                _player.MoveTowards(
                    _playerTargetPosition,
                    deltaTime * _engine.SimulationSpeed
                );
            }

            _engine.Update(deltaTime);

            DrawSimulation();
            UpdateAgentStatus();
        }
        private void DrawSimulation()
        {
            SimulationCanvas.Children.Clear();

            // Draw enemy detection and attack ranges first
            foreach (EnemyAgent agent in _engine.Agents)
            {
                DrawPatrolRoute(agent);
                Enemy enemy = agent.Simulation.Enemy;

                DrawRange(
                    enemy.Position,
                    enemy.DetectionRange,
                    Brushes.Gold,
                    2
                );

                DrawRange(
                    enemy.Position,
                    enemy.AttackRange,
                    Brushes.OrangeRed,
                    2
                );
            }

            // Draw player
            DrawCircle(
                _player.Position,
                Brushes.DeepSkyBlue,
                "PLAYER",
                12
            );

            // Draw enemies
            foreach (EnemyAgent agent in _engine.Agents)
            {
                Brush enemyColour = agent.CurrentStatus switch
                {
                    AILab.Core.BehaviourTree.NodeStatus.Success
                        => Brushes.OrangeRed,

                    AILab.Core.BehaviourTree.NodeStatus.Running
                        => Brushes.LimeGreen,

                    _ => Brushes.Gray
                };

                DrawCircle(
                    agent.Simulation.Enemy.Position,
                    enemyColour,
                    agent.Name,
                    10
                );
            }
        }

        private void DrawRange(
            Vector2 position,
            float range,
            Brush colour,
            double thickness)
        {
            double scaleX = SimulationCanvas.Width / 20;
            double scaleY = SimulationCanvas.Height / 20;

            double x = (position.X + 10) * scaleX;
            double y = (10 - position.Y) * scaleY;

            Ellipse rangeCircle = new Ellipse
            {
                Width = range * 2 * scaleX,
                Height = range * 2 * scaleY,
                Stroke = colour,
                StrokeThickness = thickness,
                Opacity = 0.45,
                Fill = Brushes.Transparent
            };

            Canvas.SetLeft(rangeCircle, x - range * scaleX);
            Canvas.SetTop(rangeCircle, y - range * scaleY);

            SimulationCanvas.Children.Add(rangeCircle);
        }

        private void DrawCircle(
            Vector2 position,
            Brush colour,
            string label,
            double radius)
        {
            double x = (position.X + 10) / 20 * SimulationCanvas.Width;
            double y = (10 - position.Y) / 20 * SimulationCanvas.Height;

            Ellipse circle = new Ellipse
            {
                Width = radius * 2,
                Height = radius * 2,
                Fill = colour
            };

            Canvas.SetLeft(circle, x - radius);
            Canvas.SetTop(circle, y - radius);

            SimulationCanvas.Children.Add(circle);

            TextBlock text = new TextBlock
            {
                Text = label,
                Foreground = Brushes.White,
                FontSize = 12,
                FontWeight = FontWeights.Bold
            };

            Canvas.SetLeft(text, x + radius + 5);
            Canvas.SetTop(text, y - 8);

            SimulationCanvas.Children.Add(text);
        }


        private void UpdateAgentStatus()
        {
            AgentStatusPanel.Children.Clear();

            foreach (EnemyAgent agent in _engine.Agents)
            {
                string behaviour = agent.Simulation.Blackboard
                    .GetValue<string>("CurrentBehaviour")
                    ?? "Initialising";

                TextBlock status = new TextBlock
                {
                    Text = $"{agent.Name}: {behaviour}",
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 3, 0, 3)
                };

                AgentStatusPanel.Children.Add(status);
            }
        }
        private void PauseButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            _engine.IsPaused = !_engine.IsPaused;

            PauseButton.Content = _engine.IsPaused
                ? "Resume"
                : "Pause";
        }

        private void SpeedSlider_ValueChanged(
            object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if (_engine == null)
            {
                return;
            }

            _engine.SimulationSpeed = (float)SpeedSlider.Value;

            SpeedLabel.Text =
                $"Simulation Speed: {SpeedSlider.Value:0.00}x";
        }
    }
}
