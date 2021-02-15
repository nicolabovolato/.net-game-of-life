using System;
using System.Numerics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GameOfLife
{
    class World
    {
        private int WorldSize { get; set; }

        private BigInteger State { get; set; }

        private List<BigInteger> _states = new List<BigInteger>();
        private ReadOnlyCollection<BigInteger> States => _states.AsReadOnly();

        public bool Stable { get; private set; }

        private const int MAX_WORLD_SIZE = 50;
        private const int UNDERPOPULATION_TRESHOLD = 2;
        private const int OVERPOPULATION_TRESHOLD = 3;
        private const int REPRODUCTION_TRIGGER = 3;

        public World(BigInteger seed, int worldSize)
        {
            this.WorldSize = worldSize;
            this.State = seed;
            this._states.Add(State);

            if (WorldSize > MAX_WORLD_SIZE) throw new Exception("World size exceeds maximum allowed.");
            if (WorldSize < 1) throw new Exception("World size must be greater than 0.");

        }

        public void Advance()
        {
            if (Stable) return;

            BigInteger newWorld = this.State;

            for (int i = 0; i < Math.Pow(WorldSize, 2); i++)
            {

                int cellCount = CountNearbyCells(i);

                int current = GetCell(i);

                if (current == 1 && (cellCount < UNDERPOPULATION_TRESHOLD || cellCount > OVERPOPULATION_TRESHOLD))
                {
                    newWorld = newWorld ^ (BigInteger.One << i);
                }
                else if (current == 0 && cellCount == REPRODUCTION_TRIGGER)
                {
                    newWorld = newWorld | (BigInteger.One << i);
                }
            }

            if (States.Contains(newWorld))
            {
                Stable = true;
                return;
            }

            State = newWorld;

            _states.Add(State);
        }

        private int CountNearbyCells(int index)
        {
            int cell_count = 0;

            bool first_col = index % WorldSize == 0;
            bool last_col = (index + 1) % WorldSize == 0;
            bool first_row = index < WorldSize;
            bool last_row = index > (Math.Pow(WorldSize, 2) - WorldSize);

            if (!first_col) cell_count += GetCell(index - 1);
            if (!last_col) cell_count += GetCell(index + 1);

            if (!first_row)
            {
                cell_count += GetCell(index - WorldSize);
                if (!first_col) cell_count += GetCell(index - WorldSize - 1);
                if (!last_col) cell_count += GetCell(index - WorldSize + 1);
            }

            if (!last_row)
            {
                cell_count += GetCell(index + WorldSize);
                if (!first_col) cell_count += GetCell(index + WorldSize - 1);
                if (!last_col) cell_count += GetCell(index + WorldSize + 1);
            }

            return cell_count;
        }

        private int GetCell(int index)
        {
           return (State & BigInteger.Pow(2, index)) >> index == 1 ? 1 : 0;
        }

        public override string ToString()
        {
            string worldStr = "";

            string spacer = new String('─', WorldSize * 2);

            string openingStr = $"┌{spacer}┐\n";
            string closingStr = $"└{spacer}┘";

            for (int i = 0; i < Math.Pow(WorldSize, 2); i++)
            {

                if (i % WorldSize == 0)
                {
                    worldStr += "|";
                }

                int current = GetCell(i);

                worldStr += current == 1 ? "██" : "  ";

                if ((i + 1) % WorldSize == 0)
                {
                    worldStr += "|";
                    worldStr += "\n";
                }
            }

            return openingStr + worldStr + closingStr;
        }
    }
}
