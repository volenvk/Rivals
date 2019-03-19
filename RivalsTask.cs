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
      private static Queue<Queue<Point>>[] _visit;
      
      public static IEnumerable<OwnedLocation> AssignOwners(Map map)
      {
         var length = map.Players.Length;
         _visit = new Queue<Queue<Point>>[length];
         _visited = new HashSet<Point>();
         _distance = new Dictionary<Point, int>();

         foreach (var location in StartPositions(map.Players))
            yield return location;

         var i = 0;
         var e = map.Players.AsEnumerable().GetEnumerator();
         while (true)
         {
            if (!e.MoveNext())
            {
               i = 0;
               e.Reset();
               e.MoveNext();
            }

            var result = GetPointsPlayer(map, e.Current, i);
            var points = result.GetEnumerator();
            while (points.MoveNext())
               yield return points.Current;
            i++;
         }   
      }
      
      private static OwnedLocation StartPosition(Point start, int i)
      {
         _visit[i] = new Queue<Queue<Point>>();
         _visit[i].Enqueue(new Queue<Point>(new []{start}));
         _distance[start] = 0;
         _visited.Add(start);  
         return new OwnedLocation(i, start, 0);
      }
     
      private static IEnumerable<OwnedLocation> GetPointsPlayer(Map map, Point start, int i)
      {
         if (_visit[i].Count == 0) yield break;
         if (!_distance.ContainsKey(start))
         {
            _visit[i] = new Queue<Queue<Point>>();
            _visit[i].Enqueue(new Queue<Point>(new []{start}));
            _distance[start] = 0;
            _visited.Add(start);   
         }
         foreach (var ownedLocation in Bfs(map, i))
            yield return ownedLocation;
      }
     
      private static IEnumerable<OwnedLocation> StartPositions(Point[] players)
      {
         if(players == null) yield break;
         var length = players.Length;
         for (var i = 0; i < length; i++)
         {
            var start = players[i];
            _visit[i] = new Queue<Queue<Point>>();
            _visit[i].Enqueue(new Queue<Point>(new []{start}));
            _distance[start] = 0;
            _visited.Add(start);  
            yield return new OwnedLocation(i, start, 0);
         }
      }
      
      private static IEnumerable<OwnedLocation> GetPointsPlayers(Map map, Point[] players)
      {
         var length = players.Length;
         while (true)
         {
            if (length == 0) yield break;
            for (var i = 0; i < length; i++)
            {
               var start = players[i];
               if (!_distance.ContainsKey(start))
               {
                  _visit[i] = new Queue<Queue<Point>>();
                  _visit[i].Enqueue(new Queue<Point>(new []{start}));
                  _distance[start] = 0;
                  _visited.Add(start);  
               }
               if (_visit.All(x=>x.Count == 0)) yield break;
               if (_visit[i].Count == 0) continue;
               foreach (var ownedLocation in Bfs(map, i))
                  yield return ownedLocation;
            }
         }
      }
      
      private static IEnumerable<OwnedLocation> Bfs(Map map, int player)
      {
         var visit = new Queue<Point>();
         var points = _visit[player].Dequeue();

         while (points.Count > 0)
         {
            var node = points.Dequeue();
            var nodes = Nodes(node);
            if (nodes == null) continue;
            foreach (var next in nodes)
            {
               if (!map.InBounds(next)) continue;
               if (map.Maze[next.X, next.Y] != MapCell.Empty) continue;
               if (_visited.Contains(next)) continue;
               if (visit.Contains(next)) continue;
               visit.Enqueue(next);
               _visited.Add(next);
               yield return new OwnedLocation(player, next, _distance[next]);
            }
         }
         
         if(visit.Count == 0) yield break;
         _visit[player].Enqueue(visit);
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