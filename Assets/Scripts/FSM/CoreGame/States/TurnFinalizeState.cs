using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TurnFinalizeState : BaseState<GameStates>
{
	private bool _isInit = false;
	private Dictionary<TilePositionIndex, Symbol> _symbolDict;
	private List<List<TilePositionIndex>> _rowsIndexes;
	private List<List<TilePositionIndex>> _columnsIndexes;
	private List<List<TilePositionIndex>> _diagonalsIndexes;
	private Dictionary<TilePositionIndex, TileController> _tileControllerDict;

	private int _xScoreChange = 0;
	private int _oScoreChange = 0;
	private List<TilePositionIndex> _tileClearList = new();
	public TurnFinalizeState(GameStates state) : base(state)
	{
	}

	private bool _customRule = false;
	private int _customRuleNumber = 1;

	private int _selectedSuddenDeathRule = 0;
	private const int _suddenDeathLimit = 2;
	private int _clearCount = 0;
	private bool _suddenDeathRuleAssigned = false;

	public override void OnEnter()
	{
		//StackTrace tracer = new StackTrace();
		//StackFrame frame = tracer.GetFrame(1);

		//if (frame != null)
		//{
		//	var method = frame.GetMethod();
		//	UnityEngine.Debug.Log($"Enter from: {method.DeclaringType.Name}.{method.Name}");
		//}

		//UnityEngine.Debug.Log("Enter TurnFinalizeState");

		if (!_isInit)
		{
			_gameManager = GameManager.Instance;
			_symbolDict = _gameManager.SymbolDict;
			_tileControllerDict = _gameManager.TileControllerDict;

			_rowsIndexes = new()
			{
				new List<TilePositionIndex>()
				{
					TilePositionIndex.Tile00,
					TilePositionIndex.Tile01,
					TilePositionIndex.Tile02
				},
				new List<TilePositionIndex>()
				{
					TilePositionIndex.Tile10,
					TilePositionIndex.Tile11,
					TilePositionIndex.Tile12
				},
				new List<TilePositionIndex>()
				{
					TilePositionIndex.Tile20,
					TilePositionIndex.Tile21,
					TilePositionIndex.Tile22
				},
			};

			_columnsIndexes = new()
			{
				new List<TilePositionIndex>()
				{
					TilePositionIndex.Tile00,
					TilePositionIndex.Tile10,
					TilePositionIndex.Tile20
				},
				new List<TilePositionIndex>()
				{
					TilePositionIndex.Tile01,
					TilePositionIndex.Tile11,
					TilePositionIndex.Tile21
				},
				new List<TilePositionIndex>()
				{
					TilePositionIndex.Tile02,
					TilePositionIndex.Tile12,
					TilePositionIndex.Tile22
				}
			};

			_diagonalsIndexes = new()
			{
				new List<TilePositionIndex>()
				{
					TilePositionIndex.Tile00,
					TilePositionIndex.Tile11,
					TilePositionIndex.Tile22
				},
				new List<TilePositionIndex>()
				{
					TilePositionIndex.Tile02,
					TilePositionIndex.Tile11,
					TilePositionIndex.Tile20
				}
			};

			_isInit = true;
		}

		OnUpdate();
	}

	async public override void OnUpdate()
	{
		await CheckClearTiles(true);

		if (_gameManager.XScore == 3 || _gameManager.OScore == 3)
		{
			_gameManager.IsGameEnd = true;
		}

		OnExit();
	}

	async public Task CheckClearTiles(bool checkSuddenDeath)
	{
		GetTileClearListByRows();
		GetTileClearListByColumns();
		GetTileClearListByDiagonals();

		if (_xScoreChange > 0 || _oScoreChange > 0)
		{
			List<Task> tweenTasks = new();

			foreach (var tileIndex in _tileClearList)
			{
				tweenTasks.Add(_tileControllerDict[tileIndex].HighlightAndClear());
				_gameManager.ScoreSFX.Play();
				_symbolDict[tileIndex] = Symbol.None;
			}

			foreach (var task in tweenTasks)
			{
				await task;
			}

			_gameManager.UpdateScore(_xScoreChange, _oScoreChange);
			_clearCount = 0;
			_suddenDeathRuleAssigned = false;

			await _gameManager.SuddenDeathHintDisplayButtonController.MoveOut();
			_gameManager.SuddenDeathHintDisplayButtonController.ResetPosition();
			_gameManager.SuddenDeathHintDisplayButtonController.Hide();
		}
		else
		{
			if (!_symbolDict.Values.Contains(Symbol.None))
			{
				if (checkSuddenDeath)
				{
					if (_clearCount != _suddenDeathLimit)
					{
						_clearCount++;
					}
					else
					{
						await CheckSuddenDeath();
						return;
					}
				}

				await _gameManager.ResetTiles();
			}
		}
	}

	async private Task CheckSuddenDeath()
	{
		if (!_suddenDeathRuleAssigned)
		{
			await AssignSuddenDeathRule();
		}

		switch (_selectedSuddenDeathRule)
		{
			case 0:
				await ExecuteSuddenDeathRule0();
				break;
			case 1:
				await ExecuteSuddenDeathRule1();
				break;
			default:
				throw new System.Exception($"Unexpected sudden death rule detected: {_selectedSuddenDeathRule}");
		}

		await CheckClearTiles(false);
	}

	async private Task AssignSuddenDeathRule()
	{
		List<Task> tasks = new();

		if (_customRule)
		{
			_selectedSuddenDeathRule = _customRuleNumber;
		}
		else
		{
			_selectedSuddenDeathRule = Random.Range(0, 2);
		}

		switch (_selectedSuddenDeathRule)
		{
			case 0:
				_gameManager.SuddenDeathBannerController.SetRule0();
				break;
			case 1:
				_gameManager.SuddenDeathBannerController.SetRule1();
				break;
			default:
				throw new System.Exception($"Invalid sudden death rule detected: {_selectedSuddenDeathRule}");
		}

		_gameManager.GameplayBlurLayerGameObject.SetActive(true);

		tasks.Add(_gameManager.GameplayBlurLayerTweenComponent.ExecuteTweenOrders("Appear"));
		_gameManager.SuddenDeathBannerController.Show();
		tasks.Add(_gameManager.SuddenDeathBannerController.Move());
		_gameManager.SuddenDeathHintDisplayButtonController.Show();
		tasks.Add(_gameManager.SuddenDeathHintDisplayButtonController.MoveIn());


		foreach (var task in tasks)
		{
			await task;
		}

		tasks.Clear();

		//await Task.Delay(2000);
		await DelayHelper.Delay(2);

		tasks.Add(_gameManager.GameplayBlurLayerTweenComponent.ExecuteTweenOrders("Disappear"));
		tasks.Add(_gameManager.SuddenDeathBannerController.Move());

		foreach (var task in tasks)
		{
			await task;
		}

		_gameManager.GameplayBlurLayerGameObject.SetActive(false);
		_gameManager.SuddenDeathBannerController.Hide();
		_gameManager.SuddenDeathBannerController.ResetPosition();

		_suddenDeathRuleAssigned = true;
	}

	async private Task ExecuteSuddenDeathRule0() // switch row to row, column to column
	{
		List<Task> tasks = new();

		//int rowOrCol = Random.Range(0, 2);
		int rowOrCol = 0;

		List<int> lineIndex = new()
		{
			0, 1, 2
		};

		int indexA = lineIndex[Random.Range(1, lineIndex.Count)];
		lineIndex.Remove(indexA);

		int indexB = lineIndex[Random.Range(0, lineIndex.Count)];
		lineIndex.Remove(indexB);

		int indexC = lineIndex[0];

		lineIndex = new()
		{
			indexA, indexB, indexC
		};

		//Debug.Log($"indexA: {indexA}, indexB: {indexB}, indexC: {indexC}");

		Dictionary<TilePositionIndex, Symbol> newDict = new();

		if (rowOrCol == 0)
		{
			for (int i = 0; i < _rowsIndexes.Count; i++)
			{
				for (int j = 0; j < _rowsIndexes[lineIndex[i]].Count; j++)
				{
					newDict.Add(_rowsIndexes[i][j], _symbolDict[_rowsIndexes[lineIndex[i]][j]]);
				}
			}
		}
		else
		{
			for (int i = 0; i < _columnsIndexes.Count; i++)
			{
				for (int j = 0; j < _columnsIndexes[lineIndex[i]].Count; j++)
				{
					newDict.Add(_columnsIndexes[i][j], _symbolDict[_columnsIndexes[lineIndex[i]][j]]);
				}
			}
		}

		await _gameManager.ResetTiles();

		foreach (var kvp in newDict)
		{
			//Debug.Log($"kvp: {kvp.Key}, {kvp.Value}");
			tasks.Add(_gameManager.MarkTile(kvp.Key, kvp.Value, false));
		}

		foreach (var task in tasks)
		{
			await task;
		}

		//await Task.Delay(2000);
		await DelayHelper.Delay(2);
	}

	async private Task ExecuteSuddenDeathRule1()
	{
		List<TilePositionIndex> indexes = new();
		List<TilePositionIndex> allIndexes = _symbolDict.Keys.ToList();

		for (int i = 0; i < 5; i++)
		{
			int index = Random.Range(0, allIndexes.Count);

			indexes.Add(allIndexes[index]);
			allIndexes.RemoveAt(index);
		}

		await _gameManager.ResetTilesByList(indexes);
	}

	private void GetTileClearListByRows()
	{
		foreach (var row in _rowsIndexes)
		{
			Symbol symbol = Symbol.None;

			bool toAdd = true;

			foreach (var index in row)
			{
				if (_symbolDict[index] == Symbol.None)
				{
					toAdd = false;
					break;
				}
				else if (symbol == Symbol.None)
				{
					symbol = _symbolDict[index];
				}
				else
				{
					if (symbol != _symbolDict[index])
					{
						toAdd = false;
						break;
					}
				}
			}

			if (toAdd)
			{
				if (symbol == Symbol.X)
				{
					_xScoreChange++;
				}
				else if (symbol == Symbol.O)
				{
					_oScoreChange++;
				}
				else
				{
					throw new System.Exception($"Unexpected symbol: {symbol}");
				}

				foreach (var index in row)
				{
					if (!_tileClearList.Contains(index))
					{
						_tileClearList.Add(index);
					}
				}
			}
		}
	}

	private void GetTileClearListByColumns()
	{
		foreach (var col in _columnsIndexes)
		{
			Symbol symbol = Symbol.None;

			bool toAdd = true;

			foreach (var index in col)
			{
				if (_symbolDict[index] == Symbol.None)
				{
					toAdd = false;
					break;
				}
				else if (symbol == Symbol.None)
				{
					symbol = _symbolDict[index];
				}
				else
				{
					if (symbol != _symbolDict[index])
					{
						toAdd = false;
						break;
					}
				}
			}

			if (toAdd)
			{
				if (symbol == Symbol.X)
				{
					_xScoreChange++;
				}
				else if (symbol == Symbol.O)
				{
					_oScoreChange++;
				}
				else
				{
					throw new System.Exception($"Unexpected symbol: {symbol}");
				}

				foreach (var index in col)
				{
					if (!_tileClearList.Contains(index))
					{
						_tileClearList.Add(index);
					}
				}
			}
		}
	}

	private void GetTileClearListByDiagonals()
	{
		foreach (var diag in _diagonalsIndexes)
		{
			Symbol symbol = Symbol.None;

			bool toAdd = true;

			foreach (var index in diag)
			{
				if (_symbolDict[index] == Symbol.None)
				{
					toAdd = false;
					break;
				}
				else if (symbol == Symbol.None)
				{
					symbol = _symbolDict[index];
				}
				else
				{
					if (symbol != _symbolDict[index])
					{
						toAdd = false;
						break;
					}
				}
			}

			if (toAdd)
			{
				if (symbol == Symbol.X)
				{
					_xScoreChange++;
				}
				else if (symbol == Symbol.O)
				{
					_oScoreChange++;
				}
				else
				{
					throw new System.Exception($"Unexpected symbol: {symbol}");
				}

				foreach (var index in diag)
				{
					if (!_tileClearList.Contains(index))
					{
						_tileClearList.Add(index);
					}
				}
			}
		}
	}

	async public override void OnExit()
	{
		_xScoreChange = 0;
		_oScoreChange = 0;
		_tileClearList.Clear();

		if (!_gameManager.IsGameEnd)
		{
			await _gameManager.ChangeSymbol();

			if (_gameManager.CurrentGameMode == GameMode.Local)
			{
				_gameManager.GameStateDict[GameStates.PlayerAction].OnEnter();
			}
			else
			{
				if (_gameManager.LastAction == LastActionTakenBy.Player)
				{
					_gameManager.GameStateDict[GameStates.AIAction].OnEnter();
				}
				else
				{
					_gameManager.GameStateDict[GameStates.PlayerAction].OnEnter();
				}
			}
		}
		else
		{
			_gameManager.GameStateDict[GameStates.EndGame].OnEnter();
		}
	}
}
