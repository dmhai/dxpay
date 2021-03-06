﻿using System.Collections.Generic;
using JmPay.PayChannelMonitor.MessageSendCollection;
using JmPay.PayChannelMonitor.Models;

namespace JmPay.PayChannelMonitor.SenderFilter
{
    /// <summary>
    /// 应用监控[无订单监控]短信发送者筛选器
    /// </summary>
    public class ForAppPaySuccessRatioFilter : ISenderFilter
    {
        private readonly List<AppPaySuccessRatioSinceLatest> _noOrderApps;

        /// <summary>
        /// 允许发送的接收者集合
        /// </summary>
        private List<AppPaySuccessRatioSinceLatest> _allowList;

        public ForAppPaySuccessRatioFilter(List<AppPaySuccessRatioSinceLatest> noOrderApps)
        {
            _noOrderApps = noOrderApps;
        }

        public void DoFilter()
        {
            if (_noOrderApps == null || _noOrderApps.Count <= 0)
            {
                return ;
            }
            _allowList = new List<AppPaySuccessRatioSinceLatest>();
            foreach (var item in _noOrderApps)
            {
                var sender = new NoOrderAppMessageSender
                {
                    Id = item.AppId,
                    Name = item.AppName,
                    MonitorTypeId = item.MonitorType,
                    MonitorTypeDispay = ""
                };
                var allowSend = NoOrderSenderSingleton.Instance.GetCollection.AllowSend(sender);
                if (allowSend)
                {
                    _allowList.Add(item);
                    NoOrderSenderSingleton.Instance.GetCollection.AddOrUpdate(sender);
                }
            }
        }
        /// <summary>
        /// 允许发送的接收者集合
        /// </summary>
        public List<AppPaySuccessRatioSinceLatest> AllowList { get { return _allowList; } }
    }
}
