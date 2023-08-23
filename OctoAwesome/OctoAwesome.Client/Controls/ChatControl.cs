using engenious;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using System;

namespace OctoAwesome.Client.Controls;


internal class ChatControl : ContainerControl
{
    private readonly Textbox textBox;
    private readonly StackPanel textStack;
    private readonly StackPanel autoCompletionsStack;
    private readonly ScrollContainer textScroll;
    private readonly ScrollContainer autoCompletionsScroll;
    private readonly ITypeContainer typeContainer;
    private readonly IUpdateHub updateHub;
    private readonly Pool<ChatNotification> chatMessagePool;
    private readonly IDisposable messageSubscription;
    private bool shouldScrollDown;

    private GameTime currentGameTime;
    private GameTime lastReceived;

    public ChatControl()
    {
        typeContainer = TypeContainer.Get<ITypeContainer>();
        updateHub = typeContainer.Get<IUpdateHub>();
        chatMessagePool = typeContainer.Get<Pool<ChatNotification>>();

        messageSubscription = updateHub.ListenOn(DefaultChannels.Chat).Subscribe(ChatMessageReceived);

        textBox = new Textbox()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = new BorderBrush(Color.Black * 0.7f),
            TextColor = Color.White,
            Visible = false
        };

        textBox.KeyDown += TextBox_KeyDown;

        textStack = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Bottom
        };

        textScroll = new ScrollContainer
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        textScroll.Content = textStack;
        autoCompletionsStack = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Bottom
        };
        autoCompletionsScroll = new ScrollContainer
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        autoCompletionsScroll.Content = autoCompletionsStack;
        autoCompletionsScroll.Visible = false;
        var mainGrid = new Grid
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = new BorderBrush(Color.Gray * 0.7f)
        };

        mainGrid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
        mainGrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.FitParts, Height = 1 });
        mainGrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto });

        mainGrid.AddControl(textScroll, 0, 0);
        mainGrid.AddControl(autoCompletionsScroll, 0, 0);
        mainGrid.AddControl(textBox, 0, 1);
        Controls.Add(mainGrid);

        HorizontalAlignment = HorizontalAlignment.Left;
        VerticalAlignment = VerticalAlignment.Bottom;
        Height = 200;
        Width = 500;
        Visible = false;
    }

    private void ChatMessageReceived(object obj)
    {
        if (obj is not ChatNotification notification)
            return;

        AddTextMessage($"[{notification.TimeStamp:HH:mm:ss}] {notification.Username}: {notification.Text}");
    }

    public void AddTextMessage(string message)
    {
        lastReceived = currentGameTime;
        Visible = true;

        var label = new Label
        {
            Text = message,
            HorizontalAlignment = HorizontalAlignment.Left,
            TextColor = Color.White
        };
        textStack.Controls.Add(label);
        shouldScrollDown = true;
    }

    public void Activate()
    {
        if (!Visible)
        {
            Visible = true;
            textBox.Visible = true;
        }
        else
        {
            textBox.Visible = true;
            ScreenManager.FreeMouse();
            textBox.Focus();
        }
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        currentGameTime = gameTime;

        base.OnUpdate(gameTime);
        if (shouldScrollDown)
        {
            shouldScrollDown = false;
            textScroll.VerticalScrollPosition = int.MaxValue;
        }

        if (Visible && textBox.Focused != TreeState.Active && lastReceived.TotalGameTime.Add(TimeSpan.FromSeconds(5)) <= gameTime.TotalGameTime)
        {
            Visible = false;
        }
    }
    private void TextBox_KeyDown(Control sender, KeyEventArgs args)
    {
        if (args.Key == Keys.Enter && Visible && !string.IsNullOrWhiteSpace(textBox.Text))
        {
            var notification = chatMessagePool.Rent();
            notification.Text = textBox.Text;
            notification.Username = "jvbsl";
            notification.TimeStamp = DateTimeOffset.Now;

            updateHub.PushNetwork(notification, DefaultChannels.Chat);
            chatMessagePool.Return(notification);
            textBox.Text = "";
        }
    }

    protected override void OnVisibleChanged(PropertyEventArgs<bool> args)
    {
        if (args.NewValue)
        {
            ScreenManager.FreeMouse();
            textBox.Focus();
        }
        else
        {
            ScreenManager.CaptureMouse();
            textBox.Unfocus();
        }
        base.OnVisibleChanged(args);
    }

    protected override void OnKeyDown(KeyEventArgs args)
    {
        if (Visible && textBox.Visible)
        {
            args.Handled = true;
            if (args.Key == Keys.Escape)
            {
                ScreenManager.CaptureMouse();
                textBox.Unfocus();
                textBox.Visible = false;
            }
        }
        base.OnKeyDown(args);
    }
}
