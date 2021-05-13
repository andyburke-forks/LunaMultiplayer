using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.VesselUtilities;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using System.Collections.Concurrent;


namespace LmpClient.Systems.VesselPartSyncFieldSys
{
    public class VesselPartSyncFieldMessageHandler : SubSystem<VesselPartSyncFieldSystem>, IMessageHandler
    {
        public static double last_update_time;

        public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

        public void HandleMessage(IServerMessageBase msg)
        {
            if (!(msg.Data is VesselPartSyncFieldMsgData msgData)) return;

            // message is from an older time
            if (msgData.GameTime < last_update_time) return;

            last_update_time = msgData.GameTime;

            //We received a msg for our own controlled/updated vessel so ignore it
            if (!VesselCommon.DoVesselChecks(msgData.VesselId))
                return;

            if (!System.VesselPartsSyncs.ContainsKey(msgData.VesselId))
            {
                System.VesselPartsSyncs.TryAdd(msgData.VesselId, new VesselPartSyncFieldQueue());
            }

            if (System.VesselPartsSyncs.TryGetValue(msgData.VesselId, out var queue))
            {
                queue.Enqueue(msgData);
            }
        }
    }
}
