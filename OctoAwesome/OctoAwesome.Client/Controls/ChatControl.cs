using System;
using engenious;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;

namespace OctoAwesome.Client.Controls;

internal class ChatControl : ContainerControl
{
    private readonly Textbox textBox;
    private readonly StackPanel textStack;
    private readonly StackPanel autoCompletionsStack;
    private readonly ScrollContainer textScroll;
    private readonly ScrollContainer autoCompletionsScroll;
    private readonly ITypeContainer typeContainer;
    private bool shouldScrollDown;

    private readonly CommandParser commandParser;

    public ChatControl()
    {
        commandParser = new CommandParser();
        typeContainer = TypeContainer.Get<ITypeContainer>();
        textBox = new Textbox()
                  {
                      HorizontalAlignment = HorizontalAlignment.Stretch,
                      Background = new BorderBrush(Color.Black * 0.7f),
                      TextColor = Color.White
                  };

        textBox.KeyDown += TextBox_KeyDown;
        textBox.TextChanged += TextBox_Changed;

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

        // mainGrid.AddControl(textScroll, 0, 0);
        mainGrid.AddControl(autoCompletionsScroll, 0, 0);
        mainGrid.AddControl(textBox, 0, 1);
        Controls.Add(mainGrid);

        HorizontalAlignment = HorizontalAlignment.Left;
        VerticalAlignment = VerticalAlignment.Bottom;
        Height = 200;
        Width = 500;
        Visible = false;
    }

    private bool ShowAutoCompletions
    {
        get => autoCompletionsScroll.Visible;
        set
        {
            autoCompletionsScroll.Visible = value;
            textScroll.Visible = !value;
        }
    }

    public void AddTextMessage(string message)
    {
        var label = new Label { 
                                           Text = message,
                                           HorizontalAlignment = HorizontalAlignment.Left,
                                           TextColor = Color.White
                                       };
        textStack.Controls.Add(label);
        shouldScrollDown = true;
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        base.OnUpdate(gameTime);
        if(shouldScrollDown)
        {
            shouldScrollDown = false;
            textScroll.VerticalScrollPosition = int.MaxValue;
        }
    }
    private void TextBox_KeyDown(Control sender, KeyEventArgs args)
    {
        if(args.Key == Keys.Enter && Visible && !string.IsNullOrWhiteSpace(textBox.Text))
        {
            if (textBox.Text.StartsWith("/"))
            {
                var command = textBox.Text[1..];
                ExecuteCommand(command);
            }
            else
            {
                OnMessage?.Invoke(textBox.Text);
            }
            textBox.Text = "";
        }
        else 
        {
        }
    }
    
    private void TextBox_Changed(Control sender, PropertyEventArgs<string> args)
    {
        
        
        RefreshAutoCompletions(args.NewValue!);
    }
    

    private void ExecuteCommand(string command)
    {
        var parsed = commandParser.ParseCommand(command, true);
        if (!parsed.WasSuccessful)
        {
            OnMessage?.Invoke(command);
            OnMessage?.Invoke(parsed.ToString() ?? "error executing command");
        }
        else
        {
            OnMessage?.Invoke($"= {parsed.Value}");
        }
    }

    private void RefreshAutoCompletions(string text)
    {
        autoCompletionsStack.Controls.Clear();
        
        ShowAutoCompletions = true;
        if (text.StartsWith("/"))
        {
            var command = text[1..];
            var parsedForCompletion = commandParser.ParseCommand(command, false);
            if (!parsedForCompletion.WasSuccessful)
            {
                Console.WriteLine("Could not parse for autocomplete");
            }
            else
            {
                var av = new AutoCompleteVisitor(typeContainer);
                var suggestions = av.Visit(parsedForCompletion.Value);
                if (suggestions.PossibleCompletions is not null)
                {
                    Console.WriteLine($"-> {command}");
                    foreach (var s in suggestions.PossibleCompletions)
                    {
                        autoCompletionsStack.Controls.Add(new Label{ 
                                                                       Text = s,
                                                                       HorizontalAlignment = HorizontalAlignment.Left,
                                                                       TextColor = Color.White
                                                                   });
                        Console.WriteLine(s);
                    }
                }
            }
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
        if (Visible)
        {
            args.Handled = true;
            if (args.Key == Keys.Escape || args.Key == Keys.Enter && string.IsNullOrEmpty(textBox.Text))
            {
                Visible = false;
            }
        }
        base.OnKeyDown(args);
    }

    public event MessageEventArgs? OnMessage;

    public delegate void MessageEventArgs(string message);
}