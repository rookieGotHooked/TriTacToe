using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Linq;
//using System.Diagnostics;

public class AIActionState : BaseState<GameStates>
{
	//GameManager _gameManager;

	private bool _isInit = false;
	private Dictionary<TilePositionIndex, Symbol> _symbolDict;

	private AIDifficulties _difficulty;

	//GameObject _screenObject;
	private List<List<TilePositionIndex>> _rows;
	private List<List<TilePositionIndex>> _columns;
	private List<List<TilePositionIndex>> _diagonals;
	
	private List<TilePositionIndex> _emptyTiles;

	private List<TilePositionIndex> _xScoringMoves;
	private List<TilePositionIndex> _oScoringMoves;

	private List<TilePositionIndex> _xScoringMovesDuplicate;
	private List<TilePositionIndex> _oScoringMovesDuplicate;
	private List<TilePositionIndex> _createMovesDuplicate;			// always for AI symbol

	private List<TilePositionIndex> _blockAndCreateMoves;
	private List<TilePositionIndex> _scoreMoves;
	private List<TilePositionIndex> _blockOnlyMoves;
	private List<TilePositionIndex> _createOnlyMoves;
	private List<TilePositionIndex> _fillerMoves;

	private Dictionary<TilePositionIndex, float> _moveScoringDict;

	private const float _blockMoveScore = 0.75f;
	private const float _scoreMoveScore = 1f;
	private const float _createMoveScore = 0.45f;
	private const float _fillerMoveScore = 0.25f;


	public AIActionState(GameStates state) : base(state)
	{
	}

	public override void OnEnter()
	{
		//Debug.Log($"Enter AI State; Symbol: {_gameManager.CurrentSymbol}");

		_symbolDict = _gameManager.SymbolDict;

		if (!_isInit)
		{
			_difficulty = GameManager.Instance.Difficulty;
			_isInit = true;

			_rows = new()
			{
				new List<TilePositionIndex>() {
					TilePositionIndex.Tile00,
					TilePositionIndex.Tile01,
					TilePositionIndex.Tile02,
				},

				new List<TilePositionIndex>() {
					TilePositionIndex.Tile10,
					TilePositionIndex.Tile11,
					TilePositionIndex.Tile12,
				},

				new List<TilePositionIndex>() {
					TilePositionIndex.Tile20,
					TilePositionIndex.Tile21,
					TilePositionIndex.Tile22,
				},
			};

			_columns = new()
			{
				new List<TilePositionIndex>() {
					TilePositionIndex.Tile00,
					TilePositionIndex.Tile10,
					TilePositionIndex.Tile20,
				},

				new List<TilePositionIndex>() {
					TilePositionIndex.Tile01,
					TilePositionIndex.Tile11,
					TilePositionIndex.Tile21,
				},

				new List<TilePositionIndex>() {
					TilePositionIndex.Tile02,
					TilePositionIndex.Tile12,
					TilePositionIndex.Tile22,
				},
			};

			_diagonals = new()
			{
				new List<TilePositionIndex>() {
					TilePositionIndex.Tile00,
					TilePositionIndex.Tile11,
					TilePositionIndex.Tile22,
				},

				new List<TilePositionIndex>() {
					TilePositionIndex.Tile02,
					TilePositionIndex.Tile11,
					TilePositionIndex.Tile20,
				},
			};

			_isInit = true;
		}

		_emptyTiles = new();

		_xScoringMoves = new();
		_oScoringMoves = new();

		if (_difficulty == AIDifficulties.Hard)
		{
			_moveScoringDict = new();
			_xScoringMovesDuplicate = new();
			_oScoringMovesDuplicate = new();
			_createMovesDuplicate = new();

			_blockAndCreateMoves = new();
			_scoreMoves = new();
			_blockOnlyMoves = new();
			_createOnlyMoves = new();
			_fillerMoves = new();
		}

		FindEmptyTilesInLine(_rows);
		FindEmptyTilesInLine(_columns);
		FindEmptyTilesInLine(_diagonals);

		OnUpdate();
	}

	public override async void OnUpdate()
	{
		TilePositionIndex tileToMark;

		if (_difficulty == AIDifficulties.Random)
		{
			tileToMark = GetRandomTile(_emptyTiles);
		}
		else if (_difficulty == AIDifficulties.Normal)
		{
			FindPossibleScoringTile(_rows, Symbol.X, false);
			FindPossibleScoringTile(_columns, Symbol.X, false);
			FindPossibleScoringTile(_diagonals, Symbol.X, false);

			FindPossibleScoringTile(_rows, Symbol.O, false);
			FindPossibleScoringTile(_columns, Symbol.O, false);
			FindPossibleScoringTile(_diagonals, Symbol.O, false);

			if (_gameManager.PlayerSymbol == Symbol.X)
			{
				if (_xScoringMoves.Count > 0)
				{
					tileToMark = GetRandomTile(_xScoringMoves);
				}
				else if (_oScoringMoves.Count > 0)
				{
					tileToMark = GetRandomTile(_oScoringMoves);
				}
				else
				{
					tileToMark = GetRandomTile(_emptyTiles);
				}
			}
			else if (_gameManager.PlayerSymbol == Symbol.O)
			{
				if (_oScoringMoves.Count > 0)
				{
					tileToMark = GetRandomTile(_oScoringMoves);
				}
				else if (_xScoringMoves.Count > 0)
				{
					tileToMark = GetRandomTile(_xScoringMoves);
				}
				else
				{
					tileToMark = GetRandomTile(_emptyTiles);
				}
			}
			else
			{
				throw new Exception($"Invalid symbol detected: {_gameManager.PlayerSymbol}");
			}
		}
		else if (_difficulty == AIDifficulties.Hard)
		{
			FindPossibleScoringTile(_rows, Symbol.X, true);
			FindPossibleScoringTile(_columns, Symbol.X, true);
			FindPossibleScoringTile(_diagonals, Symbol.X, true);

			FindPossibleScoringTile(_rows, Symbol.O, true);
			FindPossibleScoringTile(_columns, Symbol.O, true);
			FindPossibleScoringTile(_diagonals, Symbol.O, true);

			InitMoveScoringDict();

			if (_gameManager.PlayerSymbol == Symbol.X)
			{
				FindCreateMoves(_rows, Symbol.O);
				FindCreateMoves(_columns, Symbol.O);
				FindCreateMoves(_diagonals, Symbol.O);
				AssignScoreToEmptyTiles(Symbol.O);
			}
			else if (_gameManager.PlayerSymbol == Symbol.O)
			{
				FindCreateMoves(_rows, Symbol.X);
				FindCreateMoves(_columns, Symbol.X);
				FindCreateMoves(_diagonals, Symbol.X);
				AssignScoreToEmptyTiles(Symbol.X);
			}
			else
			{
				throw new Exception($"Invalid player symbol: {_gameManager.PlayerSymbol}");
			}

			tileToMark = FindBestMove();
		}
		else
		{
			throw new System.Exception($"Unexpected AI difficulty detected: {_difficulty}");
		}

		await TileClick(tileToMark);
	}

	public override void OnExit()
	{
		_gameManager.SetLastAction(LastActionTakenBy.AI);

		//if (_difficulty == AIDifficulties.Hard)
		//{
		//	if (_blockAndCreateMoves.Count > 0)
		//	{
		//		UnityEngine.Debug.Log($"_blockAndCreateMoves: {_blockAndCreateMoves[0]}, {_moveScoringDict[_blockAndCreateMoves[0]]}");
		//	}
		//	if (_scoreMoves.Count > 0)
		//	{
		//		UnityEngine.Debug.Log($"_scoreMoves: {_scoreMoves[0]}, {_moveScoringDict[_scoreMoves[0]]}");
		//	}
		//	if (_blockOnlyMoves.Count > 0)
		//	{
		//		UnityEngine.Debug.Log($"_blockOnlyMoves: {_blockOnlyMoves[0]}, {_moveScoringDict[_blockOnlyMoves[0]]}");
		//	}
		//	if (_createOnlyMoves.Count > 0)
		//	{
		//		UnityEngine.Debug.Log($"_createOnlyMoves: {_createOnlyMoves[0]}, {_moveScoringDict[_createOnlyMoves[0]]}");
		//	}
		//}


		_gameManager.GameStateDict[GameStates.TurnFinalize].OnEnter();
	}

	public void FindEmptyTilesInLine(List<List<TilePositionIndex>> list)
	{
		foreach (var line in list)
		{
			foreach (var index in line)
			{
				if (_symbolDict[index] == Symbol.None && !_emptyTiles.Contains(index))
				{
					_emptyTiles.Add(index);
				}
			}
		}
	}

	public void FindPossibleScoringTile(List<List<TilePositionIndex>> list, Symbol symbol, bool allowDuplicate)
	{
		foreach (var line in list)
		{
			List<TilePositionIndex> noneIndexList = new();

			bool checkAdd = true;

			foreach (var index in line)
			{
				if (_symbolDict[index] == Symbol.None)
				{
					noneIndexList.Add(index);

					if (noneIndexList.Count > 1)
					{
						checkAdd = false;
						break;
					}
				}
				else if (_symbolDict[index] != symbol)
				{
					checkAdd = false;
					break;
				}
			}

			if (checkAdd)
			{
				if (noneIndexList.Count == 1)
				{
					if (symbol == Symbol.X)
					{
						if (!allowDuplicate && _xScoringMoves.Contains(noneIndexList[0]))
						{
							continue;
						}
						else if (!allowDuplicate && !_xScoringMoves.Contains(noneIndexList[0]))
						{
							_xScoringMoves.Add(noneIndexList[0]);
						}
						else if (allowDuplicate)
						{
							_xScoringMovesDuplicate.Add(noneIndexList[0]);
						}
					}
					else if (symbol == Symbol.O)
					{
						if (!allowDuplicate && _oScoringMoves.Contains(noneIndexList[0]))
						{
							continue;
						}
						else if (!allowDuplicate && !_oScoringMoves.Contains(noneIndexList[0]))
						{
							_oScoringMoves.Add(noneIndexList[0]);
						}
						else if (allowDuplicate)
						{
							_oScoringMovesDuplicate.Add(noneIndexList[0]);
						}
					}
				}
			}
		}
	}

	public void InitMoveScoringDict()
	{
		foreach (var tile in _emptyTiles)
		{
			_moveScoringDict.Add(tile, 0f);
		}
	}

	public void AssignScoreToEmptyTiles(Symbol symbol)
	{
		List<TilePositionIndex> aiScoringMoves, playerScoringMoves;

		if (symbol == Symbol.O)
		{
			aiScoringMoves = _oScoringMovesDuplicate;
			playerScoringMoves = _xScoringMovesDuplicate;
		}
		else if (symbol == Symbol.X)
		{
			aiScoringMoves = _xScoringMovesDuplicate;
			playerScoringMoves = _oScoringMovesDuplicate;
		}
		else
		{
			throw new Exception($"Invalid symbol detected: {symbol}");
		}

		foreach (var index in _emptyTiles)
		{
			if (aiScoringMoves.Contains(index))
			{
				if (!_scoreMoves.Contains(index))
				{
					_scoreMoves.Add(index);
				}

				foreach (var scoreIndex in aiScoringMoves)
				{
					if (scoreIndex == index)
					{
						_moveScoringDict[index] += _scoreMoveScore;
					}
				}
			}
			else
			{
				if (!playerScoringMoves.Contains(index) && !_createMovesDuplicate.Contains(index))
				{
					_moveScoringDict[index] += _fillerMoveScore;

					if (!_fillerMoves.Contains(index))
					{
						_fillerMoves.Add(index);
					}
				}
				else
				{
					bool isBlock = true;
					bool isCreate = true;

					if (playerScoringMoves.Contains(index))
					{
						foreach (var blockIndex in playerScoringMoves)
						{
							if (blockIndex == index)
							{
								_moveScoringDict[index] += _blockMoveScore;
							}
						}
					}
					else
					{
						isBlock = false;
					}

					if (_createMovesDuplicate.Contains(index))
					{
						foreach (var createIndex in _createMovesDuplicate)
						{
							if (createIndex == index)
							{
								_moveScoringDict[index] += _createMoveScore;
							}
						}
					}
					else
					{
						isCreate = false;
					}

					if (isBlock && !isCreate)
					{
						_blockOnlyMoves.Add(index);
					}
					else if (!isBlock && isCreate)
					{
						_createOnlyMoves.Add(index);
					}
					else if (isBlock && isCreate)
					{
						_blockAndCreateMoves.Add(index);
					}
				}
			}
		}
	}

	public TilePositionIndex FindBestMove()
	{
		float maxBlockAndCreateScore = 0f;
		TilePositionIndex maxBlockAndCreateIndex = TilePositionIndex.Tile00;

		float maxScoringScore = 0f;
		TilePositionIndex maxScoringIndex = TilePositionIndex.Tile00;

		float maxBlockOnlyScore = 0f;
		TilePositionIndex maxBlockOnlyIndex = TilePositionIndex.Tile00;

		float maxCreateOnlyScore = 0f;
		TilePositionIndex maxCreateOnlyIndex = TilePositionIndex.Tile00;

		if (_blockAndCreateMoves.Count > 0) 
		{
			if (_blockAndCreateMoves.Count > 1)
			{
				Dictionary<TilePositionIndex, float> temp = new();

				foreach (var value in _blockAndCreateMoves)
				{
					temp.Add(value, _moveScoringDict[value]);
				}

				if (temp.Values.Distinct().Count() == 1)
				{
					maxBlockAndCreateIndex = _blockAndCreateMoves[UnityEngine.Random.Range(0, _blockAndCreateMoves.Count)];
					maxBlockAndCreateScore = _moveScoringDict[maxBlockAndCreateIndex];
				}
				else
				{
					var maxBlockAndCreateElement = _blockAndCreateMoves
						.Select(index => new { Index = index, Value = _moveScoringDict[index] })
						.OrderByDescending(x => x.Value).
						FirstOrDefault();

					maxBlockAndCreateIndex = maxBlockAndCreateElement.Index;
					maxBlockAndCreateScore = maxBlockAndCreateElement.Value;
				}
			}
			else
			{
				maxBlockAndCreateIndex = _blockAndCreateMoves[0];
				maxBlockAndCreateScore = _moveScoringDict[maxBlockAndCreateIndex];
			}
		}

		if (_scoreMoves.Count > 0)
		{
			if (_scoreMoves.Count > 1)
			{
				if ((_gameManager.PlayerSymbol == Symbol.X && _gameManager.OScore == 2) 
					|| (_gameManager.PlayerSymbol == Symbol.O && _gameManager.XScore == 2))
				{
					return _scoreMoves[0];
				}

				Dictionary<TilePositionIndex, float> temp = new();

				foreach (var value in _scoreMoves)
				{
					temp.Add(value, _moveScoringDict[value]);
				}

				if (temp.Values.Distinct().Count() == 1)
				{
					maxScoringIndex = _scoreMoves[UnityEngine.Random.Range(0, _scoreMoves.Count)];
					maxScoringScore = _moveScoringDict[maxScoringIndex];
				}
				else
				{
					var maxScoringElement = _scoreMoves
						.Select(index => new { Index = index, Value = _moveScoringDict[index] })
						.OrderByDescending(x => x.Value).
						FirstOrDefault();

					maxScoringIndex = maxScoringElement.Index;
					maxScoringScore = maxScoringElement.Value;
				}
			}
			else
			{
				maxScoringIndex = _scoreMoves[0];
				maxScoringScore = _moveScoringDict[maxScoringIndex];
			}
		}

		if (_blockOnlyMoves.Count > 0)
		{
			if (_blockOnlyMoves.Count > 1)
			{
				Dictionary<TilePositionIndex, float> temp = new();

				foreach (var value in _blockOnlyMoves)
				{
					temp.Add(value, _moveScoringDict[value]);
				}

				if (temp.Values.Distinct().Count() == 1)
				{
					maxBlockOnlyIndex = _blockOnlyMoves[UnityEngine.Random.Range(0, _blockOnlyMoves.Count)];
					maxBlockOnlyScore = _moveScoringDict[maxBlockOnlyIndex];
				}
				else
				{
					var maxBlockOnlyElement = _blockOnlyMoves
						.Select(index => new { Index = index, Value = _moveScoringDict[index] })
						.OrderByDescending(x => x.Value).
						FirstOrDefault();

					maxBlockOnlyIndex = maxBlockOnlyElement.Index;
					maxBlockOnlyScore = maxBlockOnlyElement.Value;
				}
			}
			else
			{
				maxBlockOnlyIndex = _blockOnlyMoves[0];
				maxBlockOnlyScore = _moveScoringDict[maxBlockOnlyIndex];
			}
		}

		if (_createOnlyMoves.Count > 0)
		{
			if (_createOnlyMoves.Count > 1)
			{
				Dictionary<TilePositionIndex, float> temp = new();

				foreach (var value in _createOnlyMoves)
				{
					temp.Add(value, _moveScoringDict[value]);
				}

				if (temp.Values.Distinct().Count() == 1)
				{
					maxCreateOnlyIndex = _createOnlyMoves[UnityEngine.Random.Range(0, _createOnlyMoves.Count)];
					maxCreateOnlyScore = _moveScoringDict[maxCreateOnlyIndex];
				}
				else
				{
					var maxCreateOnlyElement = _createOnlyMoves
						.Select(index => new { Index = index, Value = _moveScoringDict[index] })
						.OrderByDescending(x => x.Value).
						FirstOrDefault();

					maxCreateOnlyIndex = maxCreateOnlyElement.Index;
					maxCreateOnlyScore = maxCreateOnlyElement.Value;
				}
			}
			else
			{
				maxCreateOnlyIndex = _createOnlyMoves[0];
				maxCreateOnlyScore = _moveScoringDict[maxCreateOnlyIndex];
			}
		}

		if (maxBlockAndCreateScore > 0f || maxScoringScore > 0f || maxBlockOnlyScore > 0f || maxCreateOnlyScore > 0f)
		{

			List<KeyValuePair<TilePositionIndex, float>> cleanScore = new();

			if (maxBlockAndCreateScore > 0f)
			{
				cleanScore.Add(new KeyValuePair<TilePositionIndex, float>(maxBlockAndCreateIndex, maxBlockAndCreateScore));
			}

			if (maxScoringScore > 0f)
			{
				cleanScore.Add(new KeyValuePair<TilePositionIndex, float>(maxScoringIndex, maxScoringScore));
			}

			if (maxBlockOnlyScore > 0f)
			{
				cleanScore.Add(new KeyValuePair<TilePositionIndex, float>(maxBlockOnlyIndex, maxBlockOnlyScore));
			}

			if (maxCreateOnlyScore > 0f)
			{
				cleanScore.Add(new KeyValuePair<TilePositionIndex, float>(maxCreateOnlyIndex, maxCreateOnlyScore));
			}

			Dictionary<TilePositionIndex, float> bestScores = new();

			foreach (var element in cleanScore)
			{
				bestScores.Add(element.Key, element.Value);
			}

			return bestScores.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
		}
		else
		{
			if (_fillerMoves.Count == 9) 
			{
				int roll = UnityEngine.Random.Range(0, 100);

				if (roll >= 0 && roll < 60)
				{
					return TilePositionIndex.Tile11;
				}
				else
				{
					List<TilePositionIndex> others = new()
					{
						TilePositionIndex.Tile00,
						TilePositionIndex.Tile01,
						TilePositionIndex.Tile02,
						TilePositionIndex.Tile10,
						TilePositionIndex.Tile12,
						TilePositionIndex.Tile20,
						TilePositionIndex.Tile21,
						TilePositionIndex.Tile22
					};

					return GetRandomTile(others);
				}
			}
			else
			{
				return GetRandomTile(_fillerMoves);
			}
		}
	}

	public void FindCreateMoves(List<List<TilePositionIndex>> list, Symbol symbol)
	{
		foreach (var line in list)
		{
			List<TilePositionIndex> noneIndexList = new();
			int sameSymbolCount = 0;

			bool checkAdd = true;

			foreach (var index in line)
			{
				if (_symbolDict[index] == Symbol.None)
				{
					noneIndexList.Add(index);

					if (noneIndexList.Count > 2)
					{
						checkAdd = false;
						break;
					}
				}
				else if (_symbolDict[index] != symbol)
				{
					checkAdd = false;
					break;
				}
				else
				{
					sameSymbolCount++;
					if (sameSymbolCount > 1)
					{
						checkAdd = false;
						break;
					}
				}
			}

			if (checkAdd)
			{
				foreach (var index in noneIndexList)
				{
					_createMovesDuplicate.Add(index);
				}
			}
		}
	}

	public TilePositionIndex GetRandomTile(List<TilePositionIndex> list)
	{
		//StackTrace tracer = new();
		//StackFrame frame = tracer.GetFrame(1);

		//if (frame != null)
		//{
		//	var method = frame.GetMethod();
		//	UnityEngine.Debug.Log($"Caller info: {method.DeclaringType.Name}.{method.Name}");
		//}

		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	async public Task TileClick(TilePositionIndex index)
	{
		await Task.Delay(500);

		if (_symbolDict[index] == Symbol.None)
		{
			//_gameManager.SetAllTilesInteractable(false);

			await _gameManager.MarkTile(index, _gameManager.CurrentSymbol);
			_gameManager.LastMarkedTile = index;
			OnExit();
		}
	}

	public void ResetData()
	{
		_isInit = false;
	}
}
