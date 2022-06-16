﻿using Model.Interfaces;

namespace Model;

public class Sudoku
{
    public readonly Cell[][] Grid;

    public Sudoku(Cell[][] grid, List<IValidatable> groups)
    {
        Grid = grid;
        Groups = groups;
        CreateCellSubscriptions();
    }

    private List<IValidatable> Groups { get; }

    public bool Validate()
    {
        return Groups.All(validatable => validatable.Validate());
    }

    private void CreateCellSubscriptions()
    {
        // Create subscriptions
        foreach (var row in Grid)
        foreach (var cell in row)
        {
            if (cell == null) continue;
            cell.TriggerSubscription();
        }


        // Broadcast their initial values
        foreach (var row in Grid)
        foreach (var cell in row)
        {
            if (cell == null) continue;
            var nextVal = new NumberSwitch(0, cell.Number);
            cell.OnNext(nextVal);
        }
    }

    public bool TryEnter(Coordinate coordinate, int number)
    {
        return FindCell(coordinate).TrySetNumber(number);
    }

    public void Enter(Coordinate coordinate, int number)
    {
        FindCell(coordinate).SetNumber(number);
    }

    public Cell FindCell(Coordinate coordinate)
    {
        return Grid[coordinate.Y][coordinate.X];
    }
}