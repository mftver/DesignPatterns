﻿using Factory.Interfaces;
using Model;
using File = Factory.models.File;
using Model.Interfaces;

namespace Factory.Factories
{
    internal class SudokuFactory : IFactory
    {
        public Sudoku Create(File file)
        {
            var sudokuString = file.Contents()[0];
            var gridSize = Convert.ToInt32(Math.Sqrt(sudokuString.Length));
            
            
            // Create a group for each column
            var columnGroups = createGroupArray(gridSize);
            var rowGroups = createGroupArray(gridSize);
            var subGroups = new List<SubGroup>();
            
            // Fill grid
            var grid = new Cell[gridSize][];
            for (var column = 0; column < gridSize; column++)
            {
                grid[column] = new Cell[gridSize];
                var rowGroup = rowGroups[column];
                
                for (var row = 0; row < gridSize; row++)
                {
                    var cellValue = sudokuString[column * gridSize + row] - '0';
                    var subGroupIndex = calculateSubGroupIndex(row, column , gridSize);

                    var subGroup = subGroups.ElementAtOrDefault(subGroupIndex);
                    if (subGroup == null)
                    {
                        subGroup = new SubGroup(subGroupIndex);
                        subGroups.Insert(subGroupIndex, subGroup);
                    }
                    
                    var groups = new List<Group>
                    {
                        columnGroups[row],
                        rowGroup,
                        subGroup,
                    };
                    
                    var isFixed = cellValue != 0;
                    
                    grid[column][row] = new Cell(cellValue, isFixed, groups, subGroup, gridSize);
                }
            }

            var allGroups = new List<IValidatable>();
            allGroups.AddRange(columnGroups);
            allGroups.AddRange(rowGroups);
            allGroups.AddRange(subGroups);
            
            // Tell cells to subscribe to each other
            return new Sudoku(grid, allGroups);
        }
        
        private Group[] createGroupArray(int size)
        {
            var groupArray = new Group[size];
            for (var i = 0; i < size; i++)
                groupArray[i] = new Group();
            
            return groupArray;
        }

        private int calculateSubGroupIndex(int x, int y, int gridSize)
        {
            if (gridSize == 9)
                return (y / (gridSize / 3)) * 3 + x / (gridSize / 3);
            if (gridSize == 6)
                return (y / (gridSize / 3)) * 2 + x / (gridSize / 2);
            if (gridSize == 4)
                return (y / (gridSize / 2)) * 2 + x / (gridSize / 2);

            throw new ArgumentException("Invalid grid size");
        }

        public bool Supports(File file)
        {
            var fileExtensions = new List<string>()
            {
                ".4x4",
                ".6x6",
                ".9x9",
            };

            return fileExtensions.Contains(file.Extension);
        }
    }
}
