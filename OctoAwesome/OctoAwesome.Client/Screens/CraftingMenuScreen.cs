using engenious;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;

using OctoAwesome.Client.Components;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.Crafting;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.EntityComponents;
using OctoAwesome.Runtime;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Client.Screens;
internal class CraftingMenuScreen : OctoScreen
{
    private readonly StackPanel recipeList;
    private readonly StackPanel selectedRecipeInfoList;
    private readonly StackPanel selectedRecipeInputList;
    private readonly RecipeService recipeService;
    private readonly IReadOnlyCollection<Recipe> craftingRecipes;
    private readonly PlayerComponent player;
    private readonly InventoryComponent playerInventory;
    private readonly IDefinitionManager definitionManager;
    private readonly Button craftButton;
    private Recipe? selectedRecipe = null;

    public CraftingMenuScreen(AssetComponent assets) : base(assets)
    {
        IsOverlay = true;
        Background = new BorderBrush(Color.Black * 0.3f);

        //backgroundBrush = new BorderBrush(Color.Black);
        //hoverBrush = new BorderBrush(Color.Brown);

        player = ScreenManager.Player;
        playerInventory = player.Inventory;
        definitionManager = TypeContainer.Get<IDefinitionManager>();

        var panelBackground = assets.LoadTexture("panel");
        Debug.Assert(panelBackground != null, nameof(panelBackground) + " != null");

        var panel = new Panel()
        {
            Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
            Width = 800,
            Height = 500,
        };

        craftButton = new Button()
        {
            ZOrder = 15,
            VerticalAlignment = VerticalAlignment.Bottom,
            HorizontalAlignment = HorizontalAlignment.Right,
            Content = new Label { Text = "Craft" },
            Margin = new Border { Right = 20, Bottom = 20 }
        };
        craftButton.LeftMouseClick += CraftButtonPressed;
        panel.Controls.Add(craftButton);

        var mainGrid = new Grid()
        {
            ZOrder = 10
        };
        panel.Controls.Add(mainGrid);
        var recipeListColumn = new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 10 };

        var recipeOverviewColumn = new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 25 };
        mainGrid.Columns.Add(recipeListColumn);
        mainGrid.Columns.Add(recipeOverviewColumn);
        mainGrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 50 });
        mainGrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
        var recipeScroll = new ScrollContainer()
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Border(10, 10, 10, 10),
            CanFocus = false
        };

        recipeList = new StackPanel()
        {
            Padding = new Border(10, 0, 10, 0),
            VerticalAlignment = VerticalAlignment.Stretch,
            Orientation = Orientation.Vertical,
        };
        recipeScroll.Content = recipeList;
        mainGrid.AddControl(recipeScroll, 0, 1);

        var recipeOverviewGrid = new Grid();
        mainGrid.AddControl(recipeOverviewGrid, 1, 0, rowSpan: 2);

        var recipeOverviewInfoColumn = new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 20 };
        var recipeOverviewInputColumn = new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 13 };
        recipeOverviewGrid.Columns.Add(recipeOverviewInfoColumn);
        recipeOverviewGrid.Columns.Add(recipeOverviewInputColumn);
        recipeOverviewGrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 50 });
        recipeOverviewGrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });

        selectedRecipeInfoList = new StackPanel
        {
            Padding = new Border(10, 0, 10, 0),
            VerticalAlignment = VerticalAlignment.Stretch,
            Orientation = Orientation.Vertical,
        };
        selectedRecipeInputList = new StackPanel
        {
            Padding = new Border(10, 0, 10, 0),
            VerticalAlignment = VerticalAlignment.Stretch,
            Orientation = Orientation.Vertical,
        };

        recipeOverviewGrid.AddControl(new Label { Text = "Info" }, 0, 0);
        recipeOverviewGrid.AddControl(selectedRecipeInfoList, 0, 1);
        recipeOverviewGrid.AddControl(new Label { Text = "Input" }, 1, 0);
        recipeOverviewGrid.AddControl(selectedRecipeInputList, 1, 1);

        recipeService = TypeContainer.Get<RecipeService>();
        craftingRecipes = recipeService.GetByType("Crafting");

        Controls.Add(panel);
    }


    private void CraftButtonPressed(Control sender, MouseEventArgs args)
    {
        if (selectedRecipe is null)
            Environment.Exit(0); //todo erziehen

        

    }

    protected override void OnNavigatedTo(NavigationEventArgs args)
    {
        ScreenManager.FreeMouse();
        FillRecipes();
        base.OnNavigatedTo(args);
    }

    private void FillRecipes()
    {
        recipeList.Controls.Clear();
        foreach (var currentRecipe in craftingRecipes)
        {
            var button = new Button { MinWidth = 175, MaxWidth = 175, Content = new Label { Text = currentRecipe.Name } };
            button.LeftMouseClick += (s, e) =>
            {
                selectedRecipe = currentRecipe;
                bool matched = true;
                foreach (var inputItem in currentRecipe.Inputs)
                {
                    if (!matched)
                        break;
                    var itemFound = playerInventory.Contains(inputItem.ItemName, inputItem.Count);

                    if (!itemFound)
                    {
                        matched = false;
                        break;
                    }
                }
                craftButton.Enabled = matched;
                Refresh();
            };
            recipeList.Controls.Add(button);
        }
    }

    private void Refresh()
    {
        FillRecipes();
        selectedRecipeInputList.Controls.Clear();
        selectedRecipeInfoList.Controls.Clear();

        if (selectedRecipe is not null)
        {
            foreach (var input in selectedRecipe.Inputs)
            {
                selectedRecipeInputList.Controls.Add(new Label { Text = $"{input.ItemName} ({input.MaterialName})" });
            }

            selectedRecipeInfoList.Controls.Add(new Label { Text = selectedRecipe.Name });
            selectedRecipeInfoList.Controls.Add(new Label { Text = selectedRecipe.Description });
            if (selectedRecipe.Time.HasValue)
                selectedRecipeInfoList.Controls.Add(new Label { Text = $"Braucht {selectedRecipe.Time.Value} Zeit" });
        }
    }

    protected override void OnKeyDown(KeyEventArgs args)
    {
        if (ScreenManager.CanGoBack && (args.Key == Keys.Escape || args.Key == Keys.C))
        {
            args.Handled = true;
            ScreenManager.NavigateBack();
        }

        base.OnKeyDown(args);
    }
}
