using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Drawing;
using MapInfo.WebControls;
using MapInfo.Data;
using MapInfo.Engine;
using MapInfo.Geometry;
using MapInfo.Mapping;
using MapInfo.Styles;

namespace User{

    [Serializable]
    public class WheelZoom : MapBaseCommand
    {
        public WheelZoom()
        {
            Name = "WheelZoom";
        }

        public override void Process()
        {
            MapControlModel model = MapControlModel.GetModelFromSession();
            model.SetMapSize(MapAlias, MapWidth, MapHeight);
            MapInfo.Mapping.Map map = model.GetMapObj(MapAlias);
            if (map == null)
            {
                return;
            }
            int wheelvalue = int.Parse(System.Convert.ToString(HttpContext.Current.Request["wheelvalue"]));
            try
            {
                double db;
                if (wheelvalue > 0)
                {
                    db = map.Zoom.Value * 0.5;
                }
                else
                {
                    db = map.Zoom.Value * 2;
                }
                if (db > Constants.MINZOOMVALUE)
                {
                    db = Constants.MINZOOMVALUE;
                }
                else if (db < Constants.MAXZOOMVALUE)
                {
                    db = Constants.MAXZOOMVALUE;
                }
                map.Zoom = new MapInfo.Geometry.Distance(db, map.Zoom.Unit);

            }
            finally
            {
                System.IO.MemoryStream ms = model.GetMap(MapAlias, MapWidth, MapHeight, ExportFormat);
                StreamImageToClient(ms);
            }
        }
    }
    [Serializable]
    public class GetOverviewMapCommand : MapBaseCommand
    {
        public GetOverviewMapCommand()
        {
            Name = "GetOverviewMap";
        }
        public override void Process()
        {
            MapControlModel model = MapControlModel.GetModelFromSession();
            model.SetMapSize(MapAlias, MapWidth, MapHeight);
            MapInfo.Mapping.Map map = model.GetMapObj(MapAlias);
            if (map == null)
            {
                return;
            }
            MapInfo.Geometry.Distance savezoom = map.Zoom;
            map.Zoom = new MapInfo.Geometry.Distance( savezoom.Value*4, savezoom.Unit );

            if (ExportFormat == "undefined")
            {
                ExportFormat = "Gif";
            }
            System.IO.MemoryStream ms = model.GetMap(MapAlias, MapWidth, MapHeight, ExportFormat);
            StreamImageToClient(ms);
            map.Zoom = savezoom;

            /*
            MapControlModel model = MapControlModel.GetModelFromSession();
            Map oMap = model.GetMapObj(MapAlias);
            if (oMap == null)
            {
                return;
            }

            MapInfo.Geometry.Distance zoom = oMap.Zoom;
            zoom = new MapInfo.Geometry.Distance(zoom.Value * 4, zoom.Unit );
            oMap.Zoom = zoom;
            if (ExportFormat == "undefined" )
            {
                ExportFormat = "Gif";
            }
            System.IO.MemoryStream ms = model.GetMap(MapAlias, MapWidth, MapHeight, ExportFormat);
            StreamImageToClient(ms);
            zoom = new MapInfo.Geometry.Distance(zoom.Value / 4, zoom.Unit);
            oMap.Zoom = zoom;
             */
        }
    }
    [Serializable]
    public class AddPointCommand : MapBaseCommand
    {
        public AddPointCommand()
        {
            Name = "AddPointCommand";
        }
        public override void Process()
        {
            MapControlModel model = MapControlModel.GetModelFromSession();
            model.SetMapSize(MapAlias, MapWidth, MapHeight);
            MapInfo.Mapping.Map map = model.GetMapObj(MapAlias);
            if (map == null)
            {
                return;
            }

            string                  strUID = HttpContext.Current.Request["uid"];
            string                  strText = HttpContext.Current.Request["Name"];
            System.Drawing.Point[]  points = this.ExtractPoints(this.DataString);
            System.Drawing.Color    clr = UserMap.stringToColor( HttpContext.Current.Request["Color"] );

          
            double  x = (double)points[0].X / 10000;
            double  y = (double)points[0].Y / 10000;

            UserMap.DeleteFeature(map, Constants.TempLayerAlias, strUID);
            UserMap.AddMarker( map, Constants.TempLayerAlias, strUID, strText, new DPoint(x, y), Constants.BMPFILENAME,Constants.POINT_SIZE,clr );

            /*
            //获取图层和表
            FeatureLayer layer = (FeatureLayer)map.Layers[Constants.TempLayerAlias];
            if (layer == null)
            {
                return;
            }

            double x = points[0].X/10000;
            double y = points[0].Y/10000;
             
            
            //创建点图元及其样式
            FeatureGeometry fg = new MapInfo.Geometry.Point( map.GetDisplayCoordSys(), x, y);
            CompositeStyle cs = new CompositeStyle(
                new BitmapPointStyle(Constants.BMPFILENAME, BitmapStyles.None, Color.Red, Constants.POINT_SIZE)
                );


            Feature feature = new Feature(layer.Table.TableInfo.Columns);
            feature.Geometry = fg;
            feature.Style = cs;
            feature["name"] = name;

            //fg.GetGeometryEditor().Rotate(dPoint, 90);
            //fg.EditingComplete();


            //MapInfo.Geometry.IGeometryEdit edit = feature.Geometry.GetGeometryEditor(); 
            //edit.OffsetByAngle(90, 500, MapInfo.Geometry.DistanceUnit.Meter, MapInfo.Geometry.DistanceType.Spherical); 
            //edit.Geometry.EditingComplete();

            layer.Table.InsertFeature(feature);*/


            MemoryStream ms = model.GetMap(MapAlias, MapWidth, MapHeight, ExportFormat);
            StreamImageToClient(ms);
            return;
        }
    }
    [Serializable]
    public class DelFeatureCommand : MapBaseCommand
    {
        public DelFeatureCommand()
        {
            this.Name = "DelFeatureCommand";
        }

        public override void Process()
        {

            MapControlModel model = MapControlModel.GetModelFromSession();
            model.SetMapSize(MapAlias, MapWidth, MapHeight);
            MapInfo.Mapping.Map map = model.GetMapObj(MapAlias);
            if (map == null)
            {
                return;
            }

            string strUID = HttpContext.Current.Request["uid"];
            UserMap.DeleteFeature(map, Constants.TempLayerAlias, strUID);

            MemoryStream ms = model.GetMap(MapAlias, MapWidth, MapHeight, ExportFormat);
            StreamImageToClient(ms);
            return;
        }
    }
    [Serializable]
    public class AddLineCommand : MapBaseCommand
    {

        public AddLineCommand()
        {
            this.Name = "AddLineCommand";

        }
        public override void Process()
        {

            MapControlModel model = MapControlModel.GetModelFromSession();
            model.SetMapSize(MapAlias, MapWidth, MapHeight);
            MapInfo.Mapping.Map map = model.GetMapObj(MapAlias);
            if (map == null)
            {
                return;
            }

            string strUID = HttpContext.Current.Request["uid"];
            string strText = HttpContext.Current.Request["Name"];
            System.Drawing.Point[]  points = this.ExtractPoints(this.DataString);
            System.Drawing.Color    clr = UserMap.stringToColor(HttpContext.Current.Request["Color"]);
            int                     lineWidth = Int32.Parse(HttpContext.Current.Request["LineWidth"]);
            if (lineWidth <= 0)
            {
                lineWidth = 2;
            }

            int     cnt = 0;
            int     len = points.Length;
            double  x = 0;
            double  y = 0;

            MapInfo.Geometry.DPoint[] dPoints = new MapInfo.Geometry.DPoint[len];

            for (cnt = 0; cnt < len; cnt++)
            {
                x = (double)points[cnt].X / 10000;
                y = (double)points[cnt].Y / 10000;
                dPoints[cnt] = new MapInfo.Geometry.DPoint(x, y);
            }

            UserMap.DeleteFeature(map, Constants.TempLayerAlias, strUID);
            UserMap.AddLine(map, Constants.TempLayerAlias,strUID,strText, dPoints, clr, lineWidth);

            /*
            FeatureLayer layer = map.Layers[Constants.TempLayerAlias] as FeatureLayer;
            if (layer == null)
            {
                return;
            }

            int cnt = 0;
            int len = points.Length;
            double x =0;
            double y = 0;

            MapInfo.Geometry.DPoint[] dPoints = new MapInfo.Geometry.DPoint[len];

            for (cnt = 0; cnt < len; cnt++)
            {
            		x = points[cnt].X/10000;
            		y = points[cnt].Y/10000;
                dPoints[cnt] = new MapInfo.Geometry.DPoint(x, y);
            }


            //创建线图元及其样式            
            FeatureGeometry fg = new MultiCurve(layer.CoordSys, CurveSegmentType.Linear, dPoints);
            CompositeStyle cs = new MapInfo.Styles.CompositeStyle(
                new SimpleLineStyle(new LineWidth(linewidth, LineWidthUnit.Pixel), 2, Color.Red)
            );

            MapInfo.Data.Feature feature = new MapInfo.Data.Feature(layer.Table.TableInfo.Columns);

            feature.Geometry = fg;
            feature.Style = cs;
            feature["name"] = name;

            //将线图元加入图层
            layer.Table.InsertFeature(feature);
            */
            MemoryStream ms = model.GetMap(MapAlias, MapWidth, MapHeight, ExportFormat);
            StreamImageToClient(ms);
            return;
        }
    }
    [Serializable]
    public class CenterCommand : MapBaseCommand
    {
        public CenterCommand()
        {
            this.Name = "CenterCommand";

        }
        public override void Process()
        {
            MapControlModel model = MapControlModel.GetModelFromSession();
            model.SetMapSize(MapAlias, MapWidth, MapHeight);
            MapInfo.Mapping.Map map = model.GetMapObj(MapAlias);
            if (map == null)
            {
                return;
            }
            System.Drawing.Point[] points = this.ExtractPoints(this.DataString);

            double x = points[0].X / 10000;
            double y = points[0].Y / 10000;
            double zoom = Double.Parse(HttpContext.Current.Request["Zoom"]);

            if ( zoom == -1 )
            {
                zoom = map.Zoom.Value;
            }
            UserMap.Center( map, x, y, zoom );
            MemoryStream ms = model.GetMap(MapAlias, MapWidth, MapHeight, ExportFormat);
            StreamImageToClient(ms);
            return;
        }

    }
    [Serializable]
    public class ZoomAllCommand : MapBaseCommand
    {
        public ZoomAllCommand()
        {
            this.Name = "ZoomAllCommand";

        }
        public override void Process()
        {
            MapControlModel model = MapControlModel.GetModelFromSession();
            model.SetMapSize(MapAlias, MapWidth, MapHeight);
            MapInfo.Mapping.Map map = model.GetMapObj(MapAlias);

            if (map == null)
            {
                return;
            }
            UserMap.ZoomAll(map);
            MemoryStream ms = model.GetMap(MapAlias, MapWidth, MapHeight, ExportFormat);
            StreamImageToClient(ms);
            return;
        }

    }
    /*
    [Serializable]
    public class DelPointCommand : MapBaseCommand
    {
        public DelPointCommand()
        {
            this.Name = "DelPointCommand";
        }
        public override void Process()
        {

            System.Drawing.Point[] points = ExtractPoints(DataString);
            MapControlModel model = MapControlModel.GetModelFromSession();
            model.SetMapSize(MapAlias, MapWidth, MapHeight);
            MapInfo.Mapping.Map map = model.GetMapObj(MapAlias);
            if (map == null) return;
            PointDeletion(map, points[0]);
            MemoryStream ms = model.GetMap(MapAlias, MapWidth, MapHeight, ExportFormat);
            StreamImageToClient(ms);
        }
        public void PointDeletion(Map map, System.Drawing.Point point)
        {
            SearchInfo si = MapInfo.Mapping.SearchInfoFactory.SearchNearest(map, point, 10);
            (si.SearchResultProcessor as ClosestSearchResultProcessor).Options = ClosestSearchOptions.StopAtFirstMatch;

            Table table = MapInfo.Engine.Session.Current.Catalog[Constants.TempTableAlias];
            if (table != null)
            {
                IResultSetFeatureCollection ifc = Session.Current.Catalog.Search(table, si);
                foreach (Feature f in ifc)
                {
                    table.DeleteFeature(f);
                }
                ifc.Close();
            }

        }

    }
    [Serializable]
    public class ModifiedRadiusSelectionCommand : MapBaseCommand
    {
        public ModifiedRadiusSelectionCommand()
        {
            Name = "ModifiedRadiusSelectionCommand";
        }

        public override void Process()
        {

            MapControlModel model = MapControlModel.GetModelFromSession();
            model.SetMapSize(MapAlias, MapWidth, MapHeight);

            System.Drawing.Point[] points = ExtractPoints(DataString);
            Map map = model.GetMapObj(MapAlias);
            RadiusSelection(map, points);
            MemoryStream ms = model.GetMap(MapAlias, MapWidth, MapHeight, ExportFormat);
            StreamImageToClient(ms);
        }
        public void RadiusSelection(Map map, System.Drawing.Point[] points)
        {
            Session.Current.Selections.DefaultSelection.Clear();
            if (points.Length <= 1 || points[0] == points[1])
            {
                return;
            }
            IMapLayerFilter _selFilter = MapLayerFilterFactory.FilterForTools(
                map,
                MapLayerFilterFactory.FilterByLayerType(LayerType.Normal),
                MapLayerFilterFactory.FilterVisibleLayers(true),
                "MapInfo.Tools.MapToolsDefault.SelectLayers",
                null);
            // alias for temp selection object.
            string tempAlias = "tempSelection";
            ITableEnumerator iTableEnum = map.Layers.GetTableEnumerator(_selFilter);
            if (iTableEnum != null)
            {
                try
                {
                    
					// Get center and radius
					System.Drawing.Point center = points[0];
					int radius = points[1].X;
                    // search within screen radius.
					SearchInfo si = MapInfo.Mapping.SearchInfoFactory.SearchWithinScreenRadius(map, center, radius, 20, ContainsType.Centroid);
					Session.Current.Catalog.Search(iTableEnum, si, Session.Current.Selections.DefaultSelection, ResultSetCombineMode.AddTo);

                    // Create the temp selection object.
					Session.Current.Selections.CreateSelection(tempAlias);

                    // Search nearest the center point.
					si = MapInfo.Mapping.SearchInfoFactory.SearchNearest(map, center, 6);
					Session.Current.Catalog.Search(iTableEnum, si, Session.Current.Selections[tempAlias], ResultSetCombineMode.AddTo);
                    
					// Subtract radius selected features from point selected features.
					IEnumerator iEnum = Session.Current.Selections[tempAlias].GetEnumerator();
                    while (iEnum.MoveNext())
                    {
                        IResultSetFeatureCollection pntCollection = iEnum.Current as IResultSetFeatureCollection;

                        IResultSetFeatureCollection radiusCollection = null;
                        for (int index = 0; index < Session.Current.Selections.DefaultSelection.Count; index++)
                        {
                            // Need to find out the IResultSetFeatureCollection based on the same BaseTable.
                            if (Session.Current.Selections.DefaultSelection[index].BaseTable.Alias == pntCollection.BaseTable.Alias)
                            {
                                radiusCollection = Session.Current.Selections.DefaultSelection[index];
                                break;
                            }
                        }
                        if (radiusCollection != null)
                        {
                            // Remove features in pntCollection from radiusCollection.
                            radiusCollection.Remove(pntCollection);
                        }
                    }
                }
                catch (Exception)
                {
                    Session.Current.Selections.DefaultSelection.Clear();
                }
                finally
                {
                    Session.Current.Selections.Remove(Session.Current.Selections[tempAlias]);
                }

            }
        }
    }
     */

}