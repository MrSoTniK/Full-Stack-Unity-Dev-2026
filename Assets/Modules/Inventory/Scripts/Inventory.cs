using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Modules.Inventories
{
    public class Inventory : IEnumerable<Item>
    {
        public event Action<Item, Vector2Int> OnAdded;
        public event Action<Item, Vector2Int> OnRemoved;
        public event Action<Item, Vector2Int> OnMoved;
        public event Action OnCleared;

        private readonly int _width;
        private readonly int _height;

        private readonly Dictionary<Item, Vector2Int> _dict;
        private readonly Item[,] _matrix;

        public int Width => _width;
        public int Height => _height;
        public int Count => _dict.Count;

        public Inventory(int width, int height)
        {
            if (width <= 0 || height <= 0) throw new ArgumentException("Not valid width or height in constructor");

            _width = width;
            _height = height;
            _dict = new();
            _matrix = new Item[_width, _height];
        }

        public Inventory(
            int width,
            int height,
            params KeyValuePair<Item, Vector2Int>[] items
        ) : this(width, height)
        {
            if (items == null) throw new ArgumentNullException("KeyValuePair<Item, Vector2Int>[] items is null in constructor");

            foreach (var item in items)
                AddItem(item.Key, item.Value);
        }

        public Inventory(
            int width,
            int height,
            params Item[] items
        ) : this(width, height)
        {
            if (items == null) throw new ArgumentNullException("Item[] items is null in constructor");

            foreach (var item in items)
                AddItem(item);
        }

        public Inventory(
            int width,
            int height,
            IEnumerable<KeyValuePair<Item, Vector2Int>> items
        ) : this(width, height)
        {
            if (items == null) throw new ArgumentNullException(" IEnumerable<KeyValuePair<Item, Vector2Int>> items is null in constructor");

            foreach (var item in items)
                AddItem(item.Key, item.Value);
        }

        public Inventory(
            int width,
            int height,
            IEnumerable<Item> items
        ) : this(width, height)
        {
            if (items == null) throw new ArgumentNullException("IEnumerable<Item> items is null in constructor");

            foreach (var item in items)
                AddItem(item);
        }

        /// <summary>
        /// Creates new inventory 
        /// </summary>
        public Inventory(Inventory inventory) :this(inventory.Width, inventory.Height)
        {
            var matrix = new Item[inventory.Width, inventory.Height];
            inventory.CopyTo(matrix);

            for (int columnIndex = 0; columnIndex < _height; columnIndex++)
            {
                for (int rowIndex = 0; rowIndex < _width; rowIndex++)
                {
                    var item = matrix[rowIndex, columnIndex];

                    if (item == null || Contains(item)) continue;
                    AddItem(item, rowIndex, columnIndex);
                }
            }
        }

        /// <summary>
        /// Checks for adding an item on a specified position
        /// </summary>
        public bool CanAddItem(Item item, Vector2Int position)
        {
            if (item == null) return false;

            if (!CheckItemCompatability(item.Size.x, item.Size.y))
                throw new ArgumentException($"Cannot work with item, null or invalid size");

            return CanAddItem(item, position.x, position.y);
        }

        public bool CanAddItem(Item item, int startX, int startY)
        {
            GetEndPositions(item.Size.x, item.Size.y, startX, startY, out int endX, out int endY);
            return !Contains(item) && IsFreeSpace(startX, startY, endX, endY);
        }    

        /// <summary>
        /// Adds an item on a specified position
        /// </summary>
        public bool AddItem(Item item, Vector2Int position)
        {
            if (item == null || !CheckItemCompatability(item.Size.x, item.Size.y))
                throw new ArgumentException($"Cannot work with item, null or invalid size");

            if (!CanAddItem(item, position)) return false;

            _dict.Add(item, position);
            AddToMatrix(item, position.x, position.y);
            OnAdded?.Invoke(item, position);
            return true;
        }

        public bool AddItem(Item item, int startX, int startY)
        {
            if (item == null || !CheckItemCompatability(item.Size.x, item.Size.y))
                return false;

            if (!CanAddItem(item, startX, startY)) return false;

            var position = new Vector2Int(startX, startY);
            _dict.Add(item, position);
            AddToMatrix(item, startX, startY);
            OnAdded?.Invoke(item, position);
            return true;
        }

        private void GetEndPositions(int sizeX, int sizeY, int startX, int startY, out int endX, out int endY) 
        {
            endX = startX + sizeX - 1;
            endY = startY + sizeY - 1;
        }

        private void AddToMatrix(Item item, int startX, int startY) 
        {
            GetEndPositions(item.Size.x, item.Size.y, startX, startY, out int endX, out int endY);

            for (int x = startX; x <= endX; x++)
                for (int y = startY; y <= endY; y++)
                    _matrix[x, y] = item;
        }

        /// <summary>
        /// Checks for adding an item on a free position
        /// </summary>
        public bool CanAddItem(Item item)
        {
            if (item == null) return false;

            if (!CheckItemCompatability(item.Size.x, item.Size.y))
                throw new ArgumentException($"Cannot work with item, null or invalid size");

            return !Contains(item) && FindFreePosition(item, out _);
        }

        /// <summary>
        /// Adds an item on a free position
        /// </summary>
        public bool AddItem(Item item)
        {
            if (item == null) return false;

            if (!CheckItemCompatability(item.Size.x, item.Size.y))
                throw new ArgumentException($"Cannot work with item with size {item.Size}");

            if (Contains(item) || !FindFreePosition(item, out var position)) return false;

            _dict.Add(item, position);
            AddToMatrix(item, position.x, position.y);
            OnAdded?.Invoke(item, position);
            return true;
        }

        private bool CheckItemCompatability(int sizeX, int sizeY) 
        {
            return sizeX > 0 && sizeY > 0;
        }

        /// <summary>
        /// Returns a free position for a specified item
        /// </summary>
        public bool FindFreePosition(Item item, out Vector2Int position)
        {
            return FindFreePosition(item.Size, out position);
        }

        public bool FindFreePosition(Vector2Int size, out Vector2Int position)
        {
            return FindFreePosition(size.x, size.y, out position);
        }

        public bool FindFreePosition(int sizeX, int sizeY, out Vector2Int position)
        {
            if (!CheckItemCompatability(sizeX, sizeY)) throw new ArgumentException("Invalid size");

            for (int columnIndex = 0; columnIndex < _height; columnIndex++ )
            {
                for (int rowIndex = 0; rowIndex < _width; rowIndex++)
                {
                    GetEndPositions(sizeX, sizeY, rowIndex, columnIndex, out int endX, out int endY);

                    if (IsFreeSpace(rowIndex, columnIndex, endX, endY))
                    {
                        position = new Vector2Int(rowIndex, columnIndex);
                        return true;
                    }
                }
            }

            position = default;
            return false;
        }

        private bool IsFreeSpace(int startX, int startY, int endX, int endY)
        {
            bool canBeInsideInv = startX >= 0 &&
                 startY >= 0 &&
                 endX < _width &&
                 endY < _height;

            if (!canBeInsideInv) return false;

            for (int x = startX; x <= endX; x++)
                for (int y = startY; y <= endY; y++)
                    if (IsOccupied(x, y)) return false;

            return true;
        }

        /// <summary>
        /// Checks if the specified element exists
        /// </summary>
        public bool Contains(Item item)
        {
            if (item == null) return false;

            return _dict.ContainsKey(item);
        }

        /// <summary>
        /// Checks if the specified position is occupied
        /// </summary>
        public bool IsOccupied(Vector2Int position)
        {
            return IsOccupied(position.x, position.y);
        }

        public bool IsOccupied(int x, int y)
        {
            return x >= 0 && y >= 0 &&
                   x < _width && y < _height &&
                   _matrix[x, y] != null;
        }

        /// <summary>
        /// Checks if the specified position is free
        /// </summary>
        public bool IsFree(Vector2Int position)
        {
            return IsFree(position.x, position.y);
        }

        public bool IsFree(int x, int y)
        {
            return !IsOccupied(x, y);
        }

        /// <summary>
        /// Removes specified item
        /// </summary>
        public bool RemoveItem(Item item)
        {
            if (!_dict.ContainsKey(item)) return false;

            var pos = _dict[item];
            RemoveFromMatrix(item);
            _dict.Remove(item);
            OnRemoved?.Invoke(item, pos);

            return true;
        }

        public bool RemoveItem(Item item, out Vector2Int position)
        {
            position = default;
            if (item == null) return false;

            if (!_dict.ContainsKey(item)) return false;
            position = _dict[item];

            return RemoveItem(item);
        }

        private void RemoveFromMatrix(Item item) 
        {
            var startPos = _dict[item];
            GetEndPositions(item.Size.x, item.Size.y, startPos.x, startPos.y, out int endX, out int endY);

            for (int x = startPos.x; x <= endX; x++)
                for (int y = startPos.y; y <= endY; y++)
                    _matrix[x, y] = null;
        }

        /// <summary>
        /// Returns an item at specified position 
        /// </summary>
        public Item GetItem(Vector2Int position)
        {
            return GetItem(position.x, position.y);
        }

        public Item GetItem(int x, int y)
        {
            if (CheckOutOfBounds(x, y)) throw new IndexOutOfRangeException("indexes are out of range");

            return IsOccupied(x, y) ? _matrix[x, y] : null;
        }

        public bool TryGetItem(Vector2Int position, out Item item)
        {
            item = null;
            if (CheckOutOfBounds(position.x, position.y)) return false;

            return TryGetItem(position.x, position.y, out item);
        }

        public bool TryGetItem(int x, int y, out Item item)
        {
            item = null;
            if (CheckOutOfBounds(x, y)) return false;

            item = GetItem(x, y);
            return item != null;
        }

        private bool CheckOutOfBounds(int x, int y) => x < 0 || y < 0 || x >= _width || y >= _height;

        /// <summary>
        /// Returns positions of a specified item 
        /// </summary>
        public Vector2Int[] GetPositions(Item item)
        {
            if (item == null) throw new NullReferenceException("item is null");

            if (!_dict.ContainsKey(item)) throw new KeyNotFoundException("item is not present in the dictionary");

            var startPos = _dict[item];
            GetEndPositions(item.Size.x, item.Size.y, startPos.x, startPos.y, out int endX, out int endY);
            Vector2Int[] array = new Vector2Int[item.Size.x * item.Size.y];

            int index = 0;
            for (int x = startPos.x; x <= endX; x++)
                for (int y = startPos.y; y <= endY; y++) 
                {
                    array[index] = new Vector2Int(x, y);
                    index++;
                }

            return array;
                    
        }

        public bool TryGetPositions(Item item, out Vector2Int[] positions)
        {
            positions = null;
            if (!Contains(item)) return false;

            positions = GetPositions(item);
            return positions != null;
        }

        /// <summary>
        /// Clears all items 
        /// </summary>
        public void Clear()
        {
            if (_dict.Count == 0) return;

            _dict.Clear();

            for (int columnIndex = 0; columnIndex < _height; columnIndex++)
            {
                for (int rowIndex = 0; rowIndex < _width; rowIndex++)
                {
                    _matrix[rowIndex, columnIndex] = null;
                }
            }

            OnCleared?.Invoke();
        }

        /// <summary>
        /// Returns count of items with a specified name
        /// </summary>
        public int GetItemCount(string name)
        {
            int quantity = 0;
            foreach (var item in this)
                if (item.Name == name) quantity++;

            return quantity;
        }

        public bool MoveItem(Item item, Vector2Int position)
        {
            if (item == null) throw new ArgumentNullException("item is null");

            if (!Contains(item)) return false;

            var previousPos = _dict[item];
            GetEndPositions(item.Size.x, item.Size.y, position.x, position.y, out int endX, out int endY);
            if (!IsFreeSpaceForMove(item, position.x, position.y, endX, endY)) 
                return false;

            RemoveFromMatrix(item);
            AddToMatrix(item, position.x, position.y);
            _dict[item] = position;

            OnMoved?.Invoke(item, position);
            return true;
        }

        private bool IsFreeSpaceForMove(Item item, int startX, int startY, int endX, int endY)
        {
            bool canBeInsideInv = startX >= 0 &&
                 startY >= 0 &&
                 endX < _width &&
                 endY < _height;

            if (!canBeInsideInv) return false;

            for (int x = startX; x <= endX; x++)
                for (int y = startY; y <= endY; y++)
                    if (IsOccupied(x, y) && GetItem(x, y) != item) return false;

            return true;
        }

        /// <summary>
        /// Rearranges an inventory space with max free slots 
        /// </summary>
        public void OptimizeSpace()
        {
            if (_dict.Count == 0)
                return;

            List<OptimizationEntry> optimizationEntries = new List<OptimizationEntry>();

            int index = 0;

            foreach (Item item in _dict.Keys)
            {
                optimizationEntries.Add(new OptimizationEntry(item, index));
                index++;
            }

            optimizationEntries.Sort(CompareItemsForOptimization);
            Clear();

            foreach (var entry in optimizationEntries) 
            {
                var item = entry.Item;

                Vector2Int position;

                if (!FindFreePosition(item.Size.x, item.Size.y, out position))
                {
                    throw new InvalidOperationException(
                        $"Cannot optimize inventory: item '{item}' does not fit."
                    );
                }

                _dict.Add(item, position);
                AddToMatrix(item, position.x, position.y);

                OnMoved?.Invoke(item, position);
            }        
        }

        private struct OptimizationEntry
        {
            public Item Item;
            public int Index;

            public OptimizationEntry(Item item, int index)
            {
                Item = item;
                Index = index;
            }
        }

        private int CompareItemsForOptimization(OptimizationEntry a, OptimizationEntry b)
        {
            int areaA = a.Item.Size.x * a.Item.Size.y;
            int areaB = b.Item.Size.x * b.Item.Size.y;

            if (areaA != areaB)
                return areaB.CompareTo(areaA);

            if (a.Item.Size.x != b.Item.Size.x)
                return b.Item.Size.x.CompareTo(a.Item.Size.x);

            if (a.Item.Size.y != b.Item.Size.y)
                return b.Item.Size.y.CompareTo(a.Item.Size.y);

            return a.Index.CompareTo(b.Index);
        }      

        /// <summary>
        /// Iterates by all items 
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dict.Keys.GetEnumerator();
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return _dict.Keys.GetEnumerator();
        }

        /// <summary>
        /// Copies items to a specified matrix
        /// </summary>
        public void CopyTo(Item[,] matrix)
        {
            for (int columnIndex = 0; columnIndex < _height; columnIndex++)
            {
                for (int rowIndex = 0; rowIndex < _width; rowIndex++)
                {
                    matrix[rowIndex, columnIndex] = _matrix[rowIndex, columnIndex];
                }
            }
        }

        /// <summary>
        /// Returns an inventory matrix in string format
        /// </summary>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"Inventory: {_width}x{_height}, Items: {_dict.Count}");

            for (int columnIndex = 0; columnIndex < _height; columnIndex++)
            {
                for (int rowIndex = 0; rowIndex < _width; rowIndex++)
                {
                    Item item = _matrix[rowIndex, columnIndex];

                    if (item == null)
                    {
                        builder.Append("[.]");
                    }
                    else
                    {
                        string itemName = string.IsNullOrEmpty(item.Name) ? "?" : item.Name;
                        builder.Append($"[{itemName}]");
                    }
                }

                if (columnIndex < _height - 1)
                    builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}