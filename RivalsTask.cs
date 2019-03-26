using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rivals
{
   public class RivalsTask
   {
      private static HashSet<Point> _visited;
      private static Dictionary<Point, int> _distance;
      private static Queue<Point>[] _visit;

      public static IEnumerable<OwnedLocation> AssignOwners(Map map)
      {
         _visit = new Queue<Point>[map.Players.Length];
         _visited = new HashSet<Point>();
         _distance = new Dictionary<Point, int>();

         foreach (var player in Bfs(map))
            yield return player;
      }

      private static IEnumerable<OwnedLocation> Bfs(Map map)
      {
         while (true)
         {
            for (int i = 0; i < map.Players.Length; i++)
            {
               var start = map.Players[i];
               if (!_distance.ContainsKey(start))
               {
                  _visit[i] = new Queue<Point>();
                  _visit[i].Enqueue(start);
                  _distance[start] = 0;
                  _visited.Add(start);
                  yield return new OwnedLocation(i, start, 0);
               }

               if (_visit[i].Count == 0) continue;
               var node = _visit[i].Dequeue();
               foreach (var next in Nodes(node))
               {
                  if (!map.InBounds(next)) continue;
                  if (map.Maze[next.X, next.Y] != MapCell.Empty) continue;
                  if (_visited.Contains(next)) continue;
                  if (_visit[i].Contains(next)) continue;
                  _visit[i].Enqueue(next);
                  _visited.Add(next);
                  yield return new OwnedLocation(i, next, _distance[next]);
               }

               if (_visit.All(x=>x.Count == 0)) yield break;
            }
         }
      }

      private static IEnumerable<Point> Nodes(Point point)
      {
         for (var dy = 1; dy >= -1; dy--)
            for (var dx = 1; dx >= -1; dx--)
               if (dx != 0 && dy != 0)
                  continue;
               else
               {
                  var next = new Point
                  {
                     X = point.X + dx,
                     Y = point.Y + dy
                  };
                  if(next.Equals(point)) continue;
                  _distance[next] = _distance[point] + 1;
                  yield return next;
               }
      }
   }
}
