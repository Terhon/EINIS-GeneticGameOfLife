using System.Collections.Generic;
using System.Linq;
using GeneticGameOfLife.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GeneticGameOfLife
{
public class MainGame : Game
{
    private List<IUiElement> _uiElements = new List<IUiElement>();
    private GraphicsDeviceManager _graphicsDeviceManager;
    private SpriteBatch _spriteBatch;

    public MainGame ()
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
    }
    protected override void Initialize()
    {
        var simulation = new Simulation();
        var menu = new Menu();
        simulation.Initialize(GraphicsDevice, new ContentManager(Content.ServiceProvider,Content.RootDirectory));
        menu.Initialize(GraphicsDevice, new ContentManager(Content.ServiceProvider,Content.RootDirectory), simulation);
        
        _uiElements.Add(simulation);
        _uiElements.Add(menu);
        
        _graphicsDeviceManager.PreferredBackBufferHeight = simulation.Board.BaseState.GetLength(1) * simulation.CellSize + 1;
        _graphicsDeviceManager.PreferredBackBufferWidth = simulation.Board.BaseState.GetLength(0) * simulation.CellSize + 1 + 250;
        _graphicsDeviceManager.ApplyChanges();
        
        IsMouseVisible = true;
        
        base.Initialize();
    }
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        foreach (var uiElement in _uiElements)
            uiElement.LoadContent();
    }
    protected override void UnloadContent()
    {
        foreach (var uiElement in _uiElements)
            uiElement.UnloadContent();
    }
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (_uiElements?.Any() == true)
        {
            _graphicsDeviceManager.PreferredBackBufferHeight =
                ((Simulation) _uiElements[0]).Board.BaseState.GetLength(1) * ((Simulation) _uiElements[0]).CellSize + 1;
            _graphicsDeviceManager.PreferredBackBufferWidth =
                ((Simulation) _uiElements[0]).Board.BaseState.GetLength(0) * ((Simulation) _uiElements[0]).CellSize + 1 + 250;
            _graphicsDeviceManager.ApplyChanges();
        }
        foreach (var uiElement in _uiElements)
            uiElement.Update(gameTime);
        
        base.Update(gameTime);
    }
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        foreach (var uiElement in _uiElements)
            uiElement.Draw(gameTime);
        
        base.Draw(gameTime);
    }
}
}