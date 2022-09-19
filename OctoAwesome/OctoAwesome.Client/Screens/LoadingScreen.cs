using engenious;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.UI.Screens;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OctoAwesome.Client.UI.Components;

namespace OctoAwesome.Client.Screens
{
    internal sealed class LoadingScreen : OctoScreen
    {
        private static readonly QuoteProvider loadingQuoteProvider;
        static LoadingScreen()
        {
            var settings = TypeContainer.Get<ISettings>();
            loadingQuoteProvider = new QuoteProvider(new FileInfo(Path.Combine(settings.Get<string>("LoadingScreenQuotesPath"))));
        }

        private readonly GameScreen gameScreen;
        private readonly CancellationTokenSource tokenSource;
        private readonly Task quoteUpdate;
    
        public LoadingScreen(AssetComponent assets)
            : base(assets)
        {
            Padding = new Border(0, 0, 0, 0);
            tokenSource = new CancellationTokenSource();

            Title = "Loading";

            Background = new TextureBrush(assets.LoadTexture("background_new"), TextureBrushMode.Stretch);

            //Main Panel
            var mainStack = new Grid();
            mainStack.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 4 });
            mainStack.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
            mainStack.Margin = Border.All(50);
            mainStack.HorizontalAlignment = HorizontalAlignment.Stretch;
            mainStack.VerticalAlignment = VerticalAlignment.Stretch;

            Controls.Add(mainStack);

            var backgroundStack = new Panel()
            {
                Background = new BorderBrush(Color.White * 0.5f),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = Border.All(10)
            };
            mainStack.AddControl(backgroundStack, 0, 0, 1, 1);

            var mainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            mainGrid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            mainGrid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 3 });
            mainGrid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            mainGrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 4 });
            mainGrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
            mainGrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
            mainGrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 4 });

            backgroundStack.Controls.Add(mainGrid);

            var text = new Label()
            {
                Text = "Konfuzius sagt: Das hier lädt...",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = Border.All(10),
            };

            quoteUpdate = Task.Run(async () => await UpdateLabel(text, loadingQuoteProvider, TimeSpan.FromSeconds(1.5), tokenSource.Token));
            mainGrid.AddControl(text, 1, 1);


            //Buttons
            var buttonStack = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Horizontal
            };
            mainGrid.AddControl(buttonStack, 1, 2);

            var cancelButton = new TextButton(UI.Languages.OctoClient.Cancel, Style, ScreenManager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            buttonStack.Controls.Add(cancelButton);

            Debug.WriteLine("Create GameScreen");
            gameScreen = new GameScreen(assets);
            gameScreen.Update(new GameTime());
            gameScreen.OnCenterChanged += SwitchToGame;

            cancelButton.LeftMouseClick += (s, e) =>
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
                ScreenManager.Player.SetEntity(null);
                ScreenManager.Game.Simulation.ExitGame();
                gameScreen.Unload();
                ScreenManager.NavigateBack();
            };


        }

        private void SwitchToGame(object? sender, System.EventArgs args)
        {
            ScreenManager.Invoke(() =>
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
                ScreenManager.NavigateToScreen(gameScreen);
                gameScreen.OnCenterChanged -= SwitchToGame;
            });
        }

        private static async Task UpdateLabel(Label label, QuoteProvider quoteProvider, TimeSpan timeSpan, CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();
                var text = quoteProvider.GetRandomQuote();

                label.ScreenManager.Invoke(() => label.Text = text + "...");

                await Task.Delay(timeSpan, token);
            }
        }
    }
}
