using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using MapInfo.Mapping;
using MapInfo.Data;
using MapInfo.Geometry;
using MapInfo.Styles;


/// <summary>
/// UserMap 的摘要说明
/// design by shinbo
/// email:shinbo@hotmail.com
/// </summary>

namespace User{

    /// <summary>
	/// Summary description for SampleConstants.
	/// </summary>
	public class Constants
	{
		public static short  POINT_CODE = 57;
        public static double POINT_SIZE = 24;

        public static string TempLayerAlias = "tempTracklayer";
        public static string TempTableAlias = "tempTracktable";

        public static double MINZOOMVALUE = 47814;
        public static double MAXZOOMVALUE = 100;

        public static string BMPFILENAME = "TRUC-64.BMP";
        private Constants() { }
	}

    public class UserMap
    {
        public UserMap()
        {

        }

        /// <summary>
        /// <param name="strTable">表名</param>
        /// <param name="strLayer">图层名</param>
        /// </summary>
        public static bool CreateTempLayer(Map oMap, string strTable, string strLayer)
        {
            //确保当前目录下不存在同名表
            MapInfo.Data.Table tblTemp = MapInfo.Engine.Session.Current.Catalog.GetTable(strTable);
            if (tblTemp != null)
            {
                MapInfo.Engine.Session.Current.Catalog.CloseTable(strTable);
            }

            //指定表名建立表信息
            MapInfo.Data.TableInfoMemTable tblInfoTemp = new MapInfo.Data.TableInfoMemTable(strTable);
            //向表信息中添加可绘图列
            tblInfoTemp.Columns.Add(MapInfo.Data.ColumnFactory.CreateFeatureGeometryColumn(oMap.GetDisplayCoordSys()));
            tblInfoTemp.Columns.Add(MapInfo.Data.ColumnFactory.CreateStyleColumn());
            //向表信息中添加自定义列
            //tblInfoTemp.Columns.Add(MapInfo.Data.ColumnFactory.CreateIntColumn("index"));
            tblInfoTemp.Columns.Add(MapInfo.Data.ColumnFactory.CreateStringColumn("uid", 128));
            tblInfoTemp.Columns.Add(MapInfo.Data.ColumnFactory.CreateStringColumn("name", 255));

            //根据表信息创建临时表
            tblTemp = MapInfo.Engine.Session.Current.Catalog.CreateTable(tblInfoTemp);

            FeatureLayer tempLayer = new FeatureLayer(tblTemp, strLayer, strLayer);
            oMap.Layers.Add(tempLayer);
            return true;
        }
        /// <summary>
        /// 添加标注图层
        /// <param name="tableName">标注的表名</param>
        /// <param name="columnName">标注的列名</param>
        /// </summary>
        public static bool ShowLabelLayer(Map oMap, string tableName, string columnName)
        {
            if (oMap == null)
            {
                return false;
            }

            //新建标注图层
            LabelLayer labellayer = new LabelLayer();
            oMap.Layers.Add(labellayer);

            //指定要标注的数据表
            LabelSource labelSource = new LabelSource(MapInfo.Engine.Session.Current.Catalog.GetTable(tableName));
            labellayer.Sources.Append(labelSource);

            //指定要标准字段所在的列
            labelSource.DefaultLabelProperties.Caption = columnName;
            //labelSource.DefaultLabelProperties.Visibility.Enabled = true;
            //labelSource.DefaultLabelProperties.Visibility.VisibleRangeEnabled = true;
            //labelSource.DefaultLabelProperties.Visibility.VisibleRange = new VisibleRange(0.01, 10, MapInfo.Geometry.DistanceUnit.Mile);
            //labelSource.DefaultLabelProperties.Visibility.AllowDuplicates = true;
            //labelSource.DefaultLabelProperties.Visibility.AllowOverlap = true;
            //labelSource.DefaultLabelProperties.Visibility.AllowOutOfView = true;
            //labelSource.Maximum = 50;
            //labelSource.DefaultLabelProperties.Layout.UseRelativeOrientation = true;
            //labelSource.DefaultLabelProperties.Layout.RelativeOrientation = MapInfo.Text.RelativeOrientation.FollowPath;
            //labelSource.DefaultLabelProperties.Layout.Angle = 33.0;
            //labelSource.DefaultLabelProperties.Priority.Major = "index";
            labelSource.DefaultLabelProperties.Layout.Offset = 7;
            labelSource.DefaultLabelProperties.Layout.Alignment = MapInfo.Text.Alignment.BottomCenter;
            //Font font = new Font("黑体", 10);
            //font.ForeColor = System.Drawing.Color.DarkBlue;
            //labelSource.DefaultLabelProperties.Style.Font = font;
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oMap"></param>
        /// <param name="strLayer"></param>
        /// <param name="strUID"></param>
        /// <returns></returns>
        public static bool DeleteFeature(Map oMap, string strLayer, string strUID)
        {

            if (oMap == null)       return false;
            FeatureLayer layer = oMap.Layers[strLayer] as FeatureLayer;
            if (layer == null)      return false;

            string searchstring = "uid='" + strUID + "'";
            SearchInfo searchInfo = MapInfo.Data.SearchInfoFactory.SearchWhere(searchstring);
            Feature feature = MapInfo.Engine.Session.Current.Catalog.SearchForFeature(layer.Table, searchInfo);
            if (feature == null)    return false;
            layer.Table.DeleteFeature(feature);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oMap"></param>
        /// <param name="strLayer"></param>
        /// <param name="strUID"></param>
        /// <param name="strName"></param>
        /// <param name="dPoint"></param>
        /// <param name="nCode"></param>
        /// <param name="dbCodeSize"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static bool AddMarker( Map   oMap, 
                                    string  strLayer, 
                                    string  strUID,
                                    string  strText, 
                                    DPoint  dPoint,
                                    int     nCode,
                                    double  dbCodeSize,
                                    System.Drawing.Color color)
        {
            if (oMap == null) return false;
            FeatureLayer layer = oMap.Layers[strLayer] as FeatureLayer;
            if (layer == null) return false;

            //创建点图元及其样式
            FeatureGeometry fg = new Point(oMap.GetDisplayCoordSys(), dPoint);
            CompositeStyle  cs = new MapInfo.Styles.CompositeStyle(new SimpleVectorPointStyle((short)nCode, color, dbCodeSize));					
   				
   					
            Feature feature = new Feature( layer.Table.TableInfo.Columns );
            feature.Geometry = fg;
            feature.Style = cs;
            feature["uid"] = strUID;
            feature["name"] = strText;            

            //fg.GetGeometryEditor().Rotate(dPoint, 90);
   			//fg.EditingComplete();	
            //MapInfo.Geometry.IGeometryEdit edit = feature.Geometry.GetGeometryEditor(); 
            //edit.OffsetByAngle(90, 500, MapInfo.Geometry.DistanceUnit.Meter, MapInfo.Geometry.DistanceType.Spherical); 
            //edit.Geometry.EditingComplete();

            layer.Table.InsertFeature(feature);
            return true;

        }
        public static bool AddMarker(Map oMap,
                                    string strLayer,
                                    string strUID, 
                                    string strText, 
                                    DPoint dPoint,
                                    string strBmpName,
                                    double dbSize,
                                    System.Drawing.Color color ){

            if (oMap == null)   return false;
            FeatureLayer layer = (FeatureLayer)oMap.Layers[strLayer];
            if (layer == null)  return false;

            FeatureGeometry     fg = new Point(oMap.GetDisplayCoordSys(), dPoint);
            CompositeStyle      cs = new CompositeStyle(new BitmapPointStyle(strBmpName, BitmapStyles.None, color, dbSize));

            Feature feature = new Feature(layer.Table.TableInfo.Columns);
            feature.Geometry = fg;
            feature.Style = cs;
            feature["uid"] = strUID;
            feature["name"] = strText;

            //feature.Geometry.GetGeometryEditor().Rotate(dPoint, 90);
            //feature.Geometry.EditingComplete ();   
          
            layer.Table.InsertFeature(feature);
            return true;
        }
        
        /// <summary>
        /// 向图层中添加线段
        /// </summary>
        public static bool AddLine(Map oMap, 
                                    string strLayer,
                                    string strUID,
                                    string strText, 
                                    DPoint[] dPoint, 
                                    System.Drawing.Color color, 
                                    int lineWidth)
        {
            //获取图层和表
            FeatureLayer layer = (FeatureLayer)oMap.Layers[strLayer];

            //创建线图元及其样式            
            FeatureGeometry fg = new MultiCurve( layer.CoordSys, CurveSegmentType.Linear, dPoint );            
            CompositeStyle  cs = new MapInfo.Styles.CompositeStyle( new SimpleLineStyle(  new LineWidth(lineWidth, LineWidthUnit.Pixel), 2, color ) );

            MapInfo.Data.Feature feature = new MapInfo.Data.Feature(layer.Table.TableInfo.Columns);

            feature.Geometry = fg;
            feature.Style = cs;
            feature["uid"] = strUID;
            feature["name"] = strText;
            
            //将线图元加入图层
            layer.Table.InsertFeature( feature );
            return true;
        }

        /// <summary>
        /// set map center point
        /// </summary>
        /// <param name="map"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static bool Center(MapInfo.Mapping.Map map, double x, double y, double zoom )
        {
            map.Center = new DPoint(x, y);
            map.Zoom = new Distance(zoom, map.Zoom.Unit);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static bool ZoomAll( MapInfo.Mapping.Map map ){
            map.SetView(map.Layers.Bounds, map.GetDisplayCoordSys());
            return true;
        }
        /// <summary>
        /// 	
        /// </summary>
        /// <param name="paramValue">#RRGGBB00, RRGGBB00</param>
        /// <returns>System.Drawing.Color</returns>
        public static System.Drawing.Color stringToColor(System.String paramValue)
        {
            int red;
            int green;
            int blue;
            int a = 0;
            System.Drawing.Color color;

            if (paramValue.Length <= 0)
            {
                color = System.Drawing.Color.FromName("Red");
            }
            else
            {
                if (paramValue[0] == '#')
                {
                    paramValue = paramValue.Trim('#');
                    red = (System.Int32.Parse(paramValue.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
                    green = (System.Int32.Parse(paramValue.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
                    blue = (System.Int32.Parse(paramValue.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
                    if (paramValue.Length > 6)
                    {
                        a = (System.Int32.Parse(paramValue.Substring(6, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
                    }
                    color = System.Drawing.Color.FromArgb(red, green, blue, a);
                }
                else
                {
                    color = System.Drawing.Color.FromName(paramValue);
                }
            }
            return color;
        }

    }
}
