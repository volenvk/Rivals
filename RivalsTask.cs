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
      public static IEnumerable<OwnedLocation> AssignOwners(Map map)
      {
         var visited = new HashSet<Point>();
         var distance = new Dictionary<Point, int>();
         var visit = new Queue<KeyValuePair<Point, int>>(map.Players.Select((x, i) => new KeyValuePair<Point, int>(x, i)));

         while(true)
         {
            if (!visit.Any()) yield break; 
            var node = visit.Dequeue();
            if (!distance.ContainsKey(node.Key)) distance[node.Key] = 0;
            yield return new OwnedLocation(node.Value, node.Key, distance[node.Key]);
            visited.Add(node.Key);
            var ee = Nodes(node.Key, distance).GetEnumerator();

            while (ee.MoveNext())
            {
               var next = ee.Current;
               if (!map.InBounds(next)) continue;
               if (map.Maze[next.X, next.Y] != MapCell.Empty) continue;
               if (visited.Contains(next)) continue;
               var player = new KeyValuePair<Point, int>(next, node.Value);
               if (visit.Contains(player)) continue;
               visit.Enqueue(player);
               visited.Add(next);
            }
         }
      }     

      private static IEnumerable<Point> Nodes(Point point, IDictionary<Point, int> distance)
      {
         for (var dy = 1; dy >= -1; dy--)
            for (var dx = 1; dx >= -1; dx--)
               if (dx != 0 && dy != 0);
               else
               {
                  var next = new Point
                  {
                     X = point.X + dx,
                     Y = point.Y + dy
                  };
                  if(next.Equals(point)) continue;
                  distance[next] = distance[point] + 1;
                  yield return next;
               }
      }
   }
}