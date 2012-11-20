using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using HAP.Web.Configuration;

namespace HAP.Web.LiveTiles
{
    public class Tiles
    {
        public static TileGroup[] Current
        {
            get
            {
                List<LinkGroup> groups = new List<LinkGroup>();
                foreach (LinkGroup group in hapConfig.Current.Homepage.Groups.Values)
                    if (group.HideTopMenu == true) continue;
                    else if (group.ShowTo == "All") groups.Add(group);
                    else if (group.ShowTo != "None")
                    {
                        bool vis = false;
                        foreach (string s in group.ShowTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                            if (!vis) vis = HttpContext.Current.User.IsInRole(s);
                        if (vis) groups.Add(group);
                    }

                List<TileGroup> tiles = new List<TileGroup>();
                foreach (LinkGroup group in groups)
                {
                    List<Tile> tile = new List<Tile>();
                    foreach (Link link in group.FilteredLinks) tile.Add(new Tile { Name = link.Name, Url = link.Url, Target = link.Target, Description = link.Description, Icon = (string.IsNullOrEmpty(link.Icon) || link.Icon.StartsWith("#") ? "" : string.Format("~/api/tiles/icons/{0}/{1}/{2}", 64, 64, link.Icon.Remove(0, 2))), Color = (string.IsNullOrEmpty(link.Icon) || link.Icon.StartsWith("#") ? "" : HAP.Web.LiveTiles.IconCache.GetColour(link.Icon)).Replace('\'', '"').Replace("{ ", "{ \"").Replace(": ", "\":").Replace(", ", ",\"") });
                    tiles.Add(new TileGroup { Group = group.Name.Replace(" ", "").Replace("'", "").Replace(",", "").Replace(".", "").Replace("*", "").Replace("&", "").Replace("/", "").Replace("\\", ""), Tiles = tile.ToArray() });
                }
                return tiles.ToArray();
            }
        }
    }

    public class TileGroup 
    {
        public string Group { get; set; }
        public Tile[] Tiles { get; set; }
    }

    public class Tile
    {
        public string Name { get; set; }
        public string Target { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public string Url { get; set; }
    }
}
