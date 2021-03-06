﻿namespace TOOL.Extensions
{
    public static class StateExtension
    {
        /// <summary>
        /// 转换用户状态为文字
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string ConvertAppUserStateToString(this int state)
        {
            return state == 0 ? "冻结" : "正常";
        }

        /// <summary>
        /// 转换状态为文字
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string ConvertStateToString(this int state)
        {
            return state == 0 ? "冻结" : "正常";
        }

        /// <summary>
        /// 转换用户审核状态为文字
        /// </summary>
        /// <param name="auditState"></param>
        /// <returns></returns>
        public static string ConvertAppUserAuditStateToString(this int auditState)
        {
            switch (auditState)
            {
                case -1:
                    return "未通过";
                case 0:
                    return "等待审核";
                case 1:
                    return "审核通过";
            }
            return "未知";
        }

        /// <summary>
        /// 转换用户审核状态为文字
        /// </summary>
        /// <param name="auditState"></param>
        /// <returns></returns>
        public static string ConvertAuditStateToString(this int auditState)
        {
            switch (auditState)
            {
                case -1:
                    return "未通过";
                case 0:
                    return "等待审核";
                case 1:
                    return "审核通过";
            }
            return "未知";
        }
        /// <summary>
        /// 日志类别
        /// </summary>
        /// <param name="type">日志类别：1 注册 2 登录 3 操作 4 错误日志 5 数据库错误日志 6归档日志 7 访问日志</param>
        /// <returns></returns>
        public static string ConvertAdminLogTypeTotString(this int type)
        {
            var res = "未定义";
            switch (type)
            {
                case 1:
                    res = "注册";
                    break;
                case 2:
                    res = "登录";
                    break;
                case 3:
                    res = "操作";
                    break;
                case 4:
                    res = "错误";
                    break;
                case 5:
                    res = "数据库";
                    break;
                case 6:
                    res = "归档";
                    break;
                case 7:
                    res = "访问";
                    break;
            }
            return res;
        }

        /// <summary>
        /// 转换应用监控类型[0:支付成功率(支付成功数/总支付数),1:xx分钟内无订单,2:金额成功率(成功支付金额/总支付金额)]
        /// </summary>
        /// <param name="aType">监控类型</param>
        /// <returns></returns>
        public static string ConvertAppMonitorTypeToString(this int aType)
        {
            switch (aType)
            {
                case 0:
                    return "支付成功率";
                case 1:
                    return "无订单监控";
                case 2:
                    return "金额成功率";
            }
            return "未定义";
        }

        /// <summary>
        /// 转换应用监控类型[0:支付成功率(支付成功数/总支付数),1:xx分钟内无订单,2:金额成功率(成功支付金额/总支付金额)]
        /// </summary>
        /// <param name="aType">监控类型</param>
        /// <param name="thresholdValue">阀值</param>
        /// <returns></returns>
        public static string ConvertAppMonitorTypeToString(this int aType,decimal thresholdValue)
        {
            switch (aType)
            {
                case 0:
                    return string.Format("<span class='red'>支付成功率:{0}%</span>",thresholdValue*100);
                case 1:
                    return "无订单监控";
                case 2:
                    return string.Format("<span class='green'>金额成功率:{0}%</span>", thresholdValue * 100);
            }
            return "未定义";
        }

        /// <summary>
        /// 转换应用监控类型[0:支付成功率(支付成功数/总支付数),1:xx分钟内无订单,2:金额成功率(成功支付金额/总支付金额)]
        /// </summary>
        /// <param name="aType">监控类型</param>
        /// <param name="thresholdValue">阀值</param>
        /// <returns></returns>
        public static string ConvertMonitorChannelTypeToString(this int aType, decimal thresholdValue)
        {
            switch (aType)
            {
                case 20:
                    return "无订单监控";
            }
            return "未定义";
        }
    }
}