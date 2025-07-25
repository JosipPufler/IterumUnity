using Iterum.DTOs;
using Mirror;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.Campaign
{
    public class NetworkCampaignGrid : NetworkBehaviour
    {
        public CampaignGridLayout Grid;

        [Command(requiresAuthority = false)]
        public void CmdLoadMapJson(string mapJson, NetworkConnectionToClient sender = null)
        {
            var player = sender.identity.GetComponent<CampaignPlayer>();
            if (player == null || !player.isCampaignHost) return;

            MapDto dto = JsonConvert.DeserializeObject<MapDto>(mapJson);
            Grid.currentMap = dto;

            RpcClearMap();
            Grid.LoadMap(dto);

            RpcSyncMap(dto);
        }

        [ClientRpc]
        private void RpcClearMap()
        {
            foreach (var hex in Grid.grid.Values)
                Destroy(hex);
            Grid.grid.Clear();
            Grid.MapLoaded = false;
        }

        [ClientRpc]
        public void RpcSyncMap(MapDto mapDto)
        {
            Grid.isFlatTopped = mapDto.IsFlatTopped;
            Grid.gridSize.x = mapDto.MaxX;
            Grid.gridSize.y = mapDto.MaxY;

            /*foreach (var hex in mapDto.Hexes)
            {
                Grid.TryAddHex(new GridCoordinate(hex.X, hex.Y, hex.Z));
            }*/
        }

        [Server]
        public void SendMapToClient(NetworkConnectionToClient conn)
        {
            TargetSyncMap(conn, Grid.currentMap);
        }

        [TargetRpc]
        private void TargetSyncMap(NetworkConnection _, MapDto dto)
        {
            RpcClearMap();
            //Grid.LoadMap(dto);
        }
    }
}
