using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MapInfo.Engine;
using MapInfo.Geometry;
using MapInfo.Mapping;
using MapInfo.WebControls; 
using User;
  
/// <summary>
/// change this file will change application rebuild
/// Summary description for AppStateManager
/// </summary>
namespace ApplicationStateManager  
{
    /// <summary>
    /// State management can be complex operation. It is efficient to save and restore what is needed.
    /// The method used here is described in the BEST PRACTISES documentation. This is a template application
    /// which changes zoom, center, default selection and layer visibility. Hence we save and restore only these objects.
    /// </summary>
    [Serializable]
    public class AppStateManager : StateManager  
    {
        private ManualSerializer _session = null;

        public AppStateManager()
        {
            _session = new ManualSerializer();
        }

        private Map GetMapObj(string mapAlias)
        {
            Map map = null;
            if (mapAlias == null || mapAlias.Length <= 0)
            {
                map = MapInfo.Engine.Session.Current.MapFactory[0];
            }
            else
            {
                map = MapInfo.Engine.Session.Current.MapFactory[mapAlias];
                if (map == null) map = MapInfo.Engine.Session.Current.MapFactory[0];
            }
            return map;
        }

        // Restore the state
        public override void RestoreState()
        { 
            string mapAlias = ParamsDictionary[ActiveMapAliasKey] as string;
            Map map = GetMapObj(mapAlias);
            if (map == null)
            {
                return; 
            }
            
            // If it was user's first time and the session was not dirty then save this default state to be applied later.
            // If it was a users's first time and the session was dirty then apply the default state  saved in above step to give users a initial state.
            if (IsUsersFirstTime())
            {
                if (IsDirtyMapXtremeSession(map))
                {
                    RestoreDefaultState(map);
                }  
                else
                {
                    SaveDefaultState(map);
                }
            }
            else
            {
                // If it is not user's first time then restore the last state they saved
                RestoreZoomCenterState(map);
                //ManualSerializer.RestoreMapXtremeObjectFromHttpSession("Layers");
                ManualSerializer.RestoreMapXtremeObjectFromHttpSession(GetKey("Selection"));

                // Just by setting it to temp variables the objects are serialized into session. There is no need to set them explicitly.
                if (StateManager.IsManualState())
                {
                    ManualSerializer.RestoreMapXtremeObjectFromHttpSession("tempTable");
                    ManualSerializer.RestoreMapXtremeObjectFromHttpSession("tempLayer");
                }
            }
        }

        // Save the state
        public override void SaveState()
        {
            string mapAlias = ParamsDictionary[ActiveMapAliasKey] as string;
            Map map = GetMapObj(mapAlias);
            if (map == null) return;

            SaveZoomCenterState(map);
            //ManualSerializer.SaveMapXtremeObjectIntoHttpSession(map.Layers, "Layers");
            ManualSerializer.SaveMapXtremeObjectIntoHttpSession(MapInfo.Engine.Session.Current.Selections.DefaultSelection, GetKey("Selection"));

            // Needs this because StateManger doens't have proper function to save them.
            // Need to serialize the temp table first since the temp layer is based on it.
            if (StateManager.IsManualState())
            {
                MapInfo.Mapping.FeatureLayer   fLyr = (MapInfo.Mapping.FeatureLayer)map.Layers[Constants.TempLayerAlias];
                MapInfo.Data.Table tbl = (fLyr != null) ? fLyr.Table : null;
                if (fLyr != null)
                {
                    ManualSerializer.SaveMapXtremeObjectIntoHttpSession(tbl, "tempTable");
                    ManualSerializer.SaveMapXtremeObjectIntoHttpSession(fLyr, "tempLayer");
                }
           }
       }


        // This method checks if the mapinfo session got from the pool is dirty or clean
        private bool IsDirtyMapXtremeSession(Map map)
        {
            // Check if the MapXtreme session is dirty
            
            return (MapInfo.Engine.Session.Current.CustomProperties["DirtyFlag"] != null);
        }

        // Check if this is user's first time accessing this page. IF there is a zoom value in the asp.net session then it is not user's first time.
        private bool IsUsersFirstTime()
        {
            return (HttpContext.Current.Session[StateManager.GetKey("Zoom")] == null);
        }
        // When the session is not dirty these values are initial state of the session.
        private void SaveDefaultState(Map map)
        {
            HttpApplicationState application = HttpContext.Current.Application;
            if (application["DefaultZoom"] == null)
            {
                /*
                byte[] bytes;
                byte[] bytes2;

                bytes = ManualSerializer.BinaryStreamFromObject(MapInfo.Engine.Session.Current.Selections.DefaultSelection);
                bytes2 = Arrays.Clone(bytes) as byte[];
                // Store default selection
                application["DefaultSelection"] = bytes2;


                bytes = ManualSerializer.BinaryStreamFromObject(map.Layers);
                bytes2 = Arrays.Clone(bytes) as byte[];  
                // Store default selection
                application["DefaultLayers"] = bytes2;*/

              
                // Store default selection
                application["DefaultSelection"] = ManualSerializer.BinaryStreamFromObject(MapInfo.Engine.Session.Current.Selections.DefaultSelection);
                // Store layers collection
                application["DefaultLayers"]    = ManualSerializer.BinaryStreamFromObject(map.Layers);
                // Store the original zoom and center.
                application["DefaultCenter"]    = map.Center;
                application["DefaultZoom"]      = map.Zoom;
            } 
            // This MapXtreme object should be marked as dirty
            MapInfo.Engine.Session.Current.CustomProperties["DirtyFlag"] = true;
            //HttpContext.Current.Response.Write("<br><br><br><br><br><br><br><br><br><br>SaveDefaultState");
            //HttpContext.Current.Response.End();
        } 
        // When session is dirty but it is first time for user, this will be applied to give users it's initial state
        private void RestoreDefaultState(Map map) 
        {
            HttpApplicationState application = HttpContext.Current.Application;
            // Get the default layers, center, and zoomfrom the Application. Clear Layers first, 
            //this resets the zoom and center which we will set later
            map.Layers.Clear();

            //Just by deserializing the binary stream we reset the MapFactory Deault layers collection
            byte[] bytes = application["DefaultLayers"] as byte[];
            Object obj = ManualSerializer.ObjectFromBinaryStream(bytes);
            //map.Layers.Add(obj as IMapLayer);

            // For default selection
            bytes = application["DefaultSelection"] as byte[]; 
            obj = ManualSerializer.ObjectFromBinaryStream(bytes);
            //MapInfo.Engine.Session.Current.Selections.DefaultSelection = obj;
            
            // For zoom and center
            map.Zoom = (MapInfo.Geometry.Distance)application["DefaultZoom"];
            map.Center = (DPoint)application["DefaultCenter"];
            // TESTONLY
           // HttpContext.Current.Response.Write("<br><br><br><br><br><br><br><br><br><br>RestoreDefaultState");
        }
    }
}
