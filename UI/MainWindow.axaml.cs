using System;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;

namespace GravityBalls.UI;

public partial class MainWindow : Window
{
    private Ellipse ball;
    private DispatcherTimer timer;
    private WorldModel world;

    public MainWindow()
    {
        InitializeComponent();
        PointerMoved += MainWindow_PointerMoved;
        PointerPressed += MainWindow_PointerPressed;
        Opened += (_, __) => OnLoad();
    }

    private void NewWorld()
    {
        world = CreateWorldModel();
        world.WorldHeight = ClientSize.Height;
        world.WorldWidth = ClientSize.Width;
    }

    private void CreateTimer()
    {
        timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(30) };
        timer.Tick += TimerOnTick;
        timer.Start();
    }

    private void OnLoad()
    {
        NewWorld ();

        var canvas = this.Find<Canvas>("Canvas");
        var size = world.BallRadius * 2;

        ball = new Ellipse
        {
            Fill = Brushes.GreenYellow,
            Width = size,
            Height = size,
        };

        canvas.Children.Add(ball);

        CreateTimer();

        ClientSizeProperty.Changed
            .Subscribe(_ => { OnResize(); });
    }

    private WorldModel CreateWorldModel()
    {
        var w = new WorldModel
        {
            WorldHeight = ClientSize.Height,
            WorldWidth = ClientSize.Width,
            BallRadius = 10
        };

        w.BallX = w.WorldHeight / 2;
        w.BallY = w.BallRadius;

        return w;
    }

    private void OnResize()
    {
        world.WorldHeight = ClientSize.Height;
        world.WorldWidth = ClientSize.Width;
    }

    private void TimerOnTick(object? sender, EventArgs eventArgs)
    {
        world.SimulateTimeframe(timer.Interval.Milliseconds / 1000d);
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        var size = world.BallRadius * 2;

        ball.Height = size;
        ball.Width = size;

        Canvas.SetLeft(ball, world.BallX - world.BallRadius);
        Canvas.SetTop(ball, world.BallY - world.BallRadius);
    }

    private void MainWindow_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        var position = e.GetPosition(this);
        world.SimulateCursorRepulsion(position);
        Title = $"Cursor ({position.X}, {position.Y})";
    }

    private void MainWindow_PointerPressed(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        var pointerProperties = e.GetCurrentPoint(this).Properties;
        if (pointerProperties.IsLeftButtonPressed)
        {
            world.BallJump();
        }

        if (pointerProperties.IsRightButtonPressed)
        {
            ball.Fill = Brushes.Beige;
        }
    }
}
