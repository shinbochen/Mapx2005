using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;



using ApplicationStateManager;
using MapInfo.WebControls;
using MapInfo.Geometry;
using MapInfo.Mapping;

using User;

public partial class _Default : System.Web.UI.Page 
{
    private void Page_Load(object sender, System.EventArgs e)
    {
        if (IsPostBack == false)
        {
            // If the StateManager doesn't exist in the session put it else get it.
            if (StateManager.GetStateManagerFromSession() == null)
            {
                StateManager.PutStateManagerInSession(new AppStateManager());
                StateManager.GetStateManagerFromSession().ParamsDictionary[StateManager.ActiveMapAliasKey] = MapControl1.MapAlias;
            }
            else
            {
                MapControl1.MapAlias = StateManager.GetStateManagerFromSession().ParamsDictionary[StateManager.ActiveMapAliasKey] as string;
            }

            //   StateManager.GetStateManagerFromSession().ParamsDictionary[StateManager.ActiveMapAliasKey] = MapControl1.MapAlias;
            StateManager.GetStateManagerFromSession().RestoreState();

            if (Session.IsNewSession)
            {
                MapControlModel model = MapControlModel.SetDefaultModelInSession();
                model.Commands.Add(new WheelZoom());
                model.Commands.Add(new GetOverviewMapCommand());
                model.Commands.Add(new AddPointCommand());
                model.Commands.Add(new DelFeatureCommand());
                model.Commands.Add(new AddLineCommand());
                model.Commands.Add(new CenterCommand());
                model.Commands.Add(new ZoomAllCommand());
                InitWorkingLayer();
            }
        }
    }

    /// <summary>
    /// status display
    /// </summary>
    protected void StatusDisplay()
    {
       // LABEL1.Text = GetMapObj().Zoom.Value + "||" + GetMapObj().GetDisplayCoordSys().ToString();
    }
    /// <summary>
    /// shinbo added
    /// 更改坐标系为LON/LAT方式
    /// add templayer
    /// add labellayer
    /// </summary>
    protected void InitWorkingLayer()
    {
        Map map = GetMapObj();
        //CoordSys csysWGS84 = MapInfo.Engine.Session.Current.CoordSysFactory.CreateCoordSys("EPSG:4326");
        //map.SetDisplayCoordSys(csysWGS84);
        UserMap.CreateTempLayer(map, Constants.TempTableAlias, Constants.TempLayerAlias);
        UserMap.ShowLabelLayer(map, Constants.TempTableAlias, "name");

    }
    /// <summary>
    /// init map path
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void InitMapPath(object sender, System.EventArgs e)
    {
                
        DropDownList1.Items.Clear();
        string strMap = ConfigurationManager.AppSettings["MapName"];

        string[] strArrMap = strMap.Split(';');

        for (int cnt = 0; cnt < strArrMap.Length; cnt++)
        {
            DropDownList1.Items.Add(strArrMap[cnt]);
        }
    }
    /// <summary>
    /// change the map
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    protected void ChangeMap(object sender, System.EventArgs e)
    {
        /*
        string  name = DropDownList1.SelectedItem.Text;

        Map map = GetMapObj();
        name = Server.MapPath("App_data") + "\\" + name;
        //LABEL1.Text = name;        

        MapWorkSpaceLoader MapWSL = new MapWorkSpaceLoader( name );
        map.Layers.Clear();
        map.Load(MapWSL);
        // very important
        map.Alias = MapControl1.MapAlias;

       	InitWorkingLayer();
        StateManager.GetStateManagerFromSession().SaveState();*/
        string strMap = DropDownList1.SelectedItem.Text;
        Trace.Write(MapControl1.MapAlias);
        int nRes = strMap.IndexOf('(');
        if (nRes != -1)
        {
            string strAlias = strMap.Substring(nRes+1);
            nRes = strAlias.IndexOf(')');
            if (nRes != -1)
            {
                strAlias = strAlias.Substring(0, nRes);
                strAlias.Trim();
                if (MapControl1.MapAlias != strAlias)
                {
                    MapControl1.MapAlias = strAlias; 
                    StateManager.GetStateManagerFromSession().ParamsDictionary[StateManager.ActiveMapAliasKey] = MapControl1.MapAlias;
                    InitWorkingLayer();
                    StateManager.GetStateManagerFromSession().SaveState();
                }
            }
        }
    }

    protected void btnZoomAll(object sender, System.EventArgs e)
    {
        Map map = GetMapObj();
        UserMap.ZoomAll(map);
        //map.SetView(map.Layers.Bounds, map.GetDisplayCoordSys());
    } 
    /// <summary>
    /// Get the current map object
    /// </summary>
    /// <returns></returns>
    private MapInfo.Mapping.Map GetMapObj()
    {
        string strAlias = (string)StateManager.GetStateManagerFromSession().ParamsDictionary[StateManager.ActiveMapAliasKey];

        MapInfo.Mapping.Map myMap = MapInfo.Engine.Session.Current.MapFactory[strAlias];
        if (myMap == null)
        {
            myMap = MapInfo.Engine.Session.Current.MapFactory[0];
        }
        return myMap;
    }
    ///<summary>
    /// At the time of unloading the page, save the state
    ///</summary>
    private void Page_UnLoad(object sender, System.EventArgs e)
    {	// Save state.

        if (StateManager.GetStateManagerFromSession() != null)
        {
            StateManager.GetStateManagerFromSession().SaveState();
        }
    }

}
